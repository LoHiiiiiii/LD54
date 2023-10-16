using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameEventContainer : MonoBehaviour {

	Dictionary<int, Dictionary<ChoiceEvent, int>> randomEventPools;
	GameEvent[] organizationEvents;
	(GameEventType eventType, int index)[] eventOrder;

	public GameEvent GetEvent(int eventIndex) {
		return eventOrder[eventIndex].eventType switch {
			GameEventType.Random => GetRandomEvent(eventOrder[eventIndex].index),
			GameEventType.Organization => organizationEvents[eventOrder[eventIndex].index],
			_ => null
		};
	}

	private ChoiceEvent GetRandomEvent(int pool) {
		if (!randomEventPools.ContainsKey(pool)) return null;
		if (!randomEventPools[pool].Any()) return null;
		var total = randomEventPools[pool].Values.Sum();
		var value = Random.Range(0f, total);
		var check = 0f;
		foreach (var roadEvent in randomEventPools[pool].Keys) {
			check += randomEventPools[pool][roadEvent];
			if (value <= check) {
				randomEventPools[pool].Remove(roadEvent);
				return roadEvent;
			}
		}
		return null;
	}

	public bool EventExists(int index) => index < 0 || index < eventOrder.Length;
	public void SetEventPools(Dictionary<int, Dictionary<ChoiceEvent, int>> pools) => randomEventPools = pools;
	public void SetOrganizationEvents(GameEvent[] events) => organizationEvents = events;
	public void SetEventOrder((GameEventType eventType, int index)[] order) => eventOrder = order;
}
