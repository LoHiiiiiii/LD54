using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object : MonoBehaviour {

	[SerializeField] Transform spriteSlot;
	[SerializeField] Transform resourceInfoSlot;
	[SerializeField] float maxSpeed;
	[SerializeField] float secondsToMax;
	[SerializeField] float deleteDuration;

	Vector3 targetPosition;
	bool valid = true;
	bool dragging = false;
	float speed = 0;
	new Collider2D collider;

	public LocationSpot CurrentSpot { get; set; }
	public int PurchasedStacks { get; set; } = 0;

	public bool Deleting { get; private set; }
	public float RemainingDeleteRatio { get; private set; } = 1;
	public Transform ResourceInfoSlot { get => resourceInfoSlot; }

	public bool AtTarget { get; private set; }


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
		name = VisualData.name;
		Instantiate(VisualData.spritePrefab, spriteSlot);
		stacks = Data.MaxStacks ?? stacks;
		StacksUpdated += HandleNoStacks;
		transform.position = (dragging || CurrentSpot == null) ? targetPosition : CurrentSpot.transform.position;
		collider = GetComponentInChildren<Collider2D>();
	}

	private void Destroy() {
		StacksUpdated -= HandleNoStacks;
	}

	public void StartHover() {
		HoverChanged?.Invoke(true);
		ResourceInfoSlot.gameObject.SetActive(true);
	}
	public void StopHover() {
		HoverChanged?.Invoke(false);
		ResourceInfoSlot.gameObject.SetActive(false);
	}

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
		if (!Deleting) targetPosition = CurrentSpot?.transform.position ?? Vector3.zero;
	}

	public void SetDragPosition(Vector3 position) {
		targetPosition = position;
	}
	public bool FitsSpot(LocationSpot spot) {
		if (spot.CurrentObject == this) return true;
		return FitsSpot(spot, Data, Stacks);
	}

	public static bool FitsSpot(LocationSpot spot, ObjectData data, int stacks) {
		if (spot.CurrentObject != null && spot.CurrentObject.Data == data && spot.CurrentObject.Stacks < data.MaxStacks) {
			return true;
		}
		return spot.CurrentObject == null;
	}

	private void LateUpdate() {
		MoveTowardsTarget();
		TryDie();
	}

	private void MoveTowardsTarget() {
		if (dragging) {
			transform.position = targetPosition;
			speed = 0;
			AtTarget = true;
			return;
		}
		speed += maxSpeed / secondsToMax * Time.deltaTime;
		Vector3 target = CurrentSpot == null ? targetPosition : CurrentSpot.transform.position; //TODO: Juice
		transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
		AtTarget = Vector3.Distance(transform.position, target) <= Mathf.Epsilon;
	}

	private void HandleNoStacks() {
		if (Stacks <= 0) Delete(DeleteType.Stacked);
	}

	public void Delete(DeleteType deleteType = DeleteType.None) {
		if (Deleting) return;
		collider.enabled = false;
		if (CurrentSpot != null) {
			CurrentSpot.ClearSpot();
			targetPosition = CurrentSpot.transform.position;
		}
		CurrentSpot = null;
		Deleting = true;
	}

	private void TryDie() {
		if (!Deleting || !AtTarget) return;
		RemainingDeleteRatio -= Time.deltaTime / deleteDuration;
		if (RemainingDeleteRatio <= 0) {
			Destroy(gameObject);
		}
	}
}
