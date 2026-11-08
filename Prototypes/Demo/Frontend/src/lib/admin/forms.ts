import { adminRequest, uploadFile } from './client';
import type { FieldConfig, TableConfig } from './config';

// A row of another table, for dropdowns and for showing names instead of numbers
export interface Choice
{
	id: string;
	label: string;
}

const LABEL_FIELDS: Record<string, string> = {
	topics: 'title',
	entries: 'title',
	people: 'fullName',
	chapters: 'title',
	simulations: 'title',
	catalogue: 'name',
	'key-words': 'name',
	instruments: 'name',
	media: 'title'
};

// Loads the rows of every table the given fields point to: { topics: [...], entries: [...] }
export async function loadChoices(key: string, fields: FieldConfig[]): Promise<Record<string, Choice[]>>
{
	const result: Record<string, Choice[]> = {};
	for (const f of fields)
	{
		if (f.type === 'ref' && !(f.refTable in result))
		{
			const answer = await adminRequest(key, 'GET', '/' + f.refTable, null);
			const choices: Choice[] = [];
			if (answer.ok)
			{
				for (const row of answer.data)
				{
					choices.push({ id: String(row.id), label: String(row[LABEL_FIELDS[f.refTable]]) });
				}
			}
			result[f.refTable] = choices;
		}
	}
	return result;
}

// What a value looks like in the list
export function displayValue(f: FieldConfig, value: unknown, choices: Record<string, Choice[]>): string
{
	if (value === null || value === undefined || value === '')
	{
		return '—';
	}
	if (Array.isArray(value))
	{
		return value.join(', ');
	}
	if (f.type === 'checkbox')
	{
		if (value === true)
		{
			return 'Yes';
		}
		return 'No';
	}
	if (f.type === 'ref')
	{
		const choice = choices[f.refTable].find((c) => c.id === String(value));
		if (choice !== undefined)
		{
			return choice.label;
		}
	}
	const text = String(value);
	if (text.length > 60)
	{
		return text.substring(0, 57) + '...';
	}
	return text;
}

export interface FormResult
{
	body: Record<string, unknown>;
	fieldErrors: Record<string, string>;
	// What was typed, to show the form again when something is wrong
	values: Record<string, unknown>;
}

// Turns the sent form into the JSON the API expects. Files chosen in 'file' fields are uploaded first.
export async function readForm(key: string, table: TableConfig, form: FormData): Promise<FormResult>
{
	const result: FormResult = { body: {}, fieldErrors: {}, values: {} };

	for (const f of table.fields)
	{
		const raw = form.get(f.name);
		let text = '';
		if (typeof raw === 'string')
		{
			text = raw.trim();
		}

		if (f.type === 'checkbox')
		{
			result.body[f.name] = raw === 'on';
			result.values[f.name] = raw === 'on';
			continue;
		}

		result.values[f.name] = text;

		// A list: one item per line, empty lines left out
		if (f.type === 'lines')
		{
			const items = text.split('\n').map((line) => line.trim()).filter((line) => line !== '');
			result.body[f.name] = items;
			result.values[f.name] = items;
			continue;
		}

		if (f.type === 'file')
		{
			// A chosen file wins over a typed address
			const upload = form.get(f.name + '__file');
			if (upload instanceof File && upload.size > 0)
			{
				const answer = await uploadFile(key, upload);
				if (answer.ok)
				{
					text = answer.data.url;
					result.values[f.name] = text;
				}
				else
				{
					let message = answer.message;
					if ('file' in answer.fieldErrors)
					{
						message = answer.fieldErrors['file'];
					}
					result.fieldErrors[f.name] = 'Upload failed: ' + message;
				}
			}
		}

		if (text === '')
		{
			// A failed upload keeps its own message
			if (f.required && !(f.name in result.fieldErrors))
			{
				result.fieldErrors[f.name] = 'This field is required.';
			}
			// Empty numbers, dates and choices are sent as "nothing"; empty text as ""
			if (f.type === 'int' || f.type === 'number' || f.type === 'date' || f.type === 'select' || f.type === 'ref')
			{
				result.body[f.name] = null;
			}
			else
			{
				result.body[f.name] = '';
			}
			if (f.type === 'int' && !f.required)
			{
				result.body[f.name] = defaultInt(f);
			}
			continue;
		}

		if (f.type === 'int' || f.type === 'ref')
		{
			const value = Number(text);
			if (!Number.isInteger(value))
			{
				result.fieldErrors[f.name] = 'Enter a whole number.';
			}
			result.body[f.name] = value;
		}
		else if (f.type === 'number')
		{
			const value = Number(text);
			if (Number.isNaN(value))
			{
				result.fieldErrors[f.name] = 'Enter a number, for example 26670 or 8.55e36.';
			}
			result.body[f.name] = value;
		}
		else
		{
			result.body[f.name] = text;
		}
	}

	return result;
}

// Order fields may be left empty: 0. A year left empty stays empty.
function defaultInt(f: FieldConfig): number | null
{
	if (f.name === 'sortOrder')
	{
		return 0;
	}
	return null;
}

// A link back into the admin, never to another site
export function safeBackLink(back: string | null, fallback: string): string
{
	if (back !== null && back.startsWith('/admin/') && !back.startsWith('//'))
	{
		return back;
	}
	return fallback;
}
