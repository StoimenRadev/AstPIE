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