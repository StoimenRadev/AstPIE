
clear;
clc;
close all;

G = 6.67430e-11;
AU = 149597870700;
seconds_per_day = 86400;
days_per_julian_year = 365.25;

star_mass = input("Enter the mass of the central body (kg): ");

if ~isscalar(star_mass) || ~isnumeric(star_mass) || ~isreal(star_mass) || ~isfinite(star_mass) || star_mass <= 0
    error("Invalid input: Central body mass must be a positive, finite real number.");
end

planet_mass = input("Enter the mass of the orbiting body (kg): ");

if ~isscalar(planet_mass) || ~isnumeric(planet_mass) || ~isreal(planet_mass) || ~isfinite(planet_mass) || planet_mass <= 0
    error("Invalid input: Orbiting body mass must be a positive, finite real number.");
end

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

% Initial distance and speed
r0_distance = sqrt(r0_planet(1)^2 + r0_planet(2)^2);
v0_speed = sqrt(v0_planet(1)^2 + v0_planet(2)^2);

if ~isfinite(r0_distance) || r0_distance == 0
    error("Initial position must be a finite, nonzero distance from the central body.");
end

total_mass = star_mass + planet_mass;
mu = G * total_mass;

% Calculating Initial Specific Orbital Energy
specific_orbital_energy = (v0_speed^2 / 2) - (mu / r0_distance);

% Calculating Initial Specific Angular Momentum
h0 = r0_planet(1) * v0_planet(2) - r0_planet(2) * v0_planet(1);

if ~isfinite(h0) || h0 == 0
    error("Initial conditions must produce finite, nonzero specific angular momentum.");
end

h0_magnitude = abs(h0);

% Calculating Orbital Eccentricity
eccentricity_squared = 1 + (2 * specific_orbital_energy * h0_magnitude^2) / mu^2;

if ~isfinite(eccentricity_squared) || eccentricity_squared < 0
    error("Invalid orbital eccentricity calculation.");
end

eccentricity = sqrt(eccentricity_squared);

% Orbital Classification
if specific_orbital_energy < 0

    orbital_type = "Elliptical";

elseif specific_orbital_energy == 0

    orbital_type = "Parabolic";

else

    orbital_type = "Hyperbolic";

end

% Calculating Semi-major axis and Orbital period
if specific_orbital_energy < 0

    semi_major = -mu / (2 * specific_orbital_energy);
    orbital_period = 2 * pi * sqrt(semi_major^3 / mu);
    period_days = orbital_period / seconds_per_day;
    period_years = period_days / days_per_julian_year;
    semi_major_AU = semi_major / AU;

end

dt = input("Enter the time step (s): ");

if ~isnumeric(dt) || ~isreal(dt) || ~isscalar(dt) || ~isfinite(dt) || dt <= 0
    error("Time step must be a positive, finite real number.");
end

T = input("Enter the total integration time (s): ");

if ~isnumeric(T) || ~isreal(T) || ~isscalar(T) || ~isfinite(T) || T <= 0
    error("Integration duration must be a positive, finite real number.");
end

num_steps = round(T / dt);

if num_steps < 1
    error("Integration duration must allow at least one integration step.");
end

actual_duration = num_steps * dt;

% Initialize Position and Velocity
r_planet = r0_planet;
v_planet = v0_planet;

time_history = (0:num_steps)' * dt;

% Initialize Time-History Arrays
planet_positions = zeros(num_steps + 1, 2);
planet_velocities = zeros(num_steps + 1, 2);
planet_accelerations = zeros(num_steps + 1, 2);
planet_speed = zeros(num_steps + 1, 1);
planet_distance = zeros(num_steps + 1, 1);
specific_orbital_energy_history = zeros(num_steps + 1, 1);
specific_angular_momentum_history = zeros(num_steps + 1, 1);

% Store Initial State
planet_positions(1, :) = r_planet;
planet_velocities(1, :) = v_planet;
planet_distance(1) = r0_distance;
planet_speed(1) = v0_speed;
planet_accelerations(1, :) = -(mu / r0_distance^3) * r0_planet;
specific_orbital_energy_history(1) = specific_orbital_energy;
specific_angular_momentum_history(1) = h0;

