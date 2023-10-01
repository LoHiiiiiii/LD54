using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object : MonoBehaviour {


	Vector3 dragOffset;
	Vector3 targetPosition;
	Vector3 defaultPosition;
	bool valid = true;

	public LocationSpot CurrentSpot { get; set; }

	private int stacks = 1;
	public int Stacks { 
		get => stacks; 
		set {
			stacks = value;
			StacksUpdated?.Invoke();
		}
	}

	static ObjectData common = new ObjectData() { MaxStacks = 2};

	public ObjectData Data { get; set; } = common; //TODO: Change to passing

	public event Action<bool> HoverChanged;
	public event Action<bool> SelectedChanged;
	public event Action<bool> ValidChanged;
	public event Action StacksUpdated;


	private void Start() {
		defaultPosition = transform.position;
		targetPosition = defaultPosition;
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
		dragOffset = transform.position - clickPoint;
	}
	public void StopDragging() {
		SelectedChanged?.Invoke(false);
		targetPosition = CurrentSpot?.transform.position ?? defaultPosition;
	}
	public void SetDragPosition(Vector3 position) {
		targetPosition = dragOffset + position;
	}
	public bool FitsSpot(LocationSpot spot) {
		if (spot.CurrentObject == this) return true;
		if (spot.CurrentObject != null && spot.CurrentObject.Data == Data && spot.CurrentObject.Stacks < Data.MaxStacks) {
			return true;
		}
		return spot.CurrentObject == null;
	}

	private void LateUpdate() {
		MoveTowardsTarget();
	}

	private void MoveTowardsTarget() {
		transform.position = targetPosition; //TODO: Juice
	}
}
