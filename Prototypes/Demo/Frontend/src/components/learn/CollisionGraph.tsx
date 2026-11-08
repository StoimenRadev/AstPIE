// "Figure 2" of the third law: two boxes collide, and the force on each is drawn at every instant.
// The collision is worked out step by step with Velocity Verlet, like the simulations.
import { useEffect, useMemo, useRef, useState } from 'react';
import { Axes, Box, COLOURS, fmt, linePath, scaleOf } from './drawing';
import type { AxesSettings } from './drawing';
import { fill } from '../../i18n/fill';
import type { TextKey } from '../../i18n/t';

// The texts it shows. The page passes them in (texts() in i18n/t.ts), so the browser never gets the whole text file.
export const COLLISION_GRAPH_TEXTS = [
	'figure.number',
	'graph.boxesLabel',
	'graph.collisionLabel',
	'graph.collisionReading',
	'graph.equalMasses',
	'graph.forceAxis',
	'graph.forceOnA',
	'graph.forceOnB',
	'graph.heavierB',
	'graph.play',
	'graph.time'
] as const satisfies readonly TextKey[];
type GraphText = (typeof COLLISION_GRAPH_TEXTS)[number];

// Width of a box (m), stiffness of the contact (N per m of overlap), time step and length of the run (s)
const WIDTH = .8;
const STIFFNESS = 120;
const DT = .001;
const END = 2.4;

// One row per time step: time, positions of A and B, speeds of A and B, force on A, force on B
type Row = number[];

function simulate(massA: number, massB: number): Row[]
{
	let xa = -1.6;
	let xb = 1.0;
	let va = 1.2;
	let vb = -.4;
	const force = (a: number, b: number) => Math.max(0, (a + WIDTH / 2) - (b - WIDTH / 2)) * STIFFNESS;
	let F = force(xa, xb);
	let aa = -F / massA;
	let ab = F / massB;
	const rows: Row[] = [];
	for (let time = 0; time <= END + 1e-9; time += DT)
	{
		rows.push([time, xa, xb, va, vb, -F, F]);
		xa += va * DT + .5 * aa * DT * DT;
		xb += vb * DT + .5 * ab * DT * DT;
		F = force(xa, xb);
		const na = -F / massA;
		const nb = F / massB;
		va += .5 * (aa + na) * DT;
		vb += .5 * (ab + nb) * DT;
		aa = na;
		ab = nb;
	}
	return rows;
}

interface Props
{
	caption: string;
	after: string;
	texts: Record<GraphText, string>;
}

