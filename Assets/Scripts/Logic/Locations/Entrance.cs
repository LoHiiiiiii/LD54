using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entrance : MonoBehaviour
{

    [SerializeField] Location entranceLocation;
	[SerializeField] ObjectSpawner spawner;

	private void Start() {
		entranceLocation.SpotChangedObject += SpawnNewObject;
		SpawnNewObject();
	}

	private void OnDestroy() {
		entranceLocation.SpotChangedObject -= SpawnNewObject;
	}

	private void SpawnNewObject(LocationSpot previousSpot = null, Object previousObject = null, Object newObject = null) {
		if (newObject != null) return;
		spawner.SpawnObject(new ObjectData() { VisualType = ObjectVisualType.Passenger, MaxStacks = 2 }, entranceLocation.GetFirstFreeSpot());
	}
}
