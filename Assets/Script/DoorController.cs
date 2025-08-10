using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DoorController : MonoBehaviour
{
    public TextMeshProUGUI doorMessage;
    public GameObject closedDoor;
    public GameObject openedDoor;

    [Header("Key UI")]
    public Image keyIconUI;                 // assign the key UI Image

    [Header("Sounds")]
    public AudioClip keyUseClip;            // plays when key UI disappears
    public AudioClip doorOpenClip;          // plays when door opens
    [Range(0f, 1f)] public float volume = 1f;
    public float playSeconds = 2.5f;        // duration for door sound
    public bool playAs2D = true;            // 2D recommended

    private bool isPlayerNearby;
    private bool doorOpened;

    void Start()
    {
        if (doorMessage) doorMessage.gameObject.SetActive(false);
        if (openedDoor) openedDoor.SetActive(false);
    }

    void Update()
    {
        if (isPlayerNearby && !doorOpened && KeyPickup.hasKey && Input.GetKeyDown(KeyCode.R))
        {
            OpenDoor();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || doorOpened) return;
        isPlayerNearby = true;

        if (doorMessage)
        {
            doorMessage.text = KeyPickup.hasKey
                ? "Press the R key to open the door."
                : "You need a key to open the door. The area near the car is suspicious.";
            doorMessage.gameObject.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        isPlayerNearby = false;
        if (doorMessage) doorMessage.gameObject.SetActive(false);
    }

    void OpenDoor()
    {
        doorOpened = true;

        if (doorMessage) doorMessage.gameObject.SetActive(false);

        // 1) consume key: hide key UI and play key-use sound right at this moment
        if (KeyPickup.hasKey)
        {
            if (keyIconUI) keyIconUI.gameObject.SetActive(false);
            if (keyUseClip) StartCoroutine(PlayDetachedSFX(keyUseClip, 0.75f, transform.position)); // short blip
            KeyPickup.hasKey = false;
        }

        // 2) play door opening sound (detached so it won't stop)
        if (doorOpenClip) StartCoroutine(PlayDetachedSFX(doorOpenClip, playSeconds, transform.position));

        // 3) swap door visuals
        if (closedDoor) closedDoor.SetActive(false);
        if (openedDoor) openedDoor.SetActive(true);
    }

    IEnumerator PlayDetachedSFX(AudioClip clip, float seconds, Vector3 pos)
    {
        if (clip == null) yield break;

        var go = new GameObject("DetachedSFX");
        var src = go.AddComponent<AudioSource>();
        src.clip = clip;
        src.volume = volume;
        src.loop = false;

        if (playAs2D)
        {
            src.spatialBlend = 0f; // 2D
        }
        else
        {
            src.spatialBlend = 1f; // 3D positional
            go.transform.position = pos;
            src.minDistance = 2f;
            src.maxDistance = 20f;
        }

        src.Play();

        // unscaled so it still plays if timeScale == 0
        float t = 0f;
        while (t < seconds) { t += Time.unscaledDeltaTime; yield return null; }

        if (src.isPlaying) src.Stop();
        Destroy(go);
    }
}
