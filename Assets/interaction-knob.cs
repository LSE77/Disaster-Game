using UnityEngine;

public class PlayerInteraction1 : MonoBehaviour
{
    [Header("Player Interaction Settings")]
    public float interactDistance = 5f;
    public GameObject repairPanel;
    public MonoBehaviour playerControllerScript; // 플레이어 움직임 스크립트

    [Header("Knob Objects")]
    public GameObject mainKnobOn;
    public GameObject mainKnobOff;

    [Header("Knob Distance Condition")]
    public Transform player; // 플레이어 Transform
    public Transform knobCenter; // Knob 중심 Transform
    public float knobActivateDistance = 3f; // 거리 제한

    private bool panelLockedAfterY = false; // Y를 누른 뒤에는 다시 열리지 않음

    void Update1()
    {
        // 🔹 F키로 패널 열기 (Y를 누른 뒤에는 열리지 않음)
        if (Input.GetKeyDown(KeyCode.F) 
            && !panelLockedAfterY 
            && (repairPanel == null || !repairPanel.activeSelf))
        {
            // 화면 중앙 기준으로 Ray 발사
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, interactDistance))
            {
                if (hit.collider.CompareTag("Interactable"))
                {
                    ShowRepairPanel1();
                }
            }
        }

        // 🔹 패널 열려있으면 Y/N 입력 처리
        if (repairPanel != null && repairPanel.activeSelf)
        {
            SetCursorAndControl1(true); // 커서 보이게, 움직임 끄기

            // Y키 → Knob 반전 + 패널 닫기 + 패널 잠금
            if (Input.GetKeyDown(KeyCode.Y))
            {
                ToggleKnob();
                panelLockedAfterY = true;
                CloseRepairPanel1();
            }

            // N키 → 변화 없이 패널 닫기
            if (Input.GetKeyDown(KeyCode.N))
            {
                CloseRepairPanel1();
            }
        }
    }

    private void ShowRepairPanel1()
    {
        if (repairPanel != null)
        {
            repairPanel.SetActive(true);
        }
    }

    private void ToggleKnob()
    {
        if (mainKnobOn != null && mainKnobOff != null && player != null && knobCenter != null)
        {
            float distance = Vector3.Distance(player.position, knobCenter.position);
            if (distance <= knobActivateDistance)
            {
                bool onState = mainKnobOn.activeSelf;
                mainKnobOn.SetActive(!onState);
                mainKnobOff.SetActive(onState);
            }
        }
    }

    private void CloseRepairPanel1()
    {
        if (repairPanel != null)
            repairPanel.SetActive(false);

        SetCursorAndControl1(false); // 커서 숨기고 플레이어 이동 활성화
    }

    private void SetCursorAndControl1(bool panelActive)
    {
        if (panelActive)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            if (playerControllerScript != null)
                playerControllerScript.enabled = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            if (playerControllerScript != null)
                playerControllerScript.enabled = true;
        }
    }
}
