### v1.0 — Kepler's Third Law Core Calculation

Implements the fundamental Newtonian two-body calculation for Kepler's Third Law by determining the orbital period of a two-body system from the masses of the central and orbiting bodies and the orbit's semi-major axis.

### Implemented:
* Defined the universal gravitational constant G in SI units.
* Used GNU Octave's built-in mathematical constant pi.
* Defined the central star's mass in kilograms.
* Defined the orbiting planet's mass in kilograms.
* Defined the orbital semi-major axis in metres.
* Used documented Earth–Sun system values as the initial test case.
* Calculated the total mass of the two-body system by adding the central star's mass and the orbiting planet's mass.
* Implemented the Newtonian two-body formulation of Kepler's Third Law.
* Rearranged Kepler's Third Law to calculate the orbital period directly from the system's physical parameters.
* Calculated the orbital period using the square root of the ratio between the numerator and denominator of the equation.
* Used the total mass of both bodies in the gravitational denominator rather than considering only the central star's mass.
* Stored the calculated orbital period in a dedicated variable.
* Expressed the calculated orbital period in seconds.
* Displayed the calculated orbital period in the Command Window using formatted numerical output.
* Included the orbital period's unit in the displayed result.
* Used GNU Octave's fprintf() function to format the numerical output.
* Verified the calculated orbital period against an independent reference calculation.
* Confirmed that the calculated Earth–Sun orbital period is approximately 31,558,345.249 seconds for the selected input values.
* Kept the orbital period calculation physically and computationally separate from time-stepped orbital simulations.
* Established a minimal computational framework for calculating orbital periods in Newtonian two-body systems.

### Purpose:
To establish the fundamental computational implementation of Kepler's Third Law by calculating the orbital period of a two-body system from its total mass and semi-major axis. This version introduces the Newtonian two-body orbital period equation, establishes the use of SI units and provides a working Earth–Sun test case. It also creates the foundation for future versions that will introduce user-defined inputs, additional calculations, numerical validation, and comparisons between different orbital systems.

### v1.1 — User Inputs and Simulation Results

Transforms the fixed Kepler's Third Law calculation into a configurable Newtonian two-body orbital period calculator by allowing the user to enter the masses of both bodies and the semi-major axis. The program validates the entered physical parameters, calculates the total system mass and orbital period, and displays the input parameters and results in seconds, days, and Julian years.

### Implemented:
* Replaced the predefined central star's mass with a user-defined input.
* Replaced the predefined orbiting planet's mass with a user-defined input.
* Replaced the predefined semi-major axis with a user-defined input.
* Used GNU Octave's input() function to receive numerical values from the user.
* Included descriptive input prompts specifying the expected SI units.
* Validated the central body's mass to ensure it is a positive, finite, real scalar numerical value.
* Validated the orbiting body's mass to ensure it is a positive, finite, real scalar numerical value.
* Validated the semi-major axis to ensure it is a positive, finite, real scalar numerical value.
* Used isscalar() to reject inputs containing multiple values.
* Used isnumeric() to check that the inputs are numerical.
* Used isreal() to reject complex-valued inputs.
* Used isfinite() to reject infinite and NaN values.
* Used conditional statements and logical operators to combine input validation requirements.
* Used error() to terminate execution and display descriptive error messages when invalid physical parameters are entered.
* Preserved the universal gravitational constant G in SI units.
* Used GNU Octave's built-in mathematical constant pi.
* Calculated the total mass of the two-body system dynamically from the user-provided masses.
* Implemented the Newtonian two-body formulation of Kepler's Third Law using the validated input parameters.
* Calculated the orbital period directly from the semi-major axis and total system mass.
* Stored the calculated orbital period in seconds.
* Converted the orbital period from seconds into days using 86,400 seconds per day.
* Converted the orbital period from days into Julian years using 365.25 days per Julian year.
* Preserved the original orbital period in seconds as the primary SI result.
* Displayed the central body's mass in the Command Window using scientific notation.
* Displayed the orbiting body's mass in the Command Window using scientific notation.
* Displayed the semi-major axis and total system mass in the Command Window.
* Displayed the calculated orbital period in seconds, days, and Julian years.
* Used GNU Octave's fprintf() function to organise and format the numerical output.
* Included explicit units in the displayed input parameters and calculated results.
* Separated the displayed input parameters from the calculated orbital results using distinct output sections.
* Replaced the fixed Earth–Sun test parameters from v1.0 with user-configurable physical inputs.
* Maintained the distinction between an analytical orbital period calculation and a time-stepped orbital simulation.
* Established the initial user-interaction and input-validation framework for future Kepler's Third Law developments.

