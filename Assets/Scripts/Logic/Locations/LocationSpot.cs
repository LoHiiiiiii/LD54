using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationSpot : MonoBehaviour {

    public Object CurrentObject { get; set; }
    public LocationRow Row { get; set; }

	public bool TryAddObject(Object o) {
        if (CurrentObject == o) return true;
        if (!o.FitsSpot(this)) return false;
        var previous = CurrentObject;
        if (CurrentObject != null) {
            var stackIncrease = (CurrentObject.Data.MaxStacks == null) ? o.Stacks
                : Mathf.Min((int)CurrentObject.Data.MaxStacks - CurrentObject.Stacks, o.Stacks);

            if (stackIncrease <= 0) return false;
            CurrentObject.Stacks += stackIncrease;
            o.Stacks -= stackIncrease;
            if (o.CurrentSpot != null) {
                o.CurrentSpot.Row.StacksChanged(o, -stackIncrease);
                if (o.Stacks == 0) {
                    o.CurrentSpot.ClearSpot();
                    Destroy(o);
                }
            }
            Row.StacksChanged(CurrentObject, stackIncrease);
            return true;
        } else {
            if (o.CurrentSpot != null) {
                o.CurrentSpot.ClearSpot();
            }
            o.CurrentSpot = this;
            CurrentObject = o;
        }
        Row.ObjectChanged(previous, CurrentObject);
        return true;
    }

    public void ClearSpot() {
        if (CurrentObject == null) return;
        var previous = CurrentObject;
        CurrentObject = null;
        Row.ObjectChanged(previous, null);
    }
}
