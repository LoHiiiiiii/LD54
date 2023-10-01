using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Location : MonoBehaviour {

	LocationRow[] rows;

	[SerializeField] ResourceHandler resourceHandler;

	public int MaxSpots { get; private set; }

	private void Awake() {
		rows = GetComponentsInChildren<LocationRow>();
		int spots = 0;
		foreach (var row in rows) {
			row.Location = this;
			spots += row.MaxSpots;
		}
		MaxSpots = spots;
	}

	private LocationSpot GetFirstFreeSpot(Object potentialObject) {
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

	public void ObjectChanged(Object previousObject, Object newObject) {
		if (previousObject != null) { }
		if ( newObject != null) { }
	}



	public void StacksChanged(Object changedObject, int stackChange) {

	}
}
