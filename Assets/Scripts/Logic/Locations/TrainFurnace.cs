using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainFurnace : MonoBehaviour
{

    [SerializeField] Location trainSpot;

	public event Action<Object> ObjectFurnaced;

	private void Start() {
		trainSpot.SpotChangedObject += HandleFurnace;
	}

	private void OnDestroy() {
		trainSpot.SpotChangedObject -= HandleFurnace;
	}

	private void HandleFurnace(LocationSpot previousSpot, Object previousObject, Object newObject) {
		if (newObject == null) return;
		if (newObject.Stacks == 1) {
			newObject.Delete(DeleteType.Destroy);
		} else {
			newObject.Stacks--;
			if (!previousSpot.TryAddObject(newObject)) {
				newObject.Delete(DeleteType.Destroy);
			}
		}
		ObjectFurnaced?.Invoke(newObject);
	}
}
