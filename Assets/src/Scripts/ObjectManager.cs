using UnityEngine;
using System.Collections.Generic;
using System.Linq; // Optional: If you need Linq for querying the list later
//this script manages objects in the scene and keeps track of their tags
public class ObjectManager : MonoBehaviour
{
    // --- Singleton Pattern Start ---
    public static ObjectManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Duplicate ObjectManager found. Destroying self.");
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            // Optional: DontDestroyOnLoad(gameObject);
        }
    }
    // --- Singleton Pattern End ---

    // List to track all active ObjectInfo components
    private List<ObjectInfo> activeObjects = new List<ObjectInfo>();

    // Public read-only access to the list if needed by other systems
    public IReadOnlyList<ObjectInfo> ActiveObjects => activeObjects.AsReadOnly();

    public void RegisterObject(ObjectInfo objInfo)
    {
        if (objInfo == null) return;

        if (!activeObjects.Contains(objInfo))
        {
            activeObjects.Add(objInfo);
            Debug.Log($"Registered: {objInfo.gameObject.name} ({objInfo.objectType})");

            // Optional: Verify components exist upon registration
            if (objInfo.GetComponent<Collider>() == null)
            {
                Debug.LogWarning($"{objInfo.gameObject.name} was registered but is missing a Collider.");
            }
            if (objInfo.GetComponent<Rigidbody>() == null)
            {
                Debug.LogWarning($"{objInfo.gameObject.name} was registered but is missing a Rigidbody.");
            }
        }
    }

    public void DeregisterObject(ObjectInfo objInfo)
    {
        if (objInfo != null && activeObjects.Contains(objInfo))
        {
            activeObjects.Remove(objInfo);
            // Debug.Log($"Deregistered: {objInfo.gameObject.name}"); // Can be noisy
        }
    }

    // Example utility function you might add later
    // public List<ObjectInfo> FindObjectsByType(ObjectType type)
    // {
    //     return activeObjects.Where(obj => obj.objectType == type).ToList();
    // }

    // Removed empty Start/Update, add back if needed for other logic
}
