using UnityEngine;

[CreateAssetMenu(fileName = "NewCombinationRule", menuName = "Alchemy/Combination Rule")]
public class CombinationRule : ScriptableObject
{
    public ObjectType input1;
    public ObjectType input2;

    [Header("Output")]
    public ObjectType outputType; // Optional: Useful for identifying the result type
    public GameObject outputPrefab; // The prefab to instantiate upon successful combination
}