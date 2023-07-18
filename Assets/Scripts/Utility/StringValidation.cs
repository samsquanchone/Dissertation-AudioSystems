using System.Collections;
using System.Collections.Generic;

public enum ConditionalLogicType { GreaterThan, LessThan, True, False, Unknown };

public static class StringValidation
{
    const string lessThan = "<";
    const string greaterThan = ">";
    const string isTrue = "true";
    const string isFalse = "false";
    
    public static ConditionalLogicType GetConditionLogicType(string conditionString, out string trimmedString)
    {

        ConditionalLogicType conditon = ConditionalLogicType.Unknown; 

        if(conditionString.Contains(lessThan))
        {
            conditon = ConditionalLogicType.LessThan;
            conditionString = conditionString.Trim('<');
        }
        if (conditionString.Contains(greaterThan))
        {
            conditon = ConditionalLogicType.GreaterThan;
            conditionString = conditionString.Trim('>');
        }
        if (conditionString.Contains(isFalse))
        {
            conditon = ConditionalLogicType.False;
           // conditionString = conditionString.Remove(0, 5); //Dont need to remove anything? Can just return the string as passed 
        }
        if (conditionString.Contains(isTrue))
        {
            conditon = ConditionalLogicType.True;
            //conditionString = conditionString.Remove(0, 4); //Dont need to remove anything? Can just return the string as passed 
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
            //Throw error that data type is not supported 
            return (T)(object)value;
        }

    }
}
