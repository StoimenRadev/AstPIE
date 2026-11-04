// What each table looks like in the admin pages. The list page, the form page and the
// dashboard are built from this, so a new field is one line here.

export type FieldType =
	| 'text'      // one line of text
	| 'textarea'  // a few lines
	| 'markdown'  // long text with Markdown and formulas
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
	// Tables edited on this table's page, linked by parentField (for entries: chapters, people, media)
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

// The page map of a mission, mystery or catalogue object: route, article, infobox
const CASE_MAP: PageMapPart[] = [
	{ key: 'route', label: 'Route', area: 'side' },
	{ key: 'intro', label: 'Title and intro', area: 'main' },
	{ key: 'info', label: 'Infobox', area: 'info' },
	{ key: 'parts', label: 'Parts of the page', area: 'main' },
	{ key: 'people', label: 'People', area: 'main' },
	{ key: 'media', label: 'Images', area: 'main' },
	{ key: 'physics', label: 'The physics behind it', area: 'wide' }
];

const TOPIC_MAP: PageMapPart[] = [
	{ key: 'menu', label: 'Side menu', area: 'side' },
	{ key: 'intro', label: 'Title and the law', area: 'main' },
	{ key: 'parts', label: 'Parts of the topic', area: 'main' },
	{ key: 'sim', label: 'Simulation (always last)', area: 'main' },
	{ key: 'list', label: 'Card on the Learn page', area: 'wide' }
];

const SLUG_HELP = 'The address of the page: lower-case letters, numbers and dashes, for example newtons-first-law.';
const FILE_HELP = 'Paste an address, or choose a file to upload (pictures, audio or video, up to 100 MB).';

