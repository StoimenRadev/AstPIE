clear;
clc;
close all;

G = 6.67430e-11; % Gravitational constant (m^3 kg^-1 s^-2)

seconds_per_day = 86400;
days_per_year = 365.25;
AU = 1.495978707e11;

% Simulation inputs
star_mass = input("Enter the mass of the star (kg): ");
planet_mass = input("Enter the mass of the planet (kg): ");

if ~isnumeric(star_mass) || ~isreal(star_mass) || ~isscalar(star_mass) || ~isfinite(star_mass) || star_mass <= 0
    error("Star mass must be a finite, positive scalar.");
end

if ~isnumeric(planet_mass) || ~isreal(planet_mass) || ~isscalar(planet_mass) || ~isfinite(planet_mass) || planet_mass <= 0
    error("Planet mass must be a finite, positive scalar.");
end

% Gravitational parameter
mu = G * (star_mass + planet_mass);

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

% Initial specific orbital energy (J/kg)
specific_energy = (v0_speed^2 / 2) - (mu / r0_distance);

if specific_energy >= 0
    error("The initial conditions do not describe a bound elliptical orbit.");
end

% Initial specific angular momentum (m^2/s)
h0 = r0_planet(1) * v0_planet(2) - r0_planet(2) * v0_planet(1);

if ~isfinite(h0) || h0 == 0
    error("Initial specific angular momentum must be finite and nonzero.");
end

% Analytical Orbital Elements

% Semimajor axis (m)
semimajor_axis = -mu / (2 * specific_energy);

% Eccentricity
eccentricity_squared = 1 + (2 * specific_energy * h0^2) / mu^2;

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

% Periapsis and apoapsis distances (m)
periapsis_distance = semimajor_axis * (1 - eccentricity);
apoapsis_distance = semimajor_axis * (1 + eccentricity);

% Eccentricity vector
eccentricity_vector = (((v0_speed^2 - mu / r0_distance) * r0_planet) - (r0_planet(1) * v0_planet(1) + r0_planet(2) * v0_planet(2)) * v0_planet) / mu;

% Orientation of the ellipse
if eccentricity > 1e-12
    periapsis_angle = atan2(eccentricity_vector(2), eccentricity_vector(1));
else
    periapsis_angle = atan2(r0_planet(2), r0_planet(1));
end

% Analytical orbital period (s)
orbital_period = 2 * pi * sqrt(semimajor_axis^3 / mu);

% Mean motion (rad/s)
mean_motion = sqrt(mu / semimajor_axis^3);

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

% Create time history
time_history = (0:num_steps)' * dt;
time_days = time_history / seconds_per_day;

% Analytical Reference Orbit

if eccentricity > 1e-12

    % Initial eccentric anomaly
    cos_E0 = (1 - r0_distance / semimajor_axis) / eccentricity;
    sin_E0 = (r0_planet(1) * v0_planet(1) + r0_planet(2) * v0_planet(2)) / (eccentricity * sqrt(mu * semimajor_axis));

    % Protect against small floating-point errors
    cos_E0 = max(-1, min(1, cos_E0));
    sin_E0 = max(-1, min(1, sin_E0));

    E0 = atan2(sin_E0, cos_E0);

    % Initial mean anomaly
    M0 = E0 - eccentricity * sin(E0);

end

% Analytical position history
analytical_positions = zeros(num_steps + 1, 2);

if eccentricity > 1e-12

    for i = 1:num_steps + 1

        % Mean anomaly at the current time
        M = mod(M0 + mean_motion * time_history(i), 2 * pi);

        % Solve Kepler's equation using bisection
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

        % Position in the orbital coordinate system
        x_local = semimajor_axis * (cos(E) - eccentricity);
        y_local = semimajor_axis * sqrt(1 - eccentricity^2) * sin(E);

        % Rotate into the global coordinate system
        analytical_positions(i, 1) = x_local * cos(periapsis_angle) - y_local * sin(periapsis_angle);

        analytical_positions(i, 2) = x_local * sin(periapsis_angle) + y_local * cos(periapsis_angle);

    end

