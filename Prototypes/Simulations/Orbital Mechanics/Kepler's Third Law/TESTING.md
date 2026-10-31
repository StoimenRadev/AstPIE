# Testing & Validation

Baseine input:
- Star mass (kg): 1.98847e30
- Planet mass (kg): 5.9722e24
- Initial position (m): [1.496e11 0]
- Initial velocity (m/s): [0, 29780]
- Integration time step (s): 3600
- Simulation duration (s): 63072000

## Test 1 — Analytical orbital calculations

## Input:
- Baseline input

## Expected results:
- Gravitational parameter: 1.32716852e20 m^3/s^2
- Specific orbital energy: -4.43720531e8 J/kg
- Specific angular momentum: 4.45508800e15 m^2/s.
- Semi-major axis: 1.49550046e11 m.
- Eccentricity: 0.000334028.
- Analytical orbital period: 31542539.78 s, or 365.075692 days.

## Actual:
- Gravitational parameter: 1.32716852e20 m^3/s^2
### Result: Pass
- Specific orbital energy: -4.43720531e8 J/kg
### Result: Pass
- Specific angular momentum: 4.45508800e15 m^2/s.
### Result: Pass
- Semi-major axis: 1.49550046e11 m.
### Result: Pass
- Eccentricity: 0.000334028.
### Result: Pass
- Analytical orbital period: 31542539.78 s, or 365.075692 days.
### Result: Pass

## Test 2 — Velocity Verlet: Earth–Sun reference orbit

## Input: 
- Baseline input

## Expected results:
- The numerical trajectory remains bounded.
- The planet completes at least one full revolution.
- The measured orbital period is close to 365.075692 days.
- The positional error remains finite.
- Specific orbital energy and angular momentum remain close to their initial values.

## Actual:
- The numerical trajectory remains bounded.
### Result: Pass
- The planet completes at least one full revolution.
### Result: Pass
- The measured orbital period is close to 365.075692 days.
### Result: Pass
- The positional error remains finite.
### Result: Pass
- Specific orbital energy and angular momentum remain close to their initial values.
### Result: Pass

## Test 3 — Semi-Implicit Euler: Earth–Sun reference orbit

## Input: 
- Baseline input

## Expected results:
- The numerical trajectory remains bounded.
- The planet completes at least one full revolution.
- The measured orbital period is finite and close to the analytical period.
- Position and energy errors remain finite.
- Specific angular momentum remains close to its initial value.

## Actual:
- The numerical trajectory remains bounded.
### Result: Pass
- The planet completes at least one full revolution.
### Result: Pass
- The measured orbital period is finite and close to the analytical period.
### Result: Pass
- Position and energy errors remain finite.
### Result: Pass
- Specific angular momentum remains close to its initial value.
### Result: Pass

## Test 4 — Circular orbit

## Input: 
- Baseline input but velocity = [0, 29784.9749264168]

## Expected results:
- Eccentricity: ~0
- Semi-major axis: 1.496e11 m
- Perihelion and apohelion distances: both 1.496e11 m
- Analytical orbital period: ~365.258626 days
- Analytical trajectory: circular

## Actual:
- Eccentricity: ~0
### Result: Pass
- Semi-major axis: 1.496e11 m
### Result: Pass
- Perihelion and apohelion distances: both 1.496e11 m
### Result: Pass
- Analytical orbital period: ~365.258626 days
### Result: Pass
- Analytical trajectory: circular
### Result: Pass

## Test 5 — Elliptical orbit

## Input: 
- Baseline input but velocity = [0, 25000]

## Expected results:
- Specific orbital energy:-5.74644731e8 J/kg.
- Semi-major axis: 1.15477307e11 m.
- Eccentricity: ~0.295493.
- Perihelion distance: 8.13546134e10 m.
- Aphelion distance: 1.496e11 m.
- Analytical orbital period: 247.712578 days.

## Actual:
- Specific orbital energy:-5.74644731e8 J/kg.
### Result: Pass
- Semi-major axis: 1.15477307e11 m.
### Result: Pass
- Eccentricity: ~0.295493.
### Result: Pass
- Perihelion distance: 8.13546134e10 m.
### Result: Pass
- Aphelion distance: 1.496e11 m.
### Result: Pass
- Analytical orbital period: 247.712578 days.
### Result: Pass

