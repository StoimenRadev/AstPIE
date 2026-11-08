// The radio on a case desk (drawn on the server by lib/deskHtml.ts). With no tape it only plays
// static, and the knob changes the static. A tape plays its recording once (the Wow! "tape" plays
// the six printed values as tones). The green screen draws the sound that is playing, live.

interface RadioState
{
	on: boolean;
	freq: number;
	lo: number;
	hi: number;
	ctx: AudioContext | null;
	master: GainNode | null;
	noiseGain: GainNode | null;
	band: BiquadFilterNode | null;
	tapeGain: GainNode | null;
	analyser: AnalyserNode | null;
	tape: HTMLAudioElement | null;
	// The tape playing, if any
	tapeButton: HTMLElement | null;
	synthTimer: number;
	// The screen is only redrawn while there is sound
	drawing: boolean;
}

const R: RadioState = {
	on: false, freq: 0, lo: 0, hi: 1, ctx: null, master: null, noiseGain: null, band: null, tapeGain: null,
	analyser: null, tape: null, tapeButton: null, synthTimer: 0, drawing: false
};

// How much the desk is scaled on screen, so dragging the knob feels the same at any size
let scaleOf: () => number = () => 1;

function element(id: string): HTMLElement
{
	return document.getElementById(id) as HTMLElement;
}

function playing(): boolean
{
	return R.tapeButton !== null;
}

export function setUpRadio(scale: () => number): void
{
	scaleOf = scale;
	const radio = element('radio');
	R.lo = Number(radio.dataset.low);
	R.hi = Number(radio.dataset.high);
	R.freq = R.lo + (R.hi - R.lo) * 0.3;
	for (const button of document.querySelectorAll<HTMLElement>('#tapes .tapebox'))
	{
		button.addEventListener('click', () => toggleTape(button));
	}
	element('power').addEventListener('click', () => setPower(!R.on));

	const knob = element('knob');
	knob.addEventListener('pointerdown', (e) =>
	{
		knob.setPointerCapture(e.pointerId);
		if (!R.on)
		{
			setPower(true);
		}
		let lastX = e.clientX;
		let lastY = e.clientY;
		const move = (ev: PointerEvent) =>
		{
			const d = (ev.clientX - lastX) - (ev.clientY - lastY);
			lastX = ev.clientX;
			lastY = ev.clientY;
			setFreq(R.freq + d / scaleOf() * (R.hi - R.lo) / 900);
		};
		const up = () =>
		{
			knob.removeEventListener('pointermove', move);
			knob.removeEventListener('pointerup', up);
		};
		knob.addEventListener('pointermove', move);
		knob.addEventListener('pointerup', up);
	});
	knob.addEventListener('wheel', (e) =>
	{
		e.preventDefault();
		if (!R.on)
		{
			setPower(true);
		}
		let direction = -1;
		if (e.deltaY < 0)
		{
			direction = 1;
		}
		setFreq(R.freq + direction * (R.hi - R.lo) / 400);
	}, { passive: false });
	drawScope();
}

function setFreq(f: number): void
{
	R.freq = Math.max(R.lo, Math.min(R.hi, f));
	const fraction = (R.freq - R.lo) / (R.hi - R.lo);
	element('needle').style.left = (4.3 + fraction * 91.4) + '%';
	element('knob').style.transform = 'rotate(' + (fraction * 300 - 150) + 'deg)';
	let decimals = 3;
	if (R.hi > 1000)
	{
		decimals = 1;
	}
	element('freq').textContent = R.freq.toFixed(decimals) + ' MHz';
	if (R.band !== null && R.ctx !== null)
	{
		R.band.frequency.setTargetAtTime(500 + fraction * 2600, R.ctx.currentTime, 0.05);
	}
}

// The sound only starts after a click (browsers allow audio only then)
function audio(): AudioContext
{
	if (R.ctx !== null)
	{
		return R.ctx;
	}
	const ctx = new AudioContext();
	R.ctx = ctx;
	R.master = ctx.createGain();
	R.analyser = ctx.createAnalyser();
	R.analyser.fftSize = 1024;
	R.master.connect(R.analyser);
	R.analyser.connect(ctx.destination);
	// Two seconds of crackling static, played in a loop through a filter the knob moves
	const buffer = ctx.createBuffer(1, ctx.sampleRate * 2, ctx.sampleRate);
	const data = buffer.getChannelData(0);
	for (let i = 0; i < data.length; i++)
	{
		let crackle = 1;
		if (Math.random() < 0.002)
		{
			crackle = 3;
		}
		data[i] = (Math.random() * 2 - 1) * crackle;
	}
	const noise = ctx.createBufferSource();
	noise.buffer = buffer;
	noise.loop = true;
	R.band = ctx.createBiquadFilter();
	R.band.type = 'bandpass';
	R.band.Q.value = 0.5;
	R.noiseGain = ctx.createGain();
	R.noiseGain.gain.value = 0;
	noise.connect(R.band);
	R.band.connect(R.noiseGain);
	R.noiseGain.connect(R.master);
	noise.start();
	R.tapeGain = ctx.createGain();
	R.tapeGain.connect(R.master);
	setFreq(R.freq);
	return ctx;
}

