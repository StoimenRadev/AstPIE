# Kepler's Third Law — 2D Orbital Simulation

## Overview

This GNU Octave prototype calculates and simulates the two-dimensional orbital motion of a planet relative to a central star under Newtonian gravity. Its primary purpose is to demonstrate Kepler's Third Law by calculating the orbital period of a two-body system from its total mass and the semi-major axis of its orbit.

The program accepts the star's mass, the planet's mass, the planet's initial position and velocity, the integration time step, and the requested simulation duration. It validates these inputs, calculates the analytical properties of the resulting bound elliptical orbit, generates an analytical reference trajectory, and integrates the orbital motion using two numerical methods.

**Velocity Verlet** is the default numerical integration method. **Semi-Implicit Euler** is retained as a comparison method. Both methods use the same initial conditions and gravitational parameter.

The analytical calculations are performed independently of the numerical integration. The program determines the orbital energy, angular momentum, eccentricity, ellipse dimensions, orbital orientation, and analytical orbital period. It then calculates numerical trajectories and compares their positions, orbital periods, energy, and angular momentum against the analytical reference values.

The prototype also generates scientific graphs, numerical histories, orbital parameter summaries, and error measurements to help evaluate the accuracy and behaviour of the numerical methods.

This prototype is intended as a scientific and educational foundation for AstPIE. It demonstrates the relationship between the physical properties of a two-body system and its orbital period. It is a simplified Newtonian orbital model, not a general-purpose N-body simulator.

---

## Features

- User-defined central star mass.
- User-defined orbiting planet mass.
- User-defined two-dimensional initial position and velocity.
- Input validation for masses, position vectors, velocity vectors, orbital conditions, time steps, and simulation duration.
- Newtonian two-body gravitational parameter calculated using the combined mass of both bodies.
- Initial distance and speed calculations.
- Initial specific orbital energy calculation.
- Initial specific angular momentum calculation.
- Analytical semi-major axis calculation.
- Analytical semi-minor axis calculation.
- Analytical eccentricity calculation.
- Analytical eccentricity vector and orbital orientation calculation.
- Analytical focal distance calculation.
- Analytical periapsis and apoapsis distance calculations.
- Analytical orbital-period calculation using Kepler's Third Law.
- Analytical mean-motion calculation.
- Analytical reference trajectories for circular and elliptical orbits.
- Numerical solution of Kepler's equation using the bisection method for eccentric elliptical orbits.
- Velocity Verlet as the default numerical integration method.
- Semi-Implicit Euler as a comparison method.
- Stored position, velocity, and acceleration histories for both numerical methods.
- Distance and speed histories.
- Specific orbital energy histories.
- Specific angular momentum histories.
- Numerical orbital-period measurement using accumulated angular progress.
- Interpolation of numerical orbital-period measurements between consecutive timesteps.
- Absolute, relative, and percentage orbital-period error calculations.
- Numerical position-error calculations relative to the analytical reference trajectory.
- Initial, final, and maximum position-error measurements.
- Numerical periapsis and apoapsis distance estimates.
- Specific orbital energy and angular momentum conservation diagnostics.
- Complete numerical time-history tables for both integration methods.
- Analytical and numerical orbital trajectory comparison.
- Graphical comparison of orbital distance, energy, angular momentum, position errors, and orbital periods.
- Graphical summary of the main analytical orbital parameters.
- Reproducible simulation inputs and numerical calculations for testing and validation.

---

## Physical Model

### Coordinate system and bodies

The simulation represents the relative motion of two bodies in a two-dimensional Cartesian coordinate system.

The position vector describes the location of the planet relative to the star:

`r = [x, y]`

The velocity vector describes the relative velocity:

`v = [vx, vy]`

The acceleration vector describes the relative gravitational acceleration:

`a = [ax, ay]`

The corresponding units are:

| Quantity     | Symbol     | Unit  |
|--------------|------------|-------|
| Position     | `r`        | m     |
| Velocity     | `v`        | m/s   |
| Acceleration | `a`        | m/s^2 |
| Star mass    | `M_star`   | kg    |
| Planet mass  | `M_planet` | kg    |
| Time         | `t`        | s     |

The central star and orbiting planet are treated as point masses. Their gravitational interaction determines the relative orbital motion.

The simulation uses a star-centred relative coordinate system. It does not separately calculate the positions of both bodies around their barycentre.

### Gravitational constant

The universal gravitational constant is:

`G = 6.67430e-11 m^3 kg^-1 s^-2`

It determines the strength of the Newtonian gravitational interaction between the two bodies.

### Gravitational parameter

For the relative two-body problem, the gravitational parameter is calculated using the combined mass of the star and planet:

`mu = G * (M_star + M_planet)`

Here:

- `M_star` is the central star's mass in kilograms.
- `M_planet` is the orbiting planet's mass in kilograms.
- `mu` is the gravitational parameter in cubic metres per second squared.