## Test 6 — Different orbital eccentricities

## Test 6A — Lower eccentricity

## Input: 
- Baseline input but velocity = [0, 29000]

## Expected results:
- Eccentricity: ~0.052015.
- Orbital period: 338.507 days.

## Actual:
- Eccentricity: ~0.052015.
### Result: Pass
- Orbital period: 338.507 days.
### Result: Pass

## Test 6B — Higher eccentricity

## Input: 
- Baseline input but velocity = [0, 28000]

## Expected results:
- Eccentricity: ~0.116266.
- Orbital period: 309.706 days.

## Actual:
- Eccentricity: ~0.116266.
### Result: Pass
- Orbital period: 309.706 days.
### Result: Pass

## Test 6C — More eccentric orbit

## Input: 
- Baseline input but velocity = [0, 25000]

## Expected results:
- Eccentricity: ~0.295493.
- Orbital period: 247.713 days.

## Actual:
- Eccentricity: ~0.295493.
### Result: Pass
- Orbital period: 247.713 days.
### Result: Pass

## Test 7 — Different timestep sizes

## Test 7A — Smaller timestep

## Input:
- Baseline input but time step = 1800 and total simuation time = 63072000
## Expected results: accept
### Result: Pass

## Test 7B — Baseline timestep

## Input:
- Baseline input but time step = 3600 and total simuation time = 63072000
## Expected results: accept
### Result: Pass

## Test 7C — Larger timestep

## Input:
- Baseline input but time step = 7200 and total simuation time = 63072000
## Expected results: accept
### Result: Pass

## Test 8 — Different total system masses

## Input:
- Baseline input but star mass = 3.97694e30 and planet mass = 5.9722e24

## Expected results: 
- The gravitational parameter approximately doubles.
- The initial specific orbital energy becomes more negative.
- The semi-major axis changes.
- The analytical orbital period becomes shorter than the baseline period.
- The initial conditions still describe a bound elliptical orbit.

## Actual:
- The gravitational parameter approximately doubles.
### Result: Pass
- The initial specific orbital energy becomes more negative.
### Result: Pass
- The semi-major axis changes.
### Result: Pass
- The analytical orbital period becomes shorter than the baseline period.
### Result: Pass
- The initial conditions still describe a bound elliptical orbit.
### Result: Pass

## Test 9 — Different semi-major axes

## Test 9A — 1 AU

## Input:
- Baseline input but position = [1.496e11, 0] and velocity = [0, 29784.9749264168]

## Expected results: 
- Expected period: 365.258626 days.

## Actual:
- Expected period: 365.258626 days.
### Result: Pass

## Test 9B — 2 AU

## Input:
- Baseline input but position = [2.992e11, 0] and velocity = [0, 21061.4]

## Expected results: 
- Expected period: 1033.13 days.

## Actual:
- Expected period: 1033.13 days.
### Result: Pass

## Test 9C — 0.5 AU

## Input:
- Baseline input but position = [7.48e10, 0] and velocity = [0, 42122.8]

## Expected results: 
- Expected period: 129.14 days.

## Actual:
Expected period: 129.14 days.
### Result: Pass

## Test 10 — Orbital period measurement

## Input: 
- Baseline input

## Expected results: 
- Both numerical methods detect a complete revolution.
- Both measured periods are finite.
- Both measured periods are close to the analytical period.
- Period errors are nonnegative.
- Relative and percentage errors are calculated consistently.

## Actual:
- Both numerical methods detect a complete revolution.
### Result: Pass
- Both measured periods are finite.
### Result: Pass
- Both measured periods are close to the analytical period.
### Result: Pass
- Period errors are nonnegative.
### Result: Pass
- Relative and percentage errors are calculated consistently.
### Result: Pass

## Test 11 — Incomplete orbital trajectories

## Input: 
- Baseline input but total simulation time = 8640000

## Expected results: 
- Neither method completes a full revolution.
- verlet_measured_period remains NaN.
- euler_measured_period remains NaN.
- The Command Window displays the incomplete-revolution messages.
- Period errors remain NaN.
- The orbital period comparison graph is not generated.

