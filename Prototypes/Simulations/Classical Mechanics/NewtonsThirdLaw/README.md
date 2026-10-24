# Newton's Third Law — 2D Simulation

## Overview

This prototype is a numerical 2D simulation demonstrating Newton's Third Law of Motion through the interaction of two bodies subjected to equal-and-opposite interaction forces and additional external forces.

The simulation accepts user-defined masses, initial positions, initial velocities, interaction forces, external forces, the time step, and the total simulation duration. It calculates the resulting motion of both bodies and provides numerical and graphical representations of their behaviour.

The implementation also calculates an analytical reference solution for constant acceleration, compares two numerical integration methods, measures numerical errors, and checks the consistency of the interaction-force pair.

The simulation was developed incrementally through successive versions, introducing the core physical model, user input, time-history data, force and acceleration experiments, visualization, analytical validation, improved numerical integration, and comprehensive testing.

The v1.8 milestone establishes the intended stable and documented core version of the simulation.

---

## Physical Model

The simulation represents two interacting bodies in a two-dimensional coordinate system. Each body is described by the following physical parameters:

- Mass
- Initial position
- Initial velocity
- Acceleration
- Interaction force
- External force
- Net force

The numerical simulation also uses a time step and a total simulation duration.

### Newton's Third Law

Newton's Third Law states that when one body exerts a force on another body, the second body exerts an equal-magnitude force in the opposite direction on the first body.

For the two-body system, the interaction forces satisfy:

F12 = -F21

Here, F12 represents the force exerted by body 1 on body 2, while F21 represents the force exerted by body 2 on body 1.

The forces have equal magnitudes and opposite directions. However, they act on different bodies and therefore must be applied separately when calculating each body's motion.

The force-pair relationship can be checked using the residual:

R = F12 + F21

For an ideal Newton's Third Law interaction, the residual is the zero vector. In the implementation, the residual magnitude is calculated numerically to check whether the two forces satisfy this relationship within a specified tolerance.

### Net Force

Each body can also experience external forces. These are distinguished from the interaction forces between the two bodies.

The net force acting on body 1 is calculated as:

Fnet1 = F21 + Fext1

The net force acting on body 2 is calculated as:

Fnet2 = F12 + Fext2

In these equations, Fext1 and Fext2 represent the external forces acting on bodies 1 and 2, respectively.

The distinction is important because Newton's Third Law applies to the interaction-force pair, not necessarily to the total net forces on the two bodies. External forces may cause the net forces to have different magnitudes or directions.

### Acceleration

The acceleration of each body is calculated independently using Newton's Second Law:

a1 = Fnet1 / m1

a2 = Fnet2 / m2

Consequently, even when the interaction forces have equal magnitudes, the resulting accelerations can differ if the bodies have different masses or experience different external forces.

The model assumes constant masses and constant forces throughout a simulation run. Therefore, the net force and acceleration of each body remain constant during that run.

---

## Numerical Simulation

The simulation creates a time array covering the requested total simulation duration.

time = (0:round(T/dt)) * dt

The final time value is explicitly set to T after checking that the total duration is an integer multiple of the time step within the accepted numerical tolerance.

The main simulation parameters are:

- `dt`: time step in seconds
- `T`: total simulation time in seconds
- `time`: array of simulation timestamps
- `num_steps`: number of stored simulation states

Input validation checks the supported parameters before the simulation proceeds. The program requires positive finite masses, finite two-component position and velocity vectors, finite two-component force vectors, a positive finite time step, and a positive finite total duration compatible with the time step.

The simulation stores the state of both bodies throughout the run. Its data includes:

- Position components
- Velocity components
- Acceleration components
- Speed
- Interaction forces
- Net forces
- Analytical reference values
- Numerical error magnitudes

The position, velocity, acceleration, and force vectors contain separate x and y components. Each stored row corresponds to a particular simulation time.

The initial conditions are recorded before integration begins. At each subsequent time step, the numerical method updates the positions and velocities of both bodies and records the resulting state.

This approach provides a complete time history of the system rather than only its final state.

---

## Velocity Verlet Integration

The primary integration method is the **Velocity Verlet method**, introduced to improve the numerical integration used in the earlier versions.

For each body, the position is updated using its current position, velocity, and acceleration:

