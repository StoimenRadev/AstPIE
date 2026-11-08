// The case desks and the observatory, drawn on the server (components/explore/DeskPage.astro).
// What the visitor opens later is made here as well and waits in the page as text (see template()):
// every page of a case, everything that can be picked up, the people's files, the photo folder and
// every page of the logbook. scripts/desk/desk.ts only puts these in place in the browser.
//
// Positions are in desk units. A wider window makes the desk wider than 1600 units; the stage then
// sets --dx (how much wider) and things move with calc(... var(--dx)).
import { CLIP, DESKS, PIN, PROPS, REEL, daysSvg, drawingSvg, paperChart, sealSvg, sketchSvg } from './deskArt';
import type { DeskCase, DeskItem, DeskObject, DeskPart, DeskPerson, Plate } from './deskTypes';
import { emptyItem } from './desk';
import { fill, t } from '../i18n/t';
import type { TextKey } from '../i18n/t';

export interface DeskView
{
	bodyClass: string;
	desk: string;
	caseName: string;
	// A case: the summary is what the page opens on
	tabs: string;
	sheet: string;
	items: string;
	tapes: string;
	manyTapes: boolean;
	radio: string;
	radioLow: number;
	radioHigh: number;
	photoBox: string;
	// The observatory: the whole log is what the page opens on
	catLeft: string;
	catRight: string;
	plate: string;
	plateCaption: string;
	instruments: string;
	plateBox: string;
	// Everything shown later (see template())
	templates: string;
}

function emptyView(bodyClass: string): DeskView
{
	return {
		bodyClass: bodyClass, desk: '', caseName: '', tabs: '', sheet: '', items: '', tapes: '', manyTapes: false,
		radio: '', radioLow: 0, radioHigh: 1, photoBox: '', catLeft: '', catRight: '', plate: '', plateCaption: '',
		instruments: '', plateBox: '', templates: ''
	};
}

function esc(text: string): string
{
	return String(text).replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/"/g, '&quot;');
}

// The same Wikimedia picture, bigger, for picking it up
function bigger(src: string): string
{
	return src.replace(/\?width=\d+/, '?width=2000');
}

function plainSrc(src: string): string
{
	return src.replace(/\?width=\d+/, '');
}

function photoStyle(src: string): string
{
	if (src === '')
	{
		return '';
	}
	return 'style="background-image:url(\'' + src + '\')"';
}

// Something shown later waits as plain text in <script type="text/html">: the browser does not turn
// it into elements until it is opened (a <template> would be, and a case has over 100 of them).
// "</script" and "<!--" would end or confuse the script element, so they get a backslash
// (scripts/desk/desk.ts takes it out again).
function template(id: string, html: string, attributes: string): string
{
	const text = html.split('</script').join('<\\/script').split('<!--').join('<\\!--');
	return '<script type="text/html" id="' + id + '"' + attributes + '>' + text + '</script>';
}

// Everything that opens over the desk gets a number and waits in template "ov-N"
function addOverlay(overlays: string[], html: string): number
{
	overlays.push(html);
	return overlays.length - 1;
}

function overlayTemplates(overlays: string[]): string
{
	let html = '';
	overlays.forEach((overlay, i) =>
	{
		html += template('ov-' + i, overlay, '');
	});
	return html;
}

// ---------------------------------------------------------------- the desk itself

// Exactly as wide as the desk: the sizing script in DeskPage.astro sets the viewBox to the desk's
// width before the first paint (the grain filters are costly, so no wood is drawn beyond the window)
function woodSvg(): string
{
	let planks = '';
	for (let y = 235; y < 940; y += 235)
	{
		planks += '<line x1="0" y1="' + y + '" x2="100%" y2="' + y + '" stroke="var(--wood-dark)" stroke-width="3" opacity=".55"/><line x1="0" y1="' + (y + 2) + '" x2="100%" y2="' + (y + 2) + '" stroke="#fff" stroke-width="1" opacity=".05"/>';
	}
	return '<svg class="wood" id="wood" width="100%" height="100%" viewBox="0 0 1600 940" preserveAspectRatio="none"><defs>'
		+ '<filter id="grain" x="0" y="0" width="100%" height="100%"><feTurbulence type="fractalNoise" baseFrequency="0.0018 0.075" numOctaves="4" seed="4"/><feColorMatrix type="matrix" values="0 0 0 0 0  0 0 0 0 0  0 0 0 0 0  0 0 0 -2.2 1.45"/></filter>'
		+ '<filter id="grain2" x="0" y="0" width="100%" height="100%"><feTurbulence type="fractalNoise" baseFrequency="0.004 0.22" numOctaves="2" seed="11"/><feColorMatrix type="matrix" values="0 0 0 0 1  0 0 0 0 .9  0 0 0 0 .75  0 0 0 -2.6 1.5"/></filter></defs>'
		+ '<rect width="100%" height="940" style="fill:var(--wood)"/>'
		+ '<rect width="100%" height="940" filter="url(#grain)" style="opacity:var(--grain)"/>'
		+ '<rect width="100%" height="940" filter="url(#grain2)" opacity=".07"/><g class="planks">' + planks + '</g></svg>';
}

function propsHtml(deskId: string): string
{
	let html = '';
	for (const name of DESKS[deskId].props)
	{
		const prop = PROPS[name];
		let classes = 'prop';
		if (name.startsWith('lamp'))
		{
			classes += ' lamp';
		}
		if (name === 'corkBoard')
		{
			classes += ' board';
		}
		let left = prop.x + 'px';
		if (prop.anchor === 'right')
		{
			left = 'calc(' + prop.x + 'px + var(--dx))';
		}
		if (prop.anchor === 'middle')
		{
			left = 'calc(' + prop.x + 'px + var(--dx) / 2)';
		}
		let width = prop.w + 'px';
		if (prop.anchor === 'stretch')
		{
			width = 'calc(' + prop.w + 'px + var(--dx))';
		}
		html += '<div class="' + classes + '" style="left:' + left + ';top:' + prop.y + 'px;width:' + width + ';height:' + prop.h + 'px;transform:rotate(' + prop.r + 'deg)">' + prop.svg() + '</div>';
	}
	return html;
}

// ---------------------------------------------------------------- folder, tabs and sheets

interface Section
{
	name: string;
	// Page numbers, counting the summary as 0
	from: number;
	to: number;
	sim: boolean;
}

