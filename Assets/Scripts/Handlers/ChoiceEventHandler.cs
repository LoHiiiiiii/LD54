using Action = System.Action;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceEventHandler : MonoBehaviour, IEventHandler {
	[SerializeField] CanvasGroup group;
	[SerializeField] Location abyss;
	[SerializeField] float duration;
	[SerializeField] OrganizationEventHandler train;
	[SerializeField] ObjectSpawner spawner;
	[SerializeField] ResourceHandler resourceHandler;
	[SerializeField] TMP_Text choiceOne;
	[SerializeField] TMP_Text choiceTwo;
	[SerializeField] TMP_Text title;
	[SerializeField] TMP_Text description;

	bool fading = false;
	ChoiceEvent currentEvent;

	Action Completed;

	public void StartEvent(GameEvent gameEvent, Action completed){
		Completed = completed;
		if (!(gameEvent is ChoiceEvent choiceEvent)) Completed?.Invoke();
		else StartCoroutine(EventPhaseRoutine(choiceEvent)); 
	}

	private IEnumerator EventPhaseRoutine(ChoiceEvent roadEvent) {
		group.alpha = 0;
		currentEvent = roadEvent;
		fading = true;
		title.text = roadEvent.Title;
		description.text = roadEvent.Description;
		choiceOne.text = roadEvent.Choices[0].ChoiceDescription;
		choiceTwo.text = roadEvent.Choices.Count > 1 ? roadEvent.Choices[1].ChoiceDescription : "There is no other option.";
		choiceOne.GetComponentInParent<Button>().interactable = CheckEligibleChoice(0);
		choiceTwo.GetComponentInParent<Button>().interactable = CheckEligibleChoice(1);
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

		Debug.Log($"{currentEvent.Choices[index].ChoiceDescription} - {currentEvent.Choices[index].ChoiceEffects.Count}");

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
					if (Random.value > 0.66f) { victory = true; break; }
					obj.Delete(ObjectDeleteType.Destroy);
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
		Completed?.Invoke();
	}

	private bool CheckEligibleChoice(int index) {
		if (currentEvent == null) return false;
		if (index >= currentEvent.Choices.Count) return true;
		if (!currentEvent.Choices[index].ChoiceEffects.Any()) return true;
		var valid = true;
		var objectCount = 0;
		foreach (var effect in currentEvent.Choices[index].ChoiceEffects) {
			if (effect.ChoiceType == ChoiceType.Object) {
				var max = spawner.GetData(effect.ObjectVisualType).MaxStacks;
				var sum = train.GetAllObjects().Where(o => o.Data.VisualType == effect.ObjectVisualType).Select(o => o.Stacks).Sum();
				var change = Mathf.FloorToInt((sum + effect.ChoiceValue)/(float)max) - sum;

				if (effect.ChoiceValue < 0) {
					if (sum < Mathf.Abs(effect.ChoiceValue)) {
						valid = false;
						break;
					}
					objectCount -= change;
				} else if (effect.ChoiceValue > 0) {
					if (train.TotalSpots - train.GetAllObjects().Count() < change) {
						valid = false;
						break;
					}
					objectCount += change;
				}
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
	
}