% Semi-Implicit Euler Integration
for i = 1:num_steps

    % Current distance from the central body
    r_current = sqrt(r_planet(1)^2 + r_planet(2)^2);

    if ~isfinite(r_current) || r_current == 0
        error("The planet reached an invalid position during integration.");
    end

    % Calculate gravitational acceleration
    acceleration = -(mu / r_current^3) * r_planet;

    if any(~isfinite(acceleration))
        error("The integration produced invalid acceleration values.");
    end

    % Update velocity first
    v_planet = v_planet + acceleration * dt;

    % Update position using the updated velocity
    r_planet = r_planet + v_planet * dt;

    % Validate the updated state
    if any(~isfinite(r_planet)) || any(~isfinite(v_planet))
        error("The integration produced invalid position or velocity values.");
    end

    % Calculate the new distance
    r_new_distance = sqrt(r_planet(1)^2 + r_planet(2)^2);

    if ~isfinite(r_new_distance) || r_new_distance == 0
        error("The planet reached an invalid position at the central body.");
    end

    % Calculate the new speed
    new_speed = sqrt(v_planet(1)^2 + v_planet(2)^2);

    % Calculate acceleration at the new position
    new_acceleration = -(mu / r_new_distance^3) * r_planet;

    if any(~isfinite(new_acceleration))
        error("The integration produced invalid acceleration values.");
    end

    % Calculate specific orbital energy
    new_specific_energy = (new_speed^2 / 2) - (mu / r_new_distance);

    % Calculate specific angular momentum
    new_h = r_planet(1) * v_planet(2) - r_planet(2) * v_planet(1);

    % Store updated position
    planet_positions(i + 1, :) = r_planet;

    % Store updated velocity
    planet_velocities(i + 1, :) = v_planet;

    % Store updated acceleration
    planet_accelerations(i + 1, :) = new_acceleration;

    % Store speed and distance
    planet_speed(i + 1) = new_speed;
    planet_distance(i + 1) = r_new_distance;

    % Store orbital quantities
    specific_orbital_energy_history(i + 1) = new_specific_energy;
    specific_angular_momentum_history(i + 1) = new_h;

end

% Analytical Validation
% Theoretical Kepler's Third Law coefficient
theoretical_coefficient = (4 * pi^2) / mu;

if specific_orbital_energy < 0

    % Calculate coefficient from orbital period and semi-major axis
    calculated_coefficient = orbital_period^2 / semi_major^3;

    % Calculate the period using the theoretical coefficient
    coefficient_period = sqrt(theoretical_coefficient * semi_major^3);

    % Calculate the Kepler relationship residual
    kepler_residual = orbital_period^2 - theoretical_coefficient * semi_major^3;

    % Calculate coefficient errors
    coefficient_absolute_error = abs(calculated_coefficient - theoretical_coefficient);
    coefficient_relative_error = coefficient_absolute_error / abs(theoretical_coefficient);

    % Calculate period residual errors
    period_residual_absolute = abs(orbital_period - coefficient_period);
    period_residual_relative = period_residual_absolute / abs(orbital_period);

end

% Numerical Conservation Analysis
initial_specific_energy = specific_orbital_energy_history(1);
final_specific_energy = specific_orbital_energy_history(end);
initial_specific_angular_momentum = specific_angular_momentum_history(1);
final_specific_angular_momentum = specific_angular_momentum_history(end);

energy_absolute_difference = abs(final_specific_energy - initial_specific_energy);
angular_momentum_absolute_difference = abs(final_specific_angular_momentum - initial_specific_angular_momentum);

angular_momentum_relative_error = angular_momentum_absolute_difference / abs(initial_specific_angular_momentum);
angular_momentum_error_percentage = angular_momentum_relative_error * 100;

% Relative energy error is undefined when initial energy is zero
if initial_specific_energy ~= 0

    energy_relative_error = energy_absolute_difference / abs(initial_specific_energy);
    energy_error_percentage = energy_relative_error * 100;

