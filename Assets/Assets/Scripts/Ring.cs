using UnityEngine;

public class Ring : MonoBehaviour
{
    public Transform snapTarget;           // The position to snap the urn to
    public string urnTag = "Grabbable";    // Tag on the urn
    public KeyCode snapKey = KeyCode.Mouse0;  // Left mouse button

    private bool hasSnapped = false;

    void Update()
    {
        if (Input.GetKeyDown(snapKey) && !hasSnapped)
        {
            TrySnapRing();
        }
    }

    void TrySnapRing()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 5f))
        {
            if (hit.collider.CompareTag(urnTag))
            {
                GameObject urn = hit.collider.gameObject;

                // Snap position
                urn.transform.position = snapTarget.position;

                // Snap + rotate 90 degrees on X axis
                Quaternion originalRotation = snapTarget.rotation;
                Quaternion rotated = originalRotation * Quaternion.Euler(90f, 90f, 0f);
                urn.transform.rotation = rotated;

                // Optional: disable physics
                if (urn.TryGetComponent<Rigidbody>(out Rigidbody rb))
                {
                    rb.isKinematic = true;
                }

                hasSnapped = true;
            }
        }
    }
}
