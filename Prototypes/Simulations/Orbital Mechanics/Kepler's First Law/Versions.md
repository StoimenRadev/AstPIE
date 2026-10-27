### v1.0 — Core Elliptical Orbit Prototype

Builds the smallest working orbital simulation by calculating a planet's trajectory around a fixed central star using Newtonian gravity and semi-implicit Euler integration.

### Implemented:
* Defined the central star's mass and gravitational parameter.
* Defined the planet's initial position and velocity.
* Represented position and velocity as two-dimensional vectors.
* Calculated the planet's distance from the central star at every time step.
* Calculated the gravitational acceleration vector from the planet's current position.
* Implemented a numerical integration loop using semi-implicit Euler.
* Updated velocity before position at every integration step.
* Stored the planet's calculated positions throughout the simulation.
* Defined a fixed one-hour time step and a simulation duration of 365 days.
* Displayed the central star at the origin of the coordinate system, identifying it as the intended orbital focus.
* Displayed the numerically calculated planetary trajectory.
* Marked the planet's starting and ending positions on the trajectory plot.
* Displayed the initial and final position and velocity components in the command window.
* Added plot labels, a title, a legend, a grid, and equal axis scaling.
* Documented the coordinate system, SI units, numerical integration method, and simulation limitations in code comments.

### Purpose:
To establish the minimum working orbital simulation for AstPIE and demonstrate how Newtonian gravity generates a planetary trajectory numerically. This version provides the foundation for subsequent development of analytical orbital elements, orbital geometry, numerical integration improvements, and validation against Kepler's First Law.

### v1.1 — User Inputs and Simulation Results

Makes the core orbital simulation configurable by accepting user-defined physical parameters and initial conditions, validating inputs, and displaying the parameters and results of the numerical simulation.

### Implemented:
* Added user input for the central star's mass.
* Added user input for the planet's mass.
* Added user input for the planet's initial position as a two-dimensional vector.
* Added user input for the planet's initial velocity as a two-dimensional vector.
* Added user input for the integration time step and total simulation duration.
* Validated that the star's mass and planet's mass are positive, finite numerical scalars.
* Validated that the time step and simulation duration are positive, finite numerical scalars.
* Validated the dimensions and numerical validity of the initial position and velocity vectors.
* Rejected complex values, invalid vector dimensions, NaN, and infinity in the relevant inputs.
* Standardised valid position and velocity vectors into row-vector format.
* Added validation to ensure that the simulation duration is an integer multiple of the time step and contains at least one integration step.
* Calculated the number of integration steps from the user-defined duration and time step.
* Calculated the gravitational parameter from the user-defined stellar mass and the gravitational constant.
* Displayed a summary of the input parameters, gravitational parameter, actual simulation duration, and number of integration steps.
* Preserved the semi-implicit Euler integration method from v1.0.
* Stored the calculated planetary positions throughout the simulation.
* Added checks for invalid positions, velocities, and numerical results during integration.
* Displayed the initial and final position and velocity components.
* Updated the trajectory plot to use the user-defined initial conditions and mark the starting and ending positions.
* Preserved the fixed central star at the origin and identified it as the intended orbital focus.
* Documented the numerical integration method and its limitations.

### Purpose:
To make the orbital simulation configurable and suitable for repeatable experiments with different stars, planets, initial positions, initial velocities, time steps, and simulation durations.

### v1.2 — Orbital Geometry and Theoretical Ellipse

Extends the configurable orbital simulation by calculating the theoretical orbital geometry from the initial conditions and gravitational parameters, generating an analytical ellipse, and plotting it alongside the numerically calculated trajectory for comparison.

### Implemented:
* Calculated the initial orbital radius from the planet's initial position vector.
* Calculated the squared magnitude of the planet's initial velocity.
* Calculated the specific orbital energy from the initial position, velocity, and gravitational parameter.
* Added validation to reject initial conditions that do not describe a bound elliptical orbit.
* Calculated the specific angular momentum from the initial position and velocity vectors.
* Added validation to reject radial trajectories with zero specific angular momentum.
* Calculated the orbital eccentricity using the specific orbital energy and specific angular momentum.
* Added validation to ensure that the calculated eccentricity is finite and within the valid range for an elliptical orbit.
* Calculated the eccentricity vector to determine the orientation of the orbital ellipse.
* Calculated the perihelion angle from the eccentricity vector, with a defined orientation for circular orbits.
* Calculated the semimajor axis using the specific orbital energy.
* Calculated the semiminor axis from the semimajor axis and eccentricity.
* Calculated the focal distance from the semimajor axis and eccentricity.
* Calculated the perihelion and aphelion distances.
* Generated theoretical ellipse coordinates using the parametric equations of an ellipse.
* Shifted the ellipse coordinates so that the central star is located at one of the ellipse's foci.
* Rotated the theoretical ellipse according to the calculated perihelion angle.
* Extended the orbital plot to display the numerical trajectory and theoretical ellipse together.
* Distinguished the numerical trajectory, theoretical ellipse, central star, initial position, and final position using separate plot styles and markers.
* Added plot labels, a title, a legend, and a grid, with equal scaling of the coordinate axes to preserve the ellipse's geometric proportions.
* Added validation for invalid orbital conditions and numerical values encountered during the calculation of orbital geometry.
* Prepared the prototype for experiments involving nearly circular, moderately eccentric, and highly eccentric bound orbits.
* Preserved the semi-implicit Euler integration method from previous versions.
* Established the theoretical ellipse as an analytical reference for comparing the numerical trajectory.

