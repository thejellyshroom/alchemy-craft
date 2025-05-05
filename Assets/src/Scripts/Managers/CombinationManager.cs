using UnityEngine;
using System.Collections.Generic; // Required for using Dictionaries or Lists if needed later
using System.Linq; // Required for Linq queries if we want more complex rule lookups later

public class CombinationManager : MonoBehaviour
{
    public static CombinationManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Duplicate CombinationManager found. Destroying self.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        // DontDestroyOnLoad(gameObject); // Optional: Uncomment if you want this manager to persist across scenes

        LoadCombinationRules();
    }

    void LoadCombinationRules()
    {
        combinationRules.Clear(); // Clear any rules potentially assigned in the inspector
        CombinationRule[] loadedRules = Resources.LoadAll<CombinationRule>("AlchemyRules");

        if (loadedRules.Length == 0)
        {
            Debug.LogWarning("No CombinationRule assets found in Resources/src/AlchemyRules. Make sure the path is correct and the assets exist.");
        }
        else
        {
            combinationRules.AddRange(loadedRules);
        }
    }

    public List<CombinationRule> combinationRules = new List<CombinationRule>();

    // check for combinations and return the prefab
    public GameObject CheckCombination(ObjectType type1, ObjectType type2)
    {

        foreach (CombinationRule rule in combinationRules)
        {
            // Check if the rule matches the input types (order doesn't matter)
            if ((rule.input1 == type1 && rule.input2 == type2) ||
                (rule.input1 == type2 && rule.input2 == type1))
            {
                if (rule.outputPrefab != null)
                {
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