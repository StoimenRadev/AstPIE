// The archive room: a drawer slides out into a full view of its folders (its name goes after # in the
// address, so the back button closes it again), and leaving the page fades to the next part's colour.

export function setUpArchive(): void
{
	let open: HTMLElement | null = null;
	let pushed = false;

	function show(key: string, push: boolean): void
	{
		let view: HTMLElement | null = null;
		if (key !== '')
		{
			view = document.getElementById('view-' + key);
		}
		if (open !== null && open !== view)
		{
			open.classList.remove('on', 'ready');
			open.setAttribute('aria-hidden', 'true');
		}
		open = view;
		document.body.classList.toggle('drawer-out', open !== null);
		if (view !== null)
		{
			const shown = view;
			shown.setAttribute('aria-hidden', 'false');
			shown.scrollTop = 0;
			shown.classList.add('on');
			window.setTimeout(() =>
			{
				if (open === shown)
				{
					shown.classList.add('ready');
				}
			}, 380);
		}
		if (push && key !== '')
		{
			history.pushState(null, '', '#' + key);
			pushed = true;
		}
	}

	function closeDrawer(): void
	{
		if (pushed)
		{
			history.back();
			return;
		}
		show('', false);
		history.replaceState(null, '', location.pathname);
	}

	for (const button of document.querySelectorAll<HTMLElement>('button[data-drawer]'))
	{
		button.addEventListener('click', () => show(String(button.dataset.drawer), true));
	}
	for (const button of document.querySelectorAll<HTMLElement>('.back'))
	{
		button.addEventListener('click', closeDrawer);
	}
	document.addEventListener('keydown', (e) =>
	{
		if (e.key === 'Escape' && open !== null)
		{
			closeDrawer();
		}
	});
	window.addEventListener('popstate', () =>
	{
		pushed = false;
		show(location.hash.slice(1), false);
	});

	// Leaving: the page fades to dark (or to the paper of Learn) first, so the next page can fade in from it
	const veil = document.querySelector('.veil') as HTMLElement;
	for (const link of document.querySelectorAll<HTMLAnchorElement>('a[href]'))
	{
		link.addEventListener('click', (e) =>
		{
			if (e.metaKey || e.ctrlKey || e.shiftKey || e.button !== 0 || link.target !== '')
			{
				return;
			}
			e.preventDefault();
			link.classList.add('picked');
			let colour = '#140d09';
			if (link.dataset.leave !== undefined)
			{
				colour = link.dataset.leave;
			}
			veil.style.background = colour;
			document.body.classList.add('leaving');
			window.setTimeout(() =>
			{
				location.href = link.href;
			}, 460);
		});
	}
	window.addEventListener('pageshow', () => document.body.classList.remove('leaving'));

	// A link to /explore#missions opens that drawer
	const start = location.hash.slice(1);
	requestAnimationFrame(() =>
	{
		document.body.classList.add('ready');
		if (start !== '')
		{
			window.setTimeout(() => show(start, false), 250);
		}
	});
}
