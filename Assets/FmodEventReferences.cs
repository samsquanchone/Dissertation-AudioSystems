using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FmodEventReferences : MonoBehaviour
{
    public static FmodEventReferences Instance => m_instance;
    private static FmodEventReferences m_instance;

    [SerializeField] public EventReference gunShotSFX;

    private void Start()
    {
        m_instance = this;
    }


}

