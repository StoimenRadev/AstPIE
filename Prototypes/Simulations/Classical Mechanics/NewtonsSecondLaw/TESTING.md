# Testing & Validation

## Test 1 - Net force proportionality

## Test 1A - Net force of 5 N

### Input
- Mass: 5 kg
- Initial position: [0, 0] m
- Initial velocity: [0, 0] m/s
- dt: 0.01 s
- T: 2 s
- Force 1: [5, 0] N
- Force 2: [0, 0] N

### Expected output
- Net force: [5.000, 0.000] N
- Acceleration: [1.000, 0.000] m/s²
- Final position: [2.010, 0.000] m
- Final velocity: [2.000, 0.000] m/s
- Final speed: 2 m/s
- Steps: 200

### Actual
- Net force: [5.000, 0.000] N
- Acceleration: [1.000, 0.000] m/s²
- Final position: [2.010, 0.000] m
- Final velocity: [2.000, 0.000] m/s
- Final Speed: 2 m/s
- Steps: 200

### Result - Pass

## Test 1B - Double the force

### Input
- Mass: 5 kg
- Initial position: [0, 0] m
- Initial velocity: [0, 0] m/s
- dt: 0.01 s
- T: 2 s
- Force 1: [10, 0] N
- Force 2: [0, 0] N

### Expected output
- Net force: [10.000, 0.000] N
- Acceleration: [2.000, 0.000] m/s²
- Final position: [4.020, 0.000] m
- Final velocity: [4.000, 0.000] m/s
- Final speed: 4 m/s
- Steps: 200

### Actual
- Net force: [10.000, .000] N
- Acceleration: [2.000, .000] m/s²
- Final position: [4.020, .000] m
- Final velocity: [4.000, .000] m/s
- Final Speed: 4 m/s
- Steps: 200

### Result - Pass

## Test 1C - Quadruple the original force

### Input
- Mass: 5 kg
- Initial position: [0, 0] m
- Initial velocity: [0, 0] m/s
- dt: 0.01 s
- T: 2 s
- Force 1: [20, 0] N
- Force 2: [0, 0] N

### Expected output
- Net force: [20.000, 0.000] N
- Acceleration: [4.000, 0.000] m/s²
- Final position: [8.040, 0.000] m
- Final velocity: [8.000, 0.000] m/s
- Final speed: 4 m/s
- Steps: 200

### Actual
- Net force: [20.000, 0.000] N
- Acceleration: [4.000, 0.000] m/s²
- Final position: [8.040, 0.000] m
- Final velocity: [8.000, 0.000] m/s
- Final Speed: 8 m/s
- Steps: 200

### Result - Pass

## Test 2 - Mass dependence

## Test 2A - Mass of 2 kg

### Input
- Mass: 2 kg
- Initial position: [0, 0] m
- Initial velocity: [0, 0] m/s
- dt: 0.01 s
- T: 2 s
- Force 1: [10, 0] N
- Force 2: [0, 0] N

### Expected output
- Net force: [10.000, 0.000] N
- Acceleration: [5.000, 0.000] m/s²
- Final position: [10.050, 0.000] m
- Final velocity: [10.000, 0.000] m/s
- Final speed: 10 m/s
- Steps: 200

### Actual
- Net force: [10.000, 0.000] N
- Acceleration: [5.000, 0.000] m/s²
- Final position: [10.050, 0.000] m
- Final velocity: [10.000, 0.000] m/s
- Final Speed: 10 m/s
- Steps: 200

### Result - Pass

## Test 2B - Mass of 5 kg

### Input
- Mass: 5 kg
- Initial position: [0, 0] m
- Initial velocity: [0, 0] m/s
- dt: 0.01 s
- T: 2 s
- Force 1: [10, 0] N
- Force 2: [0, 0] N

### Expected output
- Net force: [10.000, 0.000] N
- Acceleration: [2.000, 0.000] m/s²
- Final position: [4.020, 0.000] m
- Final velocity: [4.000, 0.000] m/s
- Final speed: 4 m/s
- Steps: 200

### Actual
- Net force: [10.000, 0.000] N
- Acceleration: [2.000, 0.000] m/s²
- Final position: [4.020, 0.000] m
- Final velocity: [4.000, 0.000] m/s
- Final Speed: 4 m/s
- Steps: 200

### Result - Pass

## Test 2C - Mass of 10 kg

### Input
- Mass: 10 kg
- Initial position: [0, 0] m
- Initial velocity: [0, 0] m/s
- dt: 0.01 s
- T: 2 s
- Force 1: [10, 0] N
- Force 2: [0, 0] N

