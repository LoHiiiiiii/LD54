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

	public void SetUp(DataDisplayType type, string primary, Sprite icon, string secondary = null) {
		primaryField.text = primary;
		if (secondary == null) secondaryField.gameObject.SetActive(false);
		else secondaryField.text = secondary;
		if (icon == null) resource.gameObject.SetActive(false);
		else resource.sprite = icon;

		switch (type) {
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
