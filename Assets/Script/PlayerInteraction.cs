using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    public float interactDistance = 5f;
    public GameObject repairPanel;              // 파이프 고치기 패널
    public GameObject notPanel;                 // "망치 부족" 패널
    public MonoBehaviour playerControllerScript; // 플레이어 움직임 제어 스크립트

    // 파이프 오브젝트 4개
    public GameObject pipeUp6;
    public GameObject pipeStraightShort7;
    public GameObject pipeUp6_1;
    public GameObject pipeStraightShort7_1;

    public GameObject wallObject;               // 벽 오브젝트

    public Image hammerIconUI;                  // 🔨 망치 UI 아이콘
    public TMP_Text hammerCountText;            // 🔨 망치 개수 텍스트

    private bool panelOpenedOnce = false;

    void Update()
    {
        // 마우스 클릭으로 상호작용
        if (Input.GetMouseButtonDown(0) && !panelOpenedOnce &&
            ((repairPanel == null || !repairPanel.activeSelf) && (notPanel == null || !notPanel.activeSelf)))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, interactDistance))
            {
                if (hit.collider.CompareTag("Interactable"))
                {
                    ShowRepairPanel();
                }
            }
        }

        // R키로 닫기
        if (repairPanel != null && repairPanel.activeSelf && Input.GetKeyDown(KeyCode.R))
        {
            CloseRepairPanel();
        }
        else if (notPanel != null && notPanel.activeSelf && Input.GetKeyDown(KeyCode.R))
        {
            ClosePanelsOnly();
        }

        // 패널이 열려 있으면 조작 잠금
        if ((repairPanel != null && repairPanel.activeSelf) || (notPanel != null && notPanel.activeSelf))
        {
            SetCursorAndControl(false);
        }
    }

    // 수리 패널 열기
    public void ShowRepairPanel()
    {
        if (!panelOpenedOnce)
        {
            if (puthammer.hammerCount >= 3)
            {
                if (repairPanel != null)
                    repairPanel.SetActive(true);
                if (notPanel != null)
                    notPanel.SetActive(false);
            }
            else
            {
                if (notPanel != null)
                    notPanel.SetActive(true);
                if (repairPanel != null)
                    repairPanel.SetActive(false);
            }
        }
    }

    // 수리 후 닫기 + 망치 0으로 초기화
    public void CloseRepairPanel()
    {
        Debug.Log("수리 완료");

        if (repairPanel != null)
            repairPanel.SetActive(false);
        if (notPanel != null)
            notPanel.SetActive(false);

        // 파이프 교체
        if (pipeUp6 != null) pipeUp6.SetActive(false);
        if (pipeStraightShort7 != null) pipeStraightShort7.SetActive(false);
        if (pipeUp6_1 != null) pipeUp6_1.SetActive(true);
        if (pipeStraightShort7_1 != null) pipeStraightShort7_1.SetActive(true);

        // 벽 제거
        if (wallObject != null) wallObject.SetActive(false);

        // 망치 초기화
        puthammer.hammerCount = 0;

        // UI도 초기화
        if (hammerIconUI != null) hammerIconUI.gameObject.SetActive(true);
        if (hammerCountText != null)
        {
            hammerCountText.gameObject.SetActive(true);
            hammerCountText.text = "0"; // ← 여기 수정됨!
        }

        panelOpenedOnce = true;

        SetCursorAndControl(false);
    }

    // 닫기 (상태 변화 없음)
    public void ClosePanelsOnly()
    {
        if (repairPanel != null) repairPanel.SetActive(false);
        if (notPanel != null) notPanel.SetActive(false);
        SetCursorAndControl(false);
    }

    // 커서 및 플레이어 조작 제어
    private void SetCursorAndControl(bool panelActive)
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
