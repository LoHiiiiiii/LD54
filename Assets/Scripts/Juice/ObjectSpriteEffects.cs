using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpriteEffects : MonoBehaviour {
	[SerializeField] Object affectedObject;
	[SerializeField] float selectedScale;
	[SerializeField] Color invalidColor;
	[SerializeField] float duration;

	Vector3 startScale;
	float scaleLerp;
	float colorLerp;
	bool valid = true;
	bool selected = false;
	Color startColor;
	new SpriteRenderer renderer;


	private void Start() {
		affectedObject.SelectedChanged += HandleSelected;
		affectedObject.ValidChanged += HandleValid;
		startScale = transform.localScale;
		renderer = GetComponent<SpriteRenderer>();
		startColor = renderer.color;
	}

	private void OnDestroy() {
		affectedObject.SelectedChanged -= HandleSelected;
		affectedObject.ValidChanged -= HandleValid;
	}

	private void HandleSelected(bool selected) => this.selected = selected;

	private void HandleValid(bool valid) => this.valid = valid;

	private void Update() {
		scaleLerp = selected ? Mathf.Clamp01(scaleLerp + 1f / duration * Time.deltaTime) : Mathf.Clamp01(scaleLerp - 1f / duration * Time.deltaTime);
		transform.localScale = Vector3.Lerp(startScale, startScale * selectedScale, scaleLerp);
		colorLerp = valid ? Mathf.Clamp01(colorLerp - 1f / duration * Time.deltaTime) : Mathf.Clamp01(colorLerp + 1f / duration * Time.deltaTime);
		renderer.color = Color.Lerp(startColor, invalidColor, colorLerp);
	}
}
