# Kepler's Second Law — 2D Orbital Simulation

## Overview

This GNU Octave prototype models the two-dimensional motion of a planet orbiting a fixed central star under Newtonian gravity. Its purpose is to demonstrate Kepler's Second Law: the line connecting a planet to its star sweeps out equal areas during equal time intervals.

The program accepts the star's mass, the planet's initial position and velocity, the integration time step, the requested simulation duration, and the measurement interval. It validates these inputs, calculates the analytical properties of the corresponding bound ellipse, integrates the trajectory using two numerical methods, and reports orbital measurements and numerical diagnostics.

**Velocity Verlet** is the primary integration method. **Semi-implicit Euler** is retained as a comparison method. Both methods use the same gravitational model and initial conditions.

The analytical orbital properties are calculated independently of the numerical integration. The program calculates an analytical reference trajectory, measures the swept area over individual time steps and equal-duration intervals, and compares the numerical results with the analytical orbital properties. It also evaluates position errors, specific angular momentum, specific orbital energy, and sensitivity to the integration time step.

This prototype is intended as a scientific and educational foundation for AstPIE. It demonstrates the relationship between orbital motion, angular momentum, and swept area. It is a simplified orbital model, not a general-purpose N-body simulator.

---

## Features

- User-defined central star mass.
- User-defined two-dimensional initial position and velocity.
- Input validation for masses, vectors, orbital conditions, time steps, simulation duration, and measurement interval.
- Newtonian gravitational acceleration from a fixed central star.
- Analytical specific orbital energy and specific angular momentum.
- Analytical eccentricity and orbital orientation.
- Analytical semi-major axis, semi-minor axis, focal distance, perihelion, and aphelion distances.
- Analytical reference positions calculated at the simulation timestamps.
- Numerical solution of Kepler's equation using bisection for eccentric elliptical orbits.
- Velocity Verlet as the default numerical integration method.
- Semi-implicit Euler as a comparison method.
- Stored position, velocity, and acceleration histories for both methods.
- Distance and speed histories.
- Specific angular momentum and areal velocity histories.
- Swept-area estimates for individual integration steps.
- Cumulative swept-area calculations.
- Equal-time-interval area measurements and average areal velocities.
- Numerical position-error calculations relative to the analytical reference trajectory.
- Numerical perihelion and aphelion estimates.
- Specific orbital energy and angular momentum conservation diagnostics.
- Comparison of integration methods using the same initial conditions.
- Timestep-sensitivity comparisons for both numerical methods.
- Command-window summaries, measurement tables, and scientific plots.
- Organised simulation outputs for testing and validation.

---

## Physical Model

### Coordinate system and bodies

Motion is represented in a two-dimensional Cartesian coordinate system. The central star remains fixed at the origin:

- Star position: `r_star = [0, 0]`.
- Planet position: `r = [x, y]`, measured in metres.
- Planet velocity: `v = [vx, vy]`, measured in metres per second.
- Planet acceleration: `a = [ax, ay]`, measured in metres per second squared.

The star is treated as the focus of the orbit. The simulation neglects the star's motion around the system barycentre and treats the planet as a test body moving in the star's gravitational field.

### Gravitational parameter

The gravitational constant is:

`G = 6.67430e-11 m^3 kg^-1 s^-2`

The gravitational parameter is calculated from the star's mass:

`mu = G * M_star`

Here, `M_star` is the central star's mass in kilograms, and `mu` is measured in cubic metres per second squared.

### Gravitational acceleration

The planet's distance from the star is:

`r_mag = sqrt(x^2 + y^2)`

The gravitational acceleration vector is:

`a = -(mu / r_mag^3) * r`

In component form:

`ax = -mu * x / r_mag^3`

`ay = -mu * y / r_mag^3`

The acceleration points towards the star and follows the inverse-square gravitational law.

The model includes no other planets, atmospheric drag, thrust, relativistic corrections, or other non-gravitational forces. The central star is treated as a point mass.

---

## Analytical Orbital Properties

The program derives the orbital properties from the initial position and velocity. These calculations are independent of the numerical trajectories produced by Velocity Verlet and semi-implicit Euler.

### Initial distance and speed squared

For the initial position `r0 = [x0, y0]` and initial velocity `v0 = [vx0, vy0]`:

`r0_mag = sqrt(x0^2 + y0^2)`

`v0_squared = vx0^2 + vy0^2`

The initial distance must be greater than zero because the gravitational acceleration is undefined at the position of the point-mass star.

