clear;
clc;
close all;

star_mass = input("Enter the mass of the star (kg): ");
planet_mass = input("Enter the mass of the planet (kg): ");

r0_planet = input("Enter the initial position [rx ry] (m): ");
v0_planet = input("Enter the initial velocity [vx vy] (m/s): ");

dt = input("Enter the time step (s): ");
T = input("Enter the total simulation duration (s): ");

% Input Validation
if ~isnumeric(star_mass) || ~isreal(star_mass) || ~isscalar(star_mass) || ~isfinite(star_mass) || star_mass <= 0
    error("Star mass must be a finite, positive scalar.");
end

if ~isnumeric(planet_mass) || ~isreal(planet_mass) || ~isscalar(planet_mass) || ~isfinite(planet_mass) || planet_mass <= 0
    error("Planet mass must be a finite, positive scalar.");
end

if ~isnumeric(r0_planet) || ~isreal(r0_planet) || ~isvector(r0_planet) || numel(r0_planet) ~= 2 || any(~isfinite(r0_planet(:)))
    error("Initial position must be a finite, real, two-component vector.");
end

if ~isnumeric(v0_planet) || ~isreal(v0_planet) || ~isvector(v0_planet) || numel(v0_planet) ~= 2 || any(~isfinite(v0_planet(:)))
    error("Initial velocity must be a finite, real, two-component vector.");
end

r0_planet = r0_planet(:).';
v0_planet = v0_planet(:).';

if ~isnumeric(dt) || ~isreal(dt) || ~isscalar(dt) || ~isfinite(dt) || dt <= 0
    error("Time step must be a finite, positive scalar.");
end

if ~isnumeric(T) || ~isreal(T) || ~isscalar(T) || ~isfinite(T) || T <= 0
    error("Simulation duration must be a finite, positive scalar.");
end

step_ratio = T / dt;

if ~isfinite(step_ratio) || step_ratio < 1 || abs(step_ratio - round(step_ratio)) > 1e-10 * max(1, abs(step_ratio))
    error("Simulation duration must be an integer multiple of the time step.");
end

num_steps = round(step_ratio);
actual_duration = num_steps * dt;
time_history = (0:num_steps)' * dt;

% Gravitational Parameters
G = 6.67430e-11;
mu_star = G * star_mass;

if ~isfinite(mu_star) || mu_star <= 0
    error("The gravitational parameter is invalid.");
end

r0 = sqrt(r0_planet(1)^2 + r0_planet(2)^2);

if ~isfinite(r0) || r0 == 0
    error("Initial position cannot be at the star.");
end

v0_squared = v0_planet(1)^2 + v0_planet(2)^2;

% Analytical variables
specific_energy = v0_squared / 2 - mu_star / r0;

if ~isfinite(specific_energy) || specific_energy >= 0
    error("Initial conditions must describe a bound elliptical orbit.");
end

specific_angular_momentum = r0_planet(1) * v0_planet(2) - r0_planet(2) * v0_planet(1);

if ~isfinite(specific_angular_momentum) || specific_angular_momentum == 0
    error("Initial conditions describe a radial trajectory.");
end

eccentricity_squared = 1 + (2 * specific_energy * specific_angular_momentum^2) / mu_star^2;

if eccentricity_squared < 0 && eccentricity_squared > -1e-12
    eccentricity_squared = 0;
end

if ~isfinite(eccentricity_squared) || eccentricity_squared < 0 || eccentricity_squared >= 1
    error("The calculated eccentricity is invalid for an ellipse.");
end

eccentricity = sqrt(eccentricity_squared);

position_velocity_dot = r0_planet(1) * v0_planet(1) + r0_planet(2) * v0_planet(2);

eccentricity_x = ((v0_squared - mu_star / r0) * r0_planet(1) - position_velocity_dot * v0_planet(1)) / mu_star;
eccentricity_y = ((v0_squared - mu_star / r0) * r0_planet(2) - position_velocity_dot * v0_planet(2)) / mu_star;

if eccentricity < 1e-10
    perihelion_angle = 0;
else
    perihelion_angle = atan2(eccentricity_y, eccentricity_x);
end

semi_major_axis = -mu_star / (2 * specific_energy);
semi_minor_axis = semi_major_axis * sqrt(1 - eccentricity^2);
focal_distance = semi_major_axis * eccentricity;

perihelion_distance = semi_major_axis * (1 - eccentricity);
aphelion_distance = semi_major_axis * (1 + eccentricity);

