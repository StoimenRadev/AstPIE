using System.Threading.Tasks;
using Engine = twodog.Engine;

internal static class Program
{
	private static async Task<int> Main(string[] args)
	{
		// The game's assembly holds the generated plugins initializer. It must be registered before Start()
		// (there is no GodotPlugins.dll on the web).
		Engine.RegisterWebPluginsInitializer(TwoDogWebBoot.PluginsInitializer());

		// args come from the page's GODOT_CONFIG.args plus the '--main-pack godot.pck' the loader adds
		Engine engine = new Engine("AstPIE.Simulations", args: args);
		try
		{
			engine.Start();
			// From here the browser's loop owns the engine
			engine.Run();
		}
		catch
		{
			await engine.DisposeAsync();
			throw;
		}
		return 0;
	}
}
