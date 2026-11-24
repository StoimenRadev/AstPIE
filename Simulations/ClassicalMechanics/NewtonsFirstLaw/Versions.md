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
