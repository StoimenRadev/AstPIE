// The name opens out of "AstPIE" when you point at the logo (CSS does the opening;
// here each hidden part learns how wide it is).

export function setUpLogoName(): void
{
	const logo = document.querySelector<HTMLElement>('.logo');
	if (logo === null)
	{
		return;
	}
	const element = logo;

	function measure(): void
	{
		for (const part of element.querySelectorAll<HTMLElement>('.x'))
		{
			part.style.setProperty('--w', part.scrollWidth + 'px');
		}
	}

	measure();
	window.addEventListener('load', measure);
	window.addEventListener('resize', measure);
	document.fonts.ready.then(measure);
}
