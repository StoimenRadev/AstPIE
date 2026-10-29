clear;
clc;
close all;

star_mass = input("Enter the mass of the star (kg): ");
G = 6.67430e-11;

if ~isnumeric(star_mass) || ~isreal(star_mass) || ...
   ~isscalar(star_mass) || ~isfinite(star_mass) || star_mass <= 0
    error("Star mass must be a finite, positive scalar.");
end

mu_star = G * star_mass;

% Initial position and velocity
r0_planet = input("Enter the initial position [rx ry] (m): ");
v0_planet = input("Enter the initial velocity [vx vy] (m/s): ");

if ~isnumeric(r0_planet) || ~isreal(r0_planet) || ~isvector(r0_planet) || numel(r0_planet) ~= 2 || any(~isfinite(r0_planet(:)))
    error("Initial position must be a finite, real, two-component vector.");
end

if ~isnumeric(v0_planet) || ~isreal(v0_planet) || ~isvector(v0_planet) || numel(v0_planet) ~= 2 || any(~isfinite(v0_planet(:)))
    error("Initial velocity must be a finite, real, two-component vector.");
end

r0_planet = r0_planet(:).';
v0_planet = v0_planet(:).';

r0_distance = sqrt(r0_planet(1)^2 + r0_planet(2)^2);

if ~isfinite(r0_distance) || r0_distance == 0
    error("Initial position must be a finite, nonzero distance from the star.");
end

v0_speed = sqrt(v0_planet(1)^2 + v0_planet(2)^2);

h0 = r0_planet(1) * v0_planet(2) - r0_planet(2) * v0_planet(1);

if ~isfinite(h0) || h0 == 0
    error("Initial specific angular momentum must be finite and nonzero.");
end

% Analytical Orbital Elements
specific_energy = (v0_speed^2 / 2) - (mu_star / r0_distance);

if specific_energy >= 0
    error("The initial conditions do not describe a bound elliptical orbit.");
end

semimajor_axis = -mu_star / (2 * specific_energy);

eccentricity_squared = 1 + (2 * specific_energy * h0^2) / mu_star^2;

if abs(eccentricity_squared) < 1e-12
    eccentricity_squared = 0;
end

if eccentricity_squared < 0 && eccentricity_squared > -1e-12
    eccentricity_squared = 0;
end

if eccentricity_squared < 0 || eccentricity_squared >= 1
    error("The initial conditions do not describe a valid elliptical orbit.");
end

eccentricity = sqrt(eccentricity_squared);

semiminor_axis = semimajor_axis * sqrt(1 - eccentricity^2);

focal_distance = semimajor_axis * eccentricity;

perihelion_distance = semimajor_axis * (1 - eccentricity);
aphelion_distance = semimajor_axis * (1 + eccentricity);

eccentricity_vector = (((v0_speed^2 - mu_star / r0_distance) * r0_planet) ...
    - (r0_planet(1) * v0_planet(1) + r0_planet(2) * v0_planet(2)) * v0_planet) / mu_star;

if eccentricity > 1e-12
    perihelion_angle = atan2(eccentricity_vector(2), eccentricity_vector(1));
else
    perihelion_angle = atan2(r0_planet(2), r0_planet(1));
end

% Generate analytical ellipse
ellipse_angle = linspace(0, 2 * pi, 1000);

ellipse_x_local = semimajor_axis * cos(ellipse_angle);
ellipse_y_local = semiminor_axis * sin(ellipse_angle);

ellipse_x_rotated = ellipse_x_local * cos(perihelion_angle) - ellipse_y_local * sin(perihelion_angle);
ellipse_y_rotated = ellipse_x_local * sin(perihelion_angle) + ellipse_y_local * cos(perihelion_angle);

ellipse_x = ellipse_x_rotated - focal_distance * cos(perihelion_angle);
ellipse_y = ellipse_y_rotated - focal_distance * sin(perihelion_angle);

% Timestep and simulation duration
dt = input("Enter the time step (s): ");
T = input("Enter the total simulation time (s): ");

if ~isnumeric(dt) || ~isreal(dt) || ~isscalar(dt) || ~isfinite(dt) || dt <= 0
    error("Time step must be a finite, positive scalar.");
end

