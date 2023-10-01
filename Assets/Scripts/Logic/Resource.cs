using System;
public class Resource {
	public string Name { get;}

	private int amount;
	public event Action<int> AmountChanged;

	public int Amount {
		get => amount;
		set {
			amount = value;
			AmountChanged?.Invoke(amount);
		}
	}

	private int change;
	public event Action<int> ChangeChanged;

	public int Change {
		get => change;
		set {
			change = value;
			ChangeChanged?.Invoke(change);
		}
	}

	public Resource(string name) {
		Name = name;
	}
}
