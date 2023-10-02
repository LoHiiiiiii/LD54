using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class ValueParser : MonoBehaviour {
	[SerializeField] TextAsset valueText;
	[SerializeField] ObjectSpawner spawner;
	[SerializeField] ResourceHandler resourceHandler;
	[SerializeField] TrainHandler train;
	[SerializeField] Repairer repairer;


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
}
