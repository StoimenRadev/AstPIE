# Testing & Validation

## Test 1 — Balanced forces

### Input
- Mass: 5 kg
- Initial position: [0, 0] m
- Initial velocity: [2, 3] m/s
- dt: 0.1 s
- T: 1 s
- Force 1: [5, 7] N
- Force 2: [-5, -7] N

### Expected output
- Net force: [0.000, 0.000] N
- Acceleration: [0.000, 0.000] m/s²
- Final position: [2.000, 3.000] m
- Final velocity: [2.000, 3.000] m/s
- Steps: 10

### Actual
- Net force: [0.000, 0.000] N
- Acceleration: [0.000, 0.000] m/s²
- Final position: [2.000, 3.000] m
- Final velocity: [2.000, 3.000] m/s
- Steps: 10

### Result - Pass

## Test 2 - Different mass

### Input
- Mass: 2 kg
- Initial position: [1, -2] m
- Initial velocity: [0, 1] m/s
- dt: 0.1 s
- T: 1 s
- Force 1: [6, 2] N
- Force 2: [0, 0] N

### Expected output
- Net force: [6.000, 2.000] N
- Acceleration: [3.000, 1.000] m/s²
- Final position: [2.650, -0.450] m
- Final velocity: [3.000, 2.000] m/s
- Steps: 10

### Actual
- Net force: [6.000, 2.000] N
- Acceleration: [3.000, 1.000] m/s²
- Final position: [2.650, -0.450] m
- Final velocity: [3.000, 2.000] m/s
- Steps: 10

### Result - Pass

## Test 3 - Different initial position

### Input
- Mass: 5 kg
- Initial position: [-4, 3] m
- Initial velocity: [1, 2] m/s
- dt: 0.2 s
- T: 1 s
- Force 1: [4, 1] N
- Force 2: [0, 0] N

### Expected output
- Net force: [4.000, 1.000] N
- Acceleration: [0.800, 0.200] m/s²
- Final position: [-2.520, 1.120] m
- Final velocity: [1.800, -1.800] m/s
- Steps: 5

### Actual
- Net force: [4.000, 1.000] N
- Acceleration: [0.800, 0.200] m/s²
- Final position: [-2.520, 1.120] m
- Final velocity: [1.800, -1.800] m/s
- Steps: 5

### Result - Pass

## Test 4 - Different initial velocity

### Input
- Mass: 5 kg
- Initial position: [0, 0] m
- Initial velocity: [-2, 5] m/s
- dt: 0.1 s
- T: 1 s
- Force 1: [8, 0] N
- Force 2: [0, 0] N

### Expected output
- Net force: [8.000, 0.000] N
- Acceleration: [1.600, 0.000] m/s²
- Final position: [-1.120, 5.000] m
- Final velocity: [-0.400, 5.000] m/s
- Steps: 5

### Actual
- Net force: [8.000, 0.000] N
- Acceleration: [1.600, 0.000] m/s²
- Final position: [-1.120, 5.000] m
- Final velocity: [-0.400, 5.000] m/s
- Steps: 10

### Result - Pass

## Test 5 - Different forces

### Input
- Mass: 5 kg
- Initial position: [0, 0] m
- Initial velocity: [1, 1] m/s
- dt: 0.1 s
- T: 1 s
- Force 1: [10, 5] N
- Force 2: [-4, 2] N

### Expected output
- Net force: [6.000, 7.000] N
- Acceleration: [1.200, 1.400] m/s²
- Final position: [1.660, 1.770] m
- Final velocity: [2.200, 2.400] m/s
- Steps: 10

### Actual
- Net force: [8.000, 0.000] N
- Acceleration: [1.600, 0.000] m/s²
- Final position: [-1.120, 5.000] m
- Final velocity: [-0.400, 5.000] m/s
- Steps: 5

### Result - Pass

## Test 6 - Different time step

### Input
- Mass: 5 kg
- Initial position: [0, 0] m
- Initial velocity: [1, 1] m/s
- dt: 0.2 s
- T: 1 s
- Force 1: [6, 4] N
- Force 2: [-2, 0] N

### Expected output
- Net force: [4.000, 4.000] N
- Acceleration: [0.800, 0.800] m/s²
- Final position: [1.480, 1.480] m
- Final velocity: [1.800, 1.800] m/s
- Steps: 5

### Actual
- Net force: [4.000, 4.000] N
- Acceleration: [0.800, 0.800] m/s²
- Final position: [1.480, 1.480] m
- Final velocity: [1.800, 1.800] m/s
- Steps: 5

### Result - Pass

## Test 7 - Different total simulation time

### Input
- Mass: 5 kg
- Initial position: [0, 0] m
- Initial velocity: [1, 1] m/s
- dt: 0.1 s
- T: 2 s
- Force 1: [4, 3] N
- Force 2: [0, 0] N

### Expected output
- Net force: [4.000, 3.000] N
- Acceleration: [0.800, 0.600] m/s²
- Final position: [3.680, 3.260] m
- Final velocity: [2.600, 2.200] m/s
- Steps: 20

### Actual
- Net force: [4.000, 3.000] N
- Acceleration: [0.800, 0.600] m/s²
- Final position: [3.680, 3.260] m
- Final velocity: [2.600, 2.200] m/s
- Steps: 20

### Result - Pass