function sectionsOf(c: DeskCase): Section[]
{
	const list: Section[] = [];
	c.parts.forEach((part, k) =>
	{
		const last = list[list.length - 1];
		if (last !== undefined && last.name === part.section)
		{
			last.to = k + 1;
		}
		else
		{
			list.push({ name: part.section, from: k + 1, to: k + 1, sim: part.sim });
		}
	});
	return list;
}

// Each section has its own colour, like the dividers in a ring binder; "Try it yourself" is always green
const SECTION_COLOURS = ['#a9503c', '#bf8a2f', '#6b8746', '#3b7a86', '#4a689c', '#80598a', '#94653f', '#5c6873'];

function sectionColour(c: DeskCase, section: Section): string
{
	if (section.sim)
	{
		return '#2f7a4a';
	}
	const k = sectionsOf(c).findIndex((s) => s.name === section.name);
	return SECTION_COLOURS[Math.max(0, k) % SECTION_COLOURS.length];
}

function sectionOfPart(c: DeskCase, i: number): Section | undefined
{
	return sectionsOf(c).find((s) => i >= s.from && i <= s.to);
}

// Tabs share the height of the folder edge, down to just above the photo folder
function tabHeight(count: number): number
{
	return Math.min(96, Math.floor(632 / count) - 6);
}

// On the summary: the summary tab and one coloured tab per section.
// Inside a section: a tab back to all sections, then one tab per page of that section.
function tabsHtml(c: DeskCase, i: number): string
{
	const current = sectionOfPart(c, i);
	const sections = sectionsOf(c);
	let html = '';
	if (i === 0 || current === undefined)
	{
		const h = tabHeight(sections.length + 1);
		html += '<button class="tab sect summary on" style="height:' + h + 'px" data-go="0">' + t('desk.caseSummary') + '</button>';
		for (const s of sections)
		{
			let classes = 'tab sect';
			if (s.sim)
			{
				classes += ' sim';
			}
			let from = '';
			if (s.to > s.from)
			{
				from = '<small>' + fill(t('desk.fromPage'), { page: esc(c.parts[s.from - 1].tab) }) + '</small>';
			}
			html += '<button class="' + classes + '" style="height:' + h + 'px;--c:' + sectionColour(c, s) + '" data-go="' + s.from + '"><b>' + esc(s.name) + '</b>' + from + '</button>';
		}
		return html;
	}
	const h = tabHeight(current.to - current.from + 2);
	html += '<button class="tab back" style="height:' + h + 'px" data-go="0"><b>&larr; ' + t('desk.allSections') + '</b><small>' + fill(t('desk.nowIn'), { section: esc(current.name) }) + '</small></button>';
	for (let p = current.from; p <= current.to; p++)
	{
		let classes = 'tab page';
		if (p === i)
		{
			classes += ' on';
		}
		html += '<button class="' + classes + '" style="height:' + h + 'px;--c:' + sectionColour(c, current) + '" data-go="' + p + '">' + esc(c.parts[p - 1].tab) + '</button>';
	}
	return html;
}

const OUTCOMES: Record<string, { text: TextKey; inner: TextKey }> = {
	success: { text: 'desk.outcome.success', inner: 'desk.seal.success' },
	partial: { text: 'desk.outcome.partial', inner: 'desk.seal.partial' },
	ongoing: { text: 'desk.outcome.ongoing', inner: 'desk.seal.ongoing' },
	failure: { text: 'desk.outcome.failure', inner: 'desk.seal.failure' }
};

function coverHtml(c: DeskCase): string
{
	const desk = DESKS[c.desk];
	let fields = '';
	for (const row of c.fields)
	{
		fields += '<div class="row"><span class="k">' + esc(row[0]) + '</span><span class="v">' + esc(row[1]) + '</span></div>';
	}
	let mark = '';
	if (c.status.word !== '')
	{
		const word = c.status.word;
		fields += '<div class="row"><span class="k">' + t('desk.status') + '</span><span class="v">' + esc(word.charAt(0) + word.slice(1).toLowerCase()) + '</span></div>';
		mark = '<div class="bigstamp ' + word.toLowerCase().replace(/\s+/g, '-') + '">' + esc(word) + '<small>' + esc(c.status.note) + '</small></div>';
	}
	else
	{
		const outcome = OUTCOMES[c.outcome];
		let outer = desk.seals.success;
		if (c.outcome in desk.seals)
		{
			outer = desk.seals[c.outcome];
		}
		fields += '<div class="row"><span class="k">' + t('desk.outcome') + '</span><span class="v">' + t(outcome.text) + '</span></div>';
		mark = '<div class="seal">' + sealSvg(outer, t(outcome.inner)) + '</div>';
	}
	let titleClass = '';
	if (c.title.length > 10)
	{
		titleClass = 'long';
	}
	let first = '';
	if (c.parts.length > 0)
	{
		first = c.parts[0].section;
	}
	return '<div class="cover"><div class="sheet-head"><span>' + t('desk.archiveName') + '</span><span class="stamp-mark">' + desk.archiveStamp + '</span></div>'
		+ '<div class="idphoto">' + CLIP + '<img src="' + c.cover.src + '" alt="' + esc(c.cover.caption) + '"><span>' + esc(c.cover.caption) + '</span></div>'
		+ '<h1 class="' + titleClass + '">' + esc(c.title) + '</h1><div class="tagline">' + esc(c.tagline) + '</div><div class="fields">' + fields + '</div>'
		+ '<p class="summary">' + c.summary + '</p>' + mark
		+ '<button class="open-btn" data-go="1">' + fill(t('desk.openCase'), { section: esc(first) }) + ' &rarr;</button><div class="open-hint">' + t('desk.orPickSection') + '</div></div>';
}

function partHtml(c: DeskCase, i: number): string
{
	const desk = DESKS[c.desk];
	const part = c.parts[i - 1];
	const section = sectionOfPart(c, i) as Section;
	let body = part.text;
	if (part.sim && c.simulation !== null)
	{
		body += '<p><a href="/learn/' + c.simulation.topicSlug + '" class="learnlink">' + fill(t('desk.readInLearn'), { topic: esc(c.simulation.topicTitle) }) + ' &rarr;</a></p>';
	}
	let margin = '';
	if (part.margin !== '')
	{
		margin = '<div class="margin-note">' + esc(part.margin) + '</div>';
	}
	let previous = '<button class="prev" data-go="0">&larr; ' + t('desk.allSections') + '</button>';
	if (i > section.from)
	{
		previous = '<button class="prev" data-go="' + (i - 1) + '">&larr; ' + esc(c.parts[i - 2].tab) + '</button>';
	}
	let next = '<span></span>';
	if (i < section.to)
	{
		next = '<button data-go="' + (i + 1) + '">' + esc(c.parts[i].tab) + ' &rarr;</button>';
	}
	else if (i < c.parts.length)
	{
		next = '<button data-go="' + (i + 1) + '">' + fill(t('desk.nextSection'), { section: esc(c.parts[i].section) }) + ' &rarr;</button>';
	}
	return '<div class="sheet-head"><span>' + esc(c.title) + ' <i class="secchip" style="--c:' + sectionColour(c, section) + '">' + esc(part.section) + '</i></span><span class="stamp-mark">' + desk.archiveStamp + '</span></div>'
		+ '<h1>' + esc(part.title) + '</h1><div class="where">' + esc(part.where) + '</div>' + body + margin
		+ '<div class="turn">' + previous + next + '</div>';
}

