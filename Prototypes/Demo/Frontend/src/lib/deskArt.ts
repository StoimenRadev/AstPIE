// Everything on the desks that is drawn by code: the props of each desk (lamps, phones, cups...),
// the charts printed on paper, the pencil sketches, the outcome seal, clips and pins.
// Used on the server by lib/deskHtml.ts. Positions are in desk units (1600 x 940).

export function f1(n: number): string
{
	return n.toFixed(1);
}

// 'M' for the first point of a line, ' L' for the next ones
function step(first: boolean): string
{
	if (first)
	{
		return 'M';
	}
	return ' L';
}

export interface Desk
{
	name: string;
	// The stamp in the corner of every sheet
	archiveStamp: string;
	// The outer ring of the seal on the cover, per outcome
	seals: Record<string, string>;
	props: string[];
}

export const DESKS: Record<string, Desk> = {
	ussr: { name: 'Soviet desk', archiveStamp: 'АРХИВ', seals: { success: 'ЗАДАНИЕ ВЫПОЛНЕНО', partial: 'ВЫПОЛНЕНО ЧАСТИЧНО', ongoing: 'ПОЛЁТ ПРОДОЛЖАЕТСЯ', failure: 'ЗАДАНИЕ НЕ ВЫПОЛНЕНО' }, props: ['lampGreen', 'phoneRed', 'teaGlass', 'pencil', 'stampTool'] },
	usa: { name: 'American desk', archiveStamp: 'ARCHIVE COPY', seals: { success: 'MISSION ACCOMPLISHED', partial: 'PARTLY SUCCESSFUL', ongoing: 'MISSION IN PROGRESS', failure: 'MISSION FAILED' }, props: ['lampBrass', 'phoneBeige', 'coffeeMug', 'slideRule', 'punchCards'] },
	mystery: { name: 'Mystery desk', archiveStamp: 'CASE FILE', seals: {}, props: ['corkBoard', 'lampBlack', 'blackCoffee', 'magnifier', 'pencil', 'stampTool'] },
	observatory: { name: 'Observatory', archiveStamp: '', seals: {}, props: ['planisphere', 'redTorch'] }
};

export interface Prop
{
	// left (the default), right or middle: which part of a wider desk it moves with; stretch: it grows with it
	anchor: string;
	x: number;
	y: number;
	w: number;
	h: number;
	r: number;
	svg: () => string;
}

function phoneSvg(body: string, light: string, dark: string, label: string): string
{
	let holes = '';
	for (let i = 0; i < 10; i++)
	{
		const a = (-60 + i * 26) * Math.PI / 180;
		holes += '<circle cx="' + f1(90 + Math.cos(a) * 30) + '" cy="' + f1(138 + Math.sin(a) * 30) + '" r="7" fill="' + dark + '" stroke="#fff" stroke-opacity=".25"/>';
	}
	return '<svg viewBox="0 0 180 210"><defs><linearGradient id="ph-' + body.slice(1) + '" x1="0" y1="0" x2="1" y2="1"><stop offset="0" stop-color="' + light + '"/><stop offset="1" stop-color="' + body + '"/></linearGradient></defs>'
		+ '<path d="M150 190 C 175 205, 182 180, 168 170 S 150 150, 172 140" fill="none" stroke="' + dark + '" stroke-width="5"/>'
		+ '<path d="M34 54 Q30 40 46 38 H134 Q150 40 146 54 L162 184 Q164 198 148 198 H32 Q16 198 18 184 Z" fill="url(#ph-' + body.slice(1) + ')"/>'
		+ '<circle cx="90" cy="138" r="46" fill="' + dark + '" opacity=".35"/><circle cx="90" cy="138" r="42" fill="' + body + '"/>' + holes
		+ '<circle cx="90" cy="138" r="14" fill="' + label + '"/>'
		+ '<path d="M18 30 Q14 8 40 10 Q60 12 62 30 L62 40 H118 L118 30 Q120 12 140 10 Q166 8 162 30 Q160 52 138 50 H42 Q20 52 18 30 Z" fill="' + dark + '"/>'
		+ '<path d="M30 22 Q40 14 54 18" stroke="#fff" stroke-opacity=".3" stroke-width="4" fill="none" stroke-linecap="round"/></svg>';
}

// Seeded random numbers, so drawn star fields look the same on every visit
export function seeded(seedText: string): () => number
{
	let h = 1779033703 ^ seedText.length;
	for (let i = 0; i < seedText.length; i++)
	{
		h = Math.imul(h ^ seedText.charCodeAt(i), 3432918353);
		h = (h << 13) | (h >>> 19);
	}
	let a = h >>> 0;
	return () =>
	{
		a = (a + 0x6D2B79F5) | 0;
		let x = Math.imul(a ^ (a >>> 15), 1 | a);
		x = (x + Math.imul(x ^ (x >>> 7), 61 | x)) ^ x;
		return ((x ^ (x >>> 14)) >>> 0) / 4294967296;
	};
}

