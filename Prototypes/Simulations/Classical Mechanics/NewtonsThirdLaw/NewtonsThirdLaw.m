clear;
clc;
close all;

% Parameters
n = 2; % number of bodies
mass = [5; 2];
% initial positions - m (x,y)
x = [0, 0;
     2, 0];
% initial velocities - m/s (vx,vy)
v = [5, 0;
     0, 0];

dt = 0.01; % time step - s
T = 10; % simulation time - s

F1 = [10, 0]; % force acting on body 1
F2 = [-10, 0]; % force acting on body 2

% test for letting only the newton's first law work
if any(F2 != -F1)
    error("Newton's Third Law test failed: forces are not equal and opposite.");
else
    disp("Newton's Third Law test passed.");
end

% calculating net force
Fnet = zeros(n, 2);
Fnet(1, :) = F1;
Fnet(2, :) = F2;

% calculating acceleration
a = Fnet ./ mass

time = 0:dt:T; % Time array

% Storing array
position = zeros(length(time), n, 2);
velocity = zeros(length(time), n, 2);
acceleration = zeros(length(time), n, 2);

position(1, :, :) = x;
velocity(1, :, :) = v;
acceleration(1, :, :) = a;

% Simulation loop
for i = 2:length(time)

  v_final = v = a * dt; % update velocity
  x_final = x + v_final * dt; % update position

  % update current state
  v = v_final;
  x = x_final;

  % Store results
  position(i, :, :) = x;
  velocity(i, :, :) = v;
  acceleration(i, :, :) = a;

end

% Display final results
for j = 1:n

    fprintf("\nSimulation results\n");
    fprintf("\nBody %d\n", j);
    fprintf("Mass (kg): %.3f\n", mass(j));
    fprintf("Final position (m): [%.3f, %.3f]\n", x(j, 1), x(j, 2));
    fprintf("Final velocity (m/s): [%.3f, %.3f]\n", v(j, 1), v(j, 2));
    fprintf("Acceleration (m/s^2): [%.3f, %.3f]\n", a(j, 1), a(j, 2));
    fprintf("Net force (N): [%.3f, %.3f]\n", Fnet(j, 1), Fnet(j, 2));

end
