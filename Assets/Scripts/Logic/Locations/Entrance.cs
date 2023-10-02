using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entrance : MonoBehaviour
{

    [SerializeField] Location entranceLocation;
	[SerializeField] ObjectSpawner spawner;

	ObjectData data = new ObjectData();

	private void Start() {
		data.DepartureEffects.Add(ResourceType.Health, -1); 
		data.DepartureEffects.Add(ResourceType.Gold, +1);
		data.VisualType = ObjectVisualType.Passenger;
		data.MaxStacks = 2;
		entranceLocation.SpotChangedObject += SpawnNewObject;
		SpawnNewObject();
	}

	private void OnDestroy() {
		entranceLocation.SpotChangedObject -= SpawnNewObject;
	}

	private void SpawnNewObject(LocationSpot previousSpot = null, Object previousObject = null, Object newObject = null) {
		if (newObject != null) return;
		spawner.SpawnObject(data, entranceLocation.GetFirstFreeSpot());
	}
}
