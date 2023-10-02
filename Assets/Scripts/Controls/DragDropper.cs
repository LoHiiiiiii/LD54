using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class DragDropper : MonoBehaviour {

	new private Camera camera;

	public Object DragObject { get; private set; }
	public bool Active { get; set; } = true;

	private Object hoverObject;
	private LocationSpot hoverSpot;

	public void Awake() {
		camera = GetComponent<Camera>();
	}

	public void Update() {
		if (!Active) return;
		var point = camera.ScreenToWorldPoint(Input.mousePosition);
		point.z = 0;
		var click = Input.GetMouseButtonDown(0);
		var hold = false;
		if (!click) {
			hold = Input.GetMouseButton(0);
		} else {
			var c = Physics2D.OverlapPoint(point, LayerMask.GetMask("Click"));
			var clickable = c?.GetComponentInParent<Clickable>();
			if (clickable != null) {
				clickable.Click();
				return;
			}

		}

		if (DragObject == null) {
			var c = Physics2D.OverlapPoint(point, LayerMask.GetMask("Drag"));
			var o = c?.GetComponentInParent<Object>();
			if (hoverObject != null && o != hoverObject) {
				hoverObject?.StopHover();
				hoverObject = null;
			}

			if (hoverObject == null && o != null) {
				o.StartHover();
				hoverObject = o;
			}

			if (click && hoverObject != null) {
				hoverObject.StartDragging(point);
				DragObject = hoverObject;
			}

		} else {
			var c = Physics2D.OverlapPoint(point, LayerMask.GetMask("Drop"));
			var s = c?.GetComponentInParent<LocationSpot>();

			DragObject.SetDragPosition(point);
			if (hoverSpot != null && s != hoverSpot) {
				DragObject.StopHoveringOver();
				hoverSpot.StopHovering();
				hoverSpot = null;
			}

			if (hoverSpot == null && s != null) {
				DragObject.StartHoveringOver(s);
				s.StartHovering();
				hoverSpot = s;
			}

			if (!hold) {
				if (hoverSpot != null) {
					s.TryAddObject(DragObject);
				}
				if (hoverSpot != null && hoverObject != null) {
					hoverObject.StopHoveringOver();
					hoverSpot.StopHovering();
					hoverSpot = null;
				}
				DragObject.StopDragging();
				DragObject = null;
			}
		}
	}
}