end

% Kepler's Third Law Visualization
if specific_orbital_energy < 0

    % Number of generated semi-major-axis values
    number_of_graph_points = 100;

    % Generate semi-major-axis range
    minimum_semi_major_axis = 0.5 * semi_major;
    maximum_semi_major_axis = 2.0 * semi_major;
    semi_major_axis_values = linspace( minimum_semi_major_axis, maximum_semi_major_axis, number_of_graph_points);

    % Calculate orbital periods for generated semi-major-axis values
    orbital_period_values = 2 * pi * sqrt(semi_major_axis_values.^3 / mu);

    % Calculate squared orbital periods
    squared_period_values = orbital_period_values.^2;

    % Calculate cubed semi-major-axis values
    cubed_semi_major_axis_values = semi_major_axis_values.^3;

    % Calculate reference values
    reference_semi_major_axis = semi_major;
    reference_orbital_period = orbital_period;
    reference_squared_period = orbital_period^2;
    reference_cubed_semi_major_axis = semi_major^3;

    % Check generated visualization values
    if any(~isfinite(semi_major_axis_values)) || ...
       any(~isfinite(orbital_period_values)) || ...
       any(~isfinite(squared_period_values)) || ...
       any(~isfinite(cubed_semi_major_axis_values))

        error("The visualization generated invalid numerical values.");

    end

    % Store generated visualization data
    visualization_results = [ ...
        semi_major_axis_values(:), ...
        orbital_period_values(:), ...
        cubed_semi_major_axis_values(:), ...
        squared_period_values(:) ...
    ];

end

% Display Input Parameters
fprintf("\nInput Parameters\n");
fprintf("Central body mass: %.6e kg\n", star_mass);
fprintf("Orbiting body mass: %.6e kg\n", planet_mass);
fprintf("Total system mass: %.6e kg\n", total_mass);
fprintf("Initial position: [%.6e, %.6e] m\n", r0_planet(1), r0_planet(2));
fprintf("Initial velocity: [%.6e, %.6e] m/s\n", v0_planet(1), v0_planet(2));
fprintf("Initial distance: %.6e m\n", r0_distance);
fprintf("Initial speed: %.6e m/s\n", v0_speed);
fprintf("Gravitational parameter: %.6e m^3/s^2\n", mu);

% Display Analytical Orbital Elements
fprintf("\nAnalytical Orbital Elements\n");
fprintf("Specific orbital energy: %.6e m^2/s^2\n", specific_orbital_energy);
fprintf("Specific angular momentum: %.6e m^2/s\n", h0_magnitude);
fprintf("Orbital eccentricity: %.8f\n", eccentricity);
fprintf("Orbital classification: %s\n", orbital_type);

if specific_orbital_energy < 0

    fprintf("Semi-major axis: %.6e m\n", semi_major);
    fprintf("Semi-major axis: %.8f AU\n", semi_major_AU);
    fprintf("Orbital period: %.6f s\n", orbital_period);
    fprintf("Orbital period: %.6f days\n", period_days);
    fprintf("Orbital period: %.6f Julian years\n", period_years);

else

    fprintf("No finite orbital period for this trajectory.\n");

end

% Display Analytical Validation
fprintf("\nAnalytical Validation\n");
fprintf("Theoretical Kepler coefficient: %.12e s^2/m^3\n", theoretical_coefficient);

if specific_orbital_energy < 0

    fprintf("Calculated Kepler coefficient: %.12e s^2/m^3\n", calculated_coefficient);
    fprintf("Kepler relationship residual: %.6e s^2\n", kepler_residual);
    fprintf("Coefficient absolute error: %.6e s^2/m^3\n", coefficient_absolute_error);
    fprintf("Coefficient relative error: %.6e %%\n", coefficient_relative_error * 100);
    fprintf("Period residual absolute error: %.6e s\n", period_residual_absolute);
    fprintf("Period residual relative error: %.6e %%\n", period_residual_relative * 100);

