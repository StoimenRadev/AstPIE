using System.ComponentModel.DataAnnotations;

namespace AstPIE.Api.Models
{
	// One task on the lab sheet under a simulation
	public class LabTask
	{
		public int Id { get; set; }

		public int SimulationId { get; set; }

		public Simulation Simulation { get; set; }

		// For example "Moving with no force"
		[Required]
		public string Title { get; set; }

		// What to set up and watch
		[Required]
		public string Task { get; set; }

		[Required]
		public string Hint { get; set; }

		[Required]
		public string Answer { get; set; }

		// True when this task answers the Predict question ("You chose…" is added to its answer)
		public bool AnswersPredict { get; set; }

		public int SortOrder { get; set; }
	}
}
