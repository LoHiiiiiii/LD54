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

	void Awake() {
		var types = Enum.GetValues(typeof(ResourceType)).Cast<ResourceType>();
		foreach (var data in resourceCollection.resources) {
			var resource = new Resource(data);
			resources.Add(data.type, resource);
			var display = Instantiate(resourceDisplayPrefab, resourceDisplayHolder).GetComponent<ResourceDisplay>();
			display.name = $"{data.name} Display";
			display.SetResource(resource);
		}
	}
}
