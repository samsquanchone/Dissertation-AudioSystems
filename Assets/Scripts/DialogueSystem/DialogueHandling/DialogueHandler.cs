using System;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Threading;

namespace DialogueUtility
{
    public enum SequenceType {RandomOneShot, Sequential, PlayerResponse };
    public class DialogueHandler
    {
        FMOD.Studio.EVENT_CALLBACK dialogueCallback;

        private string dialogueKey;
        private FMODUnity.EventReference fmodEvent;
        private Transform emitterTransform;

        //Pass in a callback version enum val
        public DialogueHandler(string key, FMODUnity.EventReference eventName, Transform _emitterTransform)
        {
            this.dialogueKey = key;
            this.fmodEvent = eventName;
            this.emitterTransform = _emitterTransform;
            dialogueCallback = new FMOD.Studio.EVENT_CALLBACK(DialogueEventCallback); //Will need to call this from sending script if trying to make this a static helper class

            PlayDialogue(dialogueKey, fmodEvent, emitterTransform);
        }




#if UNITY_EDITOR
        void Reset()
        {
            //EventName = FMODUnity.EventReference.Find("event:/Character/Radio/Command");
        }
#endif

        void Start()
        {

            // Explicitly create the delegate object and assign it to a member so it doesn't get freed
            // by the garbage collected while it's being used
            //  dialogueCallback = new FMOD.Studio.EVENT_CALLBACK(DialogueEventCallback); //Will need to call this from sending script if trying to make this a static helper class
        }

        public void PlayDialogue(string key, FMODUnity.EventReference eventName, Transform emitterTransform)
        {
            var dialogueInstance = FMODUnity.RuntimeManager.CreateInstance(eventName);

            // Pin the key string in memory and pass a pointer through the user data
            GCHandle stringHandle = GCHandle.Alloc(key);
            dialogueInstance.setUserData(GCHandle.ToIntPtr(stringHandle));

            dialogueInstance.setCallback(dialogueCallback);

            if (emitterTransform != null)
            {
                FMODUnity.RuntimeManager.AttachInstanceToGameObject(dialogueInstance, emitterTransform);
            }

            dialogueInstance.start();
            dialogueInstance.release();
        }

