using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class FinalCinematicDynamicTimelineBinding : MonoBehaviour
{
    public PlayableDirector director;
    
    public void BindTimelineTracks()
    {
        Debug.Log("Binding Timeline Tracks!");
        TimelineAsset timelineAsset = (TimelineAsset)director.playableAsset;
        // iterate through tracks and map the objects appropriately

        int i = 0;
        foreach (PlayableBinding binding in timelineAsset.outputs)
        {
            Debug.Log("Binding[ " + i + " ]: " + binding);
            Debug.Log("Track: " + binding.sourceObject);
            Debug.Log("Object: " + director.GetGenericBinding(binding.sourceObject));

            if (director.GetGenericBinding(binding.sourceObject) == null)
            {
                if (i == 1)
                {
                    GameObject trackValue = GameObject.Find("friend_pieces");
                    Debug.Log("Set Binding to: " + trackValue);
                    director.SetGenericBinding((TrackAsset)binding.sourceObject, trackValue);
                }
                else if (i == 4)
                {
                    GameObject trackValue = GameObject.Find("friend_whole_parent").transform.GetChild(0).gameObject;
                    Debug.Log("Set Binding to: " + trackValue);
                    director.SetGenericBinding((TrackAsset)binding.sourceObject, trackValue);
                }
                else if (i == 8)
                {
                    GameObject trackValue = GameObject.Find("dorg");
                    Debug.Log("Set Binding to: " + trackValue);
                    director.SetGenericBinding((TrackAsset)binding.sourceObject, trackValue);
                }
            }

            i++;
        }
    }

    /// 0: [Referenced] TimelineCinematic (Animator)
    /// 1: [Find at Runtime] friend_pieces
    /// 2: [Referenced] Friend_Pieces_Apart
    /// 3: [Referenced] Friend_Pieces_Apart (Animator)
    /// 4: [Find at Runtime] friend_whole
    /// 5: [Referenced] AssemblyParticle
    /// 6: [Referenced] FinalCinematicCamera
    /// 7: [Referenced] camera Follow (Animator)
    /// 8: [Find at Runtime] dorg (Animator)
}
