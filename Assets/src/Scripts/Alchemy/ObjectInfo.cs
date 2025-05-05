using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))] // Ensure the object has a Collider
public class ObjectInfo : MonoBehaviour
{
    public ObjectType objectType;
    private bool hasCombined = false;
    private Transform playerTransform; // Added to store player transform

    void Start()
    {
        // Added: Find Player Transform
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player object with tag 'Player' not found! Combination rotation may be incorrect.", this.gameObject);
        }

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
        StartCoroutine(ResetCombineFlag());
    }

    void OnDestroy()
    {
        if (ObjectManager.Instance != null)
        {
            ObjectManager.Instance.DeregisterObject(this);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (hasCombined) return;
        ObjectInfo otherInfo = collision.gameObject.GetComponent<ObjectInfo>();

        if (otherInfo != null && !otherInfo.hasCombined)
        {
            // valid combination rule?
            GameObject resultPrefab = CombinationManager.Instance.CheckCombination(this.objectType, otherInfo.objectType);

            if (resultPrefab != null)
            {
                this.hasCombined = true;
                otherInfo.hasCombined = true;

                Vector3 spawnPosition = collision.contacts[0].point + new Vector3(0, 0.5f, 0); // Position where they touched

                Quaternion finalRotation = Quaternion.identity; // Default rotation
                if (resultPrefab != null)
                {
                    Quaternion originalPrefabRotation = resultPrefab.transform.rotation;
                    finalRotation = PrimitiveSpawner.CalculateYAxisFacingPlayerRotation(originalPrefabRotation, spawnPosition, playerTransform);
                }

                GameObject resultObject = Instantiate(resultPrefab, spawnPosition, finalRotation);

                Destroy(this.gameObject);
                Destroy(otherInfo.gameObject);
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        hasCombined = false;
    }

    System.Collections.IEnumerator ResetCombineFlag()
    {
        // Wait for the end of the frame to ensure collision processing is complete
        yield return new WaitForEndOfFrame();
        hasCombined = false;
    }
}