### Specific orbital energy

The initial specific orbital energy is:

`epsilon = v0_squared / 2 - mu / r0_mag`

Specific orbital energy is energy per unit mass, measured in `m^2/s^2`.

For the ideal Kepler problem:

- `epsilon < 0` corresponds to a bound elliptical orbit when angular momentum is nonzero.
- `epsilon = 0` corresponds to the parabolic boundary.
- `epsilon > 0` corresponds to an unbound hyperbolic trajectory.

The prototype requires bound elliptical initial conditions.

### Specific angular momentum

The two-dimensional specific angular momentum is calculated as:

`h = x0 * vy0 - y0 * vx0`

Its units are `m^2/s`.

For motion under a central gravitational force, specific angular momentum is conserved in the continuous physical model. A nonzero value is required for a non-degenerate elliptical orbit.

The magnitude of the specific angular momentum is:

`h_mag = abs(h)`

### Eccentricity

The orbital eccentricity is calculated from specific energy and specific angular momentum:

`e_squared = 1 + (2 * epsilon * h^2) / mu^2`

`e = sqrt(e_squared)`

For an elliptical orbit:

`0 <= e < 1`

A circular orbit has eccentricity zero. Larger eccentricities describe increasingly elongated ellipses.

### Eccentricity vector and orbital orientation

The eccentricity vector identifies the direction of perihelion for a non-circular orbit.

The initial position-velocity dot product is:

`r_dot_v = x0 * vx0 + y0 * vy0`

The eccentricity vector is:

`e_vec = (((v0_squared - mu / r0_mag) * r0) - (r_dot_v * v0)) / mu`

Its components are:

`e_x = (((v0_squared - mu / r0_mag) * x0) - (r_dot_v * vx0)) / mu`

`e_y = (((v0_squared - mu / r0_mag) * y0) - (r_dot_v * vy0)) / mu`

For a non-circular orbit, the orientation angle is:

`omega = atan2(e_y, e_x)`

The angle specifies the direction of the eccentricity vector relative to the positive x-axis. For a circular orbit, the perihelion direction is undefined, so a reference orientation can be chosen without changing the circular geometry.

### Ellipse dimensions

The semi-major axis is:

`a = -mu / (2 * epsilon)`

The semi-minor axis is:

`b = a * sqrt(1 - e^2)`

The focal distance, measured from the ellipse centre to either focus, is:

`c = a * e`

The perihelion distance is:

`r_peri = a * (1 - e)`

The aphelion distance is:

`r_apo = a * (1 + e)`

These analytical properties define the size and shape of the orbit and provide reference values for the numerical simulation.

---

## Analytical Reference Trajectory

The prototype calculates reference positions at the same timestamps used by the numerical integration. This allows direct position-error comparisons rather than only comparing the numerical trajectory with a geometric ellipse.

### Mean motion

The mean motion of the orbit is:

`n = sqrt(mu / a^3)`

It is measured in radians per second.

### Mean anomaly

The mean anomaly evolves linearly with time:

`M(t) = M0 + n * t`

Here, `M0` is the initial mean anomaly, `n` is the mean motion, and `t` is the elapsed time.

### Kepler's equation

The eccentric anomaly `E` is related to mean anomaly by:

`M = E - e * sin(E)`

For an eccentric elliptical orbit, this equation generally cannot be rearranged into a simple elementary expression for `E`. The prototype solves it numerically using a bisection procedure.

For a circular orbit, where `e = 0`, the equation simplifies to:

`E = M`

### Position in the orbital plane

Once the eccentric anomaly is known, the position in coordinates aligned with the ellipse's major axis is:

`x_orbit = a * (cos(E) - e)`

`y_orbit = b * sin(E)`

The position is rotated through the orbital orientation angle `omega`:

`x_ref = x_orbit * cos(omega) - y_orbit * sin(omega)`

`y_ref = x_orbit * sin(omega) + y_orbit * cos(omega)`

The resulting reference position is expressed in the same star-centred coordinate system as the numerical trajectory.

The reference solution is calculated from the analytical Keplerian orbit rather than from either numerical integration method. It therefore provides an independent basis for evaluating numerical position errors, subject to the precision of the analytical calculations and numerical solution of Kepler's equation.

---

## Numerical Simulation and Time History

### Inputs and validation

The program requests:

| Input                | Meaning                            | Unit / format              |
|----------------------|------------------------------------|----------------------------|
| `star_mass`          | Central star mass                  | Positive finite scalar, kg |
| `r0_planet`          | Initial planet position            | Two-component vector, m    |
| `v0_planet`          | Initial planet velocity            | Two-component vector, m/s  |
| `dt`                 | Integration time step              | Positive finite scalar, s  |
| `T`                  | Requested simulation duration      | Positive duration, s       |
| Measurement interval | Duration between area measurements | Positive duration, s       |

The program checks that the input values have valid dimensions and numerical values. It also checks that the initial conditions correspond to a bound elliptical orbit with nonzero initial distance and angular momentum.

The requested duration must be compatible with the integration time step. The measurement interval must also be compatible with the integration time step so that the interval measurements align with stored numerical states.

The number of integration steps is determined from the relationship between the simulation duration and the time step:

`num_steps = T / dt`

The time history includes the initial state at time zero and every subsequent integration state:

`time_history = (0:num_steps)' * dt`

Consequently, the number of stored states is:

`number_of_states = num_steps + 1`

### Stored arrays

The primary Velocity Verlet histories use names such as:

- `planet_positions`
- `planet_velocities`
- `planet_accelerations`

The semi-implicit Euler comparison histories use:

- `euler_planet_positions`
- `euler_planet_velocities`
- `euler_planet_accelerations`

The program also stores derived quantities such as distance, speed, specific angular momentum, areal velocity, swept area, cumulative area, and position error.

Each history corresponds to the simulation timestamps, allowing the quantities to be plotted and compared over the same duration.

---

## Semi-Implicit Euler Integration

Semi-implicit Euler is retained as a comparison method. It calculates acceleration from the current position, updates velocity, and then advances the position using the updated velocity.

The velocity update is:

`v_next = v + a * dt`

The position update is:

`r_next = r + v_next * dt`

The gravitational acceleration at the updated position is:

`a_next = -(mu / |r_next|^3) * r_next`

This update order distinguishes semi-implicit Euler from explicit Euler, which uses the old velocity to update position.

Semi-implicit Euler is relatively simple, but its numerical accuracy depends on the selected time step and simulation duration. The prototype uses it as a comparison method rather than assuming that it reproduces the analytical orbit exactly.

---

## Velocity Verlet Integration

Velocity Verlet is the default numerical integration method.

### Position update

The next position is calculated from the current position, velocity, and acceleration:

`r_next = r + v * dt + 0.5 * a * dt^2`

### Acceleration update

The gravitational acceleration is recalculated at the new position:

`a_next = -(mu / |r_next|^3) * r_next`

### Velocity update

The next velocity is calculated using the average of the current and next accelerations:

`v_next = v + 0.5 * (a + a_next) * dt`

The new position, velocity, and acceleration are stored in their respective histories.

Velocity Verlet is a second-order numerical integration method for suitable position-dependent forces. It often exhibits useful long-term behaviour in conservative systems, but it is not exact. Its accuracy depends on the time step, initial conditions, simulation duration, and numerical implementation.

The prototype compares its results with semi-implicit Euler and the analytical reference solution. It does not assume in advance that Velocity Verlet must be more accurate in every possible configuration.

---

## Kepler's Second Law and Swept-Area Measurements

### Radius vector

The radius vector points from the central star to the planet:

`r = [x, y]`

Its magnitude is the planet's distance from the star:

`r_mag = sqrt(x^2 + y^2)`

As the planet moves, the radius vector changes direction and length, sweeping out an area in the orbital plane.

### Areal velocity

Areal velocity describes the rate at which the radius vector sweeps out area:

`dA/dt = 0.5 * |r x v|`

For planar motion, this becomes:

`dA/dt = 0.5 * abs(x * vy - y * vx)`

Since the specific angular momentum is:

`h = x * vy - y * vx`

the areal velocity can also be expressed as:

`dA/dt = abs(h) / 2`

Under the ideal central gravitational force, specific angular momentum is conserved. Therefore, the areal velocity is constant, which is the mathematical basis of Kepler's Second Law.

### Swept area during one time step

The program estimates the area swept between consecutive position vectors using the triangle formed by the origin and the two positions.

For positions `r_i = [x_i, y_i]` and `r_next = [x_next, y_next]`:

`delta_A_i = 0.5 * abs(x_i * y_next - y_i * x_next)`

This is the area of the triangle formed by the two radius vectors. For a curved orbit segment, it is a numerical approximation to the actual swept area over the time step.

The approximation generally improves as the time step becomes smaller, provided the numerical trajectory remains well resolved.

