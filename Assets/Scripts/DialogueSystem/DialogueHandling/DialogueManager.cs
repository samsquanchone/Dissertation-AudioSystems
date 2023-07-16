using System.Collections.Generic;
using UnityEngine;
using DialogueUtility;


public enum SequenceState { SEQUENTIAL, RANDOM, RESPONSE };

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

    void Start()
    {
        /*
        string newString;
        Debug.Log("Condition type: " + StringValidation.GetConditionLogicType("<10", out newString).ToString() + "/// New String:  " + newString);

        string hey = "10";
        string hey2 = "10.5";
        Debug.Log("Condition type: " + StringValidation.GetConditionLogicType("<10.5", out hey2).ToString() + "/// New String:  " + hey2);
        string hey3 = "hey";
        Debug.Log("Condition type: " + StringValidation.GetConditionLogicType("Hey", out hey3).ToString() + "/// New String:  " + hey3);


        dynamic val = StringValidation.ConvertStringToDataType<dynamic>(newString);
        dynamic valFloat = StringValidation.ConvertStringToDataType<dynamic>(hey2);
        dynamic valString = StringValidation.ConvertStringToDataType<dynamic>(hey3);

        test = val;
        stringtest = valString;
        floatTest = valFloat;

        for (int i = 0; i < 5; i++)
        {
            GameDataValue dataVal = new(Random.Range(0, 2000));
            gameDataHashTable.AddGameDataToHashTable((uint)i, (GameDataValue)dataVal);

        }

        GameDataValue value = (GameDataValue)gameDataHashTable.hashTable[(uint)1];
        Debug.Log("GameDataDefaultValue: " + value.m_default);
        */
    }

    public void AddEntityToHashTable(Entity entity)
    {
        //Add an entitys dialogue information within the scene to the hash table 
        npcDialogueHashTable.hashTable.Add(uint.Parse(entity.id), entity);

        Entity _entity = (Entity)npcDialogueHashTable.hashTable[uint.Parse(entity.id)]; //NOTE NEED A BETTER WAY OF READING CONDITION ID: MAYBE ADD SOUND DESIGNER ID
        Debug.Log(_entity.lines[0].conditions[(uint)1].triggerCondition);


    }

    public void Notify()
    {
        for (int i = 0; i < npcs.Count; i++)
        {
            //Notify all observers even though some may not be interested in what has happened
            //Each observer should check if it is interested in this event
            npcs[i].OnNotify();
        }
    }

    //Add observer to the list
    public void AddObserver(TestXML observer)
    {
        npcs.Add(observer);
    }

    //Remove observer from the list
    public void RemoveObserver(TestXML observer)
    {
    }
    public void PlayDialogueSequence(List<Line> lineSequence, string npcName, FMODUnity.EventReference eventName )
    {

        StartCoroutine(DialogueSequenceTimer(lineSequence, eventName));

        /*
        if (CheckDialogueCondition(lineSequence[0].conditions[(uint)1]))
        {
            Debug.Log("great succsess");
        }
        else
        {
            Debug.Log("Failed");
        }
        // if(lineSequence[0].conditions)

        /*Queue<Line> dialogueLineSequence = new();

        //Create a queue from the list of dialogue lines passed to the manager
        foreach (var line in lineSequence)
        {
            
            
            
            //Add to queue for each line in an entity 
            //dialogueLineSequence.Enqueue(line);
            //Could handle dialogue here and use a coroutine to to queue the dialogue and text properlly, get fmod sounds length

        }

       // dialogueLineSequence.Elemen

        //Print queue after line is populated 
       subtitleManager.QueueDialogue(dialogueLineSequence, npcName);
        */
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
                        //  Debug.Log("Length: " + diaInfoCallback.GetDialogueLength());
                        DialogueHandler programmerCallback = new(line.key, eventName, null); //Make programmer deceleration in function, to make memory management better!!!

                        double diaLength = diaInfoCallback.GetDialogueLength() / 1000; //This will return MS, for now we use seconds conversion, but if there are timing issues, maybe revert back to MS

                        yield return new WaitForSeconds((float)diaLength); //We want a dialogue call back for info to get the length of an audio table clip.... UNITY WHY CANT I USE A DOUBLE >_< 
                    }

                   
                }

               
            }
        }

        yield return 0;
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
