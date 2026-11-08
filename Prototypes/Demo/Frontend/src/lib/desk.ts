// Turns a case or a catalogue object from the API into what the desk is drawn from
// (lib/deskTypes.ts). Runs on the server, while the page is made.
import type { CatalogueObject, Entry, EntryDesk, Explanation, MediaItem, Topic } from './api';
import type { DeskCase, DeskItem, DeskObject, DeskPerson, DeskTape, LogItem, Plate } from './deskTypes';
import { t } from '../i18n/t';
import type { TextKey } from '../i18n/t';

export function deskIdOf(desk: EntryDesk): string
{
	if (desk === 'Soviet')
	{
		return 'ussr';
	}
	if (desk === 'American')
	{
		return 'usa';
	}
	return 'mystery';
}

// "1907-01-12" -> "12 January 1907"; empty when there is no date
export function longDate(iso: string | null): string
{
	if (iso === null)
	{
		return '';
	}
	const format = new Intl.DateTimeFormat('en-GB', { day: 'numeric', month: 'long', year: 'numeric', timeZone: 'UTC' });
	return format.format(new Date(iso + 'T00:00:00Z'));
}

function text(value: string | null): string
{
	if (value === null)
	{
		return '';
	}
	return value;
}

export function emptyItem(type: string): DeskItem
{
	return {
		type: type, src: '', title: '', caption: '', credit: '', label: '', color: '', dark: false, photoLike: false,
		text: '', who: '', when: '', head: '', year: '', outcome: '', rows: [], notes: [], chart: '', sketch: '', drawing: '',
		from: '', to: '', loud: 0, weekly: false, poster: '', length: '', person: '', shows: [], by: '', forText: '', against: '',
		stamp: '', verdict: '', fit: '', group: '', film: null
	};
}

interface ItemData
{
	rows: string[][];
	notes: { text: string }[];
	outcome: string;
	from: string;
	to: string;
	loud: number;
	weekly: boolean;
}

function dataOf(item: MediaItem): ItemData
{
	const data: ItemData = { rows: [], notes: [], outcome: '', from: '', to: '', loud: 0, weekly: false };
	if (item.data !== null)
	{
		Object.assign(data, JSON.parse(item.data));
	}
	return data;
}

// One media item from the database as a thing on the desk
function fromMedia(item: MediaItem): DeskItem
{
	const styles = text(item.style).split(' ');
	const data = dataOf(item);
	let type = 'photo';
	if (item.type === 'Document')
	{
		type = 'doc';
	}
	if (item.type === 'Drawing')
	{
		type = 'drawing';
	}
	if (item.type === 'Note')
	{
		type = 'note';
	}
	if (item.type === 'Quote')
	{
		type = 'quote';
	}
	if (item.type === 'Clipping')
	{
		type = 'clipping';
	}
	if (item.type === 'Printout')
	{
		type = 'printout';
	}
	if (item.type === 'Card')
	{
		type = 'card';
	}
	if (item.type === 'PersonFile')
	{
		type = 'person';
	}
	if (item.type === 'DayGrid')
	{
		type = 'days';
	}
	if (item.type === 'Video')
	{
		type = 'film';
	}
	if (item.type === 'Simulation')
	{
		type = 'monitor';
	}

	const result = emptyItem(type);
	result.src = text(item.url);
	result.title = item.title;
	result.caption = text(item.caption);
	result.credit = text(item.credit);
	result.dark = styles.includes('dark');
	result.photoLike = styles.includes('photo');
	result.text = text(item.text);
	result.label = text(item.text);
	result.rows = data.rows;
	result.notes = data.notes.map((note) => note.text);

	// Drawings made by code: "sketch:..." or "svg:..."
	const key = text(item.chartKey);
	if (type === 'drawing' && key.startsWith('sketch:'))
	{
		result.type = 'sketch';
		result.sketch = key.substring(7);
	}
	if (type === 'drawing' && key.startsWith('svg:'))
	{
		result.type = 'svgdrawing';
		result.drawing = key.substring(4);
	}
	if (type === 'printout')
	{
		result.chart = key;
	}
	if (type === 'note')
	{
		result.color = text(item.style);
	}
	if (type === 'quote')
	{
		result.who = text(item.author);
		result.when = text(item.dateLabel);
	}
	if (type === 'clipping')
	{
		result.head = item.title;
		result.year = text(item.dateLabel);
		result.outcome = data.outcome;
	}
	if (type === 'days')
	{
		result.from = data.from;
		result.to = data.to;
		result.loud = data.loud;
		result.weekly = data.weekly;
	}
	if (type === 'film')
	{
		result.poster = text(item.posterUrl);
		result.length = text(item.lengthLabel);
	}
	if (type === 'person' && item.personId !== null)
	{
		result.person = String(item.personId);
	}
	result.shows = item.shownPeopleIds.map((id) => String(id));
	return result;
}