### Expected output
- Net force: [10.000, 0.000] N
- Acceleration: [1.000, 0.000] m/s²
- Final position: [2.010, 0.000] m
- Final velocity: [2.000, 0.000] m/s
- Final speed: 2 m/s
- Steps: 200

### Actual
- Net force: [10.000, 0.000] N
- Acceleration: [1.000, 0.000] m/s²
- Final position: [2.010, 0.000] m
- Final velocity: [2.000, 0.000] m/s
- Final Speed: 2 m/s
- Steps: 200

### Result - Pass

## Test 3 - Force direction

## Test 3A - Positive x-direction

### Input
- Mass: 2 kg
- Initial position: [0, 0] m
- Initial velocity: [0, 0] m/s
- dt: 0.01 s
- T: 1 s
- Force 1: [8, 0] N
- Force 2: [0, 0] N

### Expected output
- Net force: [8.000, 0.000] N
- Acceleration: [4.000, 0.000] m/s²
- Final position: [2.020, 0.000] m
- Final velocity: [4.000, 0.000] m/s
- Final speed: 4 m/s
- Steps: 100

### Actual
- Net force: [8.000, 0.000] N
- Acceleration: [4.000, 0.000] m/s²
- Final position: [2.020, 0.000] m
- Final velocity: [4.000, 0.000] m/s
- Final Speed: 4 m/s
- Steps: 100

### Result - Pass

## Test 3B - Positive y-direction

### Input
- Mass: 2 kg
- Initial position: [0, 0] m
- Initial velocity: [0, 0] m/s
- dt: 0.01 s
- T: 1 s
- Force 1: [0, 8] N
- Force 2: [0, 0] N

### Expected output
- Net force: [0.000, 8.000] N
- Acceleration: [0.000, 4.000] m/s²
- Final position: [0.000, 2.020] m
- Final velocity: [0.000, 4.000] m/s
- Final speed: 4 m/s
- Steps: 100

### Actual
- Net force: [0.000, 8.000] N
- Acceleration: [0.000, 4.000] m/s²
- Final position: [0.000, 2.020] m
- Final velocity: [0.000, 4.000] m/s
- Final Speed: 4 m/s
- Steps: 100

### Result - Pass

## Test 3C - Negative y-direction

### Input
- Mass: 2 kg
- Initial position: [0, 0] m
- Initial velocity: [0, 0] m/s
- dt: 0.01 s
- T: 1 s
- Force 1: [0, -8] N
- Force 2: [0, 0] N

### Expected output
- Net force: [0.000, -8.000] N
- Acceleration: [0.000, -4.000] m/s²
- Final position: [0.000, -2.020] m
- Final velocity: [0.000, -4.000] m/s
- Final speed: 4 m/s
- Steps: 100

### Actual
- Net force: [0.000, -8.000] N
- Acceleration: [0.000, -4.000] m/s²
- Final position: [0.000, -2.020] m
- Final velocity: [0.000, -4.000] m/s
- Final Speed: 4 m/s
- Steps: 100

### Result - Pass

## Test 4 - Balanced forces

## Test 4A - Opposing forces with an initially stationary object

### Input
- Mass: 5 kg
- Initial position: [0, 0] m
- Initial velocity: [0, 0] m/s
- dt: 0.01 s
- T: 2 s
- Force 1: [10, 0] N
- Force 2: [-10, 0] N

### Expected output
- Net force: [0.000, 0.000] N
- Acceleration: [0.000, 0.000] m/s²
- Final position: [0.000, 0.000] m
- Final velocity: [0.000, 0.000] m/s
- Final speed: 0 m/s
- Steps: 200

### Actual
- Net force: [0.000, 0.000] N
- Acceleration: [0.000, 0.000] m/s²
- Final position: [0.000, 0.000] m
- Final velocity: [0.000, 0.000] m/s
- Final Speed: 0 m/s
- Steps: 100

### Result - Pass

## Test 4B - Balanced forces with initial motion

### Input
- Mass: 5 kg
- Initial position: [1, 2] m
- Initial velocity: [3, 4] m/s
- dt: 0.01 s
- T: 2 s
- Force 1: [10, 0] N
- Force 2: [-10, 0] N

### Expected output
- Net force: [0.000, 0.000] N
- Acceleration: [0.000, 0.000] m/s²
- Final position: [7.000, 10.000] m
- Final velocity: [3.000, 4.000] m/s
- Final speed: 5 m/s
- Steps: 200

