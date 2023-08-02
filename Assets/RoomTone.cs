using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class RoomTone : MonoBehaviour
{
    [SerializeField] private EventReference roomToneEvent;
    // Start is called before the first frame update
    void Start()
    {
        EventInstance instance = RuntimeManager.CreateInstance(roomToneEvent);

        instance.start();
        instance.release();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
