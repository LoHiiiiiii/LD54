using System.Collections;
using UnityEngine;

public class TransformFloater : MonoBehaviour {

	[SerializeField] float offset;

	float amount;
	float cycleLength;

	bool Floating { get; set; }


	void Update() {
		HandleFloating();
	}

	public void StopFloating() {
		transform.localPosition = Vector3.zero;
		Floating = false;
	}

	public void SetFloating() => SetFloating(amount, cycleLength);

	public void SetFloating(float amount, float cycleLength) {
		this.amount = amount;
		this.cycleLength = cycleLength;
		Floating = true;
	}

	void HandleFloating() {
		if (!Floating) return;
		transform.localPosition = Vector3.up * amount * Mathf.Sin((Time.time - offset)/(cycleLength / Mathf.PI));
	}
}
