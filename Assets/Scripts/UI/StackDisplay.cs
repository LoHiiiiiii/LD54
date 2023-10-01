using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StackDisplay : MonoBehaviour {

	[SerializeField] Object displayObject;
	[SerializeField] TMP_Text textField;
	

	void Start () {
		displayObject.StacksUpdated += UpdateStacks;
		UpdateStacks();
	}

	private void OnDestroy() {
		displayObject.StacksUpdated -= UpdateStacks;
	}

	private void UpdateStacks () {
		if (displayObject == null || displayObject.Data?.MaxStacks == 1) {
			textField.gameObject.SetActive(false);
			return;
		}
		if (!textField.gameObject.activeSelf) { textField.gameObject.SetActive(true); }
		if (displayObject.Data?.MaxStacks == null) {
			textField.text = displayObject.Stacks.ToString();
		} else {
			textField.text = $"{displayObject.Stacks} / {displayObject.Data.MaxStacks}";
		}
	}
}
