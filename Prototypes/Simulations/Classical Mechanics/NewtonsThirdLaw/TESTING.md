# Testing & Validation

Baseline inputs:
Number of bodies: 2
Time step: 0.1
Total simulation time: 1
Body 1:
- Mass: 2
- Initial position: [0 0]
- Initial velocity: [1 0]
- Interaction force: [4 0]
- External force: [0 0]
Body2:
- Mass: 4
- Initial position: [10 0]
- Initial velocity: [0 0]
- Interaction force: [-4 0]
- External force: [0 0]

## Test 1 — Number of bodies

## 1A
- Input for n: 2
- Expected result: Accepted
### Result - Pass

## 1B
- Input for n: 1
- Expected result: Error: exactly 2 bodies required
### Result - Pass

## 1C
- Input for n: 3
- Expected result: Error: exactly 2 bodies required
### Result - Pass

## 1D
- Input for n: 0
- Expected result: Error
### Result - Pass

## 1E
- Input for n: -2
- Expected result: Error
### Result - Pass

## 1F
- Input for n: 2.5
- Expected result: Error
### Result - Pass

## Test 2 — Mass validation

## 2A
- Input for Mass (kg): 2
- Expected result: Accepted
### Result - Pass

## 2B
- Input for Mass (kg): 0
- Expected result: Error: mass must be positive
### Result - Pass

## 2C
- Input for Mass (kg): -2
- Expected result: Error
### Result - Pass

## 2D
- Input for Mass (kg): Inf
- Expected result: Error
### Result - Pass

## 2E
- Input for Mass (kg): NaN
- Expected result: Error
### Result - Pass

## Test 3 — Initial positions and velocities

## 3A
- Input for Position: [0 0]
- Expected result: Accepted
### Result - Pass

## 3B
- Input for Velocity: [1 0]
- Expected result: Accepted
### Result - Pass

## 3C
- Input for Position: [0 NaN]
- Expected result: Validation error
### Result - Pass

## 3D
- Input for Velocity: [1 Inf]
- Expected result: Validation error
### Result - Pass

## 3E
- Input for Position: [0 0 1]
- Expected result: Must reject the 3-component vector
### Result - Pass

## 3F
- Input for Velocity: 1
- Expected result: Must reject the 1-component/scalar vector
### Result - Pass

## Test 4 — Time-step validation

## 4A
- Input for Time-step: 0.1
- Expected result: Accepted
### Result - Pass

## 4B
- Input for Time-step: 0
- Expected result: Error
### Result - Pass

## 4C
- Input for Time-step: -0.1
- Expected result: Error
### Result - Pass

## 4D
- Input for Time-step: Inf
- Expected result: Error
### Result - Pass

## Test 5 — Total simulation time

## 5A
- Input for Total simulation time: 1
- Expected result: Accepted
### Result - Pass

## 5B
- Input for Total simulation time: 0
- Expected result: Error
### Result - Pass

## 5C
- Input for Total simulation time: -1
- Expected result: Error
### Result - Pass

## 5D
- Input for Total simulation time: 1.05
- Expected result: Error because it is not an integer multiple of dt
### Result - Pass

## Test 6 — Interaction forces: valid directions

## 6A
- Input for interaction forces: Body 1 force (N): [4 0] Body 2 force (N): [-4 0]
- Expected result: Accepted
### Result - Pass

## 6B
- Input for interaction forces: Body 1 force (N): [0 4] Body 2 force (N): [0 -4]
- Expected result: Accepted
### Result - Pass

## 6C
- Input for interaction forces: Body 1 force (N): [3 4] Body 2 force (N): [-3 -4]
- Expected result: Accepted
### Result - Pass

## Test 7 — Interaction forces: invalid pairs

## 7A
- Input for interaction forces: Body 1 force (N): [4 0] Body 2 force (N): [0 0]
- Expected result: Newton's Third Law validation error
### Result - Pass

## 7B
- Input for interaction forces: Body 1 force (N): [4 0] Body 2 force (N): [-3 0]
- Expected result: Newton's Third Law validation error
### Result - Pass

## 7C
- Input for interaction forces: Body 1 force (N): [NaN 0] Body 2 force (N): [-4 0]
- Expected result: Force validation error
### Result - Pass

## 7D
- Input for interaction forces: Body 1 force (N): [4 0 1] Body 2 force (N): [-4 0]
- Expected result: Force validation error
### Result - Pass

## Test 8 — External forces

## 8A
- Input for external forces: Body 1 external force (N): [0 0] Body 2 force (N): [0 0]
- Expected result: Accepted
### Result - Pass

