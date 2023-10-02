using System.Collections;
using System.Collections.Generic;

public class ObjectData {
	public string Id { get; set; }
	public ObjectVisualType VisualType { get; set; }
	public int? MaxStacks { get; set; } = 1;
	public Dictionary<ResourceType, int> DepartureEffects { get; set; } = new Dictionary<ResourceType, int>();
	public Dictionary<ResourceType, int> Stats { get; set; } = new Dictionary<ResourceType, int>();
	public Dictionary<ResourceType, int> ExitEffects { get; set; } = new Dictionary<ResourceType, int>();
	public Dictionary<ResourceType, int> BurnEffects { get; set; } = new Dictionary<ResourceType, int>(); 
	public Dictionary<ResourceType, int> AddEffects { get; set; } = new Dictionary<ResourceType, int>();



	public List<string> Tags { get; set; } = new List<string>();
}