// ---------------------------------------------------------------- evidence on the desk

const WIDTH: Record<string, number> = { film: 330, quote: 300, photo: 320, doc: 300, drawing: 370, svgdrawing: 420, printout: 340, note: 230, card: 312, days: 330, sketch: 340, person: 200, monitor: 610, theory: 300, clipping: 262 };
// Where 1 to 4 things land on the desk: left, top and a slight turn
const SLOTS: Record<number, number[][]> = {
	1: [[985, 52, -2]],
	2: [[958, 44, -3], [1262, 214, 3]],
	3: [[948, 34, -3], [1268, 206, 4], [990, 392, 2]],
	4: [[944, 30, -4], [1266, 200, 3], [980, 384, 2], [1236, 420, -5]]
};
// On the cork board everything stays inside the frame
const BOARD: Record<number, number[][]> = {
	1: [[1050, 70, -2]],
	2: [[920, 60, -3], [1250, 150, 3]],
	3: [[912, 56, -3], [1262, 64, 3], [1150, 334, 2]],
	4: [[908, 50, -3], [1250, 56, 3], [918, 300, 2], [1264, 300, -3]]
};
const PIN_COLOURS = ['#c22b1f', '#2f4f8f', '#d9a72b', '#2f7a4a'];

function personOf(c: DeskCase, id: string): DeskPerson | undefined
{
	return c.people.find((p) => p.id === id);
}

function stampClass(word: string): string
{
	const w = word.toLowerCase();
	if (w.includes('rejected') || w.includes('ruled') || w.includes('does not') || w.includes('unlikely'))
	{
		return 'no';
	}
	if (w.includes('confirmed') || w.includes('very likely'))
	{
		return 'yes';
	}
	return 'maybe';
}

function evidenceHtml(item: DeskItem, c: DeskCase): string
{
	if (item.type === 'photo')
	{
		let label = item.label;
		if (label === '')
		{
			label = item.title;
		}
		return '<div class="body"><div class="tape-strip"></div><img src="' + item.src + '" alt="' + esc(item.title) + '"><div class="label">' + esc(label) + '</div></div>';
	}
	if (item.type === 'doc')
	{
		return '<div class="body">' + CLIP + '<img src="' + item.src + '" alt="' + esc(item.title) + '"></div>';
	}
	if (item.type === 'drawing')
	{
		let notes = '';
		if (item.notes.length > 0)
		{
			notes = '<div class="dnotes">' + item.notes.map(esc).join(' &middot; ') + '</div>';
		}
		return '<div class="body"><img src="' + item.src + '" alt="' + esc(item.title) + '">' + notes + '</div>';
	}
	if (item.type === 'svgdrawing')
	{
		return '<div class="body">' + drawingSvg(item.drawing) + '</div>';
	}
	if (item.type === 'printout')
	{
		return '<div class="body"><div class="ptitle">' + esc(item.title) + '</div>' + paperChart(item.chart) + '</div>';
	}
	if (item.type === 'note')
	{
		return '<div class="body">' + esc(item.text) + '</div>';
	}
	if (item.type === 'card')
	{
		let rows = '';
		for (const row of item.rows)
		{
			rows += '<div class="crow"><span>' + esc(row[0]) + '</span><span>' + esc(row[1]) + '</span></div>';
		}
		return '<div class="body"><h4>' + esc(item.title) + '</h4>' + rows + '</div>';
	}
	if (item.type === 'theory')
	{
		let picture = '';
		let bodyClass = 'body';
		if (item.src !== '')
		{
			let pictureClass = 'tpic';
			if (item.fit === 'contain')
			{
				pictureClass += ' contain';
			}
			picture = '<div class="' + pictureClass + '" ' + photoStyle(item.src) + '></div>';
			bodyClass += ' withpic';
		}
		return '<div class="' + bodyClass + '">' + picture + '<h4>' + esc(item.title) + '</h4><div class="by">' + esc(item.by) + '</div><div class="tstamp ' + stampClass(item.stamp) + '">' + esc(item.stamp) + '</div><div class="more">' + t('desk.pickUpTheory') + '</div></div>';
	}
	if (item.type === 'film')
	{
		return '<div class="body"><div class="reel"><div class="frame" ' + photoStyle(item.poster) + '><span class="play"></span></div></div><div class="ftag">' + t('desk.film') + ' &middot; ' + esc(item.length) + '</div><div class="ftitle">' + esc(item.title) + '</div></div>';
	}
	if (item.type === 'quote')
	{
		return '<div class="body"><div class="qhead">' + t('desk.transcript') + '</div><div class="qtext">&ldquo;' + esc(item.text) + '&rdquo;</div><div class="qwho">' + esc(item.who) + '</div><div class="qwhen">' + esc(item.when) + '</div></div>';
	}
	if (item.type === 'clipping')
	{
		let picture = '';
		if (item.src !== '')
		{
			picture = '<div class="cpic" ' + photoStyle(item.src) + '></div>';
		}
		return '<div class="body"><div class="cyear">' + esc(item.year) + '</div>' + picture + '<div class="chead">' + esc(item.head) + '</div><div class="ctext">' + esc(item.text) + '</div><div class="cout">' + esc(item.outcome) + '</div></div>';
	}
	if (item.type === 'days')
	{
		return '<div class="body"><div class="dtitle">' + esc(item.title) + '</div>' + daysSvg(item.from, item.to, item.loud, item.weekly) + '</div>';
	}
	if (item.type === 'sketch')
	{
		return '<div class="body">' + sketchSvg(item.sketch) + '</div>';
	}
	if (item.type === 'person')
	{
		const person = personOf(c, item.person);
		if (person === undefined)
		{
			return '';
		}
		let empty = '';
		if (person.photo === '')
		{
			empty = t('desk.noPhoto');
		}
		return '<div class="body">' + CLIP + '<div class="ph" ' + photoStyle(person.photo) + '>' + empty + '</div><div class="nm">' + esc(person.name) + '</div><div class="rl">' + esc(person.role) + '</div></div>';
	}
	if (item.type === 'monitor')
	{
		return monitorHtml(c);
	}
	return '';
}