else

    % Analytical positions for a circular orbit
    initial_angle = atan2(r0_planet(2), r0_planet(1));
    rotation_direction = sign(h0);

    for i = 1:num_steps + 1

        angle = initial_angle + rotation_direction * mean_motion * time_history(i);

        analytical_positions(i, 1) = r0_distance * cos(angle);
        analytical_positions(i, 2) = r0_distance * sin(angle);

    end

end

% Initialize Velocity Verlet and Euler simulations

verlet_r = r0_planet;
verlet_v = v0_planet;

euler_r = r0_planet;
euler_v = v0_planet;

% Initialize position histories
planet_positions = zeros(num_steps + 1, 2);
euler_planet_positions = zeros(num_steps + 1, 2);

planet_positions(1, :) = verlet_r;
euler_planet_positions(1, :) = euler_r;

% Initialize velocity histories
planet_velocities = zeros(num_steps + 1, 2);
euler_planet_velocities = zeros(num_steps + 1, 2);

planet_velocities(1, :) = verlet_v;
euler_planet_velocities(1, :) = euler_v;

% Initialize acceleration histories
planet_accelerations = zeros(num_steps + 1, 2);
euler_planet_accelerations = zeros(num_steps + 1, 2);

% Initial accelerations
initial_acceleration = -(mu / r0_distance^3) * r0_planet;

planet_accelerations(1, :) = initial_acceleration;
euler_planet_accelerations(1, :) = initial_acceleration;

% Numerical Integration

for i = 1:num_steps

    % Velocity Verlet

    verlet_distance = sqrt(verlet_r(1)^2 + verlet_r(2)^2);

    if ~isfinite(verlet_distance) || verlet_distance == 0
        error("Velocity Verlet produced an invalid position.");
    end

    verlet_acceleration = -(mu / verlet_distance^3) * verlet_r;

    % Update position
    verlet_r_new = verlet_r + verlet_v * dt + 0.5 * verlet_acceleration * dt^2;
    verlet_new_distance = sqrt(verlet_r_new(1)^2 + verlet_r_new(2)^2);

    if ~isfinite(verlet_new_distance) || verlet_new_distance == 0
        error("Velocity Verlet produced an invalid position.");
    end

    % Calculate acceleration at the new position
    verlet_acceleration_new = -(mu / verlet_new_distance^3) * verlet_r_new;

    % Update velocity
    verlet_v_new = verlet_v + 0.5 * (verlet_acceleration + verlet_acceleration_new) * dt;

    if any(~isfinite(verlet_r_new)) || any(~isfinite(verlet_v_new))
        error("Velocity Verlet produced invalid numerical values.");
    end

    % Store Velocity Verlet results
    verlet_r = verlet_r_new;
    verlet_v = verlet_v_new;

    planet_positions(i + 1, :) = verlet_r;
    planet_velocities(i + 1, :) = verlet_v;
    planet_accelerations(i + 1, :) = verlet_acceleration_new;

    % Semi-Implicit Euler

    euler_distance = sqrt(euler_r(1)^2 + euler_r(2)^2);

    if ~isfinite(euler_distance) || euler_distance == 0
        error("Euler produced an invalid position.");
    end

    euler_acceleration = -(mu / euler_distance^3) * euler_r;

    % Update velocity first
    euler_v_new = euler_v + euler_acceleration * dt;

    % Update position using the updated velocity
    euler_r_new = euler_r + euler_v_new * dt;

    euler_new_distance = sqrt(euler_r_new(1)^2 + euler_r_new(2)^2);

    if ~isfinite(euler_new_distance) || euler_new_distance == 0
        error("Euler produced an invalid position.");
    end

    % Calculate acceleration at the new position
    euler_acceleration_new = -(mu / euler_new_distance^3) * euler_r_new;

    if any(~isfinite(euler_r_new)) || any(~isfinite(euler_v_new))
        error("Euler produced invalid numerical values.");
    end

    % Store Euler results
    euler_r = euler_r_new;
    euler_v = euler_v_new;

    euler_planet_positions(i + 1, :) = euler_r;
    euler_planet_velocities(i + 1, :) = euler_v;
    euler_planet_accelerations(i + 1, :) = euler_acceleration_new;

end

