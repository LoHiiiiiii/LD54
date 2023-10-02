using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LocationRow : MonoBehaviour {

	[SerializeField] GameObject spotPrefab;
	[SerializeField] int maxSpots;
	[SerializeField] float spotGap;
	[SerializeField] float spotWidth;

	private LocationSpot[] spots;

	public Location Location { get; set; }
	public int MaxSpots { get => maxSpots; }

	public void Awake() {
		this.spots = new LocationSpot[maxSpots];
		for (int i = 0; i < MaxSpots; ++i) {
			var spot = Instantiate(spotPrefab, transform).GetComponent<LocationSpot>();
			spot.transform.position = transform.position + Vector3.left * (spotWidth * (i+0.5f) + (i+1) * spotGap);
			spot.transform.localScale = Vector3.one * spotWidth;
			spot.Row = this;
			spots[i] = spot;
			spot.name = name + " - Spot " + (i + 1);
		}
	}

	public LocationSpot GetFirstFreeSpot(Object potentialObject) => GetFirstFreeSpot(potentialObject, spots.ToList());
	private LocationSpot GetFirstFreeSpot(Object potentialObject, List<LocationSpot> spots) {
		foreach (var spot in spots) {
			if (spot.CurrentObject == null) return spot;
			if (potentialObject == null) continue;
			if (spot.CurrentObject.Data != potentialObject.Data) continue;
			if (spot.CurrentObject.Data.MaxStacks == null
				|| potentialObject.Stacks + spot.CurrentObject.Stacks < spot.CurrentObject.Data.MaxStacks) return spot;
		}
		return null;
	}

	public LocationSpot GetRandomFreeSpot(Object potentialObject) {
		var randomSpots = spots.ToList();
		randomSpots.Shuffle();
		return GetFirstFreeSpot(potentialObject, randomSpots);
	}

	public void ObjectChanged(LocationSpot newObjectPreviousSpot, Object spotPreviousObject, Object newObject) {
		if (newObject == null && spotPreviousObject != null && spots.Contains(spotPreviousObject.CurrentSpot)) spotPreviousObject = null;
		if (spotPreviousObject == null && spots.Contains(newObjectPreviousSpot)) newObject = null;
		if (spotPreviousObject == newObject) return;
		Location.InvokeSpotChangedObject(newObjectPreviousSpot, spotPreviousObject, newObject);
	}

	public void StacksChanged(Object increaseObject, int stackChange) => Location.InvokeStacksChanged(increaseObject, stackChange);
}
