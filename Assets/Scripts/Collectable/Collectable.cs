using UnityEngine;

[RequireComponent(typeof(UniqueId))]
public class Collectable : MonoBehaviour
{
    private UniqueId uniqueIdComponent;
    public string id
    {
        get { return uniqueIdComponent.id; }
    }
    
    void Awake()
    {
        uniqueIdComponent = gameObject.GetComponent<UniqueId>();
        if (GameProgress.IsIdCollected(id))
        {
            gameObject.SetActive(false);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        GameProgress.Collect(id);
        gameObject.SetActive(false);
        Debug.Log(GameProgress.NumCollected);
    }
}
