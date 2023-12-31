using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour {
	[SerializeField] ResourceHandler resourceHandler;
	[SerializeField] GameObject objectPrefab;
	[SerializeField] GameObject infoPrefab;

	[SerializeField] ObjectVisualCollection visualCollection;

	Dictionary<ObjectVisualType, ObjectVisualData> visualData;
	Dictionary<ObjectVisualType, ObjectData> objectData;

	public ObjectData GetData(ObjectVisualType type) => objectData[type];

	public void Initialize(int fuelMaxStacks, int passengerReward, int fuelPrice, int brawlerPrice, int burnCost, int brawlBonus) {

		objectData = new Dictionary<ObjectVisualType, ObjectData>();
		visualData = new Dictionary<ObjectVisualType, ObjectVisualData>();
		foreach (var visual in visualCollection.objectVisuals) {
			var data = new ObjectData();
			data.VisualType = visual.type;
			switch (visual.type) {
				case ObjectVisualType.Box:
					data.MaxStacks = fuelMaxStacks;
					data.AddEffects.Add(ResourceType.Gold, -fuelPrice);
					data.Stats.Add(ResourceType.Fuel, 1);
					break;
				case ObjectVisualType.Passenger:
					data.ExitEffects.Add(ResourceType.Gold, passengerReward);
					data.BurnEffects.Add(ResourceType.Health, -burnCost);
					break;
				case ObjectVisualType.Guard:
					data.DepartureEffects.Add(ResourceType.Gold, -brawlerPrice);
					data.BurnEffects.Add(ResourceType.Health, -burnCost);
					data.Stats.Add(ResourceType.Brawn, brawlBonus);
					break;
			}
			objectData.Add(visual.type, data);
			visualData.Add(visual.type, visual);
		}
	}

	public Object SpawnObject(ObjectVisualType visualType, LocationSpot spot, bool purchased = false) {
		var data = objectData[visualType];
		if (!Object.FitsSpot(spot, data, 1)) return null;
		var newObject = Instantiate(objectPrefab, spot.transform).GetComponent<Object>();
		newObject.name = visualData[data.VisualType].name;
		newObject.Data = data;
		newObject.VisualData = visualData[data.VisualType];
		if (purchased) newObject.PurchasedStacks = newObject.Stacks;
		if (!spot.TryAddObject(newObject)) { Destroy(newObject.gameObject); }
		AddDataInfo(newObject);
		return newObject;
	}

	public void AddDataInfo(Object infoObject) {
		AddInfo(infoObject.Data.Stats, infoObject, (int i) => i < 0 ? DataDisplayType.Decrease : DataDisplayType.Default);
		AddInfo(infoObject.Data.ExitEffects, infoObject, (int i) => DataDisplayType.Default,"", "on next station");
		AddInfo(infoObject.Data.DepartureEffects, infoObject, (int i) => i < 0 ? DataDisplayType.Decrease : DataDisplayType.Increase,"", "per stop");
		AddInfo(infoObject.Data.BurnEffects, infoObject, (int i) => i < 0 ? DataDisplayType.Decrease : DataDisplayType.Increase,"", "when burnt");
		AddInfo(infoObject.Data.AddEffects, infoObject, (int i) => DataDisplayType.Default, "Cost: ", null, true);
	}

	private void AddInfo(Dictionary<ResourceType, int> effects, Object obj, Func<int, DataDisplayType> GetDisplayType, string pretext = null, string secondary = null, bool cost = false) {
		if (!effects.Any()) return; 
		foreach (var resourceType in effects.Keys) {
			var value = effects[resourceType];
			if (value == 0) continue;
			var icon = resourceHandler.GetTypeIcon(resourceType);
			var displayInfo = Instantiate(infoPrefab, obj.ResourceInfoSlot).GetComponent<ObjectDataDisplay>();
			displayInfo.SetUp(obj, GetDisplayType(value), $"{pretext}{(value < 0 || cost ? "" : "+")}", (cost ? -value : value), icon, secondary, cost);
		}

	}
}
