using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrganizationPhaseHandler : MonoBehaviour {

	[SerializeField] Location stand;
	[SerializeField] ObjectSpawner spawner;

	private void Start() {
		var count = Random.Range(stand.MaxSpots/3, stand.MaxSpots*2/3);
		for (int i = 0; i < count; i++) {
			var spot = stand.GetRandomFreeSpot();
			var data = new ObjectData();
			data.VisualType = (ObjectVisualType)Random.Range(0, 3);
			spawner.SpawnObject(data, spot);
		}
	}

}
