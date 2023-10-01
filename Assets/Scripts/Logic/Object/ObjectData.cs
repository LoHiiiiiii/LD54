using System.Collections;
using System.Collections.Generic;

public class ObjectData {
	public string Id { get; set; }
	public ObjectVisualType VisualType { get; set; }
	public int DepartureCostGold { get; set; }
	public int? MaxStacks { get; set; } = 1;
	public int? LeavesAfter { get; set; }
	public int? LeaveGold { get; set; }	
	public int Brawn { get; set; }
	public int Fuel { get; set; }
	public List<string> Tags { get; set; } = new List<string>();
}
