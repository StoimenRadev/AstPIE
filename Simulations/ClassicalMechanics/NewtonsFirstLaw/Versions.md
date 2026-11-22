# Newton's First Law: versions

### v1.0 — Core Prototype

The first working Godot version of the Newton's First Law simulation, built on the Octave prototype.

### Implemented:
* 2D position and velocity (y stays 0: the box's weight W = m*g and the ground's push N = m*g cancel)
* Any number of forces, each with an x and a y part
* Net force calculation
* Acceleration calculation using Newton's Second Law, a = Fnet / m
* Velocity and position updates with Velocity Verlet, in fixed time steps dt
* Mass, start position, start velocity, forces, time step and total time from the panel (the boxes start empty)
* Live results: time, position, velocity, acceleration and net force
* The box moving on the grass field, with distance marks and a camera that follows it

### Purpose:
This version establishes the core physics and the structure of the Godot simulation that the later versions build on.

### Differences from the Octave version:
The values come from the panel instead of fixed numbers, Velocity Verlet replaces semi-implicit Euler, and any number of forces can be added instead of two.
