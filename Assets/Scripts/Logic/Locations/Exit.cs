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
		if (newObject.Stacks == 1) {
			newObject.Delete(newObject.Exiting ? DeleteType.Exit : DeleteType.Fired);
		} else {
			newObject.Stacks--;
			if (!previousSpot.TryAddObject(newObject)) {
				newObject.Delete(DeleteType.Destroy);
			}
		}
	}
}
