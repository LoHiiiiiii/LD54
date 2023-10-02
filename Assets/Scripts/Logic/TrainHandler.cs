using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class TrainHandler : MonoBehaviour {

	[SerializeField] ResourceHandler resourceHandler;
	[SerializeField] OrganizationPhaseHandler organization;
	[SerializeField] DragDropper dragDropper;
	[SerializeField] Location cart;
	[SerializeField] Location exit;

	private void Awake() {
		organization.OrganizationStarted += OrganizationStart;
		cart.SpotChangedObject += HandleSpotChangeResources;
		cart.StacksChanged += HandleStackChangeResources;
	}

	private void OrganizationStart() => StartCoroutine(OrganizationStartRoutine());

	private IEnumerator OrganizationStartRoutine() { 
		var objects = cart.GetAllObjects();
		var exitObjects = new List<Object>();
		if (objects.Any()) {
			foreach (var obj in objects) {
				if (obj.Data.ExitEffects.Any()) exitObjects.Add(obj);
			}
		}
		if (exitObjects.Any()) {
			exitObjects.Shuffle();
			foreach (var obj in exitObjects) {
				foreach(var type in obj.Data.ExitEffects.Keys) {
					resourceHandler.GetResource(type).Target += obj.Data.ExitEffects[type];
				}
				exit.GetFirstFreeSpot().TryAddObject(obj);
				while (!obj.Deleting || !obj.AtTarget) {
					yield return null;
				}
			}
		}
		dragDropper.Active = true;
	}

	private void HandleSpotChangeResources(LocationSpot _, Object previousObject, Object newObject) {
		if (previousObject != null) {
			RemoveTargetResources(previousObject.Data.DepartureEffects, previousObject.Stacks);
			RemoveTargetResources(previousObject.Data.Stats, previousObject.Stacks);
		}
		if (newObject != null) {
			AddTargetResources(newObject.Data.DepartureEffects, newObject.Stacks);
			AddTargetResources(newObject.Data.Stats, newObject.Stacks);
		}
	}

	private void HandleStackChangeResources(Object obj, int stackChange) {
		if (obj == null || stackChange == 0) return;
		ChangeTargetResources(obj.Data.DepartureEffects, stackChange);
		ChangeTargetResources(obj.Data.Stats, stackChange);
	}

	private void AddTargetResources(Dictionary<ResourceType, int> effects, int stacks) => ChangeTargetResources(effects, stacks); 
	private void RemoveTargetResources(Dictionary<ResourceType, int> effects, int stacks) => ChangeTargetResources(effects, -1 * stacks);

	private void ChangeTargetResources(Dictionary<ResourceType, int> effects, int multiplier) {
		foreach (var type in effects.Keys) { resourceHandler.GetResource(type).Target += effects[type] * multiplier; }
	}

}