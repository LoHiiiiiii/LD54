using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class FlowHandler : MonoBehaviour {

	[SerializeField] ValueParser valueParser;
	[SerializeField] ViewHandler viewHandler;
	[SerializeField] ResourceHandler resourceHandler;
	[SerializeField] GameEventContainer gameEventContainer; 
	[SerializeField] ChoiceEventHandler choiceEventHandler;
	[SerializeField] OrganizationEventHandler organizationEventHandler;
	[SerializeField] GameOverHandler gameOverHandler;
	[SerializeField] EventSystem eventSystem;

	private GameEvent currentEvent;
	private int nextEventIndex;
	private int eventsCompleted;

	public void Start() {
		valueParser.Initialize();
		EnterMainMenu();
	}
	
	private async void EnterMainMenu() {
		eventSystem.enabled = false;
		await viewHandler.EnterView(View.MainMenu, 0);
		eventSystem.enabled = true;
	}

	bool started;
	public void StartGame() {
		if (started) return;
		organizationEventHandler.Prepare();
		StartNextEvent();
		started = true;
	}

	public async void StartNextEvent() {
		await Task.CompletedTask;
		var newView = false;
		currentEvent = null;
		while(currentEvent == null) {
			View targetView;
			var victory = CheckVictory();
			if (victory != null) {
				targetView = View.Win;
				newView = true;
			} else {
				currentEvent = gameEventContainer.GetEvent(nextEventIndex);
				if (currentEvent == null) {
					nextEventIndex++;
					continue;
				}
				newView = newView || currentEvent.NewView;
				targetView = currentEvent.View;
			}

			if (newView) {
				await viewHandler.ExitView(targetView);
			} else {
				await viewHandler.StayInView();
			}

			var gameOver = CheckGameOver(targetView);
			if (gameOver != null) {
				gameOverHandler.SetGameOver((GameOverType)gameOver);
				await viewHandler.EnterView(View.GameOver, (int)gameOver);
				return;
			} else {
				if (victory != null) {
					await viewHandler.EnterView(targetView, (int)victory);
					return;
				}
				
				if (currentEvent.NewView) {
					await viewHandler.EnterView(currentEvent.View, currentEvent.ViewIndex);
				}
			}
			IEventHandler handler = currentEvent.Type switch {
				GameEventType.Random => choiceEventHandler,
				GameEventType.Organization => organizationEventHandler,
				_ => null
			};

			if (handler != null) {
				handler.StartEvent(currentEvent, FinishEvent);
			}
		}
	}

	private VictoryType? CheckVictory() {
		if (gameEventContainer.EventExists(nextEventIndex)) return null;
		return null;
	}

	private GameOverType? CheckGameOver(View targetView) {
		if (targetView == View.Station) return null;
		if (resourceHandler.resources[ResourceType.Gold].Amount < 0) return GameOverType.Mutiny;
		if (resourceHandler.resources[ResourceType.Health].Amount < 0) return GameOverType.Broken;
		if (organizationEventHandler.GetAllObjects().Count == 0) return GameOverType.Stranded;
		return null;
	}

	private void FinishEvent() {
		nextEventIndex++;
		if(currentEvent.CountedEvent) eventsCompleted++;
		StartNextEvent();
	}
}
