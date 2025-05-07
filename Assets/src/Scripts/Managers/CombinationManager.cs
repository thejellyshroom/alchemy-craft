using UnityEngine;
using System.Collections.Generic; // Required for using Dictionaries or Lists if needed later
using System.Linq; // Required for Linq queries if we want more complex rule lookups later
using System; // Needed for Action event







[RequireComponent(typeof(AudioSource))] // Ensure AudioSource exists for PlayClipAtPoint fallback/alternative
public class CombinationManager : MonoBehaviour
{
    public static CombinationManager Instance { get; private set; }

    private HashSet<ObjectType> discoveredCombinations = new HashSet<ObjectType>();
    private int totalUniqueCombinations = 0;
    public static event Action<ObjectType, ObjectType, ObjectType> OnNewDiscoveryMade;
    public static event Action OnGameCompleted; // Event triggered when all combinations are found

    [Header("Audio")]
    [SerializeField] private AudioClip newDiscoverySound;
    [SerializeField] private AudioClip completionSound;


    // Sama's shelf implementation
    [SerializeField] private Transform discoveryShelfParent; // assigned DiscoveryShelf in inspector
    private List<Transform> shelfSlots = new List<Transform>(); // filled in Start()

    void Start() {
        shelfSlots.Clear(); // Just in case
        // Fill shelfSlots from children of shelf parent
        foreach (Transform child in discoveryShelfParent) {
            shelfSlots.Add(child);
        }
    }

    ////////




    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Duplicate CombinationManager found. Destroying self.");
            Destroy(gameObject);
            return;
        }

        Instance = this;

        LoadCombinationRules();
        CalculateTotalUniqueCombinations();
    }

    void LoadCombinationRules()
    {
        combinationRules.Clear();
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

    void CalculateTotalUniqueCombinations()
    {
        HashSet<ObjectType> uniqueResults = new HashSet<ObjectType>();
        foreach (var rule in combinationRules)
        {
            if (rule.outputPrefab != null)
            {
                ObjectInfo info = rule.outputPrefab.GetComponent<ObjectInfo>();
                if (info != null)
                {
                    uniqueResults.Add(info.objectType);
                }
                else
                {
                    Debug.LogWarning($"Output prefab '{rule.outputPrefab.name}' in a rule does not have an ObjectInfo component.", rule.outputPrefab);
                }
            }
        }
        totalUniqueCombinations = uniqueResults.Count;
        Debug.Log($"Calculated Total Unique Combinations: {totalUniqueCombinations}");
    }

    public List<CombinationRule> combinationRules = new List<CombinationRule>();

    public CombinationRule CheckCombination(ObjectType type1, ObjectType type2)
    {
        foreach (CombinationRule rule in combinationRules)
        {
            if ((rule.input1 == type1 && rule.input2 == type2) || (rule.input1 == type2 && rule.input2 == type1))
            {
                return rule;
            }
        }
        return null;
    }

    public bool RegisterDiscovery(CombinationRule discoveredRule)
    {
        if (discoveredRule?.outputPrefab == null)
        {
            return false;
        }

        ObjectInfo resultInfo = discoveredRule.outputPrefab.GetComponent<ObjectInfo>();
        if (resultInfo == null)
        {
            return false;
        }

        ObjectType resultType = resultInfo.objectType;
        bool added = discoveredCombinations.Add(resultType);

        if (added)
        {
            OnNewDiscoveryMade?.Invoke(discoveredRule.input1, discoveredRule.input2, resultType);

            if (newDiscoverySound != null)
            {
                AudioSource.PlayClipAtPoint(newDiscoverySound, this.transform.position);
            }

            if (discoveredCombinations.Count == totalUniqueCombinations)
            {
                Debug.Log("All combinations discovered! Playing completion sound.");
                if (completionSound != null)
                {
                    AudioSource.PlayClipAtPoint(completionSound, this.transform.position);
                }
                // Trigger the game completion event
                OnGameCompleted?.Invoke();
            }
        }

        // Sama's Shelf implementation
        if (added && discoveredCombinations.Count <= shelfSlots.Count)
        {
            // Sama's updated shelf logic - now you can combine things from the shelf'
            // Freeze physics to lock in place
            if (added && discoveredCombinations.Count <= shelfSlots.Count)
            {
            int slotIndex = discoveredCombinations.Count - 1;
            Transform slot = shelfSlots[slotIndex];

            GameObject shelfCopy = Instantiate(discoveredRule.outputPrefab, slot.position, slot.rotation);
            shelfCopy.transform.SetParent(slot);

            // Freeze it until grabbed
            Rigidbody rb = shelfCopy.GetComponent<Rigidbody>();
            if (rb == null)
                rb = shelfCopy.AddComponent<Rigidbody>();

            rb.useGravity = false;
            rb.isKinematic = true;
            rb.constraints = RigidbodyConstraints.FreezeAll;

            UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab = shelfCopy.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
            if (grab == null)
                grab = shelfCopy.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

            grab.throwOnDetach = true;
            grab.movementType = UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable.MovementType.VelocityTracking;

            // Unlock the item once it is grabbed
            grab.selectEntered.AddListener((args) =>
            {
                Rigidbody shelfRB = shelfCopy.GetComponent<Rigidbody>();
                if (shelfRB != null)
                {
                    shelfRB.useGravity = true;
                    shelfRB.isKinematic = false;
                    shelfRB.constraints = RigidbodyConstraints.None;
                }
            });
        }


            //// parked code:

            // // Disable physics so it stays on the shelf
            // Destroy(shelfCopy.GetComponent<Rigidbody>());

            // // Set up interaction for spawning
            // UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab = shelfCopy.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
            // if (grab == null) grab = shelfCopy.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

            // grab.throwOnDetach = false; // Optional: don't throw shelf items

            // ShelfItemSpawner spawner = shelfCopy.AddComponent<ShelfItemSpawner>();
            // spawner.Initialize(discoveredRule.outputPrefab);

            // grab.selectEntered.AddListener(spawner.OnSelected);
        }

        /////

        return added;
    }

    public int GetDiscoveredCount()
    {
        return discoveredCombinations.Count;
    }

    public int GetTotalCombinationsCount()
    {
        return totalUniqueCombinations;
    }
}