### Cumulative swept area

The cumulative swept area is calculated by summing the individual step areas:

`A_k = sum(delta_A_i)` for all steps before state `k`

Equivalently:

`A_k = A_(k-1) + delta_A_(k-1)`

with the initial cumulative area set to zero.

This history shows how the total swept area increases over time.

### Equal-time-interval measurements

For a measurement interval containing several integration steps, the program sums the swept areas within that interval:

`delta_A_interval = sum(delta_A_i)` over the interval

The average areal velocity over the interval is:

`average_areal_velocity = delta_A_interval / measurement_interval`

The program compares these measurements across intervals of equal duration.

For an ideal Keplerian orbit, equal-duration intervals should have equal swept areas and equal average areal velocities. Numerical differences can arise from the area approximation and the integration method.

### Interpreting the orbital motion

A planet in an elliptical orbit moves faster near perihelion and slower near aphelion. The radius vector is shorter near perihelion and longer near aphelion.

The relationship between distance and orbital speed ensures that the swept area per unit time remains constant in the ideal model. Therefore, equal areas are swept during equal time intervals even though the planet's speed and distance from the star vary throughout the orbit.

---

## Numerical Diagnostics

### Distance and speed

At every stored timestamp, the distance from the star is:

`r_mag = sqrt(x^2 + y^2)`

The speed is:

`speed = sqrt(vx^2 + vy^2)`

Distance is measured in metres and speed in metres per second.

### Position error against the analytical reference

The program compares each numerical position with the analytical reference position at the same timestamp.

The Euclidean position error is:

`position_error = sqrt((x_num - x_ref)^2 + (y_num - y_ref)^2)`

The program calculates position errors for Velocity Verlet and semi-implicit Euler.

It can then determine the maximum position error:

`maximum_position_error = max(position_error_history)`

and the final position error:

`final_position_error = position_error_history(end)`

These measures describe how far the numerical trajectory deviates from the time-matched analytical reference during the selected simulation.

### Specific angular momentum conservation

The specific angular momentum at each timestamp is:

`h_i = x_i * vy_i - y_i * vx_i`

Its signed change relative to the initial value is:

`delta_h_i = h_i - h_0`

The absolute relative error is:

`relative_h_error_i = abs(h_i - h_0) / abs(h_0)`

When expressed as a percentage:

`relative_h_error_percent_i = 100 * relative_h_error_i`

The maximum relative angular momentum error is:

`maximum_relative_h_error_percent = max(relative_h_error_percent_history)`

This diagnostic evaluates how well the numerical method preserves angular momentum throughout the run.

### Specific orbital energy conservation

Specific orbital energy is calculated from the instantaneous speed and distance:

`epsilon_i = speed_i^2 / 2 - mu / r_mag_i`

The signed energy change relative to the initial value is:

`delta_epsilon_i = epsilon_i - epsilon_0`

The absolute relative energy error is:

`relative_energy_error_i = abs(epsilon_i - epsilon_0) / abs(epsilon_0)`

When expressed as a percentage:

`relative_energy_error_percent_i = 100 * relative_energy_error_i`

The maximum relative energy error is:

`maximum_relative_energy_error_percent = max(relative_energy_error_percent_history)`

Energy and angular momentum diagnostics help evaluate numerical behaviour. Neither diagnostic alone proves that the entire trajectory is accurate, so both should be interpreted alongside the position errors and timestep comparisons.

### Numerical perihelion and aphelion

The numerical minimum and maximum distances are determined from the simulated distance history:

`numerical_perihelion = min(distance_history)`

`numerical_aphelion = max(distance_history)`

These are compared with the analytical values:

`analytical_perihelion = a * (1 - e)`

`analytical_aphelion = a * (1 + e)`

The signed differences are:

`perihelion_error = numerical_perihelion - analytical_perihelion`

`aphelion_error = numerical_aphelion - analytical_aphelion`

The numerical extrema depend on the simulated time interval and the sampling resolution. If the simulation does not include perihelion or aphelion, the observed minimum or maximum distance may not equal the corresponding analytical orbital extremum.

### Timestep comparison

The prototype compares the two numerical integration methods at multiple time steps derived from the selected time step. Only time steps compatible with the total simulation duration are retained.

For each selected time step, the program calculates position-error measures for Velocity Verlet and semi-implicit Euler relative to the analytical reference.

The comparison is intended to show how numerical error responds to changes in time-step size. Smaller time steps often improve accuracy, but the result depends on the orbit, the integration method, and the measured quantity. Velocity Verlet is not assumed to be exact or universally superior.

