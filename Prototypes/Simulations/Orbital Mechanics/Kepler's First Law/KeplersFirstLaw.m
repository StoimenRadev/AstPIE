clear;
clc;
close all;

star_mass = input("Enter the mass of the star (kg): ");
planet_mass = input("Enter the mass of the planet (kg): ");

r0_planet = input("Enter the initial position [rx ry] (m): ");
v0_planet = input("Enter the initial velocity [vx vy] (m/s): ");

dt = input("Enter the time step (s): ");
T = input("Enter the total simulation duration (s): ");

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

% Ensure position and velocity use row-vector format
r0_planet = r0_planet(:).';
v0_planet = v0_planet(:).';

if ~isnumeric(dt) || ~isreal(dt) || ~isscalar(dt) || ~isfinite(dt) || dt <= 0
    error("Time step must be a finite, positive scalar.");
end

if ~isnumeric(T) || ~isreal(T) || ~isscalar(T) || ~isfinite(T) || T <= 0
    error("Simulation duration must be a finite, positive scalar.");
end

% Validate duration and time-step compatibility
step_ratio = T / dt;

if ~isfinite(step_ratio) || step_ratio < 1 || abs(step_ratio - round(step_ratio)) > 1e-10 * max(1, abs(step_ratio))
    error(["Simulation duration must be an integer multiple of the time step, with at least one integration step."]);
end

num_steps = round(step_ratio);
actual_duration = num_steps * dt;

G = 6.67430e-11; % Gravitational constant (m^3 kg^-1 s^-2)
mu_star = G * star_mass; % Gravitational parameter (m^3/s^2)

if ~isfinite(mu_star) || mu_star <= 0
    error("The gravitational parameter is invalid.");
end

% 10. Display simulation parameters
fprintf("\nSimulation Parameters \n");
fprintf("Star mass: %.3e kg\n", star_mass);
fprintf("Planet mass: %.3e kg\n", planet_mass);
fprintf("Gravitational parameter: %.3e m^3/s^2\n", mu_star);

fprintf("Initial position: [%.3e, %.3e] m\n", r0_planet(1), r0_planet(2));
fprintf("Initial velocity: [%.3f, %.3f] m/s\n", v0_planet(1), v0_planet(2));

fprintf("Time step: %.3f s\n", dt);
fprintf("Requested duration: %.3f s\n", T);
fprintf("Actual duration: %.3f s\n", actual_duration);
fprintf("Number of integration steps: %d\n", num_steps);

r_planet = r0_planet;
v_planet = v0_planet;

planet_positions = zeros(num_steps + 1, 2);
planet_positions(1, :) = r_planet;

% Calculate initial distance and speed
r0 = sqrt(r0_planet(1)^2 + r0_planet(2)^2);
v0_squared = v0_planet(1)^2 + v0_planet(2)^2;
if r0 == 0
    error("Initial position cannot be at the star.");
end

% Calculate specific orbital energy
specific_energy = v0_squared / 2 - mu_star / r0;
%
if specific_energy >= 0
    error("Initial conditions do not describe a bound elliptical orbit.");
end

% Calculate specific angular momentum (2D)
specific_angular_momentum = r0_planet(1) * v0_planet(2) - r0_planet(2) * v0_planet(1);
if specific_angular_momentum == 0
    error("Initial conditions describe a radial trajectory, not a regular ellipse.");
end


% Calculate eccentricity
eccentricity = sqrt(1 + (2 * specific_energy * specific_angular_momentum^2) / mu_star^2);
if eccentricity < 0 && eccentricity > -1e-12
    eccentricity = 0;
end

if ~isfinite(eccentricity) || eccentricity < 0 || eccentricity >= 1
    error("The calculated orbital eccentricity is invalid for an ellipse.");
end

% Calculate the eccentricity vector
% Calculate the dot product of initial position and velocity
position_velocity_dot = r0_planet(1) * v0_planet(1) + r0_planet(2) * v0_planet(2);

% Calculate the X component of the eccentricity vector
eccentricity_x = ((v0_squared - mu_star / r0) * r0_planet(1) - position_velocity_dot * v0_planet(1)) / mu_star;

% Calculate the Y component of the eccentricity vector
eccentricity_y = ((v0_squared - mu_star / r0) * r0_planet(2) - position_velocity_dot * v0_planet(2)) / mu_star;

% Calculate the direction of perihelion
if eccentricity < 1e-10
    perihelion_angle = 0; % Orientation is arbitrary for a circular orbit
