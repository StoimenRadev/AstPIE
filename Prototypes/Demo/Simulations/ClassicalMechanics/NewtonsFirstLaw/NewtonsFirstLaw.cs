using System;
using System.Collections.Generic;
using System.Globalization;
using Godot;

public partial class NewtonsFirstLaw : Node2D
{
	private enum RunState
	{
		Setup,
		Running,
		Paused,
		Finished
	}

	[Export] public PackedScene ForceRowScene;

	private const float PixelsPerMeter = 80;
	private const float BoxSize = 80;
	private const int MaxForces = 100;

	// Colours from the AstPIE palette
	private static readonly Color Cream = new Color("F2E8D5");
	private static readonly Color Navy = new Color("1A2033");
	private static readonly Color Teal = new Color("4F8A8B");
	private static readonly Color Coral = new Color("D9725B");

	// Panel nodes
	private Control panel;
	private Control content;
	private Button hideButton;
	private LineEdit massBox;
	private LineEdit positionBox;
	private LineEdit velocityBox;
	private LineEdit timeStepBox;
	private LineEdit totalTimeBox;
	private ScrollContainer forceScroll;
	private VBoxContainer forceList;
	private Button addForceButton;
	private Button startButton;
	private Button pauseButton;
	private Label messageLabel;
	private Label timeValue;
	private Label positionValue;
	private Label velocityValue;
	private Label accelerationValue;
	private Label netForceValue;
	private Label displacementValue;

	// World nodes
	private Sprite2D box;
	private Camera2D camera;

	// Inputs
	private double mass;
	private double initialPosition;
	private double initialVelocity;
	private List<double> forces = new List<double>();
	private double timeStep;
	private double totalTime;

	// Simulation
	private double netForce;
	private double acceleration;
	private double position;
	private double velocity;
	private int stepCount;
	private int totalSteps;
	private double timeAccumulator;
	private RunState state = RunState.Setup;

	// A faint copy of the box is left behind every second
	private List<double> ghostPositions = new List<double>();
	private double nextGhostTime;

	// Velocity arrow scale, chosen so the fastest speed of the run is 200 px long
	private float pixelsPerSpeed;

	public override void _Ready()
	{
		// Always write numbers as 0.5 and not 0,5
		CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

		panel = GetNode<Control>("%Panel");
		content = GetNode<Control>("%Content");
		hideButton = GetNode<Button>("%HideButton");
		massBox = GetNode<LineEdit>("%MassBox");
		positionBox = GetNode<LineEdit>("%PositionBox");
		velocityBox = GetNode<LineEdit>("%VelocityBox");
		timeStepBox = GetNode<LineEdit>("%TimeStepBox");
		totalTimeBox = GetNode<LineEdit>("%TotalTimeBox");
		forceScroll = GetNode<ScrollContainer>("%ForceScroll");
		forceList = GetNode<VBoxContainer>("%ForceList");
		addForceButton = GetNode<Button>("%AddForceButton");
		startButton = GetNode<Button>("%StartButton");
		pauseButton = GetNode<Button>("%PauseButton");
		messageLabel = GetNode<Label>("%MessageLabel");
		timeValue = GetNode<Label>("%TimeValue");
		positionValue = GetNode<Label>("%PositionValue");
		velocityValue = GetNode<Label>("%VelocityValue");
		accelerationValue = GetNode<Label>("%AccelerationValue");
		netForceValue = GetNode<Label>("%NetForceValue");
		displacementValue = GetNode<Label>("%DisplacementValue");
		box = GetNode<Sprite2D>("%Box");
		camera = GetNode<Camera2D>("%Camera");

		// Move the view so the box is in the middle of the free space right of the panel
		camera.Offset = new Vector2(-190, 0);

		ShowStartingState();
		UpdateButtons();
	}

	// Panel
	private void OnHidePressed()
	{
		content.Visible = !content.Visible;

		if (content.Visible)
		{
			hideButton.Text = "Hide";
			camera.Offset = new Vector2(-190, 0);
		}
		else
		{
			hideButton.Text = "Show";
			camera.Offset = new Vector2(0, 0);
		}

		panel.ResetSize();
	}

	private void OnInputChanged(string newText)
	{
		if (state == RunState.Setup)
		{
			ShowStartingState();
		}
	}

