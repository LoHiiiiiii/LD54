using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformFollower : MonoBehaviour {
	[SerializeField] Transform follow;

	void Update() {
		if (follow != null) transform.position = follow.position;
	}
}
