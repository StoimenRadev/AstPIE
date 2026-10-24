clear;
clc;
close all;

% Parameters
n = input("Enter number of bodies: ");

if ~isscalar(n) || ~isfinite(n) || n ~= 2
    error("This simulation requires exactly 2 bodies!");
end

mass = zeros(n, 1);
x = zeros(n, 2);
v = zeros(n, 2);

% Initial conditions
for i = 1:n
    mass(i) = input(sprintf("Enter mass of body %d (kg): ", i));
    x(i, :) = input(sprintf("Enter initial position of body %d [x y] (m): ", i));
    v(i, :) = input(sprintf("Enter initial velocity of body %d [vx vy] (m/s): ", i));

    if ~isscalar(mass(i)) || ~isfinite(mass(i)) || mass(i) <= 0
        error("Mass must be a positive finite number!");
    end

    if numel(x(i, :)) ~= 2 || any(~isfinite(x(i, :))) || numel(v(i, :)) ~= 2 || any(~isfinite(v(i, :)))
        error("Position and velocity must contain two finite components!");
    end
end

% Save initial values
x_initial = x;
v_initial = v;

% Simulation parameters
dt = input("Enter time step (s): ");
T = input("Enter total simulation time (s): ");

if ~isnumeric(dt) || ~isscalar(dt) || ~isreal(dt) || ~isfinite(dt) || dt <= 0 || ~isnumeric(T) || ~isscalar(T) || ~isreal(T) || ~isfinite(T) || T <= 0
    error("Time step and simulation time must be positive finite numbers!");
end

if abs(T / dt - round(T / dt)) > 1e-10
    error("Total simulation time must be an integer multiple of the time step!");
end

time = (0:round(T / dt))' * dt;
time(end) = T;
num_steps = length(time);

% Interaction forces
F1 = input("Enter interaction force on body 1 [Fx Fy] (N): ");
F2 = input("Enter interaction force on body 2 [Fx Fy] (N): ");

if ~isnumeric(F1) || numel(F1) ~= 2 || ~isreal(F1) || any(~isfinite(F1(:))) || ~isnumeric(F2) || numel(F2) ~= 2 ||  ~isreal(F2) || any(~isfinite(F2(:)))
    error("Each interaction force must contain two finite real components!");
end

F1 = F1(:)';
F2 = F2(:)';

% Newton's Third Law
force_residual = F1 + F2;
force_residual_magnitude = sqrt( force_residual(1)^2 + force_residual(2)^2);

if force_residual_magnitude > 1e-10
    error("Newton's Third Law failed: forces must be equal and opposite!");
end

Fext1 = input("Enter external force on body 1 [Fx Fy] (N): ");
Fext2 = input("Enter external force on body 2 [Fx Fy] (N): ");

if ~isnumeric(Fext1) || numel(Fext1) ~= 2 || ~isreal(Fext1) || any(~isfinite(Fext1(:))) || ~isnumeric(Fext2) || numel(Fext2) ~= 2 || ~isreal(Fext2) || any(~isfinite(Fext2(:)))
    error("Each external force must contain two finite real components!");
end

Fext1 = Fext1(:)';
Fext2 = Fext2(:)';

% Net force and acceleration
Fnet = zeros(n, 2);
Fnet(1, :) = F1 + Fext1;
Fnet(2, :) = F2 + Fext2;

a = Fnet ./ mass;

% Store simulation data
position = zeros(num_steps, n, 2);
velocity = zeros(num_steps, n, 2);
acceleration = zeros(num_steps, n, 2);
force = zeros(num_steps, n, 2);
speed = zeros(num_steps, n);

position(1, :, :) = x;
velocity(1, :, :) = v;
acceleration(1, :, :) = a;
force(1, :, :) = Fnet;
speed(1, :) = sqrt(sum(v.^2, 2))';

