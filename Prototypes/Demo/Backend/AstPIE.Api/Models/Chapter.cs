using System.ComponentModel.DataAnnotations;

namespace AstPIE.Api.Models
{
	// One part of an entry's story, for example "Launch". Deleted together with its entry.
	public class Chapter
	{
		public int Id { get; set; }

		public int EntryId { get; set; }
		public Entry Entry { get; set; }

		[Required]
		public string Title { get; set; }

		// Text, because some dates are vague: "October 1957", "1970s"
		public string DateLabel { get; set; }

		// One sentence for the timeline
		[Required]
		public string Summary { get; set; }

		// Markdown
		[Required]
		public string Body { get; set; }

		public int SortOrder { get; set; }
	}
}
