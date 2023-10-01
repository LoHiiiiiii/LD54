using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationSpot : MonoBehaviour {

    public Object CurrentObject { get; set; }
    public LocationRow Row { get; set; }
	public int Index { get; set; }
	public bool Blocked { get; set; }

    public int[] BlockingIndices { get; set; }


    public event Action<Object, bool> ObjectChanged;

	public bool TryAddObject(Object o) {
        if (CurrentObject == o) return true;
        if (!o.FitsSpot(this)) return false;
        var previous = CurrentObject;
        if (CurrentObject != null) {
            Destroy(o.gameObject); // TODO: Juice
            CurrentObject.Stacks++;
            if (CurrentObject?.Data.MaxStacks != null) CurrentObject.Stacks = Mathf.Min((int)CurrentObject.Data.MaxStacks, CurrentObject.Stacks);
        } else {
            if (o.CurrentSpot != null) {
                o.CurrentSpot.CurrentObject = null;
                o.CurrentSpot.InvokeObjectChanged(o, false);
            }
            o.CurrentSpot = this;
            CurrentObject = o;
        }
        InvokeObjectChanged(previous, true);
        return true;
    }

    public void InvokeObjectChanged(Object previous, bool selfActivated) => ObjectChanged?.Invoke(previous, selfActivated);
}
