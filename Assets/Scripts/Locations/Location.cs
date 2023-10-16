using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Location : MonoBehaviour {

	LocationRow[] rows;

	public int MaxSpots { get; private set; }

	public event Action<LocationSpot, Object, Object> SpotChangedObject; 
	public event Action<Object, int> StacksChanged;


	private void Awake() {
		rows = GetComponentsInChildren<LocationRow>();
		int spots = 0;
		foreach (var row in rows) {
			row.Location = this;
			spots += row.MaxSpots;
		}
		MaxSpots = spots;
	}

	public List<Object> GetAllObjects() {
		var objects = new List<Object>();
		foreach (var row in rows) {
			objects.AddRange(row.GetAllObjects());
		}
		return objects;
	}

	public LocationSpot GetFirstFreeSpot(ObjectData data = null, int stacks = 1) {
		foreach(var row in rows) {
			var result = row.GetFirstFreeSpot(data, stacks);
			if (result != null) return result;
		}
		return null;
	}

	public LocationSpot GetRandomFreeSpot(ObjectData data = null, int stacks = 1) {
		var randomRows = rows.ToList();
		randomRows.Shuffle(); 
		foreach (var row in randomRows) {
			var result = row.GetRandomFreeSpot(data, stacks);
			if (result != null) return result;
		}
		return null;
	}

	public void InvokeSpotChangedObject(
		LocationSpot newObjectPreviousSpot, 
		Object spotPreviousObject, 
		Object newObject) => SpotChangedObject?.Invoke(newObjectPreviousSpot, spotPreviousObject, newObject);

	public void InvokeStacksChanged(Object obj, int stacks) => StacksChanged?.Invoke(obj, stacks);

	public void SetSpotCount(int count) {
		var rows = GetComponentsInChildren<LocationRow>(true);
		foreach (var row in rows) {
			var prev = row.maxSpots;
			row.maxSpots = count;
			row.spotWidth = prev / (float)count * row.spotWidth;
		}
	}

}
