using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))] // Ensure the object has a Collider
public class ObjectInfo : MonoBehaviour
{
    public ObjectType objectType;
    private bool hasCombined = false;

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

                Vector3 spawnPosition = collision.contacts[0].point; // Position where they touched

                // Instantiate obj
                GameObject resultObject = Instantiate(resultPrefab, spawnPosition, Quaternion.identity);
                Debug.Log($"Instantiated {resultObject.name} at {spawnPosition}");

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