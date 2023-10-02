using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class ValueParser : MonoBehaviour {
	[SerializeField] TextAsset valueText;
	[SerializeField] ObjectSpawner spawner;
	[SerializeField] ResourceHandler resourceHandler;

	public void InitializeValues() {
		var rawValues = Regex.Split(valueText.text, "\n|\r|\r\n");
		var rawSplits = rawValues.Select(x => Regex.Split(x, " "));


		var values = rawSplits.Where(x => x.Length > 1).Select(x => int.Parse(x[x.Length-1])).ToArray();

		spawner.Initialize(
			fuelMaxStacks: values[1],
			passengerReward: values[3],
			fuelPrice: values[4],
			brawlerPrice: values[5],
			burnCost: values[6],
			brawlBonus: values[17]
		);
		resourceHandler.Initialize(
			initialGold: values[7],
			initialHp: values[11]
		);
	}
}
