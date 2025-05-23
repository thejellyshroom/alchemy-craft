using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class ObjectInfo : MonoBehaviour
{
    public ObjectType objectType;
    private bool hasCombined = false;
    private Transform playerTransform;

    void Start()
    {
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player object with tag 'Player' not found! Combination rotation may be incorrect.", this.gameObject);
        }

        if (ObjectManager.Instance != null)
        {
            ObjectManager.Instance.RegisterObject(this);
        }
        else
        {
            Debug.LogError("ObjectManager instance not found! Make sure an object with ObjectManager script exists in the scene.");
        }

        if (GetComponent<Rigidbody>() == null)
        {
            gameObject.AddComponent<Rigidbody>();
        }
        StartCoroutine(ResetCombineFlag());
    }

    void OnDestroy()
    {
        if (ObjectManager.Instance != null)
        {
            ObjectManager.Instance.DeregisterObject(this);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (hasCombined) return;
        ObjectInfo otherInfo = collision.gameObject.GetComponent<ObjectInfo>();

        if (otherInfo != null && !otherInfo.hasCombined)
        {
            // valid combination rule?
            CombinationRule matchedRule = CombinationManager.Instance.CheckCombination(this.objectType, otherInfo.objectType);

            if (matchedRule != null && matchedRule.outputPrefab != null)
            {
                this.hasCombined = true;
                otherInfo.hasCombined = true;

                GameObject resultPrefab = matchedRule.outputPrefab;
                Vector3 spawnPosition = collision.contacts[0].point + new Vector3(0, 0.5f, 0);

                Quaternion finalRotation = Quaternion.identity;
                Quaternion originalPrefabRotation = resultPrefab.transform.rotation;
                finalRotation = PrimitiveSpawner.CalculateYAxisFacingPlayerRotation(originalPrefabRotation, spawnPosition, playerTransform);

                bool isNewDiscovery = CombinationManager.Instance.RegisterDiscovery(matchedRule);

                GameObject resultObject = Instantiate(resultPrefab, spawnPosition, finalRotation);

                Destroy(this.gameObject);
                Destroy(otherInfo.gameObject);
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        hasCombined = false;
    }

    System.Collections.IEnumerator ResetCombineFlag()
    {
        yield return new WaitForEndOfFrame();
        hasCombined = false;
    }
}

// Trashcan by Poly by Google [CC-BY] (https://creativecommons.org/licenses/by/3.0/) via Poly Pizza (https://poly.pizza/m/fw6F3liNvHQ)