## Actual:
- Neither method completes a full revolution.
### Result: Pass
- verlet_measured_period remains NaN.
### Result: Pass
- euler_measured_period remains NaN.
### Result: Pass
- The Command Window displays the incomplete-revolution messages.
### Result: Pass
- Period errors remain NaN.
### Result: Pass
- The orbital period comparison graph is not generated.
### Result: Pass

## Test 12 — Input validation and invalid values

## Test 12A — Invalid star mass
## Input:
- Baseline input but star mass = -1
## Expected results: reject
### Result: Pass

## Test 12B — Invalid planet mass
## Input:
- Baseline input but planet mass = 0
## Expected results: reject
### Result: Pass

## Test 12C — Invalid initial position
## Input:
- Baseline input but initial position = [0, 0]
## Expected results: reject
### Result: Pass

## Test 12D — Invalid initial velocity dimensions
## Input:
- Baseline input but initial velcoity = [0, 29780, 100]
## Expected results: reject
### Result: Pass

## Test 12E — Unbound orbit
## Input:
- Baseline input but initial velcoity = [0, 50000]
## Expected results: reject
### Result: Pass

## Test 12F — Zero angular momentum
## Input:
- Baseline input but initial velcoity = [29780, 0]
## Expected results: reject
### Result: Pass

## Test 12G — Invalid timestep
## Input:
- Baseline input but time step = -3600
## Expected results: reject
### Result: Pass

## Test 12H — Invalid simulation duration
## Input:
- Baseline input but total simulation time = 0
## Expected results: reject
### Result: Pass

## Test 12I — Non-integer number of integration steps
## Input:
- Baseline input but time step = 3600 and total simulation time = 63072001
## Expected results: reject
### Result: Pass

## Test 13 — Error calculations and numerical tolerances

## Test 13A — Position error

## Input: 
- Baseline input

## Expected results: 
- Initial position error for both methods: approximately zero.
- Final and maximum position errors: finite and nonnegative.
- Position errors correspond to the differences between numerical and analytical positions.

## Actual:
- Initial position error for both methods: approximately zero.
### Result: Pass
- Final and maximum position errors: finite and nonnegative.
### Result: Pass
- Position errors correspond to the differences between numerical and analytical positions.
### Result: Pass

## Test 13B — Orbital period error

## Input: 
- Baseline input

## Expected results: 
- Absolute error equals the absolute difference between the measured and analytical periods.
- Relative error equals absolute error divided by the analytical period.
- Percentage error equals relative error multiplied by 100.

## Actual:
- Absolute error equals the absolute difference between the measured and analytical periods.
### Result: Pass
- Relative error equals absolute error divided by the analytical period.
### Result: Pass
- Percentage error equals relative error multiplied by 100.
### Result: Pass

## Test 13C — Specific orbital energy conservation

## Input: 
- Baseline input

## Expected results: 
- Both methods produce finite energy histories.
- The displayed energy changes agree with the difference between the initial and final specific orbital energies.
- Energy conservation is not assumed to be exact in numerical integration.

## Actual:
- Both methods produce finite energy histories.
### Result: Pass
- The displayed energy changes agree with the difference between the initial and final specific orbital energies.
### Result: Pass
- Energy conservation is not assumed to be exact in numerical integration.
### Result: Pass

## Test 13D — Specific angular momentum conservation

## Input: 
- Baseline input

## Expected results: 
- Both methods produce finite angular momentum histories.
- The displayed angular momentum changes agree with the initial and final values.
- Relative angular momentum changes remain close to zero, subject to floating-point precision.

## Actual:
- Both methods produce finite angular momentum histories.
### Result: Pass
- The displayed angular momentum changes agree with the initial and final values.
### Result: Pass
- Relative angular momentum changes remain close to zero, subject to floating-point precision.
### Result: Pass

## Test 14 — Stored data dimensions and indexing

## Test 14A — Initial state

## Input: 
- Baseline input

## Expected results: 
- The first position entries match the user-defined initial position.
- The first velocity entries match the user-defined initial velocity.
- The first time entry is zero.

## Actual: 
- The first position entries match the user-defined initial position.
### Result: Pass
- The first velocity entries match the user-defined initial velocity.
### Result: Pass
- The first time entry is zero.
### Result: Pass

## Test 14B — Final state

## Input: 
- Baseline input