export const PROPS: Record<string, Prop> = {
	lampGreen: { anchor: 'left', x: -95, y: -70, w: 360, h: 250, r: 18, svg: () =>
	{
		return '<svg viewBox="0 0 360 250"><defs>'
			+ '<linearGradient id="lg-glass" x1="0" y1="0" x2="0" y2="1"><stop offset="0" stop-color="#3f8a60"/><stop offset=".45" stop-color="#1f5e3d"/><stop offset="1" stop-color="#0f3a24"/></linearGradient>'
			+ '<radialGradient id="lg-brass" cx=".38" cy=".35" r=".7"><stop offset="0" stop-color="#f3d58a"/><stop offset=".5" stop-color="#b98a35"/><stop offset="1" stop-color="#6e4d17"/></radialGradient>'
			+ '<linearGradient id="lg-rim" x1="0" y1="0" x2="1" y2="0"><stop offset="0" stop-color="#7a5a20"/><stop offset=".5" stop-color="#f0d080"/><stop offset="1" stop-color="#7a5a20"/></linearGradient></defs>'
			+ '<ellipse cx="96" cy="180" rx="74" ry="62" fill="url(#lg-brass)"/><ellipse cx="96" cy="180" rx="56" ry="46" fill="none" stroke="#5e4214" stroke-opacity=".5" stroke-width="2"/>'
			+ '<rect x="88" y="96" width="16" height="84" rx="6" fill="url(#lg-rim)"/>'
			+ '<rect x="36" y="44" width="300" height="74" rx="37" fill="url(#lg-glass)"/>'
			+ '<rect x="54" y="52" width="264" height="14" rx="7" fill="#9fe0b8" opacity=".35"/>'
			+ '<rect x="30" y="40" width="20" height="82" rx="8" fill="url(#lg-rim)"/><rect x="322" y="40" width="20" height="82" rx="8" fill="url(#lg-rim)"/>'
			+ '<path d="M184 118 v40" stroke="#b98a35" stroke-width="3"/><circle cx="184" cy="164" r="7" fill="url(#lg-brass)"/></svg>';
	} },
	lampBrass: { anchor: 'left', x: -175, y: -165, w: 330, h: 330, r: 0, svg: () =>
	{
		return '<svg viewBox="0 0 330 330"><defs>'
			+ '<radialGradient id="lb-dome" cx=".4" cy=".38" r=".62"><stop offset="0" stop-color="#2f6b47"/><stop offset=".7" stop-color="#173f29"/><stop offset="1" stop-color="#0c2618"/></radialGradient>'
			+ '<radialGradient id="lb-brass" cx=".4" cy=".35" r=".7"><stop offset="0" stop-color="#f6dc95"/><stop offset=".55" stop-color="#b48530"/><stop offset="1" stop-color="#5f4212"/></radialGradient></defs>'
			+ '<circle cx="165" cy="165" r="132" fill="url(#lb-brass)"/><circle cx="165" cy="165" r="118" fill="url(#lb-dome)"/>'
			+ '<path d="M98 112 a88 88 0 0 1 96 -38" stroke="#8fd3a8" stroke-opacity=".35" stroke-width="12" fill="none" stroke-linecap="round"/>'
			+ '<circle cx="165" cy="165" r="20" fill="url(#lb-brass)"/></svg>';
	} },
	phoneRed: { anchor: 'right', x: 1430, y: -8, w: 180, h: 210, r: -14, svg: () => phoneSvg('#8f2620', '#b33a2f', '#5e1612', '#f3ead8') },
	phoneBeige: { anchor: 'right', x: 1430, y: -8, w: 180, h: 210, r: -10, svg: () => phoneSvg('#cdbf9f', '#e2d6b8', '#a49572', '#f7f2e6') },
	teaGlass: { anchor: 'middle', x: 786, y: -18, w: 112, h: 112, r: 0, svg: () =>
	{
		let ornament = '';
		for (let i = 0; i < 24; i++)
		{
			const a = i / 24 * Math.PI * 2;
			ornament += '<circle cx="' + f1(56 + Math.cos(a) * 49) + '" cy="' + f1(56 + Math.sin(a) * 49) + '" r="1.6" fill="#6f6a5e"/>';
		}
		return '<svg viewBox="0 0 112 112"><defs>'
			+ '<radialGradient id="tg-metal" cx=".35" cy=".3" r=".8"><stop offset="0" stop-color="#f1ede2"/><stop offset=".6" stop-color="#a9a291"/><stop offset="1" stop-color="#5d584c"/></radialGradient>'
			+ '<radialGradient id="tg-tea" cx=".45" cy=".4" r=".65"><stop offset="0" stop-color="#c77426"/><stop offset=".7" stop-color="#7c3a0c"/><stop offset="1" stop-color="#4a2006"/></radialGradient></defs>'
			+ '<rect x="96" y="44" width="16" height="24" rx="7" fill="url(#tg-metal)"/>'
			+ '<circle cx="56" cy="56" r="54" fill="url(#tg-metal)"/>' + ornament
			+ '<circle cx="56" cy="56" r="43" fill="#e8f0ec" opacity=".85"/><circle cx="56" cy="56" r="39" fill="url(#tg-tea)"/>'
			+ '<circle cx="66" cy="46" r="13" fill="#f6e48a" stroke="#e8c84a" stroke-width="2"/><path d="M66 33 v26 M53 46 h26 M57 37 l18 18 M75 37 l-18 18" stroke="#e8c84a" stroke-width="1.2"/>'
			+ '<path d="M38 66 L4 100" stroke="#cfc8b8" stroke-width="5" stroke-linecap="round"/><ellipse cx="42" cy="62" rx="9" ry="6" transform="rotate(-45 42 62)" fill="#d9d3c4"/>'
			+ '<ellipse cx="40" cy="38" rx="12" ry="5" fill="#fff" opacity=".18" transform="rotate(-30 40 38)"/></svg>';
	} },
	coffeeMug: { anchor: 'middle', x: 780, y: -20, w: 130, h: 118, r: 0, svg: () =>
	{
		return '<svg viewBox="0 0 130 118"><defs><radialGradient id="cm-c" cx=".45" cy=".4" r=".65"><stop offset="0" stop-color="#5a3518"/><stop offset="1" stop-color="#2a160a"/></radialGradient></defs>'
			+ '<circle cx="58" cy="60" r="56" fill="none" stroke="#3b2414" stroke-opacity=".35" stroke-width="3"/>'
			+ '<rect x="94" y="46" width="34" height="26" rx="12" fill="none" stroke="#efe9dc" stroke-width="9"/>'
			+ '<circle cx="58" cy="58" r="44" fill="#f2ede1"/><circle cx="58" cy="58" r="37" fill="url(#cm-c)"/>'
			+ '<ellipse cx="46" cy="46" rx="14" ry="5" fill="#fff" opacity=".2" transform="rotate(-30 46 46)"/></svg>';
	} },
	pencil: { anchor: 'left', x: 530, y: 892, w: 270, h: 26, r: -7, svg: () =>
	{
		return '<svg viewBox="0 0 270 26"><rect x="0" y="4" width="214" height="18" fill="#2f6e4a"/><rect x="0" y="4" width="214" height="6" fill="#4b9a6c"/>'
			+ '<rect x="0" y="4" width="20" height="18" fill="#c9b27a"/><path d="M214 4 L252 13 L214 22 Z" fill="#e8c999"/><path d="M240 10 L262 13 L240 16 Z" fill="#333"/></svg>';
	} },
	stampTool: { anchor: 'left', x: 806, y: 850, w: 120, h: 86, r: 6, svg: () =>
	{
		return '<svg viewBox="0 0 120 86"><rect x="2" y="16" width="70" height="56" rx="6" fill="#3b2c22"/><rect x="8" y="22" width="58" height="44" rx="3" fill="#8e1f1b"/>'
			+ '<rect x="70" y="20" width="46" height="46" rx="5" fill="#5b3a22"/><circle cx="93" cy="43" r="17" fill="#7a5130"/><circle cx="88" cy="38" r="6" fill="#a8774b" opacity=".6"/></svg>';
	} },
	slideRule: { anchor: 'left', x: 470, y: 900, w: 420, h: 46, r: -4, svg: () =>
	{
		let ticks = '';
		for (let i = 0; i <= 80; i++)
		{
			const x = 14 + i * 4.8;
			let h = 4;
			if (i % 5 === 0)
			{
				h = 7;
			}
			if (i % 10 === 0)
			{
				h = 10;
			}
			ticks += '<line x1="' + f1(x) + '" y1="9" x2="' + f1(x) + '" y2="' + (9 + h) + '" stroke="#333" stroke-width=".8"/><line x1="' + f1(x) + '" y1="37" x2="' + f1(x) + '" y2="' + (37 - h) + '" stroke="#333" stroke-width=".8"/>';
		}
		return '<svg viewBox="0 0 420 46"><rect x="0" y="4" width="420" height="38" rx="3" fill="#efe8d4"/><rect x="0" y="19" width="420" height="8" fill="#e2d9bf"/>' + ticks
			+ '<rect x="190" y="0" width="34" height="46" rx="3" fill="#cfe3e8" fill-opacity=".45" stroke="#5c5c5c" stroke-width="2"/><line x1="207" y1="0" x2="207" y2="46" stroke="#b02a2a" stroke-width="1"/></svg>';
	} },
	punchCards: { anchor: 'left', x: 790, y: 846, w: 150, h: 92, r: 8, svg: () =>
	{
		let holes = '';
		for (let row = 0; row < 6; row++)
		{
			for (let c = 0; c < 18; c++)
			{
				if ((row * 7 + c * 3) % 5 === 0)
				{
					holes += '<rect x="' + (14 + c * 7) + '" y="' + (18 + row * 11) + '" width="3" height="6" fill="#3a2a1c"/>';
				}
			}
		}
		return '<svg viewBox="0 0 150 92"><path d="M10 6 h134 v80 h-140 v-74 z" fill="#d9cfae" transform="rotate(-6 75 46)"/>'
			+ '<path d="M10 6 h134 v80 h-140 v-74 z" fill="#ece3c6"/>' + holes + '<path d="M4 12 L10 6" stroke="#cbbf9a"/></svg>';
	} },
	corkBoard: { anchor: 'stretch', x: 872, y: 14, w: 724, h: 574, r: 0, svg: () =>
	{
		return '<svg viewBox="0 0 724 574" preserveAspectRatio="none"><defs>'
			+ '<filter id="cork" x="0" y="0" width="100%" height="100%"><feTurbulence type="fractalNoise" baseFrequency=".9" numOctaves="2" seed="7"/><feColorMatrix type="matrix" values="0 0 0 0 .55  0 0 0 0 .36  0 0 0 0 .2  0 0 0 -1.6 1.25"/></filter>'
			+ '<filter id="cork2" x="0" y="0" width="100%" height="100%"><feTurbulence type="fractalNoise" baseFrequency=".06" numOctaves="3" seed="2"/><feColorMatrix type="matrix" values="0 0 0 0 .3  0 0 0 0 .18  0 0 0 0 .08  0 0 0 -1.8 1.1"/></filter>'
			+ '<linearGradient id="frame" x1="0" y1="0" x2="1" y2="1"><stop offset="0" stop-color="#6b4528"/><stop offset="1" stop-color="#3a2414"/></linearGradient></defs>'
			+ '<rect width="724" height="574" rx="6" fill="url(#frame)"/>'
			+ '<rect x="16" y="16" width="692" height="542" fill="#b88a5a"/>'
			+ '<rect x="16" y="16" width="692" height="542" filter="url(#cork)"/>'
			+ '<rect x="16" y="16" width="692" height="542" filter="url(#cork2)" opacity=".5"/>'
			+ '<rect x="16" y="16" width="692" height="542" fill="none" stroke="#2a180c" stroke-opacity=".5" stroke-width="3"/></svg>';
	} },
	lampBlack: { anchor: 'left', x: -120, y: -110, w: 330, h: 330, r: 0, svg: () =>
	{
		return '<svg viewBox="0 0 330 330"><defs><radialGradient id="lk-dome" cx=".42" cy=".38" r=".62"><stop offset="0" stop-color="#4a4a48"/><stop offset=".7" stop-color="#1c1c1b"/><stop offset="1" stop-color="#0c0c0c"/></radialGradient></defs>'
			+ '<circle cx="165" cy="165" r="128" fill="#0b0b0b"/><circle cx="165" cy="165" r="118" fill="url(#lk-dome)"/>'
			+ '<path d="M96 118 a88 88 0 0 1 92 -44" stroke="#fff" stroke-opacity=".16" stroke-width="12" fill="none" stroke-linecap="round"/>'
			+ '<circle cx="165" cy="165" r="18" fill="#2c2c2b"/></svg>';
	} },
	blackCoffee: { anchor: 'middle', x: 780, y: -20, w: 130, h: 118, r: 0, svg: () =>
	{
		return '<svg viewBox="0 0 130 118"><defs><radialGradient id="bc-c" cx=".45" cy=".4" r=".65"><stop offset="0" stop-color="#3a2210"/><stop offset="1" stop-color="#140a04"/></radialGradient></defs>'
			+ '<rect x="94" y="46" width="34" height="26" rx="12" fill="none" stroke="#2c2c2c" stroke-width="9"/>'
			+ '<circle cx="58" cy="58" r="44" fill="#333"/><circle cx="58" cy="58" r="37" fill="url(#bc-c)"/>'
			+ '<ellipse cx="46" cy="46" rx="14" ry="5" fill="#fff" opacity=".14" transform="rotate(-30 46 46)"/></svg>';
	} },
	magnifier: { anchor: 'left', x: 790, y: 830, w: 250, h: 120, r: -16, svg: () =>
	{
		return '<svg viewBox="0 0 250 120"><defs><radialGradient id="mg-g" cx=".4" cy=".35" r=".7"><stop offset="0" stop-color="#e9f3f6" stop-opacity=".55"/><stop offset="1" stop-color="#9fb6bd" stop-opacity=".25"/></radialGradient></defs>'
			+ '<rect x="112" y="50" width="130" height="20" rx="9" fill="#2b1a10"/><rect x="112" y="50" width="130" height="6" rx="3" fill="#5a3a22"/>'
			+ '<circle cx="60" cy="60" r="56" fill="#b9a46a"/><circle cx="60" cy="60" r="48" fill="url(#mg-g)"/>'
			+ '<path d="M30 40 a36 36 0 0 1 30 -16" stroke="#fff" stroke-opacity=".6" stroke-width="5" fill="none" stroke-linecap="round"/></svg>';
	} },
	planisphere: { anchor: 'left', x: -120, y: 700, w: 300, h: 300, r: -18, svg: () =>
	{
		let s = '<svg viewBox="0 0 300 300"><circle cx="150" cy="150" r="148" fill="#2d3c58"/><circle cx="150" cy="150" r="134" fill="#13203a"/>';
		const random = seeded('planisphere');
		for (let i = 0; i < 90; i++)
		{
			const a = random() * Math.PI * 2;
			const d = Math.sqrt(random()) * 128;
			s += '<circle cx="' + f1(150 + Math.cos(a) * d) + '" cy="' + f1(150 + Math.sin(a) * d) + '" r="' + (0.6 + random() * 1.4).toFixed(2) + '" fill="#e8eef8"/>';
		}
		for (let k = 0; k < 72; k++)
		{
			const a = k / 72 * Math.PI * 2;
			let length = 140;
			if (k % 6 === 0)
			{
				length = 146;
			}
			s += '<line x1="' + f1(150 + Math.cos(a) * 134) + '" y1="' + f1(150 + Math.sin(a) * 134) + '" x2="' + f1(150 + Math.cos(a) * length) + '" y2="' + f1(150 + Math.sin(a) * length) + '" stroke="#c9d3e6" stroke-width="1"/>';
		}
		s += '<ellipse cx="150" cy="150" rx="120" ry="90" fill="none" stroke="#c9a85b" stroke-width="3" opacity=".8"/><circle cx="150" cy="150" r="5" fill="#c9a85b"/></svg>';
		return s;
	} },
	redTorch: { anchor: 'left', x: 836, y: 880, w: 190, h: 56, r: -8, svg: () =>
	{
		return '<svg viewBox="0 0 190 56"><rect x="0" y="14" width="140" height="28" rx="8" fill="#26292d"/><rect x="0" y="14" width="140" height="8" rx="4" fill="#3d4247"/>'
			+ '<rect x="136" y="6" width="44" height="44" rx="10" fill="#2f3337"/><circle cx="168" cy="28" r="16" fill="#ff3a2e"/><circle cx="168" cy="28" r="22" fill="#ff3a2e" opacity=".25"/>'
			+ '<rect x="60" y="20" width="14" height="16" rx="3" fill="#8a1d16"/></svg>';
	} }
};