        [AOT.MonoPInvokeCallback(typeof(FMOD.Studio.EVENT_CALLBACK))] //Put in static class
        static FMOD.RESULT DialogueEventCallback(FMOD.Studio.EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
        {
            FMOD.Studio.EventInstance instance = new FMOD.Studio.EventInstance(instancePtr);

            // Retrieve the user data
            IntPtr stringPtr;
            instance.getUserData(out stringPtr);

            // Get the string object
            GCHandle stringHandle = GCHandle.FromIntPtr(stringPtr);
            String key = stringHandle.Target as String;

            switch (type)
            {
                case FMOD.Studio.EVENT_CALLBACK_TYPE.CREATE_PROGRAMMER_SOUND:
                    {
                        FMOD.MODE soundMode = FMOD.MODE.LOOP_NORMAL | FMOD.MODE.CREATECOMPRESSEDSAMPLE | FMOD.MODE.NONBLOCKING;
                        var parameter = (FMOD.Studio.PROGRAMMER_SOUND_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.PROGRAMMER_SOUND_PROPERTIES));


                        if (key.Contains("."))
                        {
                            FMOD.Sound dialogueSound;
                            var soundResult = FMODUnity.RuntimeManager.CoreSystem.createSound(Application.streamingAssetsPath + "/" + key, soundMode, out dialogueSound);
                            if (soundResult == FMOD.RESULT.OK)
                            {
                                parameter.sound = dialogueSound.handle;
                                parameter.subsoundIndex = -1;
                                Marshal.StructureToPtr(parameter, parameterPtr, false);
                            }
                        }
                        else
                        {
                            FMOD.Studio.SOUND_INFO dialogueSoundInfo;
                            var keyResult = FMODUnity.RuntimeManager.StudioSystem.getSoundInfo(key, out dialogueSoundInfo);
                            if (keyResult != FMOD.RESULT.OK)
                            {

                                break;
                            }
                            FMOD.Sound dialogueSound;
                            var soundResult = FMODUnity.RuntimeManager.CoreSystem.createSound(dialogueSoundInfo.name_or_data, soundMode | dialogueSoundInfo.mode, ref dialogueSoundInfo.exinfo, out dialogueSound);
                            if (soundResult == FMOD.RESULT.OK)
                            {
                                parameter.sound = dialogueSound.handle;
                                parameter.subsoundIndex = dialogueSoundInfo.subsoundindex;
                                Marshal.StructureToPtr(parameter, parameterPtr, false);
                            }
                        }
                        break;
                    }
                case FMOD.Studio.EVENT_CALLBACK_TYPE.DESTROY_PROGRAMMER_SOUND:
                    {
                        var parameter = (FMOD.Studio.PROGRAMMER_SOUND_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.PROGRAMMER_SOUND_PROPERTIES));
                        var sound = new FMOD.Sound(parameter.sound);
                        sound.release();

                        break;
                    }
                case FMOD.Studio.EVENT_CALLBACK_TYPE.DESTROYED:
                    {
                        // Now the event has been destroyed, unpin the string memory so it can be garbage collected
                        stringHandle.Free();

                        break;
                    }
            }
            return FMOD.RESULT.OK;
        }


       

    }
    

    public class DialogueInfoHandler
    {
        FMOD.Studio.EVENT_CALLBACK dialogueInfoCallback;
        private string _key;
        private FMODUnity.EventReference _eventName;
        private static uint _length;
        public DialogueInfoHandler(string key, FMODUnity.EventReference eventName)
        {
            this._key = key;
            this._eventName = eventName;
            dialogueInfoCallback = new FMOD.Studio.EVENT_CALLBACK(DialogueEventInfoCallback);

            ExecuteDialogueInfoCallback();
        }

        void ExecuteDialogueInfoCallback()
        {
            var dialogueInstance = FMODUnity.RuntimeManager.CreateInstance(_eventName);

            // Pin the key string in memory and pass a pointer through the user data
            GCHandle stringHandle = GCHandle.Alloc(_key);
            dialogueInstance.setUserData(GCHandle.ToIntPtr(stringHandle));

            dialogueInstance.setCallback(dialogueInfoCallback);

            //int ms = 10;
            //Thread.Sleep(ms);

            dialogueInstance.start();

            dialogueInstance.release();
        }

        public uint GetDialogueLength()
        {
            return _length;
        }

        public static void SetDialogueLength(uint length)
        {
            _length = length;
        }


        [AOT.MonoPInvokeCallback(typeof(FMOD.Studio.EVENT_CALLBACK))] //Put in static class
        static FMOD.RESULT DialogueEventInfoCallback(FMOD.Studio.EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
        {


            FMOD.Studio.EventInstance instance = new FMOD.Studio.EventInstance(instancePtr);

            // Retrieve the user data
            IntPtr stringPtr;
            instance.getUserData(out stringPtr);

            // Get the string object
            GCHandle stringHandle = GCHandle.FromIntPtr(stringPtr);
            String key = stringHandle.Target as String;

            switch (type)
            {
                case FMOD.Studio.EVENT_CALLBACK_TYPE.CREATE_PROGRAMMER_SOUND:
                    {
                        //We are using this callback to get a audio table clip length, so we only want a readonly sound (AKA cheapeast sound mode ;) )
                        FMOD.MODE soundMode = FMOD.MODE.OPENONLY;
                        var parameter = (FMOD.Studio.PROGRAMMER_SOUND_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.PROGRAMMER_SOUND_PROPERTIES));


                        if (key.Contains("."))
                        {
                            FMOD.Sound dialogueSound;
                            var soundResult = FMODUnity.RuntimeManager.CoreSystem.createSound(Application.streamingAssetsPath + "/" + key, soundMode, out dialogueSound);
                            if (soundResult == FMOD.RESULT.OK)
                            {
                                parameter.sound = dialogueSound.handle;
                                parameter.subsoundIndex = -1;
                                Marshal.StructureToPtr(parameter, parameterPtr, false);
                            }
                        }
                        else
                        {
                            FMOD.Studio.SOUND_INFO dialogueSoundInfo;
                            var keyResult = FMODUnity.RuntimeManager.StudioSystem.getSoundInfo(key, out dialogueSoundInfo);
                            if (keyResult != FMOD.RESULT.OK)
                            {
                                
                                break;
                            }
                            FMOD.Sound dialogueSound;
                            var soundResult = FMODUnity.RuntimeManager.CoreSystem.createSound(dialogueSoundInfo.name_or_data, soundMode | dialogueSoundInfo.mode, ref dialogueSoundInfo.exinfo, out dialogueSound);
                            if (soundResult == FMOD.RESULT.OK)
                            {
                                FMOD.Sound subSound;
                                parameter.sound = dialogueSound.handle;
                                parameter.subsoundIndex = dialogueSoundInfo.subsoundindex;
                                dialogueSound.getSubSound(dialogueSoundInfo.subsoundindex, out subSound);
                                subSound.getLength(out uint length, FMOD.TIMEUNIT.MS);
                                Debug.Log("Length: " + length);
                                SetDialogueLength(length);
                                Marshal.StructureToPtr(parameter, parameterPtr, false);
                            }
                        }
                        break;
                    }
                case FMOD.Studio.EVENT_CALLBACK_TYPE.DESTROY_PROGRAMMER_SOUND:
                    {
                        var parameter = (FMOD.Studio.PROGRAMMER_SOUND_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.PROGRAMMER_SOUND_PROPERTIES));
                        var sound = new FMOD.Sound(parameter.sound);
                        sound.release();

                        break;
                    }
                case FMOD.Studio.EVENT_CALLBACK_TYPE.DESTROYED:
                    {
                        // Now the event has been destroyed, unpin the string memory so it can be garbage collected
                        stringHandle.Free();

                        break;
                    }
            }
            return FMOD.RESULT.OK;
        }

       
    }
    
}