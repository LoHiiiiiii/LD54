using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationSpot : MonoBehaviour {

	[SerializeField] SpriteRenderer hoverGlow;
	[SerializeField] float glowTransitionDuration;
	[SerializeField] Transform pivot;

	public Transform Pivot { get => pivot; }
	public Object CurrentObject { get; set; }
	public LocationRow Row { get; set; }

	bool hovering;

	public void Start() {
		hoverGlow = GetComponentInChildren<SpriteRenderer>();
	}

	private void Update() {
		if (hoverGlow != null) {
			if (hovering && hoverGlow.color.a < 1) {
				float a = Mathf.Min(hoverGlow.color.a + 1 / glowTransitionDuration, 1);
				hoverGlow.color = new Color(hoverGlow.color.r, hoverGlow.color.g, hoverGlow.color.b, a);
			}
			if (!hovering && hoverGlow.color.a > 0) {
				float a = Mathf.Max(hoverGlow.color.a - 1 / glowTransitionDuration, 0);
				hoverGlow.color = new Color(hoverGlow.color.r, hoverGlow.color.g, hoverGlow.color.b, a);
			}
		}
	}

	public bool TryAddObject(Object o) {
		if (CurrentObject == o) return false;
		if (!o.FitsSpot(this)) return false;
		var previous = CurrentObject;
		var previousSpot = o.CurrentSpot;
		if (CurrentObject != null) {
			var stackIncrease = (CurrentObject.Data.MaxStacks == null) ? o.Stacks
				: Mathf.Min((int)CurrentObject.Data.MaxStacks - CurrentObject.Stacks, o.Stacks);

			if (stackIncrease <= 0) return false;
			CurrentObject.PurchasedStacks += Mathf.Min(stackIncrease, o.PurchasedStacks);
			CurrentObject.Stacks += stackIncrease;
			o.PurchasedStacks -= Mathf.Min(stackIncrease, o.PurchasedStacks);
			o.Stacks -= stackIncrease;
			if ( previousSpot != null && previousSpot.Row.Location != Row.Location) previousSpot.Row.StacksChanged(o, -stackIncrease);
			if (previousSpot == null || previousSpot.Row.Location != Row.Location) Row.StacksChanged(CurrentObject, stackIncrease);
			return false;
		} else {
			o.CurrentSpot = this;
			CurrentObject = o;
			if (previousSpot != null) { previousSpot.ClearSpot(); }
		}
		Row.ObjectChanged(previousSpot, previous, CurrentObject);
		return true;
	}

	public void ClearSpot() {
		if (CurrentObject == null) return;
		var previous = CurrentObject;
		CurrentObject = null;
		Row.ObjectChanged(this, previous, null);
	}

	public void StartHovering() => hovering = true;

	public void StopHovering() => hovering = false;
}