Including both masses accounts for the gravitational interaction between the two bodies in the relative-motion equation.

### Gravitational acceleration

The magnitude of the relative position vector is:

`r_mag = sqrt(x^2 + y^2)`

The relative gravitational acceleration is:

`a = -(mu / r_mag^3) * r`

In component form:

`ax = -mu * x / r_mag^3`

`ay = -mu * y / r_mag^3`

The negative sign indicates that the acceleration points towards the central gravitational focus.

The acceleration follows the inverse-square gravitational law.

The model neglects additional orbiting bodies, atmospheric drag, thrust, relativistic corrections, and other non-gravitational forces.

---

## Analytical Orbital Properties

The prototype derives the orbital parameters from the initial position and velocity. These calculations are independent of the numerical trajectories generated by Velocity Verlet and Semi-Implicit Euler.

### Initial distance and speed

For the initial position:

`r0 = [x0, y0]`

and initial velocity:

`v0 = [vx0, vy0]`

the initial distance is:

`r0_mag = sqrt(x0^2 + y0^2)`

The initial speed is:

`v0_speed = sqrt(vx0^2 + vy0^2)`

The initial distance must be greater than zero because the gravitational acceleration is undefined at the location of the point-mass gravitational focus.

### Specific orbital energy

The initial specific orbital energy is calculated as:

`epsilon = (v0_speed^2 / 2) - (mu / r0_mag)`

Specific orbital energy represents orbital energy per unit mass and is measured in joules per kilogram:

`J/kg`

For the ideal Newtonian two-body problem:

- `epsilon < 0` describes a bound elliptical orbit when the specific angular momentum is nonzero.
- `epsilon = 0` corresponds to the parabolic boundary.
- `epsilon > 0` describes an unbound hyperbolic trajectory.

The prototype accepts only bound elliptical initial conditions.

### Specific angular momentum

The two-dimensional specific angular momentum is calculated using the initial position and velocity:

`h = x0 * vy0 - y0 * vx0`

Its unit is:

`m^2/s`

Specific angular momentum describes the rotational motion of the orbiting body relative to the gravitational focus.

For motion under an ideal central gravitational force, specific angular momentum is conserved.

A nonzero initial value is required by the prototype to avoid degenerate radial motion.

### Semi-major axis

The semi-major axis describes half of the longest diameter of the orbital ellipse.

It is calculated from the gravitational parameter and specific orbital energy:

`a = -mu / (2 * epsilon)`

Its unit is metres.

The semi-major axis determines the characteristic size of the orbit and is one of the principal parameters in Kepler's Third Law.

### Eccentricity

The eccentricity determines the shape of the orbit.

The squared eccentricity is calculated using specific orbital energy and specific angular momentum:

`e_squared = 1 + (2 * epsilon * h^2) / mu^2`

The eccentricity is:

`e = sqrt(e_squared)`

For a bound elliptical orbit:

`0 <= e < 1`

The corresponding orbital shapes are:

- `e = 0`: circular orbit.
- `0 < e < 1`: elliptical orbit.

The prototype includes a small numerical tolerance when evaluating the squared eccentricity to account for floating-point rounding errors close to zero.

### Semi-minor axis

The semi-minor axis describes half of the shortest diameter of the orbital ellipse.

It is calculated as:

`b = a * sqrt(1 - e^2)`

Its unit is metres.

### Focal distance

The focal distance describes the distance between the centre of the ellipse and either of its two foci.

It is calculated as:

`c = a * e`

Its unit is metres.

For a circular orbit, the focal distance is zero.

### Periapsis and apoapsis distances

The periapsis distance is the minimum distance between the orbiting body and the gravitational focus.

It is calculated as:

`r_peri = a * (1 - e)`

The apoapsis distance is the maximum distance between the orbiting body and the gravitational focus.

It is calculated as:

`r_apo = a * (1 + e)`

Both distances are measured in metres.

These parameters describe the closest and farthest points of the analytical elliptical orbit.

### Eccentricity vector

The eccentricity vector determines the direction and magnitude of the orbital eccentricity.

The initial position-velocity dot product is:

`r_dot_v = x0 * vx0 + y0 * vy0`

The eccentricity vector is calculated as:

`e_vec = (((v0_speed^2 - mu / r0_mag) * r0) - (r_dot_v * v0)) / mu`

Its components are:

`e_x = (((v0_speed^2 - mu / r0_mag) * x0) - (r_dot_v * vx0)) / mu`

`e_y = (((v0_speed^2 - mu / r0_mag) * y0) - (r_dot_v * vy0)) / mu`

The eccentricity vector points towards periapsis for a non-circular orbit.

### Orbital orientation

The orientation of the ellipse is determined from the eccentricity vector:

`omega = atan2(e_y, e_x)`

The resulting angle describes the orientation of the periapsis direction relative to the positive x-axis.

For a circular orbit, the eccentricity vector is approximately zero and the periapsis direction is undefined. The prototype therefore uses the initial position direction as a reference orientation for the circular case.

