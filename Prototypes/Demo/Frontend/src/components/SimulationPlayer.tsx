import { useEffect, useRef, useState } from 'react';
import './SimulationPlayer.css';

export interface SimulationLabels
{
	start: string;
	loading: string;
	starting: string;
	error: string;
	retry: string;
	expand: string;
	minimise: string;
}

interface Props
{
	title: string;
	buildUrl: string;
	thumbnailUrl: string;
	labels: SimulationLabels;
}

// What the simulation page sends while it loads (see wwwroot/index.html in the Godot web host)
interface SimulationMessage
{
	source: string;
	status: string;
	loaded: number;
	total: number;
}

// idle: thumbnail with a Start button. Nothing heavy is downloaded yet.
// loading / starting: the frame loads under our own loading screen.
// ready: the simulation runs. error: something went wrong, the visitor can try again.
type Status = 'idle' | 'loading' | 'starting' | 'ready' | 'error';

// If the simulation page says nothing for this long, it probably could not be reached
const NO_ANSWER_TIMEOUT_MS = 20000;

export default function SimulationPlayer(props: Props)
{
	const [status, setStatus] = useState<Status>('idle');
	// From 0 to 1. -1 while the size of the download is not known yet
	// (also when everything comes from the browser cache).
	const [progress, setProgress] = useState(-1);
	const [expanded, setExpanded] = useState(false);
	// A new number creates a new frame (used by "Try again")
	const [attempt, setAttempt] = useState(0);
	const frameRef = useRef<HTMLIFrameElement>(null);
	const slotRef = useRef<HTMLDivElement>(null);
	const boxRef = useRef<HTMLDivElement>(null);
	const gotAnswer = useRef(false);

	// In the page the box can land between two screen pixels (for example at x = 390.5),
	// and the browser then blurs the whole simulation. Move it onto whole pixels.
	// When expanded, the CSS already places it on whole pixels.
	useEffect(() =>
	{
		if (status === 'idle')
		{
			return;
		}

		function snapToPixels()
		{
			if (slotRef.current === null || boxRef.current === null)
			{
				return;
			}
			if (expanded)
			{
				boxRef.current.style.transform = '';
				return;
			}
			const ratio = window.devicePixelRatio;
			const rect = slotRef.current.getBoundingClientRect();
			const moveX = Math.round(rect.left * ratio) / ratio - rect.left;
			const moveY = Math.round(rect.top * ratio) / ratio - rect.top;
			boxRef.current.style.transform = 'translate(' + moveX + 'px, ' + moveY + 'px)';
		}

		snapToPixels();
		// Layout changes (window size, fonts loading, images above) can move the box again
		const observer = new ResizeObserver(snapToPixels);
		observer.observe(document.body);
		window.addEventListener('resize', snapToPixels);
		return () =>
		{
			observer.disconnect();
			window.removeEventListener('resize', snapToPixels);
		};
	}, [status, expanded]);

	// Listen to the loading messages from the simulation frame
	useEffect(() =>
	{
		if (status === 'idle')
		{
			return;
		}

		function onMessage(event: MessageEvent)
		{
			if (frameRef.current === null || event.source !== frameRef.current.contentWindow)
			{
				return;
			}
			const message = event.data as SimulationMessage;
			if (typeof message !== 'object' || message === null || message.source !== 'astpie-simulation')
			{
				return;
			}

			gotAnswer.current = true;
			if (message.status === 'progress' && message.total > 0)
			{
				setProgress(Math.min(1, message.loaded / message.total));
			}
			else if (message.status === 'starting')
			{
				// The engine can report the end of the download after it is already
				// running, so "starting" must never go back from ready or error
				setStatus((previous) =>
				{
					if (previous === 'loading')
					{
						return 'starting';
					}
					return previous;
				});
			}
			else if (message.status === 'ready')
			{
				setStatus('ready');
				focusFrame();
			}
			else if (message.status === 'error')
			{
				setStatus('error');
			}
		}

		window.addEventListener('message', onMessage);
		return () => window.removeEventListener('message', onMessage);
	}, [status]);

	// Give up if the simulation page never answers (for example the server is down)
	useEffect(() =>
	{
		if (status !== 'loading')
		{
			return;
		}
		const timer = window.setTimeout(() =>
		{
			if (!gotAnswer.current)
			{
				setStatus('error');
			}
		}, NO_ANSWER_TIMEOUT_MS);
		return () => window.clearTimeout(timer);
	}, [status, attempt]);

	// While expanded: Esc closes it and the page behind does not scroll
	useEffect(() =>
	{
		if (!expanded)
		{
			return;
		}

		function onKey(event: KeyboardEvent)
		{
			if (event.key === 'Escape')
			{
				setExpanded(false);
			}
		}

		window.addEventListener('keydown', onKey);
		document.body.style.overflow = 'hidden';
		return () =>
		{
			window.removeEventListener('keydown', onKey);
			document.body.style.overflow = '';
		};
	}, [expanded]);

	function focusFrame()
	{
		if (frameRef.current !== null)
		{
			frameRef.current.focus();
		}
	}

	function start()
	{
		gotAnswer.current = false;
		setProgress(-1);
		setStatus('loading');
	}

	function retry()
	{
		setAttempt(attempt + 1);
		start();
	}

	function toggleExpanded()
	{
		setExpanded(!expanded);
		// The frame keeps running; give it the keyboard back after the click on the button
		window.setTimeout(focusFrame, 0);
	}

	if (status === 'idle')
	{
		return (
			<figure className="simulation">
				<button
					type="button"
					className="simulation-thumb"
					onClick={start}
					aria-label={props.labels.start + ': ' + props.title}
				>
					{props.thumbnailUrl !== '' && <img src={props.thumbnailUrl} alt="" loading="lazy" />}
					<span className="thumb-run">{props.labels.start}</span>
				</button>
			</figure>
		);
	}

	let figureClass = 'simulation';
	if (expanded)
	{
		figureClass = figureClass + ' is-expanded';
	}

	let expandLabel = props.labels.expand;
	if (expanded)
	{
		expandLabel = props.labels.minimise;
	}

	// Text and bar: a percentage while downloading, a moving bar while it is unknown or starting
	let loadingText = props.labels.loading;
	let barClass = 'simulation-progress is-busy';
	let barWidth = '0%';
	if (status === 'starting')
	{
		loadingText = props.labels.starting;
	}
	else if (progress >= 0)
	{
		loadingText = props.labels.loading + ' ' + Math.round(progress * 100) + '%';
		barClass = 'simulation-progress';
		barWidth = Math.round(progress * 100) + '%';
	}

	return (
		<figure className={figureClass}>
			{/* Keeps the place in the page while the frame is expanded */}
			<div className="simulation-slot" ref={slotRef}>
				<div className="simulation-frame" ref={boxRef}>
					<iframe
						key={attempt}
						ref={frameRef}
						src={props.buildUrl}
						title={props.title}
						allow="fullscreen"
					/>

					{(status === 'loading' || status === 'starting') && (
						<div className="simulation-overlay" role="status">
							<p>{loadingText}</p>
							<div className={barClass}>
								<div style={{ width: barWidth }} />
							</div>
						</div>
					)}

					{status === 'error' && (
						<div className="simulation-overlay" role="alert">
							<p>{props.labels.error}</p>
							<button type="button" className="simulation-retry" onClick={retry}>
								{props.labels.retry}
							</button>
						</div>
					)}

					<button type="button" className="simulation-expand" onClick={toggleExpanded}>
						{expandLabel}
					</button>
				</div>
			</div>
			{expanded && <div className="simulation-backdrop" onClick={toggleExpanded} />}
			<figcaption>{props.title}</figcaption>
		</figure>
	);
}
