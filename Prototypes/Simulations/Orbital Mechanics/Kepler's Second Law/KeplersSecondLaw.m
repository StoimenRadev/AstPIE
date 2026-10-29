clear;
clc;
close all;

star_mass = input("Enter the mass of the star (kg): ");
G = 6.67430e-11; % Gravitational constant (m^3 kg^-1 s^-2)

if ~isnumeric(star_mass) || ~isreal(star_mass) || ...
   ~isscalar(star_mass) || ~isfinite(star_mass) || star_mass <= 0
    error("Star mass must be a finite, positive scalar.");
end

mu_star = G * star_mass; % Gravitational parameter (m^3/s^2)

% Initial position and velocity

r0_planet = input("Enter the initial position [rx ry] (m): ");
v0_planet = input("Enter the initial velocity [vx vy] (m/s): ");

if ~isnumeric(r0_planet) || ~isreal(r0_planet) || ~isvector(r0_planet) || numel(r0_planet) ~= 2 || any(~isfinite(r0_planet(:)))
    error("Initial position must be a finite, real, two-component vector.");
end

if ~isnumeric(v0_planet) || ~isreal(v0_planet) || ~isvector(v0_planet) || numel(v0_planet) ~= 2 || any(~isfinite(v0_planet(:)))
    error("Initial velocity must be a finite, real, two-component vector.");
end

% Standardize vectors to row format
r0_planet = r0_planet(:).';
v0_planet = v0_planet(:).';

% Initial distance from the star
r0_distance = sqrt(r0_planet(1)^2 + r0_planet(2)^2);

if ~isfinite(r0_distance) || r0_distance == 0
    error("Initial position must be a finite, nonzero distance from the star.");
end

% Initial speed
v0_speed = sqrt(v0_planet(1)^2 + v0_planet(2)^2);

% Initial specific angular momentum (m^2/s)
h0 = r0_planet(1) * v0_planet(2) - r0_planet(2) * v0_planet(1);

if ~isfinite(h0) || h0 == 0
    error("Initial specific angular momentum must be finite and nonzero.");
end

% Analytical Orbital Elements
% Initial specific orbital energy (J/kg)
specific_energy = (v0_speed^2 / 2) - (mu_star / r0_distance);

if specific_energy >= 0
    error("The initial conditions do not describe a bound elliptical orbit.");
end

% Semimajor axis (m)
semimajor_axis = -mu_star / (2 * specific_energy);

% Eccentricity
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

% Semiminor axis (m)
semiminor_axis = semimajor_axis * sqrt(1 - eccentricity^2);

% Focal distance (m)
focal_distance = semimajor_axis * eccentricity;

% Perihalion and aphelion distances (m)
perihelion_distance = semimajor_axis * (1 - eccentricity);
aphelion_distance = semimajor_axis * (1 + eccentricity);

% Eccentricity vector
eccentricity_vector = (((v0_speed^2 - mu_star / r0_distance) * r0_planet)- (r0_planet(1) * v0_planet(1) + r0_planet(2) * v0_planet(2)) * v0_planet) / mu_star;

% Orientation of the ellipse
if eccentricity > 1e-12
    perihelion_angle = atan2(eccentricity_vector(2), eccentricity_vector(1));
else
    % For a circular orbit, the perihelion direction is undefined.
    perihelion_angle = atan2(r0_planet(2), r0_planet(1));
end

% Generate analytical ellipse
ellipse_angle = linspace(0, 2 * pi, 1000);

% Ellipse coordinates relative to its centre
ellipse_x_local = semimajor_axis * cos(ellipse_angle);
ellipse_y_local = semiminor_axis * sin(ellipse_angle);

% Rotate the ellipse to match its orbital orientation
ellipse_x_rotated = ellipse_x_local * cos(perihelion_angle) - ellipse_y_local * sin(perihelion_angle);
ellipse_y_rotated = ellipse_x_local * sin(perihelion_angle) + ellipse_y_local * cos(perihelion_angle);

% Place the ellipse so that the star is at its focus
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
% Include the initial state at t = 0
time_history = (0:num_steps)' * dt;

