# Testing & Validation

Baseine inputs:
- Star mass (kg): 1.98847e30
- Planet mass (kg): 5.9722e24
- Initial Position (m): [1.496e11, 0]
- Initial velocity (m/s): [0, 29784.9301982]
- Time step: 60
- Simulation duration: 3600

## Test 1 — Circular orbit

## Input:
- Baseline inputs:

## Expected outputs:
- Specific orbital energy (m^2/s^2): -4.43571033e8
- Specific angular momentum (m^2/s): 4.45582556e15
- Eccentricity: ~0
- Semi-major axis (m): 1.496e11
- Semi-minor axis (m): 1.496e11
- Perihelion distance (m): 1.496e11
- Aphelion distance (m): 1.496e11
- Initial acceleration magnitude (m/s^2): 5.93009403e-3

## Actual:
- Specific orbital energy (m^2/s^2): -4.43571033e8
### Result: Pass
- Specific angular momentum (m^2/s): 4.45582556e15
### Result: Pass
- Eccentricity: ~0
### Result: Pass
- Semi-major axis (m): 1.496e11
### Result: Pass
- Semi-minor axis (m): 1.496e11
### Result: Pass
- Perihelion distance (m): 1.496e11
### Result: Pass
- Aphelion distance (m): 1.496e11
### Result: Pass
- Initial acceleration magnitude (m/s^2): 5.93009403e-3
### Result: Pass

## Test 2 — Moderately eccentric elliptical orbit

## Input:
- Mass of the star (kg): 1.98847e30
- Mass of the planet (kg): 5.9722e24
- Initial position [rx ry] (m): [1.496e11, 0]
- Initial velocity [vx vy] (m/s): [0, 33960.04545]
- Time step (s): 60
- Total simulation time (s): 3600

## Expected outputs:
- Specific orbital energy (m^2/s^2): -3.10499723e8
- Specific angular momentum (m^2/s): 5.08042280e15
- Eccentricity: ~0.3
- Semi-major axis (m): 2.13714286e11
- Semi-minor axis (m): 2.03870435e11
- Perihelion distance (m): 1.496e11
- Aphelion distance (m): 2.77828571e11

## Actual:
- Specific orbital energy (m^2/s^2): -3.10499723e8
### Result: Pass
- Specific angular momentum (m^2/s): 5.08042280e15
### Result: Pass
- Eccentricity: ~0.3
### Result: Pass
- Semi-major axis (m): 2.13714286e11
### Result: Pass
- Semi-minor axis (m): 2.03870435e11
### Result: Pass
- Perihelion distance (m): 1.496e11
### Result: Pass
- Aphelion distance (m): 2.77828571e11
### Result: Pass

## Test 3 — Highly eccentric but bound orbit

## Input: 
- Beseline input but initial velocity = [0, 41055.69299], time step = 10, total simulation time = 600

## Expected outputs:
- Specific orbital energy (m^2/s^2): -4.43571033e7
- Specific angular momentum (m^2/s): 6.14193167e15
- Eccentricity: ~0.9
- Semi-major axis (m): 1.496e12
- Semi-minor axis (m): 6.52091282e11
- Perihelion distance (m): 1.496e11
- Aphelion distance (m): 2.8424e12

## Actual:
- Specific orbital energy (m^2/s^2): -4.43571033e7
### Result: Pass
- Specific angular momentum (m^2/s): 6.14193167e15
### Result: Pass
- Eccentricity: ~0.9
### Result: Pass
- Semi-major axis (m): 1.496e12
### Result: Pass
- Semi-minor axis (m): 6.52091282e11
### Result: Pass
- Perihelion distance (m): 1.496e11
### Result: Pass
- Aphelion distance (m): 2.8424e12
### Result: Pass

## Test 4 — Invalid star mass

## 4A
- Input for star mass: 0
- Expected result: Rejected
### Result: Pass

## 4B
- Input for star mass: -1
- Expected result: Rejected
### Result: Pass

## 4C
- Input for star mass: NaN
- Expected result: Rejected
### Result: Pass

## 4D
- Input for star mass: Inf
- Expected result: Rejected
### Result: Pass

## 4E
- Input for star mass: [1 2]
- Expected result: Rejected
### Result: Pass

## Test 5 — Invalid planet mass

## 5A
- Input for planet mass: 0
- Expected result: Rejected
### Result: Pass

## 5B
- Input for planet mass: -3
- Expected result: Rejected
### Result: Pass

## 5C
- Input for planet mass: NaN
- Expected result: Rejected
### Result: Pass

## 5D
- Input for planet mass: Inf
- Expected result: Rejected
### Result: Pass

## 5E
- Input for planet mass: [1 2]
- Expected result: Rejected
### Result: Pass

## Test 6 — Invalid time step

## 6A
- Input for time step: 0
- Expected result: Rejected
### Result: Pass

## 6B
- Input for time step: -60
- Expected result: Rejected
### Result: Pass

