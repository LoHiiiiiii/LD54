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
			newObject.Delete(newObject.Exiting ? ObjectDeleteType.Exit : ObjectDeleteType.Fired);
		} else {			
			newObject.PurchasedStacks = Mathf.Min(newObject.PurchasedStacks, newObject.Stacks - 1);
			newObject.Stacks--;
			if (!previousSpot.TryAddObject(newObject)) {
				newObject.Delete(ObjectDeleteType.Destroy);
			}
		}
	}
}
