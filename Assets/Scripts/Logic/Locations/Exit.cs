using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : MonoBehaviour
{

    [SerializeField] Location exitLocation;

	private void Start() {
		exitLocation.SpotChangedObject += DeleteChangedObject;
	}

	private void OnDestroy() {
		exitLocation.SpotChangedObject -= DeleteChangedObject;
	}

	private void DeleteChangedObject(LocationSpot previousSpot, Object previousObject, Object newObject) {
		if (newObject == null) return;
		newObject.Delete();
	}
}