% Calculate distances and speeds
planet_distance = sqrt(planet_positions(:, 1).^2 + planet_positions(:, 2).^2);
euler_planet_distance = sqrt(euler_planet_positions(:, 1).^2 + euler_planet_positions(:, 2).^2);
planet_speed = sqrt(planet_velocities(:, 1).^2 + planet_velocities(:, 2).^2);
euler_planet_speed = sqrt(euler_planet_velocities(:, 1).^2 + euler_planet_velocities(:, 2).^2);

% Calculate specific orbital energy histories
specific_energy_history = (planet_speed.^2 / 2) - (mu ./ planet_distance);
euler_specific_energy_history = (euler_planet_speed.^2 / 2) - (mu ./ euler_planet_distance);

% Calculate specific angular momentum histories
specific_angular_momentum = planet_positions(:, 1) .* planet_velocities(:, 2) - planet_positions(:, 2) .* planet_velocities(:, 1);
euler_specific_angular_momentum = euler_planet_positions(:, 1) .* euler_planet_velocities(:, 2) - euler_planet_positions(:, 2) .* euler_planet_velocities(:, 1);

% Numerical Orbital Period Measurement
% Measure the time required for each numerical orbit to complete one full revolution.
verlet_angle = zeros(num_steps + 1, 1);
euler_angle = zeros(num_steps + 1, 1);

verlet_measured_period = NaN;
euler_measured_period = NaN;

target_angle = sign(h0) * 2 * pi;

for i = 2:num_steps + 1

    % Velocity Verlet angular progress
    cross_product = planet_positions(i - 1, 1) * planet_positions(i, 2) - planet_positions(i - 1, 2) * planet_positions(i, 1);

    dot_product = planet_positions(i - 1, 1) * planet_positions(i, 1) + planet_positions(i - 1, 2) * planet_positions(i, 2);

    angle_change = atan2(cross_product, dot_product);
    verlet_angle(i) = verlet_angle(i - 1) + angle_change;

    % Check for one complete revolution
    if isnan(verlet_measured_period) && sign(h0) * verlet_angle(i) >= 2 * pi

        angle_fraction = (target_angle - verlet_angle(i - 1)) / (verlet_angle(i) - verlet_angle(i - 1));
        verlet_measured_period = time_history(i - 1) + angle_fraction * dt;

    end

    % Semi-Implicit Euler angular progress
    euler_cross_product = euler_planet_positions(i - 1, 1) * euler_planet_positions(i, 2) - euler_planet_positions(i - 1, 2) * euler_planet_positions(i, 1);
    euler_dot_product = euler_planet_positions(i - 1, 1) * euler_planet_positions(i, 1) + euler_planet_positions(i - 1, 2) * euler_planet_positions(i, 2);
    euler_angle_change = atan2(euler_cross_product, euler_dot_product);
    euler_angle(i) = euler_angle(i - 1) + euler_angle_change;

    % Check for one complete revolution
    if isnan(euler_measured_period) && sign(h0) * euler_angle(i) >= 2 * pi

        angle_fraction = (target_angle - euler_angle(i - 1)) / (euler_angle(i) - euler_angle(i - 1));
        euler_measured_period = time_history(i - 1) + angle_fraction * dt;

    end

end

% Period Errors
verlet_absolute_error = NaN;
verlet_relative_error = NaN;
verlet_percentage_error = NaN;

euler_absolute_error = NaN;
euler_relative_error = NaN;
euler_percentage_error = NaN;

if ~isnan(verlet_measured_period)

    verlet_absolute_error = abs(verlet_measured_period - orbital_period);
    verlet_relative_error = verlet_absolute_error / orbital_period;
    verlet_percentage_error = verlet_relative_error * 100;

end

if ~isnan(euler_measured_period)

    euler_absolute_error = abs(euler_measured_period - orbital_period);
    euler_relative_error = euler_absolute_error / orbital_period;
    euler_percentage_error = euler_relative_error * 100;

end

% Numerical Position Errors
verlet_position_error = zeros(num_steps + 1, 1);
euler_position_error = zeros(num_steps + 1, 1);

for i = 1:num_steps + 1

    % Velocity Verlet position error
    error_x = planet_positions(i, 1) - analytical_positions(i, 1);
    error_y = planet_positions(i, 2) - analytical_positions(i, 2);

    verlet_position_error(i) = sqrt(error_x^2 + error_y^2);

    % Euler position error
    error_x = euler_planet_positions(i, 1) - analytical_positions(i, 1);
    error_y = euler_planet_positions(i, 2) - analytical_positions(i, 2);

    euler_position_error(i) = sqrt(error_x^2 + error_y^2);

