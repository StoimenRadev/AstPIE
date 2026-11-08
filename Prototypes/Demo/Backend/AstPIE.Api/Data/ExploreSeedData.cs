using System.Collections.Generic;
using System.Linq;
using AstPIE.Api.Models;

namespace AstPIE.Api.Data
{
	// First content of Explore and the Catalogue, from Data/Content/explore and Data/Content/catalogue
	public static class ExploreSeedData
	{
		public static void AddEntries(AppDbContext db)
		{
			// Everyone is created once; a person can be in several cases
			Dictionary<string, Person> people = new Dictionary<string, Person>();
			foreach (SeedContent.PersonFile file in SeedContent.Read<List<SeedContent.PersonFile>>("explore", "people.json"))
			{
				Person person = new Person();
				person.FullName = file.FullName;
				person.Nationality = file.Nationality;
				person.BirthDate = file.BirthDate;
				person.DeathDate = file.DeathDate;
				person.PhotoUrl = file.PhotoUrl;
				person.PhotoCredit = file.PhotoCredit;
				person.Bio = file.Bio;
				people.Add(file.Key, person);
				db.People.Add(person);
			}

			foreach (string fileName in SeedContent.FileNames("explore"))
			{
				if (fileName == "people.json")
				{
					continue;
				}
				SeedContent.EntryFile file = SeedContent.Read<SeedContent.EntryFile>("explore", fileName);
				db.Entries.Add(NewEntry(db, file, people));
			}
		}

		private static Entry NewEntry(AppDbContext db, SeedContent.EntryFile file, Dictionary<string, Person> people)
		{
			Entry entry = new Entry();
			entry.Kind = file.Kind;
			entry.Slug = file.Slug;
			entry.Title = file.Title;
			entry.Tagline = file.Tagline;
			entry.Summary = file.Summary;
			entry.CoverImageUrl = file.CoverImageUrl;
			entry.CoverCaption = file.CoverCaption;
			entry.CoverCredit = file.CoverCredit;
			entry.Country = file.Country;
			entry.Desk = file.Desk;
			entry.StartDate = file.StartDate;
			entry.EndDate = file.EndDate;
			entry.Agency = file.Agency;
			entry.MissionOutcome = file.MissionOutcome;
			entry.EvidenceStatus = file.EvidenceStatus;
			entry.StatusNote = file.StatusNote;
			entry.RadioMinMHz = file.RadioMinMHz;
			entry.RadioMaxMHz = file.RadioMaxMHz;
			entry.IsPublished = true;

			if (file.RelatedTopic != null)
			{
				entry.RelatedTopic = (from topic in db.Topics where topic.Slug == file.RelatedTopic select topic).FirstOrDefault();
			}

			int sortOrder = 1;
			foreach (EntryFact fact in file.Facts)
			{
				fact.SortOrder = sortOrder;
				entry.Facts.Add(fact);
				sortOrder++;
			}

			foreach (SeedContent.EntryPersonFile link in file.People)
			{
				EntryPerson entryPerson = new EntryPerson();
				entryPerson.Person = people[link.Person];
				entryPerson.Role = link.Role;
				entryPerson.IsCrew = link.IsCrew;
				entryPerson.SortOrder = link.SortOrder;
				entry.People.Add(entryPerson);
			}

			// What lies on the desk with the case summary
			foreach (SeedContent.MediaFile media in file.Media)
			{
				entry.Media.Add(NewMedia(media, people));
			}
			foreach (Explanation explanation in file.Explanations)
			{
				entry.Explanations.Add(explanation);
			}

			// The pages, each with what lands on the desk with it
			foreach (SeedContent.ChapterFile page in file.Chapters)
			{
				Chapter chapter = new Chapter();
				chapter.Section = page.Section;
				chapter.TabLabel = page.TabLabel;
				chapter.Title = page.Title;
				chapter.Place = page.Place;
				chapter.DateLabel = page.DateLabel;
				chapter.Summary = page.Summary;
				chapter.Body = page.Body;
				chapter.MarginNote = page.MarginNote;
				chapter.ShowsRadioHint = page.ShowsRadioHint;
				chapter.SortOrder = page.SortOrder;

				foreach (SeedContent.MediaFile media in page.Media)
				{
					MediaItem item = NewMedia(media, people);
					item.Entry = entry;
					chapter.Media.Add(item);
				}
				foreach (Explanation explanation in page.Explanations)
				{
					explanation.Entry = entry;
					chapter.Explanations.Add(explanation);
				}
				entry.Chapters.Add(chapter);
			}

			return entry;
		}