% Simulation loop: velocity Verlet
for i = 2:num_steps

    % Save previous velocity and acceleration
    v_previous = v;
    a_previous = a;

    x = x + v_previous * dt + 0.5 * a_previous * dt^2;

    % Calculate acceleration at the new state
    a_new = Fnet ./ mass;

    v = v_previous + 0.5 * (a_previous + a_new) * dt;

    a = a_new;

    position(i, :, :) = x;
    velocity(i, :, :) = v;
    acceleration(i, :, :) = a;
    force(i, :, :) = Fnet;
    speed(i, :) = sqrt(sum(v.^2, 2))';

end

% Analytical solution for constant acceleration
analytical_position = zeros(num_steps, n, 2);
analytical_velocity = zeros(num_steps, n, 2);
analytical_speed = zeros(num_steps, n);

for i = 1:num_steps
    for j = 1:n
        analytical_position(i, j, 1) = x_initial(j, 1) + v_initial(j, 1) * time(i) + 0.5 * a(j, 1) * time(i)^2;

        analytical_position(i, j, 2) = x_initial(j, 2) + v_initial(j, 2) * time(i) + 0.5 * a(j, 2) * time(i)^2;

        analytical_velocity(i, j, 1) = v_initial(j, 1) + a(j, 1) * time(i);

        analytical_velocity(i, j, 2) = v_initial(j, 2) + a(j, 2) * time(i);

        analytical_speed(i, j) = sqrt(analytical_velocity(i, j, 1)^2 + analytical_velocity(i, j, 2)^2);
    end
end

% Semi-implicit Euler simulation
euler_position = zeros(num_steps, n, 2);
euler_velocity = zeros(num_steps, n, 2);

euler_position(1, :, :) = x_initial;
euler_velocity(1, :, :) = v_initial;

x_euler_history = x_initial;
v_euler_history = v_initial;

for i = 2:num_steps

    v_euler_history = v_euler_history + a * dt;
    x_euler_history = x_euler_history + v_euler_history * dt;

    euler_position(i, :, :) = x_euler_history;
    euler_velocity(i, :, :) = v_euler_history;

end

% Euler numerical error analysis
euler_position_error = zeros(num_steps, n);
euler_velocity_error = zeros(num_steps, n);

for i = 1:num_steps
    for j = 1:n

        position_error_x = euler_position(i, j, 1) - analytical_position(i, j, 1);
        position_error_y = euler_position(i, j, 2) - analytical_position(i, j, 2);

        velocity_error_x = euler_velocity(i, j, 1) - analytical_velocity(i, j, 1);
        velocity_error_y = euler_velocity(i, j, 2) - analytical_velocity(i, j, 2);

        euler_position_error(i, j) = sqrt(position_error_x^2 + position_error_y^2);
        euler_velocity_error(i, j) = sqrt(velocity_error_x^2 + velocity_error_y^2);

    end
end

% Numerical error analysis
position_error = zeros(num_steps, n);
velocity_error = zeros(num_steps, n);

for i = 1:num_steps
    for j = 1:n
        position_error_x = position(i, j, 1) - analytical_position(i, j, 1);
        position_error_y = position(i, j, 2) - analytical_position(i, j, 2);

        velocity_error_x = velocity(i, j, 1) - analytical_velocity(i, j, 1);
        velocity_error_y = velocity(i, j, 2) - analytical_velocity(i, j, 2);

        position_error(i, j) = sqrt( ...
            position_error_x^2 + position_error_y^2);

        velocity_error(i, j) = sqrt(velocity_error_x^2 + velocity_error_y^2);
    end
end

% Final values
x_final = zeros(n, 2);
v_final = zeros(n, 2);
analytical_position_final = zeros(n, 2);
analytical_velocity_final = zeros(n, 2);

for j = 1:n
    x_final(j, :) = position(end, j, :);
    v_final(j, :) = velocity(end, j, :);

    analytical_position_final(j, :) = analytical_position(end, j, :);
    analytical_velocity_final(j, :) = analytical_velocity(end, j, :);
end

speed_initial = speed(1, :);
speed_final = speed(end, :);

final_position_error = position_error(end, :);
final_velocity_error = velocity_error(end, :);

% Position visualization
figure;
hold on;

