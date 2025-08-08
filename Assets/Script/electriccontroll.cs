using UnityEngine;
using System.Collections;

public class ElectricPanelInteraction : MonoBehaviour
{
    [Header("상호작용 거리")]
    public float interactDistance = 5f;

    [Header("연결된 UI 패널")]
    public GameObject electricPanelUI;

    [Header("플레이어 조작 스크립트")]
    public MonoBehaviour playerControllerScript;

    [Header("MainKnob 오브젝트")]
    public GameObject mainKnobOn;
    public GameObject mainKnobOff;

    private bool panelOpen = false;
    private bool interactionLocked = false;  // knob 조작 이후 재상호작용 방지
    private bool justOpened = false;         // 패널 연 직후 한 프레임 knob 조작 방지

    void Update()
    {
        // 패널이 열려있고 R 키 입력 → 닫기
        if (panelOpen && Input.GetKeyDown(KeyCode.R))
        {
            ClosePanel();
            return;
        }

        // 마우스 클릭 시
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactDistance))
            {
                // 1. 패널이 닫힌 상태이고 상호작용 가능한 오브젝트인지 확인
                if (!panelOpen && !interactionLocked)
                {
                    if (hit.collider.CompareTag("electric"))
                    {
                        // 연결된 electricPanelUI만 열도록 제한
                        if (electricPanelUI != null && !electricPanelUI.activeSelf)
                        {
                            electricPanelUI.SetActive(true);
                            playerControllerScript.enabled = false;
                            panelOpen = true;
                            justOpened = true;  // 한 프레임 동안 knob 조작 방지
                        }
                    }
                }

                // 2. 패널이 열린 상태에서 마우스 클릭 → knob 전환
                else if (panelOpen && !justOpened)
                {
                    if (mainKnobOn != null && mainKnobOff != null)
                    {
                        StartCoroutine(HandleKnobSwitch());
                    }
                }
            }
        }

        // justOpened는 한 프레임만 유지
        if (justOpened)
        {
            justOpened = false;
        }
    }

    void ClosePanel()
    {
        if (electricPanelUI != null)
            electricPanelUI.SetActive(false);

        playerControllerScript.enabled = true;
        panelOpen = false;
    }

    IEnumerator HandleKnobSwitch()
    {
        ClosePanel();
        yield return new WaitForSeconds(1f);

        mainKnobOn.SetActive(false);
        mainKnobOff.SetActive(true);

        interactionLocked = true;
    }
}
