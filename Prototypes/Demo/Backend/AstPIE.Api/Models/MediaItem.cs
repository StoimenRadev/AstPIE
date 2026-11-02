using System.ComponentModel.DataAnnotations;

namespace AstPIE.Api.Models
{
	public enum MediaType
	{
		Image,
		Audio,
		Signal,
		Video
	}

	// A picture, recording or signal that belongs to an entry. Deleted together with its entry.
	public class MediaItem
	{
		public int Id { get; set; }

		public int EntryId { get; set; }
		public Entry Entry { get; set; }

		public MediaType Type { get; set; }

		[Required]
		public string Url { get; set; }

		[Required]
		public string Title { get; set; }

		public string Caption { get; set; }

		// Who made it, and under which licence it may be shown
		public string Credit { get; set; }
		public string License { get; set; }

		public int SortOrder { get; set; }
	}
}
