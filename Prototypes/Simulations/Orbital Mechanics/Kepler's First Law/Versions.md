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