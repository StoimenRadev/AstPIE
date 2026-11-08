// "Figure 1" of a topic: the law as a small drawing you can change with sliders.
// first: two pushes on one box; second: a force on a box of some mass; third: two boxes pushing apart.
import { useState } from 'react';
import type { ReactElement } from 'react';
import { Arrow, Box, COLOURS, Ground, fmt, signed } from './drawing';
import { fill } from '../../i18n/fill';
import type { TextKey } from '../../i18n/t';

// The texts it shows. The page passes them in (texts() in i18n/t.ts), so the browser never gets the whole text file.
export const LAW_FIGURE_TEXTS = [
	'figure.firstCancel',
	'figure.firstLeftOver',
	'figure.firstSum',
	'figure.left',
	'figure.netLabel',
	'figure.netZero',
	'figure.number',
	'figure.pushOnA',
	'figure.pushOnB',
	'figure.right',
	'figure.secondSum',
	'figure.secondText',
	'figure.thirdHeavier',
	'figure.thirdSame',
	'figure.thirdSum',
	'figure.thirdText',
	'figure.firstPush',
	'figure.secondPush',
	'figure.netForce',
	'figure.mass',
	'figure.pushBetween',
	'figure.massA',
	'figure.massB'
] as const satisfies readonly TextKey[];
type FigureText = (typeof LAW_FIGURE_TEXTS)[number];
// Looks up one of those texts
type Say = (key: FigureText) => string;

interface SliderSetting
{
	name: string;
	label: FigureText;
	min: number;
	max: number;
	step: number;
	start: number;
	unit: string;
}

const SLIDERS: Record<string, SliderSetting[]> = {
	first: [
		{ name: 'f1', label: 'figure.firstPush', min: -40, max: 40, step: 5, start: 30, unit: 'N' },
		{ name: 'f2', label: 'figure.secondPush', min: -40, max: 40, step: 5, start: -30, unit: 'N' }
	],
	second: [
		{ name: 'f', label: 'figure.netForce', min: 0, max: 100, step: 5, start: 40, unit: 'N' },
		{ name: 'm', label: 'figure.mass', min: 1, max: 20, step: 1, start: 4, unit: 'kg' }
	],
	third: [
		{ name: 'p', label: 'figure.pushBetween', min: 0, max: 60, step: 2, start: 24, unit: 'N' },
		{ name: 'ma', label: 'figure.massA', min: 1, max: 10, step: 1, start: 2, unit: 'kg' },
		{ name: 'mb', label: 'figure.massB', min: 1, max: 10, step: 1, start: 6, unit: 'kg' }
	]
};

const VIEWS: Record<string, string> = { first: '0 80 640 136', second: '0 0 640 180', third: '0 -4 640 222' };

interface Drawing
{
	shapes: ReactElement;
	// HTML: the sum in symbols, then a sentence
	readout: string;
}

function drawFirst(v: Record<string, number>, t: Say): Drawing
{
	const a = v.f1;
	const b = v.f2;
	const n = a + b;
	let aSide = -28;
	if (a > 0)
	{
		aSide = 28;
	}
	let bSide = -28;
	if (b > 0)
	{
		bSide = 28;
	}
	let net = <text x={320} y={184} textAnchor="middle" fill={COLOURS.ink} fontSize={15}>{t('figure.netZero')}</text>;
	if (n !== 0)
	{
		net = <Arrow x1={320} y={178} x2={320 + n * 5} colour={COLOURS.ink} width={4} label={fill(t('figure.netLabel'), { n: signed(n) })} labelX={320 + n * 2.5} labelY={200} />;
	}
	const shapes = (
		<g>
			<Ground y={150} />
			<Box x={280} y={96} w={80} h={54} label="" />
			<Arrow x1={320} y={110} x2={320 + a * 5} colour={COLOURS.teal} width={3.5} label={signed(a) + ' N'} labelX={320 + a * 5 + aSide} labelY={115} />
			<Arrow x1={320} y={136} x2={320 + b * 5} colour={COLOURS.coral} width={3.5} label={signed(b) + ' N'} labelX={320 + b * 5 + bSide} labelY={141} />
			{net}
		</g>
	);
	let sentence = t('figure.firstCancel');
	if (n !== 0)
	{
		let side = t('figure.left');
		if (n > 0)
		{
			side = t('figure.right');
		}
		sentence = fill(t('figure.firstLeftOver'), { n: String(Math.abs(n)), side: side });
	}
	return { shapes: shapes, readout: fill(t('figure.firstSum'), { a: fmt(a, 0), b: fmt(b, 0), n: fmt(n, 0) }) + '<br>' + sentence };
}

