# Newton's Second Law — 2D Simulation

## Overview

This prototype is a numerical 2D simulation demonstrating Newton's Second Law of Motion through the motion of an object subjected to external forces.

The simulation accepts initial physical conditions and two applied forces, calculates the resulting motion, and provides numerical and graphical representations of the results. It also calculates an analytical reference solution for constant acceleration and compares it with the numerical solution to evaluate numerical error.

The implementation was developed incrementally, with successive versions introducing user input, time-history data, force experiments, visualization, analytical validation, and testing.

---

## Physical Model

The simulation represents an object using the following physical and numerical parameters:

- Mass
- Initial 2D position
- Initial 2D velocity
- 2D acceleration
- Two applied 2D forces
- Time step
- Total simulation time

The net force is calculated as the vector sum of the applied forces:

Fnet = F1 + F2;

Acceleration is calculated using Newton's Second Law:

a = Fnet / mass;

The resulting acceleration is used to update the object's velocity at each timestep:

v_final = v + a * dt;

The object's position is then updated using the new velocity:

x_final = x + v_final * dt;

This update sequence implements the **semi-implicit Euler integration method**.

The simulation assumes a constant mass and constant applied forces throughout the simulated interval. Consequently, the net force and acceleration remain constant during each simulation run.

---

## Numerical Simulation

The simulation generates a time array covering the requested total simulation time:

time = 0:dt:T;

Input validation checks that the mass, timestep, and total simulation time are positive, that the position, velocity, and force vectors have two components, and that the total simulation time is an integer multiple of the timestep.

The simulation maintains separate arrays for position, velocity, acceleration, speed, and net force:

position = zeros(length(time), 2);
velocity = zeros(length(time), 2);
acceleration = zeros(length(time), 2);
speed = zeros(length(time), 1);
net_force = zeros(length(time), 2);

Each row represents one simulation timestep. The two columns of the vector arrays represent the x and y components.

The initial conditions are stored before the integration loop begins. The numerical state is then updated and recorded at every subsequent timestep:

for i = 2:length(time)

    v_final = v + a * dt;
    x_final = x + v_final * dt;

    v = v_final;
    x = x_final;

    position(i, :) = x;
    velocity(i, :) = v;
    acceleration(i, :) = a;
    speed(i) = sqrt(v(1)^2 + v(2)^2);
    net_force(i, :) = Fnet;

end

This provides a complete numerical history of the object's motion rather than only its final state.

---

## Analytical Solution

The prototype calculates an analytical reference solution for motion under constant acceleration. This reference allows the numerical integration results to be evaluated against the expected physical motion.

The analytical position is calculated using:

   x(t) = x_0 + v_0t + 1/2at^2

The analytical velocity is calculated using:

   v(t) = v_0 + at

In the implementation, these calculations are performed for every timestep:

analytical_position = repmat(x_initial, length(time), 1) ...
    + time(:) * v_initial ...
    + 0.5 * (time(:).^2) * a;

analytical_velocity = repmat(v_initial, length(time), 1) ...
    + time(:) * a;

The analytical speed is calculated as the magnitude of the analytical velocity vector:

analytical_speed = sqrt(sum(analytical_velocity.^2, 2));

The analytical solution provides a reference for examining the accuracy of the semi-implicit Euler method. It is valid for the current model because acceleration is constant throughout the simulation.

---

## Numerical Error Analysis

The prototype calculates the differences between the numerical and analytical position and velocity components at every timestep.

The component-wise position error is calculated as:

position_error_components = position - analytical_position;

The component-wise velocity error is calculated as:

velocity_error_components = velocity - analytical_velocity;

The corresponding error magnitudes are calculated using the Euclidean norm:

position_error = sqrt(sum(position_error_components.^2, 2));
velocity_error = sqrt(sum(velocity_error_components.^2, 2));

These values represent the absolute position error in metres and absolute velocity error in metres per second.

The prototype also extracts the final errors and reports the maximum error magnitudes across the entire simulation:

final_position_error = position_error(end);
final_velocity_error = velocity_error(end);

max(position_error);
max(velocity_error);

This makes it possible to investigate how the timestep affects numerical accuracy.

For the current constant-acceleration model, the semi-implicit Euler method introduces a position error because the position update uses the newly calculated velocity. The velocity calculation should match the analytical solution up to floating-point rounding, provided the same constant acceleration is used in both calculations.

---

## Vector Visualization

The simulation visualizes the numerical trajectory and analytical trajectory on the same coordinate plot.

figure;
plot(position(:, 1), position(:, 2));
hold on;

plot(analytical_position(:, 1), ...
     analytical_position(:, 2), "--");

plot(x_initial(1), x_initial(2), "o");
plot(x_final(1), x_final(2), "o");

The visualization includes:

- Numerical trajectory
- Analytical trajectory
- Initial position
- Final numerical position
- First applied force vector
- Second applied force vector
- Net force vector
- Final velocity vector

The force and velocity vectors are drawn using `quiver`. Labels display the applied forces, net force, acceleration, and final velocity. Their rotation angles are calculated from the corresponding vector components to align the labels with their vectors.

The plot also includes a grid, axis labels, a title, a legend, and equal scaling for the spatial axes.

This visualization makes it possible to compare the numerical trajectory with the analytical reference and observe the directions of the forces and final velocity.

---

## Velocity and Speed

Velocity is represented as a two-dimensional vector:

   v = [v_x,v_y]

Speed is calculated separately as the magnitude of the velocity vector:

   speed = sqrt(v_x^2 + v_y^2)

The analytical speed is calculated independently from the analytical velocity values.