// "Try it yourself": a monitor on the desk that runs the simulation of the case's Learn topic
function monitorHtml(c: DeskCase): string
{
	if (c.simulation === null)
	{
		return '<div class="body"><div class="screen"><div class="glass"></div></div><div class="knobs"><i></i><i></i></div></div>';
	}
	let picture = '';
	if (c.simulation.thumbnailUrl !== '')
	{
		picture = '<img class="shot" src="' + c.simulation.thumbnailUrl + '" alt="" onerror="this.remove()">';
	}
	return '<div class="body"><div class="screen">' + picture + '<div class="glass"></div>'
		+ '<button class="startbtn" aria-label="' + t('desk.startSimulation') + '" data-src="' + esc(c.simulation.buildUrl) + '" data-title="' + esc(c.simulation.title) + '"><svg viewBox="0 0 24 24"><path d="M6 4v16l14-8z" fill="#fff"/></svg></button>'
		+ '<div class="loading"><span>' + t('desk.startingSimulation') + '</span><div class="bar"><i></i></div></div></div>'
		+ '<div class="mlabel">' + esc(c.simulation.title) + '</div><div class="knobs"><i></i><i></i></div></div>';
}

function monitorItem(part: DeskPart): DeskItem
{
	const item = emptyItem('monitor');
	item.title = part.title;
	return item;
}

// The things of one page on the desk. data-x and data-shift let the browser move them when the
// desk gets wider: with the middle of the desk, or (on the cork board) across the stretched board.
function itemsHtml(list: DeskItem[], c: DeskCase, overlays: string[]): string
{
	const pinned = c.desk === 'mystery';
	let table = SLOTS;
	if (pinned)
	{
		table = BOARD;
	}
	let slots: number[][] = [];
	if (list.length > 0)
	{
		slots = table[Math.min(4, list.length)];
	}
	let html = '';
	list.forEach((item, k) =>
	{
		let base = slots[k];
		if (item.type === 'monitor')
		{
			base = [960, 70, -1];
		}
		let shift = 0.5;
		if (pinned)
		{
			shift = (base[0] - 872) / 724;
		}
		let classes = 'item ' + item.type;
		if (item.color !== '')
		{
			classes += ' ' + item.color;
		}
		if (item.photoLike)
		{
			classes += ' photoLike';
		}
		if (pinned)
		{
			classes += ' pinned';
		}
		// Photos are a little smaller on the cork board, so four pieces of evidence fit
		let w = 300;
		if (item.type in WIDTH)
		{
			w = WIDTH[item.type];
		}
		if (pinned && item.type === 'photo')
		{
			w = 250;
		}
		let opens = '';
		if (item.type === 'person')
		{
			opens = ' data-person="' + esc(item.person) + '"';
		}
		else
		{
			const overlay = itemOverlay(item);
			if (overlay !== '')
			{
				opens = ' data-ov="' + addOverlay(overlays, overlay) + '"';
			}
		}
		let pin = '';
		if (pinned)
		{
			pin = '<span class="pinwrap" style="color:' + PIN_COLOURS[k % 4] + '">' + PIN + '</span>';
		}
		const left = 'calc(' + base[0] + 'px + var(--dx) * ' + shift.toFixed(4) + ')';
		html += '<div class="' + classes + '" data-x="' + base[0] + '" data-shift="' + shift.toFixed(4) + '" data-w="' + w + '"' + opens
			+ ' style="width:' + w + 'px;left:' + left + ';top:' + base[1] + 'px;--r:' + base[2] + 'deg;z-index:' + (11 + k) + '">'
			+ evidenceHtml(item, c) + pin + '</div>';
	});
	return html;
}

// ---------------------------------------------------------------- picking things up

function pickHtml(big: string, title: string, caption: string, group: string, credit: string, button: string): string
{
	let text = '';
	if (caption !== '' && caption !== title)
	{
		text = '<p>' + esc(caption) + '</p>';
	}
	let filed = '';
	if (group !== '')
	{
		filed = '<p class="filed">' + fill(t('desk.filedUnder'), { group: esc(group) }) + '</p>';
	}
	return '<div class="pick"><div class="big">' + big + '</div><div class="info"><h3>' + esc(title) + '</h3>' + text + filed + '<div class="cr">' + esc(credit) + '</div>' + button + '</div></div>';
}

function putBackButton(): string
{
	return '<button class="close" data-close>' + t('desk.putBack') + '</button>';
}

function backToPhotosButton(): string
{
	return '<button class="close" data-back>&larr; ' + t('desk.backToPhotos') + '</button>';
}

// What opens when a thing on the desk is clicked ('' when nothing does). People open their file instead.
function itemOverlay(item: DeskItem): string
{
	if (item.type === 'theory')
	{
		let picture = '';
		if (item.src !== '')
		{
			let pictureClass = 'tphoto';
			if (item.fit === 'contain')
			{
				pictureClass += ' contain';
			}
			picture = '<img class="' + pictureClass + '" src="' + bigger(item.src) + '" alt="">';
		}
		let credit = '';
		if (item.credit !== '')
		{
			credit = '<div class="tcredit">' + esc(item.credit) + '</div>';
		}
		return '<div class="theory-big">' + picture + '<h3>' + esc(item.title) + '</h3><div class="by">' + esc(item.by) + '</div><div class="two"><div><h4>' + t('desk.supports') + '</h4><p>' + esc(item.forText) + '</p></div><div><h4>' + t('desk.against') + '</h4><p>' + esc(item.against) + '</p></div></div><div class="verdict"><span class="tstamp ' + stampClass(item.stamp) + '">' + esc(item.stamp) + '</span><span>' + esc(item.verdict) + '</span></div>' + credit + '<button class="close" data-close>' + t('desk.pinBack') + '</button></div>';
	}
	if (item.type === 'film')
	{
		return filmHtml(item, putBackButton());
	}
	if (item.type === 'monitor' || item.type === 'note' || item.type === 'card' || item.type === 'quote' || (item.type === 'clipping' && item.src === ''))
	{
		return '';
	}
	let big = '';
	if (item.src !== '')
	{
		big = '<img src="' + bigger(item.src) + '" alt="' + esc(item.title) + '">';
	}
	else if (item.type === 'printout')
	{
		big = '<div class="svgwrap">' + paperChart(item.chart) + '</div>';
	}
	else if (item.type === 'svgdrawing')
	{
		big = '<div class="svgwrap">' + drawingSvg(item.drawing) + '</div>';
	}
	else if (item.type === 'days')
	{
		big = '<div class="svgwrap">' + daysSvg(item.from, item.to, item.loud, item.weekly) + '</div>';
	}
	else if (item.type === 'sketch')
	{
		big = '<div class="svgwrap">' + sketchSvg(item.sketch) + '</div>';
	}
	return pickHtml(big, item.title, item.caption, item.group, item.credit, putBackButton());
}

