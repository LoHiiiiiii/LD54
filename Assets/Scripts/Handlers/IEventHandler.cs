using System;

public interface IEventHandler {
	void StartEvent(GameEvent gameEvent, Action completed);
}
