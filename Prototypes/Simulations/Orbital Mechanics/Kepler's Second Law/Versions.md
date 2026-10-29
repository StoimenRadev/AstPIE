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