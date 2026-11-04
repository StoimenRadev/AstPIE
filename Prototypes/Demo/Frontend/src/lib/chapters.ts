import { renderMarkdown } from './markdown';

// A long page is shown in parts, with a route at the side to jump between them.

export interface ChapterView
{
	// For example "chapter-3"
	id: string;
	title: string;
	// One sentence under the title
	summary: string;
	// The text, cut at the [[marker]] lines where a widget goes (radio, simulation, ...)
	parts: ChapterPart[];
}

export interface ChapterPart
{
	// 'html' for text, or the name of the widget
	kind: string;
	html: string;
}

// [[radio]] on a line of its own puts the widget called "radio" there
const MARKER = /<p>\[\[([a-z-]+)\]\]<\/p>|\[\[([a-z-]+)\]\]/g;

function splitParts(html: string): ChapterPart[]
{
	const parts: ChapterPart[] = [];
	let last = 0;
	for (const match of html.matchAll(MARKER))
	{
		const index = Number(match.index);
		if (index > last)
		{
			parts.push({ kind: 'html', html: html.substring(last, index) });
		}
		let name = match[1];
		if (name === undefined)
		{
			name = match[2];
		}
		parts.push({ kind: name, html: '' });
		last = index + match[0].length;
	}
	if (last < html.length)
	{
		parts.push({ kind: 'html', html: html.substring(last) });
	}
	return parts;
}

// One chapter from the database (missions and mysteries)
export function chapterView(index: number, title: string, summary: string, body: string): ChapterView
{
	return {
		id: 'chapter-' + (index + 1),
		title: title,
		summary: summary,
		parts: splitParts(renderMarkdown(body))
	};
}

// A Markdown text cut into chapters at every "## Title" line.
// A line starting with "> " right after the title becomes the chapter's summary.
// Text before the first title is returned as the introduction.
export function splitMarkdown(text: string): { intro: string; chapters: ChapterView[] }
{
	const lines = text.replace(/\r\n/g, '\n').split('\n');
	const blocks: { title: string; summary: string; lines: string[] }[] = [];
	const intro: string[] = [];
	for (const line of lines)
	{
		if (line.startsWith('## '))
		{
			blocks.push({ title: line.substring(3).trim(), summary: '', lines: [] });
		}
		else if (blocks.length === 0)
		{
			intro.push(line);
		}
		else
		{
			const block = blocks[blocks.length - 1];
			if (block.summary === '' && block.lines.join('').trim() === '' && line.startsWith('> '))
			{
				block.summary = line.substring(2).trim();
			}
			else
			{
				block.lines.push(line);
			}
		}
	}

	const chapters: ChapterView[] = [];
	for (let i = 0; i < blocks.length; i++)
	{
		chapters.push({
			id: 'chapter-' + (i + 1),
			title: blocks[i].title,
			summary: blocks[i].summary,
			parts: splitParts(renderMarkdown(blocks[i].lines.join('\n')))
		});
	}
	return { intro: renderMarkdown(intro.join('\n')), chapters: chapters };
}

// True when one of the chapter's parts is the simulation
export function hasSimulation(chapter: ChapterView): boolean
{
	return chapter.parts.some((part) => part.kind === 'simulation');
}

// The chapters with the simulation moved to the end: a topic ends with trying it
export function simulationLast(chapters: ChapterView[]): ChapterView[]
{
	const others = chapters.filter((chapter) => !hasSimulation(chapter));
	const withSimulation = chapters.filter((chapter) => hasSimulation(chapter));
	return others.concat(withSimulation);
}