% Analytical Ellipse
E = linspace(0, 2*pi, 3601);

x_ellipse_local = semi_major_axis * cos(E) - focal_distance;
y_ellipse_local = semi_minor_axis * sin(E);

x_ellipse = x_ellipse_local * cos(perihelion_angle) - y_ellipse_local * sin(perihelion_angle);
y_ellipse = x_ellipse_local * sin(perihelion_angle) + y_ellipse_local * cos(perihelion_angle);

% Display Simulation ParametersDISPLAY SIMULATION PARAMETERS
fprintf("\nSIMULATION PARAMETERS\n");
fprintf("Star mass: %.3e kg\n", star_mass);
fprintf("Planet mass: %.3e kg\n", planet_mass);
fprintf("Gravitational parameter: %.3e m^3/s^2\n", mu_star);
fprintf("Initial position: [%.3e, %.3e] m\n", r0_planet(1), r0_planet(2));
fprintf("Initial velocity: [%.3f, %.3f] m/s\n", v0_planet(1), v0_planet(2));
fprintf("Time step: %.3f s\n", dt);
fprintf("Requested duration: %.3f s\n", T);
fprintf("Actual duration: %.3f s\n", actual_duration);
fprintf("Number of integration steps: %d\n", num_steps);

% Semi-Implicit Euler Integration
% Euler is retained as the comparison method.
euler_planet_positions = zeros(num_steps + 1, 2);
euler_planet_velocities = zeros(num_steps + 1, 2);
euler_planet_accelerations = zeros(num_steps + 1, 2);

r_planet = r0_planet;
v_planet = v0_planet;

euler_planet_positions(1, :) = r_planet;
euler_planet_velocities(1, :) = v_planet;

a_planet = -(mu_star / r0^3) * r_planet;
euler_planet_accelerations(1, :) = a_planet;

for i = 1:num_steps

    r_star = sqrt(r_planet(1)^2 + r_planet(2)^2);

    if ~isfinite(r_star) || r_star == 0
        error("Euler integration reached an invalid position.");
    end

    a_planet = -(mu_star / r_star^3) * r_planet;

    % Semi-implicit Euler: update velocity, then position.
    v_planet = v_planet + a_planet * dt;
    r_planet = r_planet + v_planet * dt;

    if any(~isfinite(r_planet)) || any(~isfinite(v_planet))
        error("Euler integration produced invalid numerical values.");
    end

    euler_planet_positions(i + 1, :) = r_planet;
    euler_planet_velocities(i + 1, :) = v_planet;

    r_star = sqrt(r_planet(1)^2 + r_planet(2)^2);

    if ~isfinite(r_star) || r_star == 0
        error("Euler integration reached an invalid position.");
    end

    a_planet = -(mu_star / r_star^3) * r_planet;
    euler_planet_accelerations(i + 1, :) = a_planet;

end

% Velocity Verlet Integration
% Velocity Verlet is the primary integration method.
planet_positions = zeros(num_steps + 1, 2);
planet_velocities = zeros(num_steps + 1, 2);
planet_accelerations = zeros(num_steps + 1, 2);

r_planet = r0_planet;
v_planet = v0_planet;

planet_positions(1, :) = r_planet;
planet_velocities(1, :) = v_planet;

a_planet = -(mu_star / r0^3) * r_planet;
planet_accelerations(1, :) = a_planet;

for i = 1:num_steps

    % Calculate the new position.
    r_next = r_planet + v_planet * dt + 0.5 * a_planet * dt^2;

    r_star = sqrt(r_next(1)^2 + r_next(2)^2);

    if ~isfinite(r_star) || r_star == 0
        error("Velocity Verlet reached an invalid position.");
    end

    % Calculate acceleration at the new position.
    a_next = -(mu_star / r_star^3) * r_next;

    % Update velocity using the average acceleration.
    v_next = v_planet + 0.5 * (a_planet + a_next) * dt;

    if any(~isfinite(r_next)) || any(~isfinite(v_next)) || any(~isfinite(a_next))
        error("Velocity Verlet produced invalid numerical values.");
    end

    % Update the state.
    r_planet = r_next;
    v_planet = v_next;
    a_planet = a_next;

    % Store the new state.
    planet_positions(i + 1, :) = r_planet;
    planet_velocities(i + 1, :) = v_planet;
    planet_accelerations(i + 1, :) = a_planet;

end

