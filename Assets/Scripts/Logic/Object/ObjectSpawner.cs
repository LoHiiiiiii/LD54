using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour {
    [SerializeField] GameObject objectPrefab;
    [SerializeField] ObjectVisualCollection visualCollection;

    Dictionary<ObjectVisualType, ObjectVisualData> visualData;

	public void Awake() {
		visualData = new Dictionary<ObjectVisualType, ObjectVisualData> ();
        foreach(var visual in visualCollection.objectVisuals) {
            visualData.Add(visual.type, visual);
        }
	}

	public Object SpawnObject(ObjectData data, LocationSpot spot, int stacks = 1) {
        if (!Object.FitsSpot(spot, data, stacks)) return null;
        var newObject = Instantiate(objectPrefab, transform).GetComponent<Object>();
        newObject.name = visualData[data.VisualType].name;
        newObject.Data = data;
        newObject.VisualData = visualData[data.VisualType];
		if (!spot.TryAddObject(newObject)) { throw new Exception("Fits, but couldn't add!"); }
        return newObject;
    }
}
