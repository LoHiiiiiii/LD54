using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoadEventHandler : MonoBehaviour
{
    [SerializeField] FlowHandler handler;

    Dictionary<int, HashSet<RoadEventData>> pools;

    public void InvokeEventPhase() => handler.EndCurrentEvent();
    public void AddPools(Dictionary<int, HashSet<RoadEventData>> pools) => this.pools = pools;

    public RoadEventData GetRandomEvent(int pool) {
        if (!pools.ContainsKey(pool)) return null;
        if (!pools[pool].Any()) return null;
        var list = pools[pool].ToList();
        list.Shuffle();
        pools[pool].Remove(list[0]);
        return list[0];
    }
}
