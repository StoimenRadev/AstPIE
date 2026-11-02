using System;
using System.Linq;
using AstPIE.Api.Models;

namespace AstPIE.Api.Data
{
	// First content of Explore and the Catalogue.
	// The facts (dates, agencies, outcomes, people and roles) are real.
	// The story texts are placeholders, to be written later.
	public static class ExploreSeedData
	{
		private const string ChapterSummary = "The timeline sentence will be here.";
		private const string ChapterBody = "The text of this chapter will be here.";
		private const string Biography = "The biography will be here.";

		public static void AddEntries(AppDbContext db)
		{
			Topic firstLaw = FindTopic(db, "newtons-first-law");
			Topic thirdLaw = FindTopic(db, "newtons-third-law");

			// People who appear in several entries are created once
			Person korolev = NewPerson("Sergei Korolev", "Soviet", new DateOnly(1907, 1, 12), new DateOnly(1966, 1, 14));
			Person gagarin = NewPerson("Yuri Gagarin", "Soviet", new DateOnly(1934, 3, 9), new DateOnly(1968, 3, 27));
			Person armstrong = NewPerson("Neil Armstrong", "American", new DateOnly(1930, 8, 5), new DateOnly(2012, 8, 25));
			Person aldrin = NewPerson("Buzz Aldrin", "American", new DateOnly(1930, 1, 20), null);
			Person collins = NewPerson("Michael Collins", "American", new DateOnly(1930, 10, 31), new DateOnly(2021, 4, 28));
			Person ehman = NewPerson("Jerry R. Ehman", "American", null, null);
			Person achille = NewPerson("Achille Judica-Cordiglia", "Italian", null, null);
			Person gianBattista = NewPerson("Gian Battista Judica-Cordiglia", "Italian", null, null);
			Person bondarenko = NewPerson("Valentin Bondarenko", "Soviet", new DateOnly(1937, 2, 16), new DateOnly(1961, 3, 23));

			// ---------- Missions ----------

			Entry sputnik = NewMission("sputnik-1", "Sputnik 1",
				"The first artificial satellite, launched by the Soviet Union.",
				new DateOnly(1957, 10, 4), new DateOnly(1958, 1, 4), "USSR", MissionOutcome.Success);
			AddChapter(sputnik, "Launch", "4 October 1957", 1);
			AddChapter(sputnik, "In orbit", "October 1957 – January 1958", 2);
			AddChapter(sputnik, "Legacy", null, 3);
			AddPerson(sputnik, korolev, "Chief designer", false, 1);
			db.Entries.Add(sputnik);

			Entry explorer = NewMission("explorer-1", "Explorer 1",
				"The first American satellite. Its instrument found the Van Allen radiation belts.",
				new DateOnly(1958, 2, 1), new DateOnly(1970, 3, 31), "U.S. Army (ABMA) and JPL", MissionOutcome.Success);
			AddChapter(explorer, "Launch", "1 February 1958", 1);
			AddChapter(explorer, "Discovery", "1958", 2);
			AddChapter(explorer, "Re-entry", "31 March 1970", 3);
			db.Entries.Add(explorer);

			Entry luna = NewMission("luna-1", "Luna 1",
				"Meant to hit the Moon, it missed and became the first spacecraft to orbit the Sun.",
				new DateOnly(1959, 1, 2), new DateOnly(1959, 1, 5), "USSR", MissionOutcome.PartialSuccess);
			AddChapter(luna, "Launch", "2 January 1959", 1);
			AddChapter(luna, "Past the Moon", "4 January 1959", 2);
			AddChapter(luna, "Around the Sun", null, 3);
			AddPerson(luna, korolev, "Chief designer", false, 1);
			db.Entries.Add(luna);

			Entry vostok = NewMission("vostok-1", "Vostok 1",
				"The first human in space: Yuri Gagarin's single orbit of the Earth.",
				new DateOnly(1961, 4, 12), new DateOnly(1961, 4, 12), "USSR", MissionOutcome.Success);
			AddChapter(vostok, "Launch", "12 April 1961", 1);
			AddChapter(vostok, "One orbit", "12 April 1961", 2);
			AddChapter(vostok, "Landing", "12 April 1961", 3);
			AddPerson(vostok, gagarin, "Pilot", true, 1);
			AddPerson(vostok, korolev, "Chief designer", false, 2);
			db.Entries.Add(vostok);

			Entry apollo = NewMission("apollo-11", "Apollo 11",
				"The first crewed landing on the Moon.",
				new DateOnly(1969, 7, 16), new DateOnly(1969, 7, 24), "NASA", MissionOutcome.Success);
			apollo.RelatedTopic = thirdLaw;
			AddChapter(apollo, "Launch", "16 July 1969", 1);
			AddChapter(apollo, "Landing", "20 July 1969", 2);
			AddChapter(apollo, "Return", "24 July 1969", 3);
			AddPerson(apollo, armstrong, "Commander", true, 1);
			AddPerson(apollo, aldrin, "Lunar Module Pilot", true, 2);
			AddPerson(apollo, collins, "Command Module Pilot", true, 3);
			db.Entries.Add(apollo);

			Entry voyager = NewMission("voyager-1", "Voyager 1",
				"Launched in 1977, now the farthest human-made object from Earth, travelling through interstellar space.",
				new DateOnly(1977, 9, 5), null, "NASA (JPL)", MissionOutcome.Ongoing);
			voyager.RelatedTopic = firstLaw;
			AddChapter(voyager, "Launch", "5 September 1977", 1);
			AddChapter(voyager, "Jupiter", "March 1979", 2);
			AddChapter(voyager, "Saturn", "November 1980", 3);
			AddChapter(voyager, "Interstellar space", "August 2012", 4);
			db.Entries.Add(voyager);

			// ---------- Mysteries ----------

			Entry wow = NewMystery("wow-signal", "Wow! signal",
				"A strong, narrow radio signal recorded once by the Big Ear telescope and never detected again.",
				new DateOnly(1977, 8, 15), EvidenceStatus.Unexplained);
			AddChapter(wow, "Detection", "15 August 1977", 1);
			AddChapter(wow, "Searching for a repeat", null, 2);
			AddChapter(wow, "Possible explanations", null, 3);
			AddPerson(wow, ehman, "Spotted the signal in the printout", false, 1);
			db.Entries.Add(wow);

			Entry lost = NewMystery("lost-cosmonauts", "The lost cosmonauts",
				"Rumours that the Soviet Union hid cosmonauts who died on secret flights before and after Gagarin.",
				null, EvidenceStatus.Debunked);
			AddChapter(lost, "The rumours", "Late 1950s – 1960s", 1);
			AddChapter(lost, "The Judica-Cordiglia recordings", "1960s", 2);
			AddChapter(lost, "A real hidden death", "23 March 1961", 3);
			AddChapter(lost, "What the archives showed", "After 1991", 4);
			AddPerson(lost, achille, "Claimed to have recorded signals from lost spacecraft", false, 1);
			AddPerson(lost, gianBattista, "Claimed to have recorded signals from lost spacecraft", false, 2);
			AddPerson(lost, bondarenko, "Cosmonaut trainee who died in a ground training fire, kept secret until 1986", false, 3);
			db.Entries.Add(lost);
		}