if ~isnumeric(T) || ~isreal(T) || ~isscalar(T) || ~isfinite(T) || T <= 0
    error("Simulation duration must be a finite, positive scalar.");
end

step_ratio = T / dt;

if ~isfinite(step_ratio) || step_ratio < 1
    error("Simulation duration must allow at least one integration step.");
end

num_steps = round(step_ratio);

if abs(step_ratio - num_steps) > 1e-10 * max(1, abs(step_ratio))
    error("Simulation duration must be an integer multiple of the timestep.");
end

actual_duration = num_steps * dt;

% Measurement interval
measurement_interval = input("Enter the measurement interval (s): ");

if ~isnumeric(measurement_interval) || ~isreal(measurement_interval) || ~isscalar(measurement_interval) || ~isfinite(measurement_interval) || measurement_interval <= 0
    error("Measurement interval must be a finite, positive scalar.");
end

interval_step_ratio = measurement_interval / dt;

if ~isfinite(interval_step_ratio) || interval_step_ratio < 1 || abs(interval_step_ratio - round(interval_step_ratio)) > 1e-10 * max(1, abs(interval_step_ratio))
    error("Measurement interval must be an integer multiple of the timestep.");
end

interval_steps = round(interval_step_ratio);

% Create time history
time_history = (0:num_steps)' * dt;

% Analytical reference orbit
mean_motion = sqrt(mu_star / semimajor_axis^3);

if eccentricity > 1e-12
    cos_E0 = (1 - r0_distance / semimajor_axis) / eccentricity;
    sin_E0 = (r0_planet(1) * v0_planet(1) + r0_planet(2) * v0_planet(2)) ...
        / (eccentricity * sqrt(mu_star * semimajor_axis));

    cos_E0 = max(-1, min(1, cos_E0));
    sin_E0 = max(-1, min(1, sin_E0));

    E0 = atan2(sin_E0, cos_E0);
    M0 = E0 - eccentricity * sin(E0);
end

analytical_positions = zeros(length(time_history), 2);

if eccentricity > 1e-12
    for i = 1:length(time_history)

        M = mod(M0 + mean_motion * time_history(i), 2 * pi);

        E_low = 0;
        E_high = 2 * pi;

        for j = 1:60
            E_mid = (E_low + E_high) / 2;
            M_mid = E_mid - eccentricity * sin(E_mid);

            if M_mid < M
                E_low = E_mid;
            else
                E_high = E_mid;
            end
        end

        E = (E_low + E_high) / 2;

        x_local = semimajor_axis * (cos(E) - eccentricity);
        y_local = semimajor_axis * sqrt(1 - eccentricity^2) * sin(E);

        analytical_positions(i, 1) = x_local * cos(perihelion_angle) - y_local * sin(perihelion_angle);
        analytical_positions(i, 2) = x_local * sin(perihelion_angle) + y_local * cos(perihelion_angle);
    end
else
    initial_angle = atan2(r0_planet(2), r0_planet(1));
    rotation_direction = sign(h0);

    for i = 1:length(time_history)
        angle = initial_angle + rotation_direction * mean_motion * time_history(i);

        analytical_positions(i, 1) = r0_distance * cos(angle);
        analytical_positions(i, 2) = r0_distance * sin(angle);
    end
end

% Integrate the orbit using Velocity Verlet
r_planet = r0_planet;
v_planet = v0_planet;

planet_positions = zeros(num_steps + 1, 2);
planet_positions(1, :) = r_planet;

planet_velocities = zeros(num_steps + 1, 2);
planet_velocities(1, :) = v_planet;

planet_accelerations = zeros(num_steps + 1, 2);
planet_accelerations(1, :) = -(mu_star / r0_distance^3) * r0_planet;

if any(~isfinite(planet_accelerations(1, :)))
    error("Initial acceleration contains invalid numerical values.");
end

