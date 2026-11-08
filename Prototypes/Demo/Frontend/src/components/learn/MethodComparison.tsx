// "Velocity Verlet or semi-implicit Euler?": the test from the Octave prototypes, run on the page.
// A 2 kg box pushed by 20 N from rest for 10 s; pick the time step and see how far off each method is.
import { useState } from 'react';
import { Axes, COLOURS, fmt, linePath, scaleOf } from './drawing';
import type { AxesSettings } from './drawing';
import { fill } from '../../i18n/fill';
import type { TextKey } from '../../i18n/t';

// The texts it shows. The page passes them in (texts() in i18n/t.ts), so the browser never gets the whole text file.
export const METHOD_COMPARISON_TEXTS = [
	'compare.errorAxis',
	'compare.euler',
	'compare.eulerShort',
	'compare.exact',
	'compare.graphLabel',
	'compare.result',
	'compare.setUp',
	'compare.time',
	'compare.verlet',
	'compare.verletShort',
	'compare.why',
	'graph.time'
] as const satisfies readonly TextKey[];
type CompareText = (typeof METHOD_COMPARISON_TEXTS)[number];

const STEPS = [0.01, 0.05, 0.1, 0.25, 0.5, 1];
const FORCE = 20;
const MASS = 2;
const DURATION = 10;

interface Run
{
	euler: number[][];
	verlet: number[][];
	// Rows of the table: time, exact position, Euler, Verlet
	rows: number[][];
	eulerError: number;
	verletWorst: number;
}

function run(dt: number): Run
{
	const a = FORCE / MASS;
	const n = Math.round(DURATION / dt);
	let xe = 0;
	let ve = 0;
	let xv = 0;
	let vv = 0;
	let worst = 0;
	const euler = [[0, 0]];
	const verlet = [[0, 0]];
	const rows: number[][] = [];
	const every = Math.max(1, Math.round(n / 10));
	for (let i = 1; i <= n; i++)
	{
		// Semi-implicit Euler: the speed first, then the position with the new speed
		ve += a * dt;
		xe += ve * dt;
		// Velocity Verlet with a constant force
		xv += vv * dt + .5 * a * dt * dt;
		vv += a * dt;
		const time = i * dt;
		const exact = .5 * a * time * time;
		euler.push([time, xe - exact]);
		verlet.push([time, xv - exact]);
		worst = Math.max(worst, Math.abs(xv - exact));
		if (i % every === 0)
		{
			rows.push([time, exact, xe, xv]);
		}
	}
	return { euler: euler, verlet: verlet, rows: rows, eulerError: xe - .5 * a * DURATION * DURATION, verletWorst: worst };
}

// A round top for the error axis: 1, 2, 2.5 or 5 times a power of ten
function roundTop(value: number): number
{
	const raw = Math.max(.01, value) * 1.08;
	const power = Math.pow(10, Math.floor(Math.log10(raw)));
	for (const k of [1, 2, 2.5, 5, 10])
	{
		if (k * power >= raw)
		{
			return k * power;
		}
	}
	return 10 * power;
}

interface Props
{
	texts: Record<CompareText, string>;
}

export default function MethodComparison(props: Props)
{
	const t = (key: CompareText) => props.texts[key];
	const [dt, setDt] = useState(.5);
	const result = run(dt);
	const top = roundTop(result.eulerError);
	let yDecimals = 0;
	if (top < 10)
	{
		yDecimals = 1;
	}
	if (top < 1)
	{
		yDecimals = 2;
	}
	const o: AxesSettings = { x0: 56, x1: 440, y0: 230, y1: 30, xa: 0, xb: DURATION, xn: 5, xd: 0, ya: 0, yb: top, yn: 5, yd: yDecimals, xLabel: t('graph.time'), yLabel: t('compare.errorAxis') };
	const scale = scaleOf(o);
	let timeDecimals = 1;
	if (dt < .1)
	{
		timeDecimals = 2;
	}
	let verletOff = '0 m';
	if (result.verletWorst !== 0)
	{
		verletOff = result.verletWorst.toExponential(1) + ' m';
	}

	return (
		<div className="comparison">
			<div className="methods">
				<div><b className="c-coral">{t('compare.euler')}</b><code>v ← v + a·dt<br />x ← x + v·dt</code></div>
				<div><b className="c-teal">{t('compare.verlet')}</b><code>x ← x + v·dt + ½·a·dt²<br />v ← v + ½·(a + a_new)·dt</code></div>
			</div>
			<div className="dtrow">
				{t('compare.setUp')}
				{STEPS.map((step) =>
				{
					let className = '';
					if (step === dt)
					{
						className = 'on';
					}
					return <button key={step} type="button" className={className} onClick={() => setDt(step)}>{step} s</button>;
				})}
			</div>
			<div className="cmpgrid">
				<figure className="cmp" style={{ margin: 0 }}>
					<div className="stage">
						<svg viewBox="0 0 470 280" aria-label={t('compare.graphLabel')}>
							<Axes o={o} />
							<path className="gline" d={linePath(result.euler, scale)} stroke={COLOURS.coral} strokeWidth={2.5} />
							<path className="gline" d={linePath(result.verlet, scale)} stroke={COLOURS.teal} strokeWidth={2.5} />
						</svg>
					</div>
				</figure>
				<table className="steps">
					<thead><tr><th>{t('compare.time')}</th><th>{t('compare.exact')}</th><th>{t('compare.eulerShort')}</th><th>{t('compare.verletShort')}</th></tr></thead>
					<tbody>
						{result.rows.map((r) => (
							<tr key={r[0]}><td>{fmt(r[0], timeDecimals)}</td><td>{fmt(r[1], 2)}</td><td className="e">{fmt(r[2], 2)}</td><td className="v">{fmt(r[3], 2)}</td></tr>
						))}
					</tbody>
				</table>
			</div>
			<p className="result" dangerouslySetInnerHTML={{ __html: fill(t('compare.result'), { dt: String(dt), err: fmt(result.eulerError, 3), off: verletOff }) }} />
			<p>{t('compare.why')}</p>
		</div>
	);
}
