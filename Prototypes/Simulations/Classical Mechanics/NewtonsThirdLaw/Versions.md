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