---

## Graphs and Visualisation

The program generates scientific plots to help interpret the orbital trajectory and the numerical results.

### Orbital trajectory comparison

The trajectory plot shows the analytical ellipse and the numerically calculated trajectories. It identifies the star at the origin and displays the orbit in a coordinate system with equal horizontal and vertical scaling.

### Distance and speed histories

The distance plot shows how the planet's distance from the star changes over time. The speed plot shows the corresponding variation in orbital speed.

Together, these plots help demonstrate that the planet moves faster near perihelion and slower near aphelion.

### Position-error plots

The position-error plots show the differences between the numerical positions and the time-matched analytical reference positions.

These plots help identify how errors accumulate during the simulation and how the two integration methods behave under the same initial conditions.

### Coordinate histories

The x-position and y-position plots display the planet's coordinate components as functions of time.

These histories make it possible to inspect the periodic orbital motion and compare the timing of different orbital phases.

### Swept-area and areal-velocity measurements

The program calculates swept areas over individual time steps and equal-duration measurement intervals. The corresponding tables and plots can be used to examine how closely the numerical orbit follows the equal-area relationship.

### Timestep comparison

The timestep comparison plots show how the position-error measures vary with integration time step for both numerical methods.

### Orbital parameter summary

The summary output presents the analytical orbital properties, including eccentricity, ellipse dimensions, focal distance, perihelion, and aphelion. Numerical diagnostics provide additional context for interpreting the calculated orbit.

---

## Example Input

The following baseline input can be used to test a bound elliptical orbit:

| Prompt 	            | Example value  |
|---------------------------|----------------|
| Star mass (kg) 	    | `1.98847e30`   |
| Initial position (m)      | `[1.496e11 0]` |
| Initial velocity (m/s)    | `[0 20000]`    |
| Integration time step (s) | `3600`         |
| Simulation duration (s)   | `31536000`     |
| Measurement interval (s)  | `86400`        |

This example uses a one-hour integration time step, a 365-day simulation duration, and one-day measurement intervals.

The initial velocity is below the circular-orbit speed at the specified initial distance, producing a bound elliptical orbit with the initial position at an apsis. The actual numerical errors depend on the integration method, time step, and duration.

---

## Numerical Output

The command-window output is intended to provide the information needed to understand and validate a simulation run.

It includes the input parameters, the initial and final numerical states, analytical orbital properties, and the calculated orbital measurements.

The numerical summaries include:

- Star mass and gravitational parameter.
- Initial position and velocity.
- Integration time step and simulation duration.
- Analytical specific orbital energy and angular momentum.
- Eccentricity and orbital geometry.
- Analytical perihelion and aphelion distances.
- Numerical position and velocity histories.
- Distance, speed, and position-error summaries.
- Specific angular momentum and areal velocity.
- Swept-area and cumulative-area measurements.
- Equal-time-interval swept areas and average areal velocities.
- Numerical perihelion and aphelion estimates.
- Energy and angular momentum conservation diagnostics.
- Position-error comparisons between the numerical methods.
- Timestep-sensitivity results.

The exact values depend on the input parameters and the numerical integration settings.

---

## Validation and Testing

The prototype should be tested against physical expectations, analytical calculations, and numerical consistency checks.

### Orbital test cases

Recommended test cases include:

- A circular orbit with eccentricity approximately zero.
- A moderately eccentric bound ellipse.
- A highly eccentric but still bound ellipse.
- An orbit simulated for less than one complete revolution.
- A case with a smaller integration time step to examine numerical convergence.
- A case with different equal-duration measurement intervals.

For each case, record the inputs, expected results, actual results, units, tolerances, and pass/fail outcome.

### Input-validation tests

Input-validation tests should cover:

- Zero or negative star mass.
- Non-finite or non-real mass values.
- Initial position vectors with the wrong number of components.
- Initial velocity vectors with the wrong number of components.
- Non-finite vector components.
- Zero initial distance.
- Zero initial angular momentum.
- Unbound initial conditions.
- Zero or negative integration time step.
- Zero or negative simulation duration.
- Invalid measurement intervals.
- Simulation durations or measurement intervals incompatible with the time step.

For rejected inputs, record the expected error message or rejection behaviour.

### Physical and numerical checks

The following checks are useful for validating the implementation:

