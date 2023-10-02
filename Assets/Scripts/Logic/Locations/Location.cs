using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Location : MonoBehaviour {

	LocationRow[] rows;

	public int MaxSpots { get; private set; }

	public event Action<LocationSpot, Object, Object> SpotChangedObject;

	private void Awake() {
		rows = GetComponentsInChildren<LocationRow>();
		int spots = 0;
		foreach (var row in rows) {
			row.Location = this;
			spots += row.MaxSpots;
		}
		MaxSpots = spots;
	}

	public LocationSpot GetFirstFreeSpot(Object potentialObject = null) {
		foreach(var row in rows) {
			var result = row.GetFirstFreeSpot(potentialObject);
			if (result != null) return result;
		}
		return null;
	}

	public LocationSpot GetRandomFreeSpot(Object potentialObject = null) {
		var randomRows = rows.ToList();
		randomRows.Shuffle(); 
		foreach (var row in randomRows) {
			var result = row.GetRandomFreeSpot(potentialObject);
			if (result != null) return result;
		}
		return null;
	}

	public void InvokeSpotChangedObject(
		LocationSpot newObjectPreviousSpot, 
		Object spotPreviousObject, 
		Object newObject) => SpotChangedObject?.Invoke(newObjectPreviousSpot, spotPreviousObject, newObject);

	public void InvokeStacksChanged(Object changedObject, int stackChange) {

	}
}
