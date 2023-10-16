using System.Collections;
using UnityEngine;

public class TransformRotator : MonoBehaviour {
	[SerializeField] float size;

	float rps;
	float maxRps;
	float secondsToMax;

	public bool Rotating { get; private set; }



	public void StartRotating(float speed, float secondsToMax) => SetRotation(speed, secondsToMax, true); 
	public void StopRotating() => StopRotating(maxRps * Mathf.PI * size, secondsToMax);
	public void StopRotating(float speed, float secondsToMax) => SetRotation(speed, secondsToMax, false);

	private void SetRotation(float speed, float secondsToMax, bool rotate) {
		this.secondsToMax = secondsToMax;
		maxRps = speed / (Mathf.PI * size);
		Rotating = rotate;
	}

	

	void Update() {
		Rotate();
	}

	void Rotate() {
		if (!Rotating && rps <= 0) return;
		float targetSpeed = Rotating ? maxRps : 0;
		rps = Mathf.MoveTowards(rps, targetSpeed, maxRps * Time.deltaTime / secondsToMax);
		transform.Rotate(0, 0, -360 * rps * Time.deltaTime);
	}
}
