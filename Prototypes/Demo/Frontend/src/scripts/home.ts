// The home page in motion: the sky changes colour from part to part, big objects and stars
// move at their own depth as you scroll, comets cross between the parts, asteroids tumble,
// and a canvas draws a twinkling starfield with the odd shooting star.

type Colour = number[];

interface SkyKey
{
	bg: Colour;
	n1: Colour;
	n2: Colour;
}

interface Mover
{
	el: HTMLElement;
	y: number;
	base: number;
	sec: HTMLElement | null;
	top: number;
	bottom: number;
	first: boolean;
}

interface Rock
{
	el: HTMLElement;
	r: number;
	sx: number;
	base: number;
}

interface Comet
{
	el: HTMLElement;
	from: number[];
	to: number[];
	base: number;
}

interface Star
{
	x: number;
	y: number;
	z: number;
	s: number;
	ph: number;
	sp: number;
	c: Colour;
	// A tiny four-point diamond instead of a dot
	d: boolean;
	// Time left of a flash
	g: number;
}

interface Shot
{
	x: number;
	y: number;
	vx: number;
	vy: number;
	life: number;
}

const reduce = window.matchMedia('(prefers-reduced-motion: reduce)').matches;
const root = document.documentElement;

function all(selector: string): HTMLElement[]
{
	return Array.from(document.querySelectorAll<HTMLElement>(selector));
}

function one(selector: string): HTMLElement
{
	return document.querySelector(selector) as HTMLElement;
}

function hex(c: string): Colour
{
	return [parseInt(c.slice(1, 3), 16), parseInt(c.slice(3, 5), 16), parseInt(c.slice(5, 7), 16)];
}

function smooth(t: number): number
{
	const k = Math.max(0, Math.min(1, t));
	return k * k * (3 - 2 * k);
}

function mix(a: Colour, b: Colour, t: number): Colour
{
	return a.map((v, i) => v + (b[i] - v) * t);
}

function rgba(c: Colour, alpha: number): string
{
	return 'rgba(' + Math.round(c[0]) + ',' + Math.round(c[1]) + ',' + Math.round(c[2]) + ',' + alpha + ')';
}

function numbers(text: string): number[]
{
	return text.split(',').map(Number);
}

// ---------------------------------------------------------------- comets: a bright head, a pale gas tail and a warm dust tail
function cometSvg(gas: string, dust: string): string
{
	const id = gas.slice(1) + dust.slice(1);
	return '<svg viewBox="0 0 420 80" aria-hidden="true"><defs>'
		+ '<linearGradient id="g' + id + '" x1="0" x2="1"><stop offset="0" stop-color="' + gas + '" stop-opacity="0"/><stop offset="1" stop-color="' + gas + '"/></linearGradient>'
		+ '<linearGradient id="d' + id + '" x1="0" x2="1"><stop offset="0" stop-color="' + dust + '" stop-opacity="0"/><stop offset="1" stop-color="' + dust + '"/></linearGradient></defs>'
		+ '<path d="M0 26 Q 220 32 396 34 L396 46 Q 220 54 10 70 Z" fill="url(#d' + id + ')" opacity=".35"/>'
		+ '<path d="M60 34 Q 240 37 396 38 L396 43 Q 240 47 80 56 Z" fill="url(#d' + id + ')" opacity=".55"/>'
		+ '<path d="M0 40 L392 33 A8 8 0 0 1 392 47 Z" fill="url(#g' + id + ')" opacity=".85"/>'
		+ '<path d="M140 40 L390 40" stroke="#fffaf0" stroke-width="2" stroke-dasharray="22 14" stroke-linecap="round" opacity=".6"/>'
		+ '<circle cx="398" cy="40" r="18" fill="' + gas + '" opacity=".18"/>'
		+ '<circle cx="398" cy="40" r="12" fill="' + gas + '" opacity=".4"/>'
		+ '<circle cx="398" cy="40" r="7.5" fill="#fffaf0"/></svg>';
}

