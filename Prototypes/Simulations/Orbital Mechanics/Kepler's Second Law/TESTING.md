# Testing & Validation

Baseine input:
- Star mass (kg): 1.98847e30
- Initial position (m): [1.496e11 0]
- Initial velocity (m/s): [0 20000]
- Integration time step (s): 3600
- Simulation duration (s): 31536000
- Measurement interval (s): 86400

## Test 1 — Circular orbit

## Test 1A — Circular orbital parameters

## Input: Baseline input but velocity = [0 29784.9301982]

## Expected results: 
- Eccentricity: 0
- Semi-major axis (m): 1.496e11
- Semiminor axis (m): 1.496e11
- Perihelion distance (m): 1.496e11
- Aphelion distance (m): 1.496e11

## Actual:
- Eccentricity: 0
### Result: Pass
- Semi-major axis (m): 1.496e11
### Result: Pass
- Semiminor axis (m): 1.496e11
### Result: Pass
- Perihelion distance (m): 1.496e11
### Result: Pass
- Aphelion distance (m): 1.496e11
### Result: Pass

## Test 1B — Constant orbital radius

## Input: Baseline input but velocity = [0 29784.9301982]

## Expected results:
- Initial distance from star (m): 1.496e11
- Expected orbital radius (m): 1.496e11

## Actual:
- Initial distance from star (m): 1.496e11
### Result: Pass
- Expected orbital radius (m): 1.496e11
### Result: Pass

## Input: Baseline input but velocity = [0 29784.9301982]

## Input:
- Baseline input

## Expected results:
- Measurement interval (s): 86400
- Number of measurement intervals: 365
- Number of integration steps: 8760

## Actual:
- Measurement interval (s): 86400
### Result: Pass
- Number of measurement intervals: 365
### Result: Pass
- Number of integration steps: 8760
### Result: Pass

## Test 2 — Moderately eccentric elliptical orbit

## Test 2A — Orbital parameters

## Input: Baseline input

## Expected results:
- Specific orbital energy (J/kg): -6.871420669e8
- Eccentricity: 0.5491139301
- Semi-major axis (m): 9.657133481e10
- Semi-minor axis (m): 8.070921010e10
- Perihelion distance (m): 4.354266962e10
- Aphelion distance (m): 1.496000000e11

## Actual:
- Specific orbital energy (J/kg): -6.871420669e8
### Result: Pass
- Eccentricity: 0.5491139301
### Result: Pass
- Semi-major axis (m): 9.657133481e10
### Result: Pass
- Semi-minor axis (m): 8.070921010e10
### Result: Pass
- Perihelion distance (m): 4.354266962e10
### Result: Pass
- Aphelion distance (m): 1.496000000e11
### Result: Pass

## Test 2B — Aphelion initial condition

## Input: Baseline input

## Expected results:
- Initial distance from star (m): 1.496e11
- Expected apoapsis distance (m): 1.496e11
- Initial radial velocity (m/s): 0

## Actual:
- Initial distance from star (m): 1.496e11
### Result: Pass
- Expected apoapsis distance (m): 1.496e11
### Result: Pass
- Initial radial velocity (m/s): 0
### Result: Pass

## Test 2C — Kepler's Second Law

## Input: Baseline input

## Expected results:
- Measurement interval (s): 86400
- Number of measurement intervals: 365

## Actual:
- Measurement interval (s): 86400
### Result: Pass
- Number of measurement intervals: 365
### Result: Pass

## Test 3 — Highly eccentric but bound orbit

## Test 3A — Highly eccentric orbital parameters

## Input: 
- Star mass (kg): 1.98847e30
- Initial position (m): [1.496e11 0]
- Initial velocity (m/s): [0 41701.0297182]
- Integration time step (s): 3600
- Simulation duration (s): 31536000
- Measurement interval (s): 86400

## Expected results:
- Specific orbital energy (J/kg): Negative
- Eccentricity: ~0.99
- Semimajor axis (m): ~1.496e13
- Perihelion distance (m): ~1.496e11
- Aphelion distance (m): ~2.98e13

