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

### v1.4 — Visualization and Graphs

Adds visual representations of the bodies, interaction forces, and resulting motion to the Newton's Third Law simulation.

### Implemented:
* Visualization of both bodies at their initial positions in a 2D coordinate system
* Display of interaction-force vectors with correct directions
* Visual distinction between the forces acting on each body using different colors
* Adjustable visual scaling of force vectors for clearer presentation
* Position-versus-time graphs for the x- and y-coordinates of both bodies
* Velocity-versus-time graphs for the x- and y-components of both bodies
* Acceleration-versus-time graphs for the x- and y-components of both bodies
* Clear graph titles, axis labels, legends, and grid lines
* Consistent SI units across the visualizations
* Validation of plotted results against the simulation's numerical output

### Purpose:
This version adds visual representations to make the simulation's physics and resulting motion easier to understand. It allows the user to observe the bodies' initial positions, compare their interaction forces, and analyze how their positions, velocities, and accelerations change over time.

### v1.5 — Analytical Validation and Numerical Error Analysis

This version extends the Newton's Third Law simulation with analytical calculations and numerical error analysis to evaluate the accuracy of the simulation results.

### Implemented:
* Analytical position and velocity calculations for both bodies using constant-acceleration equations
* Comparison of numerical and analytical position and velocity result
* Calculation of position and velocity error magnitudes at every time step
* Display of final numerical and analytical results
* Validation of Newton's Third Law using the force-pair residual
* Comparison of position errors for two time-step sizes, dt and dt/2
* Visualization of numerical and analytical trajectories
* Position-versus-time and velocity-versus-time graphs with analytical reference curves
* Position-error and velocity-error graphs
* Extended time-history output containing numerical results, analytical results, and error magnitudes
* Automatic validation output for Newton's Third Law and time-step comparison

### Limitations:
* The analytical comparison assumes constant masses and constant applied forces.
* The simulation uses semi-implicit Euler integration, which introduces position error for constant acceleration, while velocity should remain accurate up to floating-point precision.
* The time-step comparison calculates the expected numerical position for each time step analytically rather than running a second simulation.
* This version supports exactly two bodies and does not yet validate general variable-force scenarios.

### Purpose:
To verify the accuracy of the simulation by comparing its numerical results with analytical solutions, measuring numerical errors, and examining how the time-step size affects the calculated motion.

### v1.6 — Improved Numerical Integration

Improves the numerical integration of the Newton's Third Law simulation by implementing velocity Verlet and comparing its results with semi-implicit Euler and the analytical solution for constant acceleration.

### Implemented:
* Implemented the velocity Verlet integration method for calculating positions and velocities.
* Updated positions using the previous velocity and acceleration.
* Updated velocities using the average of the previous and new accelerations.
* Added a separate semi-implicit Euler simulation for comparison.
* Stored position and velocity histories for both numerical methods at every time step.
* Calculated position and velocity errors for both methods against the analytical solution.
* Added trajectory comparisons between semi-implicit Euler, velocity Verlet, and the analytical solution.
* Added position-versus-time and velocity-versus-time graphs comparing all three approaches.
* Added error-versus-time graphs for both numerical methods.
* Compared the final results of semi-implicit Euler and velocity Verlet using the original time step.
* Added a second velocity Verlet calculation using half the original time step to examine timestep sensitivity.
* Preserved Newton's Third Law validation, simulation results, and time-history output.
* Tested the implementation using known masses, initial conditions, interaction forces, and external forces.
* Verified that velocity Verlet reproduces the analytical results to floating-point precision for constant acceleration.

### Purpose:
To improve the accuracy and evaluation of numerical integration by comparing different methods against a known analytical solution. This version demonstrates the position error introduced by semi-implicit Euler and the accuracy of velocity Verlet under constant-acceleration conditions.