using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadEventHandler : MonoBehaviour
{
    [SerializeField] FlowHandler handler;

    public void InvokeEventPhase() => handler.EndCurrentEvent();
}
