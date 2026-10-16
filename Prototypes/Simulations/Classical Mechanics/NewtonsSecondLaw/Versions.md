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

### v1.3 — Force and Acceleration Experiments

An extended Octave prototype of the Newton's Second Law simulation that tests the relationship between net force, mass, and acceleration through controlled experiments.

### Implemented:
* Experiments investigating the relationship between net force and acceleration while keeping mass constant
* Experiments investigating the relationship between mass and acceleration while keeping net force constant
* Experiments investigating how the direction of the net force determines the direction of acceleration
* Experiments with balanced forces to verify that zero net force produces zero acceleration
* Tests demonstrating that zero net force does not necessarily mean zero velocity
* Verification that doubling the net force doubles the acceleration when mass remains constant
* Verification that increasing mass reduces acceleration when the net force remains constant
* Tests of positive and negative force components in two-dimensional motion
* Comparison of simulation outputs across different experimental conditions

### Purpose:
This version uses controlled experiments to verify the relationships described by Newton's Second Law. By varying net force, mass, and force direction independently, the prototype demonstrates how these quantities affect acceleration and motion. It also reinforces the distinction between net force and velocity, establishing a foundation for more advanced physical experiments and numerical validation.

### v1.4 — Visualization and Graphs

An extended Octave prototype of the Newton's Second Law simulation that visualizes the object's trajectory, position and velocity over time, and the force vectors acting on the object.

### Implemented:
* Two-dimensional trajectory visualization using the object's recorded position history
* Markers identifying the initial and final positions
* Visualization of the first applied force vector
* Visualization of the second applied force vector
* Visualization of the net force vector
* Visualization of the final velocity vector
* Labels displaying the applied forces, net force, acceleration, and final velocity
* Position-versus-time graph showing both spatial components
* Velocity-versus-time graph showing both velocity components
* Grid lines, axis labels, titles, and legends for the graphs
* Verification that the generated time array reaches the intended total simulation time
* Integration of visualizations with the existing time-history data and simulation results

### Purpose:
This version extends the numerical prototype with graphical representations of the object's motion and the forces acting on it. The trajectory illustrates the object's path in two dimensions, while the position-versus-time and velocity-versus-time graphs show how its motion evolves throughout the simulation. Force and velocity vectors provide a visual representation of their directions and relationships.