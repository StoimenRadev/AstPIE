using System;
using System.Linq;
using AstPIE.Api.Models;

namespace AstPIE.Api.Data
{
	// First content of Explore and the Catalogue: the facts here, the long texts
	// (the parts of each page and the catalogue descriptions) in Data/Content.
	public static class ExploreSeedData
	{
		private const string Commons = "https://commons.wikimedia.org/wiki/Special:FilePath/";

		// Two cover pictures were uploaded through the admin page. Uploads are not in git,
		// so on another computer these two pages show an empty picture box until they are uploaded again.
		private const string Uploads = "http://localhost:5080/uploads/2026/";

		public static void AddEntries(AppDbContext db)
		{
			Topic firstLaw = FindTopic(db, "newtons-first-law");
			Topic secondLaw = FindTopic(db, "newtons-second-law");
			Topic thirdLaw = FindTopic(db, "newtons-third-law");

			// ---------- People (each one is created once and can be on several pages) ----------

			Person korolev = NewPerson("Sergei Korolev", "Soviet", new DateOnly(1907, 1, 12), new DateOnly(1966, 1, 14),
				Commons + "%D0%9D%D0%B8%D0%BD%D0%B0_%D0%B8_%D0%A1%D0%B5%D1%80%D0%B3%D0%B5%D0%B9_%D0%9A%D0%BE%D1%80%D0%BE%D0%BB%D0%B5%D0%B2%D1%8B_%28cropped%29.jpg?width=1600",
				"Sergei Korolev was the chief designer of the early Soviet space programme. He survived years in a Stalin-era labour camp, then led the team that built the R-7, the first intercontinental missile, and turned it into the rocket that launched Sputnik, the first Luna probes and Yuri Gagarin. His name was kept secret during his life; the public only knew of 'the Chief Designer'. He died after surgery in January 1966, and only then was his role revealed.");
			Person gagarin = NewPerson("Yuri Gagarin", "Soviet", new DateOnly(1934, 3, 9), new DateOnly(1968, 3, 27),
				Commons + "Yuri_Gagarin_%281961%29_-_Restoration.jpg?width=800",
				"Yuri Gagarin was a Soviet fighter pilot who became the first person in space on 12 April 1961, orbiting the Earth once in the Vostok 1 spacecraft. The son of a carpenter, he was chosen from twenty cosmonaut candidates for his calm, his skill and his friendly manner. After the flight he toured the world as a hero. He died on 27 March 1968, aged 34, when his jet trainer crashed during a training flight.");
			Person titov = NewPerson("Gherman Titov", "Soviet", new DateOnly(1935, 9, 11), new DateOnly(2000, 9, 20),
				Commons + "Gherman_Titov_1961_%28cropped%29.jpg?width=800",
				"Gherman Titov was a Soviet pilot and Yuri Gagarin's backup for Vostok 1. Four months later, in August 1961, he flew Vostok 2 for a whole day, orbiting the Earth seventeen times. He was the first person to sleep in space, and the first to suffer from space sickness. At 25 he is still the youngest person ever to fly in space.");
			Person pickering = NewPerson("William H. Pickering", "American (born in New Zealand)", new DateOnly(1910, 12, 24), new DateOnly(2004, 3, 15),
				Commons + "Bill_Pickering_1964_%28cropped%29.jpg?width=800",
				"William Pickering was a New Zealand-born engineer who led the Jet Propulsion Laboratory in California from 1954 to 1976. His laboratory built Explorer 1, the first American satellite, and went on to build the Ranger and Mariner probes that took the first close-up pictures of the Moon, Venus and Mars. Under his leadership JPL became NASA's centre for exploring the planets.");
			Person vanAllen = NewPerson("James Van Allen", "American", new DateOnly(1914, 9, 7), new DateOnly(2006, 8, 9),
				Commons + "James_van_Allen.jpg?width=800",
				"James Van Allen was an American physicist at the University of Iowa. He built the Geiger counter flown on Explorer 1, and when its readings dropped to zero high above the Earth he realised that the counter was being overwhelmed by radiation. The zones of trapped particles he discovered around the Earth are named the Van Allen belts in his honour. His instruments flew on dozens of later missions.");
			Person vonBraun = NewPerson("Wernher von Braun", "German-American", new DateOnly(1912, 3, 23), new DateOnly(1977, 6, 16),
				Commons + "Wernher%20von%20Braun%20crop.jpg?width=800",
				"Wernher von Braun was a German rocket engineer. During the Second World War he led the development of the V-2 missile, built by prisoners from concentration camps under terrible conditions, a part of his past that remains deeply controversial. After the war he was brought to the United States, where his team built the Redstone and Jupiter-C rockets that launched Explorer 1, and later the Saturn V that carried astronauts to the Moon.");
			Person armstrong = NewPerson("Neil Armstrong", "American", new DateOnly(1930, 8, 5), new DateOnly(2012, 8, 25),
				Commons + "Neil_Armstrong_pose.jpg?width=800",
				"Neil Armstrong was an American test pilot and engineer, and the commander of Apollo 11. On 20 July 1969 he flew the lunar module Eagle to a landing on the Moon by hand, avoiding a field of boulders with little fuel left, and a few hours later became the first person to walk on its surface. He had earlier flown Gemini 8, where he saved the spacecraft from a dangerous spin. After NASA he taught engineering and avoided fame.");
			Person aldrin = NewPerson("Buzz Aldrin", "American", new DateOnly(1930, 1, 20), null,
				Commons + "Buzz_Aldrin.jpg?width=800",
				"Buzz Aldrin was a US Air Force fighter pilot with a doctorate in orbital mechanics, and the lunar module pilot of Apollo 11. On 21 July 1969 he became the second person to walk on the Moon, about twenty minutes after Neil Armstrong. Earlier, on Gemini 12, he had shown that astronauts could work outside a spacecraft without exhausting themselves. He later became a strong voice for human missions to Mars.");
			Person collins = NewPerson("Michael Collins", "American", new DateOnly(1930, 10, 31), new DateOnly(2021, 4, 28),
				Commons + "Michael_Collins_%28S69-31742%2C_restoration%29.jpg?width=800",
				"Michael Collins was an American test pilot and the command module pilot of Apollo 11. While Armstrong and Aldrin walked on the Moon, he flew Columbia around it alone, cut off from all radio contact each time he passed behind it. He had earlier flown Gemini 10. After NASA he became the first director of the National Air and Space Museum in Washington and wrote one of the best books about spaceflight, Carrying the Fire.");
			Person stone = NewPerson("Edward C. Stone", "American", new DateOnly(1936, 1, 23), new DateOnly(2024, 6, 9),
				Commons + "Stone_Voyager_4c.jpg?width=800",
				"Ed Stone was an American physicist and the project scientist of the Voyager mission for fifty years, from 1972 to 2022. He led the scientists who turned Voyager's flybys of Jupiter, Saturn, Uranus and Neptune into discoveries, and who later followed Voyager 1 out of the Sun's bubble and into interstellar space. He also directed the Jet Propulsion Laboratory from 1991 to 2001.");
			Person sagan = NewPerson("Carl Sagan", "American", new DateOnly(1934, 11, 9), new DateOnly(1996, 12, 20),
				Commons + "Carl%20Sagan%20Planetary%20Society.JPG?width=800",
				"Carl Sagan was an American astronomer and one of the best-known science communicators of the twentieth century, through his television series Cosmos and his books. He led the committee that chose the sounds and pictures of the Voyager Golden Record, and he persuaded NASA to turn Voyager 1 around in 1990 to photograph the Earth as a 'pale blue dot'.");
			Person ehman = NewPerson("Jerry R. Ehman", "American", null, null,
				Commons + "Wow_signal.jpg?width=800",
				"Jerry Ehman was an American astronomer who worked as a volunteer for the Big Ear radio telescope at Ohio State University. Checking its computer printouts by hand a few days after 15 August 1977, he spotted a strong, unusual signal, circled the code 6EQUJ5 in red and wrote 'Wow!' beside it. He spent years studying the signal and remained careful about what it might have been.");
			Person achille = NewPerson("Achille Judica-Cordiglia", "Italian", null, null,
				Commons + "Judica-cordiglia-brothers.jpg?width=800",
				"Achille Judica-Cordiglia was an Italian radio amateur who, with his brother Gian Battista, ran a listening station called Torre Bert near Turin from the late 1950s. They recorded signals from early satellites, and became famous for recordings they said came from secret Soviet flights that ended in disaster. Later research found that the recordings do not match any real flight.");
			Person gianBattista = NewPerson("Gian Battista Judica-Cordiglia", "Italian", null, null,
				Commons + "Judica-cordiglia-brothers.jpg?width=800",
				"Gian Battista Judica-Cordiglia was an Italian radio amateur who, with his brother Achille, built a listening station in an old bunker near Turin, which they called Torre Bert. Their recordings of satellite signals were played on radio and television across Europe. Their claims to have heard cosmonauts dying in space are now considered unfounded.");
			Person bondarenko = NewPerson("Valentin Bondarenko", "Soviet", new DateOnly(1937, 2, 16), new DateOnly(1961, 3, 23),
				Commons + "Valentin_Bondarenko.jpg?width=800",
				"Valentin Bondarenko was a Soviet fighter pilot and, at 24, the youngest member of the first group of cosmonauts. On 23 March 1961, near the end of a test in a sealed chamber full of oxygen-rich air, a cotton pad he threw away landed on a hot plate and started a fire. He died in hospital the same day. His death was kept secret until 1986.");

			// ---------- Missions ----------

			Entry sputnik = NewMission("sputnik-1", "Sputnik 1",
				"A polished metal ball, 58 centimetres across, with four long antennas. On 4 October 1957 it became the first object people ever put into orbit, and for three weeks the whole planet listened to it beep.",
				Commons + "%D0%9F%D0%B5%D1%80%D0%B2%D1%8B%D0%B9_%D0%B2_%D0%BC%D0%B8%D1%80%D0%B5_%D0%B8%D1%81%D0%BA%D1%83%D1%81%D1%81%D1%82%D0%B2%D0%B5%D0%BD%D0%BD%D1%8B%D0%B9_%D1%81%D0%BF%D1%83%D1%82%D0%BD%D0%B8%D0%BA_%D0%97%D0%B5%D0%BC%D0%BB%D0%B8.jpg?width=1600",
				new DateOnly(1957, 10, 4), new DateOnly(1958, 1, 4), "USSR", MissionOutcome.Success);
			sputnik.RelatedTopic = firstLaw;
			AddChapters(sputnik);
			AddPerson(sputnik, korolev, "Chief designer", false, 1);
			db.Entries.Add(sputnik);

			Entry explorer = NewMission("explorer-1", "Explorer 1",
				"America's answer to Sputnik. A thin, pencil-shaped satellite that carried one small instrument, and with it made the first great discovery of the space age: belts of radiation wrapped around the Earth.",
				Commons + "Explorer1.jpg?width=1600",
				new DateOnly(1958, 2, 1), new DateOnly(1970, 3, 31), "U.S. Army (ABMA) and JPL", MissionOutcome.Success);
			explorer.RelatedTopic = firstLaw;
			AddChapters(explorer);
			AddPerson(explorer, pickering, "Director of the Jet Propulsion Laboratory", false, 1);
			AddPerson(explorer, vanAllen, "Built the radiation instrument", false, 2);
			AddPerson(explorer, vonBraun, "Led the rocket team", false, 3);
			db.Entries.Add(explorer);

			Entry luna = NewMission("luna-1", "Luna 1",
				"It was supposed to hit the Moon. It missed by about 6 000 kilometres, flew on, and became the first object made by people to escape the Earth's gravity and circle the Sun.",
				Uploads + "luna1-af71e5ae.png",
				new DateOnly(1959, 1, 2), new DateOnly(1959, 1, 5), "USSR", MissionOutcome.PartialSuccess);
			luna.RelatedTopic = secondLaw;
			AddChapters(luna);
			AddPerson(luna, korolev, "Chief designer", false, 1);
			db.Entries.Add(luna);

			Entry vostok = NewMission("vostok-1", "Vostok 1",
				"On 12 April 1961 Yuri Gagarin, a 27-year-old pilot from a small Russian village, became the first person to leave the Earth. One trip around the planet, 108 minutes, and the world changed.",
				Commons + "Vostok_spacecraft.jpg?width=1600",
				new DateOnly(1961, 4, 12), new DateOnly(1961, 4, 12), "USSR", MissionOutcome.Success);
			vostok.RelatedTopic = thirdLaw;
			AddChapters(vostok);
			AddPerson(vostok, gagarin, "Pilot", true, 1);
			AddPerson(vostok, korolev, "Chief designer", false, 2);
			AddPerson(vostok, titov, "Backup pilot", false, 3);
			db.Entries.Add(vostok);

			Entry apollo = NewMission("apollo-11", "Apollo 11",
				"Four days to the Moon, two and a half hours walking on it, and a landing that came within seconds of running out of fuel. In July 1969 two people stood on another world for the first time.",
				Commons + "A_Man_on_the_Moon%2C_AS11-40-5903_%28cropped%29.jpg?width=1600",
				new DateOnly(1969, 7, 16), new DateOnly(1969, 7, 24), "NASA", MissionOutcome.Success);
			apollo.RelatedTopic = thirdLaw;
			AddChapters(apollo);
			AddPerson(apollo, armstrong, "Commander", true, 1);
			AddPerson(apollo, aldrin, "Lunar Module Pilot", true, 2);
			AddPerson(apollo, collins, "Command Module Pilot", true, 3);
			db.Entries.Add(apollo);

			Entry voyager = NewMission("voyager-1", "Voyager 1",
				"Launched in 1977 to visit Jupiter and Saturn, it found volcanoes on another world, took a photograph of the Earth as a tiny dot, and is now travelling between the stars, still sending data home.",
				Commons + "Voyager_spacecraft_model.png?width=1600",
				new DateOnly(1977, 9, 5), null, "NASA (JPL)", MissionOutcome.Ongoing);
			voyager.RelatedTopic = firstLaw;
			AddChapters(voyager);
			AddPerson(voyager, stone, "Project scientist", false, 1);
			AddPerson(voyager, sagan, "Golden Record and Pale Blue Dot", false, 2);
			db.Entries.Add(voyager);

			// ---------- Mysteries ----------

			Entry wow = NewMystery("wow-signal", "Wow! signal",
				"On a summer night in 1977 a radio telescope in Ohio heard something from deep space that looked exactly like a message. It lasted 72 seconds. In almost fifty years of searching, it has never come back.",
				Commons + "Wow_signal.jpg?width=1600",
				new DateOnly(1977, 8, 15), EvidenceStatus.Unexplained);
			AddChapters(wow);
			AddPerson(wow, ehman, "Spotted the signal in the printout", false, 1);
			db.Entries.Add(wow);

			Entry lost = NewMystery("lost-cosmonauts", "The lost cosmonauts",
				"For decades people whispered that the Soviet Union had hidden cosmonauts who died in space before and after Gagarin. Two brothers in Italy even said they had recorded their last words. What really happened is a story about secrecy.",
				Uploads + "lost-cosmonauts-dd2b5068.png",
				null, EvidenceStatus.Debunked);
			AddChapters(lost);
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
			sgrA.Summary = "The black hole at the centre of our galaxy: four million times as heavy as the Sun, squeezed into a space smaller than Mercury's orbit, 26 670 light years away. In 2022 we saw its shadow for the first time.";
			sgrA.Description = SeedContent.Read("catalogue", "sagittarius-a-star.md");
			sgrA.ImageUrl = Commons + "EHT_Sagittarius_A_black_hole.tif?width=1600";
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
			k2.Summary = "A planet 2.6 times wider than Earth, circling a small red star 124 light years away. It might be covered by a deep, warm ocean under a sky of hydrogen. Or it might not, and finding out has become one of the great detective stories of modern astronomy.";
			k2.Description = SeedContent.Read("catalogue", "k2-18b.md");
			k2.ImageUrl = Commons + "Exoplanet_K2-18_b_%28Illustration%29.jpg?width=1600";
			k2.IsPublished = true;
			db.CatalogueObjects.Add(k2);
		}

