using UnityEngine;

public class CursorController : MonoBehaviour
{
    void Start()
    {
        // 게임 시작하면 커서 숨기고 중앙에 고정
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // ESC 누르면 커서 다시 보이게 (중앙 고정 해제)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        // (선택) 마우스 왼쪽버튼 누르면 다시 숨기고 고정
        if (Input.GetMouseButtonDown(0) && Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
