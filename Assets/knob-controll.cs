using UnityEngine;

public class MainKnobController : MonoBehaviour
{
    public GameObject knobOn;
    public GameObject knobOff;

    void Start()
    {
        if (knobOn != null && knobOff != null)
        {
            knobOn.SetActive(true);
            knobOff.SetActive(false);
        }
    }

    public void TurnOffKnob()
    {
        if (knobOn != null && knobOff != null)
        {
            knobOn.SetActive(false);
            knobOff.SetActive(true);
            Debug.Log("차단기가 내려갔습니다.");
        }
    }
}