end

% Position error results
verlet_initial_position_error = verlet_position_error(1);
verlet_final_position_error = verlet_position_error(end);
verlet_maximum_position_error = max(verlet_position_error);

euler_initial_position_error = euler_position_error(1);
euler_final_position_error = euler_position_error(end);
euler_maximum_position_error = max(euler_position_error);

% Orbital Distance Comparison
verlet_periapsis_distance = min(planet_distance);
verlet_apoapsis_distance = max(planet_distance);

euler_periapsis_distance = min(euler_planet_distance);
euler_apoapsis_distance = max(euler_planet_distance);

% Conservation Errors
verlet_energy_error = abs(specific_energy_history(end) - specific_energy_history(1));
euler_energy_error = abs(euler_specific_energy_history(end) - euler_specific_energy_history(1));
verlet_angular_momentum_error = abs(specific_angular_momentum(end) - specific_angular_momentum(1));
euler_angular_momentum_error = abs(euler_specific_angular_momentum(end) - euler_specific_angular_momentum(1));

% Display Simulation Setup
fprintf("\nSimulation Setup\n");
fprintf("Star mass: %.6e kg\n", star_mass);
fprintf("Planet mass: %.6e kg\n", planet_mass);
fprintf("Gravitational parameter: %.6e m^3/s^2\n", mu);
fprintf("Time step: %.6f s\n", dt);
fprintf("Total simulation time: %.6f s\n", actual_duration);
fprintf("Number of integration steps: %d\n", num_steps);
fprintf("Number of recorded states: %d\n", num_steps + 1);

% Display Initial Conditions
fprintf("\nInitial Conditions\n");
fprintf("Initial position: [%.6e, %.6e] m\n", r0_planet(1), r0_planet(2));
fprintf("Initial velocity: [%.6e, %.6e] m/s\n", v0_planet(1), v0_planet(2));
fprintf("Initial speed: %.6e m/s\n", v0_speed);
fprintf("Initial distance: %.6e m\n", r0_distance);
fprintf("Initial specific orbital energy: %.6e J/kg\n", specific_energy);
fprintf("Initial specific angular momentum: %.6e m^2/s\n", h0);

fprintf("\nAnalytical Solution\n");
fprintf("Method: Newtonian two-body orbital mechanics\n");

fprintf("\nNumerical Solutions\n");
fprintf("Default method: %s\n", default_method);
fprintf("Comparison method: Semi-Implicit Euler\n");

% Display Analytical Orbital Parameters
fprintf("\nAnalytical Orbital Parameters\n");
fprintf("Semimajor axis: %.6e m\n", semimajor_axis);
fprintf("Semiminor axis: %.6e m\n", semiminor_axis);
fprintf("Eccentricity: %.8f\n", eccentricity);
fprintf("Focal distance: %.6e m\n", focal_distance);
fprintf("Periapsis distance: %.6e m\n", periapsis_distance);
fprintf("Apoapsis distance: %.6e m\n", apoapsis_distance);

% Display Analytical Period
fprintf("\nAnalytical Orbital Period\n");
fprintf("Orbital period: %.6e s\n", orbital_period);
fprintf("Orbital period: %.6f days\n", orbital_period / seconds_per_day);
fprintf("Orbital period: %.6f years\n", orbital_period / (days_per_year * seconds_per_day));

% Display Numerical Period Results
fprintf("\nOrbital Period Comparison\n");
fprintf("Analytical period: %.6e s\n", orbital_period);

if ~isnan(verlet_measured_period)

    fprintf("\nVelocity Verlet\n");
    fprintf("Measured period: %.6e s\n", verlet_measured_period);
    fprintf("Absolute period error: %.6e s\n", verlet_absolute_error);
    fprintf("Relative period error: %.6e\n", verlet_relative_error);
    fprintf("Percentage period error: %.6f %%\n", verlet_percentage_error);

else

    fprintf("\nVelocity Verlet: A complete revolution was not recorded.\n");

end

