using System.ComponentModel.DataAnnotations;

namespace AstPIE.Api.Models
{
	// One row of "Reading it symbol by symbol" under a topic's equation
	public class TopicSymbol
	{
		public int Id { get; set; }

		public int TopicId { get; set; }

		public Topic Topic { get; set; }

		// The symbol as shown, for example "Σ", "F" or "="
		[Required]
		public string Symbol { get; set; }

		// Its name in bold, for example "force"
		[Required]
		public string Name { get; set; }

		// What it means, in plain words
		[Required]
		public string Meaning { get; set; }

		// For example "Measured in newtons (N)."; empty when it has no unit
		public string Unit { get; set; }

		// The part of the equation (data-k in LawEquation) that lights up with this row; empty for none
		public string EquationPart { get; set; }

		public int SortOrder { get; set; }
	}
}