## 8B
- Input for external forces: Body 1 external force (N): [2 3] Body 2 force (N): [-1 4]
- Expected result: Accepted
### Result - 

## 8C
- Input for external forces: Body 1 external force (N): [Inf 0] Body 2 force (N): [0 0]
- Expected result: Force validation error
### Result - Pass

## 8D
- Input for external forces: Body 1 external force (N): [1 2 3] Body 2 force (N): [0 0]
- Expected result: Force validation error
### Result - Pass

## Test 9 — Baseline physics and final results

### Input:
- Baseline inputs

### Expected final results:
- Acceleration (m/s²): Body 1: [2 0]; Body 2: [-1 0]
- Final position (m): Body 1: [2 0]; Body 2: [9.5 0]
- Final velocity (m/s): Body 1: [3 0]; Body 2: [-1 0]
- Final speed (m/s): Body 1: 3; Body 2: 1

### Actual:
- Acceleration (m/s²): Body 1: [2 0]  Body 2: [-1 0]
### Result: Pass
- Final position (m): Body 1: [2 0]; Body 2: [9.5 0]
### Result: Pass
- Final velocity (m/s): Body 1: [3 0]; Body 2: [-1 0]
### Result: Pass
- Final speed (m/s): Body 1: 3; Body 2: 1
### Result: Pass

## Test 10 — Equal masses

### Input:
- Baseline inputs but mass on body 2 = 2

### Expected final results:
- Acceleration (m/s²): Body 1: [2 0]; Body 2: [-2 0]
- Final position (m): Body 1: [2 0]; Body 2: [9 0]
- Final velocity (m/s): Body 1: [3 0]; Body 2: [-2 0]
- Final speed (m/s): Body 1: 3; Body 2: 1

### Actual:
- Acceleration (m/s²): Body 1: [2 0]  Body 2: [-2 0]
### Result: Pass
- Final position (m): Body 1: [2 0]; Body 2: [9 0]
### Result: Pass
- Final velocity (m/s): Body 1: [3 0]; Body 2: [-2 0]
### Result: Pass
- Final speed (m/s): Body 1: 3; Body 2: 2
### Result: Pass

## Test 11 — Unequal masses

### Input:
- Baseline inputs

### Expected final results:
- Acceleration (m/s²): Body 1: [2 0]  Body 2: [-1 0] - The lighter body must have twice the acceleration magnitude of the heavier body because the interaction-force magnitudes are equal.

### Actual:
- Acceleration (m/s²): Body 1: [2 0]  Body 2: [-1 0] - The lighter body has twice the acceleration magnitude of the heavier body because the interaction-force magnitudes are equal.
### Result: Pass

## Test 12 — Vertical forces

### Input: 
- Baseline inputs but interaction forces are equal to: Body 1: [0 4]; Body 2: [0 -4]

### Expected final results:
- Acceleration (m/s²): Body 1: [0 2]  Body 2: [0 -1]
- Final position (m): Body 1: [1 1]; Body 2: [10 -0.5]
- Final velocity (m/s): Body 1: [1 2]; Body 2: [0 -1]
- Final speed (m/s): Body 1: 2.236; Body 2: 1

### Actual:
- Acceleration (m/s²): Body 1: [0 2]  Body 2: [0 -1]
### Result: Pass
- Final position (m): Body 1: [1 1]; Body 2: [10 -0.5]
### Result: Pass
- Final velocity (m/s): Body 1: [1 2]; Body 2: [0 -1]
### Result: Pass
- Final speed (m/s): Body 1: 2.236; Body 2: 1
### Result: Pass

## Test 13 — Diagonal forces

### Input:
- Baseline inputs but interaction forces are equal to: Body 1: [3 4]; Body 2: [-3 -4]

### Expected final results:
- Acceleration (m/s²): Body 1: [1.5 2]  Body 2: [-0.75 -1]
- Final position (m): Body 1: [1.75 1]; Body 2: [9.625 -0.5]
- Final velocity (m/s): Body 1: [2.5 2]; Body 2: [-0.75 -1]
- Final speed (m/s): Body 1: 3.202; Body 2: 1.250

### Actual:
- Acceleration (m/s²): Body 1: [1.5 2]  Body 2: [-0.75 -1]
### Result: Pass
- Final position (m): Body 1: [1.75 1]; Body 2: [9.625 -0.5]
### Result: Pass
- Final velocity (m/s): Body 1: [2.5 2]; Body 2: [-0.75 -1]
### Result: Pass
- Final speed (m/s): Body 1: 3.202; Body 2: 1.250
### Result: Pass