---

## Kepler's Third Law and Analytical Orbital Period

### Analytical orbital period

Kepler's Third Law relates the orbital period of a two-body system to the semi-major axis and the total mass of the system.

The Newtonian two-body formulation is:

`T^2 = (4 * pi^2 * a^3) / (G * (M_star + M_planet))`

Using the gravitational parameter:

`mu = G * (M_star + M_planet)`

the equation becomes:

`T^2 = (4 * pi^2 * a^3) / mu`

Rearranging the equation to calculate the orbital period directly:

`T = sqrt((4 * pi^2 * a^3) / mu)`

The prototype implements the calculation as:

`orbital_period = 2 * pi * sqrt(a^3 / mu)`

The calculated orbital period is expressed in seconds.

It is also converted into days:

`orbital_period_days = orbital_period / 86400`

and years:

`orbital_period_years = orbital_period / (365.25 * 86400)`

The calculation uses the total mass of the two-body system rather than considering only the central star's mass.

The orbital period is calculated analytically from the initial conditions and the resulting semi-major axis. It does not depend on the selected numerical integration method or timestep.

### Mean motion

Mean motion describes the average angular progression of the orbit.

It is calculated as:

`n = sqrt(mu / a^3)`

Its unit is radians per second.

Mean motion is used when generating the analytical reference trajectory.

### Relationship between orbital period and semi-major axis

For a given two-body system, the orbital period increases with the semi-major axis.

The relationship can be expressed as:

`T^2 proportional to a^3`

This means that orbits with larger semi-major axes have longer orbital periods when the total system mass remains constant.

The relationship is the central physical principle demonstrated by this prototype.

---

## Analytical Reference Trajectory

The prototype generates analytical reference positions at the same timestamps used by the numerical integration methods.

This provides an independent reference for evaluating the numerical trajectories.

### Initial eccentric anomaly

For an eccentric elliptical orbit, the initial eccentric anomaly is calculated using the initial distance, semi-major axis, eccentricity, and radial motion.

The cosine component is:

`cos_E0 = (1 - r0_mag / a) / e`

The sine component is:

`sin_E0 = (r0 dot v0) / (e * sqrt(mu * a))`

The calculated components are restricted to the interval `[-1, 1]` to protect against small floating-point errors.

The initial eccentric anomaly is:

`E0 = atan2(sin_E0, cos_E0)`

### Initial mean anomaly

The initial mean anomaly is calculated as:

`M0 = E0 - e * sin(E0)`

It establishes the initial orbital phase for the analytical reference trajectory.

### Mean anomaly over time

Mean anomaly evolves linearly with time:

`M(t) = M0 + n * t`

The prototype restricts the calculated mean anomaly to one revolution using modulo arithmetic:

`M = mod(M0 + n * t, 2 * pi)`

### Kepler's equation

The eccentric anomaly and mean anomaly are related through Kepler's equation:

`M = E - e * sin(E)`

For an eccentric elliptical orbit, the equation is solved numerically using the bisection method.

The prototype initially defines the interval:

`E_low = 0`

`E_high = 2 * pi`

It then repeatedly evaluates the midpoint and compares the resulting mean anomaly with the required value.

The procedure performs 60 iterations before accepting the midpoint as the calculated eccentric anomaly.

### Position in the orbital coordinate system

Once the eccentric anomaly is known, the analytical position is calculated in a coordinate system aligned with the major axis of the ellipse.

The x-coordinate is:

`x_local = a * (cos(E) - e)`

The y-coordinate is:

`y_local = a * sqrt(1 - e^2) * sin(E)`

Using the semi-minor axis, the y-coordinate can also be expressed as:

`y_local = b * sin(E)`

### Rotation into the global coordinate system

The position is rotated using the orbital orientation angle:

`x_ref = x_local * cos(omega) - y_local * sin(omega)`

`y_ref = x_local * sin(omega) + y_local * cos(omega)`

The resulting coordinates describe the analytical reference position relative to the central star.

### Circular analytical orbit

For a circular orbit, the eccentricity is approximately zero.

The analytical reference position is calculated directly from the initial angular position and mean motion.

The initial angular position is:

`theta0 = atan2(y0, x0)`

The rotation direction is determined from the sign of the initial specific angular momentum:

`direction = sign(h0)`

The angular position at time `t` is:

`theta(t) = theta0 + direction * n * t`

The analytical coordinates are:

`x_ref = r0_mag * cos(theta)`

`y_ref = r0_mag * sin(theta)`

This branch avoids dividing by the eccentricity when generating a circular reference orbit.

---

## Numerical Simulation and Time History

### Inputs and validation

The program requests the following inputs:

