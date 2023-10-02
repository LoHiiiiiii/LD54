using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowHandler : MonoBehaviour {

	[SerializeField] CanvasGroup canvas;
	[SerializeField] OrganizationPhaseHandler organizer;
	[SerializeField] RoadEventHandler roadHandler;
	[SerializeField] ResourceHandler resourceHandler;
	[Space]
	[SerializeField] float transitionDuration; 
	[SerializeField] float eventGap;


	bool[] events = new bool[] { true, false, true, false, true };
	int nextEventId;

	public bool Transitioning { get; private set; }

	void Start() {
		canvas.alpha = 1;
		NextEvent();
	}

	void NextEvent() => StartCoroutine(NextEventRoutine());

	IEnumerator NextEventRoutine() {
		if (events.Length <= nextEventId) yield break;
		Transitioning = true;
		float alpha = transitionDuration <= 0 ? 0 : 1;
		while (alpha > 0) {
			alpha -= Time.deltaTime / transitionDuration;
			canvas.alpha = alpha;
			yield return null;
		}
		Transitioning = false;

		alpha = 0;
		canvas.alpha = 0;

		//if (events[nextEventId]) organizer.InvokeEventStart();
		//else roadHandler.InvokeEventStart();
		nextEventId++;
	}

	IEnumerator EndEventRoutine() {
		Transitioning = true;
		float alpha = transitionDuration <= 0 ? 1 : 0;
		while (alpha < 1) {
			alpha -= Time.deltaTime / transitionDuration;
			canvas.alpha = alpha;
			yield return null;
		}
		Transitioning = false;
		alpha = 1;
		canvas.alpha = 1;

		yield return new WaitForSeconds(eventGap);

		if (CheckFailState()) { }
		else NextEvent();
	}

	public bool CheckFailState() {
		//if (resourceHandler.)
		return false;
	}
}
