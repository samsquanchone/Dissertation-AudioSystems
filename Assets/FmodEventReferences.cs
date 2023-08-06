using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FmodEventReferences : MonoBehaviour
{
    //Singleton decleration
    public static FmodEventReferences Instance => m_instance;
    private static FmodEventReferences m_instance;

    //What-ever fmod events you would like to reference
    public EventReference gunShotSFX;
    public EventReference footstepSFX;

    private void Start()
    {
        //Singleton instantiation
        m_instance = this;
    }


}

