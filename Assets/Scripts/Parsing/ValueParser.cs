using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class ValueParser : MonoBehaviour {
	[SerializeField] TextAsset valueText;
	[SerializeField] TextAsset eventText;
	[Space]
	[SerializeField] ObjectSpawner spawner;
	[SerializeField] ResourceHandler resourceHandler; 
	[SerializeField] FlowHandler flowHandler;
	[SerializeField] TrainHandler train;
	[SerializeField] Repairer repairer;
	[SerializeField] RoadEventHandler roadEventHandler;
	

	public void Initialize() {
		InitializeValues();
		InitializeEvents();
		InitializeProgression();
	}

	public void InitializeValues() {
		var rawValues = Regex.Split(valueText.text, "\n|\r|\r\n");
		var rawSplits = rawValues.Select(x => Regex.Split(x, " |\t"));


		var values = rawSplits.Where(x => x.Length > 1).ToDictionary(x => x[0], x => int.Parse(x[x.Length - 1]));

		spawner.Initialize(
			fuelMaxStacks: values["FuelMaxStacks"],
			passengerReward: values["PassengerReward"],
			fuelPrice: values["FuelPrice"],
			brawlerPrice: values["BrawlerPrice"],
			burnCost: values["NoFuelHpCost"],
			brawlBonus: values["BrawlerBrawlBonus"]
		);
		resourceHandler.Initialize(
			initialGold: values["InitialGold"],
			initialHp: values["InitialHp"]
		);
		train.Initialize(
			spawner: spawner,
			initialPassengers: values["InitialPassengers"],
			initialBrawlers: values["InitialBrawlers"],
			initialFuel: values["InitialFuel"],
			trainSpots: values["TrainSize"]
		);
		repairer.Initialize(repairPerHpPrice: values["RepairPerHpPrice"]);
	}

	public void InitializeEvents() {
		var rawValues = Regex.Split(eventText.text, "\n|\r|\r\n");
		var filteredValues = rawValues
			.Where(s => !string.IsNullOrWhiteSpace(s) && !string.IsNullOrEmpty(s))
			.Select(s => Regex.Split(s, ";"))
			.ToArray();

		var pools = new Dictionary<int, HashSet<RoadEventData>>();
		for (int i= 1; i < filteredValues.Length; ++i) {
			var roadEvent = new RoadEventData();
			Debug.Log(string.Join(" - ", filteredValues[i]));

			roadEvent.Title = filteredValues[i][1];
			roadEvent.Description = filteredValues[i][2];
			
			var choice = new Choice();
			choice.ChoiceDescription = filteredValues[i][5];
			var choiceEffect = new ChoiceEffect();
			switch (filteredValues[i][6]) {
				case "Hp":
					choiceEffect.ChoiceType = ChoiceType.Resource;
					choiceEffect.ResourceType = ResourceType.Health;
					break;
				case "Fuel":
					choiceEffect.ChoiceType = ChoiceType.Object;
					choiceEffect.ObjectVisualType = ObjectVisualType.Box;
					break;
				case "Brawler":
					choiceEffect.ChoiceType = ChoiceType.Object;
					choiceEffect.ObjectVisualType = ObjectVisualType.Guard;
					break;
				case "Passenger":
					choiceEffect.ChoiceType = ChoiceType.Object;
					choiceEffect.ObjectVisualType = ObjectVisualType.Passenger;
					break;
				case "Gold":
					choiceEffect.ChoiceType = ChoiceType.Resource;
					choiceEffect.ObjectVisualType = ObjectVisualType.Guard;
					break;

			}
			choiceEffect.ChoiceValue = int.Parse(filteredValues[i][7]);
			choice.ChoiceEffects.Add(choiceEffect);
			if (!string.IsNullOrEmpty(filteredValues[i][8])){
				choiceEffect = new ChoiceEffect();
				switch (filteredValues[i][8]) {
					case "Hp":
						choiceEffect.ChoiceType = ChoiceType.Resource;
						choiceEffect.ResourceType = ResourceType.Health;
						break;
					case "Fuel":
						choiceEffect.ChoiceType = ChoiceType.Object;
						choiceEffect.ObjectVisualType = ObjectVisualType.Box;
						break;
					case "Brawler":
						choiceEffect.ChoiceType = ChoiceType.Object;
						choiceEffect.ObjectVisualType = ObjectVisualType.Guard;
						break;
					case "Passenger":
						choiceEffect.ChoiceType = ChoiceType.Object;
						choiceEffect.ObjectVisualType = ObjectVisualType.Passenger;
						break;
					case "Gold":
						choiceEffect.ChoiceType = ChoiceType.Resource;
						choiceEffect.ObjectVisualType = ObjectVisualType.Guard;
						break;

				}
				choiceEffect.ChoiceValue = int.Parse(filteredValues[i][9]);
				choice.ChoiceEffects.Add(choiceEffect);
			}
			roadEvent.Choices.Add(choice);
			if (!string.IsNullOrEmpty(filteredValues[i][10])){
				choice = new Choice();
				choice.ChoiceDescription = filteredValues[i][10];
				choiceEffect = new ChoiceEffect();
				switch (filteredValues[i][11]) {
					case "Hp":
						choiceEffect.ChoiceType = ChoiceType.Resource;
						choiceEffect.ResourceType = ResourceType.Health;
						break;
					case "Fuel":
						choiceEffect.ChoiceType = ChoiceType.Object;
						choiceEffect.ObjectVisualType = ObjectVisualType.Box;
						break;
					case "Brawler":
						choiceEffect.ChoiceType = ChoiceType.Object;
						choiceEffect.ObjectVisualType = ObjectVisualType.Guard;
						break;
					case "Passenger":
						choiceEffect.ChoiceType = ChoiceType.Object;
						choiceEffect.ObjectVisualType = ObjectVisualType.Passenger;
						break;
					case "Gold":
						choiceEffect.ChoiceType = ChoiceType.Resource;
						choiceEffect.ObjectVisualType = ObjectVisualType.Guard;
						break;

				}
				choiceEffect.ChoiceValue = int.Parse(filteredValues[i][12]);
				choice.ChoiceEffects.Add(choiceEffect);
				if (!string.IsNullOrEmpty(filteredValues[i][13])) {
					choiceEffect = new ChoiceEffect();
					switch (filteredValues[i][13]) {
						case "Hp":
							choiceEffect.ChoiceType = ChoiceType.Resource;
							choiceEffect.ResourceType = ResourceType.Health;
							break;
						case "Fuel":
							choiceEffect.ChoiceType = ChoiceType.Object;
							choiceEffect.ObjectVisualType = ObjectVisualType.Box;
							break;
						case "Brawler":
							choiceEffect.ChoiceType = ChoiceType.Object;
							choiceEffect.ObjectVisualType = ObjectVisualType.Guard;
							break;
						case "Passenger":
							choiceEffect.ChoiceType = ChoiceType.Object;
							choiceEffect.ObjectVisualType = ObjectVisualType.Passenger;
							break;
						case "Gold":
							choiceEffect.ChoiceType = ChoiceType.Resource;
							choiceEffect.ObjectVisualType = ObjectVisualType.Guard;
							break;

					}
					choiceEffect.ChoiceValue = int.Parse(filteredValues[i][14]);
					choice.ChoiceEffects.Add(choiceEffect);
				}
				roadEvent.Choices.Add(choice);
			}

			int parse = int.Parse(filteredValues[i][3]);
			if (!pools.ContainsKey(parse)) { pools.Add(parse, new HashSet<RoadEventData>()); }
			pools[parse].Add(roadEvent);
		}
		roadEventHandler.AddPools(pools);
	}

	public void InitializeProgression() {
	}
}