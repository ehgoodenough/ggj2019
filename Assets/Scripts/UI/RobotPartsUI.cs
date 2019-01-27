using UnityEngine;

public class RobotPartsUI : MonoBehaviour
{
    public GameObject head;
    public GameObject body;
    public GameObject leftArm;
    public GameObject rightArm;
    public GameObject leftWheel;
    public GameObject rightWheel;

    public void FindHead()
    {
        head.SetActive(true);
    }

    public void FindBody()
    {
        body.SetActive(true);
    }

    public void FindLeftArm()
    {
        leftArm.SetActive(true);
    }

    public void FindRightArm()
    {
        rightArm.SetActive(true);
    }

    public void FindLeftWheel()
    {
        leftWheel.SetActive(true);
    }

    public void FindRightWheel()
    {
        rightWheel.SetActive(true);
    }
}
