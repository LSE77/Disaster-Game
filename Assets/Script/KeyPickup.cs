using UnityEngine;
using UnityEngine.UI;

public class KeyPickup : MonoBehaviour
{
    public Image keyIconUI;
    public float pickupDistance = 10f;

    public static bool hasKey = false;  //  여기가 중요!

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
        hasKey = true;  //  키를 얻었다고 표시
        if (keyIconUI != null)
            keyIconUI.gameObject.SetActive(true);

        Destroy(gameObject);
    }
}