function fromExplanation(explanation: Explanation): DeskItem
{
	const result = emptyItem('theory');
	result.src = text(explanation.imageUrl);
	result.credit = text(explanation.imageCredit);
	result.fit = text(explanation.imageFit);
	result.title = explanation.title;
	result.by = text(explanation.proposedBy);
	result.forText = explanation.forText;
	result.against = explanation.againstText;
	result.stamp = explanation.stamp;
	result.verdict = explanation.verdict;
	return result;
}

// Media items and cork-board cards of one page, in their order on the desk
function evidenceOf(media: MediaItem[], explanations: Explanation[]): DeskItem[]
{
	const list: { order: number; item: DeskItem }[] = [];
	for (const item of media)
	{
		list.push({ order: item.sortOrder, item: fromMedia(item) });
	}
	for (const explanation of explanations)
	{
		list.push({ order: explanation.sortOrder, item: fromExplanation(explanation) });
	}
	list.sort((a, b) => a.order - b.order);
	return list.map((entry) => entry.item);
}

const OUTCOMES: Record<string, string> = { Success: 'success', PartialSuccess: 'partial', Ongoing: 'ongoing', Failure: 'failure' };

// The radio's range on each desk, unless a case has its own
const RADIO: Record<string, number[]> = { ussr: [15, 45], usa: [100, 116], mystery: [1400, 1440] };

export function toDeskCase(entry: Entry, topic: Topic | null): DeskCase
{
	const desk = deskIdOf(entry.desk);

	// Every picture of the case, for people without a portrait of their own
	let allMedia: MediaItem[] = entry.media;
	for (const chapter of entry.chapters)
	{
		allMedia = allMedia.concat(chapter.media);
	}

	const people: DeskPerson[] = entry.people.map((person) =>
	{
		let photo = text(person.photoUrl);
		if (photo === '')
		{
			// A photo of them alone
			const theirs = allMedia.find((item) => item.type === 'Image' && item.shownPeopleIds.length === 1 && item.shownPeopleIds[0] === person.personId && text(item.url) !== '');
			if (theirs !== undefined)
			{
				photo = text(theirs.url);
			}
		}
		return {
			id: String(person.personId),
			name: person.fullName,
			role: person.role,
			photo: photo,
			photoCredit: text(person.photoCredit),
			born: longDate(person.birthDate),
			died: longDate(person.deathDate),
			from: person.nationality,
			bio: person.bio
		};
	});

	// On the summary desk: tapes go to the radio, "folder" photos only into the photo folder
	const tapes: DeskTape[] = [];
	const extraPhotos: DeskItem[] = [];
	const onDesk: MediaItem[] = [];
	for (const item of entry.media)
	{
		if (item.type === 'Audio' || item.type === 'Signal')
		{
			let synth = '';
			if (item.type === 'Signal')
			{
				synth = text(item.chartKey);
			}
			tapes.push({ id: 'tape' + tapes.length, label: item.title, src: text(item.url), credit: text(item.credit), synth: synth });
		}
		else if (item.style === 'folder')
		{
			extraPhotos.push(fromMedia(item));
		}
		else
		{
			onDesk.push(item);
		}
	}

	let outcome = 'success';
	if (entry.missionOutcome !== null && entry.missionOutcome in OUTCOMES)
	{
		outcome = OUTCOMES[entry.missionOutcome];
	}
	let word = '';
	if (entry.evidenceStatus !== null)
	{
		word = t(('desk.status.' + entry.evidenceStatus) as TextKey);
	}

	let range = RADIO[desk];
	if (entry.radioMinMHz !== null && entry.radioMaxMHz !== null)
	{
		range = [entry.radioMinMHz, entry.radioMaxMHz];
	}

	let simulation = null;
	if (topic !== null && topic.simulations.length > 0)
	{
		const sim = topic.simulations[0];
		simulation = { title: sim.title, goal: sim.learningGoal, buildUrl: sim.buildUrl, thumbnailUrl: sim.thumbnailUrl, topicTitle: topic.title, topicSlug: topic.slug };
	}

	return {
		id: entry.slug,
		desk: desk,
		title: entry.title,
		tagline: text(entry.tagline),
		cover: { src: entry.coverImageUrl, caption: text(entry.coverCaption), credit: text(entry.coverCredit) },
		fields: entry.facts.map((fact) => [fact.label, fact.value]),
		outcome: outcome,
		status: { word: word, note: text(entry.statusNote) },
		summary: entry.summary,
		summaryEvidence: evidenceOf(onDesk, entry.explanations),
		people: people,
		tapes: tapes,
		parts: entry.chapters.map((chapter) =>
		{
			const where = [text(chapter.place), text(chapter.dateLabel)].filter((part) => part !== '').join(' · ');
			const evidence = evidenceOf(chapter.media, chapter.explanations);
			return {
				tab: chapter.tabLabel,
				title: chapter.title,
				where: where,
				text: chapter.body,
				margin: text(chapter.marginNote),
				section: chapter.section,
				evidence: evidence.filter((item) => item.type !== 'monitor'),
				radioHint: chapter.showsRadioHint,
				sim: evidence.some((item) => item.type === 'monitor')
			};
		}),
		extraPhotos: extraPhotos,
		radioRange: range,
		simulation: simulation
	};
}

