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