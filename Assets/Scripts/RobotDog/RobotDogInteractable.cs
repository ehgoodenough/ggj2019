﻿using FMOD.Studio;
using UnityEngine;

public class RobotDogInteractable : Interactable
{
    //GCHandle positionHandle;
    //Vector3 position;

    private EventInstance banterInstance;
    private bool isBantering;
    private bool isResponding;

    void Start()
    {
        //positionHandle = GCHandle.Alloc(position, GCHandleType.Pinned);
    }

    void Update() {
        //position.x = transform.position.x;
        //position.y = transform.position.y;
        //position.z = transform.position.z;

        PLAYBACK_STATE state;
        banterInstance.getPlaybackState(out state);
        if (!isResponding && state == PLAYBACK_STATE.STOPPING)
        {
            Debug.Log("Start Response");
            banterInstance.release();
            FMODUnity.RuntimeManager.PlayOneShotAttached("event:/SFX/Ambience/Dorg_Bark", gameObject);
            isResponding = true;
        }
        else if (state == PLAYBACK_STATE.STOPPED)
        {
            isResponding = false;
            isBantering = false;
        }
    }

    //void OnDestroy()
    //{
    //    positionHandle.Free();
    //}

    public override bool CanInteractWith(Pickupable heldItem)
    {
        return !isBantering;
    }

    public override void Interact(Pickupable heldItem)
    {
        Debug.Log("Start banter");
        isBantering = true;
        banterInstance = FMODUnity.RuntimeManager.CreateInstance("event:/VO/Dorg_Banter");

        //banterInstance.setCallback(BanterCallback, EVENT_CALLBACK_TYPE.SOUND_STOPPED);
        //banterInstance.setUserData(GCHandle.ToIntPtr(positionHandle));
        banterInstance.start();
    }

    //    private RESULT BanterCallback(EVENT_CALLBACK_TYPE type, EventInstance eventInstance, IntPtr parameters)
    //    {
    //        IntPtr positionPtr;
    //        eventInstance.getUserData(out positionPtr);
    //        GCHandle positionHandle = GCHandle.FromIntPtr(positionPtr);
    //        Vector3 position = (Vector3)positionHandle.Target;
    //        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Ambience/Dorg_Bark", position);
    //        return RESULT.OK;
    //    }
}