for i = 1:num_steps

    a_current = planet_accelerations(i, :);
    r_planet = planet_positions(i, :) + planet_velocities(i, :) * dt + 0.5 * a_current * dt^2;
    r_new_distance = sqrt(r_planet(1)^2 + r_planet(2)^2);

    if ~isfinite(r_new_distance) || r_new_distance == 0
        error("Velocity Verlet produced an invalid position.");
    end

    a_new = -(mu_star / r_new_distance^3) * r_planet;

    if any(~isfinite(a_new))
        error("Velocity Verlet produced invalid acceleration values.");
    end

    v_planet = planet_velocities(i, :) + 0.5 * (a_current + a_new) * dt;

    if any(~isfinite(r_planet)) || any(~isfinite(v_planet))
        error("Velocity Verlet produced invalid position or velocity values.");
    end

    planet_positions(i + 1, :) = r_planet;
    planet_velocities(i + 1, :) = v_planet;
    planet_accelerations(i + 1, :) = a_new;

end

% Integrate the orbit using semi-implicit Euler
euler_r_planet = r0_planet;
euler_v_planet = v0_planet;

euler_planet_positions = zeros(num_steps + 1, 2);
euler_planet_positions(1, :) = euler_r_planet;

euler_planet_velocities = zeros(num_steps + 1, 2);
euler_planet_velocities(1, :) = euler_v_planet;

euler_planet_accelerations = zeros(num_steps + 1, 2);
euler_planet_accelerations(1, :) = -(mu_star / r0_distance^3) * r0_planet;

for i = 1:num_steps

    euler_r_distance = sqrt(euler_r_planet(1)^2 + euler_r_planet(2)^2);

    if ~isfinite(euler_r_distance) || euler_r_distance == 0
        error("Euler produced an invalid position.");
    end

    euler_a_planet = -(mu_star / euler_r_distance^3) * euler_r_planet;

    if any(~isfinite(euler_a_planet))
        error("Euler produced invalid acceleration values.");
    end

    euler_v_planet = euler_v_planet + euler_a_planet * dt;
    euler_r_planet = euler_r_planet + euler_v_planet * dt;

    if any(~isfinite(euler_r_planet)) || any(~isfinite(euler_v_planet))
        error("Euler produced invalid position or velocity values.");
    end

    euler_r_new_distance = sqrt(euler_r_planet(1)^2 + euler_r_planet(2)^2);

    if ~isfinite(euler_r_new_distance) || euler_r_new_distance == 0
        error("Euler produced an invalid position.");
    end

    euler_a_new = -(mu_star / euler_r_new_distance^3) * euler_r_planet;

    if any(~isfinite(euler_a_new))
        error("Euler produced invalid acceleration values.");
    end

    euler_planet_positions(i + 1, :) = euler_r_planet;
    euler_planet_velocities(i + 1, :) = euler_v_planet;
    euler_planet_accelerations(i + 1, :) = euler_a_new;

end

% Default-method measurements: Velocity Verlet
planet_distance = sqrt(planet_positions(:, 1).^2 + planet_positions(:, 2).^2);
planet_speed = sqrt(planet_velocities(:, 1).^2 + planet_velocities(:, 2).^2);
specific_angular_momentum = planet_positions(:, 1) .* planet_velocities(:, 2) - planet_positions(:, 2) .* planet_velocities(:, 1);
areal_velocity = specific_angular_momentum / 2;
swept_area_steps = 0.5 * abs(planet_positions(1:end-1, 1) .* planet_positions(2:end, 2) - planet_positions(1:end-1, 2) .* planet_positions(2:end, 1));
swept_area_history = [0; swept_area_steps];
cumulative_swept_area = [0; cumsum(swept_area_steps)];

% Equal-time interval measurements
num_intervals = ceil(num_steps / interval_steps);
interval_start_time = zeros(num_intervals, 1);
interval_end_time = zeros(num_intervals, 1);
interval_duration = zeros(num_intervals, 1);
interval_swept_area = zeros(num_intervals, 1);
interval_average_areal_velocity = zeros(num_intervals, 1);

for i = 1:num_intervals

    first_step = (i - 1) * interval_steps + 1;
    last_step = min(i * interval_steps, num_steps);
    interval_start_time(i) = time_history(first_step);
    interval_end_time(i) = time_history(last_step + 1);
    interval_duration(i) = interval_end_time(i) - interval_start_time(i);
    interval_swept_area(i) = sum(swept_area_steps(first_step:last_step));
    interval_average_areal_velocity(i) = interval_swept_area(i) / interval_duration(i);

end

% Velocity Verlet position errors

