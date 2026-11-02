import type { AstroCookies } from 'astro';
import { API_URL } from 'astro:env/server';

// Calls to /api/admin. They run only on the Astro server: the admin key comes from
// the httpOnly cookie and is never sent to the browser's JavaScript.

export const ADMIN_COOKIE = 'astpie_admin';
const KEY_HEADER = 'X-Admin-Key';

// The answer of an admin call, read once
export interface AdminResult
{
	status: number;
	ok: boolean;
	data: any;
	// Messages to show: one per field (key = field name) and general ones
	fieldErrors: Record<string, string>;
	message: string;
}

// "Slug" or "$.slug" (how ASP.NET names fields) -> "slug" (how the form names them)
function fieldName(apiName: string): string
{
	let name = apiName;
	if (name.startsWith('$.'))
	{
		name = name.substring(2);
	}
	return name.charAt(0).toLowerCase() + name.substring(1);
}

async function readResult(response: Response): Promise<AdminResult>
{
	const result: AdminResult = { status: response.status, ok: response.ok, data: null, fieldErrors: {}, message: '' };
	const text = await response.text();
	if (text !== '')
	{
		result.data = JSON.parse(text);
	}

	if (response.status === 401)
	{
		result.message = 'The admin key is not accepted. Log in again.';
	}
	else if (response.status === 429)
	{
		result.message = 'Too many requests in a minute. Wait a moment and try again.';
	}
	else if (response.status === 404)
	{
		result.message = 'This item does not exist (any more).';
	}
	else if (!response.ok && result.data !== null)
	{
		// 409: one message. 400: messages per field.
		if (typeof result.data.detail === 'string')
		{
			result.message = result.data.detail;
		}
		if (result.data.errors)
		{
			for (const key of Object.keys(result.data.errors))
			{
				result.fieldErrors[fieldName(key)] = result.data.errors[key].join(' ');
			}
			if (result.message === '')
			{
				result.message = 'Some fields need fixing.';
			}
		}
	}
	return result;
}

// GET, POST, PUT or DELETE on /api/admin + path, with a JSON body for POST and PUT
export async function adminRequest(key: string, method: string, path: string, body: unknown): Promise<AdminResult>
{
	const headers: Record<string, string> = {};
	headers[KEY_HEADER] = key;
	const init: RequestInit = { method: method, headers: headers };
	if (body !== null)
	{
		headers['Content-Type'] = 'application/json';
		init.body = JSON.stringify(body);
	}
	const response = await fetch(API_URL + '/api/admin' + path, init);
	return await readResult(response);
}

// true when the API accepts the key
export async function checkKey(key: string): Promise<boolean>
{
	const result = await adminRequest(key, 'GET', '/check', null);
	return result.status === 204;
}

// Sends a file to /api/admin/uploads. On success, data.url is the address of the saved file.
export async function uploadFile(key: string, file: File): Promise<AdminResult>
{
	const form = new FormData();
	form.append('file', file, file.name);
	const headers: Record<string, string> = {};
	headers[KEY_HEADER] = key;
	const response = await fetch(API_URL + '/api/admin/uploads', { method: 'POST', headers: headers, body: form });
	return await readResult(response);
}

// The admin key from the cookie, or '' when there is none
export function readKey(cookies: AstroCookies): string
{
	const cookie = cookies.get(ADMIN_COOKIE);
	if (cookie === undefined)
	{
		return '';
	}
	return cookie.value;
}