## Actual:
- Specific orbital energy (J/kg): Negative
### Result: Pass
- Eccentricity: ~0.99
### Result: Pass
- Semimajor axis (m): ~1.496e13
### Result: Pass
- Perihelion distance (m): ~1.496e11
### Result: Pass
- Aphelion distance (m): ~2.98e13
### Result: Pass

## Test 3B — Numerical stability near a highly eccentric orbit

## Input: 
- Star mass (kg): 1.98847e30
- Initial position (m): [1.496e11 0]
- Initial velocity (m/s): [0 41701.0297182]
- Integration time step (s): 900
- Simulation duration (s): 31536000
- Measurement interval (s): 86400

## Expected results:
- Number of integration steps: 35040

## Test 4 — Invalid inputs

## Test 4A — Zero star mass

## Input:
- Baseline input but star mass = 0

## Expected results: rejected
### Result: Pass

## Test 4B — Negative star mass

## Input:
- Baseline input but star mass = -1.98847e30

## Expected results: rejected
### Result: Pass

## Test 4C — Zero integration timestep

## Input:
- Baseline input but time step = 0

## Expected results: rejected
### Result: Pass

## Test 4D — Negative integration timestep

## Input:
- Baseline input but time step = -3600

## Expected results: rejected
### Result: Pass

## Test 4E — Zero simulation time

## Input:
- Baseline input but simulation time = 0

## Expected results: rejected
### Result: Pass

## Test 4F — Negative simulation duration

## Input:
- Baseline input but simulation time = -31536000

## Expected results: rejected
### Result: Pass

## Test 4G — Invalid initial position vector

## Input:
- Baseline input but initial position = [1.496e11 0 1.0e10]

## Expected results: rejected
### Result: Pass

## Test 4H — Invalid initial velocity vector

## Input:
- Baseline input but initial velocity = [0 20000 1000]

## Expected results: rejected
### Result: Pass

## Test 4I — Invalid measurement interval

## Input:
- Baseline input but measurement interval = 0

## Expected results: rejected
### Result: Pass

## Test 5 — Zero initial velocity and non-elliptical motion

## Test 5A — Zero initial velocity

## Input:
- Star mass (kg): 1.98847e30
- Initial position (m): [1.496e11 0]
- Initial velocity (m/s): [0 0]
- Integration time step (s): 60
- Simulation duration (s): 3600
- Measurement interval (s): 600

## Expected results:
- Initial speed (m/s): 0
- Initial acceleration magnitude (m/s^2): 5.930094030e-3
- Initial acceleration direction: Toward the central star
- Initial orbital energy (J/kg): Negative

## Actual:
- Initial speed (m/s): 0
### Result: Pass
- Initial acceleration magnitude (m/s^2): 5.930094030e-3
### Result: Pass
- Initial acceleration direction: Toward the central star
### Result: Pass
- Initial orbital energy (J/kg): Negative
### Result: Pass

## Test 5B — Unbound trajectory

## Input:
- Star mass (kg): 1.98847e30
- Initial position (m): [1.496e11 0]
- Initial velocity (m/s): [0 45000]
- Integration time step (s): 3600
- Simulation duration (s): 31536000
- Measurement interval (s): 86400

## Expected results:
- Specific orbital energy (J/kg): Positive
- Eccentricity: >1

## Actual:
- Specific orbital energy (J/kg): Positive
### Result: Pass
- Eccentricity: >1
### Result: Pass

## Test 6 — Trajectories that do not complete an orbit

## Test 6A — Short circular-orbit simulation

## Input:
- Star mass (kg): 1.98847e30
- Initial position (m): [1.496e11 0]
- Initial velocity (m/s): [0 29784.9301982]
- Integration time step (s): 3600
- Simulation duration (s): 2592000
- Measurement interval (s): 86400

## Expected results:
- Number of integration steps: 720
- Number of measurement intervals: 30

## Actual:
- Number of integration steps: 720
### Result: Pass
- Number of measurement intervals: 30
### Result: Pass

