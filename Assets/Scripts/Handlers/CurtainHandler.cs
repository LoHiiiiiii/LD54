using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CurtainHandler : MonoBehaviour {

	[SerializeField] CanvasGroup curtain;
	[SerializeField] float fadeDuration;
	public float Duration { get => fadeDuration; }

	private void Awake() {
		curtain.alpha = 1;
	}

	public async Task FadeOut(bool forceTransition = false) => await FadeCurtain(forceTransition, 1, Duration);
	public async Task FadeIn(bool forceTransition = false) => await FadeCurtain(forceTransition, 0, Duration);

	private async Task FadeCurtain(bool forceTransition, float target, float duration) {
		float alpha = forceTransition ? ((target == 1) ? 0 : 1) : curtain.alpha;
		while(alpha != target) {
			alpha = Mathf.MoveTowards(alpha, target, Time.deltaTime / duration);
			curtain.alpha = alpha;
			await Awaiters.NextFrame;
		}
	}
}