position_error = zeros(length(time_history), 1);

for i = 1:length(time_history)
    error_x = planet_positions(i, 1) - analytical_positions(i, 1);
    error_y = planet_positions(i, 2) - analytical_positions(i, 2);
    position_error(i) = sqrt(error_x^2 + error_y^2);
end

initial_position_error = position_error(1);
maximum_position_error = max(position_error);
final_position_error = position_error(end);

% Euler measurements
euler_planet_distance = sqrt(euler_planet_positions(:, 1).^2 + euler_planet_positions(:, 2).^2);
euler_planet_speed = sqrt(euler_planet_velocities(:, 1).^2 + euler_planet_velocities(:, 2).^2);
euler_specific_angular_momentum = euler_planet_positions(:, 1) .* euler_planet_velocities(:, 2) - euler_planet_positions(:, 2) .* euler_planet_velocities(:, 1);
euler_areal_velocity = euler_specific_angular_momentum / 2;
euler_position_error = zeros(length(time_history), 1);

for i = 1:length(time_history)
    euler_error_x = euler_planet_positions(i, 1) - analytical_positions(i, 1);
    euler_error_y = euler_planet_positions(i, 2) - analytical_positions(i, 2);
    euler_position_error(i) = sqrt(euler_error_x^2 + euler_error_y^2);
end

euler_initial_position_error = euler_position_error(1);
euler_maximum_position_error = max(euler_position_error);
euler_final_position_error = euler_position_error(end);

% Numerical perihelion and aphelion distances
numerical_perihelion_distance = min(planet_distance);
numerical_aphelion_distance = max(planet_distance);

perihelion_distance_error = abs(numerical_perihelion_distance - perihelion_distance);
aphelion_distance_error = abs(numerical_aphelion_distance - aphelion_distance);

distance_tolerance = 1e-6 * semimajor_axis;

perihelion_captured = perihelion_distance_error <= distance_tolerance;
aphelion_captured = aphelion_distance_error <= distance_tolerance;

% Display simulation setup
fprintf('\nSimulation Setup\n');
fprintf('Default integration method: Velocity Verlet\n');
fprintf('Comparison integration method: Semi-implicit Euler\n');
fprintf('Star mass: %.6e kg\n', star_mass);
fprintf('Gravitational parameter: %.6e m^3/s^2\n', mu_star);
fprintf('Time step: %.6f s\n', dt);
fprintf('Total simulation time: %.6f s\n', actual_duration);
fprintf('Number of integration steps: %d\n', num_steps);
fprintf('Number of recorded states: %d\n', num_steps + 1);

% Display initial conditions
fprintf('\nInitial Conditions\n');
fprintf('Initial position: [%.6e, %.6e] m\n', r0_planet(1), r0_planet(2));
fprintf('Initial velocity: [%.6e, %.6e] m/s\n', v0_planet(1), v0_planet(2));
fprintf('Initial speed: %.6e m/s\n', planet_speed(1));
fprintf('Initial distance: %.6e m\n', planet_distance(1));
fprintf('Initial acceleration: [%.6e, %.6e] m/s^2\n', planet_accelerations(1, 1), planet_accelerations(1, 2));
fprintf('Initial specific angular momentum: %.6e m^2/s\n', h0);
fprintf('Initial areal velocity: %.6e m^2/s\n', areal_velocity(1));

% Display final results
fprintf('\nFinal Results - Velocity Verlet\n');
fprintf('Final time: %.6e s\n', time_history(end));
fprintf('Final position: [%.6e, %.6e] m\n', planet_positions(end, 1), planet_positions(end, 2));
fprintf('Final velocity: [%.6e, %.6e] m/s\n', planet_velocities(end, 1), planet_velocities(end, 2));
fprintf('Final speed: %.6e m/s\n', planet_speed(end));
fprintf('Final distance: %.6e m\n', planet_distance(end));
fprintf('Final acceleration: [%.6e, %.6e] m/s^2\n', planet_accelerations(end, 1), planet_accelerations(end, 2));
fprintf('Final specific angular momentum: %.6e m^2/s\n', specific_angular_momentum(end));
fprintf('Final areal velocity: %.6e m^2/s\n', areal_velocity(end));