		private static MediaItem NewMedia(SeedContent.MediaFile file, Dictionary<string, Person> people)
		{
			MediaItem item = new MediaItem();
			item.Type = file.Type;
			item.Url = file.Url;
			item.Title = file.Title;
			item.Caption = file.Caption;
			item.Credit = file.Credit;
			item.License = file.License;
			item.Text = file.Text;
			item.Author = file.Author;
			item.DateLabel = file.DateLabel;
			item.PosterUrl = file.PosterUrl;
			item.LengthLabel = file.LengthLabel;
			item.Style = file.Style;
			item.ChartKey = file.ChartKey;
			item.SortOrder = file.SortOrder;

			if (file.Person != null)
			{
				item.Person = people[file.Person];
			}
			if (file.People != null)
			{
				foreach (string key in file.People)
				{
					MediaItemPerson shown = new MediaItemPerson();
					shown.Person = people[key];
					item.ShownPeople.Add(shown);
				}
			}
			if (file.Data != null)
			{
				item.Data = file.Data.ToJsonString();
			}
			return item;
		}

		public static void AddCatalogue(AppDbContext db)
		{
			// Instruments are shared: one row each, however many objects list them
			Dictionary<string, Instrument> instruments = new Dictionary<string, Instrument>();

			foreach (string fileName in SeedContent.FileNames("catalogue"))
			{
				SeedContent.CatalogueFile file = SeedContent.Read<SeedContent.CatalogueFile>("catalogue", fileName);

				CatalogueObject item = new CatalogueObject();
				item.Slug = file.Slug;
				item.Name = file.Name;
				item.OtherNames = file.OtherNames;
				item.Type = file.Type;
				item.TypeLabel = file.TypeLabel;
				item.Constellation = file.Constellation;
				item.RightAscension = file.RightAscension;
				item.Declination = file.Declination;
				item.DistanceLy = file.DistanceLy;
				item.MassKg = file.MassKg;
				item.RadiusKm = file.RadiusKm;
				item.ApparentMagnitude = file.ApparentMagnitude;
				item.DiscoveredBy = file.DiscoveredBy;
				item.DiscoveryYear = file.DiscoveryYear;
				item.Summary = file.Summary;
				item.Description = file.Description;
				item.ImageUrl = file.ImageUrl;
				item.ImageCaption = file.ImageCaption;
				item.ImageCredit = file.ImageCredit;
				item.ImageIsDark = file.ImageIsDark;
				item.IsPublished = true;

				int sortOrder = 1;
				foreach (CatalogueFact fact in file.Facts)
				{
					fact.SortOrder = sortOrder;
					item.Facts.Add(fact);
					sortOrder++;
				}

				foreach (ObservationLogEntry log in file.LogEntries)
				{
					item.LogEntries.Add(log);
				}

				foreach (SeedContent.InstrumentFile used in file.Instruments)
				{
					if (!instruments.ContainsKey(used.Name))
					{
						Instrument instrument = new Instrument();
						instrument.Name = used.Name;
						instrument.PhotoUrl = used.PhotoUrl;
						instrument.PhotoCredit = used.PhotoCredit;
						instrument.PhotoIsDark = used.PhotoIsDark;
						instrument.Description = used.Description;
						instruments.Add(used.Name, instrument);
					}

					CatalogueObjectInstrument link = new CatalogueObjectInstrument();
					link.Instrument = instruments[used.Name];
					link.Role = used.Role;
					link.SortOrder = used.SortOrder;
					item.Instruments.Add(link);
				}

				db.CatalogueObjects.Add(item);
			}
		}
	}
}
