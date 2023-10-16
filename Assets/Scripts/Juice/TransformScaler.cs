using System.Collections;
using UnityEngine;

public class TransformScaler : MonoBehaviour {

	[SerializeField] AnimationCurve smoothCurve;

	float scaleLerp = 1;
	float targetScale = 1;
	float startScale = 1;
	float scaleDuration;
	Vector3 scaleVector;

	public bool IsScaling() { return scaleLerp != 1; }

	private void Awake() {
		scaleVector = transform.localScale;
		targetScale = 0;
		startScale = 0;
		Scale();
	}

	public void SetScale(float scale, float duration) {
		startScale = GetScale();
		if (duration <= 0) {
			scaleLerp = 1;
			Scale();
		} else {
			scaleDuration = duration;
			scaleLerp = 0;
		}
		targetScale = scale;
	}


	void Update() {
		HandleScaling();
	}

	void HandleScaling() {
		if (!IsScaling()) return;
		scaleLerp = Mathf.MoveTowards(scaleLerp, 1, scaleDuration * Time.deltaTime);
		Scale();
	}

	void Scale() {
		transform.localScale = scaleVector * GetScale();
	}

	float GetScale() => Mathf.Lerp(startScale, targetScale, smoothCurve.Evaluate(scaleLerp));
}
