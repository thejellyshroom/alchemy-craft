using UnityEngine;
using UnityEngine.AI;

public class MRNavMeshBaker : MonoBehaviour
{
    public NavMeshSurface navMeshSurface;

    void Start()
    {
        Invoke("BakeNavMesh", 3f);
    }

    void BakeNavMesh()
    {
        navMeshSurface.BuildNavMesh();
    }
}
