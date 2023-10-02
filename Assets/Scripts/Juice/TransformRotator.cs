using UnityEngine;

public class TransformRotator : MonoBehaviour {
	[SerializeField] float maxRPS;
	[SerializeField] float secondsToMax;

	float rps;

	public bool Rotating { get; set; }

	void Update() {
		if ((rps < maxRPS && Rotating) || ( rps >= 0 && !Rotating)) {
			rps = Rotating ? Mathf.Min(maxRPS, rps + maxRPS/secondsToMax * Time.deltaTime) : 0;
		}

		if (rps <= 0 && !Rotating) return;
		transform.Rotate(0, 0, -360 * rps * Time.deltaTime);
	}
}
