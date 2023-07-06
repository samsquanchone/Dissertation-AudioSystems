using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class Test : MonoBehaviour
{
    [SerializeField] EventReference test;
    // Start is called before the first frame update
    void Start()
    {
        EventInstance instance = RuntimeManager.CreateInstance(test);

        instance.start();
        instance.release();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
