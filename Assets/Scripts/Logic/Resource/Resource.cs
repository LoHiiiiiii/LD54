using System;
public class Resource {
	public ResourceData Data { get;}

	private int amount;
	public event Action AmountChanged;

	public int Amount {
		get => amount;
		set {
			amount = value;
			AmountChanged?.Invoke();
		}
	}

	private int target;
	public event Action TargetChanged;

	public int Target {
		get => target;
		set {
			target = value;
			TargetChanged?.Invoke();
		}
	}

	public Resource(ResourceData data) {
		Data = data;
	}
}