		private static Topic FindTopic(AppDbContext db, string slug)
		{
			return (from topic in db.Topics where topic.Slug == slug select topic).FirstOrDefault();
		}

		private static Entry NewMission(string slug, string title, string summary, string coverImageUrl, DateOnly? start, DateOnly? end, string agency, MissionOutcome outcome)
		{
			Entry entry = new Entry();
			entry.Kind = EntryKind.Mission;
			entry.Slug = slug;
			entry.Title = title;
			entry.Summary = summary;
			entry.CoverImageUrl = coverImageUrl;
			entry.StartDate = start;
			entry.EndDate = end;
			entry.Agency = agency;
			entry.MissionOutcome = outcome;
			entry.IsPublished = true;
			return entry;
		}

		private static Entry NewMystery(string slug, string title, string summary, string coverImageUrl, DateOnly? start, EvidenceStatus status)
		{
			Entry entry = new Entry();
			entry.Kind = EntryKind.Mystery;
			entry.Slug = slug;
			entry.Title = title;
			entry.Summary = summary;
			entry.CoverImageUrl = coverImageUrl;
			entry.StartDate = start;
			entry.EvidenceStatus = status;
			entry.IsPublished = true;
			return entry;
		}

		private static Person NewPerson(string fullName, string nationality, DateOnly? birth, DateOnly? death, string photoUrl, string bio)
		{
			Person person = new Person();
			person.FullName = fullName;
			person.Nationality = nationality;
			person.BirthDate = birth;
			person.DeathDate = death;
			person.PhotoUrl = photoUrl;
			person.Bio = bio;
			return person;
		}

		// The parts of the page, in the order of the file Data/Content/entries/<slug>.html
		private static void AddChapters(Entry entry)
		{
			int sortOrder = 1;
			foreach (SeedContent.ChapterText text in SeedContent.ReadChapters(entry.Slug))
			{
				Chapter chapter = new Chapter();
				chapter.Title = text.Title;
				chapter.DateLabel = text.DateLabel;
				chapter.Summary = text.Summary;
				chapter.Body = text.Body;
				chapter.SortOrder = sortOrder;
				entry.Chapters.Add(chapter);
				sortOrder++;
			}
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