// A person's file: photo, born, died, from, and their story. The "back to the photos" button
// is only kept when the file was opened from the photo folder.
function personHtml(person: DeskPerson, withBack: boolean): string
{
	let facts = '';
	if (person.born !== '')
	{
		facts += t('desk.born') + '&nbsp;&nbsp;' + esc(person.born) + '<br>';
	}
	if (person.died !== '')
	{
		facts += t('desk.died') + '&nbsp;&nbsp;' + esc(person.died) + '<br>';
	}
	if (person.from !== '')
	{
		facts += t('desk.from') + '&nbsp;&nbsp;' + esc(person.from);
	}
	if (facts !== '')
	{
		facts = '<div class="facts">' + facts + '</div>';
	}
	let empty = '';
	if (person.photo === '')
	{
		empty = t('desk.noPhoto');
	}
	let credit = '';
	if (person.photoCredit !== '')
	{
		credit = '<div class="pcredit">' + t('desk.photo') + ' ' + esc(person.photoCredit) + '</div>';
	}
	let back = '';
	if (withBack)
	{
		back = backToPhotosButton();
	}
	return '<div class="pfile-big"><div class="ph">' + CLIP + '<div ' + photoStyle(person.photo) + '>' + empty + '</div></div><div class="paper"><h2>' + esc(person.name) + '</h2><div class="role">' + esc(person.role) + '</div>' + facts + '<p class="bio">' + esc(person.bio) + '</p>' + credit + back + '<button class="close" data-close>' + t('desk.close') + '</button></div></div>';
}

// A film is picked up like a photo, but plays (scripts/desk/desk.ts stops the radio tape first)
function filmHtml(item: DeskItem, button: string): string
{
	let length = t('desk.filmLength');
	if (item.length !== '')
	{
		length += ' &middot; ' + esc(item.length);
	}
	return '<div class="pick filmpick"><div class="big"><video src="' + item.src + '" poster="' + item.poster + '" controls autoplay preload="metadata"></video></div><div class="info"><h3>' + esc(item.title) + '</h3><p>' + esc(item.caption) + '</p><p class="filed">' + length + '</p><div class="cr">' + esc(item.credit) + '</div>' + button + '</div></div>';
}

// ---------------------------------------------------------------- the folder of photos and films

interface FolderTab
{
	label: string;
	// The number on the tab: photos, films or people
	count: number;
	groups: PhotoGroup[];
}

interface PhotoGroup
{
	title: string;
	items: PhotoCard[];
}

interface PhotoCard
{
	src: string;
	title: string;
	caption: string;
	credit: string;
	dark: boolean;
	group: string;
	// A person's card opens their file; a film's card plays it
	person: string;
	film: DeskItem | null;
}

// What the folder holds so far, sorted into photos and films; each picture or film only once
interface Sorter
{
	photos: PhotoGroup[];
	films: PhotoGroup[];
	seen: Set<string>;
}

function cardOf(item: DeskItem, group: string): PhotoCard
{
	const card: PhotoCard = { src: item.src, title: item.title, caption: item.caption, credit: item.credit, dark: item.dark, group: group, person: '', film: null };
	if (item.type === 'film')
	{
		card.src = item.poster;
		card.dark = false;
		card.film = item;
	}
	if (card.title === '')
	{
		card.title = item.head;
	}
	if (card.caption === '')
	{
		card.caption = item.text;
	}
	if (card.caption === '')
	{
		card.caption = item.by;
	}
	return card;
}

// Puts the pictures and films of one page into the folder, under that page's name
function sortInto(sorter: Sorter, items: DeskItem[], group: string): void
{
	const photos: PhotoCard[] = [];
	const films: PhotoCard[] = [];
	for (const item of items)
	{
		const key = plainSrc(item.src);
		if (item.type === 'quote' || item.type === 'person' || item.src === '' || sorter.seen.has(key))
		{
			continue;
		}
		sorter.seen.add(key);
		if (item.type === 'film')
		{
			films.push(cardOf(item, group));
		}
		else
		{
			photos.push(cardOf(item, group));
		}
	}
	if (photos.length > 0)
	{
		sorter.photos.push({ title: group, items: photos });
	}
	if (films.length > 0)
	{
		sorter.films.push({ title: group, items: films });
	}
}

// The folder lists its photos (and its films) as one collection, without headings: the desk already
// shows them page by page. A case's folder: the photos of the case summary first, then the photos kept
// only in the folder that show people of the case, then those of each page in order, then the rest.
function caseFolder(c: DeskCase): FolderTab[]
{
	const sorter: Sorter = { photos: [], films: [], seen: new Set<string>() };
	const cover = emptyItem('photo');
	cover.src = c.cover.src;
	cover.title = c.cover.caption;
	cover.caption = c.cover.caption;
	cover.credit = c.cover.credit;
	const ofPeople = c.extraPhotos.filter((item) => item.shows.length > 0);
	const others = c.extraPhotos.filter((item) => item.shows.length === 0);
	sortInto(sorter, [cover].concat(c.summaryEvidence, ofPeople), '');
	for (const part of c.parts)
	{
		sortInto(sorter, part.evidence, '');
	}
	sortInto(sorter, others, '');

	// One card per person, with their portrait; it opens their file
	const people: PhotoCard[] = c.people.map((person) =>
	{
		return { src: person.photo, title: person.name, caption: person.role, credit: '', dark: false, group: '', person: person.id, film: null };
	});

	const photos = cardsOf(sorter.photos);
	const tabs: FolderTab[] = [{ label: t('desk.photosTab'), count: photos.length, groups: [{ title: '', items: photos }] }];
	const films = cardsOf(sorter.films);
	if (films.length > 0)
	{
		tabs.push({ label: t('desk.filmsTab'), count: films.length, groups: [{ title: '', items: films }] });
	}
	tabs.push({ label: t('desk.people'), count: people.length, groups: [{ title: '', items: people }] });
	return tabs;
}

