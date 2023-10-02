using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectMaster : MonoBehaviour {

    [SerializeField] public SoundHolder buttonSFX;
    [SerializeField] public SoundHolder bellSFX;
    [SerializeField] public SoundHolder chainSFX;
    [SerializeField] public SoundHolder trainDepartsSFX;
    [SerializeField] public SoundHolder trainArrivesSFX;
    [SerializeField] public SoundHolder burnSFX;
    [SerializeField] public SoundHolder stationMusic;
    [SerializeField] public SoundHolder abyssMusic;
    [SerializeField] public SoundHolder endMusic;
    public static EffectMaster Instance { get; private set; }

    private void Awake() { Instance = this; }
}

