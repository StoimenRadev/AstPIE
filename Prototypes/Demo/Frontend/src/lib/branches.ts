// What each branch of Learn says about itself on the Learn page.
// A branch without a description here simply shows its name.

const DESCRIPTIONS: Record<string, string> = {
	'Classical mechanics': 'How things move, and why they start, stop and turn.'
};

export function branchDescription(branch: string): string
{
	if (branch in DESCRIPTIONS)
	{
		return DESCRIPTIONS[branch];
	}
	return '';
}

// The id of a branch on the Learn page, for links straight to it: "Classical mechanics" -> "classical-mechanics"
export function branchId(branch: string): string
{
	return branch.toLowerCase().replace(/[^a-z0-9]+/g, '-').replace(/^-|-$/g, '');
}
