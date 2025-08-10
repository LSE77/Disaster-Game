using UnityEngine;

public class CursorController : MonoBehaviour
{
    public bool cursorLockOnStart = false; // 게임 시작시 커서 잠글지 여부

    void Start()
    {
        if (cursorLockOnStart)
        {
            LockCursor();
        }
        else
        {
            UnlockCursor();
        }
    }

    void Update()
    {
        // ESC 누르면 커서 다시 보이게 (중앙 고정 해제)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UnlockCursor();
        }
        // (선택) 마우스 왼쪽버튼 누르면 다시 숨기고 고정
        if (Input.GetMouseButtonDown(0) && Cursor.lockState != CursorLockMode.Locked)
        {
            LockCursor();
        }
    }

    public void LockCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void UnlockCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
