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