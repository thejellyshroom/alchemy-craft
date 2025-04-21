using UnityEngine;
using System.Collections.Generic; // Required for using Dictionaries or Lists if needed later
using System.Linq; // Required for Linq queries if we want more complex rule lookups later

public class CombinationManager : MonoBehaviour
{
    public static CombinationManager Instance { get; private set; }

    void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Duplicate CombinationManager found. Destroying self.");
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject);
        }
    }

    public List<CombinationRule> combinationRules = new List<CombinationRule>();

    // check for combinations and return the prefab
    public GameObject CheckCombination(ObjectType type1, ObjectType type2)
    {
        Debug.Log($"Checking combination for: {type1} and {type2}");

        foreach (CombinationRule rule in combinationRules)
        {
            // Check if the rule matches the input types (order doesn't matter)
            if ((rule.input1 == type1 && rule.input2 == type2) ||
                (rule.input1 == type2 && rule.input2 == type1))
            {
                if (rule.outputPrefab != null)
                {
                    Debug.Log($"Combination found: {type1} + {type2} = {rule.outputType}");
                    return rule.outputPrefab;
                }
                else
                {
                    Debug.LogWarning($"Combination rule found for {type1} and {type2}, but no output prefab is assigned!");
                    return null;
                }
            }
        }

        Debug.Log("No matching combination rule found.");
        return null; // No rule found
    }
}