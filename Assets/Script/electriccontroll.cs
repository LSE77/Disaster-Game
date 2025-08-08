using UnityEngine;

public class ElectricPanelInteraction : MonoBehaviour
{
    [Header("상호작용 거리")]
    public float interactDistance = 5f;

    [Header("전자 패널 UI")]
    public GameObject electricPanelUI;

    [Header("플레이어 조작 스크립트")]
    public MonoBehaviour playerControllerScript;

    [Header("Main Knob 상태 오브젝트")]
    public GameObject mainKnobOn;
    public GameObject mainKnobOff;

    private bool interactionLocked = false; // 클릭 후 재사용 금지
    private bool panelOpen = false;
    private bool panelJustOpened = false;   // 첫 클릭 후 즉시 knob 반응 방지용

    void Update()
    {
        // 패널 열기
        if (Input.GetMouseButtonDown(0) && !panelOpen && !interactionLocked &&
            (electricPanelUI == null || !electricPanelUI.activeSelf))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactDistance))
            {
                if (hit.collider.CompareTag("electric") && hit.collider.gameObject == this.gameObject)
                {
                    OpenPanel();
                }
            }
        }

        // R 키로 닫기 (재사용 가능)
        if (panelOpen && electricPanelUI.activeSelf && Input.GetKeyDown(KeyCode.R))
        {
            ClosePanelWithoutLock();
        }

        // 패널 열린 상태에서 마우스 클릭 시 상태 전환 (단, 막 열렸을 경우는 제외)
        if (panelOpen && electricPanelUI.activeSelf && Input.GetMouseButtonDown(0) && !panelJustOpened)
        {
            ConfirmInteractionAndDelayKnobSwitch();
        }

        // 첫 프레임 이후에는 false로 되돌리기
        if (panelJustOpened)
        {
            panelJustOpened = false;
        }

        // UI 열린 동안 조작 잠금
        if (panelOpen)
        {
            SetCursorAndControl(false);
        }
    }

    private void OpenPanel()
    {
        if (electricPanelUI != null)
        {
            electricPanelUI.SetActive(true);
            panelOpen = true;
            panelJustOpened = true; // 이 프레임에서는 knob 작동 못하게 막음
            SetCursorAndControl(false);
        }
    }

    private void ClosePanelWithoutLock()
    {
        if (electricPanelUI != null)
        {
            electricPanelUI.SetActive(false);
        }

        panelOpen = false;
        SetCursorAndControl(true);
    }

    private void ConfirmInteractionAndDelayKnobSwitch()
    {
        Debug.Log(">> UI 닫힘. 1초 후 Knob 상태 전환");

        if (electricPanelUI != null)
        {
            electricPanelUI.SetActive(false);
        }

        panelOpen = false;
        interactionLocked = true;

        SetCursorAndControl(true);

        // 1초 후 knob 상태 전환
        Invoke(nameof(SwitchKnobState), 1f);
    }

    private void SwitchKnobState()
    {
        Debug.Log(">> Knob 상태 전환 실행");

        if (mainKnobOn != null) mainKnobOn.SetActive(false);
        if (mainKnobOff != null) mainKnobOff.SetActive(true);
    }

    private void SetCursorAndControl(bool enablePlayerControl)
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (playerControllerScript != null)
            playerControllerScript.enabled = enablePlayerControl;
    }
}

