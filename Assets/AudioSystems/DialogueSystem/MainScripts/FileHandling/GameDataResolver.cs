using UnityEngine;


/// <summary>
/// Allows handling of data, it creates a new hashtable called gameDataHashTable and provides public functions to add and remove from hashtable. Either handle game data in here or through a seperate script.
/// 
/// 
/// Author:: Sam Soctt
/// </summary>
//Namespace to avoid the unlikely chance that someone has a class/script of the same name. However, referencing will require including this namespace!
namespace DialogueSystem.DataResolver
{
    public enum GameDataReturnType { GameDataOk, GameDataDefault, GameDataUnknown };
    public class GameDataResolver : MonoBehaviour
    {
        public static GameDataResolver Instance => m_instance;
        private static GameDataResolver m_instance;

        private HashTable gameDataHashTable = new();

        private void Awake()
        {
            m_instance = this;
        }

        public void AddGameDataToHashTable<T>(uint id, T initialValue)
        {
            gameDataHashTable.AddDataToHashTable(id, initialValue);
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
}