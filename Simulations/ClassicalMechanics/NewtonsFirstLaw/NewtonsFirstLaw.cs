using System;
using System.Collections.Generic;
using System.Globalization;
using Godot;

public partial class NewtonsFirstLaw : Node2D
{
	public const string Version = "1.1";

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

	// Limits that keep a browser tab safe: the number of steps in one run, and how far the box can go
	// from the 0 m mark (Godot draws with float, so farther away the picture can't place the box exactly)
	private const int MaxSteps = 100000;
	private const double MaxDistance = 50000;

	// Colours from the AstPIE palette
	private static readonly Color Cream = new Color("F2E8D5");
	private static readonly Color Coral = new Color("D9725B");

	// Panel nodes
	private Control panel;
	private Control content;
	private Button hideButton;
	private ScrollContainer scroll;
	private Control inputs;
	private Label massLabel;
	private LineEdit massBox;
	private Label positionLabel;
	private LineEdit positionXBox;
	private LineEdit positionYBox;
	private Label velocityLabel;
	private LineEdit velocityXBox;
	private LineEdit velocityYBox;
	private VBoxContainer forceList;
	private Button addForceButton;
	private Label timeStepLabel;
	private LineEdit timeStepBox;
	private Label totalTimeLabel;
	private LineEdit totalTimeBox;
	private Button startButton;
	private Button pauseButton;
	private Label messageLabel;
	private Label timeValue;
	private Label positionValue;
	private Label velocityValue;
	private Label accelerationValue;
	private Label netForceValue;
	private Label distanceValue;

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
		massLabel = GetNode<Label>("%MassLabel");
		massBox = GetNode<LineEdit>("%MassBox");
		positionLabel = GetNode<Label>("%PositionLabel");
		positionXBox = GetNode<LineEdit>("%PositionXBox");
		positionYBox = GetNode<LineEdit>("%PositionYBox");
		velocityLabel = GetNode<Label>("%VelocityLabel");
		velocityXBox = GetNode<LineEdit>("%VelocityXBox");
		velocityYBox = GetNode<LineEdit>("%VelocityYBox");
		forceList = GetNode<VBoxContainer>("%ForceList");
		addForceButton = GetNode<Button>("%AddForceButton");
		timeStepLabel = GetNode<Label>("%TimeStepLabel");
		timeStepBox = GetNode<LineEdit>("%TimeStepBox");
		totalTimeLabel = GetNode<Label>("%TotalTimeLabel");
		totalTimeBox = GetNode<LineEdit>("%TotalTimeBox");
		startButton = GetNode<Button>("%StartButton");
		pauseButton = GetNode<Button>("%PauseButton");
		messageLabel = GetNode<Label>("%MessageLabel");
		timeValue = GetNode<Label>("%TimeValue");
		positionValue = GetNode<Label>("%PositionValue");
		velocityValue = GetNode<Label>("%VelocityValue");
		accelerationValue = GetNode<Label>("%AccelerationValue");
		netForceValue = GetNode<Label>("%NetForceValue");
		distanceValue = GetNode<Label>("%DistanceValue");
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

	// Reads the boxes. Returns false, shows what is wrong and marks the wrong box if a value is wrong.
	private bool ReadInputs()
	{
		ClearMarks();

		string emptyBox = FirstEmptyBox();
		if (emptyBox != "")
		{
			ShowMessage("\"" + emptyBox + "\" must contain a number.");
			return false;
		}

		double newMass = ReadNumber(massBox);
		Vector2D newPosition = ReadVector(positionXBox, positionYBox);
		Vector2D newVelocity = ReadVector(velocityXBox, velocityYBox);
		double newTimeStep = ReadNumber(timeStepBox);
		double newTotalTime = ReadNumber(totalTimeBox);

		List<Vector2D> newForces = new List<Vector2D>();
		Vector2D newNetForce = new Vector2D(0, 0);
		foreach (HBoxContainer row in GetForceRows())
		{
			Vector2D force = ReadVector(row.GetNode<LineEdit>("XBox"), row.GetNode<LineEdit>("YBox"));
			newForces.Add(force);
			newNetForce = newNetForce + force;
		}

		if (newMass <= 0)
		{
			MarkWrong(massLabel);
			ShowMessage("The mass must be more than 0 kg.");
			return false;
		}
		if (newTimeStep <= 0)
		{
			MarkWrong(timeStepLabel);
			ShowMessage("The time step dt must be more than 0 s.");
			return false;
		}
		if (newTotalTime <= 0)
		{
			MarkWrong(totalTimeLabel);
			ShowMessage("The total time T must be more than 0 s.");
			return false;
		}
		if (newTimeStep > newTotalTime)
		{
			MarkWrong(timeStepLabel);
			MarkWrong(totalTimeLabel);
			ShowMessage("The time step dt can't be longer than the total time T.");
			return false;
		}

		double steps = Math.Floor(newTotalTime / newTimeStep + 1e-9);
		if (steps > MaxSteps)
		{
			MarkWrong(timeStepLabel);
			MarkWrong(totalTimeLabel);
			ShowMessage("That is more than " + MaxSteps + " steps: make dt bigger or T shorter.");
			return false;
		}

		if (FarthestDistance(newPosition, newVelocity, newNetForce / newMass, newTotalTime) > MaxDistance)
		{
			ShowMessage("The box would go more than 50 km from the 0 m mark. Use a smaller start position, velocity, force or time.");
			return false;
		}

		mass = newMass;
		startPosition = newPosition;
		startVelocity = newVelocity;
		forces = newForces;
		timeStep = newTimeStep;
		totalSteps = (int)steps;
		ShowMessage("");
		return true;
	}

