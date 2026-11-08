using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AstPIE.Api.Models
{
	// What goes on the light table next to a log entry
	public enum PlateKind
	{
		Photo,
		Chart,
		Video
	}

	// One dated entry on the right page of an object's logbook
	public class ObservationLogEntry
	{
		public int Id { get; set; }

		public int CatalogueObjectId { get; set; }

		public CatalogueObject CatalogueObject { get; set; }

		// The date as written in the log, for example "Mar. 2009"
		[Required]
		public string DateLabel { get; set; }

		// For example "Kepler is launched"
		[Required]
		public string Title { get; set; }

		// The short line in the list of entries
		[Required]
		public string Summary { get; set; }

		// Where and when, for example "Cape Canaveral, Florida · 7 March 2009"
		public string Place { get; set; }

		// The full entry, one HTML paragraph per item
		public List<string> Body { get; set; } = new List<string>();

		// The "In short" rows, each written "Label: value"
		public List<string> Facts { get; set; } = new List<string>();

		// A handwritten note beside it
		public string Note { get; set; }

		public PlateKind PlateKind { get; set; }

		// The picture or video on the light table; empty when the chart is drawn by code
		public string PlateUrl { get; set; }

		// A chart drawn by code ("transit", "s2"), and the still frame of a video
		public string PlateChartKey { get; set; }

		public string PlatePosterUrl { get; set; }

		public string PlateTitle { get; set; }

		public string PlateCaption { get; set; }

		public string PlateCredit { get; set; }

		public bool PlateIsDark { get; set; }

		public int SortOrder { get; set; }
	}
}
