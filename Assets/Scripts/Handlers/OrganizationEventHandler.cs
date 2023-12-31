using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class OrganizationEventHandler : MonoBehaviour, IEventHandler {

	[SerializeField] ResourceHandler resourceHandler;
	[SerializeField] DragDropper dragDropper;
	[SerializeField] GameObject trainRoot;
	[SerializeField] Location cart;
	[SerializeField] Location front;
	[SerializeField] Location exit;

	Action PrepareTrain;

	public event Action Completed;

	public int TotalSpots { get; private set; }

	private void Awake() {
		cart.SpotChangedObject += HandleSpotChangeResources;
		cart.StacksChanged += HandleStackChangeResources;
		front.SpotChangedObject += HandleBurn;
	}

	public void StartEvent(GameEvent gameEvent, Action completed) {
		Completed = completed;
		StartCoroutine(OrganizationStartRoutine(gameEvent.View == View.Abyss));
	}
	public void Prepare() => PrepareTrain?.Invoke();

	private IEnumerator OrganizationStartRoutine(bool abyss) {


		var objects = cart.GetAllObjects();
		var exitObjects = new List<Object>();
		if (objects.Any()) {
			foreach (var obj in objects) {
				if (obj.Data.ExitEffects.Any()) exitObjects.Add(obj);
				if (obj.PurchasedStacks < obj.Stacks) obj.PurchasedStacks = obj.Stacks;
				if (obj.Data.DepartureEffects.Any()) {
					foreach(var pair in  obj.Data.DepartureEffects) {
						resourceHandler.GetResource(pair.Key).Target -= pair.Value;
					}
				}
			}
		}

		if (!abyss && exitObjects.Any()) {
			exitObjects.Shuffle();
			foreach (var obj in exitObjects) {
				obj.Exiting = true;
				foreach (var type in obj.Data.ExitEffects.Keys) {
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
			if (previousObject.PurchasedStacks != previousObject.Stacks) RemoveTargetResources(previousObject.Data.AddEffects, previousObject.Stacks - previousObject.PurchasedStacks);
		}
		if (newObject != null) {
			AddTargetResources(newObject.Data.DepartureEffects, newObject.Stacks);
			AddTargetResources(newObject.Data.Stats, newObject.Stacks);
			if (newObject.PurchasedStacks != newObject.Stacks) AddTargetResources(newObject.Data.AddEffects, newObject.Stacks - newObject.PurchasedStacks);
		}
	}

	private void HandleStackChangeResources(Object obj, int stackChange) {

		if (obj == null || stackChange == 0) return;
		ChangeTargetResources(obj.Data.DepartureEffects, stackChange);
		ChangeTargetResources(obj.Data.Stats, stackChange);
		if (stackChange > 0) {
			var change = Mathf.Min(stackChange, obj.Stacks - obj.PurchasedStacks);
			ChangeTargetResources(obj.Data.AddEffects, change);
		} else {
			ChangeTargetResources(obj.Data.AddEffects, Mathf.Max(0, obj.PurchasedStacks + stackChange));
		}
	}

	private void AddTargetResources(Dictionary<ResourceType, int> effects, int stacks) => ChangeTargetResources(effects, stacks);
	private void RemoveTargetResources(Dictionary<ResourceType, int> effects, int stacks) => ChangeTargetResources(effects, -1 * stacks);

	private void ChangeTargetResources(Dictionary<ResourceType, int> effects, int multiplier) {
		foreach (var type in effects.Keys) { resourceHandler.GetResource(type).Target += effects[type] * multiplier; }
	}

	private void HandleBurn(LocationSpot _, Object previousObject, Object newObject) {
		if (newObject == null) return;
		dragDropper.Active = false;
		AddTargetResources(newObject.Data.BurnEffects, 1);
		AddTargetResources(newObject.Data.AddEffects, 1);
		resourceHandler.ApplyResourceTargets();
		Completed?.Invoke();
	}

	public List<Object> GetAllObjects() => cart.GetAllObjects();
	public LocationSpot GetFirstFreeSpot(ObjectData data = null, int stacks = 1) => cart.GetFirstFreeSpot(data);

	public void Initialize(ObjectSpawner spawner, int initialPassengers, int initialBrawlers, int initialFuel, int trainSpots) {
		cart.SetSpotCount(trainSpots);
		TotalSpots = trainSpots;
		trainRoot.SetActive(true);

		PrepareTrain = () => {
			for (int i = 0; i < initialPassengers; i++) {
				var spot = cart.GetFirstFreeSpot(spawner.GetData(ObjectVisualType.Passenger));
				if (spot == null) break;
				spawner.SpawnObject(ObjectVisualType.Passenger, spot, true);
			}
			for (int i = 0; i < initialBrawlers; i++) {
				var spot = cart.GetFirstFreeSpot(spawner.GetData(ObjectVisualType.Guard));
				if (spot == null) break;
				spawner.SpawnObject(ObjectVisualType.Guard, spot, true);
			}
			for (int i = 0; i < initialFuel; i++) {
				var spot = cart.GetFirstFreeSpot(spawner.GetData(ObjectVisualType.Box));
				if (spot == null) break;
				spawner.SpawnObject(ObjectVisualType.Box, spot, true);
			}
			resourceHandler.ApplyResourceTargets();
		};
	}
}
