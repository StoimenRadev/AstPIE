// A text with {names} in it, filled in: fill('{n} pages', { n: '33' }) -> '33 pages'.
// Kept apart from t.ts so code that runs in the browser can use it without the whole text file.
export function fill(text: string, values: Record<string, string>): string
{
	let result = text;
	for (const name of Object.keys(values))
	{
		result = result.split('{' + name + '}').join(values[name]);
	}
	return result;
}