### Purpose:
To extend the numerical orbital simulation with analytical orbital geometry based on Kepler's First Law. This version enables the calculation and visualisation of elliptical orbital parameters and allows the numerical trajectory to be compared with the theoretically predicted orbit under the same initial conditions. It also establishes a foundation for further experiments involving orbital eccentricity, orientation, and numerical accuracy.

### v1.3 — Time History and Orbital Measurements

Extends the orbital simulation by recording the planet’s position, velocity, and acceleration throughout the numerical integration, analysing its distance from the star and speed over time, identifying the closest and farthest observed approaches, and comparing the numerical trajectory with the analytically calculated elliptical orbit.

###Implemented:
* Preserved the user-defined physical parameters and initial conditions from v1.1.
* Preserved input validation for masses, initial position, initial velocity, time step, and total simulation duration.
* Added validation to ensure that the initial conditions produce a bound elliptical orbit.
* Calculated the planet’s initial specific orbital energy.
* Calculated the planet’s specific angular momentum.
* Calculated the orbital eccentricity from the initial conditions.
* Calculated the eccentricity vector to determine the orientation of the analytical ellipse.
* Calculated the semi-major axis and semi-minor axis of the analytical ellipse.
* Calculated the focal distance of the analytical ellipse.
* Calculated the analytical perihelion and aphelion distances.
* Generated the analytical elliptical orbit from its geometric parameters and orientation.
* Created a time history containing every integration time point.
* Stored the planet’s position at every integration step.
* Stored the planet’s velocity at every integration step.
* Stored the planet’s acceleration at every integration step.
* Preserved the semi-implicit Euler integration method.
* Added checks for invalid planetary positions and numerical values during integration.
* Calculated the planet’s distance from the star at every recorded time point.
* Calculated the planet’s speed at every recorded time point.
* Identified the minimum and maximum observed star–planet distances during the simulated interval.
* Recorded the times, positions, and velocities corresponding to the minimum and maximum observed distances.
* Displayed the initial and final planetary positions and velocities.
* Displayed the calculated orbital elements and analytical ellipse geometry.
* Displayed the minimum and maximum observed distances and their corresponding times.
* Updated the trajectory plot to compare the numerical trajectory with the analytical ellipse.
* Marked the star, initial position, final position, closest observed approach, and farthest observed approach on the trajectory plot.
* Added a distance-versus-time graph.
* Added a speed-versus-time graph.
* Distinguished the observed minimum and maximum distances over the simulated interval from the analytical perihelion and aphelion distances.
* Documented the numerical integration method and its limitations, including the possibility of numerical errors causing the calculated trajectory to diverge from the analytical ellipse.

### Purpose:
To analyse the evolution of an orbit throughout a numerical simulation, examine how the planet’s distance and speed change over time, identify its closest and farthest observed approaches to the star, and compare the numerically calculated trajectory with the analytically determined elliptical orbit.

### v1.4 — Visualisation and Graphs

Enhances the orbital simulation with scientific visualisations, analytical reference geometry, time-dependent graphs, and a display of the main analytical orbital parameters.

