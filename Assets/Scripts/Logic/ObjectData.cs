using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using UnityEngine;

public class ObjectData {
	public string Name { get; }
	public int DepartureCostGold { get; set; }
	public int? MaxStacks { get; set; } = 1;
	public int? LeavesAfter { get; set; }
	public int? LeaveGold { get; set; }	
	public int Brawn { get; set; }
	public int Fuel { get; set; }
	public List<string> Tags { get; set; } = new List<string>();
}
