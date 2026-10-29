clear;
clc;
close all;

star_mass = input("Enter the mass of the star (kg): ");
G = 6.67430e-11; % Gravitational constant (m^3 kg^-1 s^-2)

if ~isnumeric(star_mass) || ~isreal(star_mass) || ~isscalar(star_mass) || ~isfinite(star_mass) || star_mass <= 0
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

% Validate initial distance from the star
r0_distance = sqrt(r0_planet(1)^2 + r0_planet(2)^2);

if r0_distance == 0
    error("Initial position must be away from the star.");
end

% Calculate and validate initial specific angular momentum (m^2/s)
h0 = r0_planet(1) * v0_planet(2) - r0_planet(2) * v0_planet(1);

if h0 == 0
    error("Initial conditions must produce nonzero specific angular momentum.");
end

dt = input("Enter the time step (s): ");
T = input("Enter the total simulation time (s): ");

if ~isnumeric(dt) || ~isreal(dt) || ~isscalar(dt) || ~isfinite(dt) || dt <= 0
    error("Time step must be a finite, positive scalar.");
end

if ~isnumeric(T) || ~isreal(T) || ~isscalar(T) || ~isfinite(T) || T <= 0
    error("Simulation duration must be a finite, positive scalar.");
end

% Validate duration and timestep compatibility
step_ratio = T / dt;

if ~isfinite(step_ratio) || step_ratio < 1
    error("Simulation duration must allow at least one integration step.");
end

nearest_step_count = round(step_ratio);
tolerance = 1e-10 * max(1, abs(step_ratio));

if abs(step_ratio - nearest_step_count) > tolerance
    error("Simulation duration must be an integer multiple of the time step.");
end

num_steps = nearest_step_count;
actual_duration = num_steps * dt;

% Initialize state variables and history arrays
r_planet = r0_planet;
v_planet = v0_planet;

% Store positions (m)
planet_positions = zeros(num_steps + 1, 2);
planet_positions(1, :) = r_planet;

% Store velocities (m/s)
planet_velocities = zeros(num_steps + 1, 2);
planet_velocities(1, :) = v_planet;

% Store speed (m/s)
planet_speed = zeros(num_steps + 1, 1);
planet_speed(1) = sqrt(v_planet(1)^2 + v_planet(2)^2);

% Store specific angular momentum (m^2/s)
specific_angular_momentum = zeros(num_steps + 1, 1);
specific_angular_momentum(1) = h0;

% Store areal velocity (m^2/s)
areal_velocity = zeros(num_steps + 1, 1);
areal_velocity(1) = h0 / 2;

% Display simulation parameters

fprintf('\nSimulation Parameters\n');
fprintf('Star mass: %.6e kg\n', star_mass);
fprintf('Gravitational constant: %.6e m^3 kg^-1 s^-2\n', G);
fprintf('Gravitational parameter: %.6e m^3/s^2\n', mu_star);
fprintf('Initial distance: %.6e m\n', r0_distance);
fprintf('Initial position: [%.6e, %.6e] m\n', r0_planet(1), r0_planet(2));
fprintf('Initial velocity: [%.6e, %.6e] m/s\n', v0_planet(1), v0_planet(2));
fprintf('Initial speed: %.6f m/s\n', planet_speed(1));
fprintf('Initial specific angular momentum: %.6e m^2/s\n', h0);
fprintf('Initial areal velocity: %.6e m^2/s\n', areal_velocity(1));
fprintf('Timestep: %.6f s\n', dt);
fprintf('Requested duration: %.6f s\n', T);
fprintf('Actual duration: %.6f s\n', actual_duration);
fprintf('Number of integration steps: %d\n', num_steps);

% Integrate the orbit using semi-implicit Euler
for i = 1:num_steps

    % Current distance from the star (m)
    r_star = sqrt(r_planet(1)^2 + r_planet(2)^2);

    % Check for collision with the star's position
    if r_star == 0
        error("Planet reached the star's position during integration.");
    end

    % Gravitational acceleration (m/s^2)
    a_planet = -(mu_star / r_star^3) * r_planet;

    % Update velocity first (m/s)
    v_planet = v_planet + a_planet * dt;

    % Update position using the new velocity (m)
    r_planet = r_planet + v_planet * dt;

    % Store position
    planet_positions(i + 1, :) = r_planet;

    % Store velocity
    planet_velocities(i + 1, :) = v_planet;

    % Store speed
    planet_speed(i + 1) = sqrt(v_planet(1)^2 + v_planet(2)^2);

    % Store specific angular momentum (m^2/s)
    specific_angular_momentum(i + 1) = ...
        r_planet(1) * v_planet(2) - r_planet(2) * v_planet(1);

    % Store areal velocity (m^2/s)
    areal_velocity(i + 1) = specific_angular_momentum(i + 1) / 2;

end

% Plot the orbit
figure;
plot(planet_positions(:, 1), planet_positions(:, 2), 'b-');
hold on;
plot(0, 0, 'yo', 'MarkerFaceColor', 'y', 'MarkerSize', 12);
xlabel('x Position (m)');
ylabel('y Position (m)');
title('Planet Orbit - Kepler''s Second Law');
legend('Planet Trajectory', 'Star');
axis equal;
grid on;

% Display initial and final states
fprintf('\nInitial State\n');
fprintf('Position: [%.6e, %.6e] m\n', r0_planet(1), r0_planet(2));
fprintf('Velocity: [%.6e, %.6e] m/s\n', v0_planet(1), v0_planet(2));
fprintf('Speed: %.6f m/s\n', planet_speed(1));

fprintf('\nFinal State\n');
fprintf('Position: [%.6e, %.6e] m\n', planet_positions(end, 1), planet_positions(end, 2));
fprintf('Velocity: [%.6e, %.6e] m/s\n', planet_velocities(end, 1), planet_velocities(end, 2));
fprintf('Speed: %.6f m/s\n', planet_speed(end));

% Check Kepler's Second Law
fprintf('\nKepler''s Second Law Checks\n');

fprintf('\nInitial State:\n');
fprintf('Specific angular momentum: %.6e m^2/s\n', specific_angular_momentum(1));
fprintf('Areal velocity: %.6e m^2/s\n', areal_velocity(1));

fprintf('\nFinal State:\n');
fprintf('Specific angular momentum: %.6e m^2/s\n', specific_angular_momentum(end));
fprintf('Areal velocity: %.6e m^2/s\n', areal_velocity(end));

% Calculate absolute differences
h_difference = specific_angular_momentum(end) - specific_angular_momentum(1);

if h_difference < 0
    h_difference = -h_difference;
end

area_rate_difference = areal_velocity(end) - areal_velocity(1);

if area_rate_difference < 0
    area_rate_difference = -area_rate_difference;
end

% Calculate relative changes (%)
h_change = h_difference / abs(specific_angular_momentum(1)) * 100;
area_rate_change = area_rate_difference / abs(areal_velocity(1)) * 100;

fprintf('\nAbsolute difference in specific angular momentum: %.6e m^2/s\n', h_difference);
fprintf('Relative change in specific angular momentum: %.6e %%\n', h_change);

fprintf('Absolute difference in areal velocity: %.6e m^2/s\n', area_rate_difference);
fprintf('Relative change in areal velocity: %.6e %%\n', area_rate_change);