// ---------------------------------------------------------------- charts printed on computer paper

function chartSvg(kind: string): string
{
	const ink = '#1f2a3a';
	let s = '<svg viewBox="0 0 300 190" style="width:100%;height:auto;display:block">';
	if (kind === 'doppler')
	{
		s += '<line x1="20" y1="95" x2="290" y2="95" stroke="' + ink + '" stroke-opacity=".35" stroke-dasharray="3 4"/>';
		s += '<line x1="155" y1="15" x2="155" y2="170" stroke="' + ink + '" stroke-opacity=".2"/>';
		let d = '';
		for (let i = 0; i <= 120; i++)
		{
			const time = -6 + i * 0.1;
			const v = -time / Math.sqrt(time * time + 1.2);
			d += step(i === 0) + f1(20 + i * 2.25) + ' ' + f1(95 - v * 62);
		}
		s += '<path d="' + d + '" fill="none" stroke="#b3342a" stroke-width="2.6"/>';
		s += '<text x="24" y="24" font-family="PT Mono" font-size="10" fill="' + ink + '">HIGHER BEEP</text><text x="286" y="182" text-anchor="end" font-family="PT Mono" font-size="10" fill="' + ink + '">LOWER BEEP</text>';
		s += '<text x="160" y="182" font-family="PT Mono" font-size="9" fill="' + ink + '" opacity=".7">CLOSEST</text>';
	}
	if (kind === 'decay')
	{
		s += '<line x1="30" y1="170" x2="290" y2="170" stroke="' + ink + '" stroke-opacity=".5"/><line x1="30" y1="15" x2="30" y2="170" stroke="' + ink + '" stroke-opacity=".5"/>';
		let high = '';
		let low = '';
		for (let i = 0; i <= 100; i++)
		{
			const f = i / 100;
			const apogee = 939 - (939 - 200) * Math.pow(f, 1.6);
			const perigee = 215 - 40 * Math.pow(f, 3);
			const x = 30 + f * 250;
			high += step(i === 0) + f1(x) + ' ' + f1(170 - apogee / 960 * 150);
			low += step(i === 0) + f1(x) + ' ' + f1(170 - perigee / 960 * 150);
		}
		s += '<path d="' + high + '" fill="none" stroke="#b3342a" stroke-width="2.4"/><path d="' + low + '" fill="none" stroke="' + ink + '" stroke-width="2"/>';
		s += '<text x="40" y="26" font-family="PT Mono" font-size="10" fill="#b3342a">HIGHEST POINT 939 KM</text><text x="40" y="150" font-family="PT Mono" font-size="10" fill="' + ink + '">LOWEST POINT 215 KM</text>';
		s += '<text x="30" y="184" font-family="PT Mono" font-size="9" fill="' + ink + '">OCT 1957</text><text x="290" y="184" text-anchor="end" font-family="PT Mono" font-size="9" fill="' + ink + '">JAN 1958</text>';
	}
	if (kind === 'counts')
	{
		s += '<line x1="30" y1="160" x2="290" y2="160" stroke="' + ink + '" stroke-opacity=".5"/>';
		const values = [6, 9, 13, 19, 27, 38, 52, 0, 0, 0];
		for (let i = 0; i < values.length; i++)
		{
			const h = values[i] * 2.4;
			let fill = ink;
			if (values[i] === 0)
			{
				fill = 'none';
				s += '<text x="' + (48 + i * 25) + '" y="154" text-anchor="middle" font-family="PT Mono" font-size="10" fill="#b3342a">0</text>';
			}
			s += '<rect x="' + (40 + i * 25) + '" y="' + f1(160 - h) + '" width="16" height="' + f1(h) + '" fill="' + fill + '"/>';
		}
		s += '<text x="30" y="176" font-family="PT Mono" font-size="9" fill="' + ink + '">LOW</text><text x="290" y="176" text-anchor="end" font-family="PT Mono" font-size="9" fill="' + ink + '">HIGH ABOVE THE EARTH</text>';
		s += '<text x="292" y="24" text-anchor="end" font-family="Caveat" font-size="19" fill="#b3342a">zero here = overwhelmed</text><path d="M262 30 L250 132" stroke="#b3342a" stroke-width="1.4" fill="none"/>';
	}
	return s + '</svg>';
}