		public static void AddCatalogue(AppDbContext db)
		{
			CatalogueObject sgrA = new CatalogueObject();
			sgrA.Slug = "sagittarius-a-star";
			sgrA.Name = "Sagittarius A*";
			sgrA.OtherNames = "Sgr A*";
			sgrA.Type = CatalogueObjectType.BlackHole;
			sgrA.Constellation = "Sagittarius";
			// 8.178 kpc (GRAVITY Collaboration, 2019)
			sgrA.DistanceLy = 26670;
			// About 4.3 million times the mass of the Sun
			sgrA.MassKg = 8.55e36;
			// Event horizon (Schwarzschild radius) for that mass
			sgrA.RadiusKm = 1.27e7;
			// Not visible to the eye or in ordinary telescopes: no apparent magnitude
			sgrA.ApparentMagnitude = null;
			sgrA.DiscoveredBy = "Bruce Balick and Robert Brown";
			sgrA.DiscoveryYear = 1974;
			sgrA.Summary = "The supermassive black hole at the centre of the Milky Way.";
			sgrA.Description = "The text about this object will be here.";
			sgrA.ImageUrl = "";
			sgrA.IsPublished = true;
			db.CatalogueObjects.Add(sgrA);

			CatalogueObject k2 = new CatalogueObject();
			k2.Slug = "k2-18b";
			k2.Name = "K2-18b";
			k2.OtherNames = "EPIC 201912552 b";
			k2.Type = CatalogueObjectType.Planet;
			k2.Constellation = "Leo";
			// About 38 parsecs
			k2.DistanceLy = 124;
			// About 8.6 times the mass of the Earth
			k2.MassKg = 5.15e25;
			// About 2.6 times the radius of the Earth
			k2.RadiusKm = 16600;
			// The planet itself can't be seen, only its star (K2-18)
			k2.ApparentMagnitude = null;
			k2.DiscoveredBy = "Kepler space telescope (K2 mission)";
			k2.DiscoveryYear = 2015;
			k2.Summary = "An exoplanet about 2.6 times the size of the Earth, orbiting in the habitable zone of a red dwarf star.";
			k2.Description = "The text about this object will be here.";
			k2.ImageUrl = "";
			k2.IsPublished = true;
			db.CatalogueObjects.Add(k2);
		}

