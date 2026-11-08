using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AstPIE.Api.Models
{
	// One page of a case: a small tab in the folder, under a section's big tab
	public class Chapter
	{
		public int Id { get; set; }

		public int EntryId { get; set; }

		public Entry Entry { get; set; }

		// The big side tab it sits under, for example "Story" or "Design"
		[Required]
		public string Section { get; set; }

		// The short name on its small tab, for example "The computer"
		[Required]
		public string TabLabel { get; set; }

		// The heading at the top of the page
		[Required]
		public string Title { get; set; }

		// Where and when it happened, shown together: "MIT Instrumentation Laboratory · 1961 to 1969"
		public string Place { get; set; }

		public string DateLabel { get; set; }

		// A one-line summary of the page
		[Required]
		public string Summary { get; set; }

		// The page's text (HTML paragraphs; <mark> highlights)
		[Required]
		public string Body { get; set; }

		// The handwritten note in the margin, for example "Software woven by hand."
		public string MarginNote { get; set; }

		// True when this page points the reader to the radio
		public bool ShowsRadioHint { get; set; }

		public int SortOrder { get; set; }

		// What lands on the desk with this page
		public List<MediaItem> Media { get; set; } = new List<MediaItem>();

		public List<Explanation> Explanations { get; set; } = new List<Explanation>();
	}
}