## Test 6B — Short eccentric-orbit simulation

## Input:
- Baseline input but simulation duration = 864000 and measurement interval = 86400

## Expected results:
- Number of integration steps: 240
- Number of measurement intervals: 10

## Actual:
- Number of integration steps: 240
### Result: Pass
- Number of measurement intervals: 10
### Result: Pass

## Test 7 — Ellipse geometry formulas

## Test 7A — Semi-minor axis

## Input:
- Semi-major axis (m): 10
- Eccentricity: 0.6

## Expected results:
- Semi-minor axis (m): 8

## Actual:
- Semi-minor axis (m): 8
### Result: Pass

## Test 7B — Focal distance

## Input:
- Semi-major axis (m): 10
- Eccentricity: 0.6

## Expected results:
- Focal distance from ellipse centre (m): 6

## Actual: 
- Focal distance from ellipse centre (m): 6
### Result: Pass

## Test 7C — Perihelion and aphelion distances

## Input:
- Semi-major axis (m): 10
- Eccentricity: 0.6

## Expected results:
- Perihelion distance (m): 4
- Aphelion distance (m): 16

## Actual:
- Perihelion distance (m): 4
### Result: Pass
- Aphelion distance (m): 16
### Result: Pass

## Test 7D — Circular geometry

## Input: 
- Semi-major axis (m): 10
- Eccentricity: 0

## Expected results:
- Semi-minor axis (m): 10
- Focal distance (m): 0
- Perihelion distance (m): 10
- Aphelion distance (m): 10

## Actual:
- Semi-minor axis (m): 10
### Result: Pass
- Focal distance (m): 0
### Result: Pass
- Perihelion distance (m): 10
### Result: Pass
- Aphelion distance (m): 10
### Result: Pass

## Test 8 — Gravitational acceleration at known positions

## Test 8A — Acceleration at the baseline initial position

## Input:
- Star mass (kg): 1.98847e30
- Position (m): [1.496e11 0]

## Expected results:
- Acceleration x-component (m/s^2): -5.930094030e-3
- Acceleration y-component (m/s^2): 0
- Acceleration magnitude (m/s^2): 5.930094030e-3

## Actual:
- Acceleration x-component (m/s^2): -5.930094030e-3
### Result: Pass
- Acceleration y-component (m/s^2): 0
### Result: Pass
- Acceleration magnitude (m/s^2): 5.930094030e-3
### Result: Pass

## Test 8B — Acceleration at twice the initial distance

## Input:
- Star mass (kg): 1.98847e30
- Position (m): [2.992e11 0]

## Expected results:
- Acceleration x-component (m/s^2): -1.482523508e-3
- Acceleration y-component (m/s^2): 0
- Acceleration magnitude (m/s^2): 1.482523508e-3

## Actual:
- Acceleration x-component (m/s^2): -1.482523508e-3
### Result: Pass
- Acceleration y-component (m/s^2): 0
### Result: Pass
- Acceleration magnitude (m/s^2): 1.482523508e-3
### Result: Pass

## Test 8C — Acceleration on the y-axis

## Input:
- Star mass (kg): 1.98847e30
- Position (m): [0 1.496e11]

## Expected results:
- Acceleration x-component (m/s^2): 0
- Acceleration y-component (m/s^2): -5.930094030e-3

## Actual:
- Acceleration x-component (m/s^2): 0
### Result: Pass
- Acceleration y-component (m/s^2): -5.930094030e-3
### Result: Pass

## Test 9 — Orbital parameters against independent reference values

## Test 9A — Baseline analytical solution

## Input:
- Baseline input

## Expected results:
- Gravitational parameter (m^3/s^2): 1.327164532e20
- Specific orbital energy (J/kg): -6.871420669e8
- Specific angular momentum (m^2/s): 2.992000000e15
- Semimajor axis (m): 9.657133481e10
- Eccentricity: 0.5491139301
- Semi-minor axis (m): 8.070921010e10
- Perihelion distance (m): 4.354266962e10
- Aphelion distance (m): 1.496000000e11