		private static Topic FindTopic(AppDbContext db, string slug)
		{
			return (from topic in db.Topics where topic.Slug == slug select topic).FirstOrDefault();
		}

		private static Entry NewMission(string slug, string title, string summary, DateOnly? start, DateOnly? end, string agency, MissionOutcome outcome)
		{
			Entry entry = new Entry();
			entry.Kind = EntryKind.Mission;
			entry.Slug = slug;
			entry.Title = title;
			entry.Summary = summary;
			entry.CoverImageUrl = "";
			entry.StartDate = start;
			entry.EndDate = end;
			entry.Agency = agency;
			entry.MissionOutcome = outcome;
			entry.IsPublished = true;
			return entry;
		}

		private static Entry NewMystery(string slug, string title, string summary, DateOnly? start, EvidenceStatus status)
		{
			Entry entry = new Entry();
			entry.Kind = EntryKind.Mystery;
			entry.Slug = slug;
			entry.Title = title;
			entry.Summary = summary;
			entry.CoverImageUrl = "";
			entry.StartDate = start;
			entry.EvidenceStatus = status;
			entry.IsPublished = true;
			return entry;
		}

		private static Person NewPerson(string fullName, string nationality, DateOnly? birth, DateOnly? death)
		{
			Person person = new Person();
			person.FullName = fullName;
			person.Nationality = nationality;
			person.BirthDate = birth;
			person.DeathDate = death;
			person.PhotoUrl = "";
			person.Bio = Biography;
			return person;
		}

		private static void AddChapter(Entry entry, string title, string dateLabel, int sortOrder)
		{
			Chapter chapter = new Chapter();
			chapter.Title = title;
			chapter.DateLabel = dateLabel;
			chapter.Summary = ChapterSummary;
			chapter.Body = ChapterBody;
			chapter.SortOrder = sortOrder;
			entry.Chapters.Add(chapter);
		}

		private static void AddPerson(Entry entry, Person person, string role, bool isCrew, int sortOrder)
		{
			EntryPerson link = new EntryPerson();
			link.Person = person;
			link.Role = role;
			link.IsCrew = isCrew;
			link.SortOrder = sortOrder;
			entry.People.Add(link);
		}
	}
}