// ---------------------------------------------------------------- asteroids: a base, a shaded side, a lit edge and craters
const ROCKS = [
	{ x: .05, y: -140, w: 110, col: ['#5d4d4a', '#4a3c3a', '#7a6660'], r: .05, sx: .5 },
	{ x: .19, y: 40, w: 46, col: ['#8a5a45', '#6d4535', '#a8735a'], r: -.12, sx: .9 },
	{ x: .32, y: -70, w: 170, col: ['#4f5568', '#3e4354', '#6a7188'], r: .035, sx: .32 },
	{ x: .5, y: 80, w: 60, col: ['#6e5a54', '#574642', '#8c766e'], r: -.08, sx: .7 },
	{ x: .61, y: -170, w: 34, col: ['#9a6a3a', '#7a5230', '#c08a4c'], r: .2, sx: 1.1 },
	{ x: .73, y: 0, w: 140, col: ['#5d4d4a', '#4a3c3a', '#7a6660'], r: -.05, sx: .42 },
	{ x: .89, y: -110, w: 74, col: ['#8a5a45', '#6d4535', '#a8735a'], r: .1, sx: .8 },
	{ x: .97, y: 120, w: 40, col: ['#4f5568', '#3e4354', '#6a7188'], r: -.15, sx: 1 }
];

function pathOf(points: number[][]): string
{
	return points.map((p, i) =>
	{
		let move = 'L';
		if (i === 0)
		{
			move = 'M';
		}
		return move + p[0].toFixed(1) + ' ' + p[1].toFixed(1);
	}).join(' ') + ' Z';
}

function rockSvg(seed: number, c: string[]): string
{
	let s = seed * 7919;
	const random = () =>
	{
		s = (s * 9301 + 49297) % 233280;
		return s / 233280;
	};
	const points: number[][] = [];
	const n = 10;
	for (let i = 0; i < n; i++)
	{
		const a = i / n * Math.PI * 2;
		const rr = 36 + random() * 10;
		points.push([50 + Math.cos(a) * rr, 50 + Math.sin(a) * rr * .84]);
	}
	const shade = [points[0], points[1], points[2], points[3], points[4], points[5], [50, 58]];
	const lit = [points[6], points[7], points[8], [52, 40]];
	// Two craters of different sizes, on a diagonal so they never look like a face
	const c1x = 36 + random() * 8;
	const c1y = 40 + random() * 8;
	const c1r = 6 + random() * 3;
	const c2x = 58 + random() * 8;
	const c2y = 56 + random() * 8;
	const c2r = 2.5 + random() * 1.5;
	const craters = '<circle cx="' + c1x.toFixed(1) + '" cy="' + c1y.toFixed(1) + '" r="' + c1r.toFixed(1) + '" fill="' + c[1] + '"/>'
		+ '<circle cx="' + (c1x + c1r * .25).toFixed(1) + '" cy="' + (c1y + c1r * .25).toFixed(1) + '" r="' + (c1r * .7).toFixed(1) + '" fill="#000" opacity=".12"/>'
		+ '<circle cx="' + c2x.toFixed(1) + '" cy="' + c2y.toFixed(1) + '" r="' + c2r.toFixed(1) + '" fill="' + c[1] + '"/>';
	return '<svg viewBox="0 0 100 100" aria-hidden="true"><path d="' + pathOf(points) + '" fill="' + c[0] + '" stroke="' + c[0] + '" stroke-width="7" stroke-linejoin="round"/>'
		+ '<path d="' + pathOf(shade) + '" fill="' + c[1] + '" stroke="' + c[1] + '" stroke-width="5" stroke-linejoin="round"/>'
		+ '<path d="' + pathOf(lit) + '" fill="' + c[2] + '" stroke="' + c[2] + '" stroke-width="4" stroke-linejoin="round" opacity=".9"/>' + craters + '</svg>';
}

