using System;
using UnityEngine;

public class Resource {
	public ResourceData Data { get;}
	public int? Maximum { get; set; } = null;

	private int amount;
	public event Action AmountChanged;

	public int Amount {
		get => amount;
		set {
			if (Maximum != null && amount == Maximum) return;
			amount = Mathf.Min(value, Maximum ?? value );
			AmountChanged?.Invoke();
		}
	}

	private int target;
	public event Action TargetChanged;

	public int Target {
		get => target;
		set {
			if (Maximum != null && target == Maximum) return;
			target = Mathf.Min(value, Maximum ?? value);
			TargetChanged?.Invoke();
		}
	}

	public Resource(ResourceData data) {
		Data = data;
	}
}
