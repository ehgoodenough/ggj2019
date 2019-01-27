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
    }

    void OnCollisionEnter(Collision collision)
    {
        InteractionDetector interactor = collision.gameObject.GetComponent<InteractionDetector>();
        if (interactor)
        {
            GameProgress.Collect(id);
            gameObject.SetActive(false);
        }
    }
}
