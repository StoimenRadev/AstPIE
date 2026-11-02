import { t } from '../i18n/t';
import type { TextKey } from '../i18n/t';

// Turns API values into text for the page. The database keeps exact values in
// kg, km and light-years; the page shows units that are easier to imagine.

const EARTH_MASS_KG = 5.972e24;
const SUN_MASS_KG = 1.989e30;
const EARTH_RADIUS_KM = 6371;

// Objects measured against the Earth; everything else against the Sun
const EARTH_SIZED_TYPES = ['Planet', 'DwarfPlanet', 'Moon'];

// A text from en.json whose key is built from a value, for example 'outcome.Success'
function label(prefix: string, value: string): string
{
	return t((prefix + '.' + value) as TextKey);
}

export function outcomeLabel(value: string): string
{
	return label('outcome', value);
}

export function statusLabel(value: string): string
{
	return label('status', value);
}

export function objectTypeLabel(value: string): string
{
	return label('objectType', value);
}

// "1957-10-04" -> "4 October 1957"
export function formatDate(isoDate: string | null): string
{
	if (isoDate === null)
	{
		return t('common.unknown');
	}
	const date = new Date(isoDate + 'T00:00:00Z');
	return new Intl.DateTimeFormat('en-GB', { day: 'numeric', month: 'long', year: 'numeric', timeZone: 'UTC' }).format(date);
}

// For cards: "1957 – 1958", "1961", "1977 – now"
export function formatYears(startDate: string | null, endDate: string | null, ongoing: boolean): string
{
	if (startDate === null)
	{
		return '';
	}
	const start = startDate.substring(0, 4);
	if (endDate === null)
	{
		if (ongoing)
		{
			return start + ' – ' + t('common.now');
		}
		return start;
	}
	const end = endDate.substring(0, 4);
	if (end === start)
	{
		return start;
	}
	return start + ' – ' + end;
}

// For entry pages: "4 October 1957 – 4 January 1958"
export function formatDateRange(startDate: string | null, endDate: string | null, ongoing: boolean): string
{
	if (startDate === null)
	{
		return t('common.unknown');
	}
	const start = formatDate(startDate);
	if (endDate === null)
	{
		if (ongoing)
		{
			return start + ' – ' + t('common.now');
		}
		return start;
	}
	if (endDate === startDate)
	{
		return start;
	}
	return start + ' – ' + formatDate(endDate);
}

// 26670 -> "26,670", 4300000 -> "4.3 million", 8.62 -> "8.6"
function formatNumber(value: number): string
{
	if (value >= 1e6)
	{
		return Number((value / 1e6).toPrecision(3)) + ' ' + t('unit.million');
	}
	if (value >= 100)
	{
		return Math.round(value).toLocaleString('en-GB');
	}
	return String(Number(value.toPrecision(2)));
}

// Measured values are never exact, so they are shown with "about"
function about(text: string): string
{
	return t('common.about') + ' ' + text;
}

export function formatDistance(lightYears: number | null): string
{
	if (lightYears === null)
	{
		return t('common.notKnown');
	}
	return about(formatNumber(lightYears) + ' ' + t('unit.lightYears'));
}

export function formatMass(kg: number | null, type: string): string
{
	if (kg === null)
	{
		return t('common.notKnown');
	}
	if (EARTH_SIZED_TYPES.includes(type))
	{
		return about(formatNumber(kg / EARTH_MASS_KG) + ' ' + t('unit.earthMasses'));
	}
	return about(formatNumber(kg / SUN_MASS_KG) + ' ' + t('unit.sunMasses'));
}

export function formatRadius(km: number | null, type: string): string
{
	if (km === null)
	{
		return t('common.notKnown');
	}
	if (EARTH_SIZED_TYPES.includes(type))
	{
		return about(formatNumber(km / EARTH_RADIUS_KM) + ' ' + t('unit.earthRadii'));
	}
	return about(formatNumber(km) + ' ' + t('unit.km'));
}

export function formatValue(value: string | number | null): string
{
	if (value === null)
	{
		return t('common.notKnown');
	}
	return String(value);
}
