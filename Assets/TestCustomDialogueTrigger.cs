using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCustomDialogueTrigger : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine("asdad");
       
    }

    public void CustomDialogueTriggerExample(string dialogueKey)
    {
        Debug.Log("SADASD");
    }

    IEnumerator asdad()
    {
        yield return new WaitForSeconds(3);
        CustomDialogueTriggerExample("Hey2");
    }
}