	private void OnAddForcePressed()
	{
		if (GetForceRows().Count >= MaxForces)
		{
			return;
		}

		HBoxContainer row = ForceRowScene.Instantiate<HBoxContainer>();
		forceList.AddChild(row);

		LineEdit forceBox = row.GetNode<LineEdit>("ForceBox");
		Button removeButton = row.GetNode<Button>("RemoveButton");
		forceBox.TextChanged += OnInputChanged;
		removeButton.Pressed += row.QueueFree;
		removeButton.Pressed += OnForceListChanged;

		OnForceListChanged();
	}

	private void OnForceListChanged()
	{
		// Number the forces F1, F2, F3...
		List<HBoxContainer> rows = GetForceRows();
		for (int i = 0; i < rows.Count; i++)
		{
			rows[i].GetNode<Label>("NameLabel").Text = "F" + (i + 1);
		}

		// The list grows up to 3 rows, then it scrolls
		forceScroll.CustomMinimumSize = new Vector2(0, Math.Min(rows.Count, 3) * 34);
		panel.ResetSize();

		ShowStartingState();
		UpdateButtons();
	}

	// The force rows that are still in the list (a removed row is only deleted at the end of the frame)
	private List<HBoxContainer> GetForceRows()
	{
		List<HBoxContainer> rows = new List<HBoxContainer>();
		foreach (Node child in forceList.GetChildren())
		{
			if (!child.IsQueuedForDeletion())
			{
				rows.Add((HBoxContainer)child);
			}
		}
		return rows;
	}

	private void OnStartPressed()
	{
		if (ReadInputs())
		{
			Restart();
			state = RunState.Running;
			UpdateButtons();
		}
	}

	private void OnPausePressed()
	{
		if (state == RunState.Running)
		{
			state = RunState.Paused;
		}
		else if (state == RunState.Paused)
		{
			state = RunState.Running;
		}
		UpdateButtons();
	}

	private void OnResetPressed()
	{
		state = RunState.Setup;
		ShowStartingState();
		UpdateButtons();
	}

	// Shows the box at its starting position with the values typed so far
	private void ShowStartingState()
	{
		if (ReadInputs())
		{
			Restart();
			return;
		}

		// Some other value is still wrong, but the start position can already be shown
		double x = ReadNumber(positionBox);
		if (!double.IsNaN(x))
		{
			initialPosition = x;
			position = x;
			ghostPositions.Clear();
		}
	}

	// The values can only be changed before Start or after Reset
	private void UpdateButtons()
	{
		bool editing = state == RunState.Setup;

		massBox.Editable = editing;
		positionBox.Editable = editing;
		velocityBox.Editable = editing;
		timeStepBox.Editable = editing;
		totalTimeBox.Editable = editing;

		foreach (HBoxContainer row in GetForceRows())
		{
			row.GetNode<LineEdit>("ForceBox").Editable = editing;
			row.GetNode<Button>("RemoveButton").Disabled = !editing;
		}

		addForceButton.Disabled = !editing || GetForceRows().Count >= MaxForces;
		startButton.Disabled = !editing;
		pauseButton.Disabled = state == RunState.Setup || state == RunState.Finished;

		if (state == RunState.Paused)
		{
			pauseButton.Text = "Resume";
		}
		else
		{
			pauseButton.Text = "Pause";
		}
	}

	// Reads the boxes. Returns false and shows a message if a value is wrong.
	private bool ReadInputs()
	{
		double newMass = ReadNumber(massBox);
		double x = ReadNumber(positionBox);
		double v = ReadNumber(velocityBox);
		double newTimeStep = ReadNumber(timeStepBox);
		double newTotalTime = ReadNumber(totalTimeBox);

		List<double> newForces = new List<double>();
		bool forcesAreNumbers = true;
		foreach (HBoxContainer row in GetForceRows())
		{
			double force = ReadNumber(row.GetNode<LineEdit>("ForceBox"));
			if (double.IsNaN(force))
			{
				forcesAreNumbers = false;
			}
			newForces.Add(force);
		}

		if (double.IsNaN(newMass) || double.IsNaN(x) || double.IsNaN(v) || double.IsNaN(newTimeStep) || double.IsNaN(newTotalTime) || !forcesAreNumbers)
		{
			ShowMessage("Every box must contain a number.");
			return false;
		}
		if (newMass <= 0)
		{
			ShowMessage("Mass must be positive.");
			return false;
		}
		if (newTimeStep <= 0 || newTotalTime <= 0)
		{
			ShowMessage("Time step and total time must be positive.");
			return false;
		}

		double steps = newTotalTime / newTimeStep;
		if (Math.Abs(steps - Math.Round(steps)) > 1e-9)
		{
			ShowMessage("Total time must be a whole number of time steps.");
			return false;
		}

		mass = newMass;
		initialPosition = x;
		initialVelocity = v;
		forces = newForces;
		timeStep = newTimeStep;
		totalTime = newTotalTime;
		totalSteps = (int)Math.Round(steps);
		ShowMessage("");
		return true;
	}

