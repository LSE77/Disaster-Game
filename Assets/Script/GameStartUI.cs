using UnityEngine;

public class GameStartUI : MonoBehaviour
{
    public GameObject startPanel;

    void Start()
    {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OnStartButton()
    {
        Debug.Log("Start Button Clicked"); // 클릭 여부 디버그 출력
        startPanel.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
