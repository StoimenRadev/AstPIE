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
