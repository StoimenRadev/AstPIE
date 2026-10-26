clear;
clc;
close all;

star_mass = input("Enter the mass of the star (kg): ");
planet_mass = input("Enter the mass of the planet (kg): ");

r0_planet = input("Enter the initial position [rx ry] (m): ");
v0_planet = input("Enter the initial velocity [vx vy] (m/s): ");

dt = input("Enter the time step (s): ");
T = input("Enter the total simulation duration (s): ");

% Validate star mass
if ~isnumeric(star_mass) || ~isreal(star_mass) || ~isscalar(star_mass) || ~isfinite(star_mass) || star_mass <= 0
    error("Star mass must be a finite, positive scalar.");
end

% Validate planet mass
if ~isnumeric(planet_mass) || ~isreal(planet_mass) || ~isscalar(planet_mass) || ~isfinite(planet_mass) || planet_mass <= 0
    error("Planet mass must be a finite, positive scalar.");
end

% Validate initial position
if ~isnumeric(r0_planet) || ~isreal(r0_planet) || ~isvector(r0_planet) || numel(r0_planet) ~= 2 || any(~isfinite(r0_planet(:)))
    error("Initial position must be a finite, real, two-component vector.");
end

% Validate initial velocity
if ~isnumeric(v0_planet) || ~isreal(v0_planet) || ~isvector(v0_planet) || numel(v0_planet) ~= 2 || any(~isfinite(v0_planet(:)))
    error("Initial velocity must be a finite, real, two-component vector.");
end

% Ensure row-vector format
r0_planet = r0_planet(:).';
v0_planet = v0_planet(:).';

% Validate time step
if ~isnumeric(dt) || ~isreal(dt) || ~isscalar(dt) || ~isfinite(dt) || dt <= 0
    error("Time step must be a finite, positive scalar.");
end

% Validate simulation duration
if ~isnumeric(T) || ~isreal(T) || ~isscalar(T) || ~isfinite(T) || T <= 0
    error("Simulation duration must be a finite, positive scalar.");
end

% Validate duration and time-step compatibility
step_ratio = T / dt;

if ~isfinite(step_ratio) || step_ratio < 1 || abs(step_ratio - round(step_ratio)) > 1e-10 * max(1, abs(step_ratio))
    error("Simulation duration must be an integer multiple of the time step.");
end

num_steps = round(step_ratio);
actual_duration = num_steps * dt;

% Create time history
time_history = (0:num_steps)' * dt;

% Gravitational constant and parameter
G = 6.67430e-11;
mu_star = G * star_mass;

if ~isfinite(mu_star) || mu_star <= 0
    error("The gravitational parameter is invalid.");
end

% Initial position magnitude
r0 = sqrt(r0_planet(1)^2 + r0_planet(2)^2);

if ~isfinite(r0) || r0 == 0
    error("Initial position cannot be at the star.");
end

% Initial speed squared
v0_squared = v0_planet(1)^2 + v0_planet(2)^2;

% Initial specific orbital energy
specific_energy = v0_squared / 2 - mu_star / r0;

if ~isfinite(specific_energy) || specific_energy >= 0
    error("Initial conditions must describe a bound elliptical orbit.");
end

% Initial specific angular momentum (2D)
specific_angular_momentum = r0_planet(1) * v0_planet(2) - r0_planet(2) * v0_planet(1);

if ~isfinite(specific_angular_momentum) || specific_angular_momentum == 0
    error("Initial conditions describe a radial trajectory.");
end

% Calculate analytical eccentricity
eccentricity_squared = 1 + (2 * specific_energy * specific_angular_momentum^2) / mu_star^2;

% Correct tiny floating-point errors near zero
if eccentricity_squared < 0 && eccentricity_squared > -1e-12
    eccentricity_squared = 0;
end

if ~isfinite(eccentricity_squared) || eccentricity_squared < 0 || eccentricity_squared >= 1
    error("The calculated eccentricity is invalid for an ellipse.");
end

eccentricity = sqrt(eccentricity_squared);

% Calculate the eccentricity vector
position_velocity_dot = r0_planet(1) * v0_planet(1) + r0_planet(2) * v0_planet(2);

eccentricity_x = ((v0_squared - mu_star / r0) * r0_planet(1) - position_velocity_dot * v0_planet(1)) / mu_star;

eccentricity_y = ((v0_squared - mu_star / r0) * r0_planet(2) - position_velocity_dot * v0_planet(2)) / mu_star;

% Direction of perihelion
if eccentricity < 1e-10
    perihelion_angle = 0;
else
    perihelion_angle = atan2(eccentricity_y, eccentricity_x);
end

% Calculate analytical ellipse dimensions
semi_major_axis = -mu_star / (2 * specific_energy);
semi_minor_axis = semi_major_axis * sqrt(1 - eccentricity^2);
focal_distance = semi_major_axis * eccentricity;

