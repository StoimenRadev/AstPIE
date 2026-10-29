### v1.0 — Kepler's Second Law Core Prototype

Implements the core numerical orbital simulation for investigating Kepler's Second Law by calculating the planet's trajectory, speed, specific angular momentum, and instantaneous areal velocity under the gravitational influence of a fixed central star.

### Implemented:
* Defined the central star's mass using the Sun's mass.
* Defined the gravitational constant in SI units.
* Calculated the gravitational parameter from the central star's mass and the gravitational constant.
* Defined the planet's initial position as a two-dimensional vector in metres.
* Defined the planet's initial velocity as a two-dimensional vector in metres per second.
* Defined the integration time step and total simulation duration.
* Calculated the number of integration steps from the simulation duration and time step.
* Initialised the planet's current position and velocity from the initial conditions.
* Implemented the gravitational acceleration vector using the inverse-square gravitational model.
* Implemented semi-implicit Euler integration, updating velocity before position at each integration step.
* Stored the planet's position at every integration step to preserve the numerical trajectory.
* Calculated and stored the planet's speed at every integration step from the velocity components.
* Calculated and stored the signed specific angular momentum from the position and velocity vectors.
* Calculated and stored the instantaneous signed areal velocity from the specific angular momentum.
* Established the relationship between specific angular momentum and instantaneous areal velocity, using A = h/2
* Plotted the numerically calculated planetary trajectory in two dimensions.
* Plotted the central star at the origin and identified it in the trajectory plot.
* Added coordinate-axis labels, a title, a legend, a grid, and equal scaling of the coordinate axes.
* Stored the velocity vector at every integration step to support numerical reporting of the initial and final states.
* Displayed the initial position, velocity components, and speed in the Command Window.
* Displayed the final position, velocity components, speed, and simulated duration.
* Displayed the initial and final specific angular momentum and instantaneous areal velocity.
* Calculated the relative changes in specific angular momentum and instantaneous areal velocity to evaluate their numerical conservation.
* Used explicit conditional statements to calculate the magnitudes of numerical differences without relying on abs().
* Preserved the central star at a fixed position at the origin.
* Established the initial numerical framework for investigating the relationship between gravitational motion, angular momentum conservation, and Kepler's Second Law.
* Identified the limitations of the numerical integration method, including possible deviations from ideal conservation due to finite time steps.

### Purpose:
To establish a working numerical prototype for Kepler's Second Law using a planet orbiting a fixed central star. This version introduces the calculation and storage of orbital position, velocity, speed, specific angular momentum, and instantaneous areal velocity. It also provides a visual representation of the orbit and numerical outputs for examining whether areal velocity remains approximately constant during the simulation.

### v1.1 — User Inputs and Simulation Results

Makes the core orbital simulation configurable by accepting user-defined physical parameters and initial conditions, validating inputs, and displaying the parameters and results of the numerical simulation.

### Implemented:
* Added user input for the central star's mass.
* Added user input for the planet's initial position as a two-dimensional vector.
* Added user input for the planet's initial velocity as a two-dimensional vector.
* Added user input for the integration time step and total simulation duration.
* Validated that the star's mass is a positive, finite numerical scalar.
* Validated that the time step and simulation duration are positive, finite numerical scalars.
* Validated the dimensions and numerical validity of the initial position and velocity vectors.
* Rejected complex values, invalid vector dimensions, NaN, and infinity in the relevant inputs.
* Validated that the initial position is not located at the central star's position.
* Validated that the initial conditions produce nonzero specific angular momentum.
* Added validation to ensure that the simulation duration is an integer multiple of the time step and contains at least one integration step.
* Calculated the number of integration steps from the user-defined duration and time step.
* Calculated the gravitational parameter from the user-defined stellar mass and the gravitational constant.
* Calculated and displayed the initial distance, speed, specific angular momentum, and areal velocity.
* Displayed a summary of the input parameters, gravitational parameter, actual simulation duration, and number of integration steps.
* Preserved the semi-implicit Euler integration method.
* Stored the planet's position, velocity, speed, specific angular momentum, and areal velocity throughout the simulation.
* Added a check to detect when the planet reaches the central star's exact position during integration.
* Displayed the initial and final position, velocity, and speed.
* Added numerical checks for Kepler's Second Law by comparing the initial and final specific angular momentum and areal velocity.
* Calculated and displayed the absolute differences and relative percentage changes in specific angular momentum and areal velocity.
* Updated the trajectory plot to use the user-defined initial conditions and mark the central star at the origin.
* Documented the numerical integration method's role in the simulation and the possibility of numerical drift.