// All the cards of some groups, in one list
function cardsOf(groups: PhotoGroup[]): PhotoCard[]
{
	let cards: PhotoCard[] = [];
	for (const group of groups)
	{
		cards = cards.concat(group.items);
	}
	return cards;
}

// The observatory's folder: the object's photographs and the instruments, and its films
function objectFolder(o: DeskObject): FolderTab[]
{
	const photos: PhotoCard[] = [];
	const films: PhotoCard[] = [];
	const seen = new Set<string>();
	const plates: Plate[] = [o.plate];
	for (const entry of o.log)
	{
		if (entry.plate !== null)
		{
			plates.push(entry.plate);
		}
	}
	for (const plate of plates)
	{
		if (plate.src === '' || seen.has(plainSrc(plate.src)))
		{
			continue;
		}
		seen.add(plainSrc(plate.src));
		if (plate.kind === 'image')
		{
			const item = emptyItem('photo');
			item.src = plate.src;
			item.title = plate.title;
			item.caption = plate.caption;
			item.credit = plate.credit;
			item.dark = plate.dark;
			photos.push(cardOf(item, ''));
		}
		if (plate.kind === 'video')
		{
			const item = emptyItem('film');
			item.src = plate.src;
			item.poster = plate.poster;
			item.title = plate.title;
			item.caption = plate.caption;
			item.credit = plate.credit;
			films.push(cardOf(item, ''));
		}
	}
	for (const instrument of o.instruments)
	{
		const role = instrument.role.charAt(0).toUpperCase() + instrument.role.slice(1);
		photos.push({ src: instrument.photo, title: instrument.name, caption: role + '. ' + instrument.text, credit: t('desk.commonsCredit'), dark: instrument.dark, group: '', person: '', film: null });
	}
	const tabs: FolderTab[] = [{ label: t('desk.photosTab'), count: photos.length, groups: [{ title: '', items: photos }] }];
	if (films.length > 0)
	{
		tabs.push({ label: t('desk.filmsTab'), count: films.length, groups: [{ title: '', items: films }] });
	}
	return tabs;
}

// "1 photo" or "12 photos"
function countText(n: number, one: TextKey, many: TextKey): string
{
	if (n === 1)
	{
		return t(one);
	}
	return fill(t(many), { n: String(n) });
}

// The folder on the desk: just a closed folder with a label saying how much is inside
function photoBoxHtml(name: string, tabs: FolderTab[]): string
{
	let photos = 0;
	let films = 0;
	for (const tab of tabs)
	{
		for (const group of tab.groups)
		{
			for (const card of group.items)
			{
				if (card.film !== null)
				{
					films++;
				}
				else if (card.src !== '')
				{
					photos++;
				}
			}
		}
	}
	let what = countText(photos, 'desk.onePhoto', 'desk.manyPhotos');
	if (films > 0)
	{
		what += ' &middot; ' + countText(films, 'desk.oneFilm', 'desk.manyFilms');
	}
	return '<span class="fback"></span><span class="ftab">' + esc(name) + '</span><span class="ffront"><span class="flabel"><b>' + t('desk.folderLabel') + '</b><i>' + fill(t('desk.inside'), { what: what }) + '</i></span></span>';
}

// The open folder, showing one of its tabs. Each card opens a person's file, a film or a photo,
// each with a button back to the folder.
function folderHtml(name: string, tabs: FolderTab[], shown: number, overlays: string[]): string
{
	let body = '';
	for (const group of tabs[shown].groups)
	{
		let heading = '';
		if (group.title !== '')
		{
			heading = '<h4>' + esc(group.title) + '</h4>';
		}
		body += '<div class="pgroup">' + heading + '<div class="pgrid">';
		for (const item of group.items)
		{
			let missing = t('desk.noPhoto');
			if (item.film !== null)
			{
				missing = t('desk.film');
			}
			let picture = '<span class="pimg none">' + missing + '</span>';
			if (item.src !== '')
			{
				picture = '<span class="pimg" ' + photoStyle(item.src) + '></span>';
			}
			let classes = 'pthumb';
			let role = '';
			let opens = '';
			if (item.person !== '')
			{
				role = '<span class="pr">' + esc(item.caption) + '</span>';
				opens = ' data-person="' + esc(item.person) + '"';
			}
			else if (item.film !== null)
			{
				classes += ' film';
				opens = ' data-ov="' + addOverlay(overlays, filmHtml(item.film, backToPhotosButton())) + '"';
			}
			else
			{
				let style = '';
				if (item.dark)
				{
					style = ' style="background:#0b0f21"';
				}
				const big = '<img src="' + bigger(item.src) + '" alt="' + esc(item.title) + '"' + style + '>';
				opens = ' data-ov="' + addOverlay(overlays, pickHtml(big, item.title, item.caption, item.group, item.credit, backToPhotosButton())) + '"';
			}
			if (item.dark)
			{
				classes += ' dark';
			}
			// A photo or film: its short description under the title
			let description = '';
			if (item.person === '' && item.caption !== '' && item.caption !== item.title)
			{
				description = '<span class="pd">' + esc(item.caption) + '</span>';
			}
			body += '<button class="' + classes + '"' + opens + '>' + picture + '<span class="pt">' + esc(item.title) + '</span>' + role + description + '</button>';
		}
		body += '</div></div>';
	}
	if (body === '')
	{
		body = '<p class="empty">' + t('desk.nothingYet') + '</p>';
	}
	let buttons = '';
	tabs.forEach((tab, k) =>
	{
		let classes = '';
		if (k === shown)
		{
			classes = 'on';
		}
		buttons += '<button class="' + classes + '" data-folder="' + k + '">' + tab.label + ' (' + tab.count + ')</button>';
	});
	return '<div class="photoarch"><div class="ph-top"><h2>' + t('desk.folderTitle') + ' &middot; ' + esc(name) + '</h2><div class="ph-tabs">' + buttons + '</div></div><div class="ph-body">' + body + '</div><button class="close" data-close>' + t('desk.closeFolder') + '</button></div>';
}

function folderTemplates(name: string, tabs: FolderTab[], overlays: string[]): string
{
	let html = '';
	tabs.forEach((tab, k) =>
	{
		html += template('folder-' + k, folderHtml(name, tabs, k, overlays), '');
	});
	return html;
}

// ---------------------------------------------------------------- the radio and its tapes

