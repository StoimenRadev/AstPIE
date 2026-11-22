using System;
using System.Collections.Generic;
using System.Globalization;
using Godot;

public partial class NewtonsFirstLaw : Node2D
{
	public const string Version = "1.0";

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
	private const int MaxStepsPerFrame = 12;

	// Colours from the AstPIE palette
	private static readonly Color Cream = new Color("F2E8D5");

	// Panel nodes
	private Control panel;
	private Control content;
	private Button hideButton;
	private ScrollContainer scroll;
	private Control inputs;
	private LineEdit massBox;
	private LineEdit positionXBox;
	private LineEdit positionYBox;
	private LineEdit velocityXBox;
	private LineEdit velocityYBox;
	private VBoxContainer forceList;
	private Button addForceButton;
	private LineEdit timeStepBox;
	private LineEdit totalTimeBox;
	private Button startButton;
	private Button pauseButton;
	private Label messageLabel;
	private Label timeValue;
	private Label positionValue;
	private Label velocityValue;
	private Label accelerationValue;
	private Label netForceValue;

	// World nodes
	private Sprite2D box;
	private Camera2D camera;

	// Inputs
	private double mass;
	private Vector2D startPosition;
	private Vector2D startVelocity;
	private List<Vector2D> forces = new List<Vector2D>();
	private double timeStep;
	private int totalSteps;

	// Simulation
	private Vector2D netForce;
	private Vector2D acceleration;
	private Vector2D position;
	private Vector2D velocity;
	private int stepCount;
	private double timeAccumulator;
	private RunState state = RunState.Setup;

	public override void _Ready()
	{
		panel = GetNode<Control>("%Panel");
		content = GetNode<Control>("%Content");
		hideButton = GetNode<Button>("%HideButton");
		scroll = GetNode<ScrollContainer>("%Scroll");
		inputs = GetNode<Control>("%Inputs");
		massBox = GetNode<LineEdit>("%MassBox");
		positionXBox = GetNode<LineEdit>("%PositionXBox");
		positionYBox = GetNode<LineEdit>("%PositionYBox");
		velocityXBox = GetNode<LineEdit>("%VelocityXBox");
		velocityYBox = GetNode<LineEdit>("%VelocityYBox");
		forceList = GetNode<VBoxContainer>("%ForceList");
		addForceButton = GetNode<Button>("%AddForceButton");
		timeStepBox = GetNode<LineEdit>("%TimeStepBox");
		totalTimeBox = GetNode<LineEdit>("%TotalTimeBox");
		startButton = GetNode<Button>("%StartButton");
		pauseButton = GetNode<Button>("%PauseButton");
		messageLabel = GetNode<Label>("%MessageLabel");
		timeValue = GetNode<Label>("%TimeValue");
		positionValue = GetNode<Label>("%PositionValue");
		velocityValue = GetNode<Label>("%VelocityValue");
		accelerationValue = GetNode<Label>("%AccelerationValue");
		netForceValue = GetNode<Label>("%NetForceValue");
		box = GetNode<Sprite2D>("%Box");
		camera = GetNode<Camera2D>("%Camera");

		GetNode<Label>("%VersionLabel").Text = "Newton's First Law, v" + Version;

		// Two empty forces to begin with, F1 and F2, as in the Octave prototype
		AddForceRow();
		AddForceRow();

		// Move the view so the box is in the middle of the free space right of the panel
		camera.Offset = new Vector2(-190, 0);

		FitPanel();
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
		AddForceRow();
		ShowStartingState();
		UpdateButtons();
	}

	private void AddForceRow()
	{
		if (GetForceRows().Count >= MaxForces)
		{
			return;
		}

		HBoxContainer row = ForceRowScene.Instantiate<HBoxContainer>();
		forceList.AddChild(row);

		LineEdit forceBox = row.GetNode<LineEdit>("XBox");
		Button removeButton = row.GetNode<Button>("RemoveButton");
		forceBox.TextChanged += OnInputChanged;
		removeButton.Pressed += row.QueueFree;
		removeButton.Pressed += OnForceRemoved;

		NumberForces();
	}

	private void OnForceRemoved()
	{
		NumberForces();
		ShowStartingState();
		UpdateButtons();
	}

	// Number the forces F1, F2, F3...
	private void NumberForces()
	{
		List<HBoxContainer> rows = GetForceRows();
		for (int i = 0; i < rows.Count; i++)
		{
			rows[i].GetNode<Label>("NameLabel").Text = "F" + (i + 1);
		}
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
		Vector2D start = ReadVector(positionXBox, positionYBox);
		if (IsNaN(start))
		{
			start = new Vector2D(0, 0);
		}
		startPosition = start;
		position = start;
		velocity = new Vector2D(0, 0);
		acceleration = new Vector2D(0, 0);
		netForce = new Vector2D(0, 0);
		stepCount = 0;
		timeAccumulator = 0;
	}

