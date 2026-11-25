# Newton's First Law: versions

### v1.0 — Core Prototype

The first working Godot version of the Newton's First Law simulation, built on the Octave prototype.

### Implemented:
* 2D position and velocity (y stays 0: the box's weight W = m*g and the ground's push N = m*g cancel)
* Any number of forces, each with an x and a y part
* Net force calculation
* Acceleration calculation using Newton's Second Law, a = Fnet / m
* Velocity and position updates with Velocity Verlet, in fixed time steps dt
* Mass, start position, start velocity, forces, time step and total time from the panel (the boxes start empty)
* Live results: time, position, velocity, acceleration and net force
* The box moving on the grass field, with distance marks and a camera that follows it

### Purpose:
This version establishes the core physics and the structure of the Godot simulation that the later versions build on.

### Differences from the Octave version:
The values come from the panel instead of fixed numbers, Velocity Verlet replaces semi-implicit Euler, and any number of forces can be added instead of two.

### v1.1 — Input Checks and Formatted Results

Every value from the panel is checked before the simulation can start, and every result is shown with its unit and three decimals.

### Implemented:
* A message that names the first box without a number, for example: "Mass" must contain a number.
* Mass more than 0 kg, time step dt more than 0 s and total time T more than 0 s
* The time step dt can't be longer than the total time T
* At most 100000 time steps in one run
* The box can't go more than 50 km from the 0 m mark (worked out before Start with x = x0 + v0*t + 0.5*a*t^2, also at the point where the box turns back)
* The name of a wrong box turns coral
* Results with units and three decimals, as in the Octave prototype: time (s), position (m), velocity (m/s), acceleration (m/s²) and net force (N), with no -0.000
* New result: distance moved (m)

### Purpose:
This version makes the simulation safe for any value typed into the panel and makes the results clear and readable, while the physics from v1.0 stays the same.

### Differences from the Octave version:
The console input of the Octave v1.1 was already replaced by the panel in v1.0, so this version adds the checks a panel needs, which the Octave prototype added in later versions.

### v1.2 — Simulation Time Handling

Improved the simulation's time handling and state storage.

### Implemented:
* The state is stored at every time index: time, position, velocity, acceleration and net force
* Index 0 holds the starting state (t = 0), index i the state after step i, so the initial state is separated from the calculated states
* The history lists get their full size at the start of the run, like the preallocated arrays in Octave
* The time is always i*dt, never dt added up again and again, so it can't drift
* T must be a whole number of time steps (T = n*dt); the message gives the two nearest totals that work
* Final simulation time check: the results show "Final time reached" when the run ends exactly at T
* Step button: one step forward while the simulation is paused
* Speed buttons 0.25x, 0.5x, 1x, 2x and 4x, which change only how fast the steps are taken, never the steps
* A message when the device can't keep up (more than 12 steps due in one frame)

### Purpose:
This version makes sure that every stored state belongs to the right simulation time and that the run ends exactly at T, and it lets the motion be followed step by step or faster.

### Differences from the Octave version:
The Octave v1.2 also allowed time steps that don't divide T exactly; here T must be a whole number of time steps, as in the final Octave version. Step by step and the speed buttons are new, because a live simulation needs them.