### Purpose:
To transform the fundamental Kepler's Third Law calculation into a configurable scientific tool that accepts user-defined physical parameters and produces corresponding orbital period results. This version introduces numerical input handling, validation of physical parameters, dynamic total mass calculation, and conversion of the orbital period into convenient time units.

### v1.2 — Three-Variable Kepler's Third Law Solver

Extends the analytical Kepler's Third Law calculator by implementing all three rearrangements of the Newtonian two-body orbital period equation. The program calculates the orbital period, semi-major axis and total system mass using the same initial input parameters, without requiring the user to select a calculation mode.

### Implemented:
* Retained the user-defined central body's mass in kilograms.
* Retained the user-defined orbiting body's mass in kilograms.
* Retained the user-defined semi-major axis in metres.
* Preserved input validation for positive, finite, real scalar values.
* Defined the universal gravitational constant G in SI units.
* Used GNU Octave's built-in mathematical constant pi.
* Calculated the total system mass by adding the central body's mass and the orbiting body's mass.
* Implemented the Newtonian two-body formulation of Kepler's Third Law to calculate the orbital period.
* Rearranged Kepler's Third Law to calculate the semi-major axis from the total system mass and calculated orbital period.
* Implemented the cube-root relationship required for the semi-major axis calculation.
* Rearranged Kepler's Third Law to calculate the total system mass from the semi-major axis and calculated orbital period.
* Stored the calculated orbital period in a dedicated variable.
* Stored the calculated semi-major axis in a separate variable.
* Stored the calculated total system mass in a separate variable.
* Converted the calculated orbital period from seconds into days.
* Converted the calculated orbital period into Julian years using 365.25 days per year.
* Displayed the original input parameters in the Command Window.
* Displayed the calculated total system mass.
* Displayed the calculated orbital period in seconds, days and Julian years.
* Displayed the recalculated semi-major axis in metres.
* Displayed the recalculated total system mass in kilograms.
* Used GNU Octave's fprintf() function to format the numerical output and display the appropriate units.
* Used the same Newtonian two-body model for all three rearranged calculations.
* Demonstrated that the calculated semi-major axis reproduces the original input semi-major axis within numerical precision.
* Demonstrated that the calculated total system mass reproduces the original total system mass within numerical precision.
* Maintained the distinction between analytical orbital calculations and time-stepped numerical orbital simulations.
* Established the mathematical foundation for later numerical integration and analytical-versus-numerical comparisons.

### Purpose
To extend the fundamental Kepler's Third Law calculation into an analytical three-variable solver capable of calculating the orbital period, semi-major axis and total system mass. This version introduces the rearrangement of the original Newtonian two-body equation to solve for different unknown quantities while preserving the same physical model and SI-unit conventions.

### v1.3 — Initial Conditions, Orbital Elements and Numerical Integration

Implements an initial-condition-based orbital calculation using the Newtonian two-body model. Determines the orbital elements, including specific orbital energy, specific angular momentum, eccentricity, semi-major axis and orbital period, from the initial relative position and velocity vectors. Introduces semi-implicit Euler integration to numerically calculate and display the orbiting body's trajectory over a specified duration.

