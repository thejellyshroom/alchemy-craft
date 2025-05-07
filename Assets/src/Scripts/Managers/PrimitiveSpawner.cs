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

        rb.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;

        UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grabInteractable == null)
        {
            grabInteractable = gameObject.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        }

        grabInteractable.movementType = UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable.MovementType.Kinematic;
        grabInteractable.throwOnDetach = false;

        grabInteractable.selectEntered.RemoveListener(HandleGrab);
        grabInteractable.selectEntered.AddListener(HandleGrab);
    }

    void Start()
    {
        GameObject playerObject = GameObject.FindWithTag("Player");

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

        Vector3 spawnPosition = playerPosition.position + new Vector3(0, 1.36144f, 12.5f);
        Quaternion originalPrefabRotation = (primitiveToSpawn != null) ? primitiveToSpawn.transform.rotation : Quaternion.identity;
        Quaternion finalRotation = CalculateYAxisFacingPlayerRotation(originalPrefabRotation, spawnPosition, playerPosition);

        Instantiate(primitiveToSpawn, spawnPosition, finalRotation);
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

    public static Quaternion CalculateYAxisFacingPlayerRotation(Quaternion originalPrefabRotation, Vector3 spawnPosition, Transform playerTransform)
    {
        Quaternion finalRotation = originalPrefabRotation; // Default to original rotation


        if (playerTransform == null)
        {
            Debug.LogWarning("Player Transform is null in CalculateYAxisFacingPlayerRotation. Returning original rotation.");
            return finalRotation;
        }

        Vector3 directionToPlayer = playerTransform.position - spawnPosition;
        Vector3 directionOnXZPlane = directionToPlayer;
        directionOnXZPlane.y = 0;

        if (directionOnXZPlane != Vector3.zero)
        {
            Quaternion targetYRotation = Quaternion.LookRotation(directionOnXZPlane, Vector3.up);
            Vector3 originalEuler = originalPrefabRotation.eulerAngles;
            Vector3 targetYEuler = targetYRotation.eulerAngles;
            finalRotation = Quaternion.Euler(originalEuler.x, targetYEuler.y, originalEuler.z);
        }

        return finalRotation;
    }
}
