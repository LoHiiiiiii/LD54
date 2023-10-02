using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Clickable : MonoBehaviour {
	public event Action Clicked;

	public void Click() => Clicked?.Invoke();
}
