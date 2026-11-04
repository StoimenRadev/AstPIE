// What each branch of Learn says about itself on the Learn page.
// A branch without a description here simply shows its name.

const DESCRIPTIONS: Record<string, string> = {
	'Classical mechanics': 'How and why things move: forces, mass and motion. The base for everything that flies, falls or orbits.'
};

export function branchDescription(branch: string): string
{
	if (branch in DESCRIPTIONS)
	{
		return DESCRIPTIONS[branch];
	}
	return '';
}

// The first sentence of a text
export function firstSentence(text: string): string
{
	const end = text.indexOf('. ');
	if (end === -1)
	{
		return text;
	}
	return text.substring(0, end + 1);
}