### Implemented:
* Defined the universal gravitational constant G in SI units.
* Defined the astronomical unit (AU) in metres.
* Defined the number of seconds per day.
* Defined the number of days per Julian year.
* Accepted the central body's mass in kilograms through user input.
* Accepted the orbiting body's mass in kilograms through user input.
* Implemented validation for both masses, requiring positive, finite, real scalar values.
* Accepted the initial relative position vector in Cartesian coordinates [rx,ry], expressed in metres.
* Accepted the initial relative velocity vector in Cartesian coordinates [vx,vy], expressed in metres per second.
* Implemented validation for both vectors, requiring exactly two finite, real numerical components.
* Converted the accepted position and velocity vectors into row-vector format for consistent calculations.
* Calculated the initial distance between the two bodies from the components of the relative position vector.
* Calculated the initial speed from the components of the relative velocity vector.
* Implemented a check preventing an initial position at the central body's location.
* Calculated the total mass of the two-body system by adding the central and orbiting bodies' masses.
* Calculated the gravitational parameter using the gravitational constant and total system mass.
* Calculated the initial specific orbital energy using the kinetic and gravitational potential energy contributions per unit reduced mass.
* Calculated the initial specific angular momentum using the two-dimensional Cartesian cross-product formulation.
* Calculated the magnitude of the initial specific angular momentum.
* Implemented a check preventing initial conditions with zero specific angular momentum.
* Calculated the orbital eccentricity from the specific orbital energy, specific angular momentum and gravitational parameter.
* Classified the resulting orbit as elliptical, parabolic or hyperbolic based on its specific orbital energy.
* Implemented the semi-major axis calculation for bound elliptical orbits using the specific orbital energy.
* Calculated the orbital period of elliptical orbits using the Newtonian two-body formulation of Kepler's Third Law.
* Converted the calculated orbital period from seconds into days and Julian years.
* Converted the calculated semi-major axis from metres into astronomical units.
* Prevented the calculation of a finite orbital period for parabolic and hyperbolic trajectories.
* Accepted a user-defined numerical integration timestep in seconds.
* Implemented validation requiring the timestep to be a positive, finite, real scalar.
* Accepted a user-defined numerical integration duration in seconds.
* Implemented validation requiring the integration duration to be a positive, finite, real scalar.
* Calculated the number of numerical integration steps using the rounded ratio between the requested duration and timestep.
* Initialized the numerical position and velocity variables using the user-provided initial conditions.
* Created arrays for storing the planet's positions, velocities and simulation time throughout the integration.
* Implemented semi-implicit Euler integration as the numerical orbital integration method.
* Calculated the gravitational acceleration vector from the current position and gravitational parameter.
* Updated the velocity before updating the position at each integration step.
* Stored the calculated positions and velocities at every integration step.
* Stored the elapsed simulation time throughout the integration.
* Calculated the final numerical position, velocity, distance and speed.
* Recalculated the specific orbital energy and specific angular momentum from the final numerical state.
* Calculated the differences between the initial and final specific orbital energy and specific angular momentum to examine numerical conservation.
* Displayed the input parameters, including both masses and the initial position and velocity vectors.
* Displayed the calculated gravitational parameter, initial distance and initial speed.
* Displayed the analytical orbital elements, including specific orbital energy, specific angular momentum, eccentricity and orbital classification.
* Displayed the calculated semi-major axis in metres and astronomical units for elliptical orbits.
* Displayed the calculated orbital period in seconds, days and Julian years for elliptical orbits.
* Displayed the numerical integration method, timestep, requested integration duration, number of integration steps and actual integration duration.
* Displayed the final numerical position, velocity, distance and speed.
* Displayed the initial and final specific orbital energy and specific angular momentum, together with their absolute differences.
* Implemented a two-dimensional orbital trajectory plot using the stored numerical positions.
* Displayed the central body, initial planetary position and final planetary position on the trajectory plot.
* Used GNU Octave's fprintf() function to format numerical output and display the corresponding physical units.
* Established a distinction between analytical orbital-element calculations and numerical trajectory integration.

### Purpose:
To extend the fundamental Kepler's Third Law calculation from a semi-major-axis-based input into an initial-condition-based orbital calculation. This version introduces the calculation of orbital elements directly from the initial relative position and velocity vectors, allowing the program to determine the eccentricity, semi-major axis and orbital period without requiring the user to provide the semi-major axis. It also introduces semi-implicit Euler integration to numerically calculate the orbital trajectory and examine the conservation of specific orbital energy and angular momentum.

### v1.4 — Analytical Validation and Numerical Conservation Analysis

Introduces analytical validation of Kepler's Third Law and numerical conservation analysis by checking the calculated orbital period relationship and evaluating the conservation of specific orbital energy and specific angular momentum during numerical integration.