% Semi-implicit Euler trajectories
plot(euler_position(:, 1, 1), euler_position(:, 1, 2), 'r:', 'LineWidth', 1.5);
plot(euler_position(:, 2, 1), euler_position(:, 2, 2), 'b:', 'LineWidth', 1.5);

% Velocity Verlet trajectories
plot(position(:, 1, 1), position(:, 1, 2), 'r-', 'LineWidth', 1.5);
plot(position(:, 2, 1), position(:, 2, 2), 'b-', 'LineWidth', 1.5);

% Analytical trajectories
plot(analytical_position(:, 1, 1), analytical_position(:, 1, 2), 'r--', 'LineWidth', 1.2);
plot(analytical_position(:, 2, 1), analytical_position(:, 2, 2), 'b--', 'LineWidth', 1.2);

% Initial positions
plot(x_initial(1, 1), x_initial(1, 2), 'ro', 'MarkerFaceColor', 'r');
plot(x_initial(2, 1), x_initial(2, 2), 'bo', 'MarkerFaceColor', 'b');

% Interaction forces
force_scale = 0.2;

quiver(x_initial(1, 1), x_initial(1, 2), force_scale * F1(1), force_scale * F1(2), 0, 'r', 'LineWidth', 1.5);
quiver(x_initial(2, 1), x_initial(2, 2), force_scale * F2(1), force_scale * F2(2), 0, 'b', 'LineWidth', 1.5);

xlabel("x position (m)");
ylabel("y position (m)");
title("Trajectories: Semi-implicit Euler, Velocity Verlet and Analytical");
legend("Body 1 Euler", "Body 2 Euler", "Body 1 Verlet", "Body 2 Verlet", "Body 1 analytical", "Body 2 analytical", "Body 1 initial", "Body 2 initial", "Force on Body 1", "Force on Body 2", "Location", "northeast");

grid on;
axis equal;
hold off;

% Position vs time
figure;
subplot(2, 1, 1);

% Euler
plot(time, euler_position(:, 1, 1), 'r:',
    time, euler_position(:, 2, 1), 'b:');
hold on;

% Verlet
plot(time, position(:, 1, 1), 'r-', time, position(:, 2, 1), 'b-');

% Analytical
plot(time, analytical_position(:, 1, 1), 'r--', time, analytical_position(:, 2, 1), 'b--');

xlabel("Time (s)");
ylabel("X-position (m)");
title("X-position vs Time");
legend("Body 1 Euler", "Body 2 Euler", "Body 1 Verlet", "Body 2 Verlet", "Body 1 analytical", "Body 2 analytical");
grid on;
hold off;

subplot(2, 1, 2);

% Euler
plot(time, euler_position(:, 1, 2), 'r:', time, euler_position(:, 2, 2), 'b:');
hold on;

% Verlet
plot(time, position(:, 1, 2), 'r-', time, position(:, 2, 2), 'b-');

% Analytical
plot(time, analytical_position(:, 1, 2), 'r--', time, analytical_position(:, 2, 2), 'b--');

xlabel("Time (s)");
ylabel("Y-position (m)");
title("Y-position vs Time");
legend("Body 1 Euler", "Body 2 Euler", "Body 1 Verlet", "Body 2 Verlet", "Body 1 analytical", "Body 2 analytical");
grid on;
hold off;

% Velocity vs time
figure;
subplot(2, 1, 1);

% Euler
plot(time, euler_velocity(:, 1, 1), 'r:', time, euler_velocity(:, 2, 1), 'b:');
hold on;

% Verlet
plot(time, velocity(:, 1, 1), 'r-', time, velocity(:, 2, 1), 'b-');

% Analytical
plot(time, analytical_velocity(:, 1, 1), 'r--', time, analytical_velocity(:, 2, 1), 'b--');

xlabel("Time (s)");
ylabel("X-velocity (m/s)");
title("X-velocity vs Time");
legend("Body 1 Euler", "Body 2 Euler", "Body 1 Verlet", "Body 2 Verlet", "Body 1 analytical", "Body 2 analytical");
grid on;
hold off;

subplot(2, 1, 2);

% Euler
plot(time, euler_velocity(:, 1, 2), 'r:', time, euler_velocity(:, 2, 2), 'b:');
hold on;