### Purpose:
To make the orbital simulation configurable and suitable for repeatable experiments with different stellar masses, initial positions, initial velocities, time steps, and simulation durations. The version also establishes the numerical quantities and diagnostic outputs needed to investigate Kepler's Second Law, including the relationship between specific angular momentum and areal velocity.

### v1.2 — Time History and Swept-Area Measurements

Extends the orbital simulation by recording the complete history of the planet's motion, calculating approximate swept areas between consecutive positions, and comparing swept areas over equal-duration measurement intervals to investigate Kepler's Second Law.

### Implemented:
* Generated a time history containing the initial time and every subsequent integration time step.
* Stored the planet's position, velocity, and acceleration at every recorded time step.
* Calculated the planet's distance from the central star at every recorded time step.
* Calculated the planet's speed at every recorded time step.
* Calculated the specific angular momentum at every recorded time step.
* Calculated the instantaneous areal velocity at every recorded time step using the relationship between specific angular momentum and areal velocity.
* Added numerical checks to detect invalid positions, velocities, accelerations, and distances during integration.
* Calculated the approximate swept area between consecutive recorded positions using the two-dimensional position-vector cross-product formula.
* Stored the individual swept-area measurements for every integration step.
* Created a swept-area history aligned with the recorded time history, with an initial value of zero.
* Calculated the cumulative swept area throughout the simulation by summing the individual swept-area measurements.
* Added user input for the duration of the swept-area measurement intervals.
* Validated that the measurement interval is positive, finite, and an integer multiple of the integration time step.
* Divided the simulation into successive measurement intervals, including a potentially shorter final interval.
* Recorded the start time, end time, and actual duration of every measurement interval.
* Calculated the total approximate swept area for each measurement interval by summing its individual integration-step areas.
* Calculated the average areal velocity for each measurement interval by dividing its estimated swept area by its actual duration.
* Displayed the complete time history, including position, velocity, speed, acceleration, distance, specific angular momentum, instantaneous areal velocity, swept area per step, and cumulative swept area.
* Displayed a comparison table containing the start time, end time, duration, total swept area, and average areal velocity of each measurement interval.
* Preserved the semi-implicit Euler integration method and the existing orbital trajectory visualization.
* Distinguished the approximate triangular swept areas calculated from sampled positions from the exact continuous swept areas described by Kepler's Second Law.

### Purpose:
To extend the orbital simulation from tracking the planet's trajectory to quantitatively investigating Kepler's Second Law. The version establishes a complete numerical record of orbital motion, measures the approximate area swept during individual time steps, and aggregates these measurements over equal-duration intervals. This enables comparisons of swept areas and average areal velocities, while accounting for numerical integration error and the limitations of approximating continuous orbital motion using discrete position samples.

### v1.3 — Orbital Geometry and Analytical Ellipse

Implements analytical orbital-element calculations and generates an analytical ellipse from the planet's initial position and velocity. This version overlays the analytical ellipse with the numerically calculated trajectory and visualises the star's position at the ellipse's focus, the geometric centre, and the major and minor axes.

