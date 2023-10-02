using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class RoadEventHandler : MonoBehaviour {
	[SerializeField] CanvasGroup group;
	[SerializeField] float eventStartDelay;
	[SerializeField] Location abyss;
	[SerializeField] float duration;
	[SerializeField] TrainHandler train;
	[SerializeField] ObjectSpawner spawner;
	[SerializeField] ResourceHandler resourceHandler;
	[SerializeField] FlowHandler flowHandler;
	[SerializeField] TMP_Text choiceOne;
	[SerializeField] TMP_Text choiceTwo;
	[SerializeField] TMP_Text title;
	[SerializeField] TMP_Text description;

	Dictionary<int, HashSet<RoadEventData>> pools;

	bool fading = false;
	RoadEventData currentEvent;

	public void InvokeEventPhase(RoadEventData roadEvent) => StartCoroutine(EventPhaseRoutine(roadEvent));


	private IEnumerator EventPhaseRoutine(RoadEventData roadEvent) {
		group.alpha = 0;
		currentEvent = roadEvent;
		fading = true;
		title.text = roadEvent.Title;
		description.text = roadEvent.Description;
		choiceOne.text = roadEvent.Choices[0].ChoiceDescription;
		choiceTwo.text = roadEvent.Choices.Count > 1 ? roadEvent.Choices[1].ChoiceDescription : "There is no other option.";
		choiceOne.GetComponentInParent<Button>().interactable = CheckEligibleChoice(0);
		choiceTwo.GetComponentInParent<Button>().interactable = CheckEligibleChoice(1);
		yield return new WaitForSeconds(eventStartDelay);
		float a = duration <= 0 ? 1 : 0;
		while (a < 1) {
			group.alpha = a;
			a += Time.deltaTime / duration;
			yield return null;
		}
		group.alpha = 1;
		fading = false;
	}

	public void OptionOne() => StartCoroutine(HandleOptionRoutine(0));
	public void OptionTwo() => StartCoroutine(HandleOptionRoutine(1));

	private IEnumerator HandleOptionRoutine(int index) {
		if (fading) yield break;
		if (index >= currentEvent.Choices.Count) yield break;

		float a = 1;
		while (a > 0) {
			group.alpha = a;
			a -= Time.deltaTime / duration;
			yield return null;
		}
		group.alpha = 0;

		foreach (var effect in currentEvent.Choices[index].ChoiceEffects) {
			if (effect.ChoiceType == ChoiceType.Object) {
				bool remove = effect.ChoiceValue < 0;
				if (remove) {
					var kills = 0;
					foreach (var obj in train.GetAllObjects()) {
						if (obj.Data.VisualType == effect.ObjectVisualType) {
							abyss.GetFirstFreeSpot().TryAddObject(obj);
							while (!obj.AtTarget) yield return null;
							kills++;
							if (kills >= Mathf.Abs(effect.ChoiceValue)) break;
						}
					}
				} else {
					for (int i = 0; i < effect.ChoiceValue; ++i) {
						spawner.SpawnObject(effect.ObjectVisualType, train.GetFirstFreeSpot(spawner.GetData(effect.ObjectVisualType)));
					}
				}
			} else if (effect.ChoiceType == ChoiceType.Resource) {
				resourceHandler.GetResource(effect.ResourceType).Target += effect.ChoiceValue;
			} else if (effect.ChoiceType == ChoiceType.Brawl) {
				bool victory = false;
				foreach (var obj in train.GetAllObjects()) {
					if (obj.Data.VisualType != ObjectVisualType.Guard) continue;
					if (Random.value > 0.5) { victory = true; break; }
					obj.Delete();
					AudioMaster.Instance.Play(EffectMaster.Instance.burnSFX);
					yield return new WaitForSeconds(2f);

				}
				if (!victory) {
					resourceHandler.GetResource(ResourceType.Health).Target -= 2 * effect.ChoiceValue;
					AudioMaster.Instance.Play(EffectMaster.Instance.burnSFX);
				} else {
					AudioMaster.Instance.Play(EffectMaster.Instance.chainSFX);
				}
			}
		}
		var state = flowHandler.CheckFailState();
		if (state.fails) {
			flowHandler.GameOver(state.type);
		} else train.InvokeOrganizingPhase(true);
	}

	private bool CheckEligibleChoice(int index) {
		if (currentEvent == null) return false;
		if (index >= currentEvent.Choices.Count) return true;
		if (!currentEvent.Choices[index].ChoiceEffects.Any()) return true;
		var valid = true;
		var objectCount = 0;
		foreach (var effect in currentEvent.Choices[index].ChoiceEffects) {
			if (effect.ChoiceType == ChoiceType.Object) {
				if (train.GetAllObjects().Where(o => o.Data.VisualType == effect.ObjectVisualType).Count() < effect.ChoiceValue) {
					valid = false;
					break;
				}
				objectCount += Mathf.CeilToInt(effect.ChoiceValue / (float)spawner.GetData(effect.ObjectVisualType).MaxStacks);
			} else if (effect.ChoiceType == ChoiceType.Resource) {
				if (resourceHandler.GetResource(effect.ResourceType).Target + effect.ChoiceValue < 0) {
					valid = false;
					break;
				}
			}
		}
		if (objectCount > train.TotalSpots - train.GetAllObjects().Count) return false;
		return valid;
	}

	public void AddPools(Dictionary<int, HashSet<RoadEventData>> pools) => this.pools = pools;

	public RoadEventData GetRandomEvent(int pool) {
		if (!pools.ContainsKey(pool)) return null;
		if (!pools[pool].Any()) return null;
		var list = pools[pool].ToList();
		list.Shuffle();
		pools[pool].Remove(list[0]);
		return list[0];
	}
}
