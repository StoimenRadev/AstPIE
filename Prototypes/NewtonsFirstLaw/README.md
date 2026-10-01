# Newton's First Law — 2D Simulation

## Overview

This prototype is a numerical 2D simulation demonstrating Newton's First Law of Motion through the motion of an object subjected to external forces.

The simulation accepts initial physical conditions and two applied forces, calculates the resulting motion, and provides both numerical and graphical representations of the results.

The implementation was developed and validated incrementally, with each stage tested against the expected physical behavior.

---

## Physical Model

- The simulation represents the object using:

* Mass
* 2D position
* 2D velocity
* 2D acceleration
* Two applied 2D forces
* Time step
* Total simulation time

- The net force is calculated from the vector sum of the applied forces:
  Fnet = F1 + F2;

- Acceleration is calculated from the net force and mass:
  a = Fnet / mass;

- Velocity is updated at each timestep:
  v_final = v + a * dt;

- The object's position is then updated using the calculated velocity:
  x_final = x + v_final * dt;

The simulation stores the state at every timestep, allowing the complete motion to be analysed after the calculation.

---

## Numerical Simulation

- The simulation maintains separate arrays for position, velocity, and acceleration:

  position = zeros(length(time), 2);
  velocity = zeros(length(time), 2);
  acceleration = zeros(length(time), 2);

Each row corresponds to one simulation timestep, while the two columns represent the x and y components.

- The simulation loop updates the object's state and stores the resulting values:

  for i = 2:length(time)

      a = Fnet / mass;

      v_final = v + a * dt;
      x_final = x + v_final * dt;

      v = v_final;
      x = x_final;

      position(i, :) = x_final;
      velocity(i, :) = v_final;
      acceleration(i, :) = a;

  end

This provides a complete numerical history of the simulated motion rather than only the final state.

---

## Vector Visualization

- The simulation visualizes the object's trajectory together with the applied forces, net force, and final velocity.

  plot(position(:, 1), position(:, 2));
  hold on;

  plot(x_initial(1), x_initial(2), "o");
  plot(x_final(1), x_final(2), "o");

  quiver(x_final(1), x_final(2), F1(1), F1(2), ...
         0, 'r', 'linewidth', 1.5);

  quiver(x_final(1), x_final(2), F2(1), F2(2), ...
         0, 'b', 'linewidth', 1.5);

  quiver(x_final(1), x_final(2), Fnet(1), Fnet(2), ...
         0, 'g', 'linewidth', 2);

  quiver(x_final(1), x_final(2), v_final(1), v_final(2), ...
         0, 'y', 'linewidth', 2);

- The visualization therefore provides a direct graphical representation of:

* Object trajectory
* Initial position
* Final position
* Force 1
* Force 2
* Net force
* Final velocity

Vector labels are positioned along their corresponding vectors and adjusted to remain readable for different vector directions.

---

## Velocity and Speed

- Velocity is represented as a two-dimensional vector:

  v_final = [vx, vy];

- The speed is calculated separately as the magnitude of the velocity vector:

  speed_final = sqrt(v_final(1)^2 + v_final(2)^2);

This distinction allows the simulation to preserve directional information while also providing a scalar speed value.
Both initial and final speeds are calculated and reported.

---

## Position and Velocity Analysis

The stored simulation data is used to generate time-dependent graphs.

- Position:

  figure;
  plot(time, position(:, 1));
  hold on;
  plot(time, position(:, 2));

  grid on;
  xlabel("Time (s)");
  ylabel("Position (m)");
  title("Position vs. Time");
  legend("x position", "y position");

This graph shows how the x and y components of the object's position evolve throughout the simulation.

- Velocity:

  figure;
  plot(time, velocity(:, 1));
  hold on;
  plot(time, velocity(:, 2));

  grid on;
  xlabel("Time (s)");
  ylabel("Velocity (m/s)");
  title("Velocity vs. Time");
  legend("x velocity", "y velocity");

This provides a direct representation of the object's velocity components over time.

---

## Numerical Output

- The command-line output reports the main physical and simulation parameters, including:

* Initial position
* Initial velocity
* Initial speed
* Force 1
* Force 2
* Net force
* Acceleration
* Final position
* Final velocity
* Final speed
* Total simulation time
* Number of simulation steps

- Example formatting:

  fprintf("Initial velocity: [%.3f, %.3f] m/s\n", ...
          v_initial(1), v_initial(2));

  fprintf("Final velocity: [%.3f, %.3f] m/s\n", ...
          v_final(1), v_final(2));

  fprintf("Final speed: %.3f m/s\n", speed_final);


---

## Validation

- The simulation was validated using multiple sets of physical and numerical conditions, including:

* Different masses
* Different initial positions
* Different initial velocities
* Different force combinations
* Balanced forces
* Different timestep values
* Different total simulation times
* Different vector directions
* Edge and invalid-input cases

The calculated acceleration, velocity, and position were compared with their expected values. The graphical trajectory and time-dependent graphs were also checked against the numerical results.

- A balanced-force case was specifically used to verify Newton's First Law:

  Fnet = F1 + F2;

When:

  Fnet = [0, 0] N

the resulting acceleration is:

  a = [0, 0] m/s²

and the object's velocity remains constant.

---

## Current State

The prototype represents the **core completed version** of the simulation.

- It currently provides:

* Numerical 2D motion simulation
* Vector-based force calculations
* Trajectory visualization
* Force and velocity visualization
* Speed calculation
* Position-time analysis
* Velocity-time analysis
* Numerical output
* Physics and numerical validation

The prototype serves as the validated foundation for future development of the simulation within AstPIE.