function updateMix(): void
{
	const radio = element('radio');
	radio.classList.toggle('on', R.on || playing());
	radio.classList.toggle('sound', playing());
	if (R.on || playing())
	{
		drawScope();
	}
	if (R.ctx === null || R.noiseGain === null)
	{
		return;
	}
	let level = 0;
	if (R.on && !playing())
	{
		level = 0.18;
	}
	R.noiseGain.gain.setTargetAtTime(level, R.ctx.currentTime, 0.05);
}

export function setPower(on: boolean): void
{
	const ctx = audio();
	if (ctx.state === 'suspended')
	{
		ctx.resume();
	}
	R.on = on;
	if (!on)
	{
		stopTape();
	}
	updateMix();
}

export function tapeIsPlaying(): boolean
{
	return playing();
}

function toggleTape(button: HTMLElement): void
{
	const ctx = audio();
	if (ctx.state === 'suspended')
	{
		ctx.resume();
	}
	if (R.tapeButton === button)
	{
		stopTape();
		return;
	}
	stopTape();
	R.on = true;
	R.tapeButton = button;
	button.classList.add('playing');
	const tapeWindow = element('tapewin');
	tapeWindow.textContent = tapeWindow.dataset.playing + ' ' + button.dataset.label;
	if (button.dataset.synth === 'wow')
	{
		playSixValues(ctx);
	}
	else
	{
		const player = new Audio();
		player.crossOrigin = 'anonymous';
		player.src = String(button.dataset.src);
		R.tape = player;
		ctx.createMediaElementSource(player).connect(R.tapeGain as GainNode);
		player.addEventListener('ended', tapeFinished);
		player.play().catch(() => stopTape());
	}
	updateMix();
}

// The Wow! signal was printed as six numbers, never recorded as sound. This turns the numbers into six tones.
function playSixValues(ctx: AudioContext): void
{
	const values = [6, 14, 26, 30, 19, 5];
	const now = ctx.currentTime + 0.05;
	for (let i = 0; i < values.length; i++)
	{
		const tone = ctx.createOscillator();
		const gain = ctx.createGain();
		tone.type = 'sine';
		tone.frequency.value = 300 + values[i] * 20;
		gain.gain.setValueAtTime(0, now + i * 0.6);
		gain.gain.linearRampToValueAtTime(0.06 + values[i] / 70, now + i * 0.6 + 0.05);
		gain.gain.linearRampToValueAtTime(0, now + i * 0.6 + 0.52);
		tone.connect(gain);
		gain.connect(R.tapeGain as GainNode);
		tone.start(now + i * 0.6);
		tone.stop(now + i * 0.6 + 0.55);
	}
	R.synthTimer = window.setTimeout(tapeFinished, 3900);
}

// When a tape is over the radio stops completely, so no static starts by itself
function tapeFinished(): void
{
	stopTape();
	setPower(false);
}

export function stopTape(): void
{
	window.clearTimeout(R.synthTimer);
	if (R.tape !== null)
	{
		R.tape.pause();
		R.tape = null;
	}
	if (R.tapeButton !== null)
	{
		R.tapeButton.classList.remove('playing');
		R.tapeButton = null;
		const tapeWindow = element('tapewin');
		tapeWindow.textContent = String(tapeWindow.dataset.none);
	}
	updateMix();
}

// The green screen: the sound that is playing right now, as a wave. With no sound it shows a flat
// line once and stops redrawing.
function drawScope(): void
{
	if (R.drawing)
	{
		return;
	}
	R.drawing = true;
	const canvas = element('scope') as HTMLCanvasElement;
	const g = canvas.getContext('2d') as CanvasRenderingContext2D;
	const wave = new Uint8Array(512);
	const frame = () =>
	{
		g.clearRect(0, 0, 224, 224);
		g.strokeStyle = 'rgba(120,255,160,.15)';
		g.lineWidth = 1;
		for (let k = 1; k < 4; k++)
		{
			g.beginPath();
			g.moveTo(0, k * 56);
			g.lineTo(224, k * 56);
			g.moveTo(k * 56, 0);
			g.lineTo(k * 56, 224);
			g.stroke();
		}
		g.strokeStyle = '#8dffb0';
		g.shadowColor = '#8dffb0';
		g.shadowBlur = 8;
		g.lineWidth = 2.4;
		g.beginPath();
		const sound = R.analyser !== null && (R.on || playing());
		if (sound)
		{
			(R.analyser as AnalyserNode).getByteTimeDomainData(wave);
			let gain = 120;
			if (playing())
			{
				gain = 220;
			}
			for (let x = 0; x <= 224; x += 2)
			{
				const v = wave[Math.floor(x / 224 * 511)] / 128 - 1;
				const y = Math.max(6, Math.min(218, 112 + v * gain));
				if (x === 0)
				{
					g.moveTo(x, y);
				}
				else
				{
					g.lineTo(x, y);
				}
			}
		}
		else
		{
			g.strokeStyle = 'rgba(141,255,176,.3)';
			g.moveTo(0, 112);
			g.lineTo(224, 112);
		}
		g.stroke();
		g.shadowBlur = 0;
		if (sound)
		{
			requestAnimationFrame(frame);
		}
		else
		{
			R.drawing = false;
		}
	};
	frame();
}
