using UnityEngine;

public class GameStartUI : MonoBehaviour
{
    public GameObject startPanel;       // 시작 화면(배경+텍스트)

    private bool started = false;

    void Start()
    {
        // 일시정지 + 커서 보이기 (시작 대기)
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (startPanel) startPanel.SetActive(true);
    }

    void Update()
    {
        if (started) return;

        // 스페이스바로 시작
        if (Input.GetKeyDown(KeyCode.Space))
        {
            BeginGame();
        }
    }

    // 버튼을 남겨두면 여기 연결해도 됨
    public void OnStartButton()
    {
        if (!started) BeginGame();
    }

    private void BeginGame()
    {
        started = true;

        if (startPanel) startPanel.SetActive(false);

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // 선택: CursorController 쓰는 프로젝트라면 아래 한 줄로 잠그게 해도 OK
        // FindObjectOfType<CursorController>()?.LockCursor();
    }
}