### Implemented:
* Implemented the analytical Kepler coefficient using the gravitational parameter of the two-body system.
* Calculated the orbital period from the analytical Kepler coefficient and the semi-major axis.
* Calculated the residual between the original analytical orbital period and the period obtained from the coefficient formulation.
* Calculated the absolute error between the calculated and analytical Kepler coefficients.
* Calculated the relative error between the calculated and analytical Kepler coefficients.
* Calculated the absolute and relative residuals between the two orbital period calculations.
* Restricted orbital period validation to elliptical trajectories.
* Initialized and stored the specific orbital energy at every numerical integration timestep.
* Initialized and stored the specific angular momentum at every numerical integration timestep.
* Compared the initial and final specific orbital energy to evaluate numerical energy conservation.
* Compared the initial and final specific angular momentum to evaluate numerical angular momentum conservation.
* Calculated the absolute differences in specific orbital energy and specific angular momentum.
* Calculated the relative numerical errors in specific orbital energy and specific angular momentum.
* Added the specific orbital energy and specific angular momentum to the complete numerical time-history table.
* Displayed the analytical validation results in the Command Window.
* Displayed the numerical conservation analysis in the Command Window.
* Tested the implementation using an Earth-like elliptical orbit, a highly eccentric elliptical orbit, and a hyperbolic trajectory.
* Verified that analytical period validation is not applied to unbound trajectories.

### Purpose:
To establish analytical validation and numerical conservation checks for the existing Kepler's Third Law implementation. This version evaluates the consistency of the analytical orbital period calculations and measures how well the numerical integration preserves specific orbital energy and specific angular momentum. It also verifies the behaviour of the calculations across different orbital configurations, improving the reliability and validation of the existing implementation without introducing additional orbital mechanics features.

### v1.5 — Visualization and Graphs

Adds graphical representations of Kepler's Third Law and the numerical orbital evolution of the two-body system. Introduces time-history graphs and a graphical summary of the main orbital parameters, making the calculated orbital relationships and simulation results easier to interpret.

### Implemented:
* Generated a range of 100 semi-major-axis values for the selected two-body system.
* Set the minimum semi-major axis to 0.5 times the reference semi-major axis.
* Set the maximum semi-major axis to 2 times the reference semi-major axis.
* Calculated the corresponding orbital periods for all generated semi-major-axis values using the Newtonian two-body formulation of Kepler's Third Law.
* Calculated the squared orbital periods for the generated values.
* Calculated the cubed semi-major-axis values for the generated values.
* Stored the generated semi-major axes, orbital periods, cubed semi-major axes and squared orbital periods in a dedicated visualization results matrix.
* Checked the generated visualization values for non-finite numerical results.
* Stored the selected system's calculated semi-major axis and orbital period as reference values.
* Calculated and stored the reference squared orbital period and cubed semi-major axis.
* Displayed the visualization range and reference values in the Command Window.
* Displayed the generated visualization data in a formatted results table.
* Implemented a graph of orbital period against semi-major axis.
* Added the selected system's reference case to the orbital period versus semi-major axis graph.
* Implemented a graph of squared orbital period against cubed semi-major axis.
* Added the selected system's reference case to the squared orbital period versus cubed semi-major axis graph.
* Added descriptive graph titles, axis labels with SI units, legends and grid formatting.
* Implemented a graph of star–planet distance against time using the numerical integration history.
* Expressed the time axis in days and the distance axis in astronomical units (AU).
* Implemented a graph of planetary speed against time using the numerical integration history.
* Implemented separate subplots for the planet's X-position and Y-position against time.
* Expressed the position coordinates in astronomical units (AU) and time in days.
* Added descriptive titles, axis labels, legends and grid formatting to the time-history graphs.
* Calculated the perihelion distance using the semi-major axis and orbital eccentricity.
* Calculated the aphelion distance using the semi-major axis and orbital eccentricity.
* Implemented a separate graphical summary window displaying the main orbital parameters.
* Displayed the semi-major axis, eccentricity, perihelion distance, aphelion distance, orbital period in seconds, orbital period in days and gravitational parameter in the orbital parameter summary.
* Restricted the orbital parameter summary and Kepler's Third Law visualizations to bound elliptical orbits.
* Generated all graphs from calculated analytical values or stored numerical integration results rather than manually constructed curves.

### Purpose:
To make Kepler's Third Law and its relationship with orbital geometry and orbital evolution visually understandable. This version introduces graphical comparisons of orbital period against semi-major axis and squared orbital period against cubed semi-major axis, allowing the predicted nonlinear and linear relationships to be examined. It also provides time-history graphs of planetary distance, speed and position, alongside a graphical summary of the main orbital parameters. The resulting visualizations make the calculated relationships and numerical results easier to inspect and interpret.

### v1.6 — Velocity Verlet Integration

