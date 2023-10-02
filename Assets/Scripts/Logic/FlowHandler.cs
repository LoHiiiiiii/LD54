using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FlowHandler : MonoBehaviour {

	[SerializeField] CanvasGroup canvas;
	[SerializeField] TrainHandler train;
	[SerializeField] RoadEventHandler roadHandler;
	[SerializeField] ResourceHandler resourceHandler;
	[SerializeField] ValueParser valueParser;
	[Space]
	[SerializeField] float transitionDuration;
	[SerializeField] float eventGap;


	bool[] events = new bool[] { true, false, true, false, true };
	int nextEventId;

	public bool Transitioning { get; private set; }

	void Awake() {
		valueParser.InitializeValues();
		canvas.alpha = 1;
		NextEvent();
	}

	public void EndCurrentEvent() => StartCoroutine(EndEventRoutine());
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

		if (events[nextEventId]) train.InvokeOrganizingPhase();
		else roadHandler.InvokeEventPhase();
		nextEventId++;
	}

	IEnumerator EndEventRoutine() {
		Transitioning = true;
		float alpha = transitionDuration <= 0 ? 1 : 0;
		while (alpha < 1) {
			alpha += Time.deltaTime / transitionDuration;
			canvas.alpha = alpha;
			yield return null;
		}
		Transitioning = false;
		alpha = 1;
		canvas.alpha = 1;

		yield return new WaitForSeconds(eventGap);

		var state = CheckFailState();

		if (state.fails) {
			Debug.LogError($"Fail because of {state.type}");
		} else NextEvent();
	}

	public (bool fails, ResourceType type) CheckFailState() {
		if (resourceHandler.GetResource(ResourceType.Gold).Amount < 0) return (true, ResourceType.Gold); 
		if (resourceHandler.GetResource(ResourceType.Health).Amount < 0) return (true, ResourceType.Health);
		if (!train.GetAllObjects().Any() && nextEventId < events.Length && !events[nextEventId]) return (true, ResourceType.Fuel);
		return (false, ResourceType.Fuel);
	}
}