This distinction allows the simulation to preserve directional information in its velocity calculations while also reporting a scalar speed value.

Both initial and final numerical speeds are calculated and displayed. The final analytical speed is also reported.

---

## Position and Velocity Analysis

The stored numerical and analytical data is used to generate time-dependent graphs.

### Position versus time

The position graph compares the numerical and analytical x and y position components:

figure;
plot(time, position(:, 1));
hold on;
plot(time, position(:, 2));
plot(time, analytical_position(:, 1), "--");
plot(time, analytical_position(:, 2), "--");

title("Numerical and Analytical Position vs. Time");
xlabel("Time (s)");
ylabel("Position (m)");

legend("Numerical x position", ...
       "Numerical y position", ...
       "Analytical x position", ...
       "Analytical y position");

grid on;
```

This graph shows how the object's position evolves over time and makes it possible to compare the numerical integration with the analytical reference.

### Velocity versus time

The velocity graph compares the numerical and analytical velocity components:

figure;
plot(time, velocity(:, 1));
hold on;
plot(time, velocity(:, 2));
plot(time, analytical_velocity(:, 1), "--");
plot(time, analytical_velocity(:, 2), "--");

title("Numerical and Analytical Velocity vs. Time");
xlabel("Time (s)");
ylabel("Velocity (m/s)");

legend("Numerical vx", ...
       "Numerical vy", ...
       "Analytical vx", ...
       "Analytical vy");

grid on;

This graph illustrates the evolution of both velocity components and allows the numerical and analytical results to be compared directly.

### Position error versus time

The position error graph displays the magnitude of the difference between the numerical and analytical position vectors:

figure;
plot(time, position_error);

title("Position Error vs. Time");
xlabel("Time (s)");
ylabel("Absolute Position Error (m)");

grid on;


It illustrates how the position error develops throughout the simulation.

### Velocity error versus time

The velocity error graph displays the magnitude of the difference between the numerical and analytical velocity vectors:

figure;
plot(time, velocity_error);

title("Velocity Error vs. Time");
xlabel("Time (s)");
ylabel("Absolute Velocity Error (m/s)");

grid on;

For the current constant-acceleration model, the velocity error should remain approximately zero, apart from floating-point rounding.

---

## Numerical Output

The prototype reports the initial state, applied forces, acceleration, final numerical state, analytical reference results, numerical errors, and simulation parameters.

The command-line output includes:

- Initial position
- Initial velocity
- Initial speed
- Force 1
- Force 2
- Net force
- Acceleration
- Final numerical position
- Final numerical velocity
- Final numerical speed
- Final analytical position
- Final analytical velocity
- Final analytical speed
- Final position error
- Final velocity error
- Maximum position error
- Maximum velocity error
- Total simulation time
- Time step
- Number of integration steps

Values are formatted with appropriate precision for readability.

For example:

fprintf("Initial velocity: [%.6f, %.6f] m/s\n", ...
        v_initial(1), v_initial(2));

fprintf("Final numerical velocity: [%.6f, %.6f] m/s\n", ...
        v_final(1), v_final(2));

fprintf("Final position error: %.9f m\n", ...
        final_position_error);

The prototype also creates a combined time-history matrix containing the time array, numerical position and velocity, numerical speed, acceleration, net force, analytical position and velocity, analytical speed, and position and velocity error magnitudes.

results = [time(:), position, velocity, speed, ...
           acceleration, net_force, ...
           analytical_position, analytical_velocity, ...
           analytical_speed, position_error, velocity_error];

disp("Time-history results:");
disp(results);

This table provides a consolidated record of the simulation and its analytical validation.

---

## Validation

The prototype's validation process covers both physical correctness and numerical accuracy.

The planned and implemented testing scope includes:

- Different masses
- Different initial positions
- Different initial velocities
- Different applied force combinations
- Balanced forces and zero net force
- Two-dimensional force configurations
- Positive and negative acceleration components
- Different timestep values
- Different total simulation times
- Numerical versus analytical position comparisons
- Numerical versus analytical velocity comparisons
- Position and velocity error calculations
- Graphical consistency with the stored simulation data
- Time-history table dimensions and column ordering
- Invalid input conditions

For constant acceleration, the analytical position and velocity provide independent reference values for comparison with the numerical results.

A balanced-force case can be used to verify the expected behaviour when:

   Fnet = 0

The resulting acceleration is:

   a = 0

The object's velocity therefore remains constant, and its position changes uniformly with time.

Timestep refinement tests can also demonstrate how the numerical position error changes as the timestep is reduced. The validation process should be completed by running the defined tests and comparing actual outputs with expected results.

---

## Current State

The prototype's v1.7 code represents the **core completed version** of the Newton's Second Law simulation.

It currently provides:

- Numerical 2D motion simulation
- Semi-implicit Euler integration
- Two-dimensional force calculations
- Position, velocity, acceleration, and speed tracking
- Analytical position and velocity calculations
- Numerical error analysis
- Numerical and analytical trajectory visualization
- Force and velocity vector visualization
- Position-versus-time analysis
- Velocity-versus-time analysis
- Position-error-versus-time analysis
- Velocity-error-versus-time analysis
- Detailed command-line output
- Combined numerical and analytical time-history data
- Input validation for supported simulation parameters

The prototype provides a foundation for further development within AstPIE. Potential extensions include variable forces, additional numerical integration methods such as Runge–Kutta methods, momentum and impulse calculations, and more advanced physical scenarios.

The analytical comparison currently applies to the constant-acceleration model and will need to be adapted or independently validated for future models involving variable forces or other more complex dynamics.