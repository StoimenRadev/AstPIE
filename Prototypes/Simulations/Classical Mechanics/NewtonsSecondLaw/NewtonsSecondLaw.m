clear;
clc;
close all;

% Parameters
mass = input("Enter mass (kg): "); % mass - kg
x = input("Enter position [x y] (m): "); % initial position - m
v = input("Enter velocity [vx vy] (m/s): "); % initial velocity - m/s

if mass <= 0
    error("Mass must be greater than zero!");
end

if numel(x) ~= 2 || numel(v) ~= 2
    error("Position and velocity must each contain two components!");
end

% Initial values
x_initial = x;
v_initial = v;

dt = input("Enter time step (s): "); % time step - s
T = input("Enter total simulation time (s): "); % simulation time - s

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

time = 0:dt:T; % time array

if abs(time(end) - T) > 1e-10
    error("Final simulation time does not match T (total time)!");
end

% Storing numerical arrays
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

% Simulation loop: semi-implicit Euler integration
for i = 2:length(time)

    v_final = v + a * dt;
    x_final = x + v_final * dt;

    v = v_final;
    x = x_final;

    % Store numerical results
    position(i, :) = x;
    velocity(i, :) = v;
    acceleration(i, :) = a;
    speed(i) = sqrt(v(1)^2 + v(2)^2);
    net_force(i, :) = Fnet;

end

% Numerical initial and final speeds
speed_initial = speed(1);
speed_final = speed(end);

% Analytical solution for constant acceleration
analytical_position = repmat(x_initial, length(time), 1) + time(:) * v_initial + 0.5 * (time(:).^2) * a;
analytical_velocity = repmat(v_initial, length(time), 1) + time(:) * a;

% Analytical speed
analytical_speed = sqrt(sum(analytical_velocity.^2, 2));

% Numerical error at every timestep
position_error_components = position - analytical_position;
velocity_error_components = velocity - analytical_velocity;

% Error magnitudes
position_error = sqrt(sum(position_error_components.^2, 2));
velocity_error = sqrt(sum(velocity_error_components.^2, 2));

% Final analytical results
analytical_position_final = analytical_position(end, :);
analytical_velocity_final = analytical_velocity(end, :);
analytical_speed_final = analytical_speed(end);

% Final error values
final_position_error = position_error(end);
final_velocity_error = velocity_error(end);

% Final numerical state
x_final = position(end, :);
v_final = velocity(end, :);

% Position visualization
figure;
plot(position(:, 1), position(:, 2));
hold on;

plot(analytical_position(:, 1), analytical_position(:, 2), "--");
plot(x_initial(1), x_initial(2), "o");
plot(x_final(1), x_final(2), "o");

% Force vectors
quiver(x_final(1), x_final(2), F1(1), F1(2), 0, 'r', 'linewidth', 1.5);

quiver(x_final(1), x_final(2), F2(1), F2(2), 0, 'b', 'linewidth', 1.5);

quiver(x_final(1), x_final(2), Fnet(1), Fnet(2), 0, 'g', 'linewidth', 2);

quiver(x_final(1), x_final(2), v_final(1), v_final(2), 0, 'y', 'linewidth', 2);

% Force 1 label
angle_1 = atan2(F1(2), F1(1)) * 180/pi;

if angle_1 > 90
    angle_1 = angle_1 - 180;
elseif angle_1 < -90
    angle_1 = angle_1 + 180;
end

text(x_final(1) + F1(1)/2, x_final(2) + F1(2)/2, "Force 1", "Rotation", angle_1, "HorizontalAlignment", "center", "VerticalAlignment", "middle");

% Force 2 label
angle_2 = atan2(F2(2), F2(1)) * 180/pi;

if angle_2 > 90
    angle_2 = angle_2 - 180;
elseif angle_2 < -90
    angle_2 = angle_2 + 180;
end

text(x_final(1) + F2(1)/2, x_final(2) + F2(2)/2, "Force 2", "Rotation", angle_2, "HorizontalAlignment", "center", "VerticalAlignment", "middle");

% Net force and acceleration label
angle_3 = atan2(Fnet(2), Fnet(1)) * 180/pi;

if angle_3 > 90
    angle_3 = angle_3 - 180;
elseif angle_3 < -90
    angle_3 = angle_3 + 180;
end

net_force_and_acceleration_text = sprintf("Net Force: [%.3f, %.3f] N\nAcceleration: [%.3f, %.3f] m/s^2", Fnet(1), Fnet(2), a(1), a(2));

text(x_final(1) + Fnet(1)/2, x_final(2) + Fnet(2)/2, net_force_and_acceleration_text, "Rotation", angle_3, "HorizontalAlignment", "center", "VerticalAlignment", "middle");