Implements Velocity Verlet as the primary numerical integration method for calculating orbital motion in a Newtonian two-body system, while retaining semi-implicit Euler as a secondary comparison method. Both methods use identical initial conditions, gravitational parameters and timesteps, allowing their numerical results to be compared independently of the analytical Kepler's Third Law calculation.

### Implemented:
* Reused the existing gravitational acceleration model for calculating the relative motion of the two-body system.
* Defined the gravitational parameter using the universal gravitational constant and the combined mass of the central and orbiting bodies.
* Initialized the Velocity Verlet integration using the existing position and velocity variables.
* Initialized the semi-implicit Euler integration using separate variables with the euler_ prefix.
* Implemented the Velocity Verlet position update using the current position, velocity and acceleration.
* Calculated the new gravitational acceleration at each updated position.
* Implemented the Velocity Verlet velocity update using the average of the current and newly calculated accelerations.
* Established Velocity Verlet as the primary numerical integration method.
* Retained semi-implicit Euler as a secondary numerical integration method for comparison.
* Implemented the semi-implicit Euler velocity update using the current acceleration.
* Implemented the semi-implicit Euler position update using the newly calculated velocity.
* Ensured that both integration methods use identical initial positions, initial velocities, gravitational parameters and timesteps.
* Maintained separate position, velocity and acceleration histories for both integration methods.
* Reused the existing planet_ variable naming convention for Velocity Verlet calculations.
* Introduced the euler_ variable naming convention for semi-implicit Euler calculations.
* Stored the calculated orbital speed and distance at every timestep for both methods.
* Calculated and stored the specific orbital energy throughout both numerical integrations.
* Calculated and stored the specific angular momentum throughout both numerical integrations.
* Implemented numerical checks for finite position, velocity and acceleration values and nonzero orbital distances.
* Calculated the initial and final specific orbital energy for both integration methods.
* Calculated the initial and final specific angular momentum for both integration methods.
* Calculated the absolute and relative conservation errors for specific orbital energy and specific angular momentum separately for each method.
* Generated separate numerical results matrices containing the time history, position, velocity, speed, acceleration, distance, specific orbital energy and specific angular momentum for each integration method.
* Retained the existing orbital trajectory and time-history visualizations using Velocity Verlet as the primary numerical method.
* Displayed the numerical conservation results for both integration methods separately.
* Preserved the analytical Kepler's Third Law orbital period calculation independently of the numerical integration methods.
* Preserved the analytical orbital parameter calculations and Kepler's Third Law visualizations independently of the numerical integration methods.
* Established a common computational framework for comparing numerical orbital integration methods under identical physical conditions.

### Purpose:
To introduce numerical orbital motion through Velocity Verlet integration while retaining semi-implicit Euler as a comparison method. This version establishes Velocity Verlet as the primary integration method and allows both methods to be evaluated using identical initial conditions and timesteps. It also introduces separate numerical histories and conservation error calculations for orbital energy and angular momentum. The analytical Kepler's Third Law calculation remains independent of the numerical integration, creating the foundation for future comparisons between analytically calculated orbital periods and periods measured from numerically calculated trajectories.

### v1.7 — Orbital Analysis and Graphical Visualisation

Extends the existing Kepler's Third Law prototype with additional orbital analysis outputs, conservation measurements, complete time-history displays, and graphical comparisons between the analytical reference orbit and the two numerical integration methods.

### Implemented:
* Generated a time history in seconds and converted it into days for graphical analysis.
* Calculated the specific orbital energy history for Velocity Verlet and Semi-Implicit Euler.
* Calculated the specific angular momentum history for both numerical integration methods.
* Implemented numerical orbital period measurement by accumulating angular progress until each numerical trajectory completes one full revolution.
* Used interpolation between consecutive timesteps to estimate the time at which each numerical trajectory completes one full revolution.
* Calculated the absolute, relative, and percentage orbital period errors for both numerical integration methods.
* Calculated the initial, final, and maximum positional errors for both numerical integration methods.
* Determined the minimum and maximum star–planet distances recorded during each numerical simulation.
* Calculated the changes in specific orbital energy between the beginning and end of each numerical simulation.
* Calculated the changes in specific angular momentum between the beginning and end of each numerical simulation.
* Displayed the analytical and numerical orbital periods together with their corresponding errors.
* Displayed the initial, final, and maximum positional errors for both integration methods.
* Displayed the specific orbital energy and angular momentum conservation errors.
* Displayed the final position, velocity, distance, and speed for both numerical integration methods.
* Generated complete time-history tables for Velocity Verlet and Semi-Implicit Euler, including position, velocity, speed, acceleration, distance, specific orbital energy, and specific angular momentum.
* Generated a graphical comparison of the analytical reference orbit and both numerical trajectories.
* Generated a star–planet distance versus time graph comparing the analytical and numerical results.
* Generated a specific orbital energy versus time graph to examine numerical energy conservation.
* Generated a specific angular momentum conservation graph using relative changes from the initial specific angular momentum.
* Generated a numerical positional error versus time graph comparing both integration methods.
* Generated an orbital period comparison graph displaying the analytical period and measured numerical periods when both numerical methods complete a revolution.
* Generated a main orbital parameters summary displaying the semi-major axis, eccentricity, orbital period, periapsis, apoapsis, and both body masses.
* Adjusted the graphical plotting order to ensure that analytical reference curves remain visible when numerical trajectories overlap them.

