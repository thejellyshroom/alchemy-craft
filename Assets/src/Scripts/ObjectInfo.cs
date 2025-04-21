using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))] // Ensure the object has a Collider
public class ObjectInfo : MonoBehaviour
{
    public ObjectType objectType;

    // Flag to prevent duplicate combinations from a single collision event
    private bool hasCombined = false;

    // Register with ObjectManager when enabled/created
    void Start()
    {
        // Ensure ObjectManager exists
        if (ObjectManager.Instance != null)
        {
            ObjectManager.Instance.RegisterObject(this);
        }
        else
        {
            Debug.LogError("ObjectManager instance not found! Make sure an object with ObjectManager script exists in the scene.");
        }

        if (GetComponent<Rigidbody>() == null)
        {
            gameObject.AddComponent<Rigidbody>();
        }
        // Reset combine flag slightly after physics updates
        StartCoroutine(ResetCombineFlag());
    }

    void OnDestroy()
    {
        // Check if ObjectManager still exists (important during scene closing)
        if (ObjectManager.Instance != null)
        {
            ObjectManager.Instance.DeregisterObject(this);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Prevent checking combination if this object has already combined in this collision cascade
        if (hasCombined) return;

        // Try to get ObjectInfo from the other object
        ObjectInfo otherInfo = collision.gameObject.GetComponent<ObjectInfo>();

        // Check if the other object has ObjectInfo and hasn't combined yet
        if (otherInfo != null && !otherInfo.hasCombined)
        {
            // Check for a valid combination rule
            GameObject resultPrefab = CombinationManager.Instance.CheckCombination(this.objectType, otherInfo.objectType);

            if (resultPrefab != null)
            {
                // Mark both objects as combined to prevent further checks in this frame/collision
                this.hasCombined = true;
                otherInfo.hasCombined = true;

                // Calculate the position for the new object (e.g., contact point or average)
                Vector3 spawnPosition = collision.contacts[0].point; // Position where they touched

                // Instantiate the result
                GameObject resultObject = Instantiate(resultPrefab, spawnPosition, Quaternion.identity);
                Debug.Log($"Instantiated {resultObject.name} at {spawnPosition}");

                // Destroy the original objects
                Destroy(this.gameObject);
                Destroy(otherInfo.gameObject);
            }
        }
    }

    // Reset the flag when the object is no longer colliding (optional, but good practice)
    void OnCollisionExit(Collision collision)
    {
        hasCombined = false;
    }

    // Reset the flag shortly after collision in case objects get stuck
    System.Collections.IEnumerator ResetCombineFlag()
    {
        // Wait for the end of the frame to ensure collision processing is complete
        yield return new WaitForEndOfFrame();
        hasCombined = false;
    }
}