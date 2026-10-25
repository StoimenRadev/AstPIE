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