## Actual: 
- Gravitational parameter (m^3/s^2): 1.327164532e20
### Result: Pass
- Specific orbital energy (J/kg): -6.871420669e8
### Result: Pass
- Specific angular momentum (m^2/s): 2.992000000e15
### Result: Pass
- Semimajor axis (m): 9.657133481e10
### Result: Pass
- Eccentricity: 0.5491139301
### Result: Pass
- Semi-minor axis (m): 8.070921010e10
### Result: Pass
- Perihelion distance (m): 4.354266962e10
### Result: Pass
- Aphelion distance (m): 1.496000000e11
### Result: Pass

## Test 10 — Time-history dimensions and summary statistics

## Test 10A — Position, velocity and acceleration histories

## Input:
- Baseline input

## Expected results:
- Integration steps: 8760
- Time-history entries: 8761
- Position history dimensions: 8761 rows, 2 columns
- Velocity history dimensions: 8761 rows, 2 columns
- Acceleration history dimensions: 8761 rows, 2 columns

## Actual: 
- Integration steps: 8760
### Result: Pass
- Time-history entries: 8761
### Result: Pass
- Position history dimensions: 8761 rows, 2 columns
### Result: Pass
- Velocity history dimensions: 8761 rows, 2 columns
### Result: Pass
- Acceleration history dimensions: 8761 rows, 2 columns
### Result: Pass

## Test 10B — Time-history endpoints

## Input:
- Baseline input

## Expected results:
- Initial time (s): 0
- Final time (s): 31536000
- Initial position (m): [1.496e11 0]
- Initial velocity (m/s): [0 20000]

## Actual:
- Initial time (s): 0
### Result: Pass
- Final time (s): 31536000
### Result: Pass
- Initial position (m): [1.496e11 0]
### Result: Pass
- Initial velocity (m/s): [0 20000]
### Result: Pass

## Test 10C — Measurement-interval summaries

## Input:
- Baseline input

## Expected results:
- Measurement interval (s): 86400
- Number of measurement intervals: 365
- First interval start time (s): 0
- First interval end time (s): 86400
- Last interval end time (s): 31536000

## Actual:
- Measurement interval (s): 86400
### Result: Pass
- Number of measurement intervals: 365
### Result: Pass
- First interval start time (s): 0
### Result: Pass
- First interval end time (s): 86400
### Result: Pass
- Last interval end time (s): 31536000
### Result: Pass

## Test 10D — Initial angular momentum

## Input:
- Baseline input

## Expected results: 
- Initial specific angular momentum (m^2/s): 2.992000000e15
- Initial areal velocity (m^2/s): 1.496000000e15

## Actual: 
- Initial specific angular momentum (m^2/s): 2.992000000e15
### Result: Pass
- Initial areal velocity (m^2/s): 1.496000000e15
### Result: Pass

## Test 11 — Numerical convergence using several timesteps

## Test 11A — Timestep comparison inputs

## Input:
- Star mass (kg): 1.98847e30
- Initial position (m): [1.496e11 0]
- Initial velocity (m/s): [0 20000]
- Integration time step (s): 3600
- Simulation duration (s): 31536000
- Measurement interval (s): 86400
- Comparison timesteps (s): [7200 3600 1800 900]

## Expected results: 
- Steps at 7200 s: 4380
- Steps at 3600 s: 8760
- Steps at 1800 s: 17520
- Steps at 900 s: 35040

## Actual:
- Steps at 7200 s: 4380
### Result: Pass
- Steps at 3600 s: 8760
### Result: Pass
- Steps at 1800 s: 17520
### Result: Pass
- Steps at 900 s: 35040
### Result: Pass

## Test 11B — Position-error comparison

## Input:
- Star mass (kg): 1.98847e30
- Initial position (m): [1.496e11 0]
- Initial velocity (m/s): [0 20000]
- Integration time step (s): 3600
- Simulation duration (s): 31536000
- Measurement interval (s): 86400
- Comparison timesteps (s): [7200 3600 1800 900]