| Input         | Meaning                       | Unit / format              |
|---------------|-------------------------------|----------------------------|
| `star_mass`   | Central star mass             | Positive finite scalar, kg |
| `planet_mass` | Orbiting planet mass          | Positive finite scalar, kg |
| `r0_planet`   | Initial relative position     | Two-component vector, m    |
| `v0_planet`   | Initial relative velocity     | Two-component vector, m/s  |
| `dt`          | Integration time step         | Positive finite scalar, s  |
| `T`           | Requested simulation duration | Positive finite scalar, s  |

The program validates the inputs before beginning the analytical and numerical calculations.

Masses must be positive, finite, real numerical scalars.

The initial position and velocity must be finite, real, two-component vectors.

The initial position must have a nonzero magnitude.

The initial conditions must describe a bound elliptical orbit with nonzero specific angular momentum.

The timestep and simulation duration must both be positive and finite.

The simulation duration must also be an integer multiple of the selected timestep within a specified floating-point tolerance.

### Number of integration steps

The ratio between the requested simulation duration and timestep is:

`step_ratio = T / dt`

The number of integration steps is:

`num_steps = round(step_ratio)`

The program checks that the ratio is sufficiently close to an integer.

The actual simulation duration is:

`actual_duration = num_steps * dt`

### Time history

The time history includes the initial state at time zero and every subsequent integration state:

`time_history = (0:num_steps)' * dt`

The time values are also converted into days:

`time_days = time_history / 86400`

The total number of recorded states is:

`number_of_states = num_steps + 1`

This means that the initial conditions are stored as the first state, followed by one recorded state for each integration step.

### Stored arrays

The primary Velocity Verlet histories are:

- `planet_positions`
- `planet_velocities`
- `planet_accelerations`

The Semi-Implicit Euler histories are:

- `euler_planet_positions`
- `euler_planet_velocities`
- `euler_planet_accelerations`

Both methods store their initial positions and velocities before integration begins.

The program also calculates and stores derived quantities, including:

- `planet_distance`
- `euler_planet_distance`
- `planet_speed`
- `euler_planet_speed`
- `specific_energy_history`
- `euler_specific_energy_history`
- `specific_angular_momentum`
- `euler_specific_angular_momentum`

The analytical reference positions are stored separately in:

- `analytical_positions`

All histories correspond to the same simulation timestamps, allowing direct comparisons between the analytical and numerical results.

---

## Semi-Implicit Euler Integration

Semi-Implicit Euler is retained as a comparison method.

It calculates gravitational acceleration from the current position, updates the velocity, and then advances the position using the updated velocity.

### Acceleration

The gravitational acceleration at the current position is:

`a = -(mu / r_mag^3) * r`

### Velocity update

The velocity is updated first:

`v_next = v + a * dt`

### Position update

The position is then advanced using the updated velocity:

`r_next = r + v_next * dt`

### Acceleration at the new position

The acceleration is recalculated using the new position:

`a_next = -(mu / r_next_mag^3) * r_next`

The new position, velocity, and acceleration are stored in their corresponding histories.

The update order distinguishes Semi-Implicit Euler from explicit Euler, which uses the previous velocity when calculating the next position.

Semi-Implicit Euler is retained to provide a numerical comparison with Velocity Verlet. Its accuracy and conservation behaviour depend on the selected timestep and simulation duration.

---

## Velocity Verlet Integration

Velocity Verlet is the default numerical integration method.

It calculates the next position using the current position, velocity, and acceleration, then recalculates acceleration at the new position before updating the velocity.

### Initial acceleration

The gravitational acceleration at the current position is:

`a = -(mu / r_mag^3) * r`

### Position update

The next position is calculated as:

`r_next = r + v * dt + 0.5 * a * dt^2`

### Acceleration update

The acceleration is recalculated at the new position:

`a_next = -(mu / r_next_mag^3) * r_next`

### Velocity update

The next velocity is calculated using the average of the current and new accelerations:

`v_next = v + 0.5 * (a + a_next) * dt`

The resulting position, velocity, and acceleration are stored in their respective histories.

Velocity Verlet is a second-order numerical integration method for suitable position-dependent forces. It often exhibits useful long-term behaviour in conservative systems, but it is not exact.

Its accuracy depends on the timestep, orbital parameters, simulation duration, and numerical implementation.

The prototype compares its results against Semi-Implicit Euler and the analytical reference trajectory. It does not assume that Velocity Verlet is exact or universally more accurate under every possible configuration.

---

## Numerical Orbital Measurements

### Distance and speed

The distance from the gravitational focus is calculated for every numerical state.

For Velocity Verlet:

`r_mag = sqrt(x^2 + y^2)`

For Semi-Implicit Euler, the same calculation is performed using its own position history.

The speed is calculated as:

`speed = sqrt(vx^2 + vy^2)`

These histories allow the program to examine how orbital distance and speed change throughout the simulation.

### Specific orbital energy history

The specific orbital energy is recalculated using the instantaneous numerical distance and speed.

For Velocity Verlet:

`epsilon_verlet = planet_speed^2 / 2 - mu / planet_distance`

For Semi-Implicit Euler:

