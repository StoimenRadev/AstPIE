// A case desk (missions and mysteries) or the observatory (catalogue objects), in the browser.
// The server draws everything (lib/deskHtml.ts): the page the desk opens on, and as text in
// <script type="text/html"> every other page, everything that can be picked up and the photo folder.
// This script turns those into elements when they are opened, lets things on the desk be dragged
// and picked up, and keeps them clear of the radio.
// The size of the stage is kept by the small script in components/explore/DeskPage.astro.
import { setUpRadio, stopTape, tapeIsPlaying } from './radio';

interface DeskState
{
	// A case: the page shown (0 is the summary) and how many there are, counting the summary
	page: number;
	pages: number;
	// The observatory: the log entry open (-1: the whole log) and how many there are
	entry: number;
	entries: number;
	// The tab of the photo folder that was open, for the "back to the photos" buttons
	folder: number;
	// How much wider than 1600 units the desk was when things were last placed
	dx: number;
}

const state: DeskState = { page: 0, pages: 1, entry: -1, entries: 0, folder: 0, dx: 0 };
let topZ = 50;

function one(selector: string): HTMLElement
{
	return document.querySelector(selector) as HTMLElement;
}

function all(selector: string, root: ParentNode): HTMLElement[]
{
	return Array.from(root.querySelectorAll<HTMLElement>(selector));
}

// The scale of the desk on screen, and how much wider than 1600 units it is
function deskScale(): number
{
	return Number(one('#stage').dataset.scale);
}

function deskExtra(): number
{
	return Number(one('#stage').dataset.dx);
}

// Something the server left as text (lib/deskHtml.ts template()), made into elements, and the nodes of one of its parts
function copyOf(id: string): DocumentFragment
{
	const text = String((document.getElementById(id) as HTMLElement).textContent);
	const holder = document.createElement('template');
	holder.innerHTML = text.split('<\\/script').join('</script').split('<\\!--').join('<!--');
	return holder.content;
}

function partOf(content: DocumentFragment, selector: string): Node[]
{
	return Array.from((content.querySelector(selector) as HTMLElement).childNodes);
}

// ---------------------------------------------------------------- a case: its pages

function showPage(target: number, instant: boolean): void
{
	const i = Math.max(0, Math.min(state.pages - 1, target));
	state.page = i;
	const holder = document.getElementById('page-' + i) as HTMLElement;
	const content = copyOf('page-' + i);
	one('#tabs').replaceChildren(...partOf(content, '.t-tabs'));
	const sheet = partOf(content, '.t-sheet');
	const inner = one('#sheet-inner');
	const fillSheet = () =>
	{
		inner.replaceChildren(...sheet);
		inner.scrollTop = 0;
		inner.classList.remove('out');
	};
	if (instant)
	{
		fillSheet();
	}
	else
	{
		inner.classList.add('out');
		window.setTimeout(fillSheet, 200);
	}
	putItems(partOf(content, '.t-items'), instant);
	const hint = holder.dataset.hint === '1';
	one('#radio').classList.toggle('hint', hint);
	for (const tape of all('.tapebox', document))
	{
		tape.classList.toggle('glow', hint);
	}
	history.replaceState(null, '', '#' + i);
}

// ---------------------------------------------------------------- things on the desk

// The things of the new page come in; the old ones slide off (or go at once)
function putItems(nodes: Node[], instant: boolean): void
{
	const box = one('#items');
	for (const old of all('.item', box))
	{
		if (instant)
		{
			old.remove();
		}
		else
		{
			old.classList.add('leave');
			window.setTimeout(() =>
			{
				old.remove();
				drawStrings();
			}, 520);
		}
	}
	let k = 0;
	for (const node of nodes)
	{
		if (node instanceof HTMLElement)
		{
			if (!instant)
			{
				node.classList.add('arrive');
			}
			box.appendChild(node);
			setUpItem(node, instant, k);
			k++;
		}
	}
	window.setTimeout(drawStrings, 900);
}

// Where a thing lies: data-x is its place on a 1600 desk, data-shift how much it moves when the desk is wider
function place(element: HTMLElement): void
{
	const left = Number(element.dataset.x) + Number(element.dataset.shift) * deskExtra();
	element.style.left = Math.round(left) + 'px';
}

