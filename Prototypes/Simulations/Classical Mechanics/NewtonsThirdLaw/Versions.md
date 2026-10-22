### v1.0 — Core Prototype

The first working Octave prototype of the Newton's Third Law simulation.

### Implemented:
* 2D position and velocity for two bodies
* Configurable masses, initial positions, and initial velocities
* A constant interaction force acting on each body
* Newton's Third Law verification using an equal-and-opposite force check
* Net force calculation for each body
* Acceleration calculation using Newton's Second Law
* Velocity and position updates using semi-implicit Euler integration
* Configurable simulation timestep and total simulation time
* Storage of position, velocity, and acceleration throughout the simulation
* Final position, velocity, acceleration, and net force output
* Basic error handling when the Newton's Third Law check fails

### Purpose:
This version establishes the core physics and simulation structure for a two-body Newton's Third Law model. It verifies that the interaction forces are equal in magnitude and opposite in direction, calculates the resulting accelerations, and simulates both bodies' motion over time.

### v1.1 — User Inputs and Simulation Results

An improved version of the Newton's Third Law simulation that introduces user-configurable parameters and organized simulation output.

### Implemented:
* User input for the number of bodies, restricted to exactly two for the current force model
* User-configurable masses for both bodies
* User-configurable initial positions and velocities in 2D
* User-configurable interaction forces for both bodies
* Newton's Third Law verification to check that the forces are equal in magnitude and opposite in direction
* Basic validation for positive masses, timestep, and simulation duration
* Configurable simulation timestep and total simulation time
* Net force and acceleration calculations for each body
* Velocity and position updates using semi-implicit Euler integration
* Storage of position, velocity, and acceleration throughout the simulation
* Organized output displaying the simulation setup, initial conditions, and final results at the end of execution
* Basic error handling for invalid parameters and force pairs

### Purpose:
This version transforms the v1.0 prototype into an interactive simulation that accepts user-defined parameters instead of relying on hardcoded values. It establishes basic input validation, verifies Newton's Third Law for the specified force pair, and presents the simulation configuration and final numerical results in a clear structure.

### v1.2 — Time History and Simulation Data

Expanded the Newton's Third Law simulation to store and display the evolution of each body's physical state throughout the simulation.

### Implemented:
* Time-history storage for position, velocity, acceleration, and net force
* Storage of speed at every time point
* Preallocation of simulation data arrays for efficient storage 
* Recording of initial conditions and subsequent simulation states 
* Generation of a time array based on the configured time step and total simulation time
* Separate time-history tables for each body
* Organized output of time, position, velocity, speed, acceleration, and net force
* Final simulation results extracted from the stored time-history data
* Verification of numerical results against analytical constant-acceleration equations

### Purpose:
To extend the simulation beyond calculating only the final state by preserving the physical state of each body at every time point. This enables analysis of motion throughout the simulation, supports future visualization and plotting features, and provides a foundation for numerical accuracy testing and validation.

### v1.3 — Independent Net Force and Acceleration Calculation

Extend the simulation to calculate each body's net force and acceleration independently, accounting for interaction forces and additional external forces.

### Implemented:
* Separate interaction forces from net forces
* Calculate each body's net force from all applied forces
* Calculate acceleration independently using each body's mass
* Support different masses and initial velocities
* Store and display interaction forces, external forces, net forces, and accelerations separately
* Prepare the simulation for systematic physical and numerical validation

### Purpose: Improve the simulation's physical model and make it capable of demonstrating how mass and net force affect the motion of each body while preserving Newton's Third Law.