% Verlet
plot(time, velocity(:, 1, 2), 'r-', time, velocity(:, 2, 2), 'b-');

% Analytical
plot(time, analytical_velocity(:, 1, 2), 'r--', time, analytical_velocity(:, 2, 2), 'b--');

xlabel("Time (s)");
ylabel("Y-velocity (m/s)");
title("Y-velocity vs Time");
legend("Body 1 Euler", "Body 2 Euler", "Body 1 Verlet", "Body 2 Verlet", "Body 1 analytical", "Body 2 analytical");
grid on;
hold off;

% Position and velocity errors
figure;

subplot(2, 1, 1);

% Euler
plot(time, euler_position_error(:, 1), 'r:', time, euler_position_error(:, 2), 'b:');
hold on;

% Verlet
plot(time, position_error(:, 1), 'r-', time, position_error(:, 2), 'b-');

xlabel("Time (s)");
ylabel("Position error (m)");
title("Position Error vs Time");
legend("Body 1 Euler", "Body 2 Euler", "Body 1 Verlet", "Body 2 Verlet");
grid on;
hold off;

subplot(2, 1, 2);

% Euler
plot(time, euler_velocity_error(:, 1), 'r:', time, euler_velocity_error(:, 2), 'b:');
hold on;

% Verlet
plot(time, velocity_error(:, 1), 'r-', time, velocity_error(:, 2), 'b-');

xlabel("Time (s)");
ylabel("Velocity error (m/s)");
title("Velocity Error vs Time");
legend("Body 1 Euler", "Body 2 Euler", "Body 1 Verlet", "Body 2 Verlet");
grid on;
hold off;

% Simulation results
fprintf("\nSimulation Results - Velocity Verlet\n");

fprintf("\nSimulation Information\n");
fprintf("Number of bodies: %d\n", n);
fprintf("Time step: %.3f s\n", dt);
fprintf("Total simulation time: %.3f s\n", T);
fprintf("Simulation steps: %d\n", num_steps - 1);

fprintf("\nInteraction Forces\n");
fprintf("Force on body 1: [%.3f, %.3f] N\n", F1(1), F1(2));
fprintf("Force on body 2: [%.3f, %.3f] N\n", F2(1), F2(2));
fprintf("Newton's Third Law residual: [%.3g, %.3g] N\n", force_residual(1), force_residual(2));

fprintf("\nBody Results - Velocity  Verlet\n");

for j = 1:n
    fprintf("\nBody %d\n", j);
    fprintf("Mass: %.6f kg\n", mass(j));
    fprintf("Initial position: [%.3f, %.3f] m\n", x_initial(j, 1), x_initial(j, 2));
    fprintf("Initial velocity: [%.3f, %.3f] m/s\n", v_initial(j, 1), v_initial(j, 2));
    fprintf("Initial speed: %.3f m/s\n", speed_initial(j));

    fprintf("Net force: [%.3f, %.3f] N\n", Fnet(j, 1), Fnet(j, 2));
    fprintf("Acceleration: [%.3f, %.3f] m/s^2\n", a(j, 1), a(j, 2));

    fprintf("Final numerical position: [%.3f, %.3f] m\n", x_final(j, 1), x_final(j, 2));
    fprintf("Final analytical position: [%.3f, %.3f] m\n", analytical_position_final(j, 1), analytical_position_final(j, 2));

    fprintf("Final numerical velocity: [%.3f, %.3f] m/s\n", v_final(j, 1), v_final(j, 2));
    fprintf("Final analytical velocity: [%.3f, %.3f] m/s\n", analytical_velocity_final(j, 1), analytical_velocity_final(j, 2));

    fprintf("Final speed: %.3f m/s\n", speed_final(j));
    fprintf("Final position error: %.3g m\n", final_position_error(j));
    fprintf("Final velocity error: %.3g m/s\n", final_velocity_error(j));
end

% Time-step comparison
fprintf("\nTime-Step and Integration Method Comparison\n");

dt_half = dt / 2;

% Initial conditions for each method
x_euler = x_initial;
v_euler = v_initial;

