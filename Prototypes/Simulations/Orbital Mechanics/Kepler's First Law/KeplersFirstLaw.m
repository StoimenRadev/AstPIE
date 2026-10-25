clear;
clc;
close all;

sun_mass = 1.98847e30; % Sun's mass (kg)
earth_mass = 5.9722e24; % Earth's mass (kg)
G = 6.67430e-11; % Gravitational constant (m^3 kg^-1 s^-2)

mu_sun = G * sun_mass; % Sun's gravitational parameter (m^3/s^2)

r0_earth = [1.496e11, 0];    % Initial position (m)
v0_earth = [0, 29780];       % Initial velocity (m/s)

dt = 3600;
T = 31536000;
num_steps = round(T / dt);

r_earth = r0_earth;
v_earth = v0_earth;

earth_positions = zeros(num_steps + 1, 2);
earth_positions(1, :) = r_earth;

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

end

figure;
plot(earth_positions(:, 1), earth_positions(:, 2), 'b-');
hold on;

plot(0, 0, 'yo', 'MarkerFaceColor', 'y', 'MarkerSize', 10);
plot(r0_earth(1), r0_earth(2), 'go', 'MarkerFaceColor', 'g');
plot(r_earth(1), r_earth(2), 'ro', 'MarkerFaceColor', 'r');

axis equal;
grid on;

xlabel("X Position (m)");
ylabel("Y Position (m)");
title("Earth Orbit Around the Sun");

legend("Earth Trajectory", "Sun (Focus)", "Starting Position", "Ending Position");

       %Display initial and final states
fprintf("\nINITIAL STATE \n");
fprintf("Initial position X: %.3e m\n", r0_earth(1));
fprintf("Initial position Y: %.3e m\n", r0_earth(2));
fprintf("Initial velocity X: %.3f m/s\n", v0_earth(1));
fprintf("Initial velocity Y: %.3f m/s\n", v0_earth(2));

fprintf('\nFINAL STATE \n');
fprintf('Final position X: %.3e m\n', r_earth(1));
fprintf('Final position Y: %.3e m\n', r_earth(2));
fprintf('Final velocity X: %.3f m/s\n', v_earth(1));
fprintf('Final velocity Y: %.3f m/s\n', v_earth(2));

