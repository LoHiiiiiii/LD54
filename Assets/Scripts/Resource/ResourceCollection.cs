using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Collections/Resource Collection")]
public class ResourceCollection : ScriptableObject {

	public List<ResourceData> resources;
	[HideInInspector] public bool initialized;

#if UNITY_EDITOR
	void Awake() {
		if (initialized || resources.Count > 0) return;
		initialized = true; 
		resources = new List<ResourceData>();
		var types = Enum.GetValues(typeof(ResourceType)).Cast<ResourceType>();


		foreach (var type in types) {
			resources.Add(new ResourceData() { type = type, name = type.ToString() });
		}
	}
#endif
}