function setUpItem(element: HTMLElement, instant: boolean, k: number): void
{
	place(element);
	topZ++;
	element.style.zIndex = String(topZ);
	const img = element.querySelector<HTMLImageElement>('img:not(.shot)');
	if (img !== null)
	{
		watchImage(img, element);
	}
	window.setTimeout(() =>
	{
		keepClear(element);
		drawStrings();
	}, 60);
	makeDraggable(element);
	if (!instant)
	{
		window.setTimeout(() => element.classList.remove('arrive'), 120 + k * 110);
	}
}

// Tall pictures lie narrower; a picture that cannot load says so
function watchImage(img: HTMLImageElement, element: HTMLElement): void
{
	const loaded = () =>
	{
		if (img.naturalHeight > img.naturalWidth * 1.1)
		{
			element.style.width = Math.round(Number(element.dataset.w) * 0.62) + 'px';
		}
		keepClear(element);
		drawStrings();
	};
	const failed = () =>
	{
		const missing = document.createElement('div');
		missing.className = 'missing';
		missing.textContent = String(one('#items').dataset.missing);
		img.replaceWith(missing);
	};
	// A picture from the cache may be there already
	if (img.complete)
	{
		if (img.naturalWidth > 0)
		{
			loaded();
		}
		else
		{
			failed();
		}
		return;
	}
	img.addEventListener('load', loaded);
	img.addEventListener('error', failed);
}

// Keeps evidence clear of the radio corner, and on the mystery desk inside the cork board
function keepClear(element: HTMLElement): void
{
	const dx = deskExtra();
	const left = parseFloat(element.style.left);
	const top = parseFloat(element.style.top);
	const h = element.offsetHeight;
	const w = element.offsetWidth;
	// How low an item may reach: above the radio's note, the radio, the tapes, or the photo folder
	const right = left + w;
	let limit = 715;
	if (right > 1105 + dx)
	{
		limit = 700;
	}
	if (right > 1225 + dx)
	{
		limit = 590;
	}
	if (right > 1331 + dx)
	{
		limit = 545;
	}
	if (element.classList.contains('pinned'))
	{
		limit = 572;
		if (left + w > 1580 + dx)
		{
			element.style.left = (1580 + dx - w) + 'px';
		}
	}
	if (top + h > limit)
	{
		element.style.top = Math.max(30, limit - h) + 'px';
	}
}

// Red string between the pins on the cork board
function drawStrings(): void
{
	const svg = one('#strings');
	if (!document.body.classList.contains('desk-mystery'))
	{
		return;
	}
	const pins = all('#items .item.pinned:not(.leave)', document).map((element) => [parseFloat(element.style.left) + element.offsetWidth / 2, parseFloat(element.style.top) + 4]);
	let s = '';
	const string = (a: number[], b: number[], sag: number) => '<path d="M' + a[0].toFixed(1) + ' ' + a[1].toFixed(1) + ' Q' + ((a[0] + b[0]) / 2).toFixed(1) + ' ' + ((a[1] + b[1]) / 2 + sag).toFixed(1) + ' ' + b[0].toFixed(1) + ' ' + b[1].toFixed(1) + '"/>';
	for (let k = 1; k < pins.length; k++)
	{
		s += string(pins[k - 1], pins[k], 26);
	}
	if (pins.length > 2)
	{
		s += string(pins[pins.length - 1], pins[0], 30);
	}
	svg.innerHTML = s;
}

// Anything can be dragged; a click without dragging picks it up
function makeDraggable(element: HTMLElement): void
{
	element.addEventListener('pointerdown', (e) =>
	{
		if ((e.target as HTMLElement).closest('button, iframe') !== null)
		{
			return;
		}
		const scale = deskScale();
		const sx = e.clientX;
		const sy = e.clientY;
		const ox = parseFloat(element.style.left);
		const oy = parseFloat(element.style.top);
		let moved = false;
		element.setPointerCapture(e.pointerId);
		topZ++;
		element.style.zIndex = String(topZ);
		const move = (ev: PointerEvent) =>
		{
			const dx = (ev.clientX - sx) / scale;
			const dy = (ev.clientY - sy) / scale;
			if (!moved && Math.hypot(dx, dy) > 5)
			{
				moved = true;
				element.classList.add('dragging');
			}
			if (moved)
			{
				element.style.left = (ox + dx) + 'px';
				element.style.top = (oy + dy) + 'px';
				drawStrings();
			}
		};
		const up = () =>
		{
			element.removeEventListener('pointermove', move);
			element.removeEventListener('pointerup', up);
			element.classList.remove('dragging');
			if (!moved)
			{
				pickUp(element, false);
			}
		};
		element.addEventListener('pointermove', move);
		element.addEventListener('pointerup', up);
	});
}

