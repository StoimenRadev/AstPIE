import { useEffect, useRef, useState } from 'react';
import type { MouseEvent } from 'react';
import './PeopleList.css';

export interface PersonCardData
{
	id: number;
	fullName: string;
	role: string;
	isCrew: boolean;
	photoUrl: string;
	nationality: string;
	// Already formatted, for example "Born 9 March 1934 · Died 27 March 1968". Empty when not known.
	lifespan: string;
	bio: string;
	otherEntries: { href: string; title: string; role: string }[];
}

export interface PeopleLabels
{
	crew: string;
	nationality: string;
	otherEntries: string;
	close: string;
}

interface Props
{
	people: PersonCardData[];
	labels: PeopleLabels;
}

// The people of an entry. Clicking a name opens their card in the middle of the screen.
export default function PeopleList(props: Props)
{
	// Whose card is shown, -1 before the first click. Whether the card is open is
	// decided only by the browser's <dialog>: it centres the card, darkens the page
	// and closes itself on Esc, so there is nothing here to keep in step with it.
	const [selected, setSelected] = useState(-1);
	const dialogRef = useRef<HTMLDialogElement>(null);
	const closeRef = useRef<HTMLButtonElement>(null);

	// Start on the Close button, so Enter or Space closes the card again
	function focusClose()
	{
		if (closeRef.current !== null)
		{
			closeRef.current.focus();
		}
	}

	function open(index: number)
	{
		setSelected(index);
		if (dialogRef.current !== null && !dialogRef.current.open)
		{
			dialogRef.current.showModal();
		}
		focusClose();
	}

	// The first time, the card's content appears only after the click: focus it then
	useEffect(focusClose, [selected]);

	function close()
	{
		if (dialogRef.current !== null)
		{
			dialogRef.current.close();
		}
	}

	// A click on the dark area around the card closes it
	function onDialogClick(event: MouseEvent<HTMLDialogElement>)
	{
		if (event.target === dialogRef.current)
		{
			close();
		}
	}

	let person: PersonCardData | null = null;
	if (selected >= 0)
	{
		person = props.people[selected];
	}

	return (
		<>
			<ul className="people">
				{props.people.map((p, index) => (
					<li key={p.id}>
						<button type="button" onClick={() => open(index)}>
							<span className="person-name">{p.fullName}</span>
							<span className="person-role">{p.role}</span>
							{p.isCrew && <span className="person-crew">{props.labels.crew}</span>}
						</button>
					</li>
				))}
			</ul>

			<dialog ref={dialogRef} className="person-card" onClick={onDialogClick}>
				{person !== null && (
					<div className="person-card-inner">
						{person.photoUrl !== '' && <img src={person.photoUrl} alt="" />}
						<h3>{person.fullName}</h3>
						<p className="person-role">{person.role}</p>
						<p>
							<strong>{props.labels.nationality}:</strong> {person.nationality}
						</p>
						{person.lifespan !== '' && <p>{person.lifespan}</p>}
						<p>{person.bio}</p>

						{person.otherEntries.length > 0 && (
							<>
								<h4>{props.labels.otherEntries}</h4>
								<ul>
									{person.otherEntries.map((other) => (
										<li key={other.href}>
											<a href={other.href}>{other.title}</a> ({other.role})
										</li>
									))}
								</ul>
							</>
						)}

						<button type="button" className="person-close" onClick={close} ref={closeRef}>
							{props.labels.close}
						</button>
					</div>
				)}
			</dialog>
		</>
	);
}
