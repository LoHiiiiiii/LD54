using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationSpot : MonoBehaviour {

    public Object CurrentObject { get; set; }
    public LocationRow Row { get; set; }

	public bool TryAddObject(Object o) {
        if (CurrentObject == o) return false;
        if (!o.FitsSpot(this)) return false;
        var previous = CurrentObject;
        var previousSpot = o.CurrentSpot;
        if (CurrentObject != null) {
            var stackIncrease = (CurrentObject.Data.MaxStacks == null) ? o.Stacks
                : Mathf.Min((int)CurrentObject.Data.MaxStacks - CurrentObject.Stacks, o.Stacks);

            if (stackIncrease <= 0) return false;
            CurrentObject.Stacks += stackIncrease;
            if (previousSpot != null) { o.CurrentSpot.Row.StacksChanged(o, -stackIncrease); }
            o.Stacks -= stackIncrease;
            Row.StacksChanged(CurrentObject, stackIncrease);
            return true;
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

    public void StartHovering() {

    }

    public void StopHovering() {

    }
}
