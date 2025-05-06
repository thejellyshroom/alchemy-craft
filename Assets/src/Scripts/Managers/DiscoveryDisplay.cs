using UnityEngine;
using TMPro; // Required for TextMeshPro
using System.Collections; // Required for Coroutine

public class DiscoveryDisplay : MonoBehaviour
{
    [Header("Persistent UI")]
    [SerializeField]
    private TextMeshPro persistentDiscoveryText;

    [Header("Popup UI")]
    [SerializeField]
    private TextMeshPro popupDiscoveryText;
    [SerializeField]
    private float popupDuration = 3.0f;
    [SerializeField]
    private string completionMessage = "All Combinations Found!";

    private int totalCombinations = 0;
    private Coroutine popupCoroutine = null;

    void OnEnable()
    {
        CombinationManager.OnNewDiscoveryMade += HandleNewDiscovery;
        CombinationManager.OnGameCompleted += HandleGameCompletion;

        if (popupDiscoveryText != null)
        {
            popupDiscoveryText.gameObject.SetActive(false);
        }

        UpdatePersistentDisplay();
    }

    void OnDisable()
    {
        CombinationManager.OnNewDiscoveryMade -= HandleNewDiscovery;
        CombinationManager.OnGameCompleted -= HandleGameCompletion;

        if (popupCoroutine != null)
        {
            StopCoroutine(popupCoroutine);
            popupCoroutine = null;
        }
    }

    void Start()
    {
        UpdatePersistentDisplay();
    }

    void HandleNewDiscovery(ObjectType input1, ObjectType input2, ObjectType result)
    {
        UpdatePersistentDisplay();
        ShowPopup($"{input1} + {input2} = {result}!");
    }

    void HandleGameCompletion()
    {
        ShowPopup(completionMessage);
    }

    void UpdatePersistentDisplay()
    {
        if (CombinationManager.Instance != null)
        {
            totalCombinations = CombinationManager.Instance.GetTotalCombinationsCount();
            int discoveredCount = CombinationManager.Instance.GetDiscoveredCount();

            if (persistentDiscoveryText != null)
            {
                persistentDiscoveryText.text = $"Combinations discovered: {discoveredCount} / {totalCombinations}";
            }
            else
            {
                Debug.LogWarning("Persistent Discovery Text is not assigned.", this.gameObject);
            }
        }
        else if (persistentDiscoveryText != null)
        {
            persistentDiscoveryText.text = "Discovered: ? / ?";
        }
    }

    void ShowPopup(string message)
    {
        if (popupDiscoveryText == null)
        {
            return;
        }

        if (popupCoroutine != null)
        {
            StopCoroutine(popupCoroutine);
        }

        popupDiscoveryText.text = message;
        popupDiscoveryText.gameObject.SetActive(true);

        popupCoroutine = StartCoroutine(HidePopupAfterDelay());
    }

    IEnumerator HidePopupAfterDelay()
    {
        yield return new WaitForSeconds(popupDuration);
        if (popupDiscoveryText != null)
        {
            popupDiscoveryText.gameObject.SetActive(false);
        }
        popupCoroutine = null;
    }
}