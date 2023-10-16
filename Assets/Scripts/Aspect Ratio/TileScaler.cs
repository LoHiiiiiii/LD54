using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScaler : MonoBehaviour {

	[SerializeField] MeshRenderer meshRenderer;

	void Start() {
		transform.localScale = new Vector3(
			AspectRatioInstance.Instance.GetWidthMultiplier(),
			AspectRatioInstance.Instance.GetHeightMultiplier());
		meshRenderer.material.mainTextureScale = new Vector2(
			meshRenderer.material.mainTextureScale.x * AspectRatioInstance.Instance.GetWidthMultiplier(),
			meshRenderer.material.mainTextureScale.y * AspectRatioInstance.Instance.GetHeightMultiplier());
	}
}
