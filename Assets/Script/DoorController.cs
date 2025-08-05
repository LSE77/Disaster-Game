using UnityEngine;
using TMPro;

public class DoorController : MonoBehaviour
{
    public TextMeshProUGUI doorMessage;  // "E�� ���� ���踦 ����ϼ���"
    public GameObject closedDoor;        // ���� �� ������Ʈ
    public GameObject openedDoor;        // ���� �� ������Ʈ (�ʱ� ��Ȱ��ȭ)
    public KeyPickup keyPickup;          // ���� ��ũ��Ʈ ����

    private bool isPlayerNearby = false;
    private bool doorOpened = false;

    void Start()
    {
        doorMessage.gameObject.SetActive(false);
        if (openedDoor != null)
            openedDoor.SetActive(false); // ���� �� ���� ���� ��Ȱ��ȭ
    }

    void Update()
    {
        if (isPlayerNearby && !doorOpened && keyPickup == null) // ���踦 ���� ����
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                OpenDoor();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !doorOpened)
        {
            isPlayerNearby = true;
            if (keyPickup == null) // ���踦 ������ ���� ���� �޽��� ǥ��
                doorMessage.gameObject.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            doorMessage.gameObject.SetActive(false);
        }
    }

    void OpenDoor()
    {
        doorOpened = true;
        doorMessage.gameObject.SetActive(false);

        // ���� �� ��Ȱ��ȭ & ���� �� Ȱ��ȭ
        closedDoor.SetActive(false);
        openedDoor.SetActive(true);

        Debug.Log("���� ���Ƚ��ϴ�!");
    }
}
