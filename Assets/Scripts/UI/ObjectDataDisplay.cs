using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectDataDisplay : MonoBehaviour
{

    [SerializeField] TMP_Text primaryField;
	[SerializeField] Image resource;
	[SerializeField] TMP_Text secondaryField;

	public Object Object { get; private set; }

	string primaryPrefix;
	bool cost;
	int value;
	
	void OnDestroy() {
		if (Object != null) Object.StacksUpdated -= SetPrimaryString;
	}

	public void SetPrimaryString() {
		var finalvalue = (cost ? (Object.Stacks - Object.PurchasedStacks) * value : value * Object.Stacks);
		gameObject.SetActive(finalvalue != 0);
		primaryField.text = primaryPrefix + (cost ? (Object.Stacks - Object.PurchasedStacks) * value : value * Object.Stacks);
	}

	public void SetUp(Object obj, DataDisplayType displayType, string primaryPrefix, int value, Sprite icon, string secondary = null, bool cost = false) {
		this.value = value;
		this.cost = cost;
		this.primaryPrefix = primaryPrefix;
		Object = obj;
		Object.StacksUpdated += SetPrimaryString;
		SetPrimaryString();
		if (secondary == null) secondaryField.gameObject.SetActive(false);
		else secondaryField.text = secondary;
		if (icon == null) resource.gameObject.SetActive(false);
		else resource.sprite = icon;

		switch (displayType) {
			case DataDisplayType.Decrease:
				primaryField.color = Color.red;
				secondaryField.color = Color.red;
				break;
			case DataDisplayType.Increase:
				primaryField.color = Color.green;
				secondaryField.color = Color.green;
				break;
			default:
				primaryField.color = Color.white;
				secondaryField.color = Color.white;
				break;
		}
	}
   
}