`epsilon_euler = euler_planet_speed^2 / 2 - mu / euler_planet_distance`

The resulting values are stored throughout the simulation.

For the ideal continuous two-body problem, specific orbital energy is conserved. Numerical integration can introduce deviations from the initial value.

### Specific angular momentum history

Specific angular momentum is calculated at every recorded state.

For Velocity Verlet:

`h_verlet = x * vy - y * vx`

For Semi-Implicit Euler:

`h_euler = x * vy - y * vx`

The position and velocity components are taken from the corresponding numerical histories.

The resulting values are stored to examine angular momentum conservation during integration.

---

## Numerical Orbital Period Measurement

The analytical orbital period is calculated independently using Kepler's Third Law.

The prototype also measures the orbital period produced by each numerical integration method.

This makes it possible to compare the measured numerical periods against the analytical orbital period.

### Angular progress

The numerical orbital period is measured by tracking the accumulated angular progress of the position vector around the central gravitational focus.

For two consecutive positions:

`r_previous = [x_previous, y_previous]`

`r_current = [x_current, y_current]`

The two-dimensional cross product is:

`cross = x_previous * y_current - y_previous * x_current`

The dot product is:

`dot = x_previous * x_current + y_previous * y_current`

The angular change between the consecutive positions is:

`delta_theta = atan2(cross, dot)`

The accumulated angular progress is:

`theta_current = theta_previous + delta_theta`

The calculation is performed independently for Velocity Verlet and Semi-Implicit Euler.

The sign of the initial specific angular momentum determines the direction of orbital motion.

### Detecting one complete revolution

A complete revolution is detected when the accumulated angular progress reaches:

`target_angle = sign(h0) * 2 * pi`

The prototype records the time at which each numerical trajectory first completes one revolution.

If the simulation does not record a complete revolution, the corresponding measured orbital period remains undefined and is represented by `NaN`.

### Period interpolation

The complete revolution can occur between two consecutive recorded states.

The prototype estimates the crossing time through linear interpolation.

The fraction of the timestep required to reach the target angle is:

`angle_fraction = (target_angle - theta_previous) / (theta_current - theta_previous)`

The measured orbital period is:

`measured_period = time_previous + angle_fraction * dt`

This provides a more precise period estimate than simply assigning the time of the first recorded state beyond one complete revolution.

The measurement is still an approximation based on the numerical angular progression and interpolation between consecutive states.

---

## Numerical Error Analysis

### Orbital-period errors

The analytical orbital period provides the reference value for the numerical period measurements.

The absolute period error is:

`absolute_error = abs(measured_period - orbital_period)`

The relative period error is:

`relative_error = absolute_error / orbital_period`

The percentage period error is:

`percentage_error = relative_error * 100`

These calculations are performed independently for Velocity Verlet and Semi-Implicit Euler.

If a complete revolution is not recorded, the corresponding period errors remain `NaN`.

### Position errors

The numerical position of each method is compared against the analytical reference position at the same timestamp.

For each recorded state, the x-coordinate difference is:

`error_x = x_num - x_ref`

The y-coordinate difference is:

`error_y = y_num - y_ref`

The Euclidean position error is:

`position_error = sqrt(error_x^2 + error_y^2)`

The calculation is performed separately for Velocity Verlet and Semi-Implicit Euler.

The prototype determines the initial position error:

`initial_position_error = position_error(1)`

The final position error:

`final_position_error = position_error(end)`

And the maximum position error:

`maximum_position_error = max(position_error_history)`

The errors are measured in metres and are converted into kilometres for graphical presentation.

These measurements describe how far the numerical trajectories deviate from the analytical reference at corresponding timestamps.

### Numerical periapsis and apoapsis

The prototype estimates the minimum and maximum orbital distances observed during the numerical simulation.

For Velocity Verlet:

`numerical_periapsis = min(planet_distance)`

`numerical_apoapsis = max(planet_distance)`

For Semi-Implicit Euler:

`numerical_periapsis = min(euler_planet_distance)`

`numerical_apoapsis = max(euler_planet_distance)`

These estimates can be compared with the analytical values:

`analytical_periapsis = a * (1 - e)`

`analytical_apoapsis = a * (1 + e)`

The numerical measurements represent the minimum and maximum distances actually recorded during the selected simulation.

If the simulation does not include the corresponding orbital phases, the measured extrema may not equal the analytical periapsis and apoapsis distances.

### Specific orbital energy conservation

The prototype calculates the absolute change between the final and initial specific orbital energy.

For Velocity Verlet:

`energy_error_verlet = abs(epsilon_final - epsilon_initial)`

For Semi-Implicit Euler:

`energy_error_euler = abs(epsilon_final - epsilon_initial)`

The values are measured in joules per kilogram.

These calculations provide a measure of the total change in specific orbital energy over the selected simulation duration.

### Specific angular momentum conservation

The prototype calculates the absolute change between the final and initial specific angular momentum.

For Velocity Verlet:

`angular_momentum_error_verlet = abs(h_final - h_initial)`

For Semi-Implicit Euler:

`angular_momentum_error_euler = abs(h_final - h_initial)`

The values are measured in square metres per second.

The program also calculates the relative change in specific angular momentum for graphical presentation:

`relative_change = (h_current - h_initial) / abs(h_initial)`

A value close to zero indicates that the numerical method has preserved specific angular momentum close to its initial value.

Energy and angular momentum diagnostics help evaluate numerical behaviour. Neither diagnostic alone proves that the entire numerical trajectory is accurate, so both should be interpreted alongside the position errors and period comparisons.

---

## Graphs and Visualisation

The prototype generates scientific plots to display the analytical and numerical orbital results.

### Analytical and numerical orbital trajectories

The orbital trajectory graph displays:

- Analytical reference orbit.
- Velocity Verlet numerical trajectory.
- Semi-Implicit Euler numerical trajectory.
- Central star at the origin.

The positions are converted from metres into astronomical units for easier visual interpretation.

The horizontal and vertical axes use equal scaling to preserve the geometric shape of the orbit.

### Star-planet distance against time

The distance graph displays the distance between the planet and the central gravitational focus throughout the simulation.

It compares:

- Velocity Verlet.
- Semi-Implicit Euler.
- Analytical reference trajectory.

Distance is expressed in astronomical units, while time is expressed in days.

This graph makes it possible to examine how the orbital distance changes throughout the elliptical trajectory and how closely the numerical methods reproduce the analytical distance history.

### Specific orbital energy against time

The energy graph displays the specific orbital energy histories of both numerical methods.

It also includes the initial analytical specific orbital energy as a reference line.

This allows the user to examine numerical energy variation throughout the simulation.

### Specific angular momentum conservation

The angular momentum graph displays the relative change in specific angular momentum for both numerical methods.

The initial value is represented by a zero reference line.

This graph helps evaluate how closely the numerical methods preserve specific angular momentum throughout the orbital integration.

### Numerical position errors

The position-error graph displays the distance between the numerical and analytical positions at corresponding timestamps.

Both Velocity Verlet and Semi-Implicit Euler are displayed.

The errors are converted into kilometres and plotted against simulation time in days.

### Orbital-period comparison

When both numerical methods complete at least one revolution, the prototype generates a graph comparing:

- Analytical orbital period.
- Velocity Verlet measured orbital period.
- Semi-Implicit Euler measured orbital period.

The periods are expressed in days.

The graph is omitted if either numerical method does not record a complete revolution.

### Main orbital parameter summary

The prototype generates a separate graphical summary containing:

- Semi-major axis.
- Eccentricity.
- Analytical orbital period.
- Periapsis distance.
- Apoapsis distance.
- Star mass.
- Planet mass.

This provides a compact overview of the physical system and its calculated analytical orbital properties.

---

## Example Input

The following baseline input can be used to test an approximately circular Earth-Sun orbit:

| Prompt                    | Example value  |
|---------------------------|----------------|
| Star mass (kg)            | `1.98847e30`   |
| Planet mass (kg)          | `5.9722e24`    |
| Initial position (m)      | `[1.496e11 0]` |
| Initial velocity (m/s)    | `[0 29780]`    |
| Integration time step (s) | `3600`         |
| Simulation duration (s)   | `63072000`     |

This example uses an initial position approximately equal to one astronomical unit and an initial velocity close to the Earth's orbital velocity.

The integration timestep is one hour, and the requested simulation duration is 730 days.

The resulting orbit is a slightly eccentric ellipse rather than an exactly circular orbit because the supplied initial velocity is not exactly the circular-orbit velocity for the selected masses and initial distance.

The expected analytical results are approximately:

| Parameter 		    | Expected value          |
|---------------------------|-------------------------|
| Gravitational parameter   | `1.32716852e20 m^3/s^2` |
| Initial distance          | `1.496e11 m`            |
| Initial speed             | `2.9780e4 m/s`          |
| Specific orbital energy   | `-4.43720531e8 J/kg`    |
| Specific angular momentum | `4.45508800e15 m^2/s`   |
| Semi-major axis           | `1.49550046e11 m`       | 
| Eccentricity              | `0.000334028`           |
| Periapsis distance        | `1.4950007e11 m`        |
| Apoapsis distance 	    | `1.4960002e11 m`        |
| Analytical orbital period | `31542539.78 s`         |
| Analytical orbital period | `365.075692 days`       |

The numerical period measurements and error values depend on the integration method and timestep.

---

## Numerical Output

The Command Window output provides a detailed summary of the simulation.

### Simulation setup

The program displays:

- Star mass.
- Planet mass.
- Gravitational parameter.
- Integration timestep.
- Actual simulation duration.
- Number of integration steps.
- Number of recorded states.

### Initial conditions

The initial conditions section displays:

- Initial position.
- Initial velocity.
- Initial speed.
- Initial distance.
- Initial specific orbital energy.
- Initial specific angular momentum.