else

    fprintf("Kepler's Third Law period validation is not applicable to an unbound orbit.\n");

end

% Display Numerical Integration Parameters
fprintf("\nNumerical Integration Parameters\n");
fprintf("Integration method: Semi-implicit Euler\n");
fprintf("Time step: %.6f s\n", dt);
fprintf("Requested integration duration: %.6f s\n", T);
fprintf("Actual integration duration: %.6f s\n", actual_duration);
fprintf("Number of integration steps: %d\n", num_steps);
fprintf("Number of recorded states: %d\n", num_steps + 1);

% Display Final Numerical State
fprintf("\nFinal Numerical State\n");
fprintf("Final position: [%.6e, %.6e] m\n", r_planet(1), r_planet(2));
fprintf("Final velocity: [%.6e, %.6e] m/s\n", v_planet(1), v_planet(2));
fprintf("Final distance: %.6e m\n", planet_distance(end));
fprintf("Final speed: %.6e m/s\n", planet_speed(end));

% Display Numerical Conservation Analysis
fprintf("\nNumerical Conservation Analysis\n");
fprintf("Initial specific orbital energy: %.6e m^2/s^2\n", initial_specific_energy);
fprintf("Final specific orbital energy: %.6e m^2/s^2\n", final_specific_energy);
fprintf("Absolute energy difference: %.6e m^2/s^2\n", energy_absolute_difference);

if initial_specific_energy ~= 0

    fprintf("Relative energy error: %.6e %%\n", energy_error_percentage);

else

    fprintf("Relative energy error: Undefined (initial energy is zero).\n");

end

fprintf("Initial specific angular momentum: %.6e m^2/s\n", initial_specific_angular_momentum);
fprintf("Final specific angular momentum: %.6e m^2/s\n", final_specific_angular_momentum);
fprintf("Absolute angular momentum difference: %.6e m^2/s\n", angular_momentum_absolute_difference);
fprintf("Relative angular momentum error: %.6e %%\n", angular_momentum_error_percentage);

results = [ ...
    time_history, ...
    planet_positions, ...
    planet_velocities, ...
    planet_speed, ...
    planet_accelerations, ...
    planet_distance, ...
    specific_orbital_energy_history, ...
    specific_angular_momentum_history ...
];

% Display Complete Time History
fprintf("\nComplete Time History\n");
fprintf(["Columns: time(s), x(m), y(m), vx(m/s), vy(m/s), ", "speed(m/s), ax(m/s^2), ay(m/s^2), distance(m), ", "specific_energy(m^2/s^2), specific_angular_momentum(m^2/s)\n"]);
disp(results);

% Plot Numerical Orbit
figure;
plot(planet_positions(:, 1), planet_positions(:, 2), 'b-');
hold on;
plot(0, 0, 'yo', 'MarkerFaceColor', 'y', 'MarkerSize', 12);
plot(r0_planet(1), r0_planet(2), 'go', 'MarkerFaceColor', 'g');
plot(r_planet(1), r_planet(2), 'ro', 'MarkerFaceColor', 'r');
xlabel('X Position (m)');
ylabel('Y Position (m)');
title('Planetary Orbit');
legend('Numerical Trajectory', 'Central Body', 'Initial Position', 'Final Position');
axis equal;
grid on;

% Display Visualization Reference Case
fprintf("\nKepler's Third Law Visualization\n");
fprintf("Number of generated semi-major-axis values: %d\n", number_of_graph_points);
fprintf("Minimum semi-major axis: %.6e m\n", minimum_semi_major_axis);
fprintf("Maximum semi-major axis: %.6e m\n", maximum_semi_major_axis);

fprintf("\nReference Case\n");
fprintf("Reference semi-major axis: %.6e m\n", reference_semi_major_axis);
fprintf("Reference orbital period: %.6e s\n", reference_orbital_period);
fprintf("Reference cubed semi-major axis: %.6e m^3\n", reference_cubed_semi_major_axis);
fprintf("Reference squared orbital period: %.6e s^2\n", reference_squared_period);