## Test 14 — Different initial positions

### Input:
- Baseline inputs but initial positions are equal to: Body 1: [-5 3]; Body 2: [20 -2]

### Expected final results:
- Final position (m): Body 1: [-3 3]; Body 2: [19.5 -2]

### Actual:
- Final position (m): Body 1: [-3 3]; Body 2: [19.5 -2]
### Result: Pass

## Test 15 — Different initial velocities

### Input:
- Baseline inputs but initial velocities are equal to: Body 1: [-2 1]; Body 2: [3 -1]

### Expected final results:
- Final position (m): Body 1: [-1 1]; Body 2: [12.5 -1]
- Final velocity (m/s): Body 1: [0 1]; Body 2: [2 -1]

### Actual:
- Final position (m): Body 1: [-1 1]; Body 2: [12.5 -1]
### Result: Pass
- Final velocity (m/s): Body 1: [0 1]; Body 2: [2 -1]
### Result: Pass

## Test 16 — External forces affect net acceleration

### Input: 
- Baseline inputs but external forces are equal to: Body 1: [2 0]; Body 2: [0 4]

### Expected final results:
- Net force (N): Body 1: [6 0]; Body 2: [-4 4]
- Acceleration (m/s²): Body 1: [3 0]; Body 2; [-1 1]
- Final position (m): Body 1: [2.5 0]; Body 2: [9.5 0.5]
- Final velocity (m/s): Body 1: [4 0]; Body 2: [-1 1]

### Actual:
- Net force (N): Body 1: [6 0]; Body 2: [-4 4]
### Result: Pass
- Acceleration (m/s²): Body 1: [3 0]; Body 2; [-1 1]
### Result: Pass
- Final position (m): Body 1: [2.5 0]; Body 2: [9.5 0.5]
### Result: Pass
- Final velocity (m/s): Body 1: [4 0]; Body 2: [-1 1]
### Result: Pass

## Test 17 — Force-pair consistency

### Input: 
- Baseline inputs
- Check that at every time step: F12 + F21 = [0 0]

### Expected final results: the residual is zero or within the program's chosen floating-point tolerance. The interaction forces must remain equal and opposite throughout the simulation.
### Result: Pass

## Test 18 — Analytical validation

### Input: 
- Baseline inputs

### Expected final results: position and velocity errors should be close to zero, allowing for floating-point rounding. A substantial discrepancy indicates a possible implementation error.
### Result: Pass

## Test 19 — Semi-implicit Euler comparison

### Input: 
- Baseline inputs

### Expected final results: 
- Final position (m): Body 1: [2.1 0]; Body 2: [9.45 0]
- Final velocity (m/s): Body 1: [3 0]; Body 2: [-1 0]
- Position-error magnitude (m): Body 1: 0.1; Body 2: 0.05
- Velocity-error magnitude (m/s): Body 1: ~0; Body 2: ~0

### Actual:
- Final position (m): Body 1: [2.1 0]; Body 2: [9.45 0]
### Result: Pass
- Final velocity (m/s): Body 1: [3 0]; Body 2: [-1 0]
### Result: Pass
- Position-error magnitude (m): Body 1: 0.1; Body 2: 0.05
### Result: Pass
- Velocity-error magnitude (m/s): Body 1: ~0; Body 2: ~0
### Result: Pass

## Test 20 — Time-step sensitivity

### Input: 
- Baseline inputs but for run A: dt = 0.1, B: dt = 0.05, C: dt = 0.01

### Expected final results: 
- A: Body 1: Euler position-error magnitude: 0.1; Body 2: Euler position-error magnitude: 0.05
- B: Body 1: Euler position-error magnitude: 0.05; Body 2: Euler position-error magnitude: 0.025
- C: Body 1: Euler position-error magnitude: 0.01; Body 2: Euler position-error magnitude: 0.005

### Actual:
- A: Body 1: Euler position-error magnitude: 0.1; Body 2: Euler position-error magnitude: 0.05
### Result: Pass
- B: Body 1: Euler position-error magnitude: 0.05; Body 2: Euler position-error magnitude: 0.025
### Result: Pass
- C: Body 1: Euler position-error magnitude: 0.01; Body 2: Euler position-error magnitude: 0.005
### Result: Pass

## Test 21 — Zero initial velocity

### Input: 
- Baseline inputs but for initial velocites for both bodies are equal to [0 0]

### Expected final results: 
- Final position (m): Body 1: [1 0]; Body 2: [9.5 0]
- Final velocity (m/s): Body 1: [2 0]; Body 2: [-1 0]

