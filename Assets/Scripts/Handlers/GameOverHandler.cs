using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverHandler : MonoBehaviour {
	
	[SerializeField] CurtainHandler curtainHandler;
	[SerializeField] TMP_Text titleField;
	[SerializeField] TMP_Text descriptionField;


	public void ToMainMenu() => MainMenu();

	private async void MainMenu() {
		await curtainHandler.FadeOut();
		SceneManager.LoadScene(0);
	}

	public void SetGameOver(GameOverType type) {
		titleField.text = type switch {
			GameOverType.Mutiny => "Murdered",
			GameOverType.Broken => "Train destroyed",
			GameOverType.Stranded => "Stranded",
			_ => "Game Over"
		};

		descriptionField.text = type switch {
			GameOverType.Mutiny => "Your debtors have come to collect what is theirs.",
			GameOverType.Broken => "The abuse the train suffered proved to be too much.",
			GameOverType.Stranded => "With nothing to run the engine with, your journey has ended.",
			_ => ""
		};
	}
}
