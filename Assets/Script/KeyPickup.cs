using UnityEngine;
using UnityEngine.UI;

public class KeyPickup : MonoBehaviour
{
    public Image keyIconUI;
    public float pickupDistance = 10f;

    [Header("Sound")]
    public AudioClip pickupClip;           // <- key-get-39925.wav 드래그
    [Range(0f, 1f)] public float volume = 1f;
    public bool playAs2D = true;           // true면 2D, false면 3D

    public static bool hasKey = false;

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
                    CollectKey(hit.point);
                }
            }
        }
    }

    void CollectKey(Vector3 hitPoint)
    {
        isCollected = true;
        hasKey = true;

        if (keyIconUI != null)
            keyIconUI.gameObject.SetActive(true);

        // ▼ 사운드 재생
        if (pickupClip != null)
        {
            if (playAs2D)
            {
                // 2D 재생
                GameObject go = new GameObject("KeyPickupAudio");
                var src = go.AddComponent<AudioSource>();
                src.clip = pickupClip;
                src.volume = volume;
                src.spatialBlend = 0f;
                src.Play();
                Destroy(go, pickupClip.length + 0.1f);
            }
            else
            {
                // 3D 재생
                AudioSource.PlayClipAtPoint(pickupClip, hitPoint, volume);
            }
        }

        Destroy(gameObject);
    }
}
