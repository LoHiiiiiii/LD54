using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationRow : MonoBehaviour {

	[SerializeField] GameObject spotPrefab;
	[SerializeField] int maxSpots;
	[SerializeField] float spotGap;
	[SerializeField] float spotWidth;

	private LocationSpot[] spots;

	public void Start() {
		this.spots = new LocationSpot[maxSpots];
		for (int i = 0; i < maxSpots; ++i) {
			var spot = Instantiate(spotPrefab, transform).GetComponent<LocationSpot>();
			spot.transform.position = transform.position + Vector3.left * (spotWidth * i + (Mathf.Max(0, i) * spotGap));
			spot.Index = i;
			spot.Row = this;
			spots[i] = spot;
		}
	}
}
