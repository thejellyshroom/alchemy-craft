using UnityEngine;
using UnityEngine.AI;

public class MRNavMeshBaker : MonoBehaviour
{
    public NavMeshSurface navMeshSurface;

    void Start()
    {
        // Bake the NavMesh after real-world planes are detected and instantiated
        Invoke("BakeNavMesh", 3f); // Delay to allow surface detection
    }

    void BakeNavMesh()
    {
        navMeshSurface.BuildNavMesh();
    }
}
