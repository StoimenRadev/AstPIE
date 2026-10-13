clear;
clc;
close all;

% Parameters
mass = 5; %mass - kg
x = [0, 0]; %initial position - m
v = [5, 0]; %initial velocity -m/s

dt = 0.01; %time step - s
T = 10; %simulation time - s

% Forces
F1 = [10, 0]; %force 1
F2 = [-5, 0]; %force 2

Fnet = F1 + F2; %net force
a = Fnet/mass;

time = 0:dt:T; %time arrayfun

% Storing arrays
position = zeros(length(time), 2);
velocity = zeros(length(time), 2);
acceleration = zeros(length(time), 2);
acceleration(1, :) = a;

% Simulation loop
for i = 2:length(time)

  v_new = v + a*dt;
  x_new = x + v_new*dt;

  v = v_new;
  x = x_new;

  %store results
  position(i, :) = x;
  velocity(i, :) = v;
end

% Display final results
disp("Final position (m):");
disp(x);

disp("Final velocity:");
disp(v);

disp("Acceleration:");
disp(a);
