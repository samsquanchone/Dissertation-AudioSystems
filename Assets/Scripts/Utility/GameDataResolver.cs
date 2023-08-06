using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This could all be static to avoid the need for references to it, then the game programmers can just add as they please
/// </summary>
public enum GameDataReturnType { GameDataOk, GameDataDefault, GameDataUnknown };
public class GameDataResolver : MonoBehaviour
{
    public static GameDataResolver Instance => m_instance;
    private static GameDataResolver m_instance;

    private HashTable gameDataHashTable = new();

    private void Start()
    {
        m_instance = this;
        GameDataValue test = new((uint)200);
        GameDataValue testBool = new((bool)false);
        gameDataHashTable.AddGameDataToHashTable((uint)1, test);
        gameDataHashTable.AddGameDataToHashTable((uint)2, testBool);
        SetGameDataVariable((uint)1, 25);
       
    }

   public void SetGameDataVariable<T>(uint key, in T inValue)
   {
        GameDataValue value = (GameDataValue)gameDataHashTable.hashTable[key];

        value.m_current = inValue;
        value.m_set = true;

   }

    public GameDataReturnType QuerryGameData<T>(uint key, out T outValue)
    {
        //Create an instance of gamedata value so we can cast the hash table value to it
        GameDataValue value = (GameDataValue)gameDataHashTable.hashTable[key];

        if (value != null)
        {

            if (value.m_set)
            {
                outValue = value.m_current;

                return GameDataReturnType.GameDataOk;
            }

            else
            {
                outValue = value.m_default;
                return GameDataReturnType.GameDataDefault;
            }
        }
        else
        {
            //Return default of the passed type, as we cant assume the type as it is generic
            outValue = default(T); 
            return GameDataReturnType.GameDataUnknown;
        }
    }




}
