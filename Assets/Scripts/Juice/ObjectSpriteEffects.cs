using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Rendering;

public class ObjectSpriteEffects : MonoBehaviour {
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
	Color[] startColors;
	SpriteRenderer[] renderers;
	SortingGroup sortingGroup;
	Canvas[] canvasLayers;
	string startingLayer; 
	Object affectedObject;

	private const string canvasLayer = "UI";
	private const string selectedLayer = "Selected";
	private const string selectedCanvasLayer = "SelectedUI";

	private void Start() {
		affectedObject = GetComponentInParent<Object>();
		affectedObject.SelectedChanged += HandleSelected;
		affectedObject.ValidChanged += HandleValid;
		affectedObject.HoverChanged += HandleHover;
		startScale = transform.localScale;
		renderers = GetComponentsInChildren<SpriteRenderer>();
		sortingGroup = GetComponentInChildren<SortingGroup>();
		startColors = renderers.Select(r => r.color).ToArray();
		startingLayer = sortingGroup.sortingLayerName;
		canvasLayers = GetComponentsInChildren<Canvas>();
	}

	private void OnDestroy() {
		affectedObject.SelectedChanged -= HandleSelected;
		affectedObject.ValidChanged -= HandleValid;
	}

	private void HandleSelected(bool selected) {
		this.selected = selected;
		sortingGroup.sortingLayerName = selected ? selectedLayer : startingLayer;
		foreach(var canvas in  canvasLayers) {
			canvas.sortingLayerName = selected ? selectedCanvasLayer : canvasLayer;
		}
	}

	private void HandleHover(bool hover) => this.hover = hover;

	private void HandleValid(bool valid) => this.valid = valid;

	private void Update() {
		scaleLerp = selected || hover ? Mathf.Clamp(scaleLerp + 1f / duration * Time.deltaTime, 0, selected ? 1 : hoverScaleRatio) : Mathf.Clamp01(scaleLerp - 1f / duration * Time.deltaTime);
		transform.localScale = Vector3.Lerp(startScale, startScale * selectedScale, scaleLerp);
		colorLerp = valid ? Mathf.Clamp01(colorLerp - 1f / duration * Time.deltaTime) : Mathf.Clamp01(colorLerp + 1f / duration * Time.deltaTime);
		for (int i = 0; i < renderers.Length; ++i) {
			renderers[i].color = Color.Lerp(startColors[i], invalidColor, colorLerp);
		}
	}
}
