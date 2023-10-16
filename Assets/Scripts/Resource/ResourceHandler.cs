using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceHandler: MonoBehaviour {

	[SerializeField] ResourceCollection resourceCollection;
	[SerializeField] GameObject resourceDisplayPrefab;
	[SerializeField] Transform resourceDisplayHolder;

	public Dictionary<ResourceType, Resource> resources = new Dictionary<ResourceType, Resource>();

	public void Initialize(int initialGold, int initialHp) {

		var types = Enum.GetValues(typeof(ResourceType)).Cast<ResourceType>();
		foreach (var data in resourceCollection.resources) {
			var resource = new Resource(data);
			resources.Add(data.type, resource);
			switch (data.type) {
				case ResourceType.Gold:
					resource.Amount = initialGold;
					resource.Target = initialGold;
					break;
				case ResourceType.Health:
					resource.Maximum = initialHp;
					resource.Amount = initialHp; 
					resource.Target = initialHp;

					break;
			}
			var display = Instantiate(resourceDisplayPrefab, resourceDisplayHolder).GetComponent<ResourceDisplay>();
			display.name = $"{data.name} Display";
			display.SetResource(resource);
		}
	}

	public Sprite GetTypeIcon(ResourceType type) {
		var resource = resourceCollection.resources.Where(r => r.type == type).FirstOrDefault();
		if (resource == null) return null;
		return resource.icon;
	}

	public Resource GetResource(ResourceType type) => resources[type];

	public void ApplyResourceTargets() { 
		foreach(var resource in resources.Values) {
			resource.Amount = resource.Target;
		}
	}
}
