// What each table looks like in the admin pages. The list page, the form page and the
// dashboard are built from this, so a new field is one line here.

export type FieldType =
	| 'text'      // one line of text
	| 'textarea'  // a few lines
	| 'markdown'  // long text (HTML) in a big box
	| 'lines'     // a list, one item per line
	| 'int'       // whole number
	| 'number'    // any number (1.27e7 works)
	| 'checkbox'
	| 'date'
	| 'select'    // one of the given options
	| 'ref'       // one row of another table (shown by its name)
	| 'file';     // an address, or a file to upload that fills it in

export interface FieldConfig
{
	name: string;
	label: string;
	type: FieldType;
	required: boolean;
	// For 'select'
	options: string[];
	// For 'ref': the key of the other table
	refTable: string;
	// Shown under the field
	help: string;
	// The entry and person of a person-in-entry link can't be changed after it is made
	lockedOnEdit: boolean;
}

// One tab of the editor: a few fields, and the part of the public page they fill
export interface SectionConfig
{
	title: string;
	// One sentence under the tab's heading
	hint: string;
	// The part of the page map that lights up (see PageMapPart.key); empty for none
	appears: string;
	fields: string[];
}

// One block of the small page map at the side of the editor
export interface PageMapPart
{
	key: string;
	label: string;
	// Grid area in the map: 'side' (left column), 'main', 'info' (right column) or 'wide'
	area: string;
}

export interface TableConfig
{
	// Part of the address (/admin/topics) and of the API path (/api/admin/topics)
	key: string;
	title: string;
	singular: string;
	// The field that names a row ("title", "fullName"...)
	labelField: string;
	// Fields shown in the list
	listColumns: string[];
	fields: FieldConfig[];
	// Tables edited on this table's page, linked by parentField (for cases: pages, desk items, people...)
	children: string[];
	parentField: string;
	// The tabs of the editor; empty means one tab with every field
	sections: SectionConfig[];
	// The page this table fills, drawn small next to the editor; empty for none
	pageMap: PageMapPart[];
}

// Builds a field; only the settings that differ from the defaults are given
function field(name: string, label: string, type: FieldType, settings: Partial<FieldConfig>): FieldConfig
{
	const result: FieldConfig = {
		name: name,
		label: label,
		type: type,
		required: false,
		options: [],
		refTable: '',
		help: '',
		lockedOnEdit: false
	};
	return Object.assign(result, settings);
}

function section(title: string, hint: string, appears: string, fields: string[]): SectionConfig
{
	return { title: title, hint: hint, appears: appears, fields: fields };
}

// The page map of a Learn concept page
const TOPIC_MAP: PageMapPart[] = [
	{ key: 'opener', label: 'Question and lead', area: 'main' },
	{ key: 'predict', label: 'Predict', area: 'main' },
	{ key: 'parts', label: 'Numbered sections', area: 'main' },
	{ key: 'keywords', label: 'Key words', area: 'info' },
	{ key: 'law', label: 'The law', area: 'main' },
	{ key: 'lab', label: 'Lab sheet', area: 'main' },
	{ key: 'recap', label: 'In short', area: 'main' },
	{ key: 'list', label: 'Line on the Learn page', area: 'wide' }
];

// The page map of a case desk
const CASE_MAP: PageMapPart[] = [
	{ key: 'tabs', label: 'Tabs and pages', area: 'side' },
	{ key: 'cover', label: 'Cover sheet', area: 'main' },
	{ key: 'desk', label: 'Things on the desk', area: 'info' },
	{ key: 'radio', label: 'Radio and tapes', area: 'info' },
	{ key: 'photos', label: 'Photo folder', area: 'wide' },
	{ key: 'archive', label: 'Folder in the archive', area: 'wide' }
];

