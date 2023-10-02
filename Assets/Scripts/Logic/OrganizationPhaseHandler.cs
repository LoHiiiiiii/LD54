using Action = System.Action;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrganizationPhaseHandler : MonoBehaviour {

	[SerializeField] Location stand;
	[SerializeField] ObjectSpawner spawner;

	public event Action OrganizationStarted;

	private void Start() {
		var count = Random.Range(stand.MaxSpots/3, stand.MaxSpots*2/3);
		for (int i = 0; i < count; i++) {
			var spot = stand.GetRandomFreeSpot();
			var data = new ObjectData();
			data.VisualType = (ObjectVisualType)Random.Range(0, 3);
			if (data.VisualType == ObjectVisualType.Box) data.Stats.Add(ResourceType.Fuel,1);
			if (data.VisualType == ObjectVisualType.Guard) {
				data.Stats.Add(ResourceType.Brawn, 1);
				data.BurnEffects.Add(ResourceType.Health, -2);
				data.DepartureEffects.Add(ResourceType.Gold, -10);
			}
			if (data.VisualType == ObjectVisualType.Passenger) {
				data.ExitEffects.Add(ResourceType.Gold, 100);
				data.BurnEffects.Add(ResourceType.Health, -2);
			}
			spawner.SpawnObject(data, spot);
		}
		OrganizationStarted?.Invoke();
	}

}
