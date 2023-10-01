using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class SpotStackDisplay : MonoBehaviour {

	[SerializeField] LocationSpot spot;
	[SerializeField] TMP_Text textField;
	

	void Start () {
		spot.ObjectChanged += UpdateStacks;
	}

	private void OnDestroy() {
		spot.ObjectChanged -= UpdateStacks;
	}

	private void UpdateStacks () {
		if (spot.CurrentObject == null || spot.CurrentObject.Data?.MaxStacks == 1) {
			textField.gameObject.SetActive(false);
			return;
		}
		if (!textField.gameObject.activeSelf) { textField.gameObject.SetActive(true); }
		if (spot.CurrentObject.Data?.MaxStacks == null) {
			textField.text = spot.CurrentObject.Stacks.ToString();
		} else {
			textField.text = $"{spot.CurrentObject.Stacks} / {spot.CurrentObject.Data.MaxStacks}";
		}

	}
}