% Velocity Verlet variables
verlet_distance_history = sqrt(planet_positions(:, 1).^2 + planet_positions(:, 2).^2);
verlet_speed_history = sqrt(planet_velocities(:, 1).^2 + planet_velocities(:, 2).^2);
verlet_specific_energy_history = verlet_speed_history.^2 / 2 - mu_star ./ verlet_distance_history;
verlet_specific_angular_momentum_history = planet_positions(:, 1) .* planet_velocities(:, 2) - planet_positions(:, 2) .* planet_velocities(:, 1);
verlet_energy_error_history = verlet_specific_energy_history - verlet_specific_energy_history(1);
verlet_angular_momentum_error_history = verlet_specific_angular_momentum_history - verlet_specific_angular_momentum_history(1);
verlet_maximum_energy_error = max(abs(verlet_energy_error_history));
verlet_maximum_angular_momentum_error = max(abs(verlet_angular_momentum_error_history));
verlet_relative_energy_error_percent = 100 * verlet_maximum_energy_error / abs(verlet_specific_energy_history(1));
verlet_relative_angular_momentum_error_percent = 100 * verlet_maximum_angular_momentum_error / abs(verlet_specific_angular_momentum_history(1));

% Euler variables
distance_history = sqrt(euler_planet_positions(:, 1).^2 + euler_planet_positions(:, 2).^2);
speed_history = sqrt(euler_planet_velocities(:, 1).^2 + euler_planet_velocities(:, 2).^2);
euler_specific_energy_history = speed_history.^2 / 2 - mu_star ./ distance_history;
euler_specific_angular_momentum_history = euler_planet_positions(:, 1) .* euler_planet_velocities(:, 2) - euler_planet_positions(:, 2) .* euler_planet_velocities(:, 1);
euler_energy_error_history = euler_specific_energy_history - euler_specific_energy_history(1);
euler_angular_momentum_error_history = euler_specific_angular_momentum_history - euler_specific_angular_momentum_history(1);
euler_maximum_energy_error =  max(abs(euler_energy_error_history));
euler_maximum_angular_momentum_error = max(abs(euler_angular_momentum_error_history));
euler_relative_energy_error_percent = 100 * euler_maximum_energy_error / abs(euler_specific_energy_history(1));
euler_relative_angular_momentum_error_percent = 100 * euler_maximum_angular_momentum_error / abs(euler_specific_angular_momentum_history(1));

% Distance From The Analytical Ellipse
euler_ellipse_distance_history = zeros(num_steps + 1, 1);
verlet_ellipse_distance_history = zeros(num_steps + 1, 1);

for i = 1:num_steps + 1

    % Euler distance from the sampled analytical ellipse.
    dx = x_ellipse - euler_planet_positions(i, 1);
    dy = y_ellipse - euler_planet_positions(i, 2);

    distances_to_ellipse = sqrt(dx.^2 + dy.^2);

    euler_ellipse_distance_history(i) = min(distances_to_ellipse);

    % Velocity Verlet distance from the sampled analytical ellipse.
    dx = x_ellipse - planet_positions(i, 1);
    dy = y_ellipse - planet_positions(i, 2);

    distances_to_ellipse = sqrt(dx.^2 + dy.^2);

    verlet_ellipse_distance_history(i) = min(distances_to_ellipse);

end

maximum_ellipse_distance = max(euler_ellipse_distance_history);
mean_ellipse_distance = mean(euler_ellipse_distance_history);

verlet_maximum_ellipse_distance = max(verlet_ellipse_distance_history);
verlet_mean_ellipse_distance = mean(verlet_ellipse_distance_history);

% Velocity Verlet Minimun And Maximum Distance
% The primary orbital analysis uses Velocity Verlet.
[minimum_distance, minimum_index] = min(verlet_distance_history);
[maximum_distance, maximum_index] = max(verlet_distance_history);

minimum_distance_time = time_history(minimum_index);
maximum_distance_time = time_history(maximum_index);

minimum_distance_position = planet_positions(minimum_index, :);
maximum_distance_position = planet_positions(maximum_index, :);

minimum_distance_velocity = planet_velocities(minimum_index, :);
maximum_distance_velocity = planet_velocities(maximum_index, :);

perihelion_distance_error = minimum_distance - perihelion_distance;
aphelion_distance_error = maximum_distance - aphelion_distance;

perihelion_distance_error_percent = 100 * abs(perihelion_distance_error) / perihelion_distance;
aphelion_distance_error_percent = 100 * abs(aphelion_distance_error) / aphelion_distance;

