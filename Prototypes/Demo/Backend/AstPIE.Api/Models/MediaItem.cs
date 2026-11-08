using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AstPIE.Api.Models
{
	// What kind of object lands on the desk
	public enum MediaType
	{
		Image,
		Audio,
		Signal,
		Video,
		Note,
		Quote,
		Drawing,
		Clipping,
		Printout,
		Document,
		Card,
		PersonFile,
		DayGrid,
		// The monitor on "Try it yourself": it runs the simulation of the case's Learn topic
		Simulation
	}

	// Everything that lands on a case desk: photos, scans, tapes, films, notes, clippings...
	public class MediaItem
	{
		public int Id { get; set; }

		public int EntryId { get; set; }

		public Entry Entry { get; set; }

		// The page it lands on; empty means the case summary
		public int? ChapterId { get; set; }

		// Deleting the page deletes what lands on it
		[DeleteBehavior(DeleteBehavior.Cascade)]
		public Chapter Chapter { get; set; }

		public MediaType Type { get; set; }

		// The file (photo, scan, recording, film); empty for things drawn by code
		public string Url { get; set; }

		// Its title, the label of a tape, or a clipping's headline
		[Required]
		public string Title { get; set; }

		public string Caption { get; set; }

		public string Credit { get; set; }

		public string License { get; set; }

		// The text of a note, quote or clipping, or the handwritten label under a photo
		public string Text { get; set; }

		// Who said a quote
		public string Author { get; set; }

		// When a quote was said or a clipping printed
		public string DateLabel { get; set; }

		// The still frame shown before a film plays
		public string PosterUrl { get; set; }

		// How long a tape or film runs, as written: "about 3 hours"
		public string LengthLabel { get; set; }

		// A personnel file: the person it opens
		public int? PersonId { get; set; }

		[DeleteBehavior(DeleteBehavior.SetNull)]
		public Person Person { get; set; }

		// Its look: a note's colour ("yellow"), "dark", "photo" (a drawing shown as a photo),
		// or "folder" (a photo kept only in the Photographs folder)
		public string Style { get; set; }

		// For things drawn by code: which chart or drawing ("orbit", "wow-values"...)
		public string ChartKey { get; set; }

		// Extra details only some types need (JSON): an index card's rows,
		// the pencil notes on a drawing, the days of a day grid, a clipping's outcome
		[Column(TypeName = "jsonb")]
		public string Data { get; set; }

		public int SortOrder { get; set; }

		// A photo: the people of the case it shows (the photo folder lists it with them)
		public List<MediaItemPerson> ShownPeople { get; set; } = new List<MediaItemPerson>();
	}

	// Who a photo shows. One person: it is one of their own photos. Several: they are on it together.
	// A link is known by its photo and its person.
	[PrimaryKey(nameof(MediaItemId), nameof(PersonId))]
	public class MediaItemPerson
	{
		public int MediaItemId { get; set; }

		public MediaItem MediaItem { get; set; }

		public int PersonId { get; set; }

		public Person Person { get; set; }
	}
}
