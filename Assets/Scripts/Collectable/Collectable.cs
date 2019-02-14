using UnityEngine;

public class Collectable : MonoBehaviour
{
    void Awake()
    {
        if (GameProgress.IsIdCollected(transform.position))
        {
            gameObject.SetActive(false);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameProgress.Collect(transform.position);
            gameObject.SetActive(false);
            Debug.Log(GameProgress.NumCollected);
        }
    }
}