% Initialize simulation and histories
r_planet = r0_planet;
v_planet = v0_planet;

% Position history (m)
planet_positions = zeros(num_steps + 1, 2);
planet_positions(1, :) = r_planet;

% Velocity history (m/s)
planet_velocities = zeros(num_steps + 1, 2);
planet_velocities(1, :) = v_planet;

% Acceleration history (m/s^2)
planet_accelerations = zeros(num_steps + 1, 2);

% Initial acceleration
planet_accelerations(1, :) = -(mu_star / r0_distance^3) * r0_planet;

if any(~isfinite(planet_accelerations(1, :)))
    error("Initial acceleration contains invalid numerical values.");
end

% Integrate the orbit using semi-implicit Euler
for i = 1:num_steps

    % Current distance from the star (m)
    r_star = sqrt(r_planet(1)^2 + r_planet(2)^2);

    if ~isfinite(r_star) || r_star == 0
        error("The planet reached an invalid position at the star.");
    end

    % Gravitational acceleration at the current position (m/s^2)
    a_planet = -(mu_star / r_star^3) * r_planet;

    if any(~isfinite(a_planet))
        error("The simulation produced invalid acceleration values.");
    end

    % Update velocity first
    v_planet = v_planet + a_planet * dt;

    % Update position using the updated velocity
    r_planet = r_planet + v_planet * dt;

    % Check the new state
    if any(~isfinite(r_planet)) || any(~isfinite(v_planet))
        error("The simulation produced invalid position or velocity values.");
    end

    % Distance at the new position
    r_new_distance = sqrt(r_planet(1)^2 + r_planet(2)^2);

    if ~isfinite(r_new_distance) || r_new_distance == 0
        error("The planet reached an invalid position at the star.");
    end

    % Acceleration corresponding to the new position
    a_new = -(mu_star / r_new_distance^3) * r_planet;

    if any(~isfinite(a_new))
        error("The simulation produced invalid acceleration values.");
    end

    % Store all vector histories at the new timestep
    planet_positions(i + 1, :) = r_planet;
    planet_velocities(i + 1, :) = v_planet;
    planet_accelerations(i + 1, :) = a_new;

end

% Distance from the star at every recorded timestep (m)
planet_distance = sqrt(planet_positions(:, 1).^2 + planet_positions(:, 2).^2);

% Speed at every recorded timestep (m/s)
planet_speed = sqrt(planet_velocities(:, 1).^2 + planet_velocities(:, 2).^2);

% Specific angular momentum at every timestep (m^2/s)
specific_angular_momentum = planet_positions(:, 1) .* planet_velocities(:, 2) - planet_positions(:, 2) .* planet_velocities(:, 1);

% Instantaneous areal velocity at every timestep (m^2/s)
areal_velocity = specific_angular_momentum / 2;

% Calculate swept-area history
% Approximate area swept between consecutive positions (m^2)
swept_area_steps = 0.5 * abs(planet_positions(1:end-1, 1) .* planet_positions(2:end, 2) - planet_positions(1:end-1, 2) .* planet_positions(2:end, 1));

% The initial state has no preceding interval, so its area is zero.
swept_area_history = [0; swept_area_steps];

% Cumulative swept area from the beginning of the simulation (m^2)
cumulative_swept_area = [0; cumsum(swept_area_steps)];

%% Equal-time interval measurements
% Number of measurement intervals, including a possible shorter final interval
num_intervals = ceil(num_steps / interval_steps);

% Initialize interval histories
interval_start_time = zeros(num_intervals, 1);
interval_end_time = zeros(num_intervals, 1);
interval_duration = zeros(num_intervals, 1);
interval_swept_area = zeros(num_intervals, 1);
interval_average_areal_velocity = zeros(num_intervals, 1);

for i = 1:num_intervals

    % First and last integration steps in this interval
    first_step = (i - 1) * interval_steps + 1;
    last_step = min(i * interval_steps, num_steps);

    % Record interval boundaries
    interval_start_time(i) = time_history(first_step);
    interval_end_time(i) = time_history(last_step + 1);

    % Actual duration of this interval (s)
    interval_duration(i) = interval_end_time(i) - interval_start_time(i);

    % Sum the approximate areas swept during its integration steps (m^2)
    interval_swept_area(i) = sum(swept_area_steps(first_step:last_step));

    % Average areal velocity over this interval (m^2/s)
    interval_average_areal_velocity(i) = interval_swept_area(i) / interval_duration(i);

