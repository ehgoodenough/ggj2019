using UnityEngine;

public class CollectableManager : MonoBehaviour
{
    void Start()
    {
        foreach (var collectable in GetComponentsInChildren<Collectable>())
        {
            if (GameProgress.IsIdCollected(collectable.id))
            {
                collectable.gameObject.SetActive(false);
            }
        }
    }
}
