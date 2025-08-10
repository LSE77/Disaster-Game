using UnityEngine;

public class GameStartUI : MonoBehaviour
{
    public GameObject startPanel;       // ���� ȭ��(���+�ؽ�Ʈ)

    private bool started = false;

    void Start()
    {
        // �Ͻ����� + Ŀ�� ���̱� (���� ���)
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (startPanel) startPanel.SetActive(true);
    }

    void Update()
    {
        if (started) return;

        // �����̽��ٷ� ����
        if (Input.GetKeyDown(KeyCode.Space))
        {
            BeginGame();
        }
    }

    // ��ư�� ���ܵθ� ���� �����ص� ��
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

        // ����: CursorController ���� ������Ʈ��� �Ʒ� �� �ٷ� ��װ� �ص� OK
        // FindObjectOfType<CursorController>()?.LockCursor();
    }
}