### Actual
- Net force: [0.000, 0.000] N
- Acceleration: [0.000, 0.000] m/s²
- Final position: [7.000, 10.000] m
- Final velocity: [3.000, 4.000] m/s
- Final Speed: 5 m/s
- Steps: 100

### Result - Pass

---------------------------------------------

# Numerical Validation and Testing

## Test 1 - Analytical vs. numerical position

### Input
- Mass: 5 kg
- Initial position: [0, 0] m
- Initial velocity: [0, 0] m/s
- dt: 0.1 s
- T: 2 s
- Force 1: [10, 0] N
- Force 2: [0, 0] N

### Expected output
- Net force: [10.000, 0.000] N
- Acceleration: [2.000, 0.000] m/s²
- Final numerical position: [4.200, 10.000] m
- Final analytical position: [4.000, 0.000] m
- Final numerical velocity: [4.000, 0.000] m/s
- Final analytical velocity: [4.000, 0.000] m/s
- Position error: 0.2 m
- Final numerical speed: 4 m/s
- Final anakytical soeed: 4 m/s
- Steps: 20

### Actual
- Net force: [10.000, 0.000] N
- Acceleration: [2.000, 0.000] m/s²
- Final numerical position: [4.200, 10.000] m
- Final analytical position: [4.000, 0.000] m
- Final velocity: [4.000, 4.000] m/s
- Position error: 0.2 m
- Final speed: 5 m/s
- Steps: 20

### Result - Pass

## Test 2 — Timestep refinement

## Test 2A

### Input
- Mass: 5 kg
- Initial position: [0, 0] m
- Initial velocity: [0, 0] m/s
- dt: 0.01 s
- T: 2 s
- Force 1: [10, 0] N
- Force 2: [0, 0] N

### Expected output
- Net force: [10.000, 0.000] N
- Acceleration: [2.000, 0.000] m/s²
- Final numerical position: [4.020, 00.000] m
- Final analytical position: [4.000, 0.000] m
- Final numerical velocity: [4.000, 0.000] m/s
- Final analytical velocity: [4.000, 0.000] m/s
- Position error: 0.02 m
- Final numerical speed: 4 m/s
- Final analytical speed: 4 m/s
- Steps: 20

### Actual
- Net force: [10.000, 0.000] N
- Acceleration: [2.000, 0.000] m/s²
- Final numerical position: [4.020, 00.000] m
- Final analytical position: [4.000, 0.000] m
- Final velocity: [4.000, 4.000] m/s
- Position error: 0.02 m
- Final numerical speed: 4 m/s
- Final analytical speed: 4 m/s
- Steps: 200

### Result - Pass

## Test 2B

### Input
- Mass: 5 kg
- Initial position: [0, 0] m
- Initial velocity: [0, 0] m/s
- dt: 0.001 s
- T: 2 s
- Force 1: [10, 0] N
- Force 2: [0, 0] N

### Expected output
- Net force: [10.000, 0.000] N
- Acceleration: [2.000, 0.000] m/s²
- Final numerical position: [4.002, 0.000] m
- Final analytical position: [4.000, 0.000] m
- Final numerical velocity: [4.000, 0.000] m/s
- Final analytical velocity: [4.000, 0.000] m/s
- Position error: 0.002 m
- Final numerical speed: 4 m/s
- Final analytical speed: 4 m/s
- Steps: 2000

### Actual
- Net force: [10.000, 0.000] N
- Acceleration: [2.000, 0.000] m/s²
- Final numerical position: [4.002, 10.000] m
- Final analytical position: [4.000, 0.000] m
- Final numerical velocity: [4.000, 4.000] m/s
- Final analytical velocity: [4.000, 4.000] m/s
- Position error: 0.002 m
- Final numerical speed: 5 m/s
- Final analytical speed: 5 m/s
- Steps: 2000

### Result - Pass

## Test 3 — Constant velocity with zero net force

### Input
- Mass: 5 kg
- Initial position: [1, 2] m
- Initial velocity: [3, 4] m/s
- dt: 0.01 s
- T: 2 s
- Force 1: [10, 0] N
- Force 2: [-10, 0] N

### Expected output
- Net force: [0.000, 0.000] N
- Acceleration: [0.000, 0.000] m/s²
- Final numerical position: [7.000, 10.000] m
- Final analytical position: [7.000, 10.000] m
- Final numerical velocity: [3.000, 4.000] m/s
- Final analytical velocity: [3.000, 4.000] m/s
- Position error: 0.000 m
- Final numerical speed: 5 m/s
- Final analytical speed: 5 m/s
- Steps: 200

