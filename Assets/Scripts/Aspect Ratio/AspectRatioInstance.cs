using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AspectRatioInstance : MonoBehaviour {

	[SerializeField] new Camera camera;

	public static AspectRatioInstance Instance { get; private set; }

	const float targetRatio = 1920f / 1080f;
	float prevAspect;
	float startSize;
	Vector3 startPos;

	private void Awake() {
		if (Instance != null) Debug.LogError("Multiple AspectRatioInstances!");
		Instance = this;
		prevAspect = targetRatio;
		startPos = camera.transform.position;
		startSize = camera.orthographicSize;
	}

	private void Update() {
		if (camera.aspect == prevAspect) return;
		camera.orthographicSize = GetHeightMultiplier() * startSize;
		camera.transform.position = startPos + Vector3.down * (camera.orthographicSize - startSize);
		prevAspect = camera.aspect;
	}

	public float GetWidthMultiplier() => Mathf.Max(1, camera.aspect / targetRatio);

	public float GetHeightMultiplier() => Mathf.Max(1, targetRatio / camera.aspect);
}
