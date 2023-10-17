using TMPro;
using UnityEngine;

public class VictoryHandler : MonoBehaviour {
	
	[SerializeField] CurtainHandler curtainHandler;
	[SerializeField] TMP_Text descriptionField;


	public void Exit() => FadeExit();

	private async void FadeExit() {
		await curtainHandler.FadeOut();
		Application.Quit();
	}

	public void SetVictory(VictoryType type) { }
}
