using System;

// 2D vector with double precision, used for the physics.
// (Godot's Vector2 uses float, so it is only used for drawing.)
public struct Vector2D
{
	public double X;
	public double Y;

	public Vector2D(double x, double y)
	{
		X = x;
		Y = y;
	}

	public double Length()
	{
		return Math.Sqrt(X * X + Y * Y);
	}

	public override string ToString()
	{
		return "(" + X + ", " + Y + ")";
	}

	public static Vector2D operator +(Vector2D a, Vector2D b)
	{
		return new Vector2D(a.X + b.X, a.Y + b.Y);
	}

	public static Vector2D operator -(Vector2D a, Vector2D b)
	{
		return new Vector2D(a.X - b.X, a.Y - b.Y);
	}

	public static Vector2D operator *(Vector2D a, double number)
	{
		return new Vector2D(a.X * number, a.Y * number);
	}

	public static Vector2D operator /(Vector2D a, double number)
	{
		return new Vector2D(a.X / number, a.Y / number);
	}
}
