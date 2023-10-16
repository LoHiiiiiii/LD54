using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Collections/Object Visual Collection")]
public class ObjectVisualCollection : ScriptableObject {

	public List<ObjectVisualData> objectVisuals;
	[HideInInspector] public bool initialized;

#if UNITY_EDITOR
	void Awake() {
		if (initialized || objectVisuals.Count > 0) return;
		initialized = true;
		objectVisuals = new List<ObjectVisualData>();
		var types = Enum.GetValues(typeof(ObjectVisualType)).Cast<ObjectVisualType>();

		foreach (var type in types) {
			objectVisuals.Add(new ObjectVisualData() { type = type, name = type.ToString() });
		}
	}
#endif
}