using System.ComponentModel.DataAnnotations;

namespace AstPIE.Api.Models
{
	// "Question 1, 2, 3" on a topic page, with its answer behind "Show the answer"
	public class CheckQuestion
	{
		public int Id { get; set; }

		public int TopicId { get; set; }

		public Topic Topic { get; set; }

		// The section it follows (1 = after the first section)
		public int AfterSection { get; set; }

		[Required]
		public string Question { get; set; }

		[Required]
		public string Answer { get; set; }

		public int SortOrder { get; set; }
	}
}
