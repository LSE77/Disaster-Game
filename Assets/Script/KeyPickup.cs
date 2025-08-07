using UnityEngine;
using UnityEngine.UI;

public class KeyPickup : MonoBehaviour
{
    public Image keyIconUI;
    public float pickupDistance = 10f;

    public static bool hasKey = false;  //  ���Ⱑ �߿�!

    private bool isCollected = false;

    void Update()
    {
        if (!isCollected && Input.GetMouseButtonDown(0))
        {
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, pickupDistance))
            {
                if (hit.collider.gameObject == this.gameObject)
                {
                    CollectKey();
                }
            }
        }
    }

    void CollectKey()
    {
        isCollected = true;
        hasKey = true;  //  Ű�� ����ٰ� ǥ��
        if (keyIconUI != null)
            keyIconUI.gameObject.SetActive(true);

        Destroy(gameObject);
    }
}
