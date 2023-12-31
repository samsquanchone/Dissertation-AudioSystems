using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayback : MonoBehaviour
{
    //Use to play basic one shot with no param values, can make 3D by passing gameobj as argument, or leave argument as null if 2D
    public static void PlayOneShot(FMODUnity.EventReference fmodEvent, Transform transformToAttachTo)
    {
        EventInstance instance = RuntimeManager.CreateInstance(fmodEvent);

        //Check if position has been given to attach event to that position and make 3D
        if (transformToAttachTo != null)
        {
            RuntimeManager.AttachInstanceToGameObject(instance, transformToAttachTo);
        }

        instance.start();
        instance.release();

    }

    //This is a genric function that has a generic param type 'value', when calling this function you will need to cast 'value' to the desired type
    // and replace <T> with desired type
    public static void PlayOneShotWithParameters<T>(EventReference fmodEvent, Transform transformToAttachTo, params (string name, T value)[] parameters)
    {
        EventInstance instance = RuntimeManager.CreateInstance(fmodEvent);

        foreach (var (name, value) in parameters)
        {  

            //If param value is of type string, set as labeled param 
            if (value.GetType() == typeof(string))
            {
                instance.setParameterByNameWithLabel(name, value.ToString());
            }

            //If param value is of type float or int, set param as continous or discrete 
            else if (value.GetType() == typeof(float) || value.GetType() == typeof(int))
            {
                dynamic paramVal = value; //Use dynaic type so it can be int or float, without needing another if statement
                instance.setParameterByName(name, paramVal);
            }
        }

        //Check if position has been given to attach event to that position and make 3D
        if (transformToAttachTo != null)
        {
            RuntimeManager.AttachInstanceToGameObject(instance, transformToAttachTo);
        }

        instance.start();
        instance.release();

    }
}