// One dot per day (or per week); the first "loud" dots in red
export function daysSvg(from: string, to: string, loud: number, weekly: boolean): string
{
	const a = new Date(from + 'T00:00:00Z').getTime();
	const b = new Date(to + 'T00:00:00Z').getTime();
	let n = Math.round((b - a) / 86400000);
	let columns = 16;
	let gap = 17;
	let radius = 5.6;
	if (weekly)
	{
		n = Math.round(n / 7);
		columns = 32;
		gap = 8.6;
		radius = 3;
	}
	const rows = Math.ceil(n / columns);
	let s = '<svg viewBox="0 0 ' + f1(columns * gap + 6) + ' ' + f1(rows * gap + 6) + '" style="width:100%;height:auto;display:block">';
	for (let i = 0; i < n; i++)
	{
		const x = 3 + (i % columns) * gap + gap / 2;
		const y = 3 + Math.floor(i / columns) * gap + gap / 2;
		let fill = '#24262b';
		if (i < loud)
		{
			fill = '#b3342a';
		}
		s += '<circle cx="' + f1(x) + '" cy="' + f1(y) + '" r="' + radius + '" fill="' + fill + '"/>';
	}
	return s + '</svg>';
}

// Round outcome seal for the cover sheet: the desk's words round the edge, the outcome inside
export function sealSvg(outer: string, inner: string): string
{
	const words = inner.split(' ');
	return '<svg viewBox="0 0 200 200"><defs><path id="seal-path" d="M100 100 m-74 0 a74 74 0 1 1 148 0 a74 74 0 1 1 -148 0"/></defs>'
		+ '<circle cx="100" cy="100" r="94" fill="none" stroke="currentColor" stroke-width="6"/><circle cx="100" cy="100" r="58" fill="none" stroke="currentColor" stroke-width="3"/>'
		+ '<text font-family="PT Sans Narrow" font-weight="700" font-size="19" letter-spacing="2" fill="currentColor"><textPath href="#seal-path" startOffset="0">' + outer + ' · ' + outer + ' · </textPath></text>'
		+ '<text x="100" y="96" text-anchor="middle" font-family="PT Sans Narrow" font-weight="700" font-size="22" fill="currentColor">' + words[0] + '</text>'
		+ '<text x="100" y="120" text-anchor="middle" font-family="PT Sans Narrow" font-weight="700" font-size="22" fill="currentColor">' + words.slice(1).join(' ') + '</text></svg>';
}

