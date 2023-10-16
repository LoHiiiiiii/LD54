using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LocationRow : MonoBehaviour {

	[SerializeField] GameObject spotPrefab;
	[SerializeField] public int maxSpots;
	[SerializeField] float spotGap;
	[SerializeField] public float spotWidth;
	[SerializeField] float spotHeight;


	private LocationSpot[] spots;

	public Location Location { get; set; }
	public int MaxSpots { get => maxSpots; }

	public void Awake() {
		this.spots = new LocationSpot[maxSpots];
		for (int i = 0; i < MaxSpots; ++i) {
			var spot = Instantiate(spotPrefab, transform).GetComponent<LocationSpot>();
			spot.transform.position = transform.position + Vector3.left * (spotWidth * (i+0.5f) + (i+1) * spotGap);
			spot.Pivot.localScale = new Vector3(spotWidth, spotHeight);
			spot.Row = this;
			spots[i] = spot;
			spot.name = name + " - Spot " + (i + 1);
		}
	}

	public List<Object> GetAllObjects() {
		var objects = new List<Object>();
		foreach (var spot in spots) {
			if (spot.CurrentObject != null) objects.Add(spot.CurrentObject);
		}
		return objects;
	}

	public LocationSpot GetFirstFreeSpot(ObjectData data, int stacks) => GetFirstFreeSpot(data, stacks, spots.ToList());
	private LocationSpot GetFirstFreeSpot(ObjectData data, int stacks, List<LocationSpot> spots) {
		foreach (var spot in spots) {
			if (spot.CurrentObject == null) return spot;
			if (data == null) continue;
			if (spot.CurrentObject.Data.VisualType != data.VisualType) continue;
			if (spot.CurrentObject.Data.MaxStacks == null
				|| stacks + spot.CurrentObject.Stacks <= spot.CurrentObject.Data.MaxStacks) return spot;
		}
		return null;
	}

	public LocationSpot GetRandomFreeSpot(ObjectData data, int stacks) {
		var randomSpots = spots.ToList();
		randomSpots.Shuffle();
		return GetFirstFreeSpot(data, stacks, randomSpots);
	}

	public void ObjectChanged(LocationSpot newObjectPreviousSpot, Object spotPreviousObject, Object newObject) {
		if (newObject == null && spotPreviousObject != null && spots.Contains(spotPreviousObject.CurrentSpot)) spotPreviousObject = null;
		if (spotPreviousObject == null && spots.Contains(newObjectPreviousSpot)) newObject = null;
		Location.InvokeSpotChangedObject(newObjectPreviousSpot, spotPreviousObject, newObject);
	}

	public void StacksChanged(Object increaseObject, int stackChange) => Location.InvokeStacksChanged(increaseObject, stackChange);
}
