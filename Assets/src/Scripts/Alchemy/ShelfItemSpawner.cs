using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

//Sama test code:
public class ShelfItemSpawner : MonoBehaviour
{
    private GameObject prefabToSpawn;

    public void Initialize(GameObject prefab)
    {
        prefabToSpawn = prefab;
    }

    public void OnSelected(SelectEnterEventArgs args)
    {
        if (prefabToSpawn == null) return;

        // Find main camera for player position
        Transform cam = Camera.main.transform;
        Vector3 spawnPos = cam.position + cam.forward * 1.5f + Vector3.down * 0.3f;
        Quaternion rotation = Quaternion.identity;

        GameObject clone = Instantiate(prefabToSpawn, spawnPos, rotation);
    }
}

// parked code:

// public class ShelfItemSpawner : MonoBehaviour
// {
//     private GameObject prefabToSpawn;

//     public void Initialize(GameObject prefab)
//     {
//         prefabToSpawn = prefab;
//     }

//     public void OnSelected(SelectEnterEventArgs args)
//     {
//         if (prefabToSpawn == null) return;

//         // Find player head position to spawn in front of
//         Transform playerHead = Camera.main.transform;
//         Vector3 spawnPos = playerHead.position + playerHead.forward * 1.5f + Vector3.down * 0.3f;
//         Quaternion spawnRot = prefabToSpawn.transform.rotation;

//         GameObject clone = Instantiate(prefabToSpawn, spawnPos, spawnRot);
//     }
// }