% Velocity label
angle_4 = atan2(v_final(2), v_final(1)) * 180/pi;

if angle_4 > 90
    angle_4 = angle_4 - 180;
elseif angle_4 < -90
    angle_4 = angle_4 + 180;
end

velocity_text = sprintf("Final Velocity: [%.3f, %.3f] m/s", v_final(1), v_final(2));

text(x_final(1) + v_final(1)/2, x_final(2) + v_final(2)/2, velocity_text, "Rotation", angle_4, "HorizontalAlignment", "center", "VerticalAlignment", "middle");

grid on;
axis equal;
xlabel("x position (m)");
ylabel("y position (m)");
title("Numerical and Analytical Trajectories");

legend("Numerical trajectory", "Analytical trajectory", "Initial position", "Final position", "Location", "best");

% Position vs time graph
figure;
plot(time, position(:, 1));
hold on;
plot(time, position(:, 2));
plot(time, analytical_position(:, 1), "--");
plot(time, analytical_position(:, 2), "--");

title("Numerical and Analytical Position vs. Time");
xlabel("Time (s)");
ylabel("Position (m)");

legend("Numerical x position", "Numerical y position", "Analytical x position", "Analytical y position");
grid on;

% Velocity vs time graph
figure;

plot(time, velocity(:, 1));
hold on;
plot(time, velocity(:, 2));
plot(time, analytical_velocity(:, 1), "--");
plot(time, analytical_velocity(:, 2), "--");

title("Numerical and Analytical Velocity vs. Time");
xlabel("Time (s)");
ylabel("Velocity (m/s)");

legend("Numerical vx", "Numerical vy", "Analytical vx", "Analytical vy");
grid on;

% Position error vs time
figure;
plot(time, position_error);
title("Position Error vs. Time");
xlabel("Time (s)");
ylabel("Absolute Position Error (m)");
grid on;

% Velocity error vs time
figure;
plot(time, velocity_error);
title("Velocity Error vs. Time");
xlabel("Time (s)");
ylabel("Absolute Velocity Error (m/s)");
grid on;

% Display final results
fprintf("\nSimulation results\n\n");

fprintf("Initial state\n");
fprintf("Initial position: [%.6f, %.6f] m\n", x_initial(1), x_initial(2));
fprintf("Initial velocity: [%.6f, %.6f] m/s\n", v_initial(1), v_initial(2));
fprintf("Initial speed: %.6f m/s\n", speed_initial);

fprintf("\nForces\n");
fprintf("Force 1: [%.6f, %.6f] N\n", F1(1), F1(2));
fprintf("Force 2: [%.6f, %.6f] N\n", F2(1), F2(2));
fprintf("Net force: [%.6f, %.6f] N\n", Fnet(1), Fnet(2));

fprintf("\nAcceleration\n");
fprintf("Acceleration: [%.6f, %.6f] m/s^2\n", a(1), a(2));

fprintf("\nFinal numerical state\n");
fprintf("Final numerical position: [%.6f, %.6f] m\n", x_final(1), x_final(2));
fprintf("Final numerical: [%.6f, %.6f] m/s\n", v_final(1), v_final(2));
fprintf("Final numerical speed: %.6f m/s\n", speed_final);

fprintf("\nAnalytical solution\n");
fprintf("Final analytical position: [%.6f, %.6f] m\n", analytical_position_final(1), analytical_position_final(2));
fprintf("Final analytical velocity: [%.6f, %.6f] m/s\n", analytical_velocity_final(1), analytical_velocity_final(2));
fprintf("Final analytical speed: %.6f m/s\n", analytical_speed_final);

fprintf("\nNumerical error analysis\n");
fprintf("Final position error: %.9f m\n", final_position_error);
fprintf("Final velocity error: %.9f m/s\n", final_velocity_error);
fprintf("Maximum position error: %.9f m\n", max(position_error));
fprintf("Maximum velocity error: %.9f m/s\n", max(velocity_error));

fprintf("\nSimulation information\n");
fprintf("Total simulation time: %.6f s\n", T);
fprintf("Time step: %.6f s\n", dt);
fprintf("Simulation steps: %.0f\n", length(time) - 1);

% Combined numerical and analytical results
results = [time(:), position, velocity, speed, acceleration, net_force, analytical_position, analytical_velocity, analytical_speed, position_error, velocity_error];
disp("Time-history results:");
disp(["t, x, y, vx, vy, speed, ax, ay, Fx_net, Fy_net, ", "x_analytical, y_analytical, vx_analytical, vy_analytical, ", "analytical_speed, position_error, velocity_error"]);
disp(results);

