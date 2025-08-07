using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float interactDistance = 5f;
    public GameObject repairPanel;              // 파이프 고치기 패널
    public GameObject notPanel;                 // "망치 부족" 패널 (에디터에서 연결)
    public MonoBehaviour playerControllerScript; // 플레이어 움직임 제어 스크립트

    // 파이프 오브젝트 4개 (에디터에서 연결)
    public GameObject pipeUp6;
    public GameObject pipeStraightShort7;
    public GameObject pipeUp6_1;
    public GameObject pipeStraightShort7_1;

    public GameObject wallObject;               // 벽 오브젝트 (에디터에서 연결)

    private bool panelOpenedOnce = false;       // 패널 한 번만 열리게 하는 플래그

    void Update()
    {
        // 1. 마우스 클릭으로 패널 열기 (아직 안 열었을 때만)
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

        // 2. R 키로 패널 닫기 (상황별 분기)
        if (repairPanel != null && repairPanel.activeSelf && Input.GetKeyDown(KeyCode.R))
        {
            CloseRepairPanel(); // 진짜 수리(파이프/벽 상태 변경)
        }
        else if (notPanel != null && notPanel.activeSelf && Input.GetKeyDown(KeyCode.R))
        {
            ClosePanelsOnly();  // 패널만 닫고 상태 변화 X
        }

        // 3. 패널이 열려 있으면 커서 보이고, 움직임 비활성화
        if ((repairPanel != null && repairPanel.activeSelf) || (notPanel != null && notPanel.activeSelf))
        {
            SetCursorAndControl(false);
        }
    }

    // 패널 보이기 함수 (망치 3개 체크)
    public void ShowRepairPanel()
    {
        if (!panelOpenedOnce)
        {
            // ★ hammerCount는 puthammer 스크립트의 public static 변수임!
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

    // 패널 닫기 함수 (파이프와 벽 상태 변경도 여기서!)
    public void CloseRepairPanel()
    {
        Debug.Log("버튼 눌림!");
        if (repairPanel != null)
            repairPanel.SetActive(false);
        if (notPanel != null)
            notPanel.SetActive(false);

        // 파이프 오브젝트들 상태 변경
        if (pipeUp6 != null)
            pipeUp6.SetActive(false);
        if (pipeStraightShort7 != null)
            pipeStraightShort7.SetActive(false);
        if (pipeUp6_1 != null)
            pipeUp6_1.SetActive(true);
        if (pipeStraightShort7_1 != null)
            pipeStraightShort7_1.SetActive(true);

        // 벽 오브젝트 비활성화 (파이프 고치면 벽이 사라짐)
        if (wallObject != null)
            wallObject.SetActive(false);

        // 한 번 닫힌 뒤 다시 열리지 않게 설정
        panelOpenedOnce = true;

        // 커서와 움직임 상태 원상복귀
        SetCursorAndControl(false);
    }

    // ★ 패널만 닫는 함수 (상태 변화 X)
    public void ClosePanelsOnly()
    {
        if (repairPanel != null)
            repairPanel.SetActive(false);
        if (notPanel != null)
            notPanel.SetActive(false);

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