### Actual
- Net force: [10.000, 0.000] N
- Acceleration: [0.000, 0.000] m/s²
- Final numerical position: [7.000, 10.000] m
- Final analytical position: [7.000, 10.000] m
- Final numerical velocity: [3.000, 4.000] m/s
- Final analytical velocity: [3.000, 4.000] m/s
- Position error: 0.000 m
- Final numerical speed: 5 m/s
- Final analytical speed: 5 m/s
- Steps: 200

### Result - Pass

## Test 4 — Two-dimensional numerical validation

### Input
- Mass: 2 kg
- Initial position: [0, 0] m
- Initial velocity: [0, 0] m/s
- dt: 0.01 s
- T: 1 s
- Force 1: [6, 8] N
- Force 2: [0, 0] N

### Expected output
- Net force: [6.000, 8.000] N
- Acceleration: [3.000, 4.000] m/s²
- Final numerical position: [1.515, 2.020] m
- Final analytical position: [1.500, 2.000] m
- Final numerical velocity: [3.000, 4.000] m/s
- Final analytical velocity: [3.000, 4.000] m/s
- Position error: 0.025 m
- Final numerical speed: 5 m/s
- Final analytical speed: 5 m/s
- Steps: 100

### Actual
- Net force: [6.000, 8.000] N
- Acceleration: [3.000, 4.000] m/s²
- Final numerical position: [1.515, 2.020] m
- Final analytical position: [1.500, 20.000] m
- Final numerical velocity: [3.000, 4.000] m/s
- Final analytical velocity: [3.000, 4.000] m/s
- Position error: 0.25 m
- Final numerical speed: 5 m/s
- Final analytical speed: 5 m/s
- Steps: 100

### Result - Pass

## Test 5 — Negative acceleration

### Input
- Mass: 2 kg
- Initial position: [0, 0] m
- Initial velocity: [5, 0] m/s
- dt: 0.01 s
- T: 1 s
- Force 1: [-4, 0] N
- Force 2: [0, 0] N

### Expected output
- Net force: [-4.000, 0.000] N
- Acceleration: [-2.000, 0.000] m/s²
- Final numerical position: [3.990, 0.000] m
- Final analytical position: [4.000, 0.000] m
- Final numerical velocity: [3.000, 0.000] m/s
- Final analytical velocity: [3.000, 0.000] m/s
- Position error: 0.025 m
- Final numerical speed: 3 m/s
- Final analytical speed: 3 m/s
- Steps: 100

### Actual
- Net force: [-4.000, 0.000] N
- Acceleration: [-2.000, 0.000] m/s²
- Final numerical position: [3.990, 0.000] m
- Final analytical position: [4.000, 0.000] m
- Final numerical velocity: [3.000, 0.000] m/s
- Final analytical velocity: [3.000, 0.000] m/s
- Position error: 0.010 m
- Final numerical speed: 3 m/s
- Final analytical speed: 3 m/s
- Steps: 100

### Result - Pass

## Test 6 — Visualization and recorded-data consistency

- The trajectory begins at [0 0] and ends at [1.515 2.02]. - Yes
- The position-versus-time graph ends at approximately x = 1.515 m and y = 2.02 m. - Yes
- The velocity-versus-time graph ends at vx = 3 m/s and vy = 4 m/s. - Yes
- The trajectory contains 101 recorded positions, including the initial position. - Yes
- The force vectors represent [6 8] N and [0 0] N for the individual forces. - Yes
- The net-force vector represents [6 8] N. - Yes
- The displayed acceleration is [3 4] m/s². - Yes
- The displayed final velocity is [3 4] m/s. - Yes
- The time-history table has 101 rows and 10 columns. - Yes

### Result - Pass

## Test 7 — Invalid-input validation

## 7A
- Invalid input: Mass = 0
- Error: mass must be positive
### Result - Pass

## 7B
- Invalid input: Mass = -5
- Error: mass must be positive
### Result - Pass

## 7C
- Invalid input: Timestep = 0
- Error: timestep and total time must be positive
### Result - Pass

## 7D
- Invalid input: Total time = -1
- Error: timestep and total time must be positive
### Result - Pass

## 7E
- Invalid input: Timestep = 0.3, total time = 1
- Error: total time is not an integer multiple of timestep
### Result - Pass

## 7F
- Invalid input: Position = [1 2 3]
- Error: position and velocity must each contain two components
### Result - Pass

## 7G
- Invalid input: Force 1 = [1 2 3]
- Error: each force must contain two components
### Result - Pass