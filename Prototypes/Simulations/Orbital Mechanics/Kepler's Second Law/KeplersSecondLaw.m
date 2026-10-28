clear;
clc;
close all;

sun_mass = 1.98847e30; % Sun's mass (kg)
G = 6.67430e-11; % Gravitational constant (m^3 kg^-1 s^-2)

mu_sun = G * sun_mass; % Sun's gravitational parameter (m^3/s^2)

r0_earth = [1.496e11, 0];    % Initial position (m)
v0_earth = [0, 29780];       % Initial velocity (m/s)

dt = 3600;
T = 31536000;
num_steps = round(T / dt);

r_earth = r0_earth;
v_earth = v0_earth;

% Store Earth's positions at every time step (m/s)
earth_positions = zeros(num_steps + 1, 2);
earth_positions(1, :) = r_earth;

% Store Earth's velocity at every time step (m/s)
earth_velocities = zeros(num_steps + 1, 2);
earth_velocities(1, :) = v_earth;

% Calculate and store the initial speed (m/s)
earth_speed = zeros(num_steps + 1, 1);
earth_speed(1) = sqrt(v_earth(1)^2 + v_earth(2)^2);

% Calculate and store the specific angular momentum(m^2/s)
specific_angular_momentum = zeros(num_steps + 1, 1);
specific_angular_momentum(1) = r0_earth(1) * v0_earth(2) - r0_earth(2) * v0_earth(1);

% Calculate and store areal velocity (m^2/s)
areal_velocity = zeros(num_steps + 1, 1);
areal_velocity(1) = specific_angular_momentum(1) / 2;

for i = 1:num_steps

   % Distance from Earth to the Sun (m)
    r_sun = sqrt(r_earth(1)^2 + r_earth(2)^2);

    % Gravitational acceleration vector (m/s^2)
    a_earth = -(mu_sun / r_sun^3) * r_earth;

    % Update velocity first (m/s)
    v_earth = v_earth + a_earth * dt;

    % Update position using the new velocity (m)
    r_earth = r_earth + v_earth * dt;

    % Store the new position
    earth_positions(i + 1, :) = r_earth;

    % Store the updated velocity
    earth_velocities(i + 1, :) = v_earth;

    % Calculate and store the current speed (m/s)
    earth_speed(i + 1) = sqrt(v_earth(1)^2 + v_earth(2)^2);

    % Calculate and store the specific angular momentum (m^2/s)
    specific_angular_momentum(i + 1) = r_earth(1) * v_earth(2) - r_earth(2) * v_earth(1);

    % Calculate and store areal velocity (m^2/s)
    areal_velocity(i + 1) = specific_angular_momentum(i + 1) / 2;

end

% Plot Earth's trajectory
figure;
plot(earth_positions(:, 1), earth_positions(:, 2), 'b-');
hold on;

% Plot the Sun at the origin
plot(0, 0, 'yo', 'MarkerFaceColor', 'y', 'MarkerSize', 12);

% Label the axes
xlabel('x Position (m)');
ylabel('y Position (m)');

% Add title and legend
title('Earth Orbit - Kepler''s Second Law');
legend('Earth Trajectory', 'Sun');

% Keep the axes equally scaled
axis equal;
grid on;

% Display initial conditions
fprintf('\nInitial State\n');
fprintf('Position: [%.6e,  %.6e] m\n', r0_earth(1), r0_earth(2));
fprintf('Velocity: [%.6e m/s,  %.6e] m/s\n', v0_earth(1), v0_earth(2));
fprintf('Speed: %.6f m/s\n', earth_speed(1));

% Display final state
fprintf('\nFinal State\n');
fprintf('Position: [%.6e, %.6e] m\n', earth_positions(end, 1), earth_positions(end, 2));
fprintf('Velocity: [%.6e, %.6e] m/s\n', earth_velocities(end, 1), earth_velocities(end, 2));
fprintf('Speed: %.6f m/s\n', earth_speed(end));

% Display specific angular momentum and areal velocity
fprintf('\nKepler Second Law Checks\n');

fprintf('\nInitial State:\n');
fprintf('Specific angular momentum: %.6e m^2/s\n', specific_angular_momentum(1));
fprintf('Areal velocity: %.6e m^2/s\n', areal_velocity(1));

fprintf('\nFinal State:\n');
fprintf('Specific angular momentum: %.6e m^2/s\n', specific_angular_momentum(end));
fprintf('Areal velocity: %.6e m^2/s\n', areal_velocity(end));

% Calculate relative changes (%)
h_difference = specific_angular_momentum(end) - specific_angular_momentum(1);

if h_difference < 0
    h_difference = -h_difference;
end

h_change = h_difference / specific_angular_momentum(1) * 100;
area_rate_difference = areal_velocity(end) - areal_velocity(1);

if area_rate_difference < 0
    area_rate_difference = -area_rate_difference;
end

area_rate_change = area_rate_difference / areal_velocity(1) * 100;