% Direct Comparison Between The Methods
position_difference_x = euler_planet_positions(:, 1) - planet_positions(:, 1);
position_difference_y = euler_planet_positions(:, 2) - planet_positions(:, 2);
position_difference_history = sqrt( position_difference_x.^2 + position_difference_y.^2);
maximum_position_difference = max(position_difference_history);
mean_position_difference = mean(position_difference_history);
[~, maximum_difference_index] = max(position_difference_history);
maximum_difference_time = time_history(maximum_difference_index);

% Display Initial State
fprintf("\nInitial State (Velocity Verlet)\n");
fprintf("Position: [%.3e, %.3e] m\n", planet_positions(1, 1), planet_positions(1, 2));
fprintf("Velocity: [%.3f, %.3f] m/s\n", planet_velocities(1, 1), planet_velocities(1, 2));
fprintf("Speed: %.3f m/s\n", verlet_speed_history(1));
fprintf("Specific orbital energy: %.6e m^2/s^2\n", verlet_specific_energy_history(1));
fprintf("Specific angular momentum: %.6e m^2/s\n", verlet_specific_angular_momentum_history(1));

%Display Final State
fprintf("\nFinal State (Velocity Verlet)\n");
fprintf("Time: %.3f s\n", time_history(end));
fprintf("Position: [%.3e, %.3e] m\n", planet_positions(end, 1), planet_positions(end, 2));
fprintf("Velocity: [%.3f, %.3f] m/s\n", planet_velocities(end, 1), planet_velocities(end, 2));
fprintf("Acceleration: [%.3e, %.3e] m/s^2\n", planet_accelerations(end, 1), planet_accelerations(end, 2));
fprintf("Distance from star: %.3e m\n", verlet_distance_history(end));
fprintf("Speed: %.3f m/s\n", verlet_speed_history(end));
fprintf("Specific orbital energy: %.6e m^2/s^2\n", verlet_specific_energy_history(end));
fprintf("Specific angular momentum: %.6e m^2/s\n", verlet_specific_angular_momentum_history(end));

% Display Final Euler state
fprintf("\nFinal State (Semi- Implicit Eulet - Comparison)\n");
fprintf("Time: %.3f s\n", time_history(end));
fprintf("Position: [%.3e, %.3e] m\n", euler_planet_positions(end, 1), euler_planet_positions(end, 2));
fprintf("Velocity: [%.3f, %.3f] m/s\n", euler_planet_velocities(end, 1), euler_planet_velocities(end, 2));
fprintf("Acceleration: [%.3e, %.3e] m/s^2\n", euler_planet_accelerations(end, 1), euler_planet_accelerations(end, 2));
fprintf("Distance from star: %.3e m\n", distance_history(end));
fprintf("Speed: %.3f m/s\n", speed_history(end));

% Display Analytical Orbital Elements
fprintf("\nAnalytical Orbital Elements\n");
fprintf("Specific orbital energy: %.6e m^2/s^2\n", specific_energy);
fprintf("Specific angular momentum: %.6e m^2/s\n", specific_angular_momentum);
fprintf("Semimajor axis: %.6e m\n", semi_major_axis);
fprintf("Semiminor axis: %.6e m\n", semi_minor_axis);
fprintf("Eccentricity: %.8f\n", eccentricity);
fprintf("Perihelion direction: %.6f degrees\n", perihelion_angle * 180 / pi);

fprintf("\nAnalytical Ellipse Geometry\n");
fprintf("Focal distance: %.6e m\n", focal_distance);
fprintf("Analytical perihelion distance: %.6e m\n", perihelion_distance);
fprintf("Analytical aphelion distance: %.6e m\n", aphelion_distance);

% Display Velocity Verlet Orbital Extrema
fprintf("\nVelocity Verlet Numerical Minimum Distance\n");
fprintf("Distance: %.6e m\n", minimum_distance);
fprintf("Time: %.3f s\n", minimum_distance_time);
fprintf("Position: [%.6e, %.6e] m\n", minimum_distance_position(1), minimum_distance_position(2));
fprintf("Velocity: [%.6e, %.6e] m/s\n", minimum_distance_velocity(1), minimum_distance_velocity(2));

fprintf("\nVelocity Verlet Numerical Maximum Distance\n");
fprintf("Distance: %.6e m\n", maximum_distance);
fprintf("Time: %.3f s\n", maximum_distance_time);
fprintf("Position: [%.6e, %.6e] m\n", maximum_distance_position(1), maximum_distance_position(2));
fprintf("Velocity: [%.6e, %.6e] m/s\n", maximum_distance_velocity(1), maximum_distance_velocity(2));

