import { API_URL } from 'astro:env/server';

// All calls to the REST API are here. The types match the DTOs in the backend.
// Optional values come from the API as null.

// One GET; null when the address does not exist (404), an error for anything else that fails
async function getJson<T>(path: string): Promise<T | null>
{
	const response = await fetch(API_URL + path);
	if (response.status === 404)
	{
		return null;
	}
	if (!response.ok)
	{
		throw new Error('API error ' + response.status + ' for ' + path);
	}
	return await response.json();
}

// The same, for addresses that always exist (lists)
async function getList<T>(path: string): Promise<T>
{
	const result = await getJson<T>(path);
	if (result === null)
	{
		throw new Error('API error 404 for ' + path);
	}
	return result;
}

// ---------- Learn ----------

export interface TopicListItem
{
	slug: string;
	title: string;
	question: string;
	branch: string;
	summary: string;
	sortOrder: number;
}

export interface LabTask
{
	title: string;
	task: string;
	hint: string;
	answer: string;
	answersPredict: boolean;
}

export interface Simulation
{
	slug: string;
	title: string;
	engine: string;
	version: string;
	buildUrl: string;
	thumbnailUrl: string;
	learningGoal: string;
	sourceUrl: string;
	labIntro: string | null;
	modelText: string | null;
	limits: string[];
	stepCode: string | null;
	labTasks: LabTask[];
}

export interface TopicSymbol
{
	symbol: string;
	name: string;
	meaning: string;
	unit: string | null;
	equationPart: string | null;
}

export interface CheckQuestion
{
	afterSection: number;
	question: string;
	answer: string;
}

export interface KeyWord
{
	slug: string;
	name: string;
	otherForms: string[];
	definition: string;
	symbol: string | null;
	unit: string | null;
}

export interface ExploreLink
{
	slug: string;
	kind: EntryKind;
	title: string;
	line: string | null;
}

export interface Topic
{
	slug: string;
	title: string;
	question: string;
	branch: string;
	summary: string;
	content: string;
	predictQuestion: string;
	predictOptions: string[];
	predictAnswer: number;
	lawStatement: string;
	lawEquation: string;
	readAloud: string;
	recapPoints: string[];
	explore: ExploreLink | null;
	symbols: TopicSymbol[];
	checkQuestions: CheckQuestion[];
	keyWords: KeyWord[];
	simulations: Simulation[];
}

// GET /api/topics
export async function getTopics(): Promise<TopicListItem[]>
{
	return await getList<TopicListItem[]>('/api/topics');
}

// GET /api/topics/{slug}; null when the topic does not exist
export async function getTopic(slug: string): Promise<Topic | null>
{
	return await getJson<Topic>('/api/topics/' + encodeURIComponent(slug));
}

// ---------- Explore ----------

export type EntryKind = 'Mission' | 'Mystery';
export type EntryDesk = 'Soviet' | 'American' | 'Mystery';

export interface EntryListItem
{
	slug: string;
	kind: EntryKind;
	title: string;
	tagline: string | null;
	summary: string;
	coverImageUrl: string;
	country: string | null;
	desk: EntryDesk;
	startDate: string | null;
	endDate: string | null;
	agency: string | null;
	missionOutcome: string | null;
	evidenceStatus: string | null;
	statusNote: string | null;
	pageCount: number;
}

export interface TopicLink
{
	slug: string;
	title: string;
}

export interface Fact
{
	label: string;
	value: string;
}

export interface MediaItem
{
	type: string;
	url: string | null;
	title: string;
	caption: string | null;
	credit: string | null;
	license: string | null;
	text: string | null;
	author: string | null;
	dateLabel: string | null;
	posterUrl: string | null;
	lengthLabel: string | null;
	// A personnel file: whose it is
	personId: number | null;
	// A photo: the people it shows
	shownPeopleIds: number[];
	style: string | null;
	chartKey: string | null;
	// JSON text
	data: string | null;
	sortOrder: number;
}