function tapesHtml(c: DeskCase): string
{
	let html = '';
	for (const tape of c.tapes)
	{
		html += '<button class="tapebox" data-src="' + esc(tape.src) + '" data-synth="' + esc(tape.synth) + '" data-label="' + esc(tape.label) + '" title="' + esc(tape.credit) + '">' + REEL + '<span class="tl">' + esc(tape.label) + '</span></button>';
	}
	return html;
}

// The radio, tuned to 30% of its range (scripts/desk/radio.ts moves the needle from there)
function radioHtml(c: DeskCase): string
{
	const lo = c.radioRange[0];
	const hi = c.radioRange[1];
	let ticks = '';
	for (let k = 0; k <= 30; k++)
	{
		const f = lo + k * (hi - lo) / 30;
		const x = 14 + k / 30 * 298;
		let bottom = 54;
		let width = .9;
		if (k % 5 === 0)
		{
			bottom = 62;
			width = 1.6;
			ticks += '<text x="' + x.toFixed(1) + '" y="36" text-anchor="middle" font-family="PT Mono" font-size="11" fill="#3a2f22">' + Math.round(f) + '</text>';
		}
		ticks += '<line x1="' + x.toFixed(1) + '" y1="44" x2="' + x.toFixed(1) + '" y2="' + bottom + '" stroke="#3a2f22" stroke-width="' + width + '"/>';
	}
	const fraction = 0.3;
	let decimals = 3;
	if (hi > 1000)
	{
		decimals = 1;
	}
	const freq = (lo + (hi - lo) * fraction).toFixed(decimals) + ' MHz';
	let label = t('desk.noTapes');
	if (c.tapes.length > 0)
	{
		label = t('desk.tapeNone');
	}
	return '<div class="dialwin"><svg viewBox="0 0 326 76" preserveAspectRatio="none">' + ticks + '</svg><div class="needle" id="needle" style="left:' + (4.3 + fraction * 91.4).toFixed(2) + '%"></div><div class="freq" id="freq">' + freq + '</div></div>'
		+ '<div class="rrow"><canvas class="scope" id="scope" width="224" height="224"></canvas><div class="knobwrap"><button class="knob" id="knob" aria-label="' + t('desk.tuningKnob') + '" style="transform:rotate(' + (fraction * 300 - 150) + 'deg)"><span class="grip"></span></button><span class="rlab">' + t('desk.tuning') + '</span></div><div class="speaker"></div></div>'
		+ '<div class="rbtns"><button class="power" id="power"><span class="sw"></span><span class="rlab">' + t('desk.power') + '</span></button><span class="lampled"></span><span class="tapewin" id="tapewin" data-none="' + esc(label) + '" data-playing="' + esc(t('desk.playing')) + '">' + label + '</span></div>'
		+ '<div class="radio-tag">' + t('desk.radioNote') + '</div>';
}

// ---------------------------------------------------------------- a case

export function caseView(c: DeskCase): DeskView
{
	const view = emptyView('desk-' + c.desk + ' mode-case');
	const overlays: string[] = [];
	let templates = '';
	view.desk = woodSvg() + propsHtml(c.desk);
	view.caseName = t('desk.casePrefix') + ' <b>' + esc(c.title) + '</b>';

	// Every page of the case, the summary being page 0. The page opens on the summary.
	for (let i = 0; i <= c.parts.length; i++)
	{
		let evidence: DeskItem[] = c.summaryEvidence;
		let sheet = '';
		let hint = false;
		if (i === 0)
		{
			sheet = coverHtml(c);
		}
		else
		{
			const part = c.parts[i - 1];
			sheet = partHtml(c, i);
			evidence = part.evidence;
			hint = part.radioHint;
			if (part.sim)
			{
				evidence = [monitorItem(part)];
			}
		}
		const tabs = tabsHtml(c, i);
		const items = itemsHtml(evidence, c, overlays);
		if (i === 0)
		{
			view.tabs = tabs;
			view.sheet = sheet;
			view.items = items;
		}
		let hintValue = '0';
		if (hint)
		{
			hintValue = '1';
		}
		templates += template('page-' + i, '<div class="t-tabs">' + tabs + '</div><div class="t-sheet">' + sheet + '</div><div class="t-items">' + items + '</div>', ' data-hint="' + hintValue + '"');
	}

	for (const person of c.people)
	{
		templates += template('person-' + esc(person.id), personHtml(person, true), '');
	}

	const folder = caseFolder(c);
	view.photoBox = photoBoxHtml(c.title, folder);
	templates += folderTemplates(c.title, folder, overlays);

	view.tapes = tapesHtml(c);
	view.manyTapes = c.tapes.length > 1;
	view.radio = radioHtml(c);
	view.radioLow = c.radioRange[0];
	view.radioHigh = c.radioRange[1];
	view.templates = templates + overlayTemplates(overlays);
	return view;
}

// ---------------------------------------------------------------- the observatory (catalogue)

function plateHtml(plate: Plate): string
{
	if (plate.kind === 'video')
	{
		return '<video src="' + plate.src + '" poster="' + plate.poster + '" controls autoplay muted loop playsinline preload="auto"></video>';
	}
	if (plate.kind === 'chart')
	{
		return '<div class="paperplate">' + paperChart(plate.chart) + '</div>';
	}
	let dark = '';
	if (plate.dark)
	{
		dark = ' class="dark"';
	}
	return '<img src="' + plate.src + '" alt="' + esc(plate.title) + '"' + dark + '>';
}

function bigPlate(plate: Plate): string
{
	if (plate.kind === 'chart')
	{
		return '<div class="svgwrap">' + paperChart(plate.chart) + '</div>';
	}
	if (plate.kind === 'video')
	{
		return '<video src="' + plate.src + '" poster="' + plate.poster + '" controls autoplay style="max-width:100%;max-height:80vh"></video>';
	}
	return '<img src="' + bigger(plate.src) + '" alt="' + esc(plate.title) + '">';
}

// The plate on the light table; clicking it picks it up
function mountHtml(plate: Plate, overlays: string[]): string
{
	const overlay = addOverlay(overlays, pickHtml(bigPlate(plate), plate.title, plate.caption, '', plate.credit, putBackButton()));
	return '<div class="mount" data-ov="' + overlay + '">' + plateHtml(plate) + '</div>';
}

function plateCaptionHtml(plate: Plate): string
{
	return '<b>' + esc(plate.title) + '</b><span>' + esc(plate.caption) + '</span><small>' + esc(plate.credit) + '</small>';
}

