using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.U2D.Path;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour {
	[SerializeField] ResourceHandler resourceHandler;
	[SerializeField] GameObject objectPrefab;
	[SerializeField] GameObject infoPrefab;

	[SerializeField] ObjectVisualCollection visualCollection;

	Dictionary<ObjectVisualType, ObjectVisualData> visualData;

	public void Awake() {
		visualData = new Dictionary<ObjectVisualType, ObjectVisualData>();
		foreach (var visual in visualCollection.objectVisuals) {
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
		AddDataInfo(newObject);
		return newObject;
	}

	private void AddDataInfo(Object infoObject) {
		AddInfo(infoObject.Data.ExitEffects, infoObject, (int i) => DataDisplayType.Default, "on next station");
		AddInfo(infoObject.Data.DepartureEffects, infoObject, (int i) => i < 0 ? DataDisplayType.Decrease : DataDisplayType.Increase, "per stop");
		AddInfo(infoObject.Data.Stats, infoObject, (int i) => i < 0 ? DataDisplayType.Decrease : DataDisplayType.Default);
		AddInfo(infoObject.Data.BurnEffects, infoObject, (int i) => i < 0 ? DataDisplayType.Decrease : DataDisplayType.Increase, "when burnt");
	}

	private void AddInfo(Dictionary<ResourceType, int> effects, Object obj, Func<int, DataDisplayType> GetDisplayType, string secondary = null) {
		if (!effects.Any()) return; 
		foreach (var resourceType in effects.Keys) {
			var value = effects[resourceType];
			if (value == 0) continue;
			var icon = resourceHandler.GetTypeIcon(resourceType);
			var displayInfo = Instantiate(infoPrefab, obj.ResourceInfoSlot).GetComponent<ObjectDataDisplay>();
			displayInfo.SetUp(GetDisplayType(value), $"{(value < 0 ? "" : "+")}{value}", icon, secondary);
		}

	}
}