export function startHome(): void
{
	one('#comet1').innerHTML = cometSvg('#cdeeea', '#e3b04b');
	one('#comet2').innerHTML = cometSvg('#f3e2b8', '#c7a6d9');

	const belt = one('#belt');
	ROCKS.forEach((r, i) =>
	{
		const el = document.createElement('div');
		el.className = 'rock';
		el.style.width = r.w + 'px';
		el.style.left = 'calc(' + (r.x * 100) + '% - ' + (r.w / 2) + 'px)';
		el.style.top = r.y + 'px';
		el.dataset.r = String(r.r);
		el.dataset.sx = String(r.sx);
		el.innerHTML = rockSvg(i + 5, r.col);
		belt.appendChild(el);
	});

	// ---------------------------------------------------------------- the journey: background colours and moving things
	const sections = all('main section');
	const keys: SkyKey[] = sections.map((s) =>
	{
		return { bg: hex(String(s.dataset.bg)), n1: numbers(String(s.dataset.n1)), n2: numbers(String(s.dataset.n2)) };
	});
	const movers: Mover[] = all('.obj, .flare').map((el) =>
	{
		const sec = el.closest('section');
		return { el: el, y: Number(el.dataset.y), base: 0, sec: sec, top: 0, bottom: 0, first: sec === sections[0] };
	});
	const rocks: Rock[] = all('#belt .rock').map((el) =>
	{
		return { el: el, r: Number(el.dataset.r), sx: Number(el.dataset.sx), base: 0 };
	});
	const comets: Comet[] = all('.comet').map((el) =>
	{
		return { el: el, from: numbers(String(el.dataset.from)), to: numbers(String(el.dataset.to)), base: 0 };
	});
	const heroText = one('.hero .text');
	const header = one('#top');
	let tops: number[] = [];

	function measure(): void
	{
		for (const o of movers)
		{
			o.el.style.transform = '';
		}
		for (const o of rocks)
		{
			o.el.style.transform = '';
		}
		const sy = window.scrollY;
		tops = sections.map((s) => s.getBoundingClientRect().top + sy);
		for (const o of movers)
		{
			const r = o.el.getBoundingClientRect();
			o.base = r.top + sy + r.height / 2;
			if (o.sec !== null)
			{
				const sr = o.sec.getBoundingClientRect();
				o.top = sr.top + sy;
				o.bottom = sr.bottom + sy;
			}
		}
		for (const o of rocks)
		{
			const r = o.el.getBoundingClientRect();
			o.base = r.top + sy + r.height / 2;
		}
		for (const c of comets)
		{
			const parent = c.el.parentElement as HTMLElement;
			c.base = parent.getBoundingClientRect().top + sy;
		}
		sizeSky();
	}

	let current: SkyKey | null = null;

	function frame(dt: number): void
	{
		const sy = window.scrollY;
		const vw = window.innerWidth;
		const vh = window.innerHeight;
		// Each part takes over the colour of the sky as it rises into view
		let goal: SkyKey = { bg: keys[0].bg, n1: keys[0].n1, n2: keys[0].n2 };
		for (let i = 1; i < sections.length; i++)
		{
			const t = smooth((sy - (tops[i] - vh * .85)) / (vh * .6));
			if (t <= 0)
			{
				break;
			}
			goal = { bg: mix(goal.bg, keys[i].bg, t), n1: mix(goal.n1, keys[i].n1, t), n2: mix(goal.n2, keys[i].n2, t) };
		}
		if (current === null || reduce)
		{
			current = goal;
		}
		else
		{
			const k = Math.min(1, dt * 4);
			current = { bg: mix(current.bg, goal.bg, k), n1: mix(current.n1, goal.n1, k), n2: mix(current.n2, goal.n2, k) };
		}
		root.style.setProperty('--bg', rgba(current.bg, 1));
		root.style.setProperty('--n1', rgba(current.n1, current.n1[3]));
		root.style.setProperty('--n2', rgba(current.n2, current.n2[3]));
		header.classList.toggle('solid', sy > 30);
		if (reduce)
		{
			return;
		}
		// Big objects and big stars lag behind the scroll at their own depth
		for (const o of movers)
		{
			const p = sy + vh / 2 - o.base;
			o.el.style.transform = 'translate3d(0,' + (p * o.y).toFixed(1) + 'px,0)';
			let fadeIn = 1;
			if (!o.first)
			{
				fadeIn = smooth((sy + vh - o.top - vh * .1) / (vh * .5));
			}
			const fadeOut = 1 - smooth((sy - (o.bottom - vh * .62)) / (vh * .42));
			o.el.style.opacity = (fadeIn * fadeOut).toFixed(3);
		}
		// The opening text drifts up and fades as you leave it
		const h = smooth(sy / (vh * .6));
		heroText.style.transform = 'translateY(' + (-h * 60).toFixed(1) + 'px)';
		heroText.style.opacity = (1 - h).toFixed(3);
		// Asteroids drift sideways and tumble
		for (const o of rocks)
		{
			const p = sy + vh / 2 - o.base;
			o.el.style.transform = 'translate3d(' + (p * o.sx * .35).toFixed(1) + 'px,' + (p * -.12 * o.sx).toFixed(1) + 'px,0) rotate(' + (p * o.r).toFixed(1) + 'deg)';
		}
		// Comets cross the screen while their stretch of the page passes
		for (const c of comets)
		{
			const t = (sy + vh - c.base) / (vh * 1.5);
			if (t > 0 && t < 1)
			{
				const x = (c.from[0] + (c.to[0] - c.from[0]) * t) * vw;
				const y = (c.from[1] + (c.to[1] - c.from[1]) * t) * vh - (sy + vh - c.base) + vh * .05;
				const angle = Math.atan2((c.to[1] - c.from[1]) * vh, (c.to[0] - c.from[0]) * vw) * 180 / Math.PI;
				c.el.style.transform = 'translate3d(' + x.toFixed(1) + 'px,' + y.toFixed(1) + 'px,0) rotate(' + angle.toFixed(1) + 'deg)';
				c.el.style.opacity = Math.min(1, Math.sin(t * Math.PI) * 1.6).toFixed(3);
			}
			else
			{
				c.el.style.opacity = '0';
			}
		}
	}

	// ---------------------------------------------------------------- the starfield: many small stars of a few colours, a few tiny diamonds
	const sky = one('#sky') as HTMLCanvasElement;
	const ctx = sky.getContext('2d') as CanvasRenderingContext2D;
	const STAR_COLOURS: Colour[] = [[207, 216, 240], [207, 216, 240], [239, 230, 210], [239, 230, 210], [143, 195, 214], [227, 176, 75], [199, 166, 217]];
	let W = 0;
	let H = 0;
	const stars: Star[] = [];
	const shots: Shot[] = [];
	let nextShot = 2.5;
	let nextGlint = .6;
	let clock = 0;

	function sizeSky(): void
	{
		const dpr = Math.min(2, window.devicePixelRatio);
		W = window.innerWidth;
		H = window.innerHeight;
		sky.width = W * dpr;
		sky.height = H * dpr;
		ctx.setTransform(dpr, 0, 0, dpr, 0, 0);
		const want = Math.round(W * H / 2400);
		while (stars.length < want)
		{
			stars.push({
				x: Math.random(),
				y: Math.random(),
				z: .2 + Math.random() * .9,
				s: .5 + Math.pow(Math.random(), 2.2) * 1.8,
				ph: Math.random() * 6.3,
				sp: .5 + Math.random() * 2,
				c: STAR_COLOURS[Math.floor(Math.random() * STAR_COLOURS.length)],
				d: Math.random() < .07,
				g: 0
			});
		}
		stars.length = want;
	}

	function drawStar(p: Star, dt: number, sy: number): void
	{
		const y = ((p.y * H - sy * .3 * p.z) % H + H) % H;
		const x = p.x * W;
		let twinkle = 1;
		if (!reduce)
		{
			twinkle = .5 + .5 * Math.sin(clock * p.sp + p.ph);
		}
		const a = (.22 + .7 * p.z) * (.22 + .78 * twinkle);
		const size = p.s * p.z * (.8 + .4 * twinkle);
		// The brighter stars glow: two faint, crisp rings that swell with the twinkle
		if (p.s * p.z > 1)
		{
			ctx.fillStyle = rgba(p.c, a * .1 * twinkle);
			ctx.beginPath();
			ctx.arc(x, y, size * 4, 0, 6.2832);
			ctx.fill();
			ctx.fillStyle = rgba(p.c, a * .2 * twinkle);
			ctx.beginPath();
			ctx.arc(x, y, size * 2.3, 0, 6.2832);
			ctx.fill();
		}
		// A flash: four thin rays that grow and fade
		if (p.g > 0)
		{
			p.g -= dt;
			const k = Math.sin((1 - Math.max(0, p.g) / 1.6) * Math.PI);
			const length = (6 + size * 7) * k;
			ctx.strokeStyle = rgba(p.c, .9 * k);
			ctx.lineWidth = 1;
			ctx.beginPath();
			ctx.moveTo(x - length, y);
			ctx.lineTo(x + length, y);
			ctx.moveTo(x, y - length * 1.3);
			ctx.lineTo(x, y + length * 1.3);
			ctx.stroke();
			ctx.fillStyle = rgba([255, 250, 240], .7 * k);
			ctx.beginPath();
			ctx.arc(x, y, size * 1.6 * k + .6, 0, 6.2832);
			ctx.fill();
		}
		ctx.fillStyle = rgba(p.c, a);
		if (p.d)
		{
			// A tiny four-point diamond
			const k = size * 3.2;
			ctx.beginPath();
			ctx.moveTo(x, y - k);
			ctx.lineTo(x + k * .32, y);
			ctx.lineTo(x, y + k);
			ctx.lineTo(x - k * .32, y);
			ctx.closePath();
			ctx.moveTo(x - k, y);
			ctx.lineTo(x, y + k * .32);
			ctx.lineTo(x + k, y);
			ctx.lineTo(x, y - k * .32);
			ctx.closePath();
			ctx.fill();
		}
		else
		{
			ctx.beginPath();
			ctx.arc(x, y, size, 0, 6.2832);
			ctx.fill();
		}
	}

	function drawSky(dt: number): void
	{
		ctx.clearRect(0, 0, W, H);
		// Every moment or so one star flashes
		if (!reduce)
		{
			nextGlint -= dt;
			if (nextGlint <= 0 && stars.length > 0)
			{
				const star = stars[Math.floor(Math.random() * stars.length)];
				if (star.z > .45)
				{
					star.g = 1.6;
				}
				nextGlint = .15 + Math.random() * .3;
			}
		}
		const sy = window.scrollY;
		for (const star of stars)
		{
			drawStar(star, dt, sy);
		}
		if (!reduce)
		{
			nextShot -= dt;
			if (nextShot <= 0)
			{
				shots.push({ x: W * (.1 + Math.random() * .7), y: H * Math.random() * .4, vx: W * (.4 + Math.random() * .3), vy: H * (.2 + Math.random() * .2), life: 0 });
				nextShot = 4 + Math.random() * 6;
			}
		}
		for (let i = shots.length - 1; i >= 0; i--)
		{
			const s = shots[i];
			s.life += dt;
			const t = s.life / .9;
			if (t >= 1)
			{
				shots.splice(i, 1);
				continue;
			}
			const hx = s.x + s.vx * s.life;
			const hy = s.y + s.vy * s.life;
			const gradient = ctx.createLinearGradient(hx - s.vx * .22, hy - s.vy * .22, hx, hy);
			gradient.addColorStop(0, 'rgba(239,230,210,0)');
			gradient.addColorStop(1, 'rgba(239,230,210,' + (.8 * Math.sin(t * Math.PI)).toFixed(3) + ')');
			ctx.strokeStyle = gradient;
			ctx.lineWidth = 1.6;
			ctx.beginPath();
			ctx.moveTo(hx - s.vx * .22, hy - s.vy * .22);
			ctx.lineTo(hx, hy);
			ctx.stroke();
		}
	}

	// ---------------------------------------------------------------- start
	let last = performance.now();
	function loop(now: number): void
	{
		const dt = Math.min(.05, (now - last) / 1000);
		last = now;
		clock += dt;
		frame(dt);
		drawSky(dt);
		requestAnimationFrame(loop);
	}

	// Words rise in when their part comes into view; drawings appear a little earlier
	const textObserver = new IntersectionObserver((entries) =>
	{
		for (const e of entries)
		{
			if (e.isIntersecting)
			{
				e.target.classList.add('in');
				textObserver.unobserve(e.target);
			}
		}
	}, { threshold: 0, rootMargin: '0px 0px -40% 0px' });
	const seenObserver = new IntersectionObserver((entries) =>
	{
		for (const e of entries)
		{
			if (e.isIntersecting)
			{
				e.target.classList.add('seen');
				seenObserver.unobserve(e.target);
			}
		}
	}, { threshold: 0, rootMargin: '0px 0px -20% 0px' });
	sections.forEach((s, i) =>
	{
		if (i > 0)
		{
			textObserver.observe(s);
			seenObserver.observe(s);
		}
	});
	sections[0].classList.add('seen');

	window.addEventListener('resize', measure);
	window.addEventListener('load', measure);
	measure();
	document.fonts.ready.then(() =>
	{
		measure();
		requestAnimationFrame(() =>
		{
			document.body.classList.remove('load');
		});
	});
	requestAnimationFrame(loop);
}
