using UnityEngine;
using System.Collections;

public class Gate : MonoBehaviour
{
    public Button buttonThatOpensGate;
    public float openingSpeed = 2f;

    private bool isOpen = false;
    private float startYPosition;

    private void Start()
    {
        EventBus.Subscribe<TestOpenGateEvent>(OnButtonPressedEvent);
    }

    private void OpenGate()
    {
        Debug.Log("Open Gate");
        isOpen = true;
        startYPosition = transform.position.y;
        StartCoroutine(OpenGateAnimation());
    }

    private IEnumerator OpenGateAnimation()
    {
        while (Mathf.Abs(startYPosition - transform.position.y) < 4f)
        {
            transform.position = transform.position - Vector3.up * Time.deltaTime * openingSpeed;
            yield return null;
        }
    }

    private void OnButtonPressedEvent(TestOpenGateEvent e)
    {
        Debug.Log("OnButtonPressedEvent");
        if (e.buttonPressed == buttonThatOpensGate)
        {
            OpenGate();
        }
    }
}