fprintf('\nFinal Results - Semi-implicit Euler\n');
fprintf('Final position: [%.6e, %.6e] m\n', euler_planet_positions(end, 1), euler_planet_positions(end, 2));
fprintf('Final velocity: [%.6e, %.6e] m/s\n', euler_planet_velocities(end, 1), euler_planet_velocities(end, 2));
fprintf('Final speed: %.6e m/s\n', euler_planet_speed(end));
fprintf('Final distance: %.6e m\n', euler_planet_distance(end));
fprintf('Final specific angular momentum: %.6e m^2/s\n', euler_specific_angular_momentum(end));
fprintf('Final areal velocity: %.6e m^2/s\n', euler_areal_velocity(end));

% Display Analytical Orbital Elements
fprintf('\nAnalytical Orbital Elements\n');
fprintf('Specific orbital energy: %.6e J/kg\n', specific_energy);
fprintf('Specific angular momentum: %.6e m^2/s\n', h0);
fprintf('Eccentricity: %.8f\n', eccentricity);
fprintf('Semi-major axis: %.6e m\n', semimajor_axis);
fprintf('Semi-minor axis: %.6e m\n', semiminor_axis);
fprintf('Focal distance: %.6e m\n', focal_distance);
fprintf('Perihelion distance: %.6e m\n', perihelion_distance);
fprintf('Aphelion distance: %.6e m\n', aphelion_distance);

% Display position error results
fprintf('\nNumerical Position Error - Velocity Verlet\n');
fprintf('Initial position error: %.6e m\n', initial_position_error);
fprintf('Final position error: %.6e m\n', final_position_error);
fprintf('Maximum position error: %.6e m\n', maximum_position_error);

fprintf('\nNumerical Position Error - Semi-implicit Euler\n');
fprintf('Initial position error: %.6e m\n', euler_initial_position_error);
fprintf('Final position error: %.6e m\n', euler_final_position_error);
fprintf('Maximum position error: %.6e m\n', euler_maximum_position_error);

% Display orbital distance validation
fprintf('\nOrbital Distance Validation - Velocity Verlet\n');
fprintf('Analytical perihelion distance: %.6e m\n', perihelion_distance);
fprintf('Numerical minimum distance: %.6e m\n', numerical_perihelion_distance);
fprintf('Perihelion distance error: %.6e m\n', perihelion_distance_error);

if perihelion_captured
    fprintf('Perihelion: captured within the selected tolerance.\n');
else
    fprintf('Perihelion: not captured within the selected tolerance.\n');
end

fprintf('\nAnalytical aphelion distance: %.6e m\n', aphelion_distance);
fprintf('Numerical maximum distance: %.6e m\n', numerical_aphelion_distance);
fprintf('Aphelion distance error: %.6e m\n', aphelion_distance_error);

if aphelion_captured
    fprintf('Aphelion: captured within the selected tolerance.\n');
else
    fprintf('Aphelion: not captured within the selected tolerance.\n');
end

% Relative changes in angular momentum and areal velocity
h_change = abs(specific_angular_momentum(end) - specific_angular_momentum(1)) / abs(specific_angular_momentum(1)) * 100;
areal_velocity_change = abs(areal_velocity(end) - areal_velocity(1)) / abs(areal_velocity(1)) * 100;
euler_h_change = abs(euler_specific_angular_momentum(end) - euler_specific_angular_momentum(1)) / abs(euler_specific_angular_momentum(1)) * 100;
euler_areal_velocity_change = abs(euler_areal_velocity(end) - euler_areal_velocity(1)) / abs(euler_areal_velocity(1)) * 100;

fprintf('\nConservation Comparison\n');
fprintf('Velocity Verlet angular momentum change: %.6e %%\n', h_change);
fprintf('Velocity Verlet areal velocity change: %.6e %%\n', areal_velocity_change);
fprintf('Euler angular momentum change: %.6e %%\n', euler_h_change);
fprintf('Euler areal velocity change: %.6e %%\n', euler_areal_velocity_change);

% Display complete time history
fprintf('\nComplete Time History - Velocity Verlet\n');