if ~isnan(euler_measured_period)

    fprintf("\nSemi-Implicit Euler\n");
    fprintf("Measured period: %.6e s\n", euler_measured_period);
    fprintf("Absolute period error: %.6e s\n", euler_absolute_error);
    fprintf("Relative period error: %.6e\n", euler_relative_error);
    fprintf("Percentage period error: %.6f %%\n", euler_percentage_error);

else

    fprintf("\nSemi-Implicit Euler: A complete revolution was not recorded.\n");

end

% Display Numerical Position Errors
fprintf("\nNumerical Position Errors\n");
fprintf("\nVelocity Verlet\n");
fprintf("Initial position error: %.6e m\n", verlet_initial_position_error);
fprintf("Final position error: %.6e m\n", verlet_final_position_error);
fprintf("Maximum position error: %.6e m\n", verlet_maximum_position_error);
fprintf("\nSemi-Implicit Euler\n");
fprintf("Initial position error: %.6e m\n", euler_initial_position_error);
fprintf("Final position error: %.6e m\n", euler_final_position_error);
fprintf("Maximum position error: %.6e m\n", euler_maximum_position_error);

% Display Orbital Conservation Errors
fprintf("\nOrbital Conservation Errors\n");
fprintf("\nVelocity Verlet\n");
fprintf("Specific orbital energy change: %.6e J/kg\n", verlet_energy_error);
fprintf("Specific angular momentum change: %.6e m^2/s\n", verlet_angular_momentum_error);
fprintf("\nSemi-Implicit Euler\n");
fprintf("Specific orbital energy change: %.6e J/kg\n", euler_energy_error);
fprintf("Specific angular momentum change: %.6e m^2/s\n", euler_angular_momentum_error);

% Display Final Results
fprintf("\nFinal Results\n");
fprintf("\nVelocity Verlet\n");
fprintf("Final position: [%.6e, %.6e] m\n", planet_positions(end, 1), planet_positions(end, 2));
fprintf("Final velocity: [%.6e, %.6e] m/s\n", planet_velocities(end, 1), planet_velocities(end, 2));
fprintf("Final distance: %.6e m\n", planet_distance(end));
fprintf("Final speed: %.6e m/s\n", planet_speed(end));

fprintf("\nSemi-Implicit Euler\n");
fprintf("Final position: [%.6e, %.6e] m\n", euler_planet_positions(end, 1), euler_planet_positions(end, 2));
fprintf("Final velocity: [%.6e, %.6e] m/s\n", euler_planet_velocities(end, 1), euler_planet_velocities(end, 2));
fprintf("Final distance: %.6e m\n", euler_planet_distance(end));
fprintf("Final speed: %.6e m/s\n", euler_planet_speed(end));

% Display Complete Time History
fprintf("\nComplete Time History\n");
results = [ ...
    time_history, ...
    planet_positions, ...
    planet_velocities, ...
    planet_speed, ...
    planet_accelerations, ...
    planet_distance, ...
    specific_energy_history, ...
    specific_angular_momentum];

disp(["Columns: time(s), x(m), y(m), vx(m/s), vy(m/s), ", "speed(m/s), ax(m/s^2), ay(m/s^2), distance(m), ", "specific_energy(J/kg), specific_angular_momentum(m^2/s)"]);
disp(results);

fprintf("\nEuler Time History\n");
euler_results = [ ...
    time_history, ...
    euler_planet_positions, ...
    euler_planet_velocities, ...
    euler_planet_speed, ...
    euler_planet_accelerations, ...
    euler_planet_distance, ...
    euler_specific_energy_history, ...
    euler_specific_angular_momentum];

disp(euler_results);

% Plot Analytical and Numerical Orbits
figure;
plot(analytical_positions(:, 1) / AU, analytical_positions(:, 2) / AU, 'r--', 'LineWidth', 1.5);
hold on;
plot(planet_positions(:, 1) / AU, planet_positions(:, 2) / AU, 'b-', 'LineWidth', 1.2);
plot(euler_planet_positions(:, 1) / AU, euler_planet_positions(:, 2) / AU, 'g-', 'LineWidth', 1.2);
plot(0, 0, 'yo', 'MarkerFaceColor', 'y', 'MarkerSize', 12);
xlabel('X Position (AU)');
ylabel('Y Position (AU)');
title('Kepler''s Third Law - Analytical and Numerical Orbits');
legend('Analytical Orbit', 'Velocity Verlet', 'Semi-Implicit Euler', 'Star');
axis equal;
grid on;

