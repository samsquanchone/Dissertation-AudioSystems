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

    private void Start()
    {
        m_instance = this;
    }
    public void PlayDialogueSequence(List<Line> lineSequence)
    {
        Queue dialogueLineSequence = new();

        //Create a queue from the list of dialogue lines passed to the manager
        foreach (var line in lineSequence)
        {
            //Add to queue for each line in an entity 
            dialogueLineSequence.Enqueue(line.line);

        }

        //Print queue after line is populated 
       subtitleManager.QueueDialogue(dialogueLineSequence);
    }

    public void PrintQueue(IEnumerable que)
    {
        foreach (var q in que)
        {
            Debug.Log(q);
        }
    }
}
