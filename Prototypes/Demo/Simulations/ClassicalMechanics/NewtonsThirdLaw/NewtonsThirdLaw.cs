using System;
using System.Collections.Generic;
using System.Globalization;
using Godot;

public partial class NewtonsThirdLaw : Node2D
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
	private const float BoxSize = 80;          // pixels; the boxes are 1 m wide
	private const double WallDistance = 7;     // metres from the contact point to each wall
	private const int MaxForces = 10;

	// Colours from the AstPIE palette
	private static readonly Color Cream = new Color("F2E8D5");
	private static readonly Color Navy = new Color("1A2033");
	private static readonly Color Teal = new Color("4F8A8B");
	private static readonly Color Coral = new Color("D9725B");

	// Panel nodes
	private Control panel;
	private ScrollContainer scroll;
	private Control content;
	private Control resultsSection;
	private Button hideButton;
	private LineEdit positionBox;
	private LineEdit velocityBox;
	private LineEdit pushBox;
	private LineEdit pushTimeBox;
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
	private Label pushAValue;
	private Label pushBValue;
	private Label hitAValue;
	private Label hitBValue;
	private Label otherAValue;
	private Label otherBValue;
	private Label netForceAValue;
	private Label netForceBValue;
	private Label accelerationAValue;
	private Label accelerationBValue;
	private Label velocityAValue;
	private Label velocityBValue;
	private Label positionAValue;
	private Label positionBValue;
	private Label pairValue;
	private Label momentumValue;
	private Label energyValue;
	private Label bouncesValue;
	private Label timeValue;

	// World nodes
	private Sprite2D boxA;
	private Sprite2D boxB;
	private Camera2D camera;

	// Inputs
	private double contactPoint;
	private double initialVelocity;
	private double push;
	private double pushTime;
	private double massA;
	private double massB;
	private List<double> otherForcesA = new List<double>();
	private List<double> otherForcesB = new List<double>();
	private double timeStep;
	private double totalTime;

	// Walls (metres)
	private double leftWall;
	private double rightWall;

	// Forces right now
	private double pushOnA;
	private double pushOnB;
	private double otherForceA;
	private double otherForceB;
	private double netForceA;
	private double netForceB;

	// Simulation (positions are the box centres, in metres)
	private double accelerationA;
	private double accelerationB;
	private double positionA;
	private double positionB;
	private double velocityA;
	private double velocityB;

	// Positions one step ago, so the boxes can be drawn between the last two steps
	private double previousPositionA;
	private double previousPositionB;

	// The last time the boxes hit each other: the kick (impulse, N*s) box A got, and how long
	// the push arrows are still shown. Box B always gets the same kick the other way.
	private double lastKickOnA;
	private double hitArrowTime;
	private int stepCount;
	private int totalSteps;
	private double timeAccumulator;
	private int wallBounces;
	private int boxBounces;
	private RunState state = RunState.Setup;

	// Arrow scales: the fastest speed is about 200 px long, the largest force is 120 px long
	private float pixelsPerSpeed;
	private float pixelsPerNewton;

	// Right edge of the panel when it is open (screen pixels), so the stage keeps its size when the panel is hidden
	private float openPanelRight = 376;

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
		pushBox = GetNode<LineEdit>("%PushBox");
		pushTimeBox = GetNode<LineEdit>("%PushTimeBox");
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
		pushAValue = GetNode<Label>("%PushAValue");
		pushBValue = GetNode<Label>("%PushBValue");
		hitAValue = GetNode<Label>("%HitAValue");
		hitBValue = GetNode<Label>("%HitBValue");
		otherAValue = GetNode<Label>("%OtherAValue");
		otherBValue = GetNode<Label>("%OtherBValue");
		netForceAValue = GetNode<Label>("%NetForceAValue");
		netForceBValue = GetNode<Label>("%NetForceBValue");
		accelerationAValue = GetNode<Label>("%AccelerationAValue");
		accelerationBValue = GetNode<Label>("%AccelerationBValue");
		velocityAValue = GetNode<Label>("%VelocityAValue");
		velocityBValue = GetNode<Label>("%VelocityBValue");
		positionAValue = GetNode<Label>("%PositionAValue");
		positionBValue = GetNode<Label>("%PositionBValue");
		pairValue = GetNode<Label>("%PairValue");
		momentumValue = GetNode<Label>("%MomentumValue");
		energyValue = GetNode<Label>("%EnergyValue");
		bouncesValue = GetNode<Label>("%BouncesValue");
		timeValue = GetNode<Label>("%TimeValue");
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
		// Number the other forces of each box F1, F2, F3...
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
			return;
		}

		// Some other value is still wrong, but the start position can already be shown
		double x = ReadNumber(positionBox);
		if (!double.IsNaN(x))
		{
			contactPoint = x;
			leftWall = x - WallDistance;
			rightWall = x + WallDistance;
			positionA = x - 0.5;
			positionB = x + 0.5;
			previousPositionA = positionA;
			previousPositionB = positionB;
		}
	}

	// The values can only be changed before Start or after Reset
	private void UpdateButtons()
	{
		bool editing = state == RunState.Setup;

		positionBox.Editable = editing;
		velocityBox.Editable = editing;
		pushBox.Editable = editing;
		pushTimeBox.Editable = editing;
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
		double newPush = ReadNumber(pushBox);
		double newPushTime = ReadNumber(pushTimeBox);
		double newMassA = ReadNumber(massABox);
		double newMassB = ReadNumber(massBBox);
		double newTimeStep = ReadNumber(timeStepBox);
		double newTotalTime = ReadNumber(totalTimeBox);
		List<double> newForcesA = ReadForces(forceListA);
		List<double> newForcesB = ReadForces(forceListB);

		if (double.IsNaN(x) || double.IsNaN(v) || double.IsNaN(newPush) || double.IsNaN(newPushTime) || double.IsNaN(newMassA) || double.IsNaN(newMassB) || double.IsNaN(newTimeStep) || double.IsNaN(newTotalTime) || newForcesA.Contains(double.NaN) || newForcesB.Contains(double.NaN))
		{
			ShowMessage("Every box must contain a number.");
			return false;
		}
		if (newPush < 0 || newPushTime < 0)
		{
			ShowMessage("The push force and the push time cannot be negative.");
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
		double pushSteps = newPushTime / newTimeStep;
		if (Math.Abs(steps - Math.Round(steps)) > 1e-9 || Math.Abs(pushSteps - Math.Round(pushSteps)) > 1e-9)
		{
			ShowMessage("Total time and push time must be whole numbers of time steps.");
			return false;
		}

		contactPoint = x;
		initialVelocity = v;
		push = newPush;
		pushTime = newPushTime;
		massA = newMassA;
		massB = newMassB;
		otherForcesA = newForcesA;
		otherForcesB = newForcesB;
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
		// Other forces on each box
		otherForceA = 0;
		foreach (double force in otherForcesA)
		{
			otherForceA = otherForceA + force;
		}
		otherForceB = 0;
		foreach (double force in otherForcesB)
		{
			otherForceB = otherForceB + force;
		}

		// The walls and the boxes (1 m wide, touching at the contact point)
		leftWall = contactPoint - WallDistance;
		rightWall = contactPoint + WallDistance;
		positionA = contactPoint - 0.5;
		positionB = contactPoint + 0.5;
		velocityA = initialVelocity;
		velocityB = initialVelocity;
		previousPositionA = positionA;
		previousPositionB = positionB;
		stepCount = 0;
		timeAccumulator = 0;
		wallBounces = 0;
		boxBounces = 0;
		lastKickOnA = 0;
		hitArrowTime = 0;
		UpdateForces();

		// Arrow scales. Without friction the kinetic energy after the push stays the same,
		// so no box can be faster than sqrt(2 * energy / smallest mass).
		double speedA = initialVelocity - push * pushTime / massA;
		double speedB = initialVelocity + push * pushTime / massB;
		double energy = 0.5 * massA * speedA * speedA + 0.5 * massB * speedB * speedB;
		double fastestSpeed = Math.Max(Math.Abs(initialVelocity), Math.Sqrt(2 * energy / Math.Min(massA, massB)));
		pixelsPerSpeed = 0;
		if (fastestSpeed > 0)
		{
			pixelsPerSpeed = 200 / (float)fastestSpeed;
		}

		double largestForce = Math.Max(push, Math.Max(Math.Abs(push - otherForceA), Math.Abs(push + otherForceB)));
		largestForce = Math.Max(largestForce, Math.Max(Math.Abs(otherForceA), Math.Abs(otherForceB)));
		pixelsPerNewton = 0;
		if (largestForce > 0)
		{
			pixelsPerNewton = 120 / (float)largestForce;
		}
	}

	// Newton's third law: while they push, box B pushes box A to the left and box A pushes box B
	// to the right, with forces of the same size: F on A = -F on B. After the push time they glide.
	private void UpdateForces()
	{
		if (stepCount * timeStep < pushTime - 1e-9)
		{
			pushOnA = -push;
			pushOnB = push;
		}
		else
		{
			pushOnA = 0;
			pushOnB = 0;
		}

		// Net force and Newton's second law: a = F / m
		netForceA = pushOnA + otherForceA;
		netForceB = pushOnB + otherForceB;
		accelerationA = netForceA / massA;
		accelerationB = netForceB / massB;
	}

	public override void _Process(double delta)
	{
		if (state == RunState.Running)
		{
			// The physics always moves in steps of exactly dt, no matter how fast the screen updates
			timeAccumulator += delta;
			hitArrowTime = hitArrowTime - delta;
			while (timeAccumulator >= timeStep && stepCount < totalSteps)
			{
				Step();
				timeAccumulator -= timeStep;
			}

			if (stepCount == totalSteps)
			{
				state = RunState.Finished;
				timeAccumulator = 0;
				previousPositionA = positionA;
				previousPositionB = positionB;
				UpdateButtons();
				PrintResults();
			}
		}

		// The screen updates at a different rate than the physics, so the boxes are drawn between
		// the last two physics steps. Both of those positions are valid (never inside a wall or the
		// other box), so every point between them is valid too. This only changes the picture.
		// (Before a time step is entered, dt is 0, so then the boxes are simply drawn where they are.)
		double fraction = 0;
		if (timeStep > 0)
		{
			fraction = timeAccumulator / timeStep;
		}
		double shownPositionA = previousPositionA + (positionA - previousPositionA) * fraction;
		double shownPositionB = previousPositionB + (positionB - previousPositionB) * fraction;
		boxA.Position = new Vector2((float)shownPositionA * PixelsPerMeter, -BoxSize / 2);
		boxB.Position = new Vector2((float)shownPositionB * PixelsPerMeter, -BoxSize / 2);

		MoveCamera();
		UpdateResults();
		FitPanel();
		QueueRedraw();
	}

	// One time step. The forces do not change during a step, so this update is exact.
	private void Step()
	{
		previousPositionA = positionA;
		previousPositionB = positionB;
		UpdateForces();

		positionA = positionA + velocityA * timeStep + 0.5 * accelerationA * timeStep * timeStep;
		velocityA = velocityA + accelerationA * timeStep;

		positionB = positionB + velocityB * timeStep + 0.5 * accelerationB * timeStep * timeStep;
		velocityB = velocityB + accelerationB * timeStep;

		stepCount++;

		BounceOffWalls();
		BounceOffEachOther();
		BounceOffWalls();
		UpdateForces();
	}

	// A box that reaches a wall bounces back with the same speed
	private void BounceOffWalls()
	{
		if (positionA - 0.5 < leftWall)
		{
			positionA = 2 * (leftWall + 0.5) - positionA;
			velocityA = -velocityA;
			wallBounces++;
		}
		if (positionB + 0.5 > rightWall)
		{
			positionB = 2 * (rightWall - 0.5) - positionB;
			velocityB = -velocityB;
			wallBounces++;
		}
	}

	// When the boxes meet, they push each other apart again (Newton's third law).
	// This is an elastic collision: the total momentum and the kinetic energy stay the same.
	private void BounceOffEachOther()
	{
		bool touching = positionB - positionA < 1;
		bool comingCloser = velocityA > velocityB;
		if (!touching || !comingCloser)
		{
			return;
		}

		// How long ago in this step they touched: the overlap divided by how fast they came closer
		double overlap = 1 - (positionB - positionA);
		double timeSinceTouch = Math.Min(overlap / (velocityA - velocityB), timeStep);

		// Go back to the moment they touched
		positionA = positionA - velocityA * timeSinceTouch;
		positionB = positionB - velocityB * timeSinceTouch;

		// New velocities after the elastic collision
		double totalMass = massA + massB;
		double newVelocityA = ((massA - massB) * velocityA + 2 * massB * velocityB) / totalMass;
		double newVelocityB = ((massB - massA) * velocityB + 2 * massA * velocityA) / totalMass;
		// The kick each box got: its change of momentum. A gets -kick if B gets +kick (Newton's third law).
		lastKickOnA = massA * (newVelocityA - velocityA);
		hitArrowTime = 0.4;
		velocityA = newVelocityA;
		velocityB = newVelocityB;

		// Move on for the rest of the step with the new velocities
		positionA = positionA + velocityA * timeSinceTouch;
		positionB = positionB + velocityB * timeSinceTouch;
		boxBounces++;
	}

	// The camera does not move: the whole stage between the walls is always on screen
	private void MoveCamera()
	{
		float screenWidth = GetViewportRect().Size.X;
		float margin = 60;
		if (scroll.Visible)
		{
			openPanelRight = panel.Position.X + panel.Size.X;
		}

		// The zoom is chosen so the stage fits right of the open panel, and it stays the same when the panel is hidden
		float stageWidth = (float)(2 * WallDistance) * PixelsPerMeter + 60;
		float zoomWidth = screenWidth - openPanelRight - 2 * margin;
		float zoom = Math.Min(1, zoomWidth / stageWidth);

		// The middle of the stage is shown in the middle of the free part of the screen
		float freeLeft = margin;
		if (scroll.Visible)
		{
			freeLeft = openPanelRight + margin;
		}
		float freeCenter = (freeLeft + screenWidth - margin) / 2;
		float stageCenter = (float)contactPoint * PixelsPerMeter;

		camera.Zoom = new Vector2(zoom, zoom);
		camera.Offset = new Vector2(0, 0);
		camera.Position = new Vector2(stageCenter - (freeCenter - screenWidth / 2) / zoom, -200);
	}

	private void UpdateResults()
	{
		pushAValue.Text = Show(pushOnA) + " N";
		pushBValue.Text = Show(pushOnB) + " N";
		hitAValue.Text = Show(lastKickOnA) + " N·s";
		hitBValue.Text = Show(-lastKickOnA) + " N·s";
		otherAValue.Text = Show(otherForceA) + " N";
		otherBValue.Text = Show(otherForceB) + " N";
		netForceAValue.Text = Show(netForceA) + " N";
		netForceBValue.Text = Show(netForceB) + " N";
		accelerationAValue.Text = Show(accelerationA) + " m/s²";
		accelerationBValue.Text = Show(accelerationB) + " m/s²";
		velocityAValue.Text = Show(velocityA) + " m/s";
		velocityBValue.Text = Show(velocityB) + " m/s";
		positionAValue.Text = Show(positionA) + " m";
		positionBValue.Text = Show(positionB) + " m";

		// The two push forces always add up to zero (Newton's third law)
		pairValue.Text = Show(pushOnA + pushOnB) + " N";

		// Total momentum p = mA*vA + mB*vB. It changes only when a wall pushes a box back.
		momentumValue.Text = Show(massA * velocityA + massB * velocityB) + " kg·m/s";

		// Kinetic energy. Without friction it stays the same after the push.
		energyValue.Text = Show(KineticEnergy()) + " J";

		bouncesValue.Text = wallBounces + " / " + boxBounces;
		timeValue.Text = Show(stepCount * timeStep) + " s";
	}

	private double KineticEnergy()
	{
		return 0.5 * massA * velocityA * velocityA + 0.5 * massB * velocityB * velocityB;
	}

	// Rounds a value to 2 decimals for the panel. Adding 0.0 turns "-0" into "0".
	private double Show(double value)
	{
		return Math.Round(value, 2) + 0.0;
	}

	private void PrintResults()
	{
		GD.Print("");
		GD.Print("Simulation results");
		GD.Print("");
		GD.Print("Contact point: " + contactPoint + " m, walls at " + leftWall + " m and " + rightWall + " m");
		GD.Print("Push force: " + push + " N for " + pushTime + " s");
		GD.Print("");
		GD.Print("Box A: mass " + massA + " kg, other forces " + otherForceA + " N");
		GD.Print("Box A final position: " + positionA + " m");
		GD.Print("Box A final velocity: " + velocityA + " m/s");
		GD.Print("");
		GD.Print("Box B: mass " + massB + " kg, other forces " + otherForceB + " N");
		GD.Print("Box B final position: " + positionB + " m");
		GD.Print("Box B final velocity: " + velocityB + " m/s");
		GD.Print("");
		GD.Print("Bounces: " + wallBounces + " off the walls, " + boxBounces + " off each other");
		GD.Print("Total momentum at the end: " + (massA * velocityA + massB * velocityB) + " kg*m/s");
		GD.Print("Kinetic energy at the end: " + KineticEnergy() + " J");
		GD.Print("");
		GD.Print("Total simulation time: " + totalTime + " s");
		GD.Print("Simulation steps: " + stepCount);
	}

	// Drawing (in world pixels: x to the right, the ground is at y = 0, up is negative y)
	public override void _Draw()
	{
		DrawDistanceMarks();
		DrawWall((float)leftWall * PixelsPerMeter, -1);
		DrawWall((float)rightWall * PixelsPerMeter, 1);

		// Arrows above each box: velocity, net force, and the push from the other box
		Font font = ThemeDB.FallbackFont;
		float top = -BoxSize - 20;
		DrawLabeledArrow(boxA.Position.X, top, (float)velocityA * pixelsPerSpeed, Navy, "v", font);
		DrawLabeledArrow(boxA.Position.X, top - 25, (float)netForceA * pixelsPerNewton, Teal, "F net", font);
		DrawLabeledArrow(boxA.Position.X, top - 50, (float)pushOnA * pixelsPerNewton, Coral, "push", font);
		DrawLabeledArrow(boxB.Position.X, top, (float)velocityB * pixelsPerSpeed, Navy, "v", font);
		DrawLabeledArrow(boxB.Position.X, top - 25, (float)netForceB * pixelsPerNewton, Teal, "F net", font);
		DrawLabeledArrow(boxB.Position.X, top - 50, (float)pushOnB * pixelsPerNewton, Coral, "push", font);

		// Just after the boxes hit each other: equal and opposite push arrows that fade out.
		// Their length shows the kick, on the same scale as the starting push (start push = 120 px).
		if (hitArrowTime > 0 && lastKickOnA != 0)
		{
			double startKick = push * pushTime;
			if (startKick <= 0)
			{
				startKick = Math.Abs(lastKickOnA);
			}
			float length = (float)Math.Min(160, 120 * Math.Abs(lastKickOnA) / startKick);
			Color fading = new Color(Coral, (float)(hitArrowTime / 0.4));
			DrawLabeledArrow(boxA.Position.X, top - 50, Math.Sign(lastKickOnA) * length, fading, "push", font);
			DrawLabeledArrow(boxB.Position.X, top - 50, -Math.Sign(lastKickOnA) * length, fading, "push", font);
		}
	}

	// A wall: a tall post with a darker edge. side = -1 for the left wall, 1 for the right wall.
	private void DrawWall(float x, float side)
	{
		float width = 24;
		float height = 220;
		float left = x;
		if (side < 0)
		{
			left = x - width;
		}
		DrawRect(new Rect2(left, -height, width, height), new Color("7A5236"));
		DrawRect(new Rect2(left, -height, width, 8), Navy);
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