fprintf("\nAnalytical vs Velocity Verlet Distance Extrema\n");
fprintf("Analytical perihelion: %.6e m\n", perihelion_distance);
fprintf("Observed minimum distance: %.6e m\n", minimum_distance);
fprintf("Minimum-distance difference: %+.6e m\n", perihelion_distance_error);
fprintf("Absolute perihelion difference: %.6f %%\n", perihelion_distance_error_percent);

fprintf("\nAnalytical aphelion: %.6e m\n", aphelion_distance);
fprintf("Observed maximum distance: %.6e m\n", maximum_distance);
fprintf("Maximum-distance difference: %+.6e m\n", aphelion_distance_error);
fprintf("Absolute aphelion difference: %.6f %%\n", aphelion_distance_error_percent);

% Display Conservation Errors
fprintf("\nMaximum Relative Energy Error\n");
fprintf("Semi-implicit Euler: %.6e %%\n", euler_relative_energy_error_percent);
fprintf("Velocity Verlet: %.6e %%\n", verlet_relative_energy_error_percent);

fprintf("\nMaximum Relative Angular Momentum Error\n");
fprintf("Semi-implicit Euler: %.6e %%\n", euler_relative_angular_momentum_error_percent);
fprintf("Velocity Verlet: %.6e %%\n", verlet_relative_angular_momentum_error_percent);

% Display Ellipse Distance Comparison
fprintf("\nTrajectory vs Analytical Ellipse\n");
fprintf("Euler maximum distance: %.6e m\n", maximum_ellipse_distance);
fprintf("Euler mean distance: %.6e m\n", mean_ellipse_distance);
fprintf("Velocity Verlet maximum distance: %.6e m\n", verlet_maximum_ellipse_distance);
fprintf("Velocity Verlet mean distance: %.6e m\n", verlet_mean_ellipse_distance);

fprintf("\nMaximum Position Difference Between Methods\n");
fprintf("Maximum difference: %.6e m\n", maximum_position_difference);
fprintf("Mean difference: %.6e m\n", mean_position_difference);
fprintf("Time of maximum difference: %.3f s\n", maximum_difference_time);

% Plot  Velocity Verlet Orbit
figure;
plot(planet_positions(:, 1), planet_positions(:, 2), 'b-', 'LineWidth', 1.5);
hold on;

plot(x_ellipse, y_ellipse, 'k--', 'LineWidth', 1.5);
plot(0, 0, 'yo', 'MarkerFaceColor', 'y', 'MarkerSize', 10);
plot(r0_planet(1), r0_planet(2), 'go', 'MarkerFaceColor', 'g');
plot(planet_positions(end, 1), planet_positions(end, 2), 'ro', 'MarkerFaceColor', 'r');

plot(minimum_distance_position(1), minimum_distance_position(2), 'mo', 'MarkerFaceColor', 'm');
plot(maximum_distance_position(1), maximum_distance_position(2), 'co', 'MarkerFaceColor', 'c');

axis equal;
grid on;
xlabel("X Position (m)");
ylabel("Y Position (m)");
title("Velocity Verlet Orbit vs Analytical Ellipse");
legend("Velocity Verlet Trajectory", "Analytical Ellipse", "Star (Focus)", "Starting Position", "Ending Position", "Observed Minimum Distance", "Observed Maximum Distance");

% Plot Euler vs Velocity Verlet Trajectorie
figure;
plot(euler_planet_positions(:, 1), euler_planet_positions(:, 2), 'b-', 'LineWidth', 1.3);
hold on;

plot(planet_positions(:, 1), planet_positions(:, 2), 'r-', 'LineWidth', 1.5);

plot(x_ellipse, y_ellipse, 'k--', 'LineWidth', 1.5);
plot(0, 0, 'yo', 'MarkerFaceColor', 'y', 'MarkerSize', 10);

axis equal;
grid on;
xlabel("X Position (m)");
ylabel("Y Position (m)");
title("Semi-Implicit Euler vs Velocity Verlet");
legend("Semi-Implicit Euler", "Velocity Verlet", "Analytical Ellipse", "Star (Focus)");

% Plot Velocity Verlet position componentsPLOT VELOCITY VERLET POSITION COMPONENTS
figure;

