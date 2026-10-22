clear;
clc;
close all;

% Parameters
n = input("Enter number of bodies: ");
if ~isscalar(n) || ~isfinite(n) || n ~= 2 || n ~= fix(n)
    error("This version requires exactly 2 bodies.");
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

if ~isscalar(dt) || ~isfinite(dt) || dt <= 0 || ~isscalar(T) || ~isfinite(T) || T <= 0
    error("Time step and simulation time must be positive finite numbers.");
end

if abs(T / dt - round(T / dt)) > 1e-10
    error("Total simulation time must be an integer multiple of the time step.");
end

% Create time array
time = 0:dt:T;
time(end) = T;

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

num_steps = length(time);

% Store simulation data
position = zeros(num_steps, n, 2);
velocity = zeros(num_steps, n, 2);
acceleration = zeros(num_steps, n, 2);
force = zeros(num_steps, n, 2);
speed = zeros(num_steps, n);

speed(1, :) = sqrt(sum(v.^2, 2));

position(1, :, :) = x;
velocity(1, :, :) = v;
acceleration(1, :, :) = a;
force(1, :, :) = Fnet;

% Simulation loop
for i = 2:num_steps

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
    force(i, :, :) = Fnet;

    speed(i, :) = sqrt(sum(v.^2, 2));

end

fprintf("\nSimulation results\n");

% Simulation setup
fprintf("\nSimulation Setup\n");
fprintf("Number of bodies: %d\n", n);
fprintf("Time step (s): %.3f\n", dt);
fprintf("Total simulation time (s): %.3f\n", T);
fprintf("Number of time points: %d\n", num_steps);
fprintf("Number of simulation steps: %d\n", num_steps - 1);

% Interaction forces
fprintf("\nInteraction forces:\n");
fprintf("Force on body 1 (N): [%.3f, %.3f]\n", F1(1), F1(2));
fprintf("Force on body 2 (N): [%.3f, %.3f]\n", F2(1), F2(2));
fprintf("Newton's Third Law test passed.\n");

% Initial conditions
fprintf("\nInitial Conditions\n");

for j = 1:n
    fprintf("\nBody %d\n", j);
    fprintf("Mass: %.4f kg\n", mass(j));
    fprintf("Initial position: [%.4f, %.4f] m\n", x_initial(j, 1), x_initial(j, 2));
    fprintf("Initial velocity: [%.4f, %.4f] m/s\n", v_initial(j, 1), v_initial(j, 2));
    fprintf("Initial speed: %.4f m/s\n", speed(1, j));
end

% Final results
fprintf("\nFinal Simulation Results\n");

for j = 1:n
    final_position = squeeze(position(end, j, :));
    final_velocity = squeeze(velocity(end, j, :));
    final_acceleration = squeeze(acceleration(end, j, :));
    final_force = squeeze(force(end, j, :));

    fprintf("\nBody %d\n", j);
    fprintf("Final position: [%.3f, %.3f] m\n",  final_position(1), final_position(2));
    fprintf("Final velocity: [%.3f, %.3f] m/s\n", final_velocity(1), final_velocity(2));
    fprintf("Final speed: %.3f m/s\n", speed(end, j));
    fprintf("Acceleration: [%.3f, %.3f] m/s^2\n", final_acceleration(1), final_acceleration(2));
    fprintf("Net force: [%.3f, %.3f] N\n", final_force(1), final_force(2));
end

%  Time history
for j = 1:n
    fprintf("\nTime History %d \n", j);

    body_position = [position(:, j, 1), position(:, j, 2)];
    body_velocity = [velocity(:, j, 1), velocity(:, j, 2)];
    body_acceleration = [acceleration(:, j, 1), acceleration(:, j, 2)];
    body_force = [force(:, j, 1), force(:, j, 2)];

    results = [time(:), body_position, body_velocity, speed(:, j), body_acceleration, body_force];

    disp("Columns: t, x, y, vx, vy, speed, ax, ay, Fx_net, Fy_net");
    disp(results);
end
