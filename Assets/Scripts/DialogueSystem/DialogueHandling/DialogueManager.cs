using System.Collections.Generic;
using UnityEngine;
using DialogueUtility;




/// <summary>
/// This is where the handling of a sequence of dialogue is handled, e.g. if just a random the queue size would be one, 
/// where with sequence it would be either: time of clip + buffer, or on input e.g. space 
/// </summary>
public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance => m_instance;
    private static DialogueManager m_instance;
    public SubtitleManager subtitleManager;




    public List<TestXML> npcs = new List<TestXML>();

    public int test;
    public string stringtest;
    public float floatTest;

    //Create an instace of our hashTableObject with abstracted hash handling functionality, for both npcsDialogue + gamedata
    private HashTable gameDataHashTable = new();
    private HashTable npcDialogueHashTable = new();

    public GameDataResolver resolver;

    private void Awake()
    {
        m_instance = this;
    }

    public void AddEntityToHashTable(Entity entity)
    {
        //Add an entitys dialogue information within the scene to the hash table 
        npcDialogueHashTable.hashTable.Add(uint.Parse(entity.id), entity);

        Entity _entity = (Entity)npcDialogueHashTable.hashTable[uint.Parse(entity.id)]; //NOTE NEED A BETTER WAY OF READING CONDITION ID: MAYBE ADD SOUND DESIGNER ID
        Debug.Log(_entity.lines[0].conditions[(uint)1].triggerCondition);


    }
    public void PlayDialogueSequence(List<Line> lineSequence, SequenceType sequenceType, FMODUnity.EventReference eventName)
    {



       // Queue dialogueLineSequence = new();



        switch (sequenceType)
        {
            case SequenceType.Sequential:
                StartCoroutine(DialogueSequenceTimer(lineSequence, eventName));
                break;

            case SequenceType.RandomOneShot:
                PlayRandomDialogue(lineSequence, eventName);
                break;
        }

        //Create a queue from the list of dialogue lines passed to the manager
        foreach (var line in lineSequence)
        {
            
            
            
            //Add to queue for each line in an entity 
            //dialogueLineSequence.Enqueue(line);
            //Could handle dialogue here and use a coroutine to to queue the dialogue and text properlly, get fmod sounds length

        }

       // dialogueLineSequence.Elemen

        //Print queue after line is populated 
       //subtitleManager.QueueDialogue(dialogueLineSequence, npcName);
        
    }

    private System.Collections.IEnumerator DialogueSequenceTimer(List<Line> lineSequence, FMODUnity.EventReference eventName)
    {
        
        foreach (var line in lineSequence)
        {
            int i = 0;
            foreach (var condition in line.conditions)
            {

                if (CheckDialogueCondition(condition.Value))
                {
                    i++;
                    if (i == line.conditions.Count) //IF all conditions true i.e at end element + all conditions are met 
                    {
                        //Play dialogue
                        DialogueInfoHandler diaInfoCallback = new(line.key, eventName);

                        double diaLength = diaInfoCallback.GetDialogueLength() / 1000; //This will return MS, for now we use seconds conversion, but if there are timing issues, maybe revert back to MS
                        //  Debug.Log("Length: " + diaInfoCallback.GetDialogueLength());
                        DialogueHandler programmerCallback = new(line.key, eventName, null); //Make programmer deceleration in function, to make memory management better!!!

                       

                        yield return new WaitForSeconds((float)diaLength); //We want a dialogue call back for info to get the length of an audio table clip.... UNITY WHY CANT I USE A DOUBLE >_< 
                    }

                   
                }

               
            }
        }

        yield return 0;
    }

    private void PlayRandomDialogue(List<Line> lineSequence, FMODUnity.EventReference eventName)
    {
        List<Line> triggerableLines = new();

        foreach (var line in lineSequence)
        {
            foreach(var condition in line.conditions)
            {
                if (CheckDialogueCondition(condition.Value))
                {
                    triggerableLines.Add(line);
                    Debug.Log("Key for lne: " + line.key);
                }
            }
        }

        int indexToPlay = Random.Range(0, triggerableLines.Count);
        DialogueHandler programmerCallback = new(triggerableLines[indexToPlay].key, eventName, null); //Make programmer deceleration in function, to make memory management better!!!
        //  Debug.Log("Length: " + diaInfoCallback.GetDialogueLength());
    }

    public bool CheckDialogueCondition(Condition diaCondition)
    {
        //GameDataResolver resolver = new();
        GameDataReturnType dataResolveType = resolver.QuerryGameData<dynamic>(diaCondition.gameDataKey, out dynamic gameDataVal);
        
        switch (diaCondition.triggerCondition)
        {
            case ConditionalLogicType.GreaterThan:
                if (gameDataVal > diaCondition.conditionValue) { return true; }
                break;

            case ConditionalLogicType.LessThan:
                if (gameDataVal < diaCondition.conditionValue) { return true; }
                break;

            case ConditionalLogicType.True:
                if (gameDataVal) { return true; }
                break;

            case ConditionalLogicType.False:
                if (!gameDataVal) { return true; }
                break;
        }

        Debug.Log("Resolution: " + dataResolveType.ToString() + " /  gameDataVal: " + gameDataVal + "ConditionVAL: " + diaCondition.conditionValue);
        return false;
    }
}