## Expected results: 
- The final time entry equals the requested simulation duration.
- The final recorded position and velocity correspond to the final numerical integration step.

## Actual: 
- The final time entry equals the requested simulation duration.
### Result: Pass
- The final recorded position and velocity correspond to the final numerical integration step.
### Result: Pass

## Test 14C — History dimensions

## Input: 
- Baseline input

## Expected results: 
- Every history contains num_steps + 1 recorded states.
- No history has missing or additional entries.

## Actual: 
- Every history contains num_steps + 1 recorded states.
### Result: Pass
- No history has missing or additional entries.
### Result: Pass

## Test 15 — Graphical consistency

## Test 15A — Orbital trajectory graph

## Input: 
- Baseline input

## Expected results: 
- Analytical, Velocity Verlet, and Semi-Implicit Euler trajectories are displayed.
- The star is located at the origin.
- Both axes use the same scale.
- The analytical trajectory is distinguishable from the numerical trajectories.

## Actual: 
- Analytical, Velocity Verlet, and Semi-Implicit Euler trajectories are displayed.
### Result: Pass
- The star is located at the origin.
### Result: Pass
- Both axes use the same scale.
### Result: Pass
- The analytical trajectory is distinguishable from the numerical trajectories.
### Result: Pass

## Test 15B — Star–planet distance graph

## Input: 
- Baseline input

## Expected results: 
- All three distance histories are displayed.
- The horizontal axis represents time in days.
- The vertical axis represents distance in AU.
- The analytical reference is visible, even when numerical results overlap it.

## Actual: 
- All three distance histories are displayed.
### Result: Pass
- The horizontal axis represents time in days.
### Result: Pass
- The vertical axis represents distance in AU.
### Result: Pass
- The analytical reference is visible, even when numerical results overlap it.
### Result: Pass

## Test 15C — Specific orbital energy graph

## Input: 
- Baseline input

## Expected results: 
- Both numerical energy histories are displayed.
- The initial analytical specific orbital energy is displayed as a reference line.
- The graph uses the correct time and energy units.

## Actual: 
- Both numerical energy histories are displayed.
### Result: Pass
- The initial analytical specific orbital energy is displayed as a reference line.
### Result: Pass
- The graph uses the correct time and energy units.
### Result: Pass

## Test 15D — Specific angular momentum graph

## Input: 
- Baseline input

## Expected results: 
- Both numerical angular momentum histories are displayed as relative changes.
- The initial angular momentum is represented by the zero reference line.
- The graph displays dimensionless relative changes.

## Actual: 
- Both numerical angular momentum histories are displayed as relative changes.
### Result: Pass
- The initial angular momentum is represented by the zero reference line.
### Result: Pass
- The graph displays dimensionless relative changes.
### Result: Pass

## Test 15E — Numerical position error graph

## Input: 
- Baseline input

## Expected results: 
- Both numerical position error histories are displayed.
- The horizontal axis represents time in days.
- The vertical axis represents positional error in kilometres.
- All plotted values correspond to the calculated position errors.

## Actual: 
- Both numerical position error histories are displayed.
### Result: Pass
- The horizontal axis represents time in days.
### Result: Pass
- The vertical axis represents positional error in kilometres.
### Result: Pass
- All plotted values correspond to the calculated position errors.
### Result: Pass

## Test 15F — Orbital period comparison graph

## Input: 
- Baseline input

## Expected results: 
- The analytical period is displayed.
- Both numerical periods are displayed when both methods complete a revolution.
- All values are expressed in days.
- The plotted values agree with the corresponding Command Window results.

## Actual: 
- The analytical period is displayed.
### Result: Pass
- Both numerical periods are displayed when both methods complete a revolution.
### Result: Pass
- All values are expressed in days.
### Result: Pass
- The plotted values agree with the corresponding Command Window results.
### Result: Pass

## Test 15G — Orbital parameter summary

## Input: 
- Baseline input

## Expected results: 
- The semi-major axis, eccentricity, analytical period, periapsis, apoapsis, star mass, and planet mass are displayed.
- All displayed values correspond to the analytical calculations.

## Actual: 
- The semi-major axis, eccentricity, analytical period, periapsis, apoapsis, star mass, and planet mass are displayed.
### Result: Pass
- All displayed values correspond to the analytical calculations.
### Result: Pass