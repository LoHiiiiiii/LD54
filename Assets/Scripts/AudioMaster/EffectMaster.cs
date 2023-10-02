using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectMaster : MonoBehaviour {

    [SerializeField] SoundHolder buttonSFX;
    [SerializeField] SoundHolder bellSFX;
    [SerializeField] SoundHolder chainSFX;
    [SerializeField] SoundHolder trainDepartsSFX;
    [SerializeField] SoundHolder trainArrivesSFX;
    [SerializeField] SoundHolder burnSFX;
    [SerializeField] SoundHolder stationMusic;
    [SerializeField] SoundHolder abyssMusic;
    [SerializeField] SoundHolder endMusic;

    public static EffectMaster Instance { get; private set; }

    private void Awake() { Instance = this; }
}