subplot(2, 1, 1);
plot(time_history, planet_positions(:, 1), 'b-', 'LineWidth', 1.5);
xlabel("Time (s)");
ylabel("X-position (m)");
title("Velocity Verlet X-position vs Time");
grid on;

subplot(2, 1, 2);
plot(time_history, planet_positions(:, 2), 'r-', 'LineWidth', 1.5);
xlabel("Time (s)");
ylabel("Y-position (m)");
title("Velocity Verlet Y-position vs Time");
grid on;

% Plot distance comparison
figure;
plot(time_history, distance_history, 'b-', 'LineWidth', 1.3);
hold on;
plot(time_history, verlet_distance_history, 'r--', 'LineWidth', 1.5);

grid on;
xlabel("Time (s)");
ylabel("Distance from Star (m)");
title("Euler vs Velocity Verlet: Distance Over Time");
legend("Semi-Implicit Euler", "Velocity Verlet");

% Plot speed comparison
figure;
plot(time_history, speed_history, 'b-', 'LineWidth', 1.3);
hold on;
plot(time_history, verlet_speed_history, 'r--', 'LineWidth', 1.5);
grid on;
xlabel("Time (s)");
ylabel("Speed (m/s)");
title("Euler vs Velocity Verlet: Speed Over Time");
legend("Semi-Implicit Euler", "Velocity Verlet");

% Plot relative energy error
figure;
plot(time_history, 100 * abs(euler_energy_error_history) / abs(euler_specific_energy_history(1)), 'b-', 'LineWidth', 1.3);
hold on;
plot(time_history, 100 * abs(verlet_energy_error_history) / abs(verlet_specific_energy_history(1)), 'r--', 'LineWidth', 1.5);
grid on;
xlabel("Time (s)");
ylabel("Absolute Relative Energy Error (%)");
title("Euler vs Velocity Verlet: Energy Conservation");
legend("Semi-Implicit Euler", "Velocity Verlet");

% Plot relative angular momentum error
figure;
plot(time_history, 100 * abs(euler_angular_momentum_error_history) / abs(euler_specific_angular_momentum_history(1)), 'b-', 'LineWidth', 1.3);
hold on;
plot(time_history, 100 * abs(verlet_angular_momentum_error_history) / abs(verlet_specific_angular_momentum_history(1)), 'r--', 'LineWidth', 1.5);
grid on;
xlabel("Time (s)");
ylabel("Absolute Relative Angular Momentum Error (%)");
title("Euler vs Velocity Verlet: Angular Momentum Conservation");
legend("Semi-Implicit Euler", "Velocity Verlet");

% Plot distance from analytical ellipse
figure;
plot(time_history, euler_ellipse_distance_history, 'b-', 'LineWidth', 1.3);
hold on;
plot(time_history, verlet_ellipse_distance_history, 'r--', 'LineWidth', 1.5);
grid on;
xlabel("Time (s)");
ylabel("Distance from Analytical Ellipse (m)");
title("Euler vs Velocity Verlet: Trajectory Error");
legend("Semi-Implicit Euler", "Velocity Verlet");

% Plot position difference between methods
figure;
plot(time_history, position_difference_history, 'm-', 'LineWidth', 1.5);
hold on;

plot(maximum_difference_time, maximum_position_difference, 'ro', 'MarkerFaceColor', 'r');

grid on;
xlabel("Time (s)");
ylabel("Position Difference (m)");
title("Position Difference: Euler vs Velocity Verlet");
legend("Position Difference", "Maximum Difference");

% Display analytical orbital parameters
figure;
axis off;

text(0.1, 0.9, "Analytical Orbital Parameters", "FontSize", 14, "FontWeight", "bold");
text(0.1, 0.78, sprintf("Eccentricity: %.6f", eccentricity));
text(0.1, 0.68, sprintf("Semi-major axis: %.3e m", semi_major_axis));
text(0.1, 0.58, sprintf("Semi-minor axis: %.3e m", semi_minor_axis));
text(0.1, 0.48, sprintf("Focal distance: %.3e m", focal_distance));
text(0.1, 0.38, sprintf("Analytical perihelion: %.3e m", perihelion_distance));
text(0.1, 0.28, sprintf("Analytical aphelion: %.3e m", aphelion_distance));
text(0.1, 0.18, sprintf("Verlet mean ellipse distance: %.3e m", verlet_mean_ellipse_distance));
text(0.1, 0.08, sprintf("Verlet maximum ellipse distance: %.3e m", verlet_maximum_ellipse_distance));
