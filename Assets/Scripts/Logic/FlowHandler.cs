using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FlowHandler : MonoBehaviour {

	[SerializeField] CanvasGroup canvas;
	[SerializeField] TrainHandler train;
	[SerializeField] RoadEventHandler roadHandler;
	[SerializeField] ResourceHandler resourceHandler;
	[SerializeField] ValueParser valueParser;
	[SerializeField] GameObject menus;
	[SerializeField] GameObject orgHolder;
	[Space]
	[SerializeField] float transitionDuration;
	[SerializeField] float eventGap;


	int[] events = new int[] { 0, 1, 1, 1, 0, 1, 1, 1, 2, 2,2,2,3,2,2,2,3,3,2,3,3,2,3 };
	int nextEventId;

	public bool Transitioning { get; private set; }

	void Start() {
		AudioMaster.Instance.Play(EffectMaster.Instance.stationMusic);
		valueParser.Initialize();
		canvas.alpha = 1;
		StartCoroutine(FadeInRoutine(null));
	}

	public void EndCurrentEvent() => StartCoroutine(FadeOutRoutine(NextEvent));
	void NextEvent() {
		var state = CheckFailState();
		if (state.fails) {
		StartCoroutine(FadeOutRoutine(() => GameOver(state.type)));
			return;
		}
		PrepNextEvent();
		StartCoroutine(FadeInRoutine(StartNextEvent));
	}

	bool started;
	public void StartGame() {
		if (started) return;
		started = true;
		StartCoroutine(FadeOutRoutine(() => {
			menus.SetActive(false);
			train.gameObject.SetActive(true);
			orgHolder.SetActive(true);
			NextEvent();
		}));

	}

	public void Reload() {
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	IEnumerator FadeInRoutine(Action Callback) {
		Transitioning = true;
		float alpha = transitionDuration <= 0 ? 0 : canvas.alpha;
		if (events.Length <= nextEventId) yield break;
		while (alpha > 0) {
			alpha -= Time.deltaTime / transitionDuration;
			canvas.alpha = alpha;
			yield return null;
		}
		Transitioning = false;

		alpha = 0;
		canvas.alpha = 0;
		Callback?.Invoke();
	}
	private void PrepNextEvent() {
		if (events[nextEventId] == 0) {
			orgHolder.SetActive(true);
			roadHandler.gameObject.SetActive(false);
		} else {
			orgHolder.SetActive(false);
			roadHandler.gameObject.SetActive(true);
		}
	}
	private void StartNextEvent() {

		if (events[nextEventId] == 0) {
			train.InvokeOrganizingPhase();
		}
		else {
			orgHolder.SetActive(false);
			roadHandler.InvokeEventPhase(roadHandler.GetRandomEvent(events[nextEventId]));
		}
		nextEventId++;
	}

	IEnumerator FadeOutRoutine(Action Callback) {
		Transitioning = true;
		float alpha = transitionDuration <= 0 ? 1 : canvas.alpha;
		while (alpha < 1) {
			alpha += Time.deltaTime / transitionDuration;
			canvas.alpha = alpha;
			yield return null;
		}
		Transitioning = false;
		alpha = 1;
		canvas.alpha = 1;

		yield return new WaitForSeconds(eventGap);
		Callback.Invoke();
	}

	public void GameOver(ResourceType type) {
		Reload();
	}

	public (bool fails, ResourceType type) CheckFailState() {
		if (resourceHandler.GetResource(ResourceType.Gold).Amount < 0) return (true, ResourceType.Gold);
		if (resourceHandler.GetResource(ResourceType.Health).Amount < 0) return (true, ResourceType.Health);
		if (!train.GetAllObjects().Any() && nextEventId < events.Length && events[nextEventId] != 0) return (true, ResourceType.Fuel);
		return (false, ResourceType.Fuel);
	}
}