// When the window changes, things keep their place on their part of the desk, dragged or not
function onResize(): void
{
	const dx = deskExtra();
	for (const element of all('#items .item', document))
	{
		const left = parseFloat(element.style.left) + (dx - state.dx) * Number(element.dataset.shift);
		element.style.left = Math.round(left) + 'px';
		keepClear(element);
	}
	state.dx = dx;
	drawStrings();
}

// "Try it yourself": the monitor starts the simulation of the case's Learn topic
function startMonitor(element: HTMLElement, button: HTMLElement): void
{
	const screen = element.querySelector('.screen') as HTMLElement;
	element.classList.add('busy');
	const frame = document.createElement('iframe');
	frame.src = String(button.dataset.src);
	frame.title = String(button.dataset.title);
	frame.allow = 'fullscreen';
	frame.addEventListener('load', () =>
	{
		element.classList.remove('busy');
		element.classList.add('running');
		frame.focus();
	});
	screen.appendChild(frame);
}

// ---------------------------------------------------------------- picking things up

// Opens a thing over the desk: a person's file (data-person) or a picked-up view (data-ov).
// From the photo folder they keep their "back to the photos" button.
function pickUp(element: HTMLElement, fromFolder: boolean): void
{
	if (element.dataset.person !== undefined)
	{
		const box = openOverlay('person-' + element.dataset.person);
		const back = box.querySelector('[data-back]');
		if (!fromFolder && back !== null)
		{
			back.remove();
		}
		return;
	}
	if (element.dataset.ov !== undefined)
	{
		openOverlay('ov-' + element.dataset.ov);
	}
}

function openOverlay(id: string): HTMLElement
{
	const box = one('#overlay');
	box.replaceChildren(copyOf(id));
	box.classList.add('on');
	// A film and the radio tape would play over each other
	if (box.querySelector('video') !== null && tapeIsPlaying())
	{
		stopTape();
	}
	for (const img of box.querySelectorAll<HTMLImageElement>('.pick .big img'))
	{
		fitPicture(img);
	}
	return box;
}

// A picked-up photo fills the space beside its description, so small originals look as big as the others
function fitPicture(img: HTMLImageElement): void
{
	const fit = () =>
	{
		const frame = 24;
		const space = (img.parentElement as HTMLElement).clientWidth - frame;
		const height = window.innerHeight * 0.84 - frame;
		const scale = Math.min(space / img.naturalWidth, height / img.naturalHeight);
		img.style.width = Math.round(img.naturalWidth * scale) + 'px';
		img.style.height = Math.round(img.naturalHeight * scale) + 'px';
	};
	if (img.complete && img.naturalWidth > 0)
	{
		fit();
	}
	else
	{
		img.addEventListener('load', fit);
	}
}

function closeOverlay(): void
{
	const box = one('#overlay');
	for (const video of box.querySelectorAll('video'))
	{
		video.pause();
	}
	box.classList.remove('on');
	box.replaceChildren();
}

function openFolder(tab: number): void
{
	state.folder = tab;
	openOverlay('folder-' + tab);
}

// Buttons inside whatever is open over the desk
function onOverlayClick(e: MouseEvent): void
{
	const target = e.target as HTMLElement;
	if (target.id === 'overlay' || target.closest('[data-close]') !== null)
	{
		closeOverlay();
		return;
	}
	if (target.closest('[data-back]') !== null)
	{
		openFolder(state.folder);
		return;
	}
	const tab = target.closest<HTMLElement>('[data-folder]');
	if (tab !== null)
	{
		openFolder(Number(tab.dataset.folder));
		return;
	}
	const card = target.closest<HTMLElement>('.pthumb');
	if (card !== null)
	{
		pickUp(card, true);
	}
}

// ---------------------------------------------------------------- the observatory

