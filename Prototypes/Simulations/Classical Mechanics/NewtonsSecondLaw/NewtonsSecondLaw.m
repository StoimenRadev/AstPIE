clear;
clc;
close all;

% Parameters
mass = input("Enter mass (kg): "); %mass - kg
x = input("Enter position [x y] (m): "); %initial position - m
v = input("Enter velocity [vx vy] (m/s): "); %initial velocity -m/s

if mass <= 0
    error("Mass must be greater than zero!");
end
if numel(x) ~= 2 || numel(v) ~= 2
    error("Position and velocity must each contain two components!");
end

% Inital values
x_initial = x;
v_initial = v;

dt = input("Enter time step (s): "); %time step - s
T = input("Enter total simulation time (s): "); %simulation time - s

% Forces
F1 = input("Enter Force 1 [Fx Fy] (N): ");
F2 = input("Enter Force 2 [Fx Fy] (N): ");

if numel(F1) ~= 2 || numel(F2) ~= 2
    error("Each force must contain two components!");
end

Fnet = F1 + F2;
a = Fnet / mass;

if dt <= 0 || T <= 0
    error("Time step and total simulation time must be positive!");
end

if abs(T/dt - round(T/dt)) > 1e-10
    error("Total simulation time must be an integer multiple of the time step!");
end

time = 0:dt:T; %time array

% Storing arrays
position = zeros(length(time), 2);
velocity = zeros(length(time), 2);
acceleration = zeros(length(time), 2);
speed = zeros(length(time), 1);
net_force = zeros(length(time), 2);

% Storing initial state
position(1, :) = x;
velocity(1, :) = v;
acceleration(1, :) = a;
speed(1) = sqrt(v(1)^2 + v(2)^2);
net_force(1, :) = Fnet;


% Simulation loop
for i = 2:length(time)

    v_final = v + a * dt;
    x_final = x + v_final * dt;

    v = v_final;
    x = x_final;

    % Store results
    position(i, :) = x;
    velocity(i, :) = v;
    acceleration(i, :) = a;
    speed(i) = sqrt(v(1)^2 + v(2)^2);
    net_force(i, :) = Fnet;

end

% Calculating speed from the components of the velocity
speed_initial = speed(1);
speed_final = speed(end);

% Display final results
fprintf("\nSimulation results\n\n");

fprintf("Initial state\n");
fprintf("Initial position: [%.3f, %.3f] m\n", x_initial(1), x_initial(2));
fprintf("Initial velocity: [%.3f, %.3f] m/s\n", v_initial(1), v_initial(2));
fprintf("Initial speed: %.3f m/s\n", speed_initial);

fprintf("\nForces\n");
fprintf("Force 1: [%.3f, %.3f] N\n", F1(1), F1(2));
fprintf("Force 2: [%.3f, %.3f] N\n", F2(1), F2(2));
fprintf("Net force: [%.3f, %.3f] N\n", Fnet(1), Fnet(2));

fprintf("\nAcceleration\n");
fprintf("Acceleration: [%.3f, %.3f] m/s^2\n", a(1), a(2));

fprintf("\nFinal state\n");
fprintf("Final position: [%.3f, %.3f] m\n", x_final(1), x_final(2));
fprintf("Final velocity: [%.3f, %.3f] m/s\n", v_final(1), v_final(2));
fprintf("Final speed: %.3f m/s\n", speed_final);

fprintf("\nSimulation Information\n");
fprintf("Total simulation time: %.3f s\n", T);
fprintf("Simulation steps: %.0f\n", length(time) - 1);

results = [time(:), position, velocity, speed, acceleration, net_force];

disp("Time-history results:");
disp("Columns: t, x, y, vx, vy, speed, ax, ay,     Fx_net,  Fy_net");
disp(results);
