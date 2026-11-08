// Moving between the parts of the site: the page fades to the colour of the next one first.
// A link says which colour with data-leave="#fbf9f4"; the page needs a <div class="veil">.

export function setUpLeaving(): void
{
	const veil = document.querySelector<HTMLElement>('.veil');
	if (veil === null)
	{
		return;
	}
	const cover = veil;

	for (const link of document.querySelectorAll<HTMLAnchorElement>('a[data-leave]'))
	{
		link.addEventListener('click', (e) =>
		{
			// A new tab or window: nothing to fade
			if (e.metaKey || e.ctrlKey || e.shiftKey || e.button !== 0)
			{
				return;
			}
			e.preventDefault();
			cover.style.background = String(link.dataset.leave);
			document.body.classList.add('leaving');
			window.setTimeout(() =>
			{
				location.href = link.href;
			}, 460);
		});
	}

	// Coming back with the browser's back button shows the page again
	window.addEventListener('pageshow', () =>
	{
		document.body.classList.remove('leaving');
	});
}
