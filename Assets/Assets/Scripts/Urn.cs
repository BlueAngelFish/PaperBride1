using UnityEngine;
using System.Collections;

public class Urn : MonoBehaviour
{
    public Transform snapTarget;           // Where the urn snaps to
    public string urnTag = "Grabbable";    // Tag on the urn
    public GameObject replacementObject;   // Object to appear after second click
    public GameObject ring;
    public KeyCode snapKey = KeyCode.Mouse0;

    private GameObject snappedUrn;
    private bool hasSnapped = false;
    private bool hasReplaced = false;

    public Transform replacementTarget; // Drag your target here in the Inspector


    void Update()
    {
        if (Input.GetKeyDown(snapKey))
        {
            if (!hasSnapped)
            {
                TrySnapUrn();
            }
            else if (snappedUrn != null && !hasReplaced)
            {
                TryReplaceUrn();
            }
        }
    }

    void TrySnapUrn()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 5f))
        {
            if (hit.collider.CompareTag(urnTag))
            {
                snappedUrn = hit.collider.gameObject;

                // Move it to the snap target
                snappedUrn.transform.position = snapTarget.position;

                // Keep its current rotation — no change needed

                // Optional: disable physics
                if (snappedUrn.TryGetComponent<Rigidbody>(out Rigidbody rb))
                {
                    rb.isKinematic = true;
                }

                hasSnapped = true;
            }
        }
    }

    void TryReplaceUrn()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 5f))
        {
            if (hit.collider.gameObject == snappedUrn && !hasReplaced)
            {
                StartCoroutine(LerpReplaceUrn(snappedUrn.transform, replacementTarget.position));

            }
        }
    }

    IEnumerator LerpReplaceUrn(Transform urnTransform, Vector3 targetPosition)
    {
        float duration = 0.5f;
        float elapsed = 0f;

        Vector3 startPosition = urnTransform.position;

        while (elapsed < duration)
        {
            urnTransform.position = Vector3.Lerp(startPosition, targetPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        urnTransform.position = targetPosition;

        snappedUrn.SetActive(false);

        if (replacementObject != null)
        {
            replacementObject.SetActive(true);
        }

        if (ring != null)
        {
            ring.SetActive(true);
        }

        hasReplaced = true;
    }

}
