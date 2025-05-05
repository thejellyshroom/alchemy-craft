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
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    // List to track all active ObjectInfo components
    private List<ObjectInfo> activeObjects = new List<ObjectInfo>();

    public IReadOnlyList<ObjectInfo> ActiveObjects => activeObjects.AsReadOnly();

    public void RegisterObject(ObjectInfo objInfo)
    {
        if (objInfo == null) return;

        if (!activeObjects.Contains(objInfo))
        {
            activeObjects.Add(objInfo);

            if (objInfo.GetComponent<Rigidbody>() == null)
            {
                objInfo.gameObject.AddComponent<Rigidbody>();
            }
            if (objInfo.GetComponent<XRGrabInteractable>() == null)
            {
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
