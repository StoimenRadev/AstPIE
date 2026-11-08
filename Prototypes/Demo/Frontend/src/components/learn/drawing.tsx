// What the Learn figures and graphs are drawn with: plain shapes and lines worked out from the numbers.
import type { ReactElement } from 'react';

export const COLOURS = {
	box: '#ffffff',
	edge: '#23211d',
	teal: '#2c7480',
	coral: '#c4573a',
	ochre: '#a8741a',
	ink: '#23211d',
	ground: '#b9b1a0',
	grid: 'rgba(35,33,29,.07)',
	axis: 'rgba(35,33,29,.45)',
	paper: '#f2eee5',
	soft: '#8c8579'
};

// A number with a fixed count of decimals and a real minus sign; -0 becomes 0
export function fmt(n: number, decimals: number): string
{
	let value = n;
	if (Math.abs(value) < 1e-9)
	{
		value = 0;
	}
	return value.toFixed(decimals).replace('-', '−');
}

// "+30" for positive numbers, "-30" for negative ones
export function signed(n: number): string
{
	if (n > 0)
	{
		return '+' + n;
	}
	return String(n);
}

interface ArrowProps
{
	x1: number;
	y: number;
	x2: number;
	colour: string;
	width: number;
	label: string;
	labelX: number;
	labelY: number;
}

// A straight arrow from x1 to x2 with a label; nothing when it is too short to see
export function Arrow(props: ArrowProps): ReactElement | null
{
	if (Math.abs(props.x2 - props.x1) < 3)
	{
		return null;
	}
	const dir = Math.sign(props.x2 - props.x1);
	const head = Math.min(12, Math.abs(props.x2 - props.x1));
	const tip = 'M' + props.x2 + ' ' + props.y + ' L' + (props.x2 - dir * head) + ' ' + (props.y - head * .5) + ' L' + (props.x2 - dir * head) + ' ' + (props.y + head * .5) + ' Z';
	return (
		<g>
			<line x1={props.x1} y1={props.y} x2={props.x2 - dir * head * .7} y2={props.y} stroke={props.colour} strokeWidth={props.width} strokeLinecap="round" />
			<path d={tip} fill={props.colour} />
			<text x={props.labelX} y={props.labelY} textAnchor="middle" fill={props.colour} fontSize={15} fontWeight={700}>{props.label}</text>
		</g>
	);
}

interface BoxProps
{
	x: number;
	y: number;
	w: number;
	h: number;
	label: string;
}

// A white box with a thin edge, and its name in the middle
export function Box(props: BoxProps): ReactElement
{
	return (
		<g>
			<rect x={props.x} y={props.y} width={props.w} height={props.h} rx={3} fill={COLOURS.box} stroke={COLOURS.edge} strokeWidth={1.5} />
			<text x={props.x + props.w / 2} y={props.y + props.h / 2 + 5} textAnchor="middle" fill={COLOURS.ink} fontSize={15}>{props.label}</text>
		</g>
	);
}

export function Ground(props: { y: number }): ReactElement
{
	return <line x1={20} y1={props.y} x2={620} y2={props.y} stroke={COLOURS.ground} strokeWidth={1.5} />;
}

// ---------------------------------------------------------------- graphs

export interface AxesSettings
{
	// Where the graph sits in the drawing
	x0: number;
	x1: number;
	y0: number;
	y1: number;
	// The values at the ends of each axis, how many steps between them, and decimals on the labels
	xa: number;
	xb: number;
	xn: number;
	xd: number;
	ya: number;
	yb: number;
	yn: number;
	yd: number;
	xLabel: string;
	yLabel: string;
}

// From values to positions in the drawing
export interface Scale
{
	X: (value: number) => number;
	Y: (value: number) => number;
}

export function scaleOf(o: AxesSettings): Scale
{
	return {
		X: (v) => o.x0 + (v - o.xa) / (o.xb - o.xa) * (o.x1 - o.x0),
		Y: (v) => o.y0 - (v - o.ya) / (o.yb - o.ya) * (o.y0 - o.y1)
	};
}

// The grid, the numbers on both axes and their names
export function Axes(props: { o: AxesSettings }): ReactElement
{
	const o = props.o;
	const columns: ReactElement[] = [];
	for (let i = 0; i <= o.xn; i++)
	{
		const x = o.x0 + (o.x1 - o.x0) * i / o.xn;
		columns.push(
			<g key={'x' + i}>
				<line x1={x} y1={o.y1} x2={x} y2={o.y0} stroke={COLOURS.grid} />
				<text x={x} y={o.y0 + 20} textAnchor="middle">{fmt(o.xa + (o.xb - o.xa) * i / o.xn, o.xd)}</text>
			</g>
		);
	}
	const rows: ReactElement[] = [];
	for (let j = 0; j <= o.yn; j++)
	{
		const y = o.y0 - (o.y0 - o.y1) * j / o.yn;
		rows.push(
			<g key={'y' + j}>
				<line x1={o.x0} y1={y} x2={o.x1} y2={y} stroke={COLOURS.grid} />
				<text x={o.x0 - 10} y={y + 5} textAnchor="end">{fmt(o.ya + (o.yb - o.ya) * j / o.yn, o.yd)}</text>
			</g>
		);
	}
	return (
		<g>
			{columns}
			{rows}
			<line x1={o.x0} y1={o.y0} x2={o.x1} y2={o.y0} stroke={COLOURS.axis} strokeWidth={1.2} />
			<line x1={o.x0} y1={o.y0} x2={o.x0} y2={o.y1} stroke={COLOURS.axis} strokeWidth={1.2} />
			<text x={o.x1} y={o.y0 + 40} textAnchor="end">{o.xLabel}</text>
			<text x={o.x0} y={o.y1 - 12}>{o.yLabel}</text>
		</g>
	);
}

// A line through points given as values
export function linePath(points: number[][], scale: Scale): string
{
	return points.map((p, i) =>
	{
		let move = 'L';
		if (i === 0)
		{
			move = 'M';
		}
		return move + scale.X(p[0]).toFixed(1) + ' ' + scale.Y(p[1]).toFixed(1);
	}).join(' ');
}
