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
* User-defined forces with independent \(x\) and \(y\) components
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