// The page map of an object in the observatory
const LOGBOOK_MAP: PageMapPart[] = [
	{ key: 'left', label: 'Left page', area: 'main' },
	{ key: 'log', label: 'Observation log', area: 'main' },
	{ key: 'table', label: 'Light table', area: 'info' },
	{ key: 'instruments', label: 'Instruments', area: 'info' },
	{ key: 'archive', label: 'Card in the archive', area: 'wide' }
];

const SLUG_HELP = 'The address of the page: lower-case letters, numbers and dashes, for example newtons-first-law.';
const FILE_HELP = 'Paste an address, or choose a file to upload (pictures, audio or video, up to 100 MB).';
const LINES_HELP = 'One item per line.';
const ORDER_HELP = 'Lower numbers come first.';

export const TABLES: TableConfig[] = [
	// ---------------------------------------------------------------- Learn
	{
		key: 'topics',
		title: 'Topics',
		singular: 'topic',
		labelField: 'title',
		listColumns: ['title', 'question', 'branch', 'isPublished'],
		children: ['topic-symbols', 'check-questions', 'topic-key-words'],
		parentField: '',
		pageMap: TOPIC_MAP,
		sections: [
			section('Question and lead', 'The page title is the question; the law\'s name stands under the lead.', 'opener', ['question', 'title', 'slug', 'branch', 'summary', 'sortOrder']),
			section('Predict', 'Asked before reading. The first lab task comes back to it.', 'predict', ['predictQuestion', 'predictOptions', 'predictAnswer']),
			section('Text of the topic', 'Every "## Title" line starts a numbered section. [[law]], [[figure first]] and [[graph linked]] mark where the law, the figure and the graph go.', 'parts', ['content']),
			section('The law', 'The law in one sentence, as an equation, and read aloud. The symbols are explained on their own tab.', 'law', ['lawStatement', 'lawEquation', 'readAloud']),
			section('In short and Explore', 'The numbered summary at the end, and the case where the law can be seen at work.', 'recap', ['recapPoints', 'exploreEntryId', 'exploreLine']),
			section('Publishing', 'Who can see this topic.', '', ['isPublished'])
		],
		fields: [
			field('question', 'Question (page title)', 'text', { required: true, help: 'For example Does moving need a push?' }),
			field('title', 'Name of the law', 'text', { required: true, help: 'For example Newton\'s first law.' }),
			field('slug', 'Slug', 'text', { required: true, help: SLUG_HELP }),
			field('branch', 'Branch', 'text', { required: true, help: 'For example Classical mechanics.' }),
			field('summary', 'Lead', 'textarea', { required: true, help: 'One sentence under the question.' }),
			field('content', 'Content', 'markdown', { required: true, help: 'HTML in sections. <mark> makes a key idea; <aside data-kind="Try it at home">...</aside> is a side note.' }),
			field('predictQuestion', 'Predict question', 'textarea', { required: true }),
			field('predictOptions', 'Choices', 'lines', { help: 'One choice per line (a, b, c).' }),
			field('predictAnswer', 'Right choice', 'int', { help: '0 for the first choice, 1 for the second, 2 for the third.' }),
			field('lawStatement', 'The law in one sentence', 'textarea', { required: true }),
			field('lawEquation', 'Equation', 'textarea', { required: true, help: 'HTML. Each symbol as <span class="sym" data-k="f">F</span>, so it lights up with its row.' }),
			field('readAloud', 'Read aloud', 'textarea', { required: true }),
			field('recapPoints', 'In short', 'lines', { help: LINES_HELP }),
			field('exploreEntryId', 'See it at work in Explore', 'ref', { refTable: 'entries' }),
			field('exploreLine', 'Sentence after the link', 'textarea', {}),
			field('sortOrder', 'Order in the branch', 'int', {}),
			field('isPublished', 'Published', 'checkbox', {})
		]
	},
	{
		key: 'topic-symbols',
		title: 'Symbols of the equation',
		singular: 'symbol',
		labelField: 'name',
		listColumns: ['symbol', 'name', 'topicId', 'sortOrder'],
		children: [],
		parentField: 'topicId',
		sections: [],
		pageMap: [],
		fields: [
			field('topicId', 'Topic', 'ref', { required: true, refTable: 'topics' }),
			field('symbol', 'Symbol', 'text', { required: true, help: 'As shown, for example Σ or F.' }),
			field('name', 'Name', 'text', { required: true, help: 'In bold, for example force.' }),
			field('meaning', 'Meaning', 'textarea', { required: true }),
			field('unit', 'Unit', 'text', { help: 'For example Measured in newtons (N). Empty when it has none.' }),
			field('equationPart', 'Part of the equation', 'text', { help: 'The data-k of its symbol in the equation, so both light up together.' }),
			field('sortOrder', 'Order', 'int', { help: ORDER_HELP })
		]
	},
	{
		key: 'check-questions',
		title: 'Questions',
		singular: 'question',
		labelField: 'question',
		listColumns: ['question', 'topicId', 'afterSection'],
		children: [],
		parentField: 'topicId',
		sections: [],
		pageMap: [],
		fields: [
			field('topicId', 'Topic', 'ref', { required: true, refTable: 'topics' }),
			field('afterSection', 'After section', 'int', { help: '1 puts it after the first section.' }),
			field('question', 'Question', 'textarea', { required: true }),
			field('answer', 'Answer', 'textarea', { required: true }),
			field('sortOrder', 'Order', 'int', { help: ORDER_HELP })
		]
	},
	{
		key: 'key-words',
		title: 'Key words',
		singular: 'key word',
		labelField: 'name',
		listColumns: ['name', 'slug', 'symbol'],
		children: [],
		parentField: '',
		sections: [],
		pageMap: [],
		fields: [
			field('name', 'Word', 'text', { required: true, help: 'In lower case, for example net force.' }),
			field('slug', 'Slug', 'text', { required: true, help: SLUG_HELP }),
			field('otherForms', 'Other spellings', 'lines', { help: 'Found in the text exactly as written, one per line, for example forces and Force.' }),
			field('definition', 'Explanation', 'textarea', { required: true, help: 'One to three short sentences.' }),
			field('symbol', 'Symbol', 'text', {}),
			field('unit', 'Unit', 'text', { help: 'For example newtons (N).' })
		]
	},
	{
		key: 'topic-key-words',
		title: 'Key words of the topic',
		singular: 'key word of this topic',
		labelField: 'keyWordId',
		listColumns: ['topicId', 'keyWordId'],
		children: [],
		parentField: 'topicId',
		sections: [],
		pageMap: [],
		fields: [
			field('topicId', 'Topic', 'ref', { required: true, refTable: 'topics', lockedOnEdit: true }),
			field('keyWordId', 'Key word', 'ref', { required: true, refTable: 'key-words', lockedOnEdit: true })
		]
	},
	{
		key: 'simulations',
		title: 'Simulations',
		singular: 'simulation',
		labelField: 'title',
		listColumns: ['title', 'topicId', 'status', 'version'],
		children: ['lab-tasks'],
		parentField: '',
		sections: [
			section('Simulation', 'Where the simulation is and what it shows.', '', ['topicId', 'title', 'slug', 'engine', 'version', 'status', 'buildUrl', 'thumbnailUrl', 'learningGoal', 'sourceUrl']),
			section('Lab sheet', 'The line above the simulation on the topic page.', '', ['labIntro']),
			section('How it works', 'The part under the lab sheet for anyone who wants the details.', '', ['modelText', 'limits', 'stepCode'])
		],
		pageMap: [],
		fields: [
			field('topicId', 'Topic', 'ref', { required: true, refTable: 'topics' }),
			field('title', 'Title', 'text', { required: true }),
			field('slug', 'Slug', 'text', { required: true, help: 'The same name as ?sim= in the Godot project, for example newtons-first-law.' }),
			field('engine', 'Engine', 'select', { required: true, options: ['Godot2D', 'ThreeJs'] }),
			field('version', 'Version', 'text', { required: true }),
			field('status', 'Status', 'select', { required: true, options: ['Draft', 'Published'] }),
			field('buildUrl', 'Address of the simulation', 'text', { required: true, help: 'For example http://localhost:8081/?sim=newtons-first-law' }),
			field('thumbnailUrl', 'Thumbnail', 'file', { help: FILE_HELP }),
			field('learningGoal', 'What you will see', 'textarea', { required: true }),
			field('sourceUrl', 'Source code address', 'text', {}),
			field('labIntro', 'Line above the lab sheet', 'textarea', { help: 'For example One box, the forces you add, no friction.' }),
			field('modelText', 'The model', 'markdown', { help: 'HTML paragraphs: what the simulation shows and what it leaves out.' }),
			field('limits', 'Limits', 'lines', { help: LINES_HELP }),
			field('stepCode', 'Code of one step', 'markdown', { help: 'The C# shown under "The method".' })
		]
	},
	{
		key: 'lab-tasks',
		title: 'Lab tasks',
		singular: 'lab task',
		labelField: 'title',
		listColumns: ['title', 'simulationId', 'sortOrder'],
		children: [],
		parentField: 'simulationId',
		sections: [],
		pageMap: [],
		fields: [
			field('simulationId', 'Simulation', 'ref', { required: true, refTable: 'simulations' }),
			field('title', 'Title', 'text', { required: true }),
			field('task', 'Task', 'textarea', { required: true }),
			field('hint', 'Hint', 'textarea', { required: true }),
			field('answer', 'Answer', 'textarea', { required: true }),
			field('answersPredict', 'Answers the Predict question', 'checkbox', { help: 'Adds "You chose ..." to the answer.' }),
			field('sortOrder', 'Order', 'int', { help: ORDER_HELP })
		]
	},

	// ---------------------------------------------------------------- Explore
	{
		key: 'entries',
		title: 'Missions and mysteries',
		singular: 'case',
		labelField: 'title',
		listColumns: ['title', 'kind', 'desk', 'isPublished'],
		children: ['chapters', 'entry-facts', 'media', 'explanations', 'entry-people'],
		parentField: '',
		pageMap: CASE_MAP,
		sections: [
			section('Cover sheet', 'The first sheet of the folder: title, short line, photo, summary and the seal or stamp.', 'cover', ['kind', 'title', 'slug', 'tagline', 'summary', 'coverImageUrl', 'coverCaption', 'coverCredit', 'missionOutcome', 'evidenceStatus', 'statusNote']),
			section('Desk and archive', 'Which desk the case opens on, and the line on its folder in the archive.', 'archive', ['desk', 'country', 'startDate', 'endDate', 'agency']),
			section('Radio', 'The radio\'s dial. Leave both empty to use the range of the desk.', 'radio', ['radioMinMHz', 'radioMaxMHz']),
			section('Try it yourself', 'The Learn topic whose simulation runs on the monitor.', 'desk', ['relatedTopicId']),
			section('Publishing', 'Who can see this case.', '', ['isPublished'])
		],
		fields: [
			field('kind', 'Kind', 'select', { required: true, options: ['Mission', 'Mystery'] }),
			field('title', 'Title', 'text', { required: true }),
			field('slug', 'Slug', 'text', { required: true, help: SLUG_HELP }),
			field('tagline', 'Short line', 'text', { help: 'On the folder and the cover sheet, for example Two people on another world.' }),
			field('summary', 'Case summary', 'textarea', { required: true, help: 'A few sentences. <mark>...</mark> highlights the key sentence.' }),
			field('coverImageUrl', 'Cover photo', 'file', { help: FILE_HELP }),
			field('coverCaption', 'Handwritten caption of the photo', 'text', {}),
			field('coverCredit', 'Photo credit', 'text', {}),
			field('desk', 'Desk', 'select', { required: true, options: ['Soviet', 'American', 'Mystery'] }),
			field('country', 'Country', 'text', { help: 'On the folder in the archive.' }),
			field('startDate', 'Start date', 'date', {}),
			field('endDate', 'End date', 'date', { help: 'Leave empty for an ongoing mission.' }),
			field('agency', 'Agency', 'text', { help: 'Missions only.' }),
			field('missionOutcome', 'Result', 'select', { options: ['Success', 'PartialSuccess', 'Failure', 'Ongoing'], help: 'Missions only: chooses the seal.' }),
			field('evidenceStatus', 'Evidence status', 'select', { options: ['Unexplained', 'UnderInvestigation', 'Hypothesis', 'Debunked'], help: 'Mysteries only: chooses the big stamp.' }),
			field('statusNote', 'Note under the stamp', 'text', { help: 'Mysteries only, for example case still open.' }),
			field('radioMinMHz', 'Lowest frequency (MHz)', 'number', {}),
			field('radioMaxMHz', 'Highest frequency (MHz)', 'number', {}),
			field('relatedTopicId', 'Learn topic', 'ref', { refTable: 'topics' }),
			field('isPublished', 'Published', 'checkbox', {})
		]
	},
	{
		key: 'chapters',
		title: 'Pages of the case',
		singular: 'page',
		labelField: 'title',
		listColumns: ['section', 'tabLabel', 'entryId', 'sortOrder'],
		children: [],
		parentField: 'entryId',
		sections: [],
		pageMap: [],
		fields: [
			field('entryId', 'Case', 'ref', { required: true, refTable: 'entries' }),
			field('section', 'Section', 'text', { required: true, help: 'The big coloured tab, for example Story. Pages of one section follow each other.' }),
			field('tabLabel', 'Tab', 'text', { required: true, help: 'The short name on the small tab.' }),
			field('title', 'Heading', 'text', { required: true }),
			field('place', 'Where', 'text', {}),
			field('dateLabel', 'When', 'text', { help: 'Written as you like, for example 1961 to 1969.' }),
			field('summary', 'One-line summary', 'textarea', { required: true }),
			field('body', 'Text', 'markdown', { required: true, help: 'HTML paragraphs. <mark>...</mark> highlights.' }),
			field('marginNote', 'Note in the margin', 'text', {}),
			field('showsRadioHint', 'Points to the radio', 'checkbox', {}),
			field('sortOrder', 'Order', 'int', { help: ORDER_HELP })
		]
	},
	{
		key: 'entry-facts',
		title: 'Cover sheet rows',
		singular: 'row',
		labelField: 'label',
		listColumns: ['label', 'value', 'entryId'],
		children: [],
		parentField: 'entryId',
		sections: [],
		pageMap: [],
		fields: [
			field('entryId', 'Case', 'ref', { required: true, refTable: 'entries' }),
			field('label', 'Label', 'text', { required: true, help: 'For example Rocket.' }),
			field('value', 'Value', 'text', { required: true }),
			field('sortOrder', 'Order', 'int', { help: ORDER_HELP })
		]
	},
	{
		key: 'media',
		title: 'Things on the desk',
		singular: 'thing on the desk',
		labelField: 'title',
		listColumns: ['title', 'type', 'chapterId', 'entryId'],
		children: ['media-people'],
		parentField: 'entryId',
		sections: [],
		pageMap: [],
		fields: [
			field('entryId', 'Case', 'ref', { required: true, refTable: 'entries' }),
			field('chapterId', 'Page', 'ref', { refTable: 'chapters', help: 'The page it lands on. Empty: the case summary (and Audio or Signal: a tape for the radio).' }),
			field('type', 'Type', 'select', { required: true, options: ['Image', 'Audio', 'Signal', 'Video', 'Note', 'Quote', 'Drawing', 'Clipping', 'Printout', 'Document', 'Card', 'PersonFile', 'DayGrid', 'Simulation'] }),
			field('title', 'Title', 'text', { required: true, help: 'Also the label of a tape and the headline of a clipping.' }),
			field('url', 'File', 'file', { help: FILE_HELP + ' Empty for things drawn by code.' }),
			field('caption', 'Caption', 'textarea', {}),
			field('credit', 'Credit', 'text', {}),
			field('license', 'Licence', 'text', {}),
			field('text', 'Words', 'textarea', { help: 'A note, quote or clipping, or the handwritten label under a photo.' }),
			field('author', 'Who said it', 'text', { help: 'Quotes only.' }),
			field('dateLabel', 'When', 'text', { help: 'Quotes and clippings.' }),
			field('posterUrl', 'Still frame', 'file', { help: 'Films only.' }),
			field('lengthLabel', 'Length', 'text', { help: 'Films and tapes, for example about 3 hours.' }),
			field('personId', 'Person', 'ref', { refTable: 'people', help: 'Personnel files only. Who a photo shows goes under People in the photo.' }),
			field('style', 'Look', 'text', { help: 'A note\'s colour (yellow, pink...), dark, photo, or folder (only in the photo folder: first, with the case summary, when it shows people of the case; otherwise under More from the archive).' }),
			field('chartKey', 'Drawn by code', 'text', { help: 'Printouts: doppler, decay, counts, wow, transit, s2. Drawings: sketch:lunapath, svg:bigear. Signal: wow.' }),
			field('data', 'Extra details (JSON)', 'textarea', { help: 'Cards: {"rows": [["Label", "Value"]]}. Day grids: {"from", "to", "loud", "weekly"}. Clippings: {"outcome"}.' }),
			field('sortOrder', 'Order', 'int', { help: ORDER_HELP })
		]
	},
	{
		key: 'media-people',
		title: 'People in the photo',
		singular: 'person in this photo',
		labelField: 'personId',
		listColumns: ['mediaItemId', 'personId'],
		children: [],
		parentField: 'mediaItemId',
		sections: [],
		pageMap: [],
		fields: [
			field('mediaItemId', 'Photo', 'ref', { required: true, refTable: 'media', lockedOnEdit: true }),
			field('personId', 'Person', 'ref', { required: true, refTable: 'people', lockedOnEdit: true, help: 'Someone of this case. One person: the photo is listed under "Photos of" them in the photo folder. Several: it is listed with the people of the case.' })
		]
	},
	{
		key: 'explanations',
		title: 'Cards on the cork board',
		singular: 'card',
		labelField: 'title',
		listColumns: ['title', 'stamp', 'chapterId'],
		children: [],
		parentField: 'entryId',
		sections: [],
		pageMap: [],
		fields: [
			field('entryId', 'Case', 'ref', { required: true, refTable: 'entries' }),
			field('chapterId', 'Page', 'ref', { refTable: 'chapters', help: 'The page it is pinned on.' }),
			field('title', 'Explanation', 'text', { required: true }),
			field('proposedBy', 'Proposed by', 'text', {}),
			field('forText', 'What supports it', 'textarea', { required: true }),
			field('againstText', 'What speaks against it', 'textarea', { required: true }),
			field('verdict', 'Verdict', 'textarea', { required: true }),
			field('stamp', 'Stamp', 'text', { required: true, help: 'For example POSSIBLE, UNLIKELY or RULED OUT.' }),
			field('imageUrl', 'Picture', 'file', { help: FILE_HELP }),
			field('imageCredit', 'Picture credit', 'text', {}),
			field('imageFit', 'Picture fit', 'select', { options: ['cover', 'contain'], help: 'contain shows the whole picture.' }),
			field('sortOrder', 'Order', 'int', { help: 'Its place among the other things on the page.' })
		]
	},
	{
		key: 'people',
		title: 'People',
		singular: 'person',
		labelField: 'fullName',
		listColumns: ['fullName', 'nationality', 'birthDate'],
		children: [],
		parentField: '',
		sections: [],
		pageMap: [],
		fields: [
			field('fullName', 'Full name', 'text', { required: true }),
			field('nationality', 'From', 'text', { required: true }),
			field('birthDate', 'Born', 'date', {}),
			field('deathDate', 'Died', 'date', {}),
			field('photoUrl', 'Photo', 'file', { help: FILE_HELP }),
			field('photoCredit', 'Photo credit', 'text', {}),
			field('bio', 'Biography', 'textarea', { required: true })
		]
	},
	{
		key: 'entry-people',
		title: 'People in the case',
		singular: 'person in this case',
		labelField: 'role',
		listColumns: ['entryId', 'personId', 'role', 'isCrew'],
		children: [],
		parentField: 'entryId',
		sections: [],
		pageMap: [],
		fields: [
			field('entryId', 'Case', 'ref', { required: true, refTable: 'entries', lockedOnEdit: true }),
			field('personId', 'Person', 'ref', { required: true, refTable: 'people', lockedOnEdit: true }),
			field('role', 'Role', 'text', { required: true, help: 'For example Commander or Chief designer.' }),
			field('isCrew', 'Was on board (crew)', 'checkbox', {}),
			field('sortOrder', 'Order', 'int', {})
		]
	},

	// ---------------------------------------------------------------- Catalogue
	{
		key: 'catalogue',
		title: 'Catalogue',
		singular: 'catalogue object',
		labelField: 'name',
		listColumns: ['name', 'type', 'constellation', 'isPublished'],
		children: ['catalogue-facts', 'log-entries', 'catalogue-instruments'],
		parentField: '',
		pageMap: LOGBOOK_MAP,
		sections: [
			section('Left page', 'The name, the type and the text at the top of the logbook.', 'left', ['name', 'slug', 'otherNames', 'type', 'typeLabel', 'constellation', 'description']),
			section('Light table', 'The main picture, shown until an entry of the log is opened.', 'table', ['imageUrl', 'imageCaption', 'imageCredit', 'imageIsDark']),
			section('Card in the archive', 'The short line and the measured values.', 'archive', ['summary', 'rightAscension', 'declination', 'distanceLy', 'massKg', 'radiusKm', 'apparentMagnitude', 'discoveredBy', 'discoveryYear']),
			section('Publishing', 'Who can see this object.', '', ['relatedTopicId', 'isPublished'])
		],
		fields: [
			field('name', 'Name', 'text', { required: true }),
			field('slug', 'Slug', 'text', { required: true, help: SLUG_HELP }),
			field('otherNames', 'Other names', 'text', { help: 'For example Sgr A*.' }),
			field('type', 'Type', 'select', { required: true, options: ['Star', 'Planet', 'Moon', 'DwarfPlanet', 'Galaxy', 'Nebula', 'BlackHole', 'Cluster', 'Other'] }),
			field('typeLabel', 'Type as written', 'text', { help: 'For example Planet (a sub-Neptune).' }),
			field('constellation', 'Constellation', 'text', {}),
			field('rightAscension', 'Right ascension (hours)', 'number', {}),
			field('declination', 'Declination (degrees)', 'number', {}),
			field('distanceLy', 'Distance (light-years)', 'number', {}),
			field('massKg', 'Mass (kg)', 'number', { help: 'For example 8.55e36.' }),
			field('radiusKm', 'Radius (km)', 'number', {}),
			field('apparentMagnitude', 'Apparent magnitude', 'number', { help: 'Leave empty when it can\'t be seen.' }),
			field('discoveredBy', 'Discovered by', 'text', {}),
			field('discoveryYear', 'Discovery year', 'int', {}),
			field('summary', 'Short line', 'textarea', { required: true }),
			field('description', 'Text on the left page', 'markdown', { required: true, help: 'HTML paragraphs. <mark>...</mark> highlights.' }),
			field('imageUrl', 'Main picture', 'file', { help: FILE_HELP }),
			field('imageCaption', 'Caption', 'textarea', {}),
			field('imageCredit', 'Credit', 'text', {}),
			field('imageIsDark', 'Dark sky picture', 'checkbox', {}),
			field('relatedTopicId', 'Learn topic', 'ref', { refTable: 'topics' }),
			field('isPublished', 'Published', 'checkbox', {})
		]
	},
	{
		key: 'catalogue-facts',
		title: 'Rows on the left page',
		singular: 'row',
		labelField: 'label',
		listColumns: ['label', 'value', 'catalogueObjectId'],
		children: [],
		parentField: 'catalogueObjectId',
		sections: [],
		pageMap: [],
		fields: [
			field('catalogueObjectId', 'Object', 'ref', { required: true, refTable: 'catalogue' }),
			field('label', 'Label', 'text', { required: true, help: 'For example Year.' }),
			field('value', 'Value', 'text', { required: true }),
			field('sortOrder', 'Order', 'int', { help: ORDER_HELP })
		]
	},
	{
		key: 'log-entries',
		title: 'Observation log',
		singular: 'log entry',
		labelField: 'title',
		listColumns: ['dateLabel', 'title', 'catalogueObjectId'],
		children: [],
		parentField: 'catalogueObjectId',
		sections: [],
		pageMap: [],
		fields: [
			field('catalogueObjectId', 'Object', 'ref', { required: true, refTable: 'catalogue' }),
			field('dateLabel', 'Date as written', 'text', { required: true, help: 'For example Mar. 2009.' }),
			field('title', 'Title', 'text', { required: true }),
			field('summary', 'Short line in the log', 'textarea', { required: true }),
			field('place', 'Where and when', 'text', {}),
			field('body', 'Full entry', 'lines', { help: 'One paragraph per line. <mark>...</mark> highlights.' }),
			field('facts', 'In short', 'lines', { help: 'One row per line, written Label: value.' }),
			field('note', 'Handwritten note', 'text', {}),
			field('plateKind', 'On the light table', 'select', { required: true, options: ['Photo', 'Chart', 'Video'] }),
			field('plateUrl', 'Picture or video', 'file', { help: FILE_HELP }),
			field('plateChartKey', 'Chart drawn by code', 'text', { help: 'transit or s2.' }),
			field('platePosterUrl', 'Still frame of the video', 'file', {}),
			field('plateTitle', 'Title of the picture', 'text', {}),
			field('plateCaption', 'Caption', 'textarea', {}),
			field('plateCredit', 'Credit', 'text', {}),
			field('plateIsDark', 'Dark sky picture', 'checkbox', {}),
			field('sortOrder', 'Order', 'int', { help: ORDER_HELP })
		]
	},
	{
		key: 'instruments',
		title: 'Instruments',
		singular: 'instrument',
		labelField: 'name',
		listColumns: ['name', 'photoCredit'],
		children: [],
		parentField: '',
		sections: [],
		pageMap: [],
		fields: [
			field('name', 'Name', 'text', { required: true, help: 'For example Kepler space telescope.' }),
			field('photoUrl', 'Photo', 'file', { required: true, help: FILE_HELP }),
			field('photoCredit', 'Photo credit', 'text', {}),
			field('photoIsDark', 'Dark photo', 'checkbox', {}),
			field('description', 'About it', 'textarea', {})
		]
	},
	{
		key: 'catalogue-instruments',
		title: 'Instruments used',
		singular: 'instrument used',
		labelField: 'role',
		listColumns: ['catalogueObjectId', 'instrumentId', 'role'],
		children: [],
		parentField: 'catalogueObjectId',
		sections: [],
		pageMap: [],
		fields: [
			field('catalogueObjectId', 'Object', 'ref', { required: true, refTable: 'catalogue', lockedOnEdit: true }),
			field('instrumentId', 'Instrument', 'ref', { required: true, refTable: 'instruments', lockedOnEdit: true }),
			field('role', 'What it did', 'text', { required: true, help: 'For example found it, 2014 to 2015.' }),
			field('sortOrder', 'Order', 'int', { help: ORDER_HELP })
		]
	}
];

export function findTable(key: string): TableConfig | null
{
	const table = TABLES.find((t) => t.key === key);
	if (table === undefined)
	{
		return null;
	}
	return table;
}