export const CLIP = '<svg class="clip" viewBox="0 0 30 80"><path d="M10 72 V14 a8 8 0 0 1 16 0 V62 a12 12 0 0 1 -24 0 V20" fill="none" stroke="#9ea3a8" stroke-width="3.2" stroke-linecap="round"/></svg>';

// Pushpin head for the cork board
export const PIN = '<svg class="pin" viewBox="0 0 30 30"><circle cx="15" cy="15" r="10" fill="currentColor"/><circle cx="12" cy="11" r="3.5" fill="#fff" opacity=".45"/></svg>';

// A reel of tape
export const REEL = '<svg viewBox="0 0 80 80"><circle cx="40" cy="40" r="37" fill="#3a3530"/><circle cx="40" cy="40" r="30" fill="#5a4636"/><circle cx="40" cy="40" r="11" fill="#d9d2c0"/><circle cx="40" cy="40" r="4" fill="#3a3530"/><path d="M40 29 v-14 M49.5 45.5 l12 7 M30.5 45.5 l-12 7" stroke="#d9d2c0" stroke-width="5" stroke-linecap="round"/></svg>';

// ---------------------------------------------------------------- charts on paper for the mystery desk and the observatory

// A bell curve fitted through the values (a parabola through their logarithms)
function bellFit(values: number[]): (x: number) => number
{
	let n = 0;
	let sx = 0;
	let sx2 = 0;
	let sx3 = 0;
	let sx4 = 0;
	let sy = 0;
	let sxy = 0;
	let sx2y = 0;
	for (let i = 0; i < values.length; i++)
	{
		const y = Math.log(values[i]);
		n += 1;
		sx += i;
		sx2 += i * i;
		sx3 += i * i * i;
		sx4 += i * i * i * i;
		sy += y;
		sxy += i * y;
		sx2y += i * i * y;
	}
	const A = [[sx4, sx3, sx2], [sx3, sx2, sx], [sx2, sx, n]];
	const B = [sx2y, sxy, sy];
	const det = (m: number[][]) => m[0][0] * (m[1][1] * m[2][2] - m[1][2] * m[2][1]) - m[0][1] * (m[1][0] * m[2][2] - m[1][2] * m[2][0]) + m[0][2] * (m[1][0] * m[2][1] - m[1][1] * m[2][0]);
	const d = det(A);
	const column = (k: number) =>
	{
		return det(A.map((row, i) =>
		{
			const copy = row.slice();
			copy[k] = B[i];
			return copy;
		})) / d;
	};
	const a = column(0);
	const b = column(1);
	const c = column(2);
	return (x) => Math.exp(a * x * x + b * x + c);
}

