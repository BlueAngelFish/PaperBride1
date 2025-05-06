using UnityEngine;

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
            if (hit.collider.gameObject == snappedUrn)
            {
                snappedUrn.SetActive(false); // or Destroy(snappedUrn);

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
    }
}
