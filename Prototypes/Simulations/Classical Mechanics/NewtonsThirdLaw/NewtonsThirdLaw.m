clear;
clc;
close all;

% Parameters
n = input("Enter number of bodies: ");
if n < 2
    error("There should be at least 2 bodies!");
end

mass = zeros(n, 1);
x = zeros(n, 2);
v = zeros(n, 2);

% Input masses, positions, and velocities
for i = 1:n

    % Input mass
    mass(i) = input(sprintf("Enter the mass of body %d (kg): ", i));

    if mass(i) <= 0
        error("Mass must be greater than zero!");
    end

    % Input initial position
    x(i, :) = input(sprintf("Enter the initial position [x, y] of body %d (m): ", i));

    % Input initial velocity
    v(i, :) = input(sprintf("Enter the initial velocity [vx, vy] of body %d (m/s): ", i));

end

% Save initial conditions for later display
x_initial = x;
v_initial = v;

% Input simulation parameters
dt = input("Enter the time step (s): ");
T = input("Enter the total simulation time (s): ");

if dt <= 0 || T <= 0
    error("Time step and simulation time must be positive!");
end

if mod(T, dt) > 1e-10
    error("Simulation time must be divisible by the time step!");
end

% Input interaction forces
F1 = input("Enter the force on body 1 [Fx, Fy] (N): ");
F2 = input("Enter the force on body 2 [Fx, Fy] (N): ");

% Test Newton's Third Law
if any(F2 != -F1)
    error("Newton's Third Law test failed: forces are not equal and opposite!");
end

% Calculate net force
Fnet = zeros(n, 2);
Fnet(1, :) = F1;
Fnet(2, :) = F2;

% Calculate acceleration
a = Fnet ./ mass;

% Create time array
time = 0:dt:T;

% Store simulation data
position = zeros(length(time), n, 2);
velocity = zeros(length(time), n, 2);
acceleration = zeros(length(time), n, 2);

position(1, :, :) = x;
velocity(1, :, :) = v;
acceleration(1, :, :) = a;

% Simulation loop
for i = 2:length(time)

    % Update velocity
    v_final = v + a * dt;

    % Update position using updated velocity
    x_final = x + v_final * dt;

    % Update current state
    v = v_final;
    x = x_final;

    % Store results
    position(i, :, :) = x;
    velocity(i, :, :) = v;
    acceleration(i, :, :) = a;

end

fprintf("\nSimulation results\n");

% Simulation setup
fprintf("\nSimulation Setup\n");
fprintf("Number of bodies: %d\n", n);
fprintf("Time step (s): %.3f\n", dt);
fprintf("Total simulation time (s): %.3f\n", T);

fprintf("\nInteraction forces:\n");
fprintf("Force on body 1 (N): [%.3f, %.3f]\n", F1(1), F1(2));
fprintf("Force on body 2 (N): [%.3f, %.3f]\n", F2(1), F2(2));
fprintf("Newton's Third Law test passed.\n");

% Initial conditions
fprintf("\nInitial Conditions\n");

for j = 1:n
    fprintf("\nBody %d\n", j);
    fprintf("Mass (kg): %.3f\n", mass(j));
    fprintf("Initial position (m): [%.3f, %.3f]\n", x_initial(j, 1), x_initial(j, 2));
    fprintf("Initial velocity (m/s): [%.3f, %.3f]\n", v_initial(j, 1), v_initial(j, 2));
end

% Final results
fprintf("\nFinal Simulation Results\n");

for j = 1:n
    fprintf("\nBody %d\n", j);
    fprintf("Mass (kg): %.3f\n", mass(j));
    fprintf("Final position (m): [%.3f, %.3f]\n", x(j, 1), x(j, 2));
    fprintf("Final velocity (m/s): [%.3f, %.3f]\n", v(j, 1), v(j, 2));
    fprintf("Acceleration (m/s^2): [%.3f, %.3f]\n", a(j, 1), a(j, 2));
    fprintf("Net force (N): [%.3f, %.3f]\n", Fnet(j, 1), Fnet(j, 2));
end