## Expected results: 
- Velocity Verlet: Maximum and final position errors calculated for all four timesteps.
- Semi-implicit Euler: Maximum and final position errors calculated for all four timesteps.

## Actual:
- Velocity Verlet: Maximum and final position errors calculated for all four timesteps.
### Result: Pass
- Semi-implicit Euler: Maximum and final position errors calculated for all four timesteps.
### Result: Pass

## Test 11C — Identical comparison conditions

## Input:
- Star mass (kg): 1.98847e30
- Initial position (m): [1.496e11 0]
- Initial velocity (m/s): [0 20000]
- Integration time step (s): 3600
- Simulation duration (s): 31536000
- Measurement interval (s): 86400
- Comparison timesteps (s): [7200 3600 1800 900]

## Expected results: 
- Initial position: Identical for both methods
- Initial velocity: Identical for both methods
- Star mass: Identical for both methods
- Simulation duration: Identical for both methods
- Comparison timesteps: Identical for both methods

## Actual:
- Initial position: Identical for both methods
### Result: Pass
- Initial velocity: Identical for both methods
### Result: Pass
- Star mass: Identical for both methods
### Result: Pass
- Simulation duration: Identical for both methods
### Result: Pass
- Comparison timesteps: Identical for both methods
### Result: Pass

## Test 12 — Numerical integration behaviour

## Test 2A — Velocity Verlet and Euler initial states

## Input: 
- Baseline input

## Expected results: 
- Velocity Verlet initial position (m): [1.496e11 0]
- Euler initial position (m): [1.496e11 0]
- Velocity Verlet initial velocity (m/s): [0 20000]
- Euler initial velocity (m/s): [0 20000]

## Actual:
- Velocity Verlet initial position (m): [1.496e11 0]
### Result: Pass
- Euler initial position (m): [1.496e11 0]
### Result: Pass
- Velocity Verlet initial velocity (m/s): [0 20000]
### Result: Pass
- Euler initial velocity (m/s): [0 20000]
### Result: Pass

## Test 12B — Finite numerical histories

## Input: 
- Baseline input

## Expected results: 
- Velocity Verlet positions: All finite
- Velocity Verlet velocities: All finite
- Velocity Verlet accelerations: All finite
- Euler positions: All finite
- Euler velocities: All finite
- Euler accelerations: All finite

## Actual:
- Velocity Verlet positions: All finite
### Result: Pass
- Velocity Verlet velocities: All finite
### Result: Pass
- Velocity Verlet accelerations: All finite
### Result: Pass
- Euler positions: All finite
### Result: Pass
- Euler velocities: All finite
### Result: Pass
- Euler accelerations: All finite
### Result: Pass

## Test 12C — Energy and angular-momentum behaviour

## Input: 
- Baseline input

## Expected results: 
- Initial specific orbital energy: Negative
- Initial specific angular momentum: 2.992000000e15 m^2/s
- Velocity Verlet: Energy and angular-momentum drift measured.
- Semi-implicit Euler: Energy and angular-momentum drift measured.

## Actual:
- Initial specific orbital energy: Negative
### Result: Pass
- Initial specific angular momentum: 2.992000000e15 m^2/s
### Result: Pass
- Velocity Verlet: Energy and angular-momentum drift measured.
### Result: Pass
- Semi-implicit Euler: Energy and angular-momentum drift measured.
### Result: Pass

## Test 13 — Analytical calculation tolerances

## Input: 
- Baseline input

## Expected results:
- Gravitational acceleration relative tolerance: 1e-6
- Orbital parameter relative tolerance: 1e-6
- Simple ellipse geometry absolute tolerance: 1e-10 m
- Initial position and velocity: Exact match
- Time-history dimensions: Exact match

## Actual:
- Gravitational acceleration relative tolerance: 1e-6
### Result: Pass
- Orbital parameter relative tolerance: 1e-6
### Result: Pass
- Simple ellipse geometry absolute tolerance: 1e-10 m
### Result: Pass
- Initial position and velocity: Exact match
### Result: Pass
- Time-history dimensions: Exact match
### Result: Pass