export default function CollisionGraph(props: Props)
{
	const t = (key: GraphText) => props.texts[key];
	const [heavy, setHeavy] = useState(false);
	const [time, setTime] = useState(0);
	const [seenEnd, setSeenEnd] = useState(false);
	const frame = useRef(0);

	let massB = 1;
	if (heavy)
	{
		massB = 3;
	}
	const data = useMemo(() => simulate(1, massB), [massB]);

	// The force axis fits the biggest force of the run
	let biggest = 0;
	for (const row of data)
	{
		biggest = Math.max(biggest, row[6]);
	}
	const top = Math.ceil(biggest * 1.2 / 5) * 5;
	const o: AxesSettings = { x0: 64, x1: 730, y0: 240, y1: 28, xa: 0, xb: END, xn: 6, xd: 1, ya: -top, yb: top, yn: 4, yd: 0, xLabel: t('graph.time'), yLabel: t('graph.forceAxis') };
	const scale = scaleOf(o);
	const onA: number[][] = [];
	const onB: number[][] = [];
	for (let i = 0; i < data.length; i += 10)
	{
		onA.push([data[i][0], data[i][5]]);
		onB.push([data[i][0], data[i][6]]);
	}

	const row = data[Math.min(data.length - 1, Math.round(time / DT))];
	const X = (x: number) => 380 + x * 105;
	const heightA = 30 + 1 * 10;
	const heightB = 30 + massB * 10;

	useEffect(() =>
	{
		if (time > 1.4)
		{
			setSeenEnd(true);
		}
	}, [time]);

	useEffect(() =>
	{
		return () => cancelAnimationFrame(frame.current);
	}, []);

	function play()
	{
		cancelAnimationFrame(frame.current);
		const startedAt = performance.now();
		let from = time;
		if (time >= END - .01)
		{
			from = 0;
		}
		const step = (now: number) =>
		{
			const next = Math.min(END, from + (now - startedAt) / 1000 * .6);
			setTime(next);
			if (next < END)
			{
				frame.current = requestAnimationFrame(step);
			}
		};
		frame.current = requestAnimationFrame(step);
	}

	let afterClass = 'after';
	if (seenEnd)
	{
		afterClass = 'after on';
	}
	let equalClass = 'btn picked';
	let heavyClass = 'btn';
	if (heavy)
	{
		equalClass = 'btn';
		heavyClass = 'btn picked';
	}

	const reading = fill(t('graph.collisionReading'), { t: fmt(row[0], 2), fa: fmt(row[5], 1), fb: fmt(row[6], 1), va: fmt(row[3], 2), vb: fmt(row[4], 2) });

	return (
		<figure>
			<div className="stage">
				<svg className="track" viewBox="0 0 760 100" aria-label={t('graph.boxesLabel')}>
					<line x1={20} x2={740} y1={90} y2={90} stroke={COLOURS.ground} strokeWidth={1.5} />
					<Box x={X(row[1] - WIDTH / 2)} y={90 - heightA} w={WIDTH * 105} h={heightA} label="A" />
					<Box x={X(row[2] - WIDTH / 2)} y={90 - heightB} w={WIDTH * 105} h={heightB} label="B" />
				</svg>
				<svg className="forces" viewBox="0 0 760 290" aria-label={t('graph.collisionLabel')}>
					<Axes o={o} />
					<line x1={o.x0} x2={o.x1} y1={scale.Y(0)} y2={scale.Y(0)} stroke={COLOURS.axis} />
					<path className="gline" d={linePath(onB, scale)} stroke={COLOURS.teal} strokeWidth={2.5} />
					<path className="gline" d={linePath(onA, scale)} stroke={COLOURS.coral} strokeWidth={2.5} />
					<text x={o.x1} y={scale.Y(top * .78)} textAnchor="end" fill={COLOURS.teal}>{t('graph.forceOnB')}</text>
					<text x={o.x1} y={scale.Y(-top * .78)} textAnchor="end" fill={COLOURS.coral}>{t('graph.forceOnA')}</text>
					<line x1={scale.X(row[0])} x2={scale.X(row[0])} y1={o.y1} y2={o.y0} stroke={COLOURS.axis} />
					<circle cx={scale.X(row[0])} cy={scale.Y(row[6])} r={5} fill={COLOURS.teal} stroke={COLOURS.paper} strokeWidth={2.5} />
					<circle cx={scale.X(row[0])} cy={scale.Y(row[5])} r={5} fill={COLOURS.coral} stroke={COLOURS.paper} strokeWidth={2.5} />
				</svg>
				<div className="ctrls">
					<button className="btn" type="button" onClick={play}>{t('graph.play')}</button>
					<input
						type="range"
						name="gt"
						min={0}
						max={END}
						step={.005}
						value={time}
						aria-label={t('graph.time')}
						onChange={(e) => setTime(Number(e.target.value))}
					/>
				</div>
				<div className="ctrls">
					<button className={equalClass} type="button" onClick={() => setHeavy(false)}>{t('graph.equalMasses')}</button>
					<button className={heavyClass} type="button" onClick={() => setHeavy(true)}>{t('graph.heavierB')}</button>
				</div>
				<p className="gread">{reading}</p>
			</div>
			<div className={afterClass}><div dangerouslySetInnerHTML={{ __html: props.after }} /></div>
			<figcaption><b>{fill(t('figure.number'), { n: '2' })}</b> {props.caption}</figcaption>
		</figure>
	);
}
