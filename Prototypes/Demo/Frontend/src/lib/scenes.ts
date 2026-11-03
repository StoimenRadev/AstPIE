// Which drawing a page or card uses when there is no photo.

// One scene per branch of Learn
const BRANCH_SCENES: Record<string, string> = {
	'Classical mechanics': 'scene-cm'
};

// The three laws have their own card art
const TOPIC_SCENES: Record<string, string> = {
	'newtons-first-law': 'law-1',
	'newtons-second-law': 'law-2',
	'newtons-third-law': 'law-3'
};

export function branchScene(branch: string): string
{
	if (branch in BRANCH_SCENES)
	{
		return BRANCH_SCENES[branch];
	}
	return 'scene-learn';
}

export function topicScene(slug: string, branch: string): string
{
	if (slug in TOPIC_SCENES)
	{
		return TOPIC_SCENES[slug];
	}
	return branchScene(branch);
}

// "ussr" or "usa", from the agency of a mission; empty when it is neither
export function countryOf(agency: string | null): string
{
	if (agency === null)
	{
		return '';
	}
	const name = agency.toUpperCase();
	if (name.includes('USSR') || name.includes('SOVIET') || name.includes('OKB'))
	{
		return 'ussr';
	}
	if (name.includes('NASA') || name.includes('USA') || name.includes('U.S.'))
	{
		return 'usa';
	}
	return '';
}

// The first sentence of a text, and the rest
export function firstSentence(text: string): { first: string; rest: string }
{
	const end = text.indexOf('. ');
	if (end === -1)
	{
		return { first: text, rest: '' };
	}
	return { first: text.substring(0, end + 1), rest: text.substring(end + 2) };
}