// The right page shows the whole log or one entry; the index marks it; its plate goes on the light table
function selectEntry(target: number): void
{
	const i = Math.max(-1, Math.min(state.entries - 1, target));
	state.entry = i;
	const content = copyOf('log-' + (i + 1));
	const right = one('#cat-right');
	right.replaceChildren(...partOf(content, '.t-right'));
	right.scrollTop = 0;
	one('#lindex').replaceChildren(...partOf(content, '.t-index'));
	const plate = partOf(content, '.t-plate');
	const caption = partOf(content, '.t-cap');
	const glass = one('#plate');
	glass.classList.add('swap');
	window.setTimeout(() =>
	{
		glass.replaceChildren(...plate);
		one('#plate-cap').replaceChildren(...caption);
		glass.classList.remove('swap');
	}, 180);
	if (i < 0)
	{
		history.replaceState(null, '', location.pathname);
	}
	else
	{
		history.replaceState(null, '', '#' + i);
	}
}

function setUpObservatory(start: number): void
{
	state.entries = document.querySelectorAll('script[id^="log-"]').length - 1;
	const onEntry = (e: MouseEvent) =>
	{
		const button = (e.target as HTMLElement).closest<HTMLElement>('[data-entry]');
		if (button !== null)
		{
			selectEntry(Number(button.dataset.entry));
		}
	};
	one('#cat-left').addEventListener('click', onEntry);
	one('#cat-right').addEventListener('click', onEntry);
	// The plate on the light table is picked up by clicking it (a playing film keeps its own controls)
	one('#plate').addEventListener('click', (e) =>
	{
		const target = e.target as HTMLElement;
		const mount = target.closest<HTMLElement>('.mount');
		if (mount !== null && target.tagName !== 'VIDEO')
		{
			pickUp(mount, false);
		}
	});
	one('#instruments').addEventListener('click', (e) =>
	{
		const card = (e.target as HTMLElement).closest<HTMLElement>('.icard');
		if (card !== null)
		{
			pickUp(card, false);
		}
	});
	one('#platebox').addEventListener('click', () => openFolder(0));
	if (start >= 0)
	{
		selectEntry(start);
	}
}

// ---------------------------------------------------------------- start

function setUpCase(start: number): void
{
	state.pages = document.querySelectorAll('script[id^="page-"]').length;
	const onGo = (e: MouseEvent) =>
	{
		const button = (e.target as HTMLElement).closest<HTMLElement>('[data-go]');
		if (button !== null)
		{
			showPage(Number(button.dataset.go), false);
		}
	};
	one('#tabs').addEventListener('click', onGo);
	one('#sheet-inner').addEventListener('click', onGo);
	one('#items').addEventListener('click', (e) =>
	{
		const button = (e.target as HTMLElement).closest<HTMLElement>('.startbtn');
		if (button !== null)
		{
			startMonitor(button.closest('.item') as HTMLElement, button);
		}
	});
	one('#photobox').addEventListener('click', () => openFolder(0));
	setUpRadio(deskScale);
	all('#items .item', document).forEach((element, k) => setUpItem(element, true, k));
	window.setTimeout(drawStrings, 900);
	if (start > 0)
	{
		showPage(start, true);
	}
}

function onKey(e: KeyboardEvent): void
{
	if (e.key === 'Escape')
	{
		closeOverlay();
		return;
	}
	if (one('#overlay').classList.contains('on'))
	{
		return;
	}
	const forward = e.key === 'ArrowRight' || e.key === 'PageDown';
	const back = e.key === 'ArrowLeft' || e.key === 'PageUp';
	if (one('#stage').dataset.mode === 'cat')
	{
		if (forward)
		{
			selectEntry(state.entry + 1);
		}
		if (back)
		{
			selectEntry(state.entry - 1);
		}
		return;
	}
	if (forward)
	{
		showPage(state.page + 1, false);
	}
	if (back)
	{
		showPage(state.page - 1, false);
	}
}

export function startDesk(): void
{
	state.dx = deskExtra();
	document.addEventListener('keydown', onKey);
	one('#overlay').addEventListener('click', onOverlayClick);
	window.addEventListener('resize', onResize);

	// A number after # opens that page or log entry (it is kept there as you turn the pages)
	let start = Number(location.hash.slice(1));
	const noPage = location.hash === '' || isNaN(start);
	if (one('#stage').dataset.mode === 'cat')
	{
		if (noPage)
		{
			start = -1;
		}
		setUpObservatory(start);
		return;
	}
	if (noPage)
	{
		start = 0;
	}
	setUpCase(start);
}