### Analytical solution

The analytical solution section identifies the Newtonian two-body orbital calculation.

The analytical orbital parameter summary displays:

- Semi-major axis.
- Semi-minor axis.
- Eccentricity.
- Focal distance.
- Periapsis distance.
- Apoapsis distance.
- Analytical orbital period in seconds, days, and years.

### Numerical solutions

The numerical solutions section identifies Velocity Verlet as the default method and Semi-Implicit Euler as the comparison method.

### Orbital-period comparison

The program displays the analytical orbital period and the measured numerical periods.

When a complete revolution is recorded, it also displays:

- Measured orbital period.
- Absolute period error.
- Relative period error.
- Percentage period error.

If a complete revolution is not recorded, the program displays a corresponding message instead of reporting an invalid period measurement.

### Numerical position errors

The program displays the initial, final, and maximum position errors for both numerical methods.

### Orbital conservation errors

The program displays the changes in specific orbital energy and specific angular momentum for both numerical methods.

### Final results

The final results section displays the final position, velocity, distance, and speed for Velocity Verlet and Semi-Implicit Euler.

### Complete time histories

The prototype displays the complete recorded numerical histories for both methods.

The Velocity Verlet time-history table contains:

- Time.
- x-position.
- y-position.
- x-velocity.
- y-velocity.
- Speed.
- x-acceleration.
- y-acceleration.
- Distance.
- Specific orbital energy.
- Specific angular momentum.

The Semi-Implicit Euler table contains the corresponding quantities for its own numerical trajectory.

The histories allow individual numerical states to be inspected and used in further calculations or validation.

---

## Validation and Testing

The prototype should be tested against analytical reference values, physical expectations, and numerical consistency checks.

### Analytical calculation tests

The analytical calculations should be tested using independent reference values.

Recommended checks include:

- Verify the gravitational parameter using the combined mass of both bodies.
- Verify the initial specific orbital energy.
- Verify the initial specific angular momentum.
- Verify the semi-major axis against an independent calculation.
- Verify the eccentricity against an independent calculation.
- Verify the semi-minor axis and focal distance.
- Verify the periapsis and apoapsis distances.
- Verify the analytical orbital period using the Newtonian two-body formulation.
- Verify that the calculated orbital period is independent of the numerical timestep.
- Verify that the circular-orbit branch produces a constant analytical orbital radius.
- Verify that the elliptical reference trajectory begins at the supplied initial position.

### Numerical integration tests

Both numerical methods should be tested against the same physical initial conditions.

Recommended checks include:

- Verify that both methods begin from identical initial positions and velocities.
- Verify that Velocity Verlet performs the position, acceleration, and velocity updates in the correct order.
- Verify that Semi-Implicit Euler updates velocity before position.
- Verify that both methods calculate gravitational acceleration using the correct gravitational parameter.
- Compare numerical trajectories against the analytical reference trajectory.
- Compare energy and angular momentum conservation behaviour.
- Repeat the simulations with different timesteps.
- Verify that reducing the timestep changes the numerical results consistently with the expected behaviour of each integration method.

The comparison should not assume that Velocity Verlet is exact or universally superior. Its numerical behaviour must be evaluated using the actual results.

### Orbital-period measurement tests

The numerical period measurement should be checked using:

- A simulation containing at least one complete revolution.
- A simulation containing multiple complete revolutions.
- A simulation that ends before completing one revolution.
- Different timestep sizes.
- Both clockwise and counterclockwise initial orbital motion.

The measured periods should be compared against the analytical orbital period.

For incomplete orbital trajectories, the program should retain `NaN` for the measured period and its corresponding errors.

### Input-validation tests

Input validation should cover:

- Zero or negative star mass.
- Zero or negative planet mass.
- Non-finite or non-real mass values.
- Initial position vectors with the wrong number of components.
- Initial velocity vectors with the wrong number of components.
- Non-finite position or velocity components.
- Zero initial distance.
- Unbound initial conditions.
- Zero initial specific angular momentum.
- Zero or negative integration timestep.
- Zero or negative simulation duration.
- Simulation duration incompatible with the timestep.

For rejected inputs, record the expected error message and actual program behaviour.

### Numerical consistency checks

The following checks should be used to verify the implementation:

- Confirm that all calculated orbital parameters are finite and physically valid for accepted inputs.
- Confirm that the analytical reference trajectory starts at the initial position.
- Confirm that both numerical methods start from the same initial conditions.
- Confirm that the number of recorded states is one greater than the number of integration steps.
- Confirm that all stored histories have consistent dimensions.
- Confirm that the initial position errors are approximately zero.
- Confirm that numerical period measurements are only produced after a complete revolution.
- Confirm that the period errors use the analytical orbital period as their reference.
- Confirm that the energy and angular momentum histories use the corresponding numerical positions and velocities.
- Confirm that the graph data correspond to the calculated numerical and analytical histories.
- Confirm that repeated runs with identical inputs produce reproducible results.