results = [ ...
    time_history, ...
    planet_positions, ...
    planet_velocities, ...
    planet_speed, ...
    planet_accelerations, ...
    planet_distance, ...
    specific_angular_momentum, ...
    areal_velocity, ...
    swept_area_history, ...
    cumulative_swept_area];

disp(['Columns: time(s), x(m), y(m), vx(m/s), vy(m/s), ', 'speed(m/s), ax(m/s^2), ay(m/s^2), distance(m), ', 'specific_angular_momentum(m^2/s), areal_velocity(m^2/s), ', 'swept_area_this_step(m^2), cumulative_swept_area(m^2)']);
disp(results);

fprintf('\nEqual-Time Interval Comparison - Velocity Verlet\n');

interval_results = [ ...
    interval_start_time, ...
    interval_end_time, ...
    interval_duration, ...
    interval_swept_area, ...
    interval_average_areal_velocity];

disp(['Columns: start_time(s), end_time(s), duration(s), ', 'swept_area(m^2), average_areal_velocity(m^2/s)']);
disp(interval_results);

% Plot the numerical trajectory and analytical ellipse
figure;
plot(planet_positions(:, 1), planet_positions(:, 2), 'b-');
hold on;
plot(euler_planet_positions(:, 1), euler_planet_positions(:, 2), 'm-');
plot(ellipse_x, ellipse_y, 'r--');
plot(0, 0, 'yo', 'MarkerFaceColor', 'y', 'MarkerSize', 12);

ellipse_center_x = -focal_distance * cos(perihelion_angle);
ellipse_center_y = -focal_distance * sin(perihelion_angle);

plot(ellipse_center_x, ellipse_center_y, 'kx', 'MarkerSize', 10, 'LineWidth', 2);

plot([ellipse_center_x - semimajor_axis * cos(perihelion_angle), ...
      ellipse_center_x + semimajor_axis * cos(perihelion_angle)], ...
     [ellipse_center_y - semimajor_axis * sin(perihelion_angle), ...
      ellipse_center_y + semimajor_axis * sin(perihelion_angle)], ...
     'k:', 'LineWidth', 1.2);

plot([ellipse_center_x - semiminor_axis * sin(perihelion_angle), ...
      ellipse_center_x + semiminor_axis * sin(perihelion_angle)], ...
     [ellipse_center_y + semiminor_axis * cos(perihelion_angle), ...
      ellipse_center_y - semiminor_axis * cos(perihelion_angle)], ...
     'g:', 'LineWidth', 1.2);

xlabel('X Position (m)');
ylabel('Y Position (m)');
title('Kepler''s Second Law - Numerical and Analytical Orbits');
legend('Velocity Verlet', 'Semi-implicit Euler', 'Analytical Ellipse', 'Star (Focus)', 'Ellipse Centre', 'Major Axis', 'Minor Axis');
axis equal;
grid on;

% Plot star-planet distance against time
figure;
time_days = time_history / 86400;
planet_distance_AU = planet_distance / 1.495978707e11;
euler_planet_distance_AU = euler_planet_distance / 1.495978707e11;
plot(time_days, planet_distance_AU, 'b-', 'LineWidth', 1.2);
hold on;
plot(time_days, euler_planet_distance_AU, 'm-', 'LineWidth', 1.2);
xlabel('Time (days)');
ylabel('Star-Planet Distance (AU)');
title('Star-Planet Distance vs Time');
legend('Velocity Verlet', 'Semi-implicit Euler');
grid on;

% Plot speed against time
figure;
plot(time_days, planet_speed / 1000, 'b-', 'LineWidth', 1.2);
hold on;
plot(time_days, euler_planet_speed / 1000, 'm-', 'LineWidth', 1.2);
xlabel('Time (days)');
ylabel('Speed (km/s)');
title('Orbital Speed vs Time');
legend('Velocity Verlet', 'Semi-implicit Euler');
grid on;

% Plot numerical position error against time
figure;
plot(time_days, position_error / 1000, 'b-', 'LineWidth', 1.2);
hold on;
plot(time_days, euler_position_error / 1000, 'm-', 'LineWidth', 1.2);
xlabel('Time (days)');
ylabel('Position Error (km)');
title('Numerical Position Error vs Time');
legend('Velocity Verlet', 'Semi-implicit Euler');
grid on;