perihelion_distance = semi_major_axis * (1 - eccentricity);
aphelion_distance = semi_major_axis * (1 + eccentricity);

% Generate analytical ellipse independently of integration
E = linspace(0, 2*pi, 3601);

x_ellipse_local = semi_major_axis * cos(E) - focal_distance;
y_ellipse_local = semi_minor_axis * sin(E);

% Rotate ellipse to match the analytical orbital orientation
x_ellipse = x_ellipse_local * cos(perihelion_angle) - y_ellipse_local * sin(perihelion_angle);

y_ellipse = x_ellipse_local * sin(perihelion_angle) + y_ellipse_local * cos(perihelion_angle);

% Display simulation parameters
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

% Initialize the planet's numerical state
r_planet = r0_planet;
v_planet = v0_planet;

% Allocate numerical history arrays
planet_positions = zeros(num_steps + 1, 2);
planet_velocities = zeros(num_steps + 1, 2);
planet_accelerations = zeros(num_steps + 1, 2);

% Store initial state
planet_positions(1, :) = r_planet;
planet_velocities(1, :) = v_planet;

% Initial acceleration
a_planet = -(mu_star / r0^3) * r_planet;
planet_accelerations(1, :) = a_planet;

% Numerical integration using semi-implicit Euler
for i = 1:num_steps

    % Calculate acceleration at the current position
    r_star = sqrt(r_planet(1)^2 + r_planet(2)^2);

    if ~isfinite(r_star) || r_star == 0
        error("The planet reached an invalid position at the star.");
    end

    a_planet = -(mu_star / r_star^3) * r_planet;

    % Update velocity, then position
    v_planet = v_planet + a_planet * dt;
    r_planet = r_planet + v_planet * dt;

    % Check numerical values
    if any(~isfinite(r_planet)) || any(~isfinite(v_planet))
        error("The simulation produced invalid numerical values.");
    end

    % Store the updated state
    planet_positions(i + 1, :) = r_planet;
    planet_velocities(i + 1, :) = v_planet;

    % Calculate acceleration at the updated position
    r_star = sqrt(r_planet(1)^2 + r_planet(2)^2);

    if ~isfinite(r_star) || r_star == 0
        error("The planet reached an invalid position at the star.");
    end

    a_planet = -(mu_star / r_star^3) * r_planet;
    planet_accelerations(i + 1, :) = a_planet;

end

% Calculate numerical distance and speed histories
distance_history = sqrt(planet_positions(:, 1).^2 + planet_positions(:, 2).^2);

speed_history = sqrt(planet_velocities(:, 1).^2 + planet_velocities(:, 2).^2);

% Compare numerical trajectory with sampled analytical ellipse
ellipse_distance_history = zeros(num_steps + 1, 1);

for i = 1:num_steps + 1

    dx = x_ellipse - planet_positions(i, 1);
    dy = y_ellipse - planet_positions(i, 2);

    distances_to_ellipse = sqrt(dx.^2 + dy.^2);

    ellipse_distance_history(i) = min(distances_to_ellipse);

end

maximum_ellipse_distance = max(ellipse_distance_history);
mean_ellipse_distance = mean(ellipse_distance_history);

% Find observed minimum and maximum distances
[minimum_distance, minimum_index] = min(distance_history);
[maximum_distance, maximum_index] = max(distance_history);

minimum_distance_time = time_history(minimum_index);
maximum_distance_time = time_history(maximum_index);

minimum_distance_position = planet_positions(minimum_index, :);
maximum_distance_position = planet_positions(maximum_index, :);

minimum_distance_velocity = planet_velocities(minimum_index, :);
maximum_distance_velocity = planet_velocities(maximum_index, :);

% Compare observed distances with analytical extrema
perihelion_distance_error = minimum_distance - perihelion_distance;

aphelion_distance_error = maximum_distance - aphelion_distance;

perihelion_distance_error_percent = 100 * abs(perihelion_distance_error) / perihelion_distance;

aphelion_distance_error_percent = 100 * abs(aphelion_distance_error) / aphelion_distance;

% Display initial state
fprintf("\nINITIAL STATE\n");
fprintf("Position: [%.3e, %.3e] m\n", planet_positions(1, 1), planet_positions(1, 2));
fprintf("Velocity: [%.3f, %.3f] m/s\n", planet_velocities(1, 1), planet_velocities(1, 2));
fprintf("Speed: %.3f m/s\n", speed_history(1));

% Display final state
fprintf("\nFINAL STATE\n");
fprintf("Time: %.3f s\n", time_history(end));
fprintf("Position: [%.3e, %.3e] m\n", planet_positions(end, 1), planet_positions(end, 2));
fprintf("Velocity: [%.3f, %.3f] m/s\n", planet_velocities(end, 1), planet_velocities(end, 2));
fprintf("Acceleration: [%.3e, %.3e] m/s^2\n", planet_accelerations(end, 1), planet_accelerations(end, 2));
fprintf("Distance from star: %.3e m\n", distance_history(end));
fprintf("Speed: %.3f m/s\n", speed_history(end));