else
    perihelion_angle = atan2(eccentricity_y, eccentricity_x);
end

% Calculate semimajor axis
semi_major_axis = -mu_star / (2 * specific_energy);

% Calculate semiminor axis
semi_minor_axis = semi_major_axis * sqrt(1 - eccentricity^2);

% Calculate focal distance from the ellipse centre
focal_distance = semi_major_axis * eccentricity;

% Calculate perihelion distance (closest point to the star)
perihelion_distance = semi_major_axis * (1 - eccentricity);

% Calculate aphelion distance (farthest point from the star)
aphelion_distance = semi_major_axis * (1 + eccentricity);

% Generate parameter angles for the theoretical ellipse
E = linspace(0, 2*pi, 361);

% Calculate theoretical ellipse coordinates in its local frame
x_ellipse_local = semi_major_axis * cos(E) - focal_distance;
y_ellipse_local = semi_minor_axis * sin(E);

% Rotate the theoretical ellipse to match the orbital orientation
x_ellipse = x_ellipse_local * cos(perihelion_angle)  - y_ellipse_local * sin(perihelion_angle);

y_ellipse = x_ellipse_local * sin(perihelion_angle)  + y_ellipse_local * cos(perihelion_angle);

% Numerical integration using semi-implicit Euler
for i = 1:num_steps

    % Distance from planet to the star (m)
    r_star = sqrt(r_planet(1)^2 + r_planet(2)^2);

    if ~isfinite(r_star) || r_star == 0
        error("The planet reached an invalid position at the star.");
    end

    % Gravitational acceleration vector (m/s^2)
    a_planet = -(mu_star / r_star^3) * r_planet;

    v_planet = v_planet + a_planet * dt;
    r_planet = r_planet + v_planet * dt;

    if any(~isfinite(r_planet)) || any(~isfinite(v_planet))
        error("The simulation produced invalid numerical values.");
    end

    planet_positions(i + 1, :) = r_planet;

end

fprintf("\nInitial State \n");
fprintf("Position X: %.3e m\n", r0_planet(1));
fprintf("Position Y: %.3e m\n", r0_planet(2));
fprintf("Velocity X: %.3f m/s\n", v0_planet(1));
fprintf("Velocity Y: %.3f m/s\n", v0_planet(2));

fprintf("\nFinal State \n");
fprintf("Position X: %.3e m\n", r_planet(1));
fprintf("Position Y: %.3e m\n", r_planet(2));
fprintf("Velocity X: %.3f m/s\n", v_planet(1));
fprintf("Velocity Y: %.3f m/s\n", v_planet(2));

fprintf("\nOrbital Elements\n");
fprintf("Specific orbital energy: %.3e m^2/s^2\n", specific_energy);
fprintf("Specific angular momentum: %.3e m^2/s\n", specific_angular_momentum);
fprintf("Semimajor axis: %.3e m\n", semi_major_axis);
fprintf("Eccentricity: %.3f\n", eccentricity);

fprintf("\nELLIPSE GEOMETRY\n");
fprintf("Semimajor axis: %.3e m\n", semi_major_axis);
fprintf("Semiminor axis: %.3e m\n", semi_minor_axis);
fprintf("Focal distance: %.3e m\n", focal_distance);
fprintf("Perihelion distance: %.3e m\n", perihelion_distance);
fprintf("Aphelion distance: %.3e m\n", aphelion_distance);

fprintf("Ellipse X range: %.3e to %.3e m\n", min(x_ellipse), max(x_ellipse));
fprintf("Ellipse Y range: %.3e to %.3e m\n", min(y_ellipse), max(y_ellipse));

figure;
plot(planet_positions(:, 1), planet_positions(:, 2), 'b-');
hold on;

% Plot the theoretical ellipse
plot(x_ellipse, y_ellipse, 'k--');

plot(0, 0, 'yo', 'MarkerFaceColor', 'y', 'MarkerSize', 10);
plot(r0_planet(1), r0_planet(2), 'go', 'MarkerFaceColor', 'g');
plot(r_planet(1), r_planet(2), 'ro', 'MarkerFaceColor', 'r');

axis equal;
grid on;
xlabel("X Position (m)");
ylabel("Y Position (m)");
title("Planet Orbit Around a Star");

legend("Numerical Trajectory", "Theoretical Ellipse",  "Star (Focus)", "Starting Position", "Ending Position");