### Implemented:
* Calculated the initial speed from the planet's initial velocity components.
* Calculated the initial specific angular momentum from the planet's initial position and velocity vectors.
* Calculated the initial specific orbital energy using the planet's initial speed, distance from the star, and gravitational parameter.
* Calculated the semimajor axis from the specific orbital energy.
* Calculated the orbital eccentricity from the specific orbital energy, specific angular momentum, and gravitational parameter.
* Included numerical tolerance handling for small eccentricity-squared values to support nearly circular orbits.
* Validated that the initial conditions describe a bound elliptical orbit.
* Calculated the semiminor axis from the semimajor axis and eccentricity.
* Calculated the focal distance from the semimajor axis and eccentricity.
* Calculated the perihelion distance and aphelion distance from the semimajor axis and eccentricity.
* Calculated the eccentricity vector from the initial position, initial velocity, and gravitational parameter.
* Determined the orientation of the analytical ellipse using the direction of the eccentricity vector.
* Used the initial position direction as a reference orientation when the orbit is circular and the eccentricity vector does not provide a defined perihelion direction.
* Generated parameter values spanning a complete revolution to define the analytical ellipse.
* Calculated the local coordinates of the ellipse using its semimajor and semiminor axes.
* Rotated the ellipse coordinates to match the orientation determined from the initial conditions.
* Translated the ellipse coordinates so that the central star is located at one of the ellipse's foci.
* Plotted the numerically calculated planetary trajectory using semi-implicit Euler integration.
* Plotted the analytically calculated ellipse alongside the numerical trajectory for visual comparison.
* Plotted the central star at the origin and identified it as the focus of the analytical ellipse.
* Calculated and plotted the geometric centre of the ellipse.
* Plotted the major and minor axes to illustrate the ellipse's geometric dimensions and orientation.
* Added coordinate-axis labels, a descriptive title, a legend, a grid, and equal scaling of the coordinate axes.
* Displayed the specific orbital energy and specific angular momentum used to calculate the orbital elements.
* Displayed the semimajor axis, eccentricity, semiminor axis, focal distance, perihelion distance, and aphelion distance in the Command Window.
* Retained the numerical trajectory, velocity, acceleration, distance, speed, specific angular momentum, and areal-velocity histories from previous versions.
* Retained swept-area calculations and equal-time interval comparisons to support the investigation of Kepler's Second Law.
* Established a visual basis for comparing the analytical orbital geometry with the trajectory produced by numerical integration.
* Identified that differences between the analytical ellipse and numerical trajectory can arise from the finite time step and the numerical integration method.

### Purpose:
To extend the Kepler's Second Law prototype with analytical orbital geometry and a reference ellipse derived from the initial conditions. This version connects the planet's initial position and velocity to the shape, dimensions, and orientation of its ideal elliptical orbit. By displaying the analytical ellipse alongside the numerically calculated trajectory, it provides a visual means of examining the accuracy of the numerical integration and understanding the geometry of elliptical orbits.

### v1.4 — Visualisation and Graphs

Extends the orbital simulation with scientific graphs and a clearer visual presentation of the planet's motion. This version visualises the numerical trajectory alongside the analytical ellipse and displays the evolution of star–planet distance, speed, and position coordinates over time. It also provides a graphical summary of the main analytical orbital parameters.

### Implemented:
* Displayed the numerical orbital trajectory with equal horizontal and vertical axis scaling to preserve the geometric proportions of the orbit.
* Displayed the central star at the origin and identified it as the focus of the analytical ellipse.
* Displayed the analytical ellipse as a separate reference curve alongside the numerically calculated trajectory.
* Marked the geometric centre of the analytical ellipse.
* Displayed the major and minor axes to illustrate the ellipse's dimensions and orientation.
* Added coordinate-axis labels, units, a descriptive title, a legend, and a grid to the orbital trajectory plot.
* Calculated and displayed the star–planet distance against time using the recorded distance history.
* Converted the distance from metres to astronomical units (AU) for more readable graph presentation.
* Calculated and displayed the planet's speed against time using the recorded speed history.
* Converted the speed from metres per second to kilometres per second for more readable graph presentation.
* Displayed the planet's x-coordinate against time using the recorded position history.
* Displayed the planet's y-coordinate against time using the recorded position history.
* Converted the x- and y-coordinates from metres to astronomical units for graph presentation.
* Used separate subplots to present the x- and y-coordinate histories within a single figure.
* Added appropriate axis labels, units, titles, and grid lines to the distance, speed, and coordinate graphs.
* Displayed the main analytical orbital parameters in a separate graphical summary.
* Included the semimajor axis, semiminor axis, eccentricity, focal distance, perihelion distance, aphelion distance, and gravitational parameter in the graphical summary.
* Used scientific notation and consistent physical units to present orbital parameters across the simulation's outputs.
* Used the existing time history, distance history, speed history, and position history as the data sources for the corresponding graphs.
* Preserved the existing semi-implicit Euler integration method and the calculations from previous versions.
* Retained the swept-area measurements and equal-time interval comparisons introduced in v1.2.
* Distinguished the numerical trajectory from the analytical reference ellipse to make their differences visually observable.
* Preserved equal coordinate-axis scaling for the orbital trajectory rather than distorting the orbit to fit the plotting window.

