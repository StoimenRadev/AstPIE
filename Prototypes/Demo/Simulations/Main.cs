using System.Collections.Generic;
using Godot;

// First scene of the browser version. It loads the simulation named in the page link,
// for example: index.html?sim=newtons-first-law
// Without a name it opens Newton's First Law. In the Godot editor, F6 runs the open scene instead.
public partial class Main : Node
{
	public override void _Ready()
	{
		// Simulation name (same as the slug in the database) and its scene
		Dictionary<string, string> simulations = new Dictionary<string, string>();
		simulations["newtons-first-law"] = "res://ClassicalMechanics/NewtonsFirstLaw/NewtonsFirstLaw.tscn";
		simulations["newtons-second-law"] = "res://ClassicalMechanics/NewtonsSecondLaw/NewtonsSecondLaw.tscn";
		simulations["newtons-third-law"] = "res://ClassicalMechanics/NewtonsThirdLaw/NewtonsThirdLaw.tscn";

		string name = "";
		if (OS.HasFeature("web"))
		{
			name = JavaScriptBridge.Eval("new URLSearchParams(location.search).get('sim') || ''", true).AsString();
		}

		// No simulation named (for example when pressing Run in Godot): open Newton's First Law
		if (name == "")
		{
			name = "newtons-first-law";
		}

		if (simulations.ContainsKey(name))
		{
			PackedScene scene = GD.Load<PackedScene>(simulations[name]);
			AddChild(scene.Instantiate());
			GD.Print("Loaded simulation: " + name);
		}
		else
		{
			Label label = new Label();
			label.Text = "Unknown simulation \"" + name + "\". Available: " + string.Join(", ", simulations.Keys);
			label.Position = new Vector2(24, 24);
			AddChild(label);
			GD.Print("Unknown simulation: " + name);
		}
	}
}
