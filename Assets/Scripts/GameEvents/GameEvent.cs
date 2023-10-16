public class GameEvent {
	public View View { get; set; }
	public int ViewIndex { get; set; }
	public bool NewView { get; set; }
	public GameEventType Type { get; set; }	
	public bool CountedEvent { get; set; }
}