###Purpose:
To expand the existing Kepler's Third Law prototype with comprehensive numerical results, conservation measurements, and graphical visualisation. This version makes it possible to examine the agreement between the analytical reference calculations and the numerical integration methods through orbital trajectories, orbital periods, positional errors, and conservation properties. It also provides a more complete presentation of simulation results through formatted output, time-history tables, and comparative graphs.

### v1.8 — Comprehensive Testing and Validation

Establishes a systematic testing framework for the existing Kepler's Third Law prototype by validating its analytical calculations, numerical integration methods, orbital measurements, input handling, stored data, and graphical outputs against defined reference cases and expected results.

### Implemented:
* Established a baseline Earth–Sun test case using documented physical parameters and user-defined initial conditions.
* Defined expected analytical reference values for the baseline test, including the gravitational parameter, specific orbital energy, specific angular momentum, semi-major axis, eccentricity, periapsis, apoapsis, and orbital period.
* Tested the analytical orbital calculations against independently calculated reference values.
* Tested Velocity Verlet integration against the baseline Earth–Sun reference orbit.
* Tested Semi-Implicit Euler integration against the same reference orbit.
* Tested the analytical and numerical calculations using circular initial conditions.
* Tested the analytical and numerical calculations using elliptical initial conditions.
* Tested different orbital eccentricities by changing the initial velocity while keeping the other baseline parameters constant.
* Tested numerical integration using smaller, baseline, and larger timestep sizes.
* Tested the effect of different total system masses on the calculated gravitational parameter and orbital period.
* Tested the relationship between different semi-major axes and their corresponding orbital periods.
* Tested numerical orbital period measurement for both integration methods.
* Tested incomplete orbital trajectories to verify that orbital periods are not incorrectly reported when a complete revolution has not been recorded.
* Tested input validation using invalid masses, initial position and velocity vectors, unbound orbital conditions, zero angular momentum, invalid timesteps, and invalid simulation durations.
* Tested the calculations of absolute, relative, and percentage orbital period errors.
* Tested positional error calculations against the analytical reference trajectory.
* Tested specific orbital energy conservation measurements for both numerical integration methods.
* Tested specific angular momentum conservation measurements for both numerical integration methods.
* Verified the mathematical relationships used in the numerical error calculations.
* Tested stored position, velocity, acceleration, distance, energy, angular momentum, and time histories for correct dimensions and indexing.
* Verified that the initial and final recorded states correspond to the specified simulation conditions and duration.
* Tested the dimensions and contents of the complete numerical time-history tables.
* Tested the graphical consistency of the analytical and numerical orbital trajectories.
* Tested the star–planet distance, specific orbital energy, specific angular momentum, and positional error graphs against their corresponding calculated data.
* Tested the orbital period comparison graph and the main orbital parameter summary.
* Established individual test cases using numbered identifiers, with lettered subtests where multiple variations belong to the same test.
* Defined expected results and pass/fail criteria for the individual tests.
* Established a test-recording procedure documenting inputs, expected outputs, actual results, numerical tolerances, outcomes, and observations.
* Documented numerical limitations and discrepancies identified during testing.
* Established repeatable test cases that can be executed again after subsequent code modifications.

## Purpose:
To establish the reliability and numerical consistency of the existing Kepler's Third Law prototype through systematic testing and validation. This version verifies that the analytical calculations, Velocity Verlet and Semi-Implicit Euler integration methods, orbital measurements, input validation, stored data, and graphical outputs operate correctly across different physical conditions.