// ---------------------------------------------------------------- the observatory

// A smaller copy of a Wikimedia Commons picture for the index cards
export function thumbnail(src: string): string
{
	if (/\?width=\d+/.test(src))
	{
		return src.replace(/\?width=\d+/, '?width=500');
	}
	return src;
}

export function toDeskObject(item: CatalogueObject): DeskObject
{
	const PLATES: Record<string, string> = { Photo: 'image', Chart: 'chart', Video: 'video' };
	const log: LogItem[] = item.logEntries.map((entry) =>
	{
		let plate: Plate | null = null;
		if (entry.plateUrl !== null || entry.plateChartKey !== null)
		{
			plate = {
				kind: PLATES[entry.plateKind],
				src: text(entry.plateUrl),
				poster: text(entry.platePosterUrl),
				chart: text(entry.plateChartKey),
				title: text(entry.plateTitle),
				caption: text(entry.plateCaption),
				credit: text(entry.plateCredit),
				dark: entry.plateIsDark
			};
		}
		return {
			date: entry.dateLabel,
			title: entry.title,
			text: entry.summary,
			where: text(entry.place),
			body: entry.body,
			facts: entry.facts.map((fact) =>
			{
				const cut = fact.indexOf(': ');
				if (cut < 0)
				{
					return [fact, ''];
				}
				return [fact.substring(0, cut), fact.substring(cut + 2)];
			}),
			note: text(entry.note),
			plate: plate
		};
	});

	return {
		id: item.slug,
		name: item.name,
		other: text(item.otherNames),
		type: text(item.typeLabel),
		shortType: t(('objectType.' + item.type) as TextKey),
		constellation: text(item.constellation),
		data: item.facts.map((fact) => [fact.label, fact.value]),
		summary: item.description,
		plate: { kind: 'image', src: item.imageUrl, poster: '', chart: '', title: item.name, caption: text(item.imageCaption), credit: text(item.imageCredit), dark: item.imageIsDark },
		log: log,
		instruments: item.instruments.map((instrument) =>
		{
			return { name: instrument.name, role: instrument.role, photo: instrument.photoUrl, text: text(instrument.description), dark: instrument.photoIsDark };
		})
	};
}
