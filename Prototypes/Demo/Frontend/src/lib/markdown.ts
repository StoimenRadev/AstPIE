import { Marked } from 'marked';
import markedKatex from 'marked-katex-extension';

// Turns Markdown into HTML on the server. Formulas: $F = ma$ inside text, $$...$$ on their own line.
const marked = new Marked();
marked.use(markedKatex({ throwOnError: false }));

export function renderMarkdown(text: string): string
{
	return marked.parse(text, { async: false });
}
