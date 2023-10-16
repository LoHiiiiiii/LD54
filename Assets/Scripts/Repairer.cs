using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Repairer : MonoBehaviour {

	[SerializeField] private TMP_Text priceField;
	[SerializeField] private ResourceHandler resourceHandler;

	int price;

	private void Start() {
		GetComponent<Clickable>().Clicked += HandleClick;
	}

	private void HandleClick() {
		var gold = resourceHandler.GetResource(ResourceType.Gold);
		var health = resourceHandler.GetResource(ResourceType.Health);
		if (gold.Target < price) return;
		if (health.Maximum <= health.Target) return;
		resourceHandler.GetResource(ResourceType.Gold).Target -= price;
		resourceHandler.GetResource(ResourceType.Health).Target ++;
	}

	public void Initialize(int repairPerHpPrice) {
		price = repairPerHpPrice;
		priceField.text = $"-{price}";
	}
}
