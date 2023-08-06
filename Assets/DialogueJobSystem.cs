using DialogueUtility;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
/// <summary>
/// Due to the tricky timing of sequencing dialogue, threading is a required method for handling any concurrancy issues with the dialogue system.
/// However, the Unity API is not thread safe, hence utilising the job system which is method provided by Unity to support multi-threading!
/// </summary>
/// 
public class DialogueJobSystem
{
    private string key;
    private FMODUnity.EventReference eventName;

    public DialogueJobSystem(string _key, FMODUnity.EventReference _eventName)
    {
        key = _key;
        eventName = _eventName;

        ThreadData.SetThreadData(key, eventName);
    }

public struct CalculateDialogueLength : IJob
    {
        
        public NativeArray<float> result;

        public void Execute()
        {
            DialogueInfoHandler diaInfoCallback = new(ThreadData.GetKey(), ThreadData.GetEvent());
            result[0] = diaInfoCallback.GetDialogueLength();
        }
    }

  
}
public static class ThreadData
{
    private static string key;
    private static FMODUnity.EventReference eventName;
    public static void SetThreadData(string _key, FMODUnity.EventReference _eventName)
    {
        key = _key;
        eventName = _eventName;
    }

    public static string GetKey()
    {
        return key;
    }

    public static FMODUnity.EventReference GetEvent()
    {
        return eventName;
    }
}