using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ConditionalLogicType { GreaterThan, LessThan, True, False, Unknown };

public static class StringValidation
{
    const string lessThan = "<";
    const string greaterThan = ">";

    public static ConditionalLogicType GetConditionLogicType(string conditionString, out string trimmedString)
    {
        ConditionalLogicType conditon = ConditionalLogicType.Unknown;

        if (conditionString.Contains(lessThan))
        {
            conditon = ConditionalLogicType.LessThan;
            conditionString = conditionString.Trim('<');
        }
        if (conditionString.Contains(greaterThan))
        {
            conditon = ConditionalLogicType.GreaterThan;
            conditionString = conditionString.Trim('>');
        }

        //Try parse bool
        if (bool.TryParse(conditionString, out bool result))
        {
            //If bool val true set to condition true type and vice versa
            if (result)
            {
                conditon = ConditionalLogicType.True;
            }
            else
            {
                conditon = ConditionalLogicType.False;
            }
        }
        else
        {
            trimmedString = conditionString;
        }

        trimmedString = conditionString;

        return conditon;
    }

    public static T ConvertStringToDataType<T>(string value)
    {

        if (int.TryParse(value, out int intVal))
        {
            return (T)(object)intVal;
        }
        if (float.TryParse(value, out float floatVal))
        {
            return (T)(object)floatVal;
        }
        if (bool.TryParse(value, out bool boolVal))
        {
            return (T)(object)boolVal;
        }
        else
        {
            //Debug Log that data type is not supported 
            Debug.Log("Data type not supported! Please use a supported data types: float, int and string");
            return (T)(object)value;
        }
    }
}