	// The values can only be changed before Start or after Reset
	private void UpdateButtons()
	{
		bool editing = state == RunState.Setup;

		massBox.Editable = editing;
		positionXBox.Editable = editing;
		velocityXBox.Editable = editing;
		timeStepBox.Editable = editing;
		totalTimeBox.Editable = editing;

		foreach (HBoxContainer row in GetForceRows())
		{
			row.GetNode<LineEdit>("XBox").Editable = editing;
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
		Vector2D newPosition = ReadVector(positionXBox, positionYBox);
		Vector2D newVelocity = ReadVector(velocityXBox, velocityYBox);
		double newTimeStep = ReadNumber(timeStepBox);
		double newTotalTime = ReadNumber(totalTimeBox);

		List<Vector2D> newForces = new List<Vector2D>();
		bool forcesAreNumbers = true;
		foreach (HBoxContainer row in GetForceRows())
		{
			Vector2D force = ReadVector(row.GetNode<LineEdit>("XBox"), row.GetNode<LineEdit>("YBox"));
			if (IsNaN(force))
			{
				forcesAreNumbers = false;
			}
			newForces.Add(force);
		}

		if (double.IsNaN(newMass) || IsNaN(newPosition) || IsNaN(newVelocity) || double.IsNaN(newTimeStep) || double.IsNaN(newTotalTime) || !forcesAreNumbers)
		{
			ShowMessage("Every box must contain a number.");
			return false;
		}
		if (newMass <= 0 || newTimeStep <= 0 || newTotalTime <= 0)
		{
			ShowMessage("Mass, time step and total time must be more than 0.");
			return false;
		}

		mass = newMass;
		startPosition = newPosition;
		startVelocity = newVelocity;
		forces = newForces;
		timeStep = newTimeStep;
		totalSteps = (int)Math.Floor(newTotalTime / newTimeStep + 1e-9);
		ShowMessage("");
		return true;
	}

	// Returns the number in the box, or NaN if it is not a number. Accepts both 0.5 and 0,5.
	private double ReadNumber(LineEdit inputBox)
	{
		string text = inputBox.Text.Trim().Replace(',', '.');
		double value;
		if (double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value) && double.IsFinite(value))
		{
			return value;
		}
		return double.NaN;
	}

	private Vector2D ReadVector(LineEdit xBox, LineEdit yBox)
	{
		return new Vector2D(ReadNumber(xBox), ReadNumber(yBox));
	}

	private static bool IsNaN(Vector2D vector)
	{
		return double.IsNaN(vector.X) || double.IsNaN(vector.Y);
	}

	// Shows an error message under the buttons. An empty message hides the line.
	private void ShowMessage(string text)
	{
		messageLabel.Text = text;
		messageLabel.Visible = text != "";
		panel.ResetSize();
	}

	// The inputs scroll when the panel would not fit on the screen
	private void FitPanel()
	{
		float otherHeight = panel.GetCombinedMinimumSize().Y - scroll.GetCombinedMinimumSize().Y;
		float maxHeight = GetViewportRect().Size.Y - 32 - otherHeight;
		float inputsHeight = inputs.GetCombinedMinimumSize().Y;
		scroll.CustomMinimumSize = new Vector2(0, Math.Max(0, Math.Min(inputsHeight, maxHeight)));
		panel.ResetSize();
	}

	// Physics
	private void Restart()
	{
		// Net force and acceleration (constant during the whole run)
		netForce = new Vector2D(0, 0);
		foreach (Vector2D force in forces)
		{
			netForce = netForce + force;
		}
		acceleration = netForce / mass;

		position = startPosition;
		velocity = startVelocity;
		stepCount = 0;
		timeAccumulator = 0;
	}

	public override void _Process(double delta)
	{
		if (state == RunState.Running)
		{
			// The physics always moves in steps of exactly dt, no matter how fast the screen updates.
			// At most 12 steps a frame: on a slow device the simulation runs slower instead of freezing.
			timeAccumulator += delta;
			int steps = 0;
			while (timeAccumulator >= timeStep && stepCount < totalSteps && steps < MaxStepsPerFrame)
			{
				Step();
				timeAccumulator -= timeStep;
				steps++;
			}
			if (timeAccumulator >= timeStep)
			{
				timeAccumulator = 0;
			}

			if (stepCount >= totalSteps)
			{
				state = RunState.Finished;
				timeAccumulator = 0;
				UpdateButtons();
			}
		}

		// The screen updates at a different rate than the physics steps, so between two steps the box
		// is drawn where it is at this moment: x + v*t + 0.5*a*t^2, with t the time since the last step.
		// This only changes the picture, not the physics.
		double t = timeAccumulator;
		Vector2D shownPosition = position + velocity * t + acceleration * (0.5 * t * t);

		// Move the box and the camera (up is negative y on the screen), and show the results
		box.Position = new Vector2((float)shownPosition.X * PixelsPerMeter, -BoxSize / 2 - (float)shownPosition.Y * PixelsPerMeter);
		camera.Position = new Vector2(box.Position.X, -200);
		FitPanel();
		UpdateResults();
		QueueRedraw();
	}

	// One Velocity Verlet step. The forces are constant, so the acceleration does not change
	// and the new velocity is v + a*dt.
	private void Step()
	{
		position = position + velocity * timeStep + acceleration * (0.5 * timeStep * timeStep);
		velocity = velocity + acceleration * timeStep;
		stepCount++;
	}

	private void UpdateResults()
	{
		timeValue.Text = Show(stepCount * timeStep);
		positionValue.Text = Show(position);
		velocityValue.Text = Show(velocity);
		accelerationValue.Text = Show(acceleration);
		netForceValue.Text = Show(netForce);
	}

	// A number rounded to 3 decimals, always written as 0.5 and not 0,5
	private static string Show(double value)
	{
		return Math.Round(value, 3).ToString(CultureInfo.InvariantCulture);
	}

	// A vector as (x, y), rounded to 3 decimals
	private static string Show(Vector2D vector)
	{
		return "(" + Show(vector.X) + ", " + Show(vector.Y) + ")";
	}

	// Drawing (in world pixels: x to the right, the ground is at y = 0, up is negative y)
	public override void _Draw()
	{
		DrawDistanceMarks();
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
}
