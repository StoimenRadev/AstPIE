clear;
clc;
close all;

% Parameters
mass = input("Enter mass (kg): "); %mass - kg
if mass <= 0
    error("Mass must be greater than zero!");
end
x = input("Enter position [x y] (m): "); %initial position - m
v = input("Enter velocity [vx vy] (m/s): "); %initial velocity -m/s

% Inital values
x_initial = x;
v_initial = v;

dt = input("Enter time step (s): "); %time step - s
T = input("Enter total simulation time (s): "); %simulation time - s

% Forces
F1 = input("Enter Force 1 [Fx Fy] (N): "); %force 1
F2 = input("Enter Force 2 [Fx Fy] (N): "); %force 2

Fnet = F1 + F2; %net force
a = Fnet/mass;

time = 0:dt:T; %time arrayfun
if dt <= 0 || T <= 0
    error("Time step and total simulation time must be positive!");
end

if abs(T/dt - round(T/dt)) > 1e-10
    error("Total simulation time must be an integer multiple of the time step!");
end

% Storing arrays
position = zeros(length(time), 2);
velocity = zeros(length(time), 2);
acceleration = zeros(length(time), 2);
acceleration(1, :) = a;

% Storing initial state
position(1, :) = x;
velocity(1, :) = v;

% Simulation loop
for i = 2:length(time)

  v_final = v + a*dt;
  x_final = x + v_final*dt;

  v = v_final;
  x = x_final;

  % Store results
  position(i, :) = x_final;
  velocity(i, :) = v_final;

  end

  % Calculating speed from the components of the velocity
speed_initial = sqrt(v_initial(1)^2 + v_initial(2)^2); % initial speed
speed_final = sqrt(v_final(1)^2 + v_final(2)^2); % final speed

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