% Plot x and y coordinates against time
figure;
subplot(2, 1, 1);
plot(time_days, planet_positions(:, 1) / 1.495978707e11, 'b-', 'LineWidth', 1.2);
hold on;
plot(time_days, euler_planet_positions(:, 1) / 1.495978707e11, 'm-', 'LineWidth', 1.2);
xlabel('Time (days)');
ylabel('X Position (AU)');
title('X Position vs Time');
legend('Velocity Verlet', 'Semi-implicit Euler');
grid on;
subplot(2, 1, 2);
plot(time_days, planet_positions(:, 2) / 1.495978707e11, 'b-', 'LineWidth', 1.2);
hold on;
plot(time_days, euler_planet_positions(:, 2) / 1.495978707e11, 'm-', 'LineWidth', 1.2);
xlabel('Time (days)');
ylabel('Y Position (AU)');
title('Y Position vs Time');
legend('Velocity Verlet', 'Semi-implicit Euler');
grid on;

% Display main orbital parameters
figure;
axis off;
text(0.05, 0.90, 'Main Orbital Parameters', 'FontSize', 16, 'FontWeight', 'bold');
text(0.05, 0.76, sprintf('Semimajor axis: %.6e m', semimajor_axis), 'FontSize', 11);
text(0.05, 0.65, sprintf('Semiminor axis: %.6e m', semiminor_axis), 'FontSize', 11);
text(0.05, 0.54, sprintf('Eccentricity: %.6f', eccentricity), 'FontSize', 11);
text(0.05, 0.43, sprintf('Focal distance: %.6e m', focal_distance), 'FontSize', 11);
text(0.05, 0.32, sprintf('Perihelion distance: %.6e m', perihelion_distance), 'FontSize', 11);
text(0.05, 0.21, sprintf('Aphelion distance: %.6e m', aphelion_distance), 'FontSize', 11);
text(0.05, 0.10, sprintf('Gravitational parameter: %.6e m^3/s^2', mu_star), 'FontSize', 11);
title('Orbital Parameter Summary');

% Timestep comparison for both methods
comparison_timesteps = [2 * dt, dt, dt / 2, dt / 4];
comparison_max_steps = round(T ./ comparison_timesteps);
valid_comparison_timesteps = abs(T ./ comparison_timesteps - comparison_max_steps) <= 1e-10 .* max(1, abs(T ./ comparison_timesteps));
comparison_timesteps = comparison_timesteps(valid_comparison_timesteps);
verlet_comparison_max_errors = zeros(length(comparison_timesteps), 1);
verlet_comparison_final_errors = zeros(length(comparison_timesteps), 1);
euler_comparison_max_errors = zeros(length(comparison_timesteps), 1);
euler_comparison_final_errors = zeros(length(comparison_timesteps), 1);
fprintf('\nTimestep Comparison\n');

