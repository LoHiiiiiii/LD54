using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class TrainHandler : MonoBehaviour {

	[SerializeField] FlowHandler flowHandler;
	[SerializeField] ResourceHandler resourceHandler;
	[SerializeField] DragDropper dragDropper;
	[SerializeField] Location cart;
	[SerializeField] Location front;
	[SerializeField] Location exit;


	TransformRotator[] rotators;

	bool started = false;
	Action StartCallback;

	private void Awake() {
		rotators = GetComponentsInChildren<TransformRotator>();
	}

	private void Start() {
		started = true;
		StartCallback?.Invoke();
		cart.SpotChangedObject += HandleSpotChangeResources;
		cart.StacksChanged += HandleStackChangeResources;
		front.SpotChangedObject += HandleBurn;
	}

	public void InvokeOrganizingPhase() => StartCoroutine(OrganizationStartRoutine());

	private IEnumerator OrganizationStartRoutine() {
		foreach (var rotator in rotators) rotator.Rotating = false;

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
		if (exitObjects.Any()) {
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
		ChangeTargetResources(obj.Data.AddEffects, obj.Stacks);
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
		foreach (var rotator in rotators) rotator.Rotating = true;
		flowHandler.EndCurrentEvent();
	}

	public List<Object> GetAllObjects() => cart.GetAllObjects();

	public void Initialize(ObjectSpawner spawner, int initialPassengers, int initialBrawlers, int initialFuel, int trainSpots) {
		cart.SetSpotCount(trainSpots);

		StartCallback = () => {
			for (int i = 0; i < initialPassengers; i++) {
				var spot = cart.GetFirstFreeSpot(spawner.GetData(ObjectVisualType.Passenger));
				if (spot == null) break;
				spawner.SpawnObject(ObjectVisualType.Passenger, spot);
				spot.CurrentObject.PurchasedStacks = spot.CurrentObject.Stacks;
			}
			for (int i = 0; i < initialBrawlers; i++) {
				var spot = cart.GetFirstFreeSpot(spawner.GetData(ObjectVisualType.Guard));
				if (spot == null) break;
				spawner.SpawnObject(ObjectVisualType.Guard, spot);
				spot.CurrentObject.PurchasedStacks = spot.CurrentObject.Stacks;
			}
			for (int i = 0; i < initialFuel; i++) {
				var spot = cart.GetFirstFreeSpot(spawner.GetData(ObjectVisualType.Box));
				if (spot == null) break;
				spawner.SpawnObject(ObjectVisualType.Box, spot);
				spot.CurrentObject.PurchasedStacks = spot.CurrentObject.Stacks;
			}
		};

		if (started) StartCallback();
	}
}