## 6C
- Input for time step: NaN
- Expected result: Rejected
### Result: Pass

## 6D
- Input for time step: Inf
- Expected result: Rejected
### Result: Pass

## 6E
- Input for time step: [10 20]
- Expected result: Rejected
### Result: Pass

## Test 7 — Invalid total simulation time, using time step of 60s

## 7A
- Input for total simulation time: 0
- Expected result: Rejected
### Result: Pass

## 7B
- Input for total simulation time: -60
- Expected result: Rejected
### Result: Pass

## 7C
- Input for total simulation time: 30
- Expected result: Rejected because it is shorter than one time step
### Result: Pass

## 7D
- Input for total simulation time: 100
- Expected result: Rejected because it is shorter than one time step
### Result: Pass

## 7E
- Input for total simulation time: NaN
- Expected result: Rejected
### Result: Pass

## 7F
- Input for total simulation time: Inf
- Expected result: Rejected
### Result: Pass

## Test 8 — Invalid initial position

## 8A
- Input for initial position: [0 0]
- Expected result: Rejected
### Result: Pass

## 8B
- Input for initial position: [1.496e11, NaN]
- Expected result: Rejected
### Result: Pass

## 8C
- Input for initial position: [1.496e11, Inf]
- Expected result: Rejected
### Result: Pass

## 8D
- Input for initial position: [1.496e11]
- Expected result: Rejected
### Result: Pass

## 8E
- Input for initial position: [1.496e11, 0, 1]
- Expected result: Rejected
### Result: Pass

## Test 9 — Invalid initial velocity

## 9A
- Input for initial position: [0, NaN]
- Expected result: Rejected
### Result: Pass

## 9B
- Input for initial velocity: [0, Inf]
- Expected result: Rejected
### Result: Pass

## 9C
- Input for initial velocity: [0]
- Expected result: Rejected
### Result: Pass

## 9D
- Input for initial velocity: [0, 100, 200]
- Expected result: Rejected
### Result: Pass

## Test 10 — Zero initial velocity and radial trajectory

## Input:
- Beseline input but initial velocity = [0, 0]

## Expected result: Initial conditions describe a radial trajectory., reject
## Actual: Initial conditions describe a radial trajectory., reject
### Result: Pass

## Test 11 — Unbound trajectory

- Beseline input but initial velocity = [0, 50000]

## Expected result: Initial conditions must describe a bound elliptical orbit., reject
## Actual: Initial conditions must describe a bound elliptical orbit., reject
### Result: Pass

## Test 12 — Short simulation, less than one orbit

## Input:
- Baseline input

## Expected result:
- Initial and final Velocity Verlet state.
- Initial and final Euler state.
- Analytical orbital parameters.
- Energy and angular momentum errors.
- The orbit and comparison plots.

## Actual:
- Initial and final Velocity Verlet state.
### Result: Pass
- Initial and final Euler state.
### Result: Pass
- Analytical orbital parameters.
### Result: Pass
- Energy and angular momentum errors.
### Result: Pass
- The orbit and comparison plots.
### Result: Pass

## Test 13 — Gravitational acceleration at different positions

## 13A
- Input for initial position: [1.496e11, 0]
- Expected result: 5.93009403e-3
### Result: Pass

## 13B
- Input for initial position: [0, 1.496e11]
- Expected result: 5.93009403e-3
### Result: Pass

## 13C
- Input for initial position: [2.992e11, 0]
- Expected result: 1.48252351e-3
### Result: Pass

## Test 14 — Analytical ellipse geometry

## Input:
- Mass of the star (kg): 1.98847e30
- Mass of the planet (kg): 5.9722e24
- Initial position [rx ry] (m): [1.496e11, 0]
- Initial velocity [vx vy] (m/s): [0, 33960.04545]
- Time step (s): 60
- Total simulation time (s): 3600

## Expected outputs:
- Specific orbital energy (m^2/s^2): -3.10499723e8
- Specific angular momentum (m^2/s): 5.08042280e15
- Eccentricity: ~0.3
- Semi-major axis (m): 2.13714286e11
- Semi-minor axis (m): 2.03870435e11
- Perihelion distance (m): 1.496e11
- Aphelion distance (m): 2.77828571e11

## Actual:
- Specific orbital energy (m^2/s^2): -3.10499723e8
### Result: Pass
- Specific angular momentum (m^2/s): 5.08042280e15
### Result: Pass
- Eccentricity: ~0.3
### Result: Pass
- Semi-major axis (m): 2.13714286e11
### Result: Pass
- Semi-minor axis (m): 2.03870435e11
### Result: Pass
- Perihelion distance (m): 1.496e11
### Result: Pass
- Aphelion distance (m): 2.77828571e11
### Result: Pass

## Test 15 — Orbital parameters against independent reference values

## Input:
- Repeat Tests 1–3 and compare the printed analytical parameters 

## 15A

## Input:
- Baseline inputs:

## Expected outputs:
- Specific orbital energy (m^2/s^2): -4.43571033e8
- Specific angular momentum (m^2/s): 4.45582556e15
- Eccentricity: ~0
- Semi-major axis (m): 1.496e11
- Semi-minor axis (m): 1.496e11
- Perihelion distance (m): 1.496e11
- Aphelion distance (m): 1.496e11
- Initial acceleration magnitude (m/s^2): 5.93009403e-3

## Actual:
- Specific orbital energy (m^2/s^2): -4.43571033e8
### Result: Pass
- Specific angular momentum (m^2/s): 4.45582556e15
### Result: Pass
- Eccentricity: ~0
### Result: Pass
- Semi-major axis (m): 1.496e11
### Result: Pass
- Semi-minor axis (m): 1.496e11
### Result: Pass
- Perihelion distance (m): 1.496e11
### Result: Pass
- Aphelion distance (m): 1.496e11
### Result: Pass
- Initial acceleration magnitude (m/s^2): 5.93009403e-3
### Result: Pass

## 15B

## Input:
- Mass of the star (kg): 1.98847e30
- Mass of the planet (kg): 5.9722e24
- Initial position [rx ry] (m): [1.496e11, 0]
- Initial velocity [vx vy] (m/s): [0, 33960.04545]
- Time step (s): 60
- Total simulation time (s): 3600

## Expected outputs:
- Specific orbital energy (m^2/s^2): -3.10499723e8
- Specific angular momentum (m^2/s): 5.08042280e15
- Eccentricity: ~0.3
- Semi-major axis (m): 2.13714286e11
- Semi-minor axis (m): 2.03870435e11
- Perihelion distance (m): 1.496e11
- Aphelion distance (m): 2.77828571e11

## Actual:
- Specific orbital energy (m^2/s^2): -3.10499723e8
### Result: Pass
- Specific angular momentum (m^2/s): 5.08042280e15
### Result: Pass
- Eccentricity: ~0.3
### Result: Pass
- Semi-major axis (m): 2.13714286e11
### Result: Pass
- Semi-minor axis (m): 2.03870435e11
### Result: Pass
- Perihelion distance (m): 1.496e11
### Result: Pass
- Aphelion distance (m): 2.77828571e11
### Result: Pass

## 15C

## Input: 
- Beseline input but initial velocity = [0, 41055.69299], time step = 10, total simulation time = 600

## Expected outputs:
- Specific orbital energy (m^2/s^2): -4.43571033e7
- Specific angular momentum (m^2/s): 6.14193167e15
- Eccentricity: ~0.9
- Semi-major axis (m): 1.496e12
- Semi-minor axis (m): 6.52091282e11
- Perihelion distance (m): 1.496e11
- Aphelion distance (m): 2.8424e12

## Actual:
- Specific orbital energy (m^2/s^2): -4.43571033e7
### Result: Pass
- Specific angular momentum (m^2/s): 6.14193167e15
### Result: Pass
- Eccentricity: ~0.9
### Result: Pass
- Semi-major axis (m): 1.496e12
### Result: Pass
- Semi-minor axis (m): 6.52091282e11
### Result: Pass
- Perihelion distance (m): 1.496e11
### Result: Pass
- Aphelion distance (m): 2.8424e12
### Result: Pass

## Test 16 — Time-history and summary outputs

## Input:
- Baseline input 

## Expected outputs:
- The initial state is followed by 60 integration steps, ending at 3600 seconds.
- The stored time history contains 61 time points, including the initial time.
- Position, velocity, acceleration, distance, speed, energy, and angular momentum histories have matching time-point counts.
- Minimum and maximum distance outputs are finite.
- Relative energy and angular momentum errors are finite and nonnegative.
- The maximum Euler-versus-Verlet position difference is finite.
- The plots display without errors.

## Actual:
- The initial state is followed by 60 integration steps, ending at 3600 seconds.
### Result: Pass
- The stored time history contains 61 time points, including the initial time.
### Result: Pass
- Position, velocity, acceleration, distance, speed, energy, and angular momentum histories have matching time-point counts.
### Result: Pass
- Minimum and maximum distance outputs are finite.
### Result: Pass
- Relative energy and angular momentum errors are finite and nonnegative.
### Result: Pass
- The maximum Euler-versus-Verlet position difference is finite.
### Result: Pass
- The plots display without errors.
### Result: Pass

## Test 17 — Numerical accuracy and conservation

## Input:
- Baseline input but for 17A - time step = 60, total simulation time = 3600; 17B - time step = 30, total simulation time = 3600; 17C - time step = 10, total simualtion time = 3600

## Expected outputs:
- All three runs reach the same final time.
- Velocity Verlet should generally show small energy and angular momentum errors for this circular-orbit test.
- Reducing the time step should generally improve the numerical trajectory's accuracy.
- The analytical orbital parameters should remain unchanged because the initial conditions are unchanged.