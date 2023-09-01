using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class LookAtNPC : MonoBehaviour
{
    public Transform mainCameraTransform;
    public float speed = 1f;

    private Coroutine lookAtCoroutine;


    public void LookAtTarget(Transform playerPos, Transform target)
    {
        playerPos = mainCameraTransform;

        if (lookAtCoroutine != null)
        {
           StopCoroutine(lookAtCoroutine);
        }

       lookAtCoroutine = StartCoroutine(LookAt(playerPos, target));
    }

    private IEnumerator LookAt(Transform playerPos, Transform target)
    {

        Quaternion lookRotation = Quaternion.LookRotation(target.position - playerPos.transform.position);

        float time = 0;

        while (time < 1)
        {
            playerPos.transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, time); //May need to change this to object ransform

            time += Time.deltaTime * speed;

            yield return null;
        }
    }
}
