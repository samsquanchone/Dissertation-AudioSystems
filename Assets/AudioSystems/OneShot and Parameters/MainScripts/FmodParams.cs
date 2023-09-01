using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FmodParams : MonoBehaviour
{

    /***************************** LOCAL PARAMETERS **********************************************************************************************************************/

    //Used to set state value of fmod labled params
    public static void SetParamByLabelName(FMOD.Studio.EventInstance eventInstance, string labelParamName, string labelParamState)
    {
        eventInstance.setParameterByNameWithLabel(labelParamName, labelParamState);
    }

    public static float GetParamByName(FMOD.Studio.EventInstance eventInstance, string paramName)
    {
        float value;
        float finalValue;

        eventInstance.getParameterByName(paramName, out value, out finalValue);

        return finalValue;
    }

    //Used to set value of fmod discrete and continous parameter types
    public static void SetParamByName<T>(FMOD.Studio.EventInstance eventInstance, string paramName, T paramValue)
    {
        dynamic value = paramValue;                          /*Generic <T> cannot be used with fmodAPI, therefore dynamic type is required for use when setting param value.
                                                             NOTE: this may cause: error CS0656: Missing compiler required member 'Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo.Create', please install the required reference
                                                             Please navigate to Edit > Project Settings > Player > Other Settings > Configuration > API Compatibility Level and change from .NET Standard 2.0 to .NET, or  4.x .NET Framework*/
        eventInstance.setParameterByName(paramName, value);
    }
    /***************************** GLOBAL PARAMETERS*********************************************************************************************************************/

    //Used to set fmod global parameters of labled type
    public static void SetGlobalParamByLabelName(string labelParamName, string labelParamState)
    {
        FMODUnity.RuntimeManager.StudioSystem.setParameterByNameWithLabel(labelParamName, labelParamState);
    }

    //Used to set fmod global parameters of continous and discrete param types
    public static void SetGlobalParamByName<T>(string globalParamName, T globalParamValue)
    {
        dynamic value = globalParamValue;                          /*Generic <T> cannot be used with FMODAPI, therefore dynamic type is required for use when setting param value.
                                                                   NOTE: this may cause: error CS0656: Missing compiler required member 'Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo.Create', please install the required reference
                                                                   Please navigate to Edit > Project Settings > Player > Other Settings > Configuration > API Compatibility Level and change from .NET Standard 2.0 to .NET, or  4.x .NET Framework*/

        FMODUnity.RuntimeManager.StudioSystem.setParameterByName(globalParamName, value);
    }

}