export interface Explanation
{
	title: string;
	proposedBy: string | null;
	forText: string;
	againstText: string;
	verdict: string;
	stamp: string;
	imageUrl: string | null;
	imageCredit: string | null;
	imageFit: string | null;
	sortOrder: number;
}

export interface Chapter
{
	section: string;
	tabLabel: string;
	title: string;
	place: string | null;
	dateLabel: string | null;
	summary: string;
	body: string;
	marginNote: string | null;
	showsRadioHint: boolean;
	media: MediaItem[];
	explanations: Explanation[];
}

export interface EntryPerson
{
	personId: number;
	fullName: string;
	nationality: string;
	birthDate: string | null;
	deathDate: string | null;
	photoUrl: string;
	photoCredit: string | null;
	bio: string;
	role: string;
	isCrew: boolean;
}

export interface Entry extends EntryListItem
{
	coverCaption: string | null;
	coverCredit: string | null;
	radioMinMHz: number | null;
	radioMaxMHz: number | null;
	relatedTopic: TopicLink | null;
	facts: Fact[];
	chapters: Chapter[];
	people: EntryPerson[];
	media: MediaItem[];
	explanations: Explanation[];
}

// GET /api/entries?kind=Mission, or every entry without a kind
export async function getEntries(kind: EntryKind | ''): Promise<EntryListItem[]>
{
	let path = '/api/entries';
	if (kind !== '')
	{
		path = path + '?kind=' + kind;
	}
	return await getList<EntryListItem[]>(path);
}

// GET /api/entries/{slug}; null when the entry does not exist
export async function getEntry(slug: string): Promise<Entry | null>
{
	return await getJson<Entry>('/api/entries/' + encodeURIComponent(slug));
}

// ---------- Catalogue ----------

export interface CatalogueListItem
{
	slug: string;
	name: string;
	type: string;
	typeLabel: string | null;
	constellation: string | null;
	distanceLy: number | null;
	imageUrl: string;
	summary: string;
	logCount: number;
}

export interface CataloguePage
{
	items: CatalogueListItem[];
	page: number;
	pageSize: number;
	totalCount: number;
	totalPages: number;
}

export interface LogEntry
{
	dateLabel: string;
	title: string;
	summary: string;
	place: string | null;
	body: string[];
	// Each written "Label: value"
	facts: string[];
	note: string | null;
	plateKind: 'Photo' | 'Chart' | 'Video';
	plateUrl: string | null;
	plateChartKey: string | null;
	platePosterUrl: string | null;
	plateTitle: string | null;
	plateCaption: string | null;
	plateCredit: string | null;
	plateIsDark: boolean;
}

export interface Instrument
{
	name: string;
	photoUrl: string;
	photoCredit: string | null;
	photoIsDark: boolean;
	description: string | null;
	role: string;
}

export interface CatalogueObject
{
	slug: string;
	name: string;
	otherNames: string | null;
	type: string;
	typeLabel: string | null;
	constellation: string | null;
	rightAscension: number | null;
	declination: number | null;
	distanceLy: number | null;
	massKg: number | null;
	radiusKm: number | null;
	apparentMagnitude: number | null;
	discoveredBy: string | null;
	discoveryYear: number | null;
	summary: string;
	description: string;
	imageUrl: string;
	imageCaption: string | null;
	imageCredit: string | null;
	imageIsDark: boolean;
	relatedTopic: TopicLink | null;
	facts: Fact[];
	logEntries: LogEntry[];
	instruments: Instrument[];
}

// GET /api/catalogue: the first page of everything (the archive shows only a few objects)
export async function getAllCatalogue(): Promise<CataloguePage>
{
	return await getList<CataloguePage>('/api/catalogue?page=1&pageSize=50');
}

// GET /api/catalogue/{slug}; null when the object does not exist
export async function getCatalogueObject(slug: string): Promise<CatalogueObject | null>
{
	return await getJson<CatalogueObject>('/api/catalogue/' + encodeURIComponent(slug));
}
