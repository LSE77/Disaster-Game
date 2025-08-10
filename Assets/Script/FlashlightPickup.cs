using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FlashlightPickup : MonoBehaviour
{
    [Header("Pickup")]
    public bool destroyOnPickup = true;
    public string interactKey = "e";         // optional: key pickup by distance
    public float interactRadius = 8f;         // key pickup radius
    public bool enableClickPickup = true;     // mouse click pickup
    public Camera cam;                        // if null => Camera.main
    public float clickMaxDistance = 100f;     // ray length for hover/click
    public LayerMask interactLayer = ~0;      // filter if needed

    [Header("Sound")]
    public AudioClip pickupClip;
    [Range(0f, 1f)] public float pickupVolume = 1f;
    public bool playAs2D = true;              // 2D = no spatialization

    [Header("Outline (cakeslice)")]
    public cakeslice.Outline outline;         // assign Outline on the flashlight object
    public bool outlineOnHover = true;        // turn on only when the cursor is on it

    private Transform player;
    private PlayerLightState playerState;
    private Collider selfCol;
    private bool picked;

    void Reset()
    {
        var col = GetComponent<Collider>();
        if (col) col.isTrigger = true;
    }

    void Awake()
    {
        selfCol = GetComponent<Collider>();
        if (!cam) cam = Camera.main;

        var p = GameObject.FindGameObjectWithTag("Player");
        if (p)
        {
            player = p.transform;
            playerState = p.GetComponent<PlayerLightState>();
            if (!playerState) playerState = p.AddComponent<PlayerLightState>();
        }

        // start with outline off
        SetOutline(false);

        if (outline == null)
            outline = GetComponent<cakeslice.Outline>();
    }

    void Update()
    {
        if (picked) return;

        // Mouse hover highlight and click pickup (like your hammer script)
        bool isHover = false;
        if (enableClickPickup && cam)
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, clickMaxDistance, interactLayer, QueryTriggerInteraction.Collide))
            {
                if (hit.collider && (hit.collider.gameObject == gameObject || hit.collider.transform.IsChildOf(transform)))
                {
                    isHover = true;

                    if (Input.GetMouseButtonDown(0))
                    {
                        DoPickup(hit.point);
                        return;
                    }
                }
            }
        }

        if (outlineOnHover) SetOutline(isHover);

        // Optional: key pickup within radius
        if (player && !string.IsNullOrEmpty(interactKey))
        {
            float dist = Vector3.Distance(player.position, transform.position);
            if (dist <= interactRadius && Input.GetKeyDown(interactKey))
            {
                DoPickup(transform.position);
                return;
            }
        }
    }

    // Optional: auto-pick when player touches trigger
    void OnTriggerEnter(Collider other)
    {
        if (picked) return;
        if (!other.CompareTag("Player")) return;
        // Comment out the next line if you don't want auto-pick by trigger
        DoPickup(transform.position);
    }

    public void DoPickup(Vector3 audioPos)
    {
        if (picked) return;
        picked = true;

        if (playerState == null && player)
        {
            playerState = player.GetComponent<PlayerLightState>();
            if (!playerState) playerState = player.gameObject.AddComponent<PlayerLightState>();
        }
        if (playerState != null) playerState.hasFlashlight = true;

        // play sound
        if (pickupClip)
        {
            if (playAs2D)
            {
                // 2D: create a temporary AudioSource at origin (spatialBlend=0)
                GameObject go = new GameObject("Pickup2D");
                var src = go.AddComponent<AudioSource>();
                src.clip = pickupClip;
                src.volume = pickupVolume;
                src.spatialBlend = 0f;
                src.Play();
                Destroy(go, pickupClip.length + 0.1f);
            }
            else
            {
                // 3D positional
                AudioSource.PlayClipAtPoint(pickupClip, audioPos, pickupVolume);
            }
        }

        SetOutline(false);

        if (destroyOnPickup) Destroy(gameObject);
        else gameObject.SetActive(false);
    }

    void SetOutline(bool on)
    {
        if (outline && outline.enabled != on)
            outline.enabled = on;
    }
}
