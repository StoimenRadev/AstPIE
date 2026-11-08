import en from './en.json';
import { fill } from './fill';

// Only English for now. Other languages will get their own file next to en.json.
export type TextKey = keyof typeof en;

export { fill };

export function t(key: TextKey): string
{
	return en[key];
}

// Only the given texts, for code that runs in the browser (an island gets them as a prop),
// so the whole text file is never sent there
export function texts<K extends TextKey>(keys: readonly K[]): Record<K, string>
{
	const result = {} as Record<K, string>;
	for (const key of keys)
	{
		result[key] = en[key];
	}
	return result;
}
