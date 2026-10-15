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