### Implemented:
* Updated the orbital trajectory plot to display the numerical trajectory calculated using the semi-implicit Euler method.
* Added the analytical orbital ellipse as a separate reference curve calculated from the initial conditions and analytical orbital parameters.
* Preserved equal horizontal and vertical scaling to ensure that the displayed orbital geometry is not distorted.
* Displayed the central star at the origin, identifying its position as the orbital focus.
* Marked the initial and final planetary positions on the trajectory plot.
* Displayed the observed minimum- and maximum-distance positions along the numerical trajectory, where implemented.
* Added a distance-versus-time graph showing the planet's distance from the central star throughout the simulation.
* Added a speed-versus-time graph showing the magnitude of the planet's velocity throughout the simulation.
* Added separate graphs for the planet's x- and y-coordinates as functions of time.
* Added graph titles, axis labels, physical units, legends, and grid lines to improve readability.
* Displayed the main analytical orbital parameters, including eccentricity, semi-major axis, semi-minor axis, focal distance, analytical perihelion distance, and analytical aphelion distance.
* Used consistent physical units and scientific notation where appropriate to present numerical values clearly.
* Preserved the distinction between the numerical integration results and analytical orbital calculations.
* Kept the analytical ellipse independent of the numerical trajectory, allowing the numerical orbit to be compared against the analytical reference.
* Used the same underlying simulation data for the numerical trajectory and time-dependent graphs.

### Purpose:
To make the orbital simulation easier to interpret, analyse, and validate by presenting the planet's motion through scientific graphs and comparing the numerical trajectory with the analytically calculated orbital geometry. The visualisations help demonstrate the relationship between orbital shape, distance, speed, and position over time.

### v1.5 — Analytical Orbit Validation and Numerical Error

Adds analytical orbital calculations and quantitative validation of the numerical trajectory against the independently calculated analytical ellipse. Compares the observed perihelion and aphelion distances with their analytical predictions and evaluates the geometric error introduced by numerical integration.

### Implemented:
* Calculated the initial specific orbital energy and specific angular momentum from the initial conditions.
* Calculated the analytical orbital eccentricity.
* Calculated the eccentricity vector to determine the orientation of the analytical orbit.
* Calculated the analytical semimajor axis and semiminor axis.
* Calculated the focal distance and analytical perihelion and aphelion distances.
* Generated an analytical elliptical reference orbit independently of the numerical integration.
* Increased the resolution of the analytical ellipse to 3,601 sampled points to improve the precision of geometric comparisons.
* Compared the numerical trajectory with the analytical ellipse in the orbital trajectory plot.
* Calculated the geometric distance between each numerical position and its nearest sampled point on the analytical ellipse.
* Calculated and displayed the maximum and mean geometric distances between the numerical trajectory and the sampled analytical ellipse.
* Identified the minimum and maximum star–planet distances observed during the numerical simulation.
* Recorded the times, positions, and velocities associated with the observed minimum and maximum distances.
* Compared the observed minimum distance with the analytical perihelion distance.
* Compared the observed maximum distance with the analytical aphelion distance.
* Calculated and displayed the absolute differences and percentage errors for the observed minimum and maximum distances relative to their analytical predictions.
* Displayed the analytical orbital elements and ellipse geometry for inspection.
* Preserved the numerical position, distance, and speed histories and their corresponding plots.
* Established test cases for comparing numerical results at different integration time steps.
* Documented the effects of numerical integration error, the limitations of the sampled analytical ellipse, and the importance of simulating long enough to capture the orbital extrema.

### Purpose:
To establish a quantitative method for validating the numerical trajectory against the analytical ellipse predicted by Kepler's First Law. This version provides measurable geometric differences between the numerical and analytical orbits, compares observed orbital extrema with analytical predictions, and establishes a basis for investigating how the integration time step affects numerical accuracy.

### v1.6 — Improved Numerical Integration and Method Comparison

Improves the numerical accuracy of the orbital simulation by making Velocity Verlet the default integration method and comparing its results with semi-implicit Euler. Both methods calculate the planet's trajectory independently using the same initial conditions, gravitational parameter, time step, and simulation duration. The results are evaluated using orbital trajectories, physical quantities, conservation errors, and comparisons with the theoretical elliptical orbit.