The v1.8 testing and validation work establishes the intended test cases and expected results. These tests should be rerun after changes to the calculations or code organisation. The existence of an error calculation or diagnostic graph alone does not demonstrate that every test has passed.

---

## Assumptions and Limitations

The prototype has the following assumptions and limitations:

- The physical system consists of two point masses interacting through Newtonian gravity.
- The calculations describe the relative motion of the two bodies.
- The position and velocity vectors are expressed in a star-centred relative coordinate system.
- The gravitational parameter includes the masses of both the star and planet.
- The prototype does not separately calculate the barycentric positions of the two bodies.
- Motion is restricted to two dimensions.
- The model neglects additional planets and other gravitational perturbations.
- The model does not include atmospheric drag, thrust, or other non-gravitational forces.
- Relativistic corrections are not included.
- The gravitational bodies are treated as point masses.
- The accepted initial conditions describe bound elliptical orbits with nonzero specific angular momentum.
- The analytical orbital elements are derived from the initial conditions and the Newtonian two-body equations.
- The analytical elliptical reference trajectory uses a numerical bisection procedure to solve Kepler's equation.
- The numerical trajectories are approximations and depend on the integration timestep.
- Velocity Verlet is not an exact solution, even though it is the default integration method.
- Semi-Implicit Euler is retained as a comparison method and may exhibit different numerical errors and conservation behaviour.
- The measured numerical orbital period is estimated through angular progression and linear interpolation between consecutive timesteps.
- A numerical period cannot be measured when the simulation does not contain a complete revolution.
- Numerical periapsis and apoapsis estimates represent the minimum and maximum recorded distances, not necessarily the exact analytical orbital extrema.
- Energy and angular momentum diagnostics evaluate numerical conservation behaviour but do not independently establish trajectory accuracy.
- The prototype is not a general-purpose N-body simulator.

---

## Version Development

The prototype's development is organised into ten milestones.

- **v1.0 — Kepler's Third Law Core Calculation:** Established the Newtonian two-body orbital period calculation using the masses of the central and orbiting bodies and the semi-major axis.
- **v1.1 — User Inputs and Simulation Parameters:** Introduced user-defined physical parameters and the initial conditions required for further orbital calculations.
- **v1.2 — Initial Conditions and Orbital Calculations:** Expanded the calculation framework to derive orbital properties from the initial position and velocity.
- **v1.3 — Analytical Orbital Properties and Reference Calculations:** Introduced the analytical orbital elements and the calculations required to describe the resulting elliptical orbit.
- **v1.4 — Analytical Validation and Error Analysis:** Established independent reference calculations and quantitative checks for the analytical results.
- **v1.5 — Analytical Reference Orbit:** Introduced the time-dependent analytical reference trajectory for comparison with numerical orbital motion.
- **v1.6 — Numerical Integration and Method Comparison:** Integrated Velocity Verlet and Semi-Implicit Euler to calculate and compare numerical orbital trajectories.
- **v1.7 — Orbital Analysis and Graphical Visualisation:** Added orbital histories, numerical orbital-period measurements, error calculations, conservation diagnostics, and scientific graphs.
- **v1.8 — Comprehensive Testing and Validation:** Established systematic test cases for analytical calculations, numerical integration, orbital measurements, input validation, stored histories, and graphical consistency.
- **v1.9 — Core Simulation Complete:** Consolidated the analytical solver, numerical integration, orbital measurements, parameter experiments, visualisation, and validation into a stable prototype, with a focus on reusable functions, consistent units, documentation, and reproducibility.

A version should be considered complete only when its implementation and validation criteria have been checked. A written test plan is not equivalent to an automated test suite, and a diagnostic calculation is not equivalent to a verified passing test.

---

## Current State and Future Development

The current prototype combines analytical orbital calculations, numerical integration, orbital-period measurement, numerical error analysis, and graphical visualisation.

It provides a computational foundation for demonstrating Kepler's Third Law and examining how the analytical orbital period relates to the physical parameters of a two-body system.

The prototype also makes it possible to compare numerical integration methods against an independent analytical reference trajectory and investigate how numerical errors develop over time.

Potential future improvements include:

- Separating the analytical calculations, numerical integration methods, orbital measurements, and graphical routines into reusable GNU Octave functions.
- Introducing a more structured automated testing framework with independent reference values and defined numerical tolerances.
- Expanding timestep-convergence experiments.
- Adding further independent analytical reference cases.
- Improving the organisation and export of simulation results.
- Extending the physical model to include additional gravitational bodies in a future N-body simulation.
- Integrating the validated calculations into the AstPIE educational platform.

Future versions should preserve the distinction between analytical calculations and numerical approximations. Any changes to the physical model or numerical methods should be accompanied by appropriate documentation and validation.

The v1.9 prototype serves as the computational baseline for the future integration of Kepler's Third Law calculations into AstPIE.