using System;
using System.Collections.Generic;
using System.Globalization;
using Godot;

public partial class NewtonsSecondLaw : Node2D
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
	private const int MaxForces = 10;

	// Box A (orange) is smaller and in front, box B (red) is bigger and behind it
	private const float BoxASize = 80;
	private const float BoxBSize = 100;

	// Colours from the AstPIE palette
	private static readonly Color Cream = new Color("F2E8D5");
	private static readonly Color Navy = new Color("1A2033");
	private static readonly Color Teal = new Color("4F8A8B");

	// Panel nodes
	private Control panel;
	private ScrollContainer scroll;
	private Control content;
	private Control resultsSection;
	private Button hideButton;
	private LineEdit positionBox;
	private LineEdit velocityBox;
	private LineEdit massABox;
	private LineEdit massBBox;
	private VBoxContainer forceListA;
	private VBoxContainer forceListB;
	private Button addForceAButton;
	private Button addForceBButton;
	private LineEdit timeStepBox;
	private LineEdit totalTimeBox;
	private Button startButton;
	private Button pauseButton;
	private Label messageLabel;
	private Label netForceAValue;
	private Label netForceBValue;
	private Label accelerationAValue;
	private Label accelerationBValue;
	private Label velocityAValue;
	private Label velocityBValue;
	private Label distanceAValue;
	private Label distanceBValue;
	private Label timeValue;
	private Label gapValue;

	// World nodes
	private Sprite2D boxA;
	private Sprite2D boxB;
	private Camera2D camera;

	// Inputs
	private double initialPosition;
	private double initialVelocity;
	private double massA;
	private double massB;
	private List<double> forcesA = new List<double>();
	private List<double> forcesB = new List<double>();
	private double timeStep;
	private double totalTime;

	// Simulation
	private double netForceA;
	private double netForceB;
	private double accelerationA;
	private double accelerationB;
	private double positionA;
	private double positionB;
	private double velocityA;
	private double velocityB;
	private int stepCount;
	private int totalSteps;
	private double timeAccumulator;
	private RunState state = RunState.Setup;

	// A faint copy of each box is left behind every second
	private List<double> ghostsA = new List<double>();
	private List<double> ghostsB = new List<double>();
	private double nextGhostTime;

	// Arrow scales: the fastest speed of the run is 200 px long, the largest net force is 120 px long
	private float pixelsPerSpeed;
	private float pixelsPerNewton;

	public override void _Ready()
	{
		// Always write numbers as 0.5 and not 0,5
		CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

		panel = GetNode<Control>("%Panel");
		scroll = GetNode<ScrollContainer>("%Scroll");
		content = GetNode<Control>("%Content");
		resultsSection = GetNode<Control>("%ResultsSection");
		hideButton = GetNode<Button>("%HideButton");
		positionBox = GetNode<LineEdit>("%PositionBox");
		velocityBox = GetNode<LineEdit>("%VelocityBox");
		massABox = GetNode<LineEdit>("%MassABox");
		massBBox = GetNode<LineEdit>("%MassBBox");
		forceListA = GetNode<VBoxContainer>("%ForceListA");
		forceListB = GetNode<VBoxContainer>("%ForceListB");
		addForceAButton = GetNode<Button>("%AddForceAButton");
		addForceBButton = GetNode<Button>("%AddForceBButton");
		timeStepBox = GetNode<LineEdit>("%TimeStepBox");
		totalTimeBox = GetNode<LineEdit>("%TotalTimeBox");
		startButton = GetNode<Button>("%StartButton");
		pauseButton = GetNode<Button>("%PauseButton");
		messageLabel = GetNode<Label>("%MessageLabel");
		netForceAValue = GetNode<Label>("%NetForceAValue");
		netForceBValue = GetNode<Label>("%NetForceBValue");
		accelerationAValue = GetNode<Label>("%AccelerationAValue");
		accelerationBValue = GetNode<Label>("%AccelerationBValue");
		velocityAValue = GetNode<Label>("%VelocityAValue");
		velocityBValue = GetNode<Label>("%VelocityBValue");
		distanceAValue = GetNode<Label>("%DistanceAValue");
		distanceBValue = GetNode<Label>("%DistanceBValue");
		timeValue = GetNode<Label>("%TimeValue");
		gapValue = GetNode<Label>("%GapValue");
		boxA = GetNode<Sprite2D>("%BoxA");
		boxB = GetNode<Sprite2D>("%BoxB");
		camera = GetNode<Camera2D>("%Camera");

		ShowStartingState();
		UpdateButtons();
		FitPanel();
	}

	// Panel
	private void OnHidePressed()
	{
		scroll.Visible = !scroll.Visible;
		resultsSection.Visible = scroll.Visible;

		if (scroll.Visible)
		{
			hideButton.Text = "Hide";
		}
		else
		{
			hideButton.Text = "Show";
		}

		panel.ResetSize();
	}

	// The inputs grow with their content, but the panel never goes past the bottom of the screen:
	// then the inputs scroll, and the title and the results stay visible.
	// Called every frame, so it always matches the rows that are in the panel.
	private void FitPanel()
	{
		float otherHeight = panel.GetCombinedMinimumSize().Y - scroll.GetCombinedMinimumSize().Y;
		float maxHeight = GetViewportRect().Size.Y - 32 - otherHeight;
		float contentHeight = content.GetCombinedMinimumSize().Y;
		scroll.CustomMinimumSize = new Vector2(0, Math.Min(contentHeight, maxHeight));
		panel.ResetSize();
	}

	private void OnInputChanged(string newText)
	{
		if (state == RunState.Setup)
		{
			ShowStartingState();
		}
	}

	private void OnAddForceAPressed()
	{
		AddForceRow(forceListA);
	}

	private void OnAddForceBPressed()
	{
		AddForceRow(forceListB);
	}

	private void AddForceRow(VBoxContainer forceList)
	{
		if (GetForceRows(forceList).Count >= MaxForces)
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
		// Number the forces of each box F1, F2, F3...
		List<HBoxContainer> rowsA = GetForceRows(forceListA);
		for (int i = 0; i < rowsA.Count; i++)
		{
			rowsA[i].GetNode<Label>("NameLabel").Text = "F" + (i + 1);
		}
		List<HBoxContainer> rowsB = GetForceRows(forceListB);
		for (int i = 0; i < rowsB.Count; i++)
		{
			rowsB[i].GetNode<Label>("NameLabel").Text = "F" + (i + 1);
		}

		ShowStartingState();
		UpdateButtons();
	}

	// The force rows that are still in the list (a removed row is only deleted at the end of the frame)
	private List<HBoxContainer> GetForceRows(VBoxContainer forceList)
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

	// Shows the boxes at their starting position with the values typed so far
	private void ShowStartingState()
	{
		if (ReadInputs())
		{
			Restart();
		}
	}

	// The values can only be changed before Start or after Reset
	private void UpdateButtons()
	{
		bool editing = state == RunState.Setup;

		positionBox.Editable = editing;
		velocityBox.Editable = editing;
		massABox.Editable = editing;
		massBBox.Editable = editing;
		timeStepBox.Editable = editing;
		totalTimeBox.Editable = editing;

		SetForceRowsEditable(forceListA, editing);
		SetForceRowsEditable(forceListB, editing);
		addForceAButton.Disabled = !editing || GetForceRows(forceListA).Count >= MaxForces;
		addForceBButton.Disabled = !editing || GetForceRows(forceListB).Count >= MaxForces;

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

	private void SetForceRowsEditable(VBoxContainer forceList, bool editing)
	{
		foreach (HBoxContainer row in GetForceRows(forceList))
		{
			row.GetNode<LineEdit>("ForceBox").Editable = editing;
			row.GetNode<Button>("RemoveButton").Disabled = !editing;
		}
	}

	// Reads the boxes. Returns false and shows a message if a value is wrong.
	private bool ReadInputs()
	{
		double x = ReadNumber(positionBox);
		double v = ReadNumber(velocityBox);
		double newMassA = ReadNumber(massABox);
		double newMassB = ReadNumber(massBBox);
		double newTimeStep = ReadNumber(timeStepBox);
		double newTotalTime = ReadNumber(totalTimeBox);
		List<double> newForcesA = ReadForces(forceListA);
		List<double> newForcesB = ReadForces(forceListB);

		if (double.IsNaN(x) || double.IsNaN(v) || double.IsNaN(newMassA) || double.IsNaN(newMassB) || double.IsNaN(newTimeStep) || double.IsNaN(newTotalTime) || newForcesA.Contains(double.NaN) || newForcesB.Contains(double.NaN))
		{
			ShowMessage("Every box must contain a number.");
			return false;
		}
		if (newMassA <= 0 || newMassB <= 0)
		{
			ShowMessage("Both masses must be positive.");
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

		initialPosition = x;
		initialVelocity = v;
		massA = newMassA;
		massB = newMassB;
		forcesA = newForcesA;
		forcesB = newForcesB;
		timeStep = newTimeStep;
		totalTime = newTotalTime;
		totalSteps = (int)Math.Round(steps);
		ShowMessage("");
		return true;
	}

	// Reads all force boxes of one box. A box that is not a number gives NaN.
	private List<double> ReadForces(VBoxContainer forceList)
	{
		List<double> forces = new List<double>();
		foreach (HBoxContainer row in GetForceRows(forceList))
		{
			forces.Add(ReadNumber(row.GetNode<LineEdit>("ForceBox")));
		}
		return forces;
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
	}

	// Physics
	private void Restart()
	{
		// Net force on each box
		netForceA = 0;
		foreach (double force in forcesA)
		{
			netForceA = netForceA + force;
		}
		netForceB = 0;
		foreach (double force in forcesB)
		{
			netForceB = netForceB + force;
		}

		// Newton's second law: a = F / m (constant during the whole run)
		accelerationA = netForceA / massA;
		accelerationB = netForceB / massB;

		positionA = initialPosition;
		positionB = initialPosition;
		velocityA = initialVelocity;
		velocityB = initialVelocity;
		stepCount = 0;
		timeAccumulator = 0;

		ghostsA.Clear();
		ghostsB.Clear();
		ghostsA.Add(positionA);
		ghostsB.Add(positionB);
		nextGhostTime = 1;

		// Arrow scales. The forces are constant, so the fastest speed is at the start or at the end (v = v0 + a*T).
		double fastestSpeed = Math.Abs(initialVelocity);
		fastestSpeed = Math.Max(fastestSpeed, Math.Abs(initialVelocity + accelerationA * totalTime));
		fastestSpeed = Math.Max(fastestSpeed, Math.Abs(initialVelocity + accelerationB * totalTime));
		pixelsPerSpeed = 0;
		if (fastestSpeed > 0)
		{
			pixelsPerSpeed = 200 / (float)fastestSpeed;
		}

		double largestForce = Math.Max(Math.Abs(netForceA), Math.Abs(netForceB));
		pixelsPerNewton = 0;
		if (largestForce > 0)
		{
			pixelsPerNewton = 120 / (float)largestForce;
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

		// Between two physics steps the boxes are drawn where they are at this moment:
		// x + v*t + 1/2*a*t^2, with t the time since the last step. This only changes the picture.
		double t = timeAccumulator;
		double shownPositionA = positionA + velocityA * t + 0.5 * accelerationA * t * t;
		double shownPositionB = positionB + velocityB * t + 0.5 * accelerationB * t * t;
		boxA.Position = new Vector2((float)shownPositionA * PixelsPerMeter, -BoxASize / 2);
		boxB.Position = new Vector2((float)shownPositionB * PixelsPerMeter, -BoxBSize / 2);

		MoveCamera();
		UpdateResults();
		FitPanel();
		QueueRedraw();
	}

	// One Velocity Verlet step for each box. The forces are constant, so the accelerations do not change.
	private void Step()
	{
		positionA = positionA + velocityA * timeStep + 0.5 * accelerationA * timeStep * timeStep;
		velocityA = velocityA + accelerationA * timeStep;

		positionB = positionB + velocityB * timeStep + 0.5 * accelerationB * timeStep * timeStep;
		velocityB = velocityB + accelerationB * timeStep;

		stepCount++;

		if (stepCount * timeStep >= nextGhostTime - 1e-9 && ghostsA.Count < 500)
		{
			ghostsA.Add(positionA);
			ghostsB.Add(positionB);
			nextGhostTime = nextGhostTime + 1;
		}
	}

	// The camera looks at the middle between the boxes and zooms out when they move apart
	private void MoveCamera()
	{
		float screenWidth = GetViewportRect().Size.X;
		float freeWidth = screenWidth - 300;
		float panelShift = 0;
		if (scroll.Visible)
		{
			freeWidth = freeWidth - 380;
			panelShift = -190;
		}

		float gap = Math.Abs(boxA.Position.X - boxB.Position.X);
		float zoom = 1;
		if (gap > freeWidth)
		{
			zoom = Math.Max(freeWidth / gap, 0.3f);
		}

		camera.Zoom = new Vector2(zoom, zoom);
		camera.Position = new Vector2((boxA.Position.X + boxB.Position.X) / 2, -200);
		camera.Offset = new Vector2(panelShift / zoom, 0);
	}

	private void UpdateResults()
	{
		netForceAValue.Text = Math.Round(netForceA, 2) + " N";
		netForceBValue.Text = Math.Round(netForceB, 2) + " N";
		accelerationAValue.Text = Math.Round(accelerationA, 2) + " m/s²";
		accelerationBValue.Text = Math.Round(accelerationB, 2) + " m/s²";
		velocityAValue.Text = Math.Round(velocityA, 2) + " m/s";
		velocityBValue.Text = Math.Round(velocityB, 2) + " m/s";
		distanceAValue.Text = Math.Round(positionA - initialPosition, 2) + " m";
		distanceBValue.Text = Math.Round(positionB - initialPosition, 2) + " m";
		timeValue.Text = Math.Round(stepCount * timeStep, 2) + " s";
		gapValue.Text = Math.Round(Math.Abs(positionA - positionB), 2) + " m";
	}

	private void PrintResults()
	{
		GD.Print("");
		GD.Print("Simulation results");
		GD.Print("");
		GD.Print("Initial position: " + initialPosition + " m");
		GD.Print("Initial velocity: " + initialVelocity + " m/s");
		GD.Print("");
		GD.Print("Box A: mass " + massA + " kg, net force " + netForceA + " N");
		GD.Print("Box A acceleration: " + accelerationA + " m/s^2");
		GD.Print("Box A final position: " + positionA + " m");
		GD.Print("Box A final velocity: " + velocityA + " m/s");
		GD.Print("");
		GD.Print("Box B: mass " + massB + " kg, net force " + netForceB + " N");
		GD.Print("Box B acceleration: " + accelerationB + " m/s^2");
		GD.Print("Box B final position: " + positionB + " m");
		GD.Print("Box B final velocity: " + velocityB + " m/s");
		GD.Print("");
		GD.Print("Total simulation time: " + totalTime + " s");
		GD.Print("Simulation steps: " + stepCount);
	}

	// Drawing (in world pixels: x to the right, the ground is at y = 0, up is negative y)
	public override void _Draw()
	{
		DrawDistanceMarks();

		// Ghost boxes, one for every second. Box B first, because it is behind box A.
		foreach (double ghost in ghostsB)
		{
			DrawGhost(boxB.Texture, ghost, BoxBSize);
		}
		foreach (double ghost in ghostsA)
		{
			DrawGhost(boxA.Texture, ghost, BoxASize);
		}

		// Arrows above the boxes: velocity and net force of box A, then of box B
		Font font = ThemeDB.FallbackFont;
		float top = -BoxBSize - 20;
		DrawLabeledArrow(boxA.Position.X, top, (float)velocityA * pixelsPerSpeed, Navy, "v A", font);
		DrawLabeledArrow(boxA.Position.X, top - 25, (float)netForceA * pixelsPerNewton, Teal, "F A", font);
		DrawLabeledArrow(boxB.Position.X, top - 50, (float)velocityB * pixelsPerSpeed, Navy, "v B", font);
		DrawLabeledArrow(boxB.Position.X, top - 75, (float)netForceB * pixelsPerNewton, Teal, "F B", font);
	}

	private void DrawGhost(Texture2D texture, double position, float size)
	{
		float x = (float)position * PixelsPerMeter;
		DrawTextureRect(texture, new Rect2(x - size / 2, -size, size, size), false, new Color(1, 1, 1, 0.3f));
	}

	// Small marks on the grass every metre, with a number every 5 metres
	private void DrawDistanceMarks()
	{
		float viewWidth = GetViewportRect().Size.X / camera.Zoom.X;
		float viewCenter = camera.GetScreenCenterPosition().X;
		int firstMeter = (int)Math.Floor((viewCenter - viewWidth) / PixelsPerMeter);
		int lastMeter = (int)Math.Ceiling((viewCenter + viewWidth) / PixelsPerMeter);
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
