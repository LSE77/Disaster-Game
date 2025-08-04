using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float interactDistance = 5f;
    public GameObject repairPanel;
    public MonoBehaviour playerControllerScript; // 움직임 스크립트 Drag&Drop

    // 파이프 오브젝트 4개 변수 추가
    public GameObject pipeUp6;
    public GameObject pipeStraightShort7;
    public GameObject pipeUp6_1;
    public GameObject pipeStraightShort7_1;

    void Update()
    {
        // 클릭으로 패널 열기
        if (Input.GetMouseButtonDown(0) && (repairPanel == null || !repairPanel.activeSelf))
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

        // ESC로 패널 닫기
        if (repairPanel != null && repairPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseRepairPanel();
        }

        // 패널이 열려 있으면 커서 보이게, 움직임 비활성화
        if (repairPanel != null && repairPanel.activeSelf)
        {
            SetCursorAndControl(true);
        }
        else
        {
            SetCursorAndControl(false);
        }
    }

    public void ShowRepairPanel()
    {
        if (repairPanel != null)
        {
            repairPanel.SetActive(true);
        }
    }

public void DebugClick()
{
    Debug.Log("OK 버튼 눌림!");
}


    public void CloseRepairPanel()
    {
        Debug.Log("버튼 눌림!");
        if (repairPanel != null)
            repairPanel.SetActive(false);

        // 파이프 활성/비활성화 처리
        if (pipeUp6 != null)
            pipeUp6.SetActive(false);
        if (pipeStraightShort7 != null)
            pipeStraightShort7.SetActive(false);
        if (pipeUp6_1 != null)
            pipeUp6_1.SetActive(true);
        if (pipeStraightShort7_1 != null)
            pipeStraightShort7_1.SetActive(true);

        // 패널이 닫힐 때도 커서/조작상태 즉시 갱신
        SetCursorAndControl(false);
    }

    // 커서/플레이어컨트롤 상태 한 번에 처리하는 함수
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
