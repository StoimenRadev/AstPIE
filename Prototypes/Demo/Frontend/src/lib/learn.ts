import type { KeyWord } from './api';

// Turns the stored text of a topic into the parts of its page.
//
// The text is HTML split into sections by lines "## Title". Inside a section:
//   <aside data-kind="Try it at home">...</aside>   a side note with a pointing hand
//   [[law]]                                          where the law block goes
//   [[figure second]] Caption                        a figure with sliders, and its caption
//   [[graph linked]] Caption                         a graph, and its caption
// Key words are marked where they first appear, and explained beside the text.

export interface TopicSection
{
	title: string;
	// HTML, with the markers still in it
	body: string;
}

export function splitSections(content: string): TopicSection[]
{
	const sections: TopicSection[] = [];
	let current: TopicSection | null = null;
	for (const line of content.replace(/\r\n/g, '\n').split('\n'))
	{
		if (line.startsWith('## '))
		{
			current = { title: line.substring(3).trim(), body: '' };
			sections.push(current);
		}
		else if (current !== null)
		{
			current.body = current.body + line + '\n';
		}
	}
	return sections;
}

// <aside data-kind="Try it at home">text</aside> -> the printed side note with a pointing hand
export function sideNotes(html: string): string
{
	return html.replace(/<aside data-kind="([^"]*)">([\s\S]*?)<\/aside>/g, (whole, kind, text) =>
	{
		return '<p class="aside3"><span class="hand" aria-hidden="true">☞</span><i>' + kind + '.</i> ' + text + '</p>';
	});
}

// One piece of a section: plain HTML, or a marker line with its kind and caption
export interface SectionPart
{
	// 'html', 'law', 'figure' or 'graph'
	kind: string;
	html: string;
	// For figures and graphs: which one ("second", "linked"...)
	name: string;
	caption: string;
}

export function sectionParts(body: string): SectionPart[]
{
	const parts: SectionPart[] = [];
	let html = '';
	for (const line of body.split('\n'))
	{
		const marker = line.match(/^\[\[(law|figure|graph)\s*([a-z-]*)\]\]\s*(.*)$/);
		if (marker === null)
		{
			html = html + line + '\n';
			continue;
		}
		if (html.trim() !== '')
		{
			parts.push({ kind: 'html', html: html, name: '', caption: '' });
		}
		html = '';
		parts.push({ kind: marker[1], html: '', name: marker[2], caption: marker[3] });
	}
	if (html.trim() !== '')
	{
		parts.push({ kind: 'html', html: html, name: '', caption: '' });
	}
	return parts;
}

// The "where Σ is ..." line under the equation sits inside the law block
export function takeWhereLine(html: string): { where: string; rest: string }
{
	const match = html.match(/<p class="where">[\s\S]*?<\/p>/);
	if (match === null)
	{
		return { where: '', rest: html };
	}
	return { where: match[0], rest: html.replace(match[0], '') };
}

// ---------------------------------------------------------------- key words in the text

function escapeRegExp(text: string): string
{
	return text.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
}

// The word and its other spellings, matched as whole words and exactly as written
function wordPattern(word: KeyWord): RegExp
{
	const forms = [word.name].concat(word.otherForms).map(escapeRegExp);
	return new RegExp('\\b(' + forms.join('|') + ')\\b');
}

function termLink(slug: string, text: string): string
{
	return '<a class="term" href="#kw-' + slug + '" data-term="' + slug + '">' + text + '</a>';
}

// Marks the first match of a key word that is not already inside a link; returns where it was found, or -1
function linkFirst(html: string, word: KeyWord): { html: string; at: number }
{
	const parts = html.split(/(<[^>]+>)/);
	const pattern = wordPattern(word);
	let inLink = 0;
	let position = 0;
	for (let i = 0; i < parts.length; i++)
	{
		const part = parts[i];
		if (part.charAt(0) === '<')
		{
			if (/^<a[\s>]/i.test(part))
			{
				inLink++;
			}
			if (/^<\/a>/i.test(part))
			{
				inLink--;
			}
			position += part.length;
			continue;
		}
		if (inLink === 0)
		{
			const match = part.match(pattern);
			if (match !== null && match.index !== undefined)
			{
				parts[i] = part.slice(0, match.index) + termLink(word.slug, match[0]) + part.slice(match.index + match[0].length);
				return { html: parts.join(''), at: position + match.index };
			}
		}
		position += part.length;
	}
	return { html: html, at: -1 };
}

// A word written as <dfn>net force</dfn> in the text is always that key word
function fromDfn(html: string, words: KeyWord[]): string
{
	return html.replace(/<dfn>([^<]+)<\/dfn>/g, (whole, text: string) =>
	{
		for (const word of words)
		{
			if (word.name === text.toLowerCase() || word.otherForms.includes(text))
			{
				return termLink(word.slug, text);
			}
		}
		return text;
	});
}

