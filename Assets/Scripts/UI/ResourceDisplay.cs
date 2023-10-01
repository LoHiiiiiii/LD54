using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceDisplay : MonoBehaviour {


	[SerializeField] TMP_Text amountField;
	[SerializeField] Image resourceIcon;
	[SerializeField] TMP_Text targetField;
	[SerializeField] GameObject arrowIcon;

	Resource resource;
	int amount;
	int target;

	public void SetResource(Resource resource) {
		this.resource = resource;
		target = resource.Target;
		amount = resource.Amount;
		TargetChange();
		HandleAmount();
		resourceIcon.sprite = resource.Data.icon;
		resource.TargetChanged += TargetChange;
		resource.AmountChanged += HandleAmount;
	}

	private void OnDestroy() {
		if (resource != null) {
			resource.TargetChanged -= TargetChange;
			resource.AmountChanged += HandleAmount;
		}
	}

	private void TargetChange() {
		target = resource.Target;
		ToggleTarget();
		targetField.text = target.ToString();
		targetField.color = target < amount ? Color.red : Color.green;
	}
	private void HandleAmount() {
		//TODO: Juice
		amount = resource.Amount;
		ToggleTarget();
		amountField.text = amount.ToString();
	}

	private void ToggleTarget() {
		if (target == amount) {
			targetField.gameObject.SetActive(false);
			arrowIcon.SetActive(false);
			return;
		}
		targetField.gameObject.SetActive(true);
		arrowIcon.SetActive(true);
	}
}
