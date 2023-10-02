using UnityEngine;

public class TransformRotator : MonoBehaviour {
	[SerializeField] float maxRPS;
	[SerializeField] float secondsToMax;
	[SerializeField] bool rotating;

	float rps;

	public bool Rotating { get => rotating; }

	void Update() {
		if ((rps < maxRPS && Rotating) || ( rps >= 0 && !Rotating)) {
			rps = rotating ? Mathf.Min(maxRPS, rps + maxRPS/secondsToMax * Time.deltaTime) : rps - maxRPS/secondsToMax * Time.deltaTime;
		}

		if (rps <= 0 && !Rotating) return;
		transform.Rotate(0, 0, -360 * rps * Time.deltaTime);
	}
}
