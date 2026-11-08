// What the Learn pages do: a branch opens to show its concepts; the prediction comes back in the
// first lab task; answers and hints open under their buttons; lab tasks can be ticked off; a symbol
// and its explanation light up together; key ideas get a bar in the left margin; and each key word
// is explained beside its line, joined to it by a thin line when you point at it.
import { fill } from '../i18n/fill';

function all(selector: string, root: ParentNode): HTMLElement[]
{
	return Array.from(root.querySelectorAll<HTMLElement>(selector));
}

function setUpBranches(): void
{
	for (const branch of all('.branch3', document))
	{
		const button = branch.querySelector('.branchbtn') as HTMLElement;
		const set = (on: boolean) =>
		{
			branch.classList.toggle('open', on);
			button.setAttribute('aria-expanded', String(on));
		};
		button.addEventListener('click', () => set(!branch.classList.contains('open')));
		// A link straight to the branch opens it
		if (location.hash === '#' + branch.id)
		{
			set(true);
		}
	}
}

// Sent by the page in data-predict, with the texts the answer is written with
interface Prediction
{
	options: string[];
	right: number;
	theAnswerIs: string;
	thatIsRight: string;
	youChose: string;
}

// Nothing is saved: every visit starts with no choice made
function setUpPrediction(): void
{
	const box = document.querySelector<HTMLElement>('.ask');
	const article = document.querySelector<HTMLElement>('article[data-predict]');
	if (box === null || article === null)
	{
		return;
	}
	const prediction: Prediction = JSON.parse(String(article.dataset.predict));
	const choices = all('.choice', box);
	choices.forEach((choice, i) =>
	{
		choice.addEventListener('click', () =>
		{
			choices.forEach((other, k) => other.classList.toggle('picked', k === i));
			box.classList.add('done');
			let verdict = fill(prediction.theAnswerIs, { answer: prediction.options[prediction.right].toLowerCase() });
			if (i === prediction.right)
			{
				verdict = prediction.thatIsRight;
			}
			for (const echo of all('.echo', document))
			{
				echo.textContent = fill(prediction.youChose, { choice: prediction.options[i].toLowerCase() }) + ' ' + verdict + ' ';
			}
		});
	});
}

function setUpAnswers(): void
{
	for (const button of all('[data-open]', document))
	{
		const label = String(button.textContent);
		const kind = String(button.dataset.open);
		button.addEventListener('click', () =>
		{
			const box = button.closest('.check, .task') as HTMLElement;
			const panel = box.querySelector('.open-ans.' + kind) as HTMLElement;
			const on = panel.classList.toggle('on');
			if (!on)
			{
				button.textContent = label;
			}
			else
			{
				// The page puts the 'Hide the answer' (or hint) text on the button
				button.textContent = String(button.dataset.hide);
			}
		});
	}
}

function setUpTicks(): void
{
	for (const tick of all('.tick', document))
	{
		tick.addEventListener('click', () =>
		{
			const on = tick.classList.toggle('done');
			const task = tick.closest('.task') as HTMLElement;
			task.classList.toggle('done', on);
			tick.setAttribute('aria-pressed', String(on));
		});
	}
}

// A symbol in the equation, its row in the list and its part of the "where" line light up together
function setUpSymbols(): void
{
	const light = (key: string, on: boolean) =>
	{
		for (const element of all('.sym[data-k="' + key + '"], .where span[data-k="' + key + '"], .symlist > div[data-k="' + key + '"]', document))
		{
			element.classList.toggle('on', on);
		}
	};
	for (const element of all('.sym[data-k], .where span[data-k], .symlist > div[data-k]', document))
	{
		const key = String(element.dataset.k);
		element.addEventListener('mouseenter', () => light(key, true));
		element.addEventListener('mouseleave', () => light(key, false));
	}
}

// ---------------------------------------------------------------- the margins of a concept page

