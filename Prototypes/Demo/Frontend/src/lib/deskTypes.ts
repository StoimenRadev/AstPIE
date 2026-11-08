// The data a case desk and the observatory are drawn from. lib/desk.ts builds it from the API
// and lib/deskHtml.ts turns it into the page. Text that is not there is an empty string.

// One thing on the desk. Which fields are used depends on its type:
// photo, doc, drawing, svgdrawing, printout, note, card, theory, film, quote, clipping, days, sketch, person, monitor
export interface DeskItem
{
	type: string;
	src: string;
	title: string;
	caption: string;
	credit: string;
	// photo: the handwritten label under it
	label: string;
	// note: its colour; photo: "dark", drawing or photo: shown like a photo
	color: string;
	dark: boolean;
	photoLike: boolean;
	// note, quote and clipping: the words
	text: string;
	// quote
	who: string;
	when: string;
	// clipping
	head: string;
	year: string;
	outcome: string;
	// card: label and value rows
	rows: string[][];
	// drawing: the pencil notes
	notes: string[];
	// printout: which chart; sketch and svgdrawing: which drawing
	chart: string;
	sketch: string;
	drawing: string;
	// days: the first and last day, how many are red, and one dot per week instead of per day
	from: string;
	to: string;
	loud: number;
	weekly: boolean;
	// film
	poster: string;
	length: string;
	// person: the id of the person in the case
	person: string;
	// A photo: the ids of the people of the case it shows (one: their own photo; several: together)
	shows: string[];
	// theory (a card on the cork board)
	by: string;
	forText: string;
	against: string;
	stamp: string;
	verdict: string;
	fit: string;
	// Set while photos are listed in the folder
	group: string;
	film: DeskItem | null;
}

export interface DeskPerson
{
	id: string;
	name: string;
	role: string;
	photo: string;
	photoCredit: string;
	born: string;
	died: string;
	from: string;
	bio: string;
}

export interface DeskTape
{
	id: string;
	label: string;
	src: string;
	credit: string;
	// A sound made on the page instead of a recording ("wow": the six printed values as tones)
	synth: string;
}

export interface DeskPart
{
	tab: string;
	title: string;
	where: string;
	// HTML paragraphs
	text: string;
	margin: string;
	section: string;
	evidence: DeskItem[];
	radioHint: boolean;
	// "Try it yourself": the monitor with the simulation
	sim: boolean;
}

export interface DeskSimulation
{
	title: string;
	goal: string;
	buildUrl: string;
	thumbnailUrl: string;
	topicTitle: string;
	topicSlug: string;
}

export interface DeskCase
{
	id: string;
	// ussr, usa or mystery
	desk: string;
	title: string;
	tagline: string;
	cover: { src: string; caption: string; credit: string };
	fields: string[][];
	// Missions: success, partial, ongoing or failure
	outcome: string;
	// Mysteries: the big stamp ("UNEXPLAINED") and the note under it; empty word for missions
	status: { word: string; note: string };
	// HTML
	summary: string;
	summaryEvidence: DeskItem[];
	people: DeskPerson[];
	tapes: DeskTape[];
	parts: DeskPart[];
	extraPhotos: DeskItem[];
	radioRange: number[];
	simulation: DeskSimulation | null;
}

// What lies on the light table of the observatory
export interface Plate
{
	// image, chart or video
	kind: string;
	src: string;
	poster: string;
	chart: string;
	title: string;
	caption: string;
	credit: string;
	dark: boolean;
}

export interface LogItem
{
	date: string;
	title: string;
	text: string;
	where: string;
	// HTML paragraphs
	body: string[];
	facts: string[][];
	note: string;
	plate: Plate | null;
}

export interface DeskInstrument
{
	name: string;
	role: string;
	photo: string;
	text: string;
	dark: boolean;
}

export interface DeskObject
{
	id: string;
	name: string;
	other: string;
	type: string;
	shortType: string;
	constellation: string;
	data: string[][];
	// HTML
	summary: string;
	plate: Plate;
	log: LogItem[];
	instruments: DeskInstrument[];
}