% Display analytical orbital elements
fprintf("\nANALYTICAL ORBITAL ELEMENTS\n");
fprintf("Specific orbital energy: %.3e m^2/s^2\n", specific_energy);
fprintf("Specific angular momentum: %.3e m^2/s\n", specific_angular_momentum);
fprintf("Semimajor axis: %.3e m\n", semi_major_axis);
fprintf("Semiminor axis: %.3e m\n", semi_minor_axis);
fprintf("Eccentricity: %.6f\n", eccentricity);
fprintf("Perihelion direction: %.3f degrees\n", perihelion_angle * 180 / pi);

% Display analytical ellipse geometry
fprintf("\nANALYTICAL ELLIPSE GEOMETRY\n");
fprintf("Focal distance: %.3e m\n", focal_distance);
fprintf("Analytical perihelion distance: %.3e m\n", perihelion_distance);
fprintf("Analytical aphelion distance: %.3e m\n", aphelion_distance);

% Display observed minimum distance
fprintf("\nNUMERICAL MINIMUM DISTANCE\n");
fprintf("Distance: %.3e m\n", minimum_distance);
fprintf("Time: %.3f s\n", minimum_distance_time);
fprintf("Position: [%.3e, %.3e] m\n", minimum_distance_position(1), minimum_distance_position(2));
fprintf("Velocity: [%.3f, %.3f] m/s\n", minimum_distance_velocity(1), minimum_distance_velocity(2));

% Display observed maximum distance
fprintf("\nNUMERICAL MAXIMUM DISTANCE\n");
fprintf("Distance: %.3e m\n", maximum_distance);
fprintf("Time: %.3f s\n", maximum_distance_time);
fprintf("Position: [%.3e, %.3e] m\n", maximum_distance_position(1), maximum_distance_position(2));
fprintf("Velocity: [%.3f, %.3f] m/s\n", maximum_distance_velocity(1), maximum_distance_velocity(2));

% Display perihelion and aphelion comparisons
fprintf("\nANALYTICAL VS NUMERICAL DISTANCE EXTREMA\n");
fprintf("Analytical perihelion: %.6e m\n", perihelion_distance);
fprintf("Observed minimum distance: %.6e m\n", minimum_distance);
fprintf("Minimum-distance difference: %+.6e m\n", perihelion_distance_error);
fprintf("Absolute perihelion difference: %.6f %%\n", perihelion_distance_error_percent);

fprintf("\nAnalytical aphelion: %.6e m\n", aphelion_distance);
fprintf("Observed maximum distance: %.6e m\n", maximum_distance);
fprintf("Maximum-distance difference: %+.6e m\n", aphelion_distance_error);
fprintf("Absolute aphelion difference: %.6f %%\n", aphelion_distance_error_percent);

% Display geometric comparison results
fprintf("\nNUMERICAL TRAJECTORY VS ANALYTICAL ELLIPSE\n");
fprintf("Maximum distance to sampled ellipse: %.6e m\n", maximum_ellipse_distance);
fprintf("Mean distance to sampled ellipse: %.6e m\n", mean_ellipse_distance);

% Plot numerical trajectory and analytical ellipse
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
title("Numerical Orbit vs Analytical Ellipse");
legend("Numerical Trajectory", "Analytical Ellipse", "Star (Focus)", "Starting Position", "Ending Position", "Observed Minimum Distance", "Observed Maximum Distance");

% Plot position components against time
figure;
subplot(2, 1, 1);
plot(time_history, planet_positions(:, 1), 'r-', 'LineWidth', 1.5);
xlabel("Time (s)");
ylabel("X-position (m)");
title("X-position vs Time");
grid on;

subplot(2, 1, 2);
plot(time_history, planet_positions(:, 2), 'b-', 'LineWidth', 1.5);
xlabel("Time (s)");
ylabel("Y-position (m)");
title("Y-position vs Time");
grid on;

% Plot distance from the star against time
figure;
plot(time_history, distance_history, 'b-', 'LineWidth', 1.5);
grid on;
xlabel("Time (s)");
ylabel("Distance from Star (m)");
title("Star-Planet Distance vs Time");

% Plot speed against time
figure;
plot(time_history, speed_history, 'r-', 'LineWidth', 1.5);
grid on;
xlabel("Time (s)");
ylabel("Speed (m/s)");
title("Planet Speed vs Time");

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
text(0.1, 0.18, sprintf("Mean ellipse distance: %.3e m", mean_ellipse_distance));
text(0.1, 0.08, sprintf("Maximum ellipse distance: %.3e m", maximum_ellipse_distance));