for k = 1:length(comparison_timesteps)

    comparison_dt = comparison_timesteps(k);
    comparison_ratio = T / comparison_dt;

    if abs(comparison_ratio - round(comparison_ratio)) > 1e-10 * max(1, abs(comparison_ratio))
        fprintf('Skipping timestep %.0f s: duration is not an integer multiple.\n', comparison_dt);
        continue;
    end

    comparison_steps = round(comparison_ratio);

    verlet_comparison_r = r0_planet;
    verlet_comparison_v = v0_planet;
    verlet_comparison_a = -(mu_star / r0_distance^3) * r0_planet;

    euler_comparison_r = r0_planet;
    euler_comparison_v = v0_planet;

    verlet_comparison_max_error = 0;
    verlet_comparison_final_error = 0;
    euler_comparison_max_error = 0;
    euler_comparison_final_error = 0;

    for i = 1:comparison_steps

        % Velocity Verlet
        verlet_comparison_r = verlet_comparison_r + verlet_comparison_v * comparison_dt + 0.5 * verlet_comparison_a * comparison_dt^2;

        verlet_comparison_distance = sqrt( verlet_comparison_r(1)^2 + verlet_comparison_r(2)^2);

        if ~isfinite(verlet_comparison_distance) || verlet_comparison_distance == 0
            error('Velocity Verlet failed during timestep comparison.');
        end

        verlet_comparison_a_new = -(mu_star / verlet_comparison_distance^3) * verlet_comparison_r;

        verlet_comparison_v = verlet_comparison_v + 0.5 * (verlet_comparison_a + verlet_comparison_a_new) * comparison_dt;

        verlet_comparison_a = verlet_comparison_a_new;

        % Semi-implicit Euler
        euler_comparison_distance = sqrt( euler_comparison_r(1)^2 + euler_comparison_r(2)^2);

        if ~isfinite(euler_comparison_distance) || euler_comparison_distance == 0
            error('Euler failed during timestep comparison.');
        end

        euler_comparison_a = -(mu_star / euler_comparison_distance^3) * euler_comparison_r;
        euler_comparison_v = euler_comparison_v + euler_comparison_a * comparison_dt;
        euler_comparison_r = euler_comparison_r + euler_comparison_v * comparison_dt;

        % Analytical reference position
        comparison_time = i * comparison_dt;

        if eccentricity > 1e-12

            M = mod(M0 + mean_motion * comparison_time, 2 * pi);

            E_low = 0;
            E_high = 2 * pi;

            for j = 1:60
                E_mid = (E_low + E_high) / 2;
                M_mid = E_mid - eccentricity * sin(E_mid);

                if M_mid < M
                    E_low = E_mid;
                else
                    E_high = E_mid;
                end
            end

            E = (E_low + E_high) / 2;

            x_local = semimajor_axis * (cos(E) - eccentricity);
            y_local = semimajor_axis * sqrt(1 - eccentricity^2) * sin(E);

            analytical_x = x_local * cos(perihelion_angle) - y_local * sin(perihelion_angle);
            analytical_y = x_local * sin(perihelion_angle) + y_local * cos(perihelion_angle);

        else

            initial_angle = atan2(r0_planet(2), r0_planet(1));
            rotation_direction = sign(h0);
            angle = initial_angle + rotation_direction * mean_motion * comparison_time;

            analytical_x = r0_distance * cos(angle);
            analytical_y = r0_distance * sin(angle);

        end

        verlet_comparison_error = sqrt((verlet_comparison_r(1) - analytical_x)^2 + (verlet_comparison_r(2) - analytical_y)^2);
        euler_comparison_error = sqrt((euler_comparison_r(1) - analytical_x)^2 + (euler_comparison_r(2) - analytical_y)^2);
        verlet_comparison_max_error = max(verlet_comparison_max_error, verlet_comparison_error);
        euler_comparison_max_error = max(euler_comparison_max_error, euler_comparison_error);
        verlet_comparison_final_error = verlet_comparison_error;
        euler_comparison_final_error = euler_comparison_error;

    end

    verlet_comparison_max_errors(k) = verlet_comparison_max_error;
    verlet_comparison_final_errors(k) = verlet_comparison_final_error;
    euler_comparison_max_errors(k) = euler_comparison_max_error;
    euler_comparison_final_errors(k) = euler_comparison_final_error;
    fprintf(['Timestep: %.0f s | Verlet max: %.6e m | Verlet final: %.6e m', ' | Euler max: %.6e m | Euler final: %.6e m\n'], comparison_dt, verlet_comparison_max_error, verlet_comparison_final_error, euler_comparison_max_error, euler_comparison_final_error);

end

% Plot timestep comparison
figure;
plot(comparison_timesteps, verlet_comparison_max_errors / 1000, 'bo-', 'LineWidth', 1.2, 'MarkerFaceColor', 'b');
hold on;
plot(comparison_timesteps, verlet_comparison_final_errors / 1000, 'bs-', 'LineWidth', 1.2, 'MarkerFaceColor', 'b');
plot(comparison_timesteps, euler_comparison_max_errors / 1000, 'mo-', 'LineWidth', 1.2, 'MarkerFaceColor', 'm');
plot(comparison_timesteps, euler_comparison_final_errors / 1000, 'ms-', 'LineWidth', 1.2, 'MarkerFaceColor', 'm');
xlabel('Timestep (s)');
ylabel('Position Error (km)');
title('Position Error vs Timestep');
legend('Verlet Maximum Error', 'Verlet Final Error','Euler Maximum Error', 'Euler Final Error', 'Location', 'northoutside', 'Orientation', 'horizontal');
grid on;