function wowChartSvg(): string
{
	const values = [6, 14, 26, 30, 19, 5];
	const chars = ['6', 'E', 'Q', 'U', 'J', '5'];
	const fit = bellFit(values);
	const yOf = (x: number) => 150 - x / 32 * 125;
	let s = '<svg viewBox="0 0 300 190" style="width:100%;height:auto;display:block">';
	s += '<line x1="20" y1="150" x2="290" y2="150" stroke="#1f2a3a" stroke-opacity=".5"/>';
	for (let i = 0; i < 6; i++)
	{
		const x = 45 + i * 44;
		s += '<rect x="' + (x - 13) + '" y="' + f1(yOf(values[i])) + '" width="26" height="' + f1(150 - yOf(values[i])) + '" fill="#1f2a3a"/>';
		s += '<text x="' + x + '" y="' + f1(yOf(values[i]) - 5) + '" text-anchor="middle" font-family="PT Mono" font-size="10" fill="#1f2a3a">' + values[i] + '</text>';
		s += '<text x="' + x + '" y="172" text-anchor="middle" font-family="PT Mono" font-size="17" font-weight="700" fill="#b3342a">' + chars[i] + '</text>';
	}
	let d = '';
	for (let x = -0.5; x <= 5.5; x += 0.1)
	{
		d += step(d === '') + f1(45 + x * 44) + ' ' + f1(yOf(fit(x)));
	}
	s += '<path d="' + d + '" fill="none" stroke="#b3342a" stroke-width="1.6" stroke-dasharray="2 4"/>';
	s += '<text x="20" y="22" font-family="Caveat" font-size="20" fill="#b3342a">Wow!</text>';
	return s + '</svg>';
}

function transitPaperSvg(): string
{
	const random = seeded('k2-18-light');
	const yOf = (b: number) => 30 + (1.0012 - b) / (1.0012 - 0.9958) * 120;
	let s = '<svg viewBox="0 0 300 190" style="width:100%;height:auto;display:block">';
	s += '<line x1="30" y1="' + f1(yOf(1)) + '" x2="295" y2="' + f1(yOf(1)) + '" stroke="#1f2a3a" stroke-opacity=".25"/>';
	s += '<text x="26" y="' + f1(yOf(1) + 3) + '" text-anchor="end" font-family="PT Mono" font-size="8" fill="#1f2a3a">100%</text>';
	s += '<text x="26" y="' + f1(yOf(0.997) + 3) + '" text-anchor="end" font-family="PT Mono" font-size="8" fill="#1f2a3a">99.7%</text>';
	const dips = [9, 42, 75];
	for (let day = 0; day <= 84; day += 0.28)
	{
		let b = 1 + (random() - 0.5) * 0.0009;
		for (let k = 0; k < 3; k++)
		{
			const dd = Math.abs(day - dips[k]);
			if (dd < 0.9)
			{
				b -= 0.003 * Math.min(1, (0.9 - dd) / 0.35);
			}
		}
		s += '<circle cx="' + f1(30 + day / 84 * 265) + '" cy="' + f1(yOf(b)) + '" r="1.3" fill="#1f2a3a"/>';
	}
	for (let k = 0; k < 3; k++)
	{
		let label = '+33 days';
		if (k === 0)
		{
			label = 'a dip';
		}
		s += '<text x="' + f1(30 + dips[k] / 84 * 265) + '" y="176" text-anchor="middle" font-family="Caveat" font-size="15" fill="#b3342a">' + label + '</text>';
	}
	return s + '</svg>';
}

// Orbit of the star S2 around Sgr A*: an ellipse with e = 0.886, the black hole at one focus
function s2OrbitSvg(): string
{
	const e = 0.886;
	const a = 120;
	const b = a * Math.sqrt(1 - e * e);
	const fx = 150 + a * e;
	let s = '<svg viewBox="0 0 300 190" style="width:100%;height:auto;display:block"><g transform="rotate(-20 150 95)">';
	s += '<ellipse cx="150" cy="95" rx="' + a + '" ry="' + f1(b) + '" fill="none" stroke="#1f2a3a" stroke-width="1.4"/>';
	for (let k = 0; k < 16; k++)
	{
		const M = k / 16 * Math.PI * 2;
		let E = M;
		for (let it = 0; it < 40; it++)
		{
			E = M + e * Math.sin(E);
		}
		const x = 150 + a * Math.cos(E);
		const y = 95 + b * Math.sin(E);
		let radius = 2.2;
		let fill = '#1f2a3a';
		if (k === 0)
		{
			radius = 4;
			fill = '#b3342a';
		}
		s += '<circle cx="' + f1(x) + '" cy="' + f1(y) + '" r="' + radius + '" fill="' + fill + '"/>';
	}
	s += '<circle cx="' + f1(fx) + '" cy="95" r="5" fill="#111"/></g>';
	s += '<text x="226" y="88" font-family="PT Mono" font-size="9" fill="#1f2a3a">SGR A*</text>';
	s += '<text x="16" y="180" font-family="Caveat" font-size="16" fill="#1f2a3a">one dot per year · red: closest, 2002 and 2018</text>';
	return s + '</svg>';
}