### Purpose:
To make the orbital simulation easier to interpret and use as an educational tool by presenting its numerical results through scientific graphs and a graphical summary of the orbital parameters. This version allows the user to examine how star–planet distance, speed, and position coordinates change throughout an orbit, while comparing the numerically integrated trajectory with the analytical elliptical reference. The visualisations support the investigation of orbital geometry, perihelion and aphelion, and the relationship between orbital distance and speed.

### v1.5 — Analytical Orbit Validation and Numerical Error

Validate the numerical orbit against an independently calculated analytical reference and quantify the accuracy and limitations of numerical integration.

### Implemented features:
* Calculate analytical orbital parameters from the initial conditions.
* Calculate analytical reference positions at every recorded simulation time using Kepler's equation.
* Handle elliptical and circular orbits separately when calculating analytical reference positions.
* Compare numerical and analytical positions at matching times.
* Calculate and display initial, maximum, and final position errors.
* Compare numerical periapsis and apoapsis distances against their analytical values.
* Check whether the simulated trajectory captures the analytical periapsis and apoapsis within a selected tolerance.
* Plot numerical position error against time.
* Compare maximum and final position errors using different timestep sizes.
* Investigate the effects of semi-implicit Euler integration on orbital accuracy.

###Purpose:
Establish a quantitative validation process for the numerical orbit, measure position errors, and investigate how timestep size affects numerical accuracy.

### v1.6 — Velocity Verlet Integration

Extends the orbital simulation by implementing Velocity Verlet as the default numerical integration method while preserving semi-implicit Euler as a comparison method. Runs both methods under identical initial conditions and compares their orbital trajectories, position errors, and sensitivity to integration timestep size against the analytical orbital solution.

### Implemented:
* Implemented Velocity Verlet integration for the planet's gravitational motion around the central star.
* Set Velocity Verlet as the primary integration method used by the simulation.
* Stored the planet's position, velocity, and acceleration at every integration time step using the primary planet_positions, planet_velocities, and planet_accelerations variables.
* Preserved the semi-implicit Euler integration method as a second numerical method for comparison.
* Used separate euler_planet_positions, euler_planet_velocities, and euler_planet_accelerations variables to store the Euler integration history independently.
* Applied the same gravitational acceleration model and initial orbital conditions to both numerical integration methods.
* Calculated the numerical orbital trajectories independently for Velocity Verlet and semi-implicit Euler.
* Preserved the analytical orbital calculations and the generated ellipse used as the reference solution.
* Compared the numerical trajectories against the analytical orbital solution to evaluate position errors.
* Calculated maximum and final position errors for the primary Velocity Verlet trajectory.
* Calculated maximum and final position errors for the semi-implicit Euler trajectory.
* Preserved the existing time-history measurements used to investigate Kepler's Second Law, including distance, speed, specific angular momentum, instantaneous areal velocity, swept area per integration step, and cumulative swept area.
* Preserved the measurement-interval calculations, including the total swept area and average areal velocity for each interval.
* Preserved the orbital trajectory visualization and the comparison with the analytical ellipse.
* Added the numerical orbital trajectory of semi-implicit Euler to the visualization for direct comparison with Velocity Verlet.
* Added a timestep comparison for both numerical integration methods using timestep candidates derived from the user-entered integration timestep.
* Compared both methods using the same simulation duration and initial conditions for each valid comparison timestep.
* Filtered out comparison timesteps that do not divide the total simulation duration into an integer number of integration steps.
* Calculated the maximum and final position errors for both methods at each valid comparison timestep.
* Added comparison plots to investigate how integration timestep size affects the numerical accuracy of both methods.
* Preserved the distinction between the numerical approximation of orbital motion and the analytical reference solution.

### Purpose:
To extend the orbital simulation from using a single numerical integration method to comparing two methods under equivalent conditions. Velocity Verlet becomes the primary integration method, while semi-implicit Euler provides a reference for evaluating how numerical integration affects orbital trajectories and position errors. By testing multiple timestep sizes against the analytical orbital solution, this version establishes a basis for investigating numerical accuracy, timestep sensitivity, and the reliability of the swept-area measurements used to study Kepler's Second Law. The comparison does not assume that Velocity Verlet is exact or universally superior. Its performance must be evaluated using the actual numerical results, with particular attention to position errors and the behavior of the orbital quantities over the simulated duration.