	// Returns the number in the box, or NaN if it is not a number. Accepts both 0.5 and 0,5.
	private double ReadNumber(LineEdit inputBox)
	{
		string text = inputBox.Text.Trim().Replace(',', '.');
		double value;
		if (double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value))
		{
			return value;
		}
		return double.NaN;
	}

	// Shows an error message under the buttons. An empty message hides the line.
	private void ShowMessage(string text)
	{
		messageLabel.Text = text;
		messageLabel.Visible = text != "";
		panel.ResetSize();
	}

	// Physics
	private void Restart()
	{
		// Net force and acceleration (constant during the whole run)
		netForce = 0;
		foreach (double force in forces)
		{
			netForce = netForce + force;
		}
		acceleration = netForce / mass;

		position = initialPosition;
		velocity = initialVelocity;
		stepCount = 0;
		timeAccumulator = 0;

		ghostPositions.Clear();
		ghostPositions.Add(position);
		nextGhostTime = 1;

		// The forces are constant, so the fastest speed is at the start or at the end (v = v0 + a*T)
		double finalVelocity = initialVelocity + acceleration * totalTime;
		double fastestSpeed = Math.Max(Math.Abs(initialVelocity), Math.Abs(finalVelocity));
		pixelsPerSpeed = 0;
		if (fastestSpeed > 0)
		{
			pixelsPerSpeed = 200 / (float)fastestSpeed;
		}
	}

	public override void _Process(double delta)
	{
		if (state == RunState.Running)
		{
			// The physics always moves in steps of exactly dt, no matter how fast the screen updates
			timeAccumulator += delta;
			while (timeAccumulator >= timeStep && stepCount < totalSteps)
			{
				Step();
				timeAccumulator -= timeStep;
			}

			if (stepCount == totalSteps)
			{
				state = RunState.Finished;
				timeAccumulator = 0;
				UpdateButtons();
				PrintResults();
			}
		}

		// The screen updates at a different rate than the physics steps, so between two steps the box
		// is drawn where it is at this moment: x + v*t + 1/2*a*t^2, with t the time since the last step.
		// This only changes the picture, not the physics.
		double t = timeAccumulator;
		double shownPosition = position + velocity * t + 0.5 * acceleration * t * t;

		// Move the box and the camera, and show the results
		box.Position = new Vector2((float)shownPosition * PixelsPerMeter, -BoxSize / 2);
		camera.Position = new Vector2(box.Position.X, -200);
		UpdateResults();
		QueueRedraw();
	}

	// One Velocity Verlet step. The forces are constant, so the acceleration does not change.
	private void Step()
	{
		position = position + velocity * timeStep + 0.5 * acceleration * timeStep * timeStep;
		velocity = velocity + acceleration * timeStep;
		stepCount++;

		if (stepCount * timeStep >= nextGhostTime - 1e-9 && ghostPositions.Count < 500)
		{
			ghostPositions.Add(position);
			nextGhostTime = nextGhostTime + 1;
		}
	}

	private void UpdateResults()
	{
		timeValue.Text = Math.Round(stepCount * timeStep, 2) + " s";
		positionValue.Text = Math.Round(position, 2) + " m";
		velocityValue.Text = Math.Round(velocity, 2) + " m/s";
		accelerationValue.Text = Math.Round(acceleration, 2) + " m/s²";
		netForceValue.Text = Math.Round(netForce, 2) + " N";
		displacementValue.Text = Math.Round(position - initialPosition, 2) + " m";
	}

	private void PrintResults()
	{
		GD.Print("");
		GD.Print("Simulation results");
		GD.Print("");
		GD.Print("Initial position: " + initialPosition + " m");
		GD.Print("Initial velocity: " + initialVelocity + " m/s");
		GD.Print("");
		for (int i = 0; i < forces.Count; i++)
		{
			GD.Print("Force " + (i + 1) + ": " + forces[i] + " N");
		}
		GD.Print("Net force: " + netForce + " N");
		GD.Print("Acceleration: " + acceleration + " m/s^2");
		GD.Print("");
		GD.Print("Final position: " + position + " m");
		GD.Print("Final velocity: " + velocity + " m/s");
		GD.Print("Distance moved: " + (position - initialPosition) + " m");
		GD.Print("");
		GD.Print("Total simulation time: " + totalTime + " s");
		GD.Print("Simulation steps: " + stepCount);
	}

	// Drawing (in world pixels: x to the right, the ground is at y = 0, up is negative y)
	public override void _Draw()
	{
		DrawDistanceMarks();

		// Ghost boxes, one for every second
		foreach (double ghost in ghostPositions)
		{
			float x = (float)ghost * PixelsPerMeter;
			DrawTextureRect(box.Texture, new Rect2(x - BoxSize / 2, -BoxSize, BoxSize, BoxSize), false, new Color(1, 1, 1, 0.3f));
		}

		// Arrows above the box: velocity, net force, then every force
		float boxX = box.Position.X;
		Font font = ThemeDB.FallbackFont;

		double largestForce = Math.Abs(netForce);
		foreach (double force in forces)
		{
			largestForce = Math.Max(largestForce, Math.Abs(force));
		}
		float pixelsPerNewton = 0;
		if (largestForce > 0)
		{
			pixelsPerNewton = 120 / (float)largestForce; // the largest force arrow is 120 px long
		}

		DrawLabeledArrow(boxX, -BoxSize - 20, (float)velocity * pixelsPerSpeed, Navy, "v", font);
		DrawLabeledArrow(boxX, -BoxSize - 45, (float)netForce * pixelsPerNewton, Teal, "Fnet", font);
		for (int i = 0; i < forces.Count; i++)
		{
			DrawLabeledArrow(boxX, -BoxSize - 70 - 25 * i, (float)forces[i] * pixelsPerNewton, Coral, "F" + (i + 1), font);
		}
	}

	// Small marks on the grass every metre, with a number every 5 metres
	private void DrawDistanceMarks()
	{
		float screenWidth = GetViewportRect().Size.X;
		float viewCenter = camera.GetScreenCenterPosition().X;
		int firstMeter = (int)Math.Floor((viewCenter - screenWidth) / PixelsPerMeter);
		int lastMeter = (int)Math.Ceiling((viewCenter + screenWidth) / PixelsPerMeter);
		Font font = ThemeDB.FallbackFont;

		for (int meter = firstMeter; meter <= lastMeter; meter++)
		{
			float x = meter * PixelsPerMeter;
			if (meter % 5 == 0)
			{
				DrawLine(new Vector2(x, 0), new Vector2(x, 22), Cream, 2);
				DrawString(font, new Vector2(x + 4, 40), meter + " m", HorizontalAlignment.Left, -1, 14, Cream);
			}
			else
			{
				DrawLine(new Vector2(x, 0), new Vector2(x, 10), Cream, 1);
			}
		}
	}

	// Horizontal arrow starting at (startX, y). A positive length points right.
	private void DrawLabeledArrow(float startX, float y, float length, Color color, string label, Font font)
	{
		if (Math.Abs(length) < 1)
		{
			return;
		}

		Vector2 start = new Vector2(startX, y);
		Vector2 end = new Vector2(startX + length, y);
		float direction = Math.Sign(length);

		DrawLine(start, end, color, 3);
		DrawColoredPolygon(new Vector2[] { end + new Vector2(direction * 4, 0), end + new Vector2(-direction * 8, -6), end + new Vector2(-direction * 8, 6) }, color);

		// The label goes just after the arrow tip
		Vector2 labelPosition = end + new Vector2(direction * 8, 5);
		if (direction < 0)
		{
			labelPosition.X = labelPosition.X - font.GetStringSize(label, HorizontalAlignment.Left, -1, 14).X;
		}
		DrawString(font, labelPosition, label, HorizontalAlignment.Left, -1, 14, color);
	}
}
