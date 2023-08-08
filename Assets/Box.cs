using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            QuestHandler.Instance.UpdateQuest();
            Destroy(this.gameObject);
        }
    }
}
