using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
//this script manages objects in the scene and keeps track of their tags
public class ObjectManager : MonoBehaviour
{
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
            // DontDestroyOnLoad(gameObject);
        }
    }

    // List to track all active ObjectInfo components
    private List<ObjectInfo> activeObjects = new List<ObjectInfo>();

    // if needed by other systems
    public IReadOnlyList<ObjectInfo> ActiveObjects => activeObjects.AsReadOnly();

    public void RegisterObject(ObjectInfo objInfo)
    {
        if (objInfo == null) return;

        if (!activeObjects.Contains(objInfo))
        {
            activeObjects.Add(objInfo);
            Debug.Log($"Registered: {objInfo.gameObject.name} ({objInfo.objectType})");

            if (objInfo.GetComponent<Rigidbody>() == null)
            {
                Debug.LogWarning($"{objInfo.gameObject.name} was registered but is missing a Rigidbody.");
                objInfo.gameObject.AddComponent<Rigidbody>();
            }
            if (objInfo.GetComponent<XRGrabInteractable>() == null)
            {
                Debug.LogWarning($"{objInfo.gameObject.name} was registered but is missing a XRGrabInteractable.");
                objInfo.gameObject.AddComponent<XRGrabInteractable>();
            }
        }
    }

    public void DeregisterObject(ObjectInfo objInfo)
    {
        if (objInfo != null && activeObjects.Contains(objInfo))
        {
            activeObjects.Remove(objInfo);
        }
    }

    //  might need later
    // public List<ObjectInfo> FindObjectsByType(ObjectType type)
    // {
    //     return activeObjects.Where(obj => obj.objectType == type).ToList();
    // }

}