end

% Display simulation setup
fprintf('\nSimulation Setup\n');
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
fprintf('\nFinal Results\n');
fprintf('Final time: %.6e s\n', time_history(end));
fprintf('Final position: [%.6e, %.6e] m\n', planet_positions(end, 1), planet_positions(end, 2));
fprintf('Final velocity: [%.6e, %.6e] m/s\n', planet_velocities(end, 1), planet_velocities(end, 2));
fprintf('Final speed: %.6e m/s\n', planet_speed(end));
fprintf('Final distance: %.6e m\n', planet_distance(end));
fprintf('Final acceleration: [%.6e, %.6e] m/s^2\n', planet_accelerations(end, 1), planet_accelerations(end, 2));
fprintf('Final specific angular momentum: %.6e m^2/s\n', specific_angular_momentum(end));
fprintf('Final areal velocity: %.6e m^2/s\n', areal_velocity(end));

% Display Analytical Orbital Elements
fprintf("\nAnalytical Orbital Elements\n");
fprintf("Specific orbital energy: %.6e J/kg\n", specific_energy);
fprintf("Specific angular momentum: %.6e m^2/s\n", h0);
fprintf("Semimajor axis: %.6e m\n", semimajor_axis);
fprintf("Eccentricity: %.8f\n", eccentricity);
fprintf("Semiminor axis: %.6e m\n", semiminor_axis);
fprintf("Focal distance: %.6e m\n", focal_distance);
fprintf("Perihelion distance: %.6e m\n", perihelion_distance);
fprintf("Aphelion distance: %.6e m\n", aphelion_distance);

% Relative changes in angular momentum and areal velocity
h_change = abs(specific_angular_momentum(end) - specific_angular_momentum(1)) / abs(specific_angular_momentum(1)) * 100;

areal_velocity_change = abs(areal_velocity(end) - areal_velocity(1)) / abs(areal_velocity(1)) * 100;

fprintf('Specific angular momentum change: %.6e %%\n', h_change);
fprintf('Areal velocity change: %.6e %%\n', areal_velocity_change);

% Display complete time history
fprintf('\nComplete Time History\n');

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

% Display equal-time interval comparison
fprintf('\nEqual-Time Interval Comparison\n');
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
plot(ellipse_x, ellipse_y, 'r--');

% Plot the star at the focus
plot(0, 0, 'yo', 'MarkerFaceColor', 'y', 'MarkerSize', 12);

% Analytical ellipse centre
ellipse_center_x = -focal_distance * cos(perihelion_angle);
ellipse_center_y = -focal_distance * sin(perihelion_angle);
plot(ellipse_center_x, ellipse_center_y, 'kx', 'MarkerSize', 10, 'LineWidth', 2);

% Semimajor axis line
plot([ellipse_center_x - semimajor_axis * cos(perihelion_angle), ...
      ellipse_center_x + semimajor_axis * cos(perihelion_angle)], ...
     [ellipse_center_y - semimajor_axis * sin(perihelion_angle), ...
      ellipse_center_y + semimajor_axis * sin(perihelion_angle)], ...
     'k:', 'LineWidth', 1.2);

% Semiminor axis line
plot([ellipse_center_x - semiminor_axis * sin(perihelion_angle), ...
      ellipse_center_x + semiminor_axis * sin(perihelion_angle)], ...
     [ellipse_center_y + semiminor_axis * cos(perihelion_angle), ...
      ellipse_center_y - semiminor_axis * cos(perihelion_angle)], ...
     'g:', 'LineWidth', 1.2);

xlabel('X Position (m)');
ylabel('Y Position (m)');
title('Kepler''s First Law - Analytical and Numerical Orbits');
legend('Numerical Trajectory', 'Analytical Ellipse', 'Star (Focus)', 'Ellipse Centre', 'Major Axis', 'Minor Axis');
axis equal;
grid on;
