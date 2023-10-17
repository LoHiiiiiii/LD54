using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class ViewHandler : MonoBehaviour {

	[SerializeField] Transform abyss;
	[SerializeField] Transform station; 
	[SerializeField] GameObject mainMenu;
	[SerializeField] GameObject gameOver;
	[SerializeField] GameObject victory;
	[SerializeField] GameObject resourceHandler;
	[SerializeField] CurtainHandler curtainHandler;
	[SerializeField] TrainMover trainMover;



	AudioSource currentMusic;

	public View CurrentView { get; private set; }

	public async Task ExitView(View targetView) {
		if (CurrentView == View.Station || targetView == View.Abyss || (targetView == View.Station && CurrentView != View.MainMenu)) {
			await trainMover.TrainExit();
		}

		_ =AudioMaster.Instance.Stop(currentMusic, curtainHandler.Duration);
		await curtainHandler.FadeOut();
		trainMover.SetFloating(targetView == View.Abyss);

		if (CurrentView == View.MainMenu) {
			trainMover.gameObject.SetActive(true);
			resourceHandler.SetActive(true);
		}
	}

	public async Task StayInView() {
		if (CurrentView != View.Abyss) return;
		await trainMover.StopTrain();
	}

	public async Task EnterView(View targetView, int viewIndex) {
		//for (int i = 0; i < abyss.childCount; ++i) {
		//	abyss.GetChild(i).gameObject.SetActive(i == viewIndex && targetView == View.Abyss);
		//}


		abyss.gameObject.SetActive(targetView == View.Abyss);
		mainMenu.SetActive(targetView == View.MainMenu);

		//for (int i = 0; i < station.childCount; ++i) {
		//	station.GetChild(i).gameObject.SetActive(i == viewIndex && targetView == View.Station);
		//}

		station.gameObject.SetActive(targetView == View.Station);
		gameOver.SetActive(targetView == View.GameOver);
		victory.SetActive(targetView == View.Win);
		resourceHandler.SetActive(targetView == View.Abyss || targetView == View.Station);

		var trainTask = (targetView == View.Abyss || targetView == View.Station) ? trainMover.TrainEnter(targetView == View.Abyss) : Task.CompletedTask;
		PlayMusic(targetView);
		await Task.WhenAll(
			curtainHandler.FadeIn(), 
			trainTask);
		CurrentView = targetView;
	}

	private void PlayMusic(View targetView) {
		var holder = targetView switch {
			View.Abyss => EffectMaster.Instance.abyssMusic,
			View.Station => EffectMaster.Instance.stationMusic,
			View.GameOver => EffectMaster.Instance.endMusic,
			_ => EffectMaster.Instance.endMusic	
		};

		if (holder == null) return;
		currentMusic = AudioMaster.Instance.Play(holder, curtainHandler.Duration);
	}
}