function drawSecond(v: Record<string, number>, t: Say): Drawing
{
	const F = v.f;
	const M = v.m;
	const A = F / M;
	const side = 42 + M * 4;
	const x = 300;
	const length = Math.min(240, A * 9);
	let decimals = 1;
	if (A < 10)
	{
		decimals = 2;
	}
	const shapes = (
		<g>
			<Ground y={160} />
			<Box x={x} y={160 - side} w={side} h={side} label={M + ' kg'} />
			<Arrow x1={x - 10 - F * 2.3} y={160 - side / 2} x2={x - 8} colour={COLOURS.coral} width={4} label={F + ' N'} labelX={x - 10 - F * 1.15} labelY={160 - side / 2 - 12} />
			<Arrow x1={x + side / 2 - length / 2} y={160 - side - 20} x2={x + side / 2 + length / 2} colour={COLOURS.teal} width={3.5} label={'a = ' + fmt(A, decimals) + ' m/s²'} labelX={x + side / 2} labelY={160 - side - 32} />
		</g>
	);
	const readout = fill(t('figure.secondSum'), { f: String(F), m: String(M), a: fmt(A, 2) }) + '<br>' + fill(t('figure.secondText'), { a: fmt(A, 2), v: fmt(A * 3, 1) });
	return { shapes: shapes, readout: readout };
}

function drawThird(v: Record<string, number>, t: Say): Drawing
{
	const P = v.p;
	const MA = v.ma;
	const MB = v.mb;
	const sa = 40 + MA * 7;
	const sb = 40 + MB * 7;
	const length = P * 2.4;
	const aa = P / MA;
	const ab = P / MB;
	const ay = 160 - Math.max(sa, sb) - 20;
	const la = Math.min(230, aa * 7);
	const lb = Math.min(230, ab * 7);
	const shapes = (
		<g>
			<Ground y={160} />
			<Box x={320 - sa} y={160 - sa} w={sa} h={sa} label={'A · ' + MA + ' kg'} />
			<Box x={320} y={160 - sb} w={sb} h={sb} label={'B · ' + MB + ' kg'} />
			<Arrow x1={320 - sa - 6} y={180} x2={320 - sa - 6 - length} colour={COLOURS.coral} width={3.5} label={fill(t('figure.pushOnA'), { p: String(P) })} labelX={320 - sa - 6 - length / 2} labelY={204} />
			<Arrow x1={320 + sb + 6} y={180} x2={320 + sb + 6 + length} colour={COLOURS.coral} width={3.5} label={fill(t('figure.pushOnB'), { p: String(P) })} labelX={320 + sb + 6 + length / 2} labelY={204} />
			<Arrow x1={320 - sa / 2} y={ay} x2={320 - sa / 2 - la} colour={COLOURS.teal} width={3} label={'a = ' + fmt(aa, 1) + ' m/s²'} labelX={320 - sa / 2 - la / 2} labelY={ay - 10} />
			<Arrow x1={320 + sb / 2} y={ay} x2={320 + sb / 2 + lb} colour={COLOURS.teal} width={3} label={'a = ' + fmt(ab, 1) + ' m/s²'} labelX={320 + sb / 2 + lb / 2} labelY={ay - 10} />
		</g>
	);
	let ending = t('figure.thirdSame');
	if (MA !== MB)
	{
		let heavier = 'A';
		if (MA < MB)
		{
			heavier = 'B';
		}
		const times = fmt(Math.max(MA, MB) / Math.min(MA, MB), 1).replace('.0', '');
		ending = fill(t('figure.thirdHeavier'), { box: heavier, times: times });
	}
	const readout = fill(t('figure.thirdSum'), { p: String(P) }) + '<br>' + fill(t('figure.thirdText'), { p: String(P), a: fmt(aa, 1), b: fmt(ab, 1) }) + ending;
	return { shapes: shapes, readout: readout };
}

interface Props
{
	// first, second or third
	kind: string;
	caption: string;
	texts: Record<FigureText, string>;
}

export default function LawFigure(props: Props)
{
	const t: Say = (key) => props.texts[key];
	const sliders = SLIDERS[props.kind];
	const start: Record<string, number> = {};
	for (const s of sliders)
	{
		start[s.name] = s.start;
	}
	const [values, setValues] = useState(start);

	let drawing: Drawing;
	if (props.kind === 'second')
	{
		drawing = drawSecond(values, t);
	}
	else if (props.kind === 'third')
	{
		drawing = drawThird(values, t);
	}
	else
	{
		drawing = drawFirst(values, t);
	}

	return (
		<figure>
			<div className="stage">
				<svg viewBox={VIEWS[props.kind]} aria-hidden="true">{drawing.shapes}</svg>
				<div className="sliders">
					{sliders.map((s) => (
						<div className="slider" key={s.name}>
							<label htmlFor={s.name}>{t(s.label)} <output htmlFor={s.name}>{fmt(values[s.name], 0) + ' ' + s.unit}</output></label>
							<input
								type="range"
								id={s.name}
								name={s.name}
								min={s.min}
								max={s.max}
								step={s.step}
								value={values[s.name]}
								onChange={(e) =>
								{
									const next = Object.assign({}, values);
									next[s.name] = Number(e.target.value);
									setValues(next);
								}}
							/>
						</div>
					))}
				</div>
				<p className="readout" dangerouslySetInnerHTML={{ __html: drawing.readout }} />
			</div>
			<figcaption><b>{fill(t('figure.number'), { n: '1' })}</b> {props.caption}</figcaption>
		</figure>
	);
}