x_verlet = x_initial;
v_verlet = v_initial;

x_verlet_half = x_initial;
v_verlet_half = v_initial;

% Semi-implicit Euler with original time step
for i = 1:round(T / dt)

    v_euler = v_euler + a * dt;
    x_euler = x_euler + v_euler * dt;

end

% Velocity Verlet with original time step
for i = 1:round(T / dt)

    x_verlet = x_verlet + v_verlet * dt + 0.5 * a * dt^2;
    v_verlet = v_verlet + a * dt;

end

% Velocity Verlet with half time step
for i = 1:round(T / dt_half)

    x_verlet_half = x_verlet_half + v_verlet_half * dt_half + 0.5 * a * dt_half^2;

    v_verlet_half = v_verlet_half + a * dt_half;

end

% Comparing the numerical methods
for j = 1:n

    analytical_x = x_initial(j, 1) + v_initial(j, 1) * T + 0.5 * a(j, 1) * T^2;
    analytical_y = x_initial(j, 2) + v_initial(j, 2) * T + 0.5 * a(j, 2) * T^2;

    error_euler = sqrt((x_euler(j, 1) - analytical_x)^2 + (x_euler(j, 2) - analytical_y)^2);

    error_verlet = sqrt((x_verlet(j, 1) - analytical_x)^2 + (x_verlet(j, 2) - analytical_y)^2);

     error_verlet_half = sqrt((x_verlet_half(j, 1) - analytical_x)^2 + (x_verlet_half(j, 2) - analytical_y)^2);

    fprintf("\nBody %d\n", j);
    fprintf("Analytical final position: [%.6f, %.6f] m\n", analytical_x, analytical_y);
    fprintf("Semi-implicit Euler final position: [%.6f, %.6f] m\n", x_euler(j, 1), x_euler(j, 2));
    fprintf("Velocity Verlet final position: [%.6f, %.6f] m\n", x_verlet(j, 1), x_verlet(j, 2));
    fprintf("Velocity Verlet final position with dt/2: [%.6f, %.6f] m\n", x_verlet_half(j, 1), x_verlet_half(j, 2));
    fprintf("Semi-implicit Euler final velocity: [%.6f, %.6f] m/s\n", v_euler(j, 1), v_euler(j, 2));
    fprintf("Velocity Verlet final velocity: [%.6f, %.6f] m/s\n", v_verlet(j, 1), v_verlet(j, 2));
    fprintf("Velocity Verlet final velocity with dt/2: [%.6f, %.6f] m/s\n",  v_verlet_half(j, 1), v_verlet_half(j, 2));
    fprintf("Semi-implicit Euler position error (dt = %.6g s): %.6g m\n", dt, error_euler);
    fprintf("Velocity Verlet position error (dt = %.6g s): %.6g m\n", dt, error_verlet);
    fprintf("Velocity Verlet position error (dt = %.6g s): %.6g m\n", dt_half, error_verlet_half);

end

% Time history for the main velocity Verlet simulation
for j = 1:n

    fprintf("\nTime History - Body %d (Velocity Verlet)\n", j);

    fprintf("Columns: Time (s), X (m), Y (m), VX (m/s), VY (m/s), Speed (m/s), AX (m/s^2), AY (m/s^2), FX (N), FY (N), Analytical X (m), Analytical Y (m), Analytical VX (m/s), Analytical VY (m/s), Analytical Speed (m/s), Position Error (m), Velocity Error (m/s)\n");

    results = [time, ...
        position(:, j, 1), ...
        position(:, j, 2), ...
        velocity(:, j, 1), ...
        velocity(:, j, 2), ...
        speed(:, j), ...
        acceleration(:, j, 1), ...
        acceleration(:, j, 2), ...
        force(:, j, 1), ...
        force(:, j, 2), ...
        analytical_position(:, j, 1), ...
        analytical_position(:, j, 2), ...
        analytical_velocity(:, j, 1), ...
        analytical_velocity(:, j, 2), ...
        analytical_speed(:, j), ...
        position_error(:, j), ...
        velocity_error(:, j)];

    disp(results);

end
