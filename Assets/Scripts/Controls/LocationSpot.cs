using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationSpot : MonoBehaviour {

    public Object CurrentObject { get; set; }
    public LocationRow Row { get; }


    public event Action ObjectChanged;

    public bool TryAddObject(Object o) {
        if (!o.FitsSpot(this)) return false;
        if (CurrentObject != null) {
            Destroy(o.gameObject); // TODO: Juice
            CurrentObject.Stacks++;
            if (CurrentObject?.Data.MaxStacks != null) CurrentObject.Stacks = Mathf.Min((int)CurrentObject.Data.MaxStacks, CurrentObject.Stacks);
        } else {
            if (o.CurrentSpot != null) o.CurrentSpot.CurrentObject = null;
            o.CurrentSpot = this;
            CurrentObject = o;
        }
        ObjectChanged?.Invoke();
        return true;
    }
}