function setUpMargins(): void
{
	const page = document.querySelector<HTMLElement>('.page3');
	if (page === null)
	{
		return;
	}
	const frame = page;
	const article = frame.querySelector('article') as HTMLElement;
	const gloss = document.querySelector<HTMLElement>('#kw');
	const wires = frame.querySelector('.wires') as SVGSVGElement;
	const narrow = () => window.matchMedia('(max-width: 1320px)').matches;

	// A box relative to the page frame
	function placeOf(element: Element): { top: number; left: number; right: number; bottom: number; height: number }
	{
		const p = frame.getBoundingClientRect();
		const r = element.getBoundingClientRect();
		return { top: r.top - p.top, left: r.left - p.left, right: r.right - p.left, bottom: r.bottom - p.top, height: r.height };
	}

	// Key ideas: a bar in the left margin beside their lines
	function bars(): void
	{
		for (const old of all('.keybar', frame))
		{
			old.remove();
		}
		const a = placeOf(article);
		let offset = 22;
		if (narrow())
		{
			offset = 14;
		}
		for (const mark of all('mark', article))
		{
			const rects = mark.getClientRects();
			if (rects.length === 0)
			{
				continue;
			}
			const p = frame.getBoundingClientRect();
			const top = rects[0].top - p.top + 3;
			const bottom = rects[rects.length - 1].bottom - p.top - 3;
			const bar = document.createElement('span');
			bar.className = 'keybar';
			bar.style.left = (a.left - offset) + 'px';
			bar.style.top = top + 'px';
			bar.style.height = (bottom - top) + 'px';
			frame.appendChild(bar);
		}
	}

	// Key words: each note sits beside its word, pushed down when it would overlap the one above
	function place(): void
	{
		if (gloss === null)
		{
			return;
		}
		const notes = all('.kwe', gloss);
		if (narrow())
		{
			for (const note of notes)
			{
				note.style.top = '';
			}
			return;
		}
		const placed = notes.map((note) =>
		{
			const word = article.querySelector('a.term[data-term="' + note.dataset.term + '"]');
			let top = 0;
			if (word !== null)
			{
				top = placeOf(word).top - 2;
			}
			return { note: note, top: top };
		});
		placed.sort((x, y) => x.top - y.top);
		let floor = 0;
		for (const item of placed)
		{
			const top = Math.max(item.top, floor);
			item.note.style.top = top + 'px';
			floor = top + item.note.offsetHeight + 16;
		}
	}

	// A thin line from the edge of the text, beside the word, to its note
	function wire(word: Element | null, note: Element | null): void
	{
		wires.innerHTML = '';
		if (word === null || note === null || narrow())
		{
			return;
		}
		const w = placeOf(word);
		const n = placeOf(note);
		const a = placeOf(article);
		const y1 = w.top + w.height * .55;
		const x1 = a.right + 6;
		const xBend = a.right + 22;
		const y2 = n.top + 11;
		const dot = document.createElementNS('http://www.w3.org/2000/svg', 'circle');
		dot.setAttribute('cx', String(x1));
		dot.setAttribute('cy', String(y1));
		dot.setAttribute('r', '2.5');
		dot.setAttribute('fill', '#b2492b');
		wires.appendChild(dot);
		const line = document.createElementNS('http://www.w3.org/2000/svg', 'path');
		line.setAttribute('d', 'M' + x1 + ' ' + y1 + ' H ' + xBend + ' L ' + (n.left - 6) + ' ' + y2);
		line.setAttribute('pathLength', '1');
		wires.appendChild(line);
		requestAnimationFrame(() => line.classList.add('on'));
	}

	let active = '';

	function noteOf(key: string): HTMLElement | null
	{
		return document.getElementById('kw-' + key);
	}

	function choose(key: string, word: HTMLElement): void
	{
		const note = noteOf(key);
		if (note === null || gloss === null)
		{
			return;
		}
		active = key;
		for (const other of all('.kwe', gloss))
		{
			other.classList.toggle('on', other === note);
		}
		for (const other of all('a.term', article))
		{
			other.classList.toggle('active', other === word);
		}
		if (narrow())
		{
			// On narrow screens the notes open as a sheet from the bottom
			gloss.classList.add('open');
			const head = (gloss.querySelector('.kwhead') as HTMLElement).offsetHeight;
			gloss.scrollTo({ top: Math.max(0, note.offsetTop - head - 10), behavior: 'smooth' });
		}
		else
		{
			wire(word, note);
			const r = note.getBoundingClientRect();
			if (r.top < 70 || r.bottom > window.innerHeight)
			{
				window.scrollBy({ top: r.top - window.innerHeight / 3, behavior: 'smooth' });
			}
		}
	}

	for (const word of all('a.term', article))
	{
		const key = String(word.dataset.term);
		word.addEventListener('click', (e) =>
		{
			e.preventDefault();
			e.stopPropagation();
			choose(key, word);
		});
		word.addEventListener('mouseenter', () =>
		{
			if (!narrow() && active !== key)
			{
				wire(word, noteOf(key));
			}
		});
		word.addEventListener('mouseleave', () =>
		{
			if (narrow() || active === key)
			{
				return;
			}
			const kept = article.querySelector('a.term.active');
			if (active !== '' && kept !== null)
			{
				wire(kept, noteOf(active));
			}
			else
			{
				wires.innerHTML = '';
			}
		});
	}

	if (gloss !== null)
	{
		const sheet = gloss;
		(sheet.querySelector('.kwclose') as HTMLElement).addEventListener('click', () => sheet.classList.remove('open'));
		document.addEventListener('click', (e) =>
		{
			if (narrow() && sheet.classList.contains('open') && !sheet.contains(e.target as Node))
			{
				sheet.classList.remove('open');
			}
		});
	}

	function layout(): void
	{
		place();
		bars();
		if (active !== '')
		{
			wire(article.querySelector('a.term.active'), noteOf(active));
		}
	}
	layout();
	window.addEventListener('load', layout);
	window.addEventListener('resize', layout);
	document.fonts.ready.then(layout);
	// Answers and hints change the height of the text, so the notes and bars follow
	new MutationObserver(() => window.setTimeout(layout, 480)).observe(article, { attributes: true, subtree: true, attributeFilter: ['class'] });
	new ResizeObserver(() => layout()).observe(article);
}

export function setUpLearnPage(): void
{
	setUpBranches();
	setUpPrediction();
	setUpAnswers();
	setUpTicks();
	setUpSymbols();
	setUpMargins();
	// The page fades in from the paper colour
	requestAnimationFrame(() => document.body.classList.add('ready'));
}
