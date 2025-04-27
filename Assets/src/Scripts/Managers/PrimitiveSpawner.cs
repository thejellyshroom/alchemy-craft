using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PrimitiveSpawner : MonoBehaviour
{
    public GameObject primitiveToSpawn;
    public Transform playerPosition;
    void Awake()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogWarning("Rigidbody component missing. Adding one.", gameObject);
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.isKinematic = true;
        rb.useGravity = false;

        UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grabInteractable == null)
        {
            Debug.LogWarning("XRGrabInteractable component missing. Adding one.", gameObject);
            grabInteractable = gameObject.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        }

        // Configure the interactable to be immovable
        grabInteractable.movementType = UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable.MovementType.Instantaneous;
        grabInteractable.throwOnDetach = false;

        //xr input
        grabInteractable.selectEntered.RemoveListener(HandleGrab);
        grabInteractable.selectEntered.AddListener(HandleGrab);
    }

    void Start()
    {
        playerPosition = GameObject.Find("Player").transform;
    }

    public void HandleGrab(SelectEnterEventArgs args = null) // Updated signature to match event
    {
        if (primitiveToSpawn == null)
        {
            Debug.LogError("Primitive to spawn is not assigned in the Inspector!", this.gameObject);
            return;
        }

        Vector3 spawnPosition = playerPosition.position + new Vector3(0, 0.5f, 0.5f);
        Instantiate(primitiveToSpawn, spawnPosition, transform.rotation);
        Debug.Log($"Spawned {primitiveToSpawn.name} from {this.gameObject.name}");
    }

    void OnDestroy()
    {
        UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(HandleGrab);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 0 is the left mouse button
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // Check if the hit object is THIS spawner
                if (hit.collider.gameObject == this.gameObject)
                {
                    // Call HandleGrab or a dedicated mouse spawn method
                    HandleGrab(); // Or potentially a new method HandleMouseSpawn()
                }
            }
        }
    }
}
