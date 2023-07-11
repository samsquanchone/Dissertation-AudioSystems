using System.Collections;
using System.Collections.Generic;
using UnityEngine;



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

    //Create an instace of our hashTableObject with abstracted hash handling functionality, for both npcsDialogue + gamedata
    private HashTable gameDataHashTable = new();
    private HashTable npcDialogueHashTable = new();

    private void Awake()
    {
        m_instance = this;
    }

    void Start()
    {
       
        for (int i = 0; i < 5; i++)
        {
            GameDataValue dataVal = new(Random.Range(0, 2000));
            gameDataHashTable.AddGameDataToHashTable((uint)i, (GameDataValue)dataVal);
            
        }

        GameDataValue value = (GameDataValue)gameDataHashTable.hashTable[(uint)1];
        Debug.Log("GameDataDefaultValue: " + value.m_default);
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
    public void PlayDialogueSequence(List<Line> lineSequence, string npcName)
    {

       
        Queue<Line> dialogueLineSequence = new();

        //Create a queue from the list of dialogue lines passed to the manager
        foreach (var line in lineSequence)
        {
            //Add to queue for each line in an entity 
            dialogueLineSequence.Enqueue(line);
            //Could handle dialogue here and use a coroutine to to queue the dialogue and text properlly, get fmod sounds length

        }

        //Print queue after line is populated 
       subtitleManager.QueueDialogue(dialogueLineSequence, npcName);
    }
}