// The right page of the logbook: the whole log, or one entry opened as a full page
function logListHtml(o: DeskObject): string
{
	let log = '<div class="lhead">' + t('desk.observationLog') + '</div>';
	o.log.forEach((entry, i) =>
	{
		let tag = '<span class="tagnote">' + esc(entry.note) + '</span>';
		if (entry.body.length > 0)
		{
			tag = '<span class="has">' + t('desk.readFullEntry') + '</span>';
		}
		log += '<button class="entry" data-entry="' + i + '"><span class="edate">' + esc(entry.date) + '</span><span class="ebody"><b>' + esc(entry.title) + '</b><span>' + esc(entry.text) + '</span>' + tag + '</span></button>';
	});
	return log + '<div class="lend">' + t('desk.logContinues') + '</div>';
}

function logPageHtml(o: DeskObject, i: number): string
{
	const entry = o.log[i];
	let body = '';
	let paragraphs = entry.body;
	if (paragraphs.length === 0)
	{
		paragraphs = [esc(entry.text)];
	}
	for (const paragraph of paragraphs)
	{
		body += '<p>' + paragraph + '</p>';
	}
	let facts = '';
	for (const fact of entry.facts)
	{
		facts += '<div class="lrow"><span>' + esc(fact[0]) + '</span><span>' + esc(fact[1]) + '</span></div>';
	}
	if (facts !== '')
	{
		facts = '<div class="afacts"><div class="afhead">' + t('desk.inShort') + '</div>' + facts + '</div>';
	}
	let plate = '';
	if (entry.plate !== null)
	{
		plate = '<div class="aplate">' + t('desk.onLightTable') + ' <b>' + esc(entry.plate.title) + '</b></div>';
	}
	let earlier = '<span></span>';
	if (i > 0)
	{
		earlier = '<button class="anav-b" data-entry="' + (i - 1) + '">&larr; ' + esc(o.log[i - 1].title) + '</button>';
	}
	let later = '<span></span>';
	if (i < o.log.length - 1)
	{
		later = '<button class="anav-b next" data-entry="' + (i + 1) + '">' + esc(o.log[i + 1].title) + ' &rarr;</button>';
	}
	let where = '';
	if (entry.where !== '')
	{
		where = '<div class="awhere">' + esc(entry.where) + '</div>';
	}
	return '<div class="logpage"><button class="lback" data-entry="-1">&larr; ' + t('desk.wholeLog') + '</button>'
		+ '<div class="adate">' + esc(entry.date) + '</div><h2>' + esc(entry.title) + '</h2>' + where + body + facts + plate
		+ '<div class="anav">' + earlier + later + '</div></div>';
}

// The log index on the left page, with entry i marked (-1: none, the whole log is open)
function logIndexHtml(o: DeskObject, i: number): string
{
	let index = '<div class="lihead">' + t('desk.entryByEntry') + '</div>';
	o.log.forEach((entry, k) =>
	{
		let classes = 'ix';
		if (k === i)
		{
			classes += ' on';
		}
		index += '<button class="' + classes + '" data-entry="' + k + '"><span>' + esc(entry.date) + '</span><b>' + esc(entry.title) + '</b></button>';
	});
	let hint = t('desk.pickEntry');
	if (i >= 0)
	{
		hint = t('desk.photoOnTable');
	}
	return index + '<div class="lihint">' + hint + '</div>';
}

export function objectView(o: DeskObject): DeskView
{
	const view = emptyView('desk-observatory mode-cat');
	const overlays: string[] = [];
	let templates = '';
	view.desk = woodSvg() + propsHtml('observatory');
	view.caseName = t('desk.cataloguePrefix') + ' <b>' + esc(o.name) + '</b>';

	let rows = '';
	for (const row of o.data)
	{
		rows += '<div class="lrow"><span>' + esc(row[0]) + '</span><span>' + esc(row[1]) + '</span></div>';
	}
	let also = '';
	if (o.other !== '')
	{
		also = '<div class="aka">' + fill(t('desk.also'), { name: esc(o.other) }) + '</div>';
	}
	view.catLeft = '<div class="lhead">' + t('desk.catalogueName') + ' &middot; ' + esc(o.shortType) + '</div><h1>' + esc(o.name) + '</h1>' + also
		+ '<div class="ltype">' + fill(t('desk.typeIn'), { type: esc(o.type), place: esc(o.constellation) }) + '</div><div class="ldata">' + rows + '</div><div class="lsum">' + o.summary + '</div>'
		+ '<div class="lindex" id="lindex">' + logIndexHtml(o, -1) + '</div>';

	// The whole log (log-0) and each entry opened (log-1 ...), each with its plate on the light table
	const mainMount = mountHtml(o.plate, overlays);
	view.catRight = logListHtml(o);
	view.plate = mainMount;
	view.plateCaption = plateCaptionHtml(o.plate);
	for (let i = -1; i < o.log.length; i++)
	{
		let right = logListHtml(o);
		let plate = o.plate;
		let mount = mainMount;
		if (i >= 0)
		{
			right = logPageHtml(o, i);
			const entryPlate = o.log[i].plate;
			if (entryPlate !== null)
			{
				plate = entryPlate;
				mount = mountHtml(entryPlate, overlays);
			}
		}
		templates += template('log-' + (i + 1), '<div class="t-right">' + right + '</div><div class="t-index">' + logIndexHtml(o, i) + '</div><div class="t-plate">' + mount + '</div><div class="t-cap">' + plateCaptionHtml(plate) + '</div>', '');
	}

	let cards = '<div class="ilabel">' + t('desk.instrumentsUsed') + '</div>';
	const list = o.instruments.slice().reverse();
	list.forEach((instrument, k) =>
	{
		const r = (k - 1) * 5;
		let classes = 'icard';
		if (instrument.dark)
		{
			classes += ' dark';
		}
		const file: DeskPerson = { id: '', name: instrument.name, role: instrument.role, photo: instrument.photo, photoCredit: '', born: '', died: '', from: '', bio: instrument.text };
		const overlay = addOverlay(overlays, personHtml(file, false));
		cards += '<button class="' + classes + '" data-ov="' + overlay + '" style="left:' + (k * 22) + 'px;top:' + (k * 12) + 'px;transform:rotate(' + r + 'deg);--r:' + r + 'deg"><span class="ph" ' + photoStyle(instrument.photo) + '></span><span class="nm">' + esc(instrument.name) + '</span></button>';
	});
	view.instruments = cards;

	const folder = objectFolder(o);
	view.plateBox = photoBoxHtml(o.name, folder);
	templates += folderTemplates(o.name, folder, overlays);
	view.templates = templates + overlayTemplates(overlays);
	return view;
}
