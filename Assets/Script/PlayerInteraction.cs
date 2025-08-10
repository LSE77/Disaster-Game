using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interact")]
    public float interactDistance = 5f;

    [Header("Panels")]
    public GameObject repairPanel;       // repair UI panel
    public GameObject notPanel;          // "not enough hammers" panel

    [Header("Player Control")]
    public MonoBehaviour playerControllerScript; // movement controller to toggle

    [Header("Pipes (swap models)")]
    public GameObject pipeUp6;                 // broken A
    public GameObject pipeStraightShort7;      // broken B
    public GameObject pipeUp6_1;               // fixed A
    public GameObject pipeStraightShort7_1;    // fixed B

    [Header("Wall")]
    public GameObject wallObject;              // wall to disable after repair

    [Header("Repair SFX")]
    public AudioClip repairClip;               // e.g. hammer-6145.wav
    [Range(0f, 1f)] public float repairVolume = 1f;
    public bool playRepairAs2D = true;         // 2D recommended

    private bool panelOpenedOnce = false;

    void Update()
    {
        // open panel with mouse click on Interactable (only once)
        if (Input.GetMouseButtonDown(0) && !panelOpenedOnce &&
            ((repairPanel == null || !repairPanel.activeSelf) && (notPanel == null || !notPanel.activeSelf)))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
            {
                if (hit.collider.CompareTag("Interactable"))
                {
                    ShowRepairPanel();
                }
            }
        }

        // close panels with R
        if (repairPanel && repairPanel.activeSelf && Input.GetKeyDown(KeyCode.R))
        {
            CloseRepairPanel();  // apply repair
        }
        else if (notPanel && notPanel.activeSelf && Input.GetKeyDown(KeyCode.R))
        {
            ClosePanelsOnly();   // just close
        }

        // when any panel is open, lock player control and show cursor
        if ((repairPanel && repairPanel.activeSelf) || (notPanel && notPanel.activeSelf))
        {
            SetCursorAndControl(true);
        }
        else
        {
            SetCursorAndControl(false);
        }
    }

    public void ShowRepairPanel()
    {
        if (panelOpenedOnce) return;

        if (puthammer.hammerCount >= 3) // need 3 or more hammers
        {
            if (repairPanel) repairPanel.SetActive(true);
            if (notPanel) notPanel.SetActive(false);
        }
        else
        {
            if (notPanel) notPanel.SetActive(true);
            if (repairPanel) repairPanel.SetActive(false);
        }
    }

    // applies the repair, plays sfx, resets hammer inventory/UI, and closes panels
    public void CloseRepairPanel()
    {
        if (repairClip)
            StartCoroutine(PlayClipForSeconds(repairClip, 2f, transform.position));

        if (repairPanel) repairPanel.SetActive(false);
        if (notPanel) notPanel.SetActive(false);

        // swap pipe models
        if (pipeUp6) pipeUp6.SetActive(false);
        if (pipeStraightShort7) pipeStraightShort7.SetActive(false);
        if (pipeUp6_1) pipeUp6_1.SetActive(true);
        if (pipeStraightShort7_1) pipeStraightShort7_1.SetActive(true);

        // remove wall
        if (wallObject) wallObject.SetActive(false);

        // reset hammer inventory and UI (use puthammer helpers)
        puthammer.hammerCount = 0;
        puthammer.HideHammerUI();     // ensures icon & count are hidden when 0

        panelOpenedOnce = true;
    }

    public void ClosePanelsOnly()
    {
        if (repairPanel) repairPanel.SetActive(false);
        if (notPanel) notPanel.SetActive(false);
    }

    // cursor & player control toggle
    private void SetCursorAndControl(bool panelActive)
    {
        if (panelActive)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            if (playerControllerScript) playerControllerScript.enabled = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            if (playerControllerScript) playerControllerScript.enabled = true;
        }
    }

    // play a clip for N seconds from a detached AudioSource
    private IEnumerator PlayClipForSeconds(AudioClip clip, float seconds, Vector3 worldPos)
    {
        GameObject go = new GameObject("RepairAudio");
        var src = go.AddComponent<AudioSource>();
        src.clip = clip;
        src.volume = repairVolume;
        src.loop = false;

        if (playRepairAs2D)
        {
            src.spatialBlend = 0f; // 2D
        }
        else
        {
            src.spatialBlend = 1f; // 3D
            go.transform.position = worldPos;
            src.minDistance = 3f;
            src.maxDistance = 20f;
        }

        src.Play();
        float t = 0f;
        while (t < seconds)
        {
            t += Time.unscaledDeltaTime; // plays even if timeScale == 0
            yield return null;
        }
        if (src.isPlaying) src.Stop();
        Destroy(go);
    }
}