### Actual: 
- Final position (m): Body 1: [1 0]; Body 2: [9.5 0]
### Result: Pass
- Final velocity (m/s): Body 1: [2 0]; Body 2: [-1 0]
### Result: Pass

## Test 22 — Zero interaction force

### Input: 
- Baseline inputs but both interaction and external forces are equal to [0 0]

### Expected final results: 
- Acceleration (m/s²): Body 1: [0 0]; Body 2; [0 0]
- Final position (m): Body 1: [1 0]; Body 2: [10 0]
- Final velocity (m/s): Body 1: [1 0]; Body 2: [0 0]

### Actual:
- Acceleration (m/s²): Body 1: [0 0]; Body 2; [0 0]
### Result: Pass
- Final position (m): Body 1: [1 0]; Body 2: [10 0]
### Result: Pass
- Final velocity (m/s): Body 1: [1 0]; Body 2: [0 0] 
### Result: Pass

## Test 23 — Time-history arrays

### Input: 
- Baseline inputs

### Expected final results: 
- There are 11 time points, from 0 through 1 s, inclusive.
- Consecutive time points differ by 0.1 s.
- Initial values correspond to the entered positions and velocities.
- Position, velocity, acceleration and error arrays have compatible dimensions.
- No history contains unexpected NaN or Inf values.

### Actual:
- There are 11 time points, from 0 through 1s, inclusive.
### Result: Pass
- Consecutive time points differ by 0.1 s.
### Result: Pass
- Initial values correspond to the entered positions and velocities.
### Result: Pass
- Position, velocity, acceleration and error arrays have compatible dimensions.
### Result: Pass
- No history contains unexpected NaN or Inf values.
### Result: Pass

## Test 24 — Graph outputs

### Input: 
- Baseline inputs

### Expected final results: 
- Both bodies' trajectories are represented.
- Position-versus-time plots show the expected motion.
- Velocity-versus-time plots show linear velocity changes for the constant accelerations.
- Error plots reflect the differences between each numerical method and the analytical reference.
- Axes have meaningful labels and units.
- No plotted data is missing or incorrectly assigned to a body.

### Actual:
- Both bodies' trajectories are represented.
### Result: Pass
- Position-versus-time plots show the expected motion.
### Result: Pass
- Velocity-versus-time plots show linear velocity changes for the constant accelerations.
### Result: Pass
- Error plots reflect the differences between each numerical method and the analytical reference.
### Result: Pass
- Axes have meaningful labels and units.
### Result: Pass
- No plotted data is missing or incorrectly assigned to a body.
### Result: Pass

## Test 25 — Numerical summaries

### Input: 
- Baseline inputs

### Expected final results: 
- The final positions and velocities agree with Test 9.
- Speeds are nonnegative.
- Acceleration values agree with net force divided by mass.
- The reported position and velocity errors agree with the corresponding comparisons.
- The displayed units are consistent.
- No summary uses values from the wrong body or integration method.

### Actual:
- The final positions and velocities agree with Test 9.
### Result: Pass
- Speeds are nonnegative.
### Result: Pass
- Acceleration values agree with net force divided by mass.
### Result: Pass
- The reported position and velocity errors agree with the corresponding comparisons.
### Result: Pass
- The displayed units are consistent.
### Result: Pass
- No summary uses values from the wrong body or integration method.
### Result: Pass

## Test 26 — Very small positive time step

### Input: 
- Baseline inputs but the Time-step is equal to 0.001

### Expected final results: 
- The program completes with 1001 time points.
- Final results remain close to the analytical solution.
- No unexpected numerical instability or invalid array indexing occurs.

### Actual:
- The program completes with 1001 time points.
### Result: Pass
- Final results remain close to the analytical solution.
### Result: Pass
- No unexpected numerical instability or invalid array indexing occurs.
### Result: Pass

## Test 27 — Single-step simulation

### Input: 
- Baseline inputs

### Expected final results: 
- the simulation completes with two time points, t = 0 and t = 1 s.

### Actual:
- the simulation completes with two time points, t = 0 and t = 1 s.
### Result: Pass

## Test 28 — Clean-start execution

### Input: 
- Baseline inputs
- Restart Octave or clear the workspace, then run the simulation normally using baseline inputs.

### Expected final results: 
- No undefined-variable errors.
- No dependence on variables left over from a previous run.
- All plots, summaries and history arrays are produced.
- The program completes without a critical error.

### Actual:
- No undefined-variable errors.
### Result: Pass
- No dependence on variables left over from a previous run.
### Result: Pass
- All plots, summaries and history arrays are produced.
### Result: Pass
- The program completes without a critical error.
### Result: Pass