// Big Ear from the side
export function bigEarSvg(): string
{
	const ink = '#28457a';
	let s = '<svg viewBox="0 0 1000 430" style="width:100%;height:auto;display:block" role="img" aria-label="Side view of the Big Ear telescope">';
	s += '<g fill="none" stroke="' + ink + '" stroke-width="3" stroke-linecap="round" stroke-linejoin="round">';
	s += '<line x1="40" y1="352" x2="960" y2="352"/><rect x="196" y="340" width="666" height="12" fill="#d7e0ee"/>';
	s += '<path d="M128 352 L196 352 L150 96 L118 104 Z" fill="#e6ecf6"/><circle cx="168" cy="352" r="7" fill="#fff"/>';
	s += '<path d="M872 352 C 918 300 922 170 880 92 L 900 86 C 946 170 942 304 896 352 Z" fill="#e6ecf6"/>';
	s += '<path d="M232 352 L226 322 L246 322 L240 352 Z" fill="#fff"/><path d="M256 352 L250 322 L270 322 L264 352 Z" fill="#fff"/></g>';
	s += '<g fill="none" stroke="#b3342a" stroke-width="2.4" stroke-dasharray="8 7" stroke-linecap="round">';
	const ys = [150, 230, 300];
	for (let i = 0; i < ys.length; i++)
	{
		const xw = 150 - (ys[i] - 96) * (32 / 256) + 6;
		s += '<path d="M' + f1(xw - 70) + ' ' + f1(ys[i] - 120) + ' L' + f1(xw) + ' ' + ys[i] + ' L894 ' + ys[i] + ' L248 326"/>';
	}
	s += '</g><g font-family="Caveat" font-size="32" fill="' + ink + '">';
	s += '<text x="30" y="406">flat reflector, tilts</text><text x="390" y="406">ground plane, aluminium</text><text x="970" y="406" text-anchor="end">curved reflector</text>';
	s += '<text x="280" y="306">two horns</text><text x="60" y="36" fill="#b3342a">radio waves from the sky</text></g>';
	return s + '</svg>';
}

// A drawing made by code, by its name (only Big Ear for now)
export function drawingSvg(name: string): string
{
	if (name === 'bigear')
	{
		return bigEarSvg();
	}
	return bigEarSvg();
}

export function paperChart(kind: string): string
{
	if (kind === 'wow')
	{
		return wowChartSvg();
	}
	if (kind === 'transit')
	{
		return transitPaperSvg();
	}
	if (kind === 's2')
	{
		return s2OrbitSvg();
	}
	return chartSvg(kind);
}

// ---------------------------------------------------------------- pencil sketches

// Spin along the length versus tumbling end over end
function spinSketchSvg(): string
{
	const pen = '#2f4f8f';
	return '<svg viewBox="0 0 320 220" style="width:100%;height:auto;display:block" fill="none" stroke="' + pen + '" stroke-width="2" stroke-linecap="round">'
		+ '<rect x="24" y="26" width="120" height="22" rx="11"/><ellipse cx="24" cy="37" rx="6" ry="11"/>'
		+ '<line x1="8" y1="37" x2="168" y2="37" stroke-dasharray="4 5" stroke-width="1.4"/>'
		+ '<path d="M160 24 a9 13 0 1 1 0 26"/><path d="M156 46 l5 5 l3 -7"/>'
		+ '<text x="20" y="82" font-family="Caveat" font-size="21" fill="' + pen + '" stroke="none">planned: spin along its length</text>'
		+ '<rect x="239" y="118" width="22" height="88" rx="11" transform="rotate(28 250 162)"/>'
		+ '<path d="M200 178 a52 52 0 0 1 94 -40"/><path d="M286 132 l9 6 l-10 4"/>'
		+ '<text x="20" y="160" font-family="Caveat" font-size="21" fill="#b3342a" stroke="none">in orbit:</text><text x="20" y="184" font-family="Caveat" font-size="21" fill="#b3342a" stroke="none">end over end</text></svg>';
}

// Luna 1: the planned path into the Moon and the real path past it (not to scale)
function lunaPathSvg(): string
{
	const pen = '#2f4f8f';
	return '<svg viewBox="0 0 340 210" style="width:100%;height:auto;display:block" fill="none" stroke-linecap="round">'
		+ '<circle cx="46" cy="150" r="26" fill="#cfe0f2" stroke="' + pen + '" stroke-width="2"/>'
		+ '<path d="M46 150 m-120 0 a250 210 0 0 1 340 -90" stroke="' + pen + '" stroke-opacity=".25" stroke-dasharray="3 6"/>'
		+ '<circle cx="250" cy="70" r="13" fill="#e8e3d6" stroke="' + pen + '" stroke-width="2"/>'
		+ '<path d="M66 134 C 130 90, 190 76, 238 72" stroke="' + pen + '" stroke-width="2" stroke-dasharray="7 6"/>'
		+ '<path d="M66 134 C 130 82, 196 52, 252 44 S 318 30, 336 18" stroke="#b3342a" stroke-width="2.6"/>'
		+ '<path d="M326 14 l10 4 l-8 8" stroke="#b3342a" stroke-width="2.4"/>'
		+ '<line x1="250" y1="57" x2="252" y2="45" stroke="#b3342a" stroke-width="1.2"/>'
		+ '<text x="14" y="196" font-family="Caveat" font-size="18" fill="' + pen + '" stroke="none">Earth</text>'
		+ '<text x="268" y="96" font-family="Caveat" font-size="18" fill="' + pen + '" stroke="none">Moon</text>'
		+ '<text x="104" y="128" font-family="Caveat" font-size="17" fill="' + pen + '" stroke="none">planned: hit it</text>'
		+ '<text x="160" y="30" font-family="Caveat" font-size="17" fill="#b3342a" stroke="none">real: 6 000 km wide</text>'
		+ '<text x="226" y="150" font-family="Caveat" font-size="17" fill="#b3342a" stroke="none">then on, around the Sun</text></svg>';
}

