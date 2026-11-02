import en from './en.json';

// Only English for now. Other languages will get their own file next to en.json.
export type TextKey = keyof typeof en;

export function t(key: TextKey): string
{
	return en[key];
}
