using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object : MonoBehaviour {

	Vector3 dragOffset = Vector3.zero;
	Vector3 targetPosition;
	Vector3 defaultPosition;
	private bool valid = true;

	public LocationSpot CurrentSpot { get; set; }
	public int Stacks = 1;

	public ObjectData Data { get; }

	private void Start() {
		defaultPosition = transform.position;
		targetPosition = defaultPosition;
	}

	public event Action<bool> SelectedChanged;
	public event Action<bool> ValidChanged;


	public void StartHover() { SelectedChanged?.Invoke(true); }
	public void StopHover() { SelectedChanged?.Invoke(false); }
	
	public void StartHoveringOver(LocationSpot spot) {
		valid = FitsSpot(spot);
		if (!valid) ValidChanged?.Invoke(false); 
	}
	public void StopHoveringOver() {
		if (!valid) ValidChanged?.Invoke(true);
		valid = true;
		Debug.Log("Stopped hovering over");
	}
	public void StartDragging(Vector3 clickPoint) {
		dragOffset = transform.position - clickPoint;
	}
	public void StopDragging() {
		targetPosition = CurrentSpot?.transform.position ?? defaultPosition;
	}
	public void SetDragPosition(Vector3 position) {
		targetPosition = dragOffset + position;
	}
	public bool FitsSpot(LocationSpot spot) => spot.CurrentObject == null || spot.CurrentObject == this;

	private void LateUpdate() {
		MoveTowardsTarget();
	}

	private void MoveTowardsTarget() {
		transform.position = targetPosition; //TODO: Juice
	}

}
