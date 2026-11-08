// "Figure 2" of the second law: acceleration against force for boxes of 4, 8 and 16 kg.
// Moving the force puts a dot on each line; after the first move the explanation opens.
import { useState } from 'react';
import { Axes, COLOURS, fmt, linePath, scaleOf } from './drawing';
import type { AxesSettings } from './drawing';
import { fill } from '../../i18n/fill';
import type { TextKey } from '../../i18n/t';

// The texts it shows. The page passes them in (texts() in i18n/t.ts), so the browser never gets the whole text file.
export const FORCE_GRAPH_TEXTS = [
	'figure.number',
	'graph.acceleration',
	'graph.force',
	'graph.forceControl',
	'graph.forceLabel',
	'graph.forceReading'
] as const satisfies readonly TextKey[];
type GraphText = (typeof FORCE_GRAPH_TEXTS)[number];

const MASSES = [
	{ kg: 4, colour: COLOURS.teal },
	{ kg: 8, colour: COLOURS.coral },
	{ kg: 16, colour: COLOURS.ochre }
];
const START_FORCE = 40;

interface Props
{
	caption: string;
	// HTML shown under the graph once the reader has moved the force
	after: string;
	texts: Record<GraphText, string>;
}

export default function ForceGraph(props: Props)
{
	const t = (key: GraphText) => props.texts[key];
	const [force, setForce] = useState(START_FORCE);
	const [moved, setMoved] = useState(false);

	const o: AxesSettings = { x0: 60, x1: 680, y0: 280, y1: 30, xa: 0, xb: 100, xn: 5, xd: 0, ya: 0, yb: 25, yn: 5, yd: 0, xLabel: t('graph.force'), yLabel: t('graph.acceleration') };
	const scale = scaleOf(o);
	const parts: string[] = [];
	for (const m of MASSES)
	{
		parts.push(m.kg + ' kg: ' + fmt(force / m.kg, 2) + ' m/s²');
	}

	let afterClass = 'after';
	if (moved)
	{
		afterClass = 'after on';
	}

	return (
		<figure>
			<div className="stage">
				<svg viewBox="0 0 760 330" aria-label={t('graph.forceLabel')}>
					<Axes o={o} />
					{MASSES.map((m) => (
						<g key={m.kg}>
							<path className="gline" d={linePath([[0, 0], [100, 100 / m.kg]], scale)} stroke={m.colour} strokeWidth={2.5} />
							<text x={scale.X(100) + 8} y={scale.Y(100 / m.kg) + 5} fill={m.colour}>{m.kg} kg</text>
						</g>
					))}
					<line x1={scale.X(force)} x2={scale.X(force)} y1={o.y1} y2={o.y0} stroke={COLOURS.axis} strokeDasharray="4 5" />
					{MASSES.map((m) =>
					{
						const a = force / m.kg;
						let decimals = 1;
						if (a < 10)
						{
							decimals = 2;
						}
						return (
							<g key={'dot' + m.kg}>
								<circle cx={scale.X(force)} cy={scale.Y(a)} r={6} fill={m.colour} stroke={COLOURS.paper} strokeWidth={2.5} />
								<text x={scale.X(force) - 10} y={scale.Y(a) - 9} fill={m.colour} textAnchor="end">{fmt(a, decimals)}</text>
							</g>
						);
					})}
				</svg>
				<div className="ctrls">
					<span>{t('graph.forceControl')}</span>
					<input
						type="range"
						name="gf"
						min={0}
						max={100}
						step={1}
						value={force}
						aria-label={t('graph.forceControl')}
						onChange={(e) =>
						{
							const value = Number(e.target.value);
							setForce(value);
							if (value !== START_FORCE)
							{
								setMoved(true);
							}
						}}
					/>
				</div>
				<p className="gread">{fill(t('graph.forceReading'), { f: String(force), parts: parts.join('   ') })}</p>
			</div>
			<div className={afterClass}><div dangerouslySetInnerHTML={{ __html: props.after }} /></div>
			<figcaption><b>{fill(t('figure.number'), { n: '2' })}</b> {props.caption}</figcaption>
		</figure>
	);
}