### Implemented:
* Preserved the user-defined stellar mass and planetary mass inputs.
* Preserved the user-defined initial position and velocity vectors.
* Preserved the user-defined integration time step and total simulation duration.
* Preserved input validation and checks for invalid numerical values during integration.
* Preserved the analytical calculation of the orbital elements from the initial conditions.
* Preserved the theoretical elliptical orbit as a reference for the numerical trajectories.
* Implemented Velocity Verlet as the primary and default numerical integration method.
* Preserved semi-implicit Euler as a separate numerical integration method for comparison.
* Implemented Velocity Verlet position updates using the current position, velocity, and acceleration.
* Implemented Velocity Verlet acceleration updates using the newly calculated position.
* Implemented Velocity Verlet velocity updates using the average of the current and next accelerations.
* Stored the planetary positions and velocities throughout the Velocity Verlet integration.
* Stored the planetary positions and velocities throughout the semi-implicit Euler integration in separate arrays.
* Calculated the planet's distance from the star at each time step for both methods.
* Calculated the planet's speed at each time step for both methods.
* Calculated the specific orbital energy throughout both numerical integrations.
* Calculated the specific angular momentum throughout both numerical integrations.
* Calculated the relative energy error for each method to evaluate the conservation of orbital energy.
* Calculated the relative angular momentum error for each method to evaluate the conservation of angular momentum.
* Compared the numerical trajectories produced by Velocity Verlet and semi-implicit Euler.
* Calculated the difference between the planetary positions obtained using the two numerical methods at corresponding time steps.
* Compared the numerical trajectories with the theoretical elliptical orbit.
* Calculated the distance between the numerically calculated positions and the sampled theoretical ellipse.
* Identified the minimum and maximum star–planet distances obtained using Velocity Verlet.
* Displayed the initial and final positions and velocities for the numerical methods.
* Displayed the calculated orbital elements, including specific orbital energy, specific angular momentum, eccentricity, semi-major axis, semi-minor axis, focal distance, perihelion distance, and aphelion distance.
* Displayed numerical conservation errors and comparison results for both integration methods.
* Added separate plots for the Velocity Verlet trajectory and theoretical ellipse, the trajectories of both numerical methods, position components, distance and speed over time, relative energy and angular momentum errors, distance from the theoretical ellipse, and the difference between the numerical trajectories.
* Preserved the fixed central star at the origin and the assumption that the star remains stationary.
* Documented the differences between numerical integration and analytical orbital calculations.
* Preserved the limitations associated with finite time steps and the numerical approximation of the theoretical ellipse.

### Purpose:
To improve the reliability of the orbital simulation by introducing Velocity Verlet as the default numerical integration method and evaluating its behaviour against semi-implicit Euler. This version provides a more comprehensive basis for studying orbital motion, numerical accuracy, conservation of physical quantities, and deviations from the theoretically predicted elliptical orbit.

### v1.7 — Testing and Validation Suite

Establishes a repeatable testing and validation suite for the orbital simulation by checking physical calculations, analytical orbital parameters, numerical integration behaviour, input validation, and consistency of simulation outputs under controlled test conditions.

### Implemented:
* Defined standard test inputs using realistic stellar and planetary masses, the real gravitational constant, physically consistent initial conditions, and short simulation durations.
* Added test cases for circular orbits with analytically predictable orbital parameters.
* Added test cases for moderately eccentric elliptical orbits.
* Added test cases for highly eccentric but gravitationally bound elliptical orbits.
* Defined expected values for specific orbital energy, specific angular momentum, eccentricity, semi-major axis, semi-minor axis, focal distance, perihelion distance, and aphelion distance.
* Added test cases for invalid stellar and planetary masses, including zero, negative values, non-finite values, and invalid input dimensions.
* Added test cases for invalid time steps and simulation durations, including non-positive values, non-finite values, and durations that are not integer multiples of the time step.
* Added test cases for invalid initial position and velocity vectors, including incorrect dimensions, non-finite components, and zero initial position.
* Added test cases for zero initial velocity and radial trajectories that cannot be represented by the current elliptical-orbit model.
* Added test cases for unbound trajectories that do not satisfy the model's bound-ellipse requirements.
* Added short-duration simulation tests to verify that the model produces valid results even when the simulated trajectory covers only a small fraction of an orbit.
* Added gravitational acceleration tests at different positions to verify the inverse-square dependence on the distance from the star.
* Added analytical ellipse geometry tests to verify the relationship between the star's position, orbital focus, semi-major axis, semi-minor axis, eccentricity, and apsidal distances.
* Defined independent reference values and comparison tolerances for analytical orbital parameters.
* Added checks for the consistency of time-history outputs, including positions, velocities, accelerations, distances, speeds, specific orbital energy, and specific angular momentum.
* Added numerical accuracy tests using different time steps while keeping the initial conditions and total simulation duration unchanged.
* Defined expected behaviours for Velocity Verlet energy and angular momentum conservation and for the comparison between Velocity Verlet and semi-implicit Euler integration.
* Established criteria for distinguishing expected physical behaviour from numerical integration errors.
* Organised the tests around explicit input values, expected outputs, physical units, and validation criteria.
* Preserved the fixed-star approximation, two-dimensional Newtonian gravitational model, and bound elliptical orbit restrictions of the existing implementation.

### Purpose:
To verify that the orbital simulation produces physically consistent results, calculates analytical orbital parameters correctly, handles invalid inputs safely, and maintains acceptable numerical accuracy under different test conditions.