	// The name of the first box (in the order of the panel) that doesn't contain a number, or "" if they
	// all do. The name of that box turns coral.
	private string FirstEmptyBox()
	{
		if (double.IsNaN(ReadNumber(massBox)))
		{
			MarkWrong(massLabel);
			return "Mass";
		}
		if (IsNaN(ReadVector(positionXBox, positionYBox)))
		{
			MarkWrong(positionLabel);
			return "Start position";
		}
		if (IsNaN(ReadVector(velocityXBox, velocityYBox)))
		{
			MarkWrong(velocityLabel);
			return "Start velocity";
		}

		List<HBoxContainer> rows = GetForceRows();
		for (int i = 0; i < rows.Count; i++)
		{
			if (IsNaN(ReadVector(rows[i].GetNode<LineEdit>("XBox"), rows[i].GetNode<LineEdit>("YBox"))))
			{
				MarkWrong(rows[i].GetNode<Label>("NameLabel"));
				return "F" + (i + 1);
			}
		}

		if (double.IsNaN(ReadNumber(timeStepBox)))
		{
			MarkWrong(timeStepLabel);
			return "Time step dt";
		}
		if (double.IsNaN(ReadNumber(totalTimeBox)))
		{
			MarkWrong(totalTimeLabel);
			return "Total time T";
		}
		return "";
	}

	// The farthest the box gets from the 0 m mark during the run: at the start, at the end, or where it
	// turns back. The forces are constant, so x = x0 + v0*t + 0.5*a*t^2.
	private static double FarthestDistance(Vector2D x0, Vector2D v0, Vector2D a, double totalTime)
	{
		double farthest = Math.Max(x0.Length(), PositionAt(x0, v0, a, totalTime).Length());

		if (a.X != 0)
		{
			double turnTime = -v0.X / a.X;
			if (turnTime > 0 && turnTime < totalTime)
			{
				farthest = Math.Max(farthest, PositionAt(x0, v0, a, turnTime).Length());
			}
		}
		return farthest;
	}

	private static Vector2D PositionAt(Vector2D x0, Vector2D v0, Vector2D a, double t)
	{
		return x0 + v0 * t + a * (0.5 * t * t);
	}

	// Turns the name of a wrong box coral
	private static void MarkWrong(Label label)
	{
		label.AddThemeColorOverride("font_color", Coral);
	}

	private void ClearMarks()
	{
		massLabel.RemoveThemeColorOverride("font_color");
		positionLabel.RemoveThemeColorOverride("font_color");
		velocityLabel.RemoveThemeColorOverride("font_color");
		timeStepLabel.RemoveThemeColorOverride("font_color");
		totalTimeLabel.RemoveThemeColorOverride("font_color");

		foreach (HBoxContainer row in GetForceRows())
		{
			row.GetNode<Label>("NameLabel").RemoveThemeColorOverride("font_color");
		}
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
		timeValue.Text = Scalar(stepCount * timeStep, "s");
		positionValue.Text = Vector(position, "m");
		velocityValue.Text = Vector(velocity, "m/s");
		accelerationValue.Text = Vector(acceleration, "m/s²");
		netForceValue.Text = Vector(netForce, "N");
		distanceValue.Text = Scalar((position - startPosition).Length(), "m");
	}

	// Results as the Octave prototype printed them: three decimals, always written as 0.5 and not 0,5
	private static string Number(double value)
	{
		double rounded = Math.Round(value, 3);
		if (rounded == 0)
		{
			rounded = 0; // no "-0.000"
		}
		return rounded.ToString("0.000", CultureInfo.InvariantCulture);
	}

	private static string Scalar(double value, string unit)
	{
		return Number(value) + " " + unit;
	}

	private static string Vector(Vector2D vector, string unit)
	{
		return "(" + Number(vector.X) + ", " + Number(vector.Y) + ") " + unit;
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
