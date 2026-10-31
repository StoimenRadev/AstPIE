clear;
clc;
close all;

% Physical Constants
G = 6.67430e-11;       % Gravitational constant (m^3 kg^-1 s^-2)
AU = 149597870700;     % Astronomical unit (m)
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

% Convert vectors into row vectors
r0_planet = r0_planet(:).';
v0_planet = v0_planet(:).';

% Initial distance and speed
r0_distance = sqrt(r0_planet(1)^2 + r0_planet(2)^2);
v0_speed = sqrt(v0_planet(1)^2 + v0_planet(2)^2);

if r0_distance == 0
    error("Initial position must be away from the central body.");
end

total_mass = star_mass + planet_mass;

mu = G * total_mass; % Gravitational parameter (m^3/s^2)

% Calculating Specific energy
specific_orbital_energy = (v0_speed^2 / 2) - (mu / r0_distance);

% Calculating specific angular momentum
h0 = r0_planet(1) * v0_planet(2) - r0_planet(2) * v0_planet(1);

h0_magnitude = abs(h0);

if h0_magnitude == 0
    error("Initial conditions must produce nonzero specific angular momentum.");
end

% Clculating eccentricity
eccentricity_squared = 1 + (2 * specific_orbital_energy * h0_magnitude^2) / mu^2;

if eccentricity_squared < 0
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

% Calculating Semi-major Axis and Orbital period
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

% Initialize Position and Velocity
r_planet = r0_planet;
v_planet = v0_planet;

planet_positions = zeros(num_steps + 1, 2);
planet_velocities = zeros(num_steps + 1, 2);
time_history = zeros(num_steps + 1, 1);

planet_positions(1, :) = r_planet;
planet_velocities(1, :) = v_planet;

% Semi-Implicit Euler Integration
for i = 1:num_steps

    % Calculate the current time step
    current_dt = dt;

    % Calculate the current distance
    r_distance = sqrt(r_planet(1)^2 + r_planet(2)^2);

    if r_distance == 0
        error("Planet reached the central body's position during integration.");
    end

    % Calculate gravitational acceleration
    acceleration = -(mu / r_distance^3) * r_planet;

    % Update velocity first
    v_planet = v_planet + acceleration * current_dt;

    % Update position using the new velocity
    r_planet = r_planet + v_planet * current_dt;

    % Store the updated state
    planet_positions(i + 1, :) = r_planet;
    planet_velocities(i + 1, :) = v_planet;

    % Store the current time
    time_history(i + 1) = time_history(i) + current_dt;

end

% Final State Calculations
final_distance = sqrt(r_planet(1)^2 + r_planet(2)^2);
final_speed = sqrt(v_planet(1)^2 + v_planet(2)^2);
final_specific_energy = (final_speed^2 / 2) - (mu / final_distance);
final_specific_angular_momentum = r_planet(1) * v_planet(2) - r_planet(2) * v_planet(1);
energy_difference = final_specific_energy - specific_orbital_energy;
angular_momentum_difference = final_specific_angular_momentum - h0;

% Simulation results
fprintf("\nInput Parameters\n");
fprintf("Central body mass: %.6e kg\n", star_mass);
fprintf("Orbiting body mass: %.6e kg\n", planet_mass);
fprintf("Total system mass: %.6e kg\n", total_mass);
fprintf("Initial position: [%.6e, %.6e] m\n", r0_planet(1), r0_planet(2));
fprintf("Initial velocity: [%.6e, %.6e] m/s\n", v0_planet(1), v0_planet(2));
fprintf("Initial distance: %.6e m\n", r0_distance);
fprintf("Initial speed: %.6e m/s\n", v0_speed);
fprintf("Gravitational parameter: %.6e m^3/s^2\n", mu);

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

fprintf("\nNumerical Integration Parameters\n");
fprintf("Integration method: Semi-implicit Euler\n");
fprintf("Time step: %.6f s\n", dt);
fprintf("Requested integration duration: %.6f s\n", T);
fprintf("Number of integration steps: %d\n", num_steps);
fprintf("Actual integration duration: %.6f s\n", time_history(end));

fprintf("\nFinal Numerical State\n");
fprintf("Final position: [%.6e, %.6e] m\n", r_planet(1), r_planet(2));
fprintf("Final velocity: [%.6e, %.6e] m/s\n", v_planet(1), v_planet(2));
fprintf("Final distance: %.6e m\n", final_distance);
fprintf("Final speed: %.6e m/s\n", final_speed);

fprintf("\nNumerical Conservation Checks\n");
fprintf("Initial specific orbital energy: %.6e m^2/s^2\n", specific_orbital_energy);
fprintf("Final specific orbital energy: %.6e m^2/s^2\n", final_specific_energy);
fprintf("Energy difference: %.6e m^2/s^2\n", energy_difference);
fprintf("Initial specific angular momentum: %.6e m^2/s\n", h0);
fprintf("Final specific angular momentum: %.6e m^2/s\n", final_specific_angular_momentum);
fprintf("Angular momentum difference: %.6e m^2/s\n", angular_momentum_difference);

% PLoting the orbit
figure;
plot(planet_positions(:, 1), planet_positions(:, 2), 'b-');
hold on;
plot(0, 0, 'yo', 'MarkerFaceColor', 'y', 'MarkerSize', 12);
plot(r0_planet(1), r0_planet(2), 'go', 'MarkerFaceColor', 'g');
plot(r_planet(1), r_planet(2), 'ro', 'MarkerFaceColor', 'r');
xlabel('X Position (m)');
ylabel('Y Position (m)');
title('Planetary Orbit - Kepler''s Third Law');
legend('Numerical Trajectory', 'Central Body', 'Initial Position', 'Final Position');
axis equal;
grid on;