- Verify the gravitational acceleration points towards the star.
- Verify the analytical eccentricity and orbital dimensions against independent calculations.
- Verify that the circular-orbit case has constant analytical radius.
- Verify that equal-time analytical swept areas are equal within the chosen numerical tolerance.
- Check that the computed angular momentum remains close to its initial value.
- Check specific orbital energy variation for both integration methods.
- Confirm that all stored histories align with the time array.
- Confirm that the number of stored states is one greater than the number of integration steps.
- Compare position errors at the same timestamps.
- Repeat runs with smaller time steps while holding the physical initial conditions and total duration fixed.
- Check that a partial-orbit run does not incorrectly treat its observed minimum and maximum distances as the true perihelion and aphelion.

The v1.7 testing plan defines the intended validation cases. These should be recorded as repeatable tests rather than treating the presence of diagnostic calculations as proof that every case has passed.

---

## Assumptions and Limitations

The prototype has the following assumptions and limitations:

- The system contains one fixed central star and one orbiting planet.
- Motion is restricted to two dimensions.
- The star is stationary at the origin.
- The gravitational field is Newtonian, and the star is treated as a point mass.
- The model neglects the motion of the star around the system barycentre.
- Other planets and non-gravitational forces are not included.
- The accepted initial conditions describe bound elliptical orbits with nonzero specific angular momentum.
- The analytical reference trajectory depends on the calculated orbital parameters and the numerical solution of Kepler's equation.
- The numerical trajectories are approximations and depend on the integration time step.
- The swept area over an individual time step is estimated using the triangle formed by consecutive radius vectors.
- The numerical swept-area measurements can therefore differ slightly from the continuous analytical swept areas.
- Energy and angular momentum conservation diagnostics measure numerical behaviour but do not independently prove trajectory accuracy.
- A numerical minimum or maximum distance is not necessarily the true perihelion or aphelion if the simulation does not cover the corresponding orbital phase.
- Timestep comparisons only cover the time steps that are compatible with the selected total duration.
- The prototype is not a general-purpose N-body simulator and does not model every physical effect present in real planetary systems.

---

## Version Development

The prototype's development is organised into eight milestones.

- **v1.0 — Core Elliptical Orbit Prototype:** Established the initial orbital model, gravitational acceleration, numerical integration, and basic trajectory visualisation.
- **v1.1 — User Inputs and Simulation Results:** Added configurable initial conditions and simulation parameters, together with input validation and command-window results.
- **v1.2 — Orbital Geometry and Analytical Properties:** Added the analytical orbital elements and geometric description of the ellipse.
- **v1.3 — Time History and Orbital Measurements:** Added stored numerical histories and the calculations needed to measure orbital quantities over time.
- **v1.4 — Visualisation and Graphs:** Added scientific plots to inspect the trajectory and the changing orbital quantities.
- **v1.5 — Analytical Reference and Error Calculations:** Added time-matched analytical reference positions and numerical error comparisons.
- **v1.6 — Velocity Verlet Integration and Numerical Method Comparison:** Established Velocity Verlet as the default method and retained semi-implicit Euler for comparison, including timestep-sensitivity analysis.
- **v1.7 — Testing and Validation Suite:** Defined repeatable test cases for orbital geometry, numerical behaviour, invalid inputs, and equal-area measurements.
- **v1.8 — Core Simulation Complete:** Consolidated the prototype's calculations, numerical methods, measurements, diagnostics, and visualisation into a coherent baseline for further development.

A version should be marked complete only when its implementation and validation criteria have been checked. A written test plan is not equivalent to an automated test suite, and a diagnostic calculation is not equivalent to a verified passing test.

---

## Current State and Future Development

The current prototype combines analytical orbital calculations, numerical integration, swept-area measurements, and comparison between two numerical methods. It provides a foundation for demonstrating Kepler's Second Law and examining how numerical methods reproduce ideal orbital behaviour.

Potential future improvements include:

- Adding an automated test runner with independent reference values and defined tolerances.
- Expanding the timestep-convergence tests.
- Recording reproducible test outputs for representative circular and eccentric orbits.
- Improving the accuracy assessment of swept areas by comparing the numerical estimates with analytical swept areas over matching time intervals.
- Separating reusable physical calculations, integration routines, validation tests, and plotting into appropriate modules as the project grows.
- Integrating the validated calculations into the AstPIE educational platform.

Future versions should preserve the established physical assumptions, document any changes to the equations or numerical methods, and include tests that distinguish physical behaviour from numerical integration error.

The v1.8 prototype serves as the baseline for these improvements.