using DialogueUtility;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
/// <summary>
/// Due to the tricky timing of sequencing dialogue, threading is a required method for handling any concurrancy issues with the dialogue system.
/// However, the Unity API is not thread safe, hence utilising the job system which is method provided by Unity to support multi-threading!
/// 
/// Note: a unity job cant have reference types in it, hence the little class and static class to be able to cache values from the dialogue job system class constructor!
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

        DialogueThreadData.SetThreadData(key, eventName);
    }

    public struct CalculateDialogueLength : IJob
    {

        public NativeArray<float> result;

        public void Execute()
        {
            DialogueInfoHandler diaInfoCallback = new(DialogueThreadData.GetKey(), DialogueThreadData.GetEvent());
            result[0] = diaInfoCallback.GetDialogueLength();
        }
    }

    public struct TriggerDialogue : IJob
    {
        public void Execute()
        {
           
        }
    }
}
//Job structs can't have reference types so use of static class to pass info to thread
public static class DialogueThreadData
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

public class ConditionValidationJobSystem<T>
{
    public ConditionValidationJobSystem(T condition)
    {
        ConditionThreadData.SetConditionData(condition);
    }

    public struct CheckCondition : IJob
    {
        public void Execute()
        {
            
        }
    }
}

public static class ConditionThreadData
{
    static dynamic conditionData;

    public static void SetConditionData<T>(T condition)
    {
        conditionData = condition;
    }
}