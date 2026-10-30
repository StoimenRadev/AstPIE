clear;
clc;
close all;

star_mass = input("Enter the mass of the star (kg): "); % Star's mass (kg)

if ~isscalar(star_mass) || ~isnumeric(star_mass) || ~isreal(star_mass) || ~isfinite(star_mass) || star_mass <= 0
    error("Invalid input: Star mass must be a positive, finite real number.");
end

planet_mass = input("Enter the mass of the planet (kg): "); % Planet's mass (kg)

if ~isscalar(planet_mass) || ~isnumeric(planet_mass) || ~isreal(planet_mass) || ~isfinite(planet_mass) || planet_mass <= 0
    error("Invalid input: Planet mass must be a positive, finite real number.");
end

semi_major = input("Enter the semi-major axis (m): "); % Semi-major axis (m)

if ~isscalar(semi_major) || ~isnumeric(semi_major) || ~isreal(semi_major) || ~isfinite(semi_major) || semi_major <= 0
    error("Invalid input: Semi-major axis must be a positive, finite real number.");
end

G = 6.67430e-11; % Gravitational constant (m^3 kg^-1 s^-2)

total_mass = star_mass + planet_mass; % Calculating total mass (kg)

P = sqrt((4 * pi^2 * semi_major^3) / (G*(total_mass))); % Calculating the orbital period (s)

period_days = P/86400;
period_years =period_days/365.25;

% Display simulation parameters
fprintf("\nSimulation Parameters\n");
fprintf("Central body mass: %.6e kg\n", star_mass);
fprintf("Orbiting body mass: %.6e kg\n", planet_mass);
fprintf("Semi-major axis: %.6e m\n", semi_major);
fprintf("Total system mass: %.6e kg\n", total_mass);

fprintf("\nFinal State\n");
fprintf("Orbital period (seconds): %.6e s\n", P);
fprintf("Orbital period (days): %.3f days\n", period_days);
fprintf("Orbital period (Julian years): %.6e Julian years\n", period_years);