// Sputnik's orbit to scale: Earth radius 6 371 km, lowest point 215 km, highest point about 940 km
function sputnikOrbitSvg(): string
{
	const pen = '#2f4f8f';
	const k = 70 / 6371;
	const a = (6371 * 2 + 215 + 940) / 2 * k;
	const c = (940 - 215) / 2 * k;
	const b = Math.sqrt(a * a - c * c);
	const cx = 150;
	const cy = 112;
	return '<svg viewBox="0 0 340 230" style="width:100%;height:auto;display:block" fill="none" stroke-linecap="round">'
		+ '<circle cx="' + cx + '" cy="' + cy + '" r="70" fill="#cfe0f2" stroke="' + pen + '" stroke-width="2"/>'
		+ '<ellipse cx="' + (cx - c).toFixed(1) + '" cy="' + cy + '" rx="' + a.toFixed(1) + '" ry="' + b.toFixed(1) + '" stroke="#b3342a" stroke-width="2" stroke-dasharray="5 4"/>'
		+ '<circle cx="' + (cx - c - a).toFixed(1) + '" cy="' + cy + '" r="3.5" fill="#b3342a" stroke="none"/>'
		+ '<circle cx="' + (cx - c + a).toFixed(1) + '" cy="' + cy + '" r="3.5" fill="#b3342a" stroke="none"/>'
		+ '<path d="M' + (cx - c + a + 6).toFixed(1) + ' ' + cy + ' L246 ' + (cy - 50) + '" stroke="#b3342a" stroke-width="1.3"/>'
		+ '<path d="M' + (cx - c - a - 6).toFixed(1) + ' ' + cy + ' L44 ' + (cy + 52) + '" stroke="#b3342a" stroke-width="1.3"/>'
		+ '<text x="204" y="' + (cy - 56) + '" font-family="Caveat" font-size="18" fill="#b3342a" stroke="none">highest: about 940 km</text>'
		+ '<text x="4" y="' + (cy + 70) + '" font-family="Caveat" font-size="18" fill="#b3342a" stroke="none">lowest: 215 km</text>'
		+ '<text x="' + (cx - 24) + '" y="' + (cy + 6) + '" font-family="Caveat" font-size="20" fill="' + pen + '" stroke="none">Earth</text>'
		+ '<text x="196" y="222" font-family="Caveat" font-size="17" fill="' + pen + '" stroke="none">drawn to scale</text></svg>';
}

// Luna 1 around the Sun, roughly to scale: Earth at 1 AU, Luna 1 between 0.98 and 1.31 AU, Mars at about 1.52 AU
function lunaSunSvg(): string
{
	const pen = '#2f4f8f';
	const k = 62;
	const cx = 170;
	const cy = 112;
	const a = (0.9766 + 1.3146) / 2 * k;
	const c = (1.3146 - 0.9766) / 2 * k;
	const b = Math.sqrt(a * a - c * c);
	return '<svg viewBox="0 0 340 230" style="width:100%;height:auto;display:block" fill="none" stroke-linecap="round">'
		+ '<circle cx="' + cx + '" cy="' + cy + '" r="9" fill="#f2c14e" stroke="#c98a1b" stroke-width="1.5"/>'
		+ '<circle cx="' + cx + '" cy="' + cy + '" r="' + k + '" stroke="' + pen + '" stroke-width="1.6"/>'
		+ '<circle cx="' + cx + '" cy="' + cy + '" r="' + (1.524 * k).toFixed(1) + '" stroke="' + pen + '" stroke-width="1.2" stroke-dasharray="2 5" stroke-opacity=".7"/>'
		+ '<ellipse cx="' + (cx - c).toFixed(1) + '" cy="' + cy + '" rx="' + a.toFixed(1) + '" ry="' + b.toFixed(1) + '" stroke="#b3342a" stroke-width="2.2"/>'
		+ '<circle cx="' + (cx + k) + '" cy="' + cy + '" r="5" fill="#9cc0e6" stroke="' + pen + '" stroke-width="1.4"/>'
		+ '<text x="' + (cx - 14) + '" y="' + (cy + 26) + '" font-family="Caveat" font-size="17" fill="#8a5a12" stroke="none">Sun</text>'
		+ '<text x="' + (cx + k + 8) + '" y="' + (cy + 4) + '" font-family="Caveat" font-size="17" fill="' + pen + '" stroke="none">Earth</text>'
		+ '<text x="6" y="22" font-family="Caveat" font-size="17" fill="' + pen + '" stroke="none">Mars\'s orbit</text>'
		+ '<text x="190" y="206" font-family="Caveat" font-size="18" fill="#b3342a" stroke="none">Luna 1: about 450 days</text>'
		+ '<text x="6" y="222" font-family="Caveat" font-size="16" fill="' + pen + '" stroke="none">roughly to scale</text></svg>';
}

export function sketchSvg(kind: string): string
{
	if (kind === 'lunapath')
	{
		return lunaPathSvg();
	}
	if (kind === 'lunasun')
	{
		return lunaSunSvg();
	}
	if (kind === 'sputnikorbit')
	{
		return sputnikOrbitSvg();
	}
	return spinSketchSvg();
}
