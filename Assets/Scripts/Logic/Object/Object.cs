using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object : MonoBehaviour {

	[SerializeField] new SpriteRenderer renderer;

	Vector3 targetPosition;
	bool valid = true;
	bool dragging = false;

	public LocationSpot CurrentSpot { get; set; }

	private int stacks = 1;
	public int Stacks { 
		get => stacks; 
		set {
			stacks = value;
			StacksUpdated?.Invoke();
		}
	}


	public ObjectData Data { get; set; }
	public ObjectVisualData VisualData { get; set; }

	public event Action<bool> HoverChanged;
	public event Action<bool> SelectedChanged;
	public event Action<bool> ValidChanged;
	public event Action StacksUpdated;


	private void Start() {
		name = VisualData.name;
		renderer.sprite = VisualData.sprite;
	}

	public void StartHover() { HoverChanged?.Invoke(true); }
	public void StopHover() { HoverChanged?.Invoke(false); }
	
	public void StartHoveringOver(LocationSpot spot) {
		valid = FitsSpot(spot);
		if (!valid) ValidChanged?.Invoke(false); 
	}
	public void StopHoveringOver() {
		if (!valid) ValidChanged?.Invoke(true);
		valid = true;
	}
	public void StartDragging(Vector3 clickPoint) {
		SelectedChanged?.Invoke(true);
		dragging = true;
		targetPosition = transform.position;
	}
	public void StopDragging() {
		SelectedChanged?.Invoke(false); 
		dragging = false;
		targetPosition = CurrentSpot?.transform.position ?? Vector3.zero;
	}

	public void SetDragPosition(Vector3 position) {
		targetPosition =  position;
	}
	public bool FitsSpot(LocationSpot spot) {
		if (spot.CurrentObject == this) return true;
		return FitsSpot(spot, Data, Stacks);
	}

	public static bool FitsSpot(LocationSpot spot, ObjectData data, int stacks) {
		if (spot.CurrentObject != null && spot.CurrentObject.Data == data && spot.CurrentObject.Stacks + stacks <= data.MaxStacks) {
			return true;
		}
		return spot.CurrentObject == null;
	}

	private void LateUpdate() {
		MoveTowardsTarget();
	}

	private void MoveTowardsTarget() {
		transform.position = (dragging || CurrentSpot == null) ? targetPosition : CurrentSpot.transform.position; //TODO: Juice
	}
}
