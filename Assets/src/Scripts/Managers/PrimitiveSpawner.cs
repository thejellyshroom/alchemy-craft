using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class PrimitiveSpawner : MonoBehaviour
{
    public GameObject primitiveToSpawn;
    private Transform playerPosition;

    void Awake()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.isKinematic = true;
        rb.useGravity = false;

        UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grabInteractable == null)
        {
            grabInteractable = gameObject.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        }

        grabInteractable.movementType = UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable.MovementType.Kinematic;
        grabInteractable.throwOnDetach = false;

        // XR input: Hook up grab event
        grabInteractable.selectEntered.RemoveListener(HandleGrab); // Remove first to prevent duplicates
        grabInteractable.selectEntered.AddListener(HandleGrab);
    }

    void Start()
    {
        GameObject playerObject = GameObject.FindWithTag("Player"); // Replace "Player" with the actual tag you're using

        if (playerObject != null)
        {
            playerPosition = playerObject.transform;
        }
        else
        {
            Debug.LogError("Could not find GameObject with tag 'Player'. Make sure the object exists and is tagged correctly.");
        }
    }

    // Called by both XR grab and Mouse click
    public void HandleGrab(SelectEnterEventArgs args = null)
    {
        if (primitiveToSpawn == null)
        {
            Debug.LogError("Primitive to spawn is not assigned in the Inspector!", this.gameObject);
            return;
        }

        if (playerPosition == null)
        {
            Debug.LogError("Player Position Transform is not assigned in the Inspector!", this.gameObject);
            return;
        }

        Vector3 spawnPosition = playerPosition.position + playerPosition.forward * 3f + playerPosition.up * 1f;
        Instantiate(primitiveToSpawn, spawnPosition, playerPosition.rotation);
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
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.gameObject == this.gameObject)
                {
                    HandleGrab();
                }
            }
        }
    }
}
