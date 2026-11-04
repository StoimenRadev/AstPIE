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

// The name of the country shown on the page
export function countryName(agency: string | null): string
{
	const country = countryOf(agency);
	if (country === 'ussr')
	{
		return 'Soviet Union';
	}
	if (country === 'usa')
	{
		return 'United States';
	}
	if (agency === null)
	{
		return '';
	}
	return agency;
}