% Display Generated Visualization Data
fprintf("\nGenerated Visualization Data\n");
fprintf(["Columns: semi_major_axis(m), orbital_period(s), ", "semi_major_axis_cubed(m^3), orbital_period_squared(s^2)\n"]);
disp(visualization_results);

% Plot Star–planet distance against time
figure;
plot(time_history / seconds_per_day, planet_distance / AU, 'b-', 'LineWidth', 1.2);
xlabel('Time (days)');
ylabel('Star-Planet Distance (AU)');
title('Star-Planet Distance vs Time');
legend('Planet Distance');
grid on;

% Plot Planet speed against time
figure;
plot(time_history / seconds_per_day, planet_speed, 'r-', 'LineWidth', 1.2);
xlabel('Time (days)');
ylabel('Planet Speed (m/s)');
title('Planet Speed vs Time');
legend('Planet Speed');
grid on;

% Plot X and Y (Position) coordinates against time
figure;
subplot(2, 1, 1);
plot(time_history / seconds_per_day, planet_positions(:, 1) / AU, 'b-', 'LineWidth', 1.2);
xlabel('Time (days)');
ylabel('X-Position (AU)');
title('X-Position vs Time');
legend('X-Position');
grid on;
subplot(2, 1, 2);
plot(time_history / seconds_per_day, planet_positions(:, 2) / AU, 'r-', 'LineWidth', 1.2);
xlabel('Time (days)');
ylabel('Y-Position (AU)');
title('Y-Position vs Time');
legend('Y-Position');
grid on;

% Plot Orbital Period Against Semi-Major Axis
figure;
plot(semi_major_axis_values, orbital_period_values, 'b-', 'LineWidth', 1.5);
hold on;
plot(reference_semi_major_axis, reference_orbital_period, 'ro', 'MarkerFaceColor', 'r', 'MarkerSize', 8);
xlabel('Semi-Major Axis (m)');
ylabel('Orbital Period (s)');
title('Kepler''s Third Law - Orbital Period vs Semi-Major Axis');
legend('Calculated Orbital Periods', 'Reference Case', 'Location', 'northwest');
grid on;

% Plot Squared Orbital Period Against Cubed Semi-Major Axis
figure;
plot(cubed_semi_major_axis_values, squared_period_values, 'b-', 'LineWidth', 1.5);
hold on;
plot(reference_cubed_semi_major_axis, reference_squared_period, 'ro', 'MarkerFaceColor', 'r', 'MarkerSize', 8);
xlabel('Semi-Major Axis Cubed (m^3)');
ylabel('Orbital Period Squared (s^2)');
title('Kepler''s Third Law - Squared Period vs Cubed Semi-Major Axis');
legend('Calculated Orbital Values', 'Reference Case', 'Location', 'northwest');
grid on;

% Calculate Perihelion and Aphelion Distances
perihelion_distance = semi_major * (1 - eccentricity);
aphelion_distance = semi_major * (1 + eccentricity);

% Display Main Orbital Parameters
figure;
axis off;
text(0.05, 0.90, 'Main Orbital Parameters', 'FontSize', 16, 'FontWeight', 'bold');
text(0.05, 0.78, sprintf('Semi-Major Axis: %.6e m', semi_major), 'FontSize', 11);
text(0.05, 0.67, sprintf('Eccentricity: %.6f', eccentricity), 'FontSize', 11);
text(0.05, 0.56, sprintf('Perihelion Distance: %.6e m', perihelion_distance), 'FontSize', 11);
text(0.05, 0.45, sprintf('Aphelion Distance: %.6e m', aphelion_distance), 'FontSize', 11);
text(0.05, 0.34, sprintf('Orbital Period: %.6e s', orbital_period), 'FontSize', 11);
text(0.05, 0.23, sprintf('Orbital Period: %.6f days', period_days), 'FontSize', 11);
text(0.05, 0.12, sprintf('Gravitational Parameter: %.6e m^3/s^2', mu),'FontSize', 11);
title('Orbital Parameter Summary');