// Marks each key word once, at its first appearance in reading order.
// Returns the texts with the words marked, and the key words in the order they appear.
export function linkKeyWords(texts: string[], words: KeyWord[]): { texts: string[]; order: KeyWord[] }
{
	const found = new Map<string, number>();
	// Longer words first, so "net force" is found before the "force" inside it
	const tryOrder = words.slice().sort((a, b) => b.name.length - a.name.length);
	const done = texts.map((text, partIndex) =>
	{
		let html = fromDfn(text, words);
		const marked = /<a class="term" href="#kw-([a-z-]+)"/g;
		let match = marked.exec(html);
		while (match !== null)
		{
			if (!found.has(match[1]))
			{
				found.set(match[1], partIndex * 1e6 + match.index);
			}
			match = marked.exec(html);
		}
		for (const word of tryOrder)
		{
			if (found.has(word.slug))
			{
				continue;
			}
			const result = linkFirst(html, word);
			if (result.at >= 0)
			{
				html = result.html;
				found.set(word.slug, partIndex * 1e6 + result.at);
			}
		}
		return html;
	});
	const order = words.filter((word) => found.has(word.slug));
	order.sort((a, b) => Number(found.get(a.slug)) - Number(found.get(b.slug)));
	return { texts: done, order: order };
}

// The line under a key word's explanation: "Symbol F, measured in newtons (N)." or "Written N."
export function keyWordUnit(word: KeyWord): string
{
	if (word.symbol !== null && word.unit !== null)
	{
		return 'Symbol ' + word.symbol + ', measured in ' + word.unit + '.';
	}
	if (word.symbol !== null)
	{
		return 'Written ' + word.symbol + '.';
	}
	return '';
}

// "Newton's second law" -> "Second law", for the line of topics above the title
export function shortName(title: string): string
{
	const name = title.replace(/^Newton's /, '');
	return name.charAt(0).toUpperCase() + name.slice(1);
}

export function lowerFirst(text: string): string
{
	return text.charAt(0).toLowerCase() + text.slice(1);
}

// The address of an Explore case
export function caseHref(kind: string, slug: string): string
{
	if (kind === 'Mystery')
	{
		return '/explore/mysteries/' + slug;
	}
	return '/explore/missions/' + slug;
}

// ---------------------------------------------------------------- a whole topic page

export interface PageSection
{
	// 'idea', 'law' or 'graph'
	kind: string;
	title: string;
	// Its number on the page (1, 2, 3...)
	number: number;
	// Its id in the page: idea-0, idea-1..., law, graph
	id: string;
	// The text, with key words marked (for the law: the text after the law block)
	html: string;
	// The law only: the "where ..." line inside the law block
	where: string;
	// The law's figure or the graph, with its caption; null for an idea
	figure: SectionPart | null;
	// The graph only: the explanation that opens under it
	after: string;
}

export interface TopicPage
{
	sections: PageSection[];
	// The key words of the page, in the order they first appear
	keyWords: KeyWord[];
}

export function buildTopicPage(content: string, words: KeyWord[]): TopicPage
{
	const sections: PageSection[] = [];
	// The texts that get key words marked, in reading order, and where each one goes back
	const texts: string[] = [];
	const asides: string[] = [];
	let ideas = 0;

	splitSections(content).forEach((section, index) =>
	{
		const parts = sectionParts(section.body);
		const page: PageSection = { kind: 'idea', title: section.title, number: index + 1, id: '', html: '', where: '', figure: null, after: '' };

		for (const part of parts)
		{
			if (part.kind === 'law')
			{
				page.kind = 'law';
			}
			if (part.kind === 'graph' && page.kind !== 'law')
			{
				page.kind = 'graph';
			}
		}

		if (page.kind === 'idea')
		{
			page.id = 'idea-' + ideas;
			ideas++;
			// Side notes are not searched for key words; they are put back afterwards
			let html = parts.map((p) => p.html).join('');
			html = html.replace(/<aside[\s\S]*?<\/aside>/g, (aside) =>
			{
				asides.push(aside);
				return '<!--aside' + (asides.length - 1) + '-->';
			});
			page.html = html;
		}
		else if (page.kind === 'law')
		{
			page.id = 'law';
			let html = '';
			for (const part of parts)
			{
				if (part.kind === 'html')
				{
					html = html + part.html;
				}
				if (part.kind === 'figure')
				{
					page.figure = part;
				}
			}
			const split = takeWhereLine(html);
			page.where = split.where;
			page.html = split.rest;
		}
		else
		{
			page.id = 'graph';
			let beforeGraph = true;
			for (const part of parts)
			{
				if (part.kind === 'graph')
				{
					page.figure = part;
					beforeGraph = false;
				}
				else if (beforeGraph)
				{
					page.html = page.html + part.html;
				}
				else
				{
					page.after = page.after + part.html.replace('<p class="after">', '<p>');
				}
			}
		}
		texts.push(page.html);
		sections.push(page);
	});

	const linked = linkKeyWords(texts, words);
	sections.forEach((page, i) =>
	{
		let html = linked.texts[i];
		html = html.replace(/<!--aside(\d+)-->/g, (whole, n) => asides[Number(n)]);
		page.html = sideNotes(html);
	});
	return { sections: sections, keyWords: linked.order };
}
