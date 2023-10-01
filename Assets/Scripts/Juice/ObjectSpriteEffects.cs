using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpriteEffects : MonoBehaviour {
	[SerializeField] Object affectedObject;
	[SerializeField] float selectedScale;
	[SerializeField] float hoverScaleRatio;
	[SerializeField] Color invalidColor;
	[SerializeField] float duration;

	Vector3 startScale;
	float scaleLerp;
	float colorLerp;
	bool valid = true;
	bool selected = false;
	bool hover = false;
	Color startColor;
	new SpriteRenderer renderer;


	private void Start() {
		affectedObject.SelectedChanged += HandleSelected;
		affectedObject.ValidChanged += HandleValid;
		affectedObject.HoverChanged += HandleHover;
		startScale = transform.localScale;
		renderer = GetComponent<SpriteRenderer>();
		startColor = renderer.color;
	}

	private void OnDestroy() {
		affectedObject.SelectedChanged -= HandleSelected;
		affectedObject.ValidChanged -= HandleValid;
	}

	private void HandleSelected(bool selected) {
		this.selected = selected;
		renderer.sortingOrder = selected ? 1 : 0;
	}

	private void HandleHover(bool hover) => this.hover = hover;

	private void HandleValid(bool valid) => this.valid = valid;

	private void Update() {
		scaleLerp = selected || hover ? Mathf.Clamp(scaleLerp + 1f / duration * Time.deltaTime, 0, selected ? 1 : hoverScaleRatio) : Mathf.Clamp01(scaleLerp - 1f / duration * Time.deltaTime);
		transform.localScale = Vector3.Lerp(startScale, startScale * selectedScale, scaleLerp);
		colorLerp = valid ? Mathf.Clamp01(colorLerp - 1f / duration * Time.deltaTime) : Mathf.Clamp01(colorLerp + 1f / duration * Time.deltaTime);
		renderer.color = Color.Lerp(startColor, invalidColor, colorLerp);
	}
}
