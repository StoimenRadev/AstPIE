import { API_URL } from 'astro:env/server';

// All calls to the REST API are here. The types match the DTOs in the backend.

export interface TopicListItem
{
	slug: string;
	title: string;
	branch: string;
	summary: string;
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
}

export interface Topic
{
	slug: string;
	title: string;
	branch: string;
	summary: string;
	content: string;
	simulations: Simulation[];
}

// GET /api/topics
export async function getTopics(): Promise<TopicListItem[]>
{
	const response = await fetch(API_URL + '/api/topics');
	if (!response.ok)
	{
		throw new Error('API error ' + response.status + ' for /api/topics');
	}
	return await response.json();
}

// GET /api/topics/{slug}
// Returns null when the topic does not exist
export async function getTopic(slug: string): Promise<Topic | null>
{
	const response = await fetch(API_URL + '/api/topics/' + encodeURIComponent(slug));
	if (response.status === 404)
	{
		return null;
	}
	if (!response.ok)
	{
		throw new Error('API error ' + response.status + ' for /api/topics/' + slug);
	}
	return await response.json();
}

// ---------- Explore ----------

export type EntryKind = 'Mission' | 'Mystery';

// Optional values come from the API as null
export interface EntryListItem
{
	slug: string;
	kind: EntryKind;
	title: string;
	summary: string;
	coverImageUrl: string;
	startDate: string | null;
	endDate: string | null;
	agency: string | null;
	missionOutcome: string | null;
	evidenceStatus: string | null;
}

export interface TopicLink
{
	slug: string;
	title: string;
}

export interface Chapter
{
	title: string;
	dateLabel: string | null;
	summary: string;
	body: string;
}

export interface EntryPerson
{
	personId: number;
	fullName: string;
	photoUrl: string;
	role: string;
	isCrew: boolean;
}

export interface MediaItem
{
	type: string;
	url: string;
	title: string;
	caption: string | null;
	credit: string | null;
	license: string | null;
}

export interface Entry extends EntryListItem
{
	relatedTopic: TopicLink | null;
	chapters: Chapter[];
	people: EntryPerson[];
	media: MediaItem[];
}

export interface PersonEntry
{
	slug: string;
	kind: EntryKind;
	title: string;
	role: string;
}

export interface Person
{
	id: number;
	fullName: string;
	nationality: string;
	birthDate: string | null;
	deathDate: string | null;
	photoUrl: string;
	bio: string;
	entries: PersonEntry[];
}

// GET /api/entries?kind=Mission
export async function getEntries(kind: EntryKind): Promise<EntryListItem[]>
{
	const response = await fetch(API_URL + '/api/entries?kind=' + kind);
	if (!response.ok)
	{
		throw new Error('API error ' + response.status + ' for /api/entries');
	}
	return await response.json();
}

// GET /api/entries/{slug}
// Returns null when the entry does not exist
export async function getEntry(slug: string): Promise<Entry | null>
{
	const response = await fetch(API_URL + '/api/entries/' + encodeURIComponent(slug));
	if (response.status === 404)
	{
		return null;
	}
	if (!response.ok)
	{
		throw new Error('API error ' + response.status + ' for /api/entries/' + slug);
	}
	return await response.json();
}

// GET /api/people/{id}
export async function getPerson(id: number): Promise<Person>
{
	const response = await fetch(API_URL + '/api/people/' + id);
	if (!response.ok)
	{
		throw new Error('API error ' + response.status + ' for /api/people/' + id);
	}
	return await response.json();
}

// ---------- Catalogue ----------

export interface CatalogueListItem
{
	slug: string;
	name: string;
	type: string;
	constellation: string | null;
	distanceLy: number | null;
	imageUrl: string;
	summary: string;
}

export interface CataloguePage
{
	items: CatalogueListItem[];
	page: number;
	pageSize: number;
	totalCount: number;
	totalPages: number;
}

export interface CatalogueObject
{
	slug: string;
	name: string;
	otherNames: string | null;
	type: string;
	constellation: string | null;
	distanceLy: number | null;
	massKg: number | null;
	radiusKm: number | null;
	apparentMagnitude: number | null;
	discoveredBy: string | null;
	discoveryYear: number | null;
	summary: string;
	description: string;
	imageUrl: string;
	relatedTopic: TopicLink | null;
}

// GET /api/catalogue: the first page, as many objects as the API allows (50).
// Search, filters and more pages come later, when there are more objects.
export async function getCatalogue(): Promise<CataloguePage>
{
	const response = await fetch(API_URL + '/api/catalogue?pageSize=50');
	if (!response.ok)
	{
		throw new Error('API error ' + response.status + ' for /api/catalogue');
	}
	return await response.json();
}

// GET /api/catalogue/{slug}
// Returns null when the object does not exist
export async function getCatalogueObject(slug: string): Promise<CatalogueObject | null>
{
	const response = await fetch(API_URL + '/api/catalogue/' + encodeURIComponent(slug));
	if (response.status === 404)
	{
		return null;
	}
	if (!response.ok)
	{
		throw new Error('API error ' + response.status + ' for /api/catalogue/' + slug);
	}
	return await response.json();
}
