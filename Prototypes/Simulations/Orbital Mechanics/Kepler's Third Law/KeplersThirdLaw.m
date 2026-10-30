clear;
clc;
close all;

star_mass = 1.98847e30; % Star's mass (kg)
planet_mass = 5.9722e24; % Planet's mass (kg)

semi_major = 1.496e11; % Semi-major axis (m)

G = 6.67430e-11; % Gravitational constant (m^3 kg^-1 s^-2)

total_mass = star_mass + planet_mass; % Calculating total mass (kg)

P = sqrt((4 * pi^2 * semi_major^3) / (G*(total_mass))); % Calculating the orbital period (s)

fprintf("Orbital period: %.3f s\n", P);
