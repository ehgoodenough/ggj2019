using UnityEngine;
using System.Collections;

public class GameStateSingleton : MonoBehaviour
{
    public static GameStateSingleton Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            DestroyImmediate(this.gameObject);
        }
    }
}
