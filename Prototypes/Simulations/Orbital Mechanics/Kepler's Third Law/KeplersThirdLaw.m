clear;
clc;
close all;

star_mass = input("Enter the mass of the central body (kg): ");

if ~isscalar(star_mass) || ~isnumeric(star_mass) || ~isreal(star_mass) || ~isfinite(star_mass) || star_mass <= 0
    error("Invalid input: Central body mass must be a positive, finite real number.");
end

planet_mass = input("Enter the mass of the orbiting body (kg): ");

if ~isscalar(planet_mass) || ~isnumeric(planet_mass) || ~isreal(planet_mass) || ~isfinite(planet_mass) || planet_mass <= 0
    error("Invalid input: Orbiting body mass must be a positive, finite real number.");
end

semi_major = input("Enter the semi-major axis (m): ");

if ~isscalar(semi_major) || ~isnumeric(semi_major) || ~isreal(semi_major) || ~isfinite(semi_major) || semi_major <= 0
    error("Invalid input: Semi-major axis must be a positive, finite real number.");
end


G = 6.67430e-11; % Gravitational constant (m^3 kg^-1 s^-2)

total_mass = star_mass + planet_mass; % Total system mass (kg)

P = sqrt((4 * pi^2 * semi_major^3) / (G * total_mass)); % Orbital period (s)

calculated_semi_major = ((G * total_mass * P^2) / (4 * pi^2))^(1/3);

calculated_total_mass = (4 * pi^2 * semi_major^3) / (G * P^2);

period_days = P / 86400;
period_years = period_days / 365.25;

% Display Input Parameters
fprintf("\nInput Parameters\n");
fprintf("Central body mass: %.6e kg\n", star_mass);
fprintf("Orbiting body mass: %.6e kg\n", planet_mass);
fprintf("Semi-major axis: %.6e m\n", semi_major);

% Display Calculated Results
fprintf("\nCalculated Results\n");
fprintf("Total system mass: %.6e kg\n", total_mass);
fprintf("Orbital period: %.6f s\n", P);
fprintf("Orbital period: %.6f days\n", period_days);
fprintf("Orbital period: %.6f Julian years\n", period_years);

fprintf("\nCalculated Semi-Major Axis: %.6e m\n", calculated_semi_major);
fprintf("Calculated Total System Mass: %.6e kg\n", calculated_total_mass);
