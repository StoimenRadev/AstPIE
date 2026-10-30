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
To transform the fundamental Kepler's Third Law calculation into a configurable scientific tool that accepts user-defined physical parameters and produces corresponding orbital period results. This version introduces numerical input handling, validation of physical parameters, dynamic total mass calculation, and conversion of the orbital period into convenient time units. It also establishes the foundation for future versions that will introduce more advanced numerical validation, comparisons between orbital systems, additional calculations, and expanded scientific functionality.