export const TABLES: TableConfig[] = [
	{
		key: 'topics',
		title: 'Topics',
		singular: 'topic',
		labelField: 'title',
		listColumns: ['title', 'branch', 'slug', 'isPublished'],
		children: [],
		parentField: '',
		pageMap: TOPIC_MAP,
		sections: [
			section('Title and branch', 'The heading of the topic and the branch it belongs to.', 'intro', ['title', 'slug', 'branch', 'sortOrder']),
			section('Card text', 'Two or three sentences shown on the Learn page and on Home.', 'list', ['summary']),
			section('Text of the topic', 'The first line is the law in one sentence. Every "## Title" starts a new part. The part with [[simulation]] is always shown last.', 'parts', ['content']),
			section('Publishing', 'Who can see this topic.', '', ['isPublished'])
		],
		fields: [
			field('title', 'Title', 'text', { required: true }),
			field('slug', 'Slug', 'text', { required: true, help: SLUG_HELP }),
			field('branch', 'Branch', 'text', { required: true, help: 'For example Classical mechanics. Topics of the same branch share their card picture.' }),
			field('summary', 'Summary', 'textarea', { required: true }),
			field('content', 'Content', 'markdown', { required: true, help: 'Markdown. Formulas: $F = ma$ in the text, $$...$$ on their own line.' }),
			field('sortOrder', 'Order in the branch', 'int', {}),
			field('isPublished', 'Published', 'checkbox', {})
		]
	},
	{
		key: 'simulations',
		title: 'Simulations',
		singular: 'simulation',
		labelField: 'title',
		listColumns: ['title', 'topicId', 'status', 'version'],
		children: [],
		parentField: '',
		sections: [],
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
			field('sourceUrl', 'Source code address', 'text', {})
		]
	},
	{
		key: 'entries',
		title: 'Missions and mysteries',
		singular: 'entry',
		labelField: 'title',
		listColumns: ['title', 'kind', 'startDate', 'isPublished'],
		children: ['chapters', 'entry-people', 'media'],
		parentField: '',
		pageMap: CASE_MAP,
		sections: [
			section('Title and intro', 'The top of the page. Most visitors read only this, so keep it clear.', 'intro', ['kind', 'title', 'slug', 'summary']),
			section('Infobox', 'The box on the right: the picture and the short facts.', 'info', ['coverImageUrl', 'startDate', 'endDate', 'agency', 'missionOutcome', 'evidenceStatus']),
			section('Physics link', 'The last block of the page, linking to one Learn topic.', 'physics', ['relatedTopicId']),
			section('Publishing', 'Who can see this page.', '', ['isPublished'])
		],
		fields: [
			field('kind', 'Kind', 'select', { required: true, options: ['Mission', 'Mystery'] }),
			field('title', 'Title', 'text', { required: true }),
			field('slug', 'Slug', 'text', { required: true, help: SLUG_HELP }),
			field('summary', 'Intro', 'textarea', { required: true, help: 'Three to five sentences that sum up the whole page, like the opening of an encyclopedia article.' }),
			field('coverImageUrl', 'Main picture', 'file', { help: FILE_HELP }),
			field('startDate', 'Start date', 'date', {}),
			field('endDate', 'End date', 'date', { help: 'Leave empty for an ongoing mission.' }),
			field('agency', 'Agency', 'text', { help: 'Missions only.' }),
			field('missionOutcome', 'Result', 'select', { options: ['Success', 'PartialSuccess', 'Failure', 'Ongoing'], help: 'Missions only.' }),
			field('evidenceStatus', 'Evidence status', 'select', { options: ['Unexplained', 'UnderInvestigation', 'Hypothesis', 'Debunked'], help: 'Mysteries only.' }),
			field('relatedTopicId', 'Physics behind it', 'ref', { refTable: 'topics' }),
			field('isPublished', 'Published', 'checkbox', {})
		]
	},
	{
		key: 'chapters',
		title: 'Parts of the page',
		singular: 'part',
		labelField: 'title',
		listColumns: ['title', 'entryId', 'dateLabel', 'sortOrder'],
		children: [],
		parentField: 'entryId',
		sections: [],
		pageMap: [],
		fields: [
			field('entryId', 'Entry', 'ref', { required: true, refTable: 'entries' }),
			field('title', 'Title', 'text', { required: true }),
			field('dateLabel', 'Date', 'text', { help: 'Small grey line in the route, for example "October 1957". Can stay empty.' }),
			field('summary', 'One-line summary', 'textarea', { required: true, help: 'Shown under the title of the part, and in the route when the reader reaches it.' }),
			field('body', 'Text', 'markdown', { required: true }),
			field('sortOrder', 'Order', 'int', {})
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
			field('nationality', 'Nationality', 'text', { required: true }),
			field('birthDate', 'Born', 'date', {}),
			field('deathDate', 'Died', 'date', {}),
			field('photoUrl', 'Photo', 'file', { help: FILE_HELP }),
			field('bio', 'Biography', 'textarea', { required: true })
		]
	},
	{
		key: 'entry-people',
		title: 'People on pages',
		singular: 'person on this page',
		labelField: 'role',
		listColumns: ['entryId', 'personId', 'role', 'isCrew'],
		children: [],
		parentField: 'entryId',
		sections: [],
		pageMap: [],
		fields: [
			field('entryId', 'Entry', 'ref', { required: true, refTable: 'entries', lockedOnEdit: true }),
			field('personId', 'Person', 'ref', { required: true, refTable: 'people', lockedOnEdit: true }),
			field('role', 'Role', 'text', { required: true, help: 'For example Commander or Chief designer.' }),
			field('isCrew', 'Was on board (crew)', 'checkbox', {}),
			field('sortOrder', 'Order', 'int', {})
		]
	},
	{
		key: 'media',
		title: 'Images and sounds',
		singular: 'image or sound',
		labelField: 'title',
		listColumns: ['title', 'entryId', 'type'],
		children: [],
		parentField: 'entryId',
		sections: [],
		pageMap: [],
		fields: [
			field('entryId', 'Entry', 'ref', { required: true, refTable: 'entries' }),
			field('type', 'Type', 'select', { required: true, options: ['Image', 'Audio', 'Signal', 'Video'] }),
			field('title', 'Title', 'text', { required: true }),
			field('url', 'File', 'file', { required: true, help: FILE_HELP }),
			field('caption', 'Caption', 'textarea', {}),
			field('credit', 'Credit', 'text', { help: 'Who made it, for example NASA.' }),
			field('license', 'Licence', 'text', { help: 'For example Public domain.' }),
			field('sortOrder', 'Order', 'int', {})
		]
	},
	{
		key: 'catalogue',
		title: 'Catalogue',
		singular: 'catalogue object',
		labelField: 'name',
		listColumns: ['name', 'type', 'constellation', 'isPublished'],
		children: [],
		parentField: '',
		pageMap: CASE_MAP,
		sections: [
			section('Title and intro', 'The name of the object and the sentences under it.', 'intro', ['name', 'slug', 'otherNames', 'summary']),
			section('Infobox', 'The picture and the measured values on the right.', 'info', ['imageUrl', 'type', 'constellation', 'distanceLy', 'massKg', 'radiusKm', 'apparentMagnitude', 'discoveredBy', 'discoveryYear']),
			section('Text of the page', 'Every "## Title" starts a new part, and each part gets a dot in the route.', 'parts', ['description']),
			section('Physics link', 'The last block of the page, linking to one Learn topic.', 'physics', ['relatedTopicId']),
			section('Publishing', 'Who can see this page.', '', ['isPublished'])
		],
		fields: [
			field('name', 'Name', 'text', { required: true }),
			field('slug', 'Slug', 'text', { required: true, help: SLUG_HELP }),
			field('otherNames', 'Other names', 'text', { help: 'Also found by the search, for example Sgr A*.' }),
			field('type', 'Type', 'select', { required: true, options: ['Star', 'Planet', 'Moon', 'DwarfPlanet', 'Galaxy', 'Nebula', 'BlackHole', 'Cluster', 'Other'] }),
			field('constellation', 'Constellation', 'text', {}),
			field('distanceLy', 'Distance (light-years)', 'number', {}),
			field('massKg', 'Mass (kg)', 'number', { help: 'For example 8.55e36. The page shows it in Earth or Sun masses.' }),
			field('radiusKm', 'Radius (km)', 'number', {}),
			field('apparentMagnitude', 'Apparent magnitude', 'number', { help: 'Leave empty when it can\'t be seen.' }),
			field('discoveredBy', 'Discovered by', 'text', {}),
			field('discoveryYear', 'Discovery year', 'int', {}),
			field('summary', 'Summary', 'textarea', { required: true }),
			field('description', 'Description', 'markdown', { required: true }),
			field('imageUrl', 'Image', 'file', { help: FILE_HELP }),
			field('relatedTopicId', 'Physics behind it', 'ref', { refTable: 'topics' }),
			field('isPublished', 'Published', 'checkbox', {})
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
