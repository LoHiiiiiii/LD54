using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class ValueParser : MonoBehaviour {
	[SerializeField] TextAsset valueText;
	[SerializeField] TextAsset eventText;
	[SerializeField] TextAsset progressText;
	[Space]
	[SerializeField] ObjectSpawner spawner;
	[SerializeField] ResourceHandler resourceHandler;
	[SerializeField] OrganizationEventHandler organizationHandler;
	[SerializeField] Repairer repairer;
	[SerializeField] GameEventContainer gameEventContainer;


	public void Initialize() {
		InitializeValues();
		InitializeRandomEvents();
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
		organizationHandler.Initialize(
			spawner: spawner,
			initialPassengers: values["InitialPassengers"],
			initialBrawlers: values["InitialBrawlers"],
			initialFuel: values["InitialFuel"],
			trainSpots: values["TrainSize"]
		);
		repairer.Initialize(repairPerHpPrice: values["RepairPerHpPrice"]);
	}

	public void InitializeRandomEvents() {
		var rawValues = Regex.Split(eventText.text, "\n|\r|\r\n");
		var filteredValues = rawValues
			.Where(s => !string.IsNullOrWhiteSpace(s) && !string.IsNullOrEmpty(s))
			.Select(s => Regex.Split(s, ";"))
			.ToArray();

		var pools = new Dictionary<int, Dictionary<ChoiceEvent, int>>();
		for (int i = 1; i < filteredValues.Length; ++i) {
			if (string.IsNullOrEmpty(filteredValues[i][3])) continue;
			var roadEvent = new ChoiceEvent();

			roadEvent.Title = filteredValues[i][1];
			roadEvent.Description = filteredValues[i][2];

			roadEvent.Choices.AddRange(GetChoices(filteredValues[i], 5));
			var pool = int.Parse(filteredValues[i][3]);
			var weight = int.Parse(filteredValues[i][4]);
			roadEvent.CountedEvent = true;
			roadEvent.ViewIndex = 0;
			roadEvent.View = View.Abyss;
			roadEvent.NewView = true;

			if (!pools.ContainsKey(pool)) { pools.Add(pool, new Dictionary<ChoiceEvent, int>()); }
			pools[pool].Add(roadEvent, weight);
		}
		gameEventContainer.SetEventPools(pools);
	}

	public List<Choice> GetChoices(string[] values, int startIndex) {
		var choices = new List<Choice>();
		int index;
		int i = 0;
		while (true) {
			index = startIndex + i * 5;
			if (index >= values.Length || string.IsNullOrEmpty(values[index])) {
				while (choices.Count < 0) {
					choices.Add(new Choice());
				}
				break;
			}
			var choice = new Choice();
			choice.ChoiceDescription = values[index];
			for (int j = 0; j < 2; ++j) {
				index = startIndex + i * 5 + j * 2 + 1;
				if (string.IsNullOrEmpty(values[index])) break;
				var choiceEffect = new ChoiceEffect();
				switch (values[index]) {
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
					case "Brawl":
						choiceEffect.ChoiceType = ChoiceType.Brawl;
						break;

				}
				choiceEffect.ChoiceValue = int.Parse(values[index + 1]);
				choice.ChoiceEffects.Add(choiceEffect);
			}
			choice.ChoiceEffects.OrderBy(e => e.ChoiceValue);
			choices.Add(choice);
			i++;
		}

		return choices;
	}

	public void InitializeProgression() {
		var rawValues = Regex.Split(progressText.text, "\n|\r|\r\n");
		var filteredValues = rawValues
			.Where(s => !string.IsNullOrWhiteSpace(s) && !string.IsNullOrEmpty(s))
			.Select(s => Regex.Split(s, ";"))
			.ToArray();
		View? previous = null;
		var orgEvents = new Dictionary<int, GameEvent>();
		var order = new List<(GameEventType eventType, int index)>();
		for(int i = 1; i < filteredValues.Length; i++) {
			var value = filteredValues[i];
			var gameEvent = new GameEvent();
			if (string.IsNullOrEmpty(value[0])) {
				gameEvent.View = (View)previous;
				gameEvent.NewView = false;
			} else {
				gameEvent.NewView = true;
				var parsed = int.Parse(value[0]);
				if (parsed == 0) gameEvent.View = View.Abyss;
				else gameEvent.View = View.Station;
				gameEvent.ViewIndex = parsed;
				previous = gameEvent.View;
			}

			if (!string.IsNullOrEmpty(value[1])) {
				gameEvent.Type = GameEventType.Organization;
				if (!orgEvents.ContainsKey(gameEvent.ViewIndex))
					orgEvents.Add(gameEvent.ViewIndex, gameEvent);
				order.Add((gameEvent.Type, gameEvent.ViewIndex));
			} else if (!string.IsNullOrEmpty(value[2])) {
				continue;
			} else if (!string.IsNullOrEmpty(value[3])){
				int parsed = int.Parse(value[3]);
				gameEvent.Type = GameEventType.Random;
				order.Add((gameEvent.Type, parsed));
			}

		}
		var orgArray = new GameEvent[orgEvents.Count];
		for (int i = 0; i < orgArray.Length; ++i) {
			orgArray[i] = orgEvents.ContainsKey(i) ? orgEvents[i] : null;
		}
		gameEventContainer.SetOrganizationEvents(orgArray);
		gameEventContainer.SetEventOrder(order.ToArray());
	}
}