% Plot Star-Planet Distance Against Time
analytical_distance = sqrt(analytical_positions(:, 1).^2 + analytical_positions(:, 2).^2);
figure;
plot(time_days, planet_distance / AU, 'b-', 'LineWidth', 1.2);
hold on;
plot(time_days, euler_planet_distance / AU, 'g-', 'LineWidth', 1.2);
plot(time_days, analytical_distance / AU, 'r--', 'LineWidth', 1.8);
xlabel('Time (days)');
ylabel('Star-Planet Distance (AU)');
title('Star-Planet Distance vs Time');
legend('Velocity Verlet', 'Semi-Implicit Euler', 'Analytical');
grid on;

% Plot Specific Orbital Energy Against Time
figure;
plot(time_days, specific_energy_history, 'b-', 'LineWidth', 1.2);
hold on;
plot(time_days, euler_specific_energy_history, 'g-', 'LineWidth', 1.2);
plot(time_days, specific_energy * ones(num_steps + 1, 1), 'r--', 'LineWidth', 1.2);
xlabel('Time (days)');
ylabel('Specific Orbital Energy (J/kg)');
title('Specific Orbital Energy vs Time');
legend('Velocity Verlet', 'Semi-Implicit Euler', 'Analytical Initial Energy');
grid on;

% Plot Specific Angular Momentum Against Time
verlet_angular_momentum_change = (specific_angular_momentum - h0) / abs(h0);
euler_angular_momentum_change = (euler_specific_angular_momentum - h0) / abs(h0);

figure;
plot(time_days, verlet_angular_momentum_change, 'b-', 'LineWidth', 1.2);
hold on;
plot(time_days, euler_angular_momentum_change, 'g-', 'LineWidth', 1.2);
plot(time_days, zeros(num_steps + 1, 1), 'r--', 'LineWidth', 1.2);
xlabel('Time (days)');
ylabel('Relative Change in Specific Angular Momentum');
title('Specific Angular Momentum Conservation');
legend('Velocity Verlet', 'Semi-Implicit Euler', 'Initial Value');
grid on;

% Plot Numerical Position Errors
figure;
plot(time_days, verlet_position_error / 1000, 'b-', 'LineWidth', 1.2);
hold on;
plot(time_days, euler_position_error / 1000, 'g-', 'LineWidth', 1.2);
xlabel('Time (days)');
ylabel('Position Error (km)');
title('Numerical Position Error vs Time');
legend('Velocity Verlet', 'Semi-Implicit Euler');
grid on;

% Plot Orbital Period Comparison
if ~isnan(verlet_measured_period) && ~isnan(euler_measured_period)

    period_values = [orbital_period, verlet_measured_period, euler_measured_period];
    figure;
    plot(1:3, period_values / seconds_per_day, 'o-', 'LineWidth', 1.5, 'MarkerSize', 8);
    xticks(1:3);
    xticklabels({'Analytical', 'Velocity Verlet', 'Euler'});
    xlim([0.5, 3.5]);
    ylabel('Orbital Period (days)');
    title('Analytical and Numerical Orbital Period Comparison');
    grid on;

end

% Display Main Orbital Parameters
figure;
axis off;
text(0.05, 0.90, 'Main Orbital Parameters', 'FontSize', 16, 'FontWeight', 'bold');
text(0.05, 0.78, sprintf('Semimajor Axis: %.6e m', semimajor_axis), 'FontSize', 11);
text(0.05, 0.67, sprintf('Eccentricity: %.6f', eccentricity), 'FontSize', 11);
text(0.05, 0.56, sprintf('Orbital Period: %.6f days', orbital_period / seconds_per_day), 'FontSize', 11);
text(0.05, 0.45, sprintf('Periapsis Distance: %.6e m', periapsis_distance), 'FontSize', 11);
text(0.05, 0.34, sprintf('Apoapsis Distance: %.6e m', apoapsis_distance), 'FontSize', 11);
text(0.05, 0.23, sprintf('Star Mass: %.6e kg', star_mass), 'FontSize', 11);
text(0.05, 0.12, sprintf('Planet Mass: %.6e kg', planet_mass), 'FontSize', 11);
title('Orbital Parameter Summary');
