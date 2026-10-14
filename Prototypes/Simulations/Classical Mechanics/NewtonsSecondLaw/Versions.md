### v1.0 — Core Prototype

The first working Octave prototype of the Newton's Second Law simulation.

### Implemented:
* 2D position and velocity
* Multiple forces with magnitude and direction
* Net force calculation
* Acceleration calculation using Newton's Second Law
* Velocity and position updates using semi-implicit Euler integration
* Configurable mass, initial position, and initial velocity
* Configurable simulation timestep and total simulation time
* Storage of position, velocity, and acceleration throughout the simulation
* Final position, velocity, and acceleration output

### Purpose:
This version establishes the core physics and simulation structure that will be expanded and improved in later versions

### v1.1 — User Inputs and Simulation Results

An interactive version of the Newton's Second Law simulation that allows users to configure the physical parameters and initial conditions.

### Implemented:
* User-defined mass
* User-defined initial position and velocity in 2D
* User-defined simulation timestep and total duration
* User-defined forces with independent (x) and (y) components
* Initial position and velocity preservation
* Net force and acceleration calculations
* Position and velocity updates using semi-implicit Euler integration
* Storage of position, velocity, and acceleration throughout the simulation
* Initial and final speed calculations
* Organized output of initial conditions, forces, acceleration, and final state
* Simulation duration and step-count reporting
* Basic check that the simulation reaches the requested final time

### Purpose:
This version makes the simulation configurable without modifying its source code. It improves usability by allowing users to investigate how mass, forces, initial conditions, and timestep affect the motion of an object.

### v1.2 — Time History

An improved Octave prototype of the Newton's Second Law simulation that records the object's physical state throughout the entire simulation.

### Implemented:
* Time-history storage for 2D position and velocity
* Time-history storage for acceleration and net force
* Speed calculation at every timestep from the velocity components
* Initial-state storage at (t=0)
* Storage of the updated physical state at every subsequent timestep
* Combined time-history results table containing time, position, velocity, speed, acceleration, and net force
* Input validation for positive mass, two-component position and velocity vectors, and two-component force vectors
* Input validation for positive timestep and total simulation time
* Verification that the total simulation time is an integer multiple of the timestep
* Initial and final speed calculations using the stored history
* Final-state and simulation information output

### Purpose:
This version expands the core simulation by recording the object's motion and the net force acting on it at every timestep. The stored history allows the results to be examined throughout the simulation, providing the foundation for future experiments, visualization, and numerical validation.