x(t + dt) = x(t) + v(t) * dt + 0.5 * a(t) * dt^2

The acceleration at the next time step is then calculated from the net force:

a(t + dt) = Fnet(t + dt) / m

Finally, the velocity is updated using the average of the acceleration at the beginning and end of the time step:

v(t + dt) = v(t) + 0.5 * [a(t) + a(t + dt)] * dt

These calculations are performed independently for both bodies using their respective masses, net forces, positions, velocities, and accelerations.

For the current model, the forces and accelerations remain constant. Therefore, the Velocity Verlet method reproduces the analytical constant-acceleration solution up to floating-point rounding, assuming the implementation and inputs are consistent with the model.

The method is retained as the primary integration method because it provides a clear, reliable numerical foundation for the supported physical scenario.

---

## Semi-Implicit Euler Comparison

The simulation also implements semi-implicit Euler as a comparison method.

For each body, velocity is updated first:

v(t + dt) = v(t) + a(t) * dt

Position is then updated using the new velocity:

x(t + dt) = x(t) + v(t + dt) * dt

This update order distinguishes semi-implicit Euler from explicit Euler, which uses the old velocity to update position.

Both methods use the same initial conditions, masses, and net forces, allowing their numerical results to be compared directly.

For constant acceleration, semi-implicit Euler calculates velocity correctly up to floating-point rounding, but its position update introduces a time-step-dependent error because it uses the new velocity for the entire interval.

The simulation compares the final positions produced by semi-implicit Euler and Velocity Verlet with the analytical reference positions. This provides a practical demonstration of how the selected integration method affects numerical accuracy.

---

## Analytical Solution

The simulation calculates an analytical reference solution for each body under constant acceleration.

For a body with initial position x0, initial velocity v0, and constant acceleration a, the analytical position is:

x(t) = x0 + v0 * t + 0.5 * a * t^2

The analytical velocity is:

v(t) = v0 + a * t

These equations are applied independently to both bodies using their own initial conditions and accelerations.

The analytical speed is calculated from the magnitude of the analytical velocity vector.

speed(t) = sqrt(vx(t)^2 + vy(t)^2)

The analytical solution provides an independent reference for validating the numerical integration. It is appropriate for the current implementation because the forces, masses, and resulting accelerations remain constant throughout each simulation run.

The reference solution is also used to calculate position and velocity errors at every stored time step.

---

## Newton's Third Law Residual

The simulation evaluates the consistency of the interaction-force pair using the residual vector:

R = F12 + F21

The magnitude of the residual is calculated using the Euclidean norm:

|R| = sqrt(Rx^2 + Ry^2)

For a correctly defined interaction-force pair, both residual components should be zero, apart from possible floating-point effects.

The program checks the residual magnitude against a small numerical tolerance. If the residual exceeds the accepted tolerance, the force-pair relationship is reported as invalid.

This check is separate from the calculation of the net forces. External forces are not included in the Newton's Third Law residual because they are not part of the interaction-force pair being tested.

The residual is reported in the numerical summary, providing a direct numerical check of the defining relationship demonstrated by the simulation.

---

## Numerical Error Analysis

The simulation compares the numerical positions and velocities with their analytical reference values at every time step.

For each body, the position-error vector is calculated as:

position_error_vector(t) = position_numerical(t) - position_analytical(t)

The velocity-error vector is calculated as:

velocity_error_vector(t) = velocity_numerical(t) - velocity_analytical(t)

The corresponding error magnitudes are calculated using the Euclidean norm.

position_error(t) = sqrt(error_x(t)^2 + error_y(t)^2)

velocity_error(t) = sqrt(error_vx(t)^2 + error_vy(t)^2)

Position error is measured in metres, while velocity error is measured in metres per second.

The simulation records these errors throughout the run and uses them to examine the accuracy of the numerical methods.

For the current constant-acceleration model, Velocity Verlet should agree with the analytical position and velocity up to floating-point rounding. Semi-implicit Euler should also produce approximately correct velocities, but its position errors can increase with time and depend on the time step.

The simulation's time-step comparison repeats the relevant calculations with a reduced time step and compares the final numerical position errors. This helps demonstrate the relationship between time-step size and integration accuracy.

---

## Vector Visualization and Graphs

The simulation provides graphical representations of the bodies' motion and the numerical results.

The visual outputs include the numerical trajectories of both bodies, the analytical reference trajectories, and comparisons between the integration methods. Force-related visualization is used to help interpret the directions of the interaction forces and their effects on the bodies.

The plots use labelled axes, legends, and appropriate physical units to make the results easier to interpret.

### Trajectories

The trajectory plot displays the paths followed by both bodies in the two-dimensional coordinate system.

It compares the numerical results from the integration methods with the analytical reference trajectories. This makes it possible to identify differences in the predicted motion and examine how the bodies move under their respective net forces.

### Position versus time

The position-versus-time graph displays the x and y position components of each body as functions of time.

The numerical results are compared with the analytical position components. The plot makes it possible to examine how the positions evolve and to identify discrepancies between the numerical and reference solutions.

### Velocity versus time

The velocity-versus-time graph displays the x and y velocity components of both bodies throughout the simulation.

Comparing the numerical and analytical values makes it possible to check whether the velocity updates are consistent with the constant-acceleration model.

### Numerical error graphs

The error graphs display the position and velocity error magnitudes throughout the simulation.

The position-error graph shows how the difference between the numerical and analytical positions develops over time. The velocity-error graph performs the same comparison for velocity.

These plots are useful for evaluating the numerical integration methods and investigating the effect of changing the time step.

---

## Velocity and Speed

Velocity is represented as a two-dimensional vector:

v = [vx, vy]

Speed is a scalar quantity calculated from the magnitude of the velocity vector:

speed = sqrt(vx^2 + vy^2)

Velocity preserves directional information, while speed describes only the magnitude of the motion.

The simulation calculates and records the speed of each body throughout the run. Analytical speed values are calculated independently from the analytical velocity components.

The initial and final speeds are included in the numerical results, allowing the initial state, final state, and analytical reference to be compared.

---

## Numerical Output and Time History

The simulation reports the principal physical parameters and numerical results for both bodies.

The command-line output includes:

- Number of bodies
- Mass of each body
- Initial positions and velocities
- Initial speeds
- Interaction forces
- External forces
- Net forces
- Accelerations
- Initial and final numerical states
- Final analytical positions and velocities
- Initial, final numerical, and analytical speed values where applicable
- Final position and velocity errors
- Newton's Third Law residual
- Time-step comparison results
- Total simulation time
- Time step
- Number of stored simulation states

Numerical values are formatted for readability. Most physical quantities are displayed to three decimal places, while small numerical errors and the force-pair residual use appropriate significant-digit formatting.

The simulation also displays a time-history table for the main Velocity Verlet run. For each body, the table records the time, position, velocity, speed, acceleration, force information, analytical reference values, and position and velocity error magnitudes.

This provides a consolidated numerical record that can be inspected independently of the graphs.

The time-history data is useful for checking individual time steps, comparing numerical and analytical values, and diagnosing unexpected results.

---

## Validation and Testing

The simulation's validation process checks physical correctness, numerical accuracy, data consistency, and input handling.

The planned and implemented testing scope includes:

- Valid and invalid input values
- Positive finite masses
- Initial positions and velocities in two dimensions
- Equal and unequal masses
- Different initial positions
- Different initial velocities
- Horizontal, vertical, and diagonal force configurations
- Equal-and-opposite interaction forces
- Newton's Third Law residual calculations
- External forces and their effects on net force
- Independent acceleration calculations for both bodies
- Constant-acceleration analytical reference cases
- Velocity Verlet results against the analytical solution
- Semi-implicit Euler results against the analytical solution
- Different time-step sizes
- Different total simulation durations
- Zero initial velocities
- Zero interaction forces and zero net-force cases
- Position and velocity error calculations
- Time-history dimensions and recorded values
- Graphical consistency with the numerical data
- Numerical summaries and final-state calculations
- Regression checks after implementation changes

### Analytical validation

For each body, the analytical constant-acceleration equations provide reference values for position and velocity.

These values are compared with the corresponding numerical results. Differences are assessed using numerical tolerances appropriate to the calculations.

### Force-pair validation

The interaction-force residual is checked to confirm that the forces have equal magnitudes and opposite directions.

The check is performed on the interaction forces themselves, independently of the external forces acting on the bodies.

### Integration-method validation

Velocity Verlet and semi-implicit Euler are compared against the analytical solution.

The expected behaviour for the current model is that Velocity Verlet closely reproduces the analytical position and velocity, while semi-implicit Euler exhibits a time-step-dependent position error.

Repeating the calculations with a smaller time step allows the numerical behaviour to be examined.

### Input and software validation

Input-validation tests check that invalid values are rejected clearly and that valid inputs produce consistent results.

The tests also examine array dimensions, time-history indexing, final-state extraction, graph generation, and numerical output.

The core test suite is intended to establish that the integrated simulation works reliably rather than merely demonstrating that individual features function in isolation. Test outcomes should be recorded and any remaining limitations documented before the v1.8 release is declared complete.

---

## Assumptions and Limitations

The current simulation uses a deliberately simplified physical model.

Its main assumptions and limitations are:

- Exactly two bodies are supported.
- Motion is represented in two spatial dimensions.
- Masses remain constant throughout each simulation.
- Interaction forces are supplied as input rather than calculated from a distance-dependent physical law.
- Interaction forces remain constant during a run.
- External forces remain constant during a run.
- The net force and acceleration of each body are therefore constant.
- The analytical reference solution applies to constant acceleration.
- The force-pair residual checks the specified interaction forces; it does not independently establish that an arbitrary user-supplied force model represents a physically realistic interaction.
- The numerical comparison is limited to Velocity Verlet and semi-implicit Euler.
- The current model does not include variable forces, collisions, rotational dynamics, or a general many-body interaction system.

These limitations define the intended scope of the core version. They do not prevent the simulation from demonstrating Newton's Third Law or comparing the numerical methods for the supported physical scenario.

---

## Version Development

The simulation was developed through eight planned milestones.

- **v1.0 — Core Prototype:** Established the two-body model, interaction-force pair, acceleration calculations, basic time stepping, and numerical output.
- **v1.1 — User Inputs and Simulation Results:** Introduced configurable initial conditions, force parameters, simulation timing, input validation, and expanded numerical reporting.
- **v1.2 — Time History and Simulation Data:** Added time arrays, stored physical quantities, and consistent recording of both bodies' states.
- **v1.3 — Two-Body Force and Acceleration Experiments:** Expanded the investigation of different masses, initial conditions, interaction forces, external forces, net forces, and accelerations.
- **v1.4 — Visualization and Graphs:** Introduced graphical representations of the bodies' motion, force relationships, and time-dependent physical quantities.
- **v1.5 — Analytical Validation and Numerical Error Analysis:** Added reference calculations, force-pair residual checks, numerical error measurements, and time-step comparisons.
- **v1.6 — Improved Numerical Integration:** Introduced Velocity Verlet as the primary integration method and retained semi-implicit Euler for comparison.
- **v1.7 — Comprehensive Testing and Validation:** Consolidated systematic tests for the physics, numerical methods, data storage, graphical outputs, and input handling.
- **v1.8 — Core Version Complete:** Consolidates the implemented features into a stable, usable, and documented core release.

---

## Current State

The v1.8 core release is intended to provide a complete foundation for the Newton's Third Law simulation within AstPIE.

Its core capabilities include:

- Numerical 2D simulation of two interacting bodies
- Equal-and-opposite interaction-force calculations
- Separate external-force and net-force calculations
- Independent acceleration calculations for each body
- User-defined initial conditions and simulation parameters
- Velocity Verlet integration
- Semi-implicit Euler comparison
- Analytical position and velocity calculations
- Newton's Third Law residual calculation
- Position and velocity error analysis
- Time-step sensitivity comparisons
- Trajectory and time-dependent graphs
- Numerical summaries and time-history output
- Input validation and systematic testing

The v1.8 milestone also includes finalizing the documentation, confirming that the program runs from a clean starting state, recording the validation results, and committing the core release to version control.

These release tasks should be marked complete only after the corresponding work has been performed and verified.

The completed core provides a foundation for further development in AstPIE. Potential extensions include momentum and impulse calculations, variable interaction forces, additional numerical integration methods, collision scenarios, and more complex physical systems.

Future extensions will require their own physical assumptions, numerical methods, analytical or independent reference cases, and validation procedures. The current constant-acceleration analytical solution should not be assumed to validate models with variable forces without further analysis.
