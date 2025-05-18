using UnityEngine;

public class VinylPickup : MonoBehaviour
{
    public float dragDistance = 3f;           // Distance from camera when dragging
    public KeyCode dragKey = KeyCode.Mouse0;  // Right mouse button
    private GameObject grabbedObject = null;
    private float objectZOffset;
    public Transform snapTarget;   // The point to snap to
    public float snapRange = 0.5f; // Distance within which the object will snap
    private bool isSnapped = false;
    public GameObject ghostVinyl;        // Assign this in the Inspector
    public float ghostShowRange = 0.5f; // Show ghost when within this range





    void Update()
    {
        if (Input.GetKeyDown(dragKey))
        {
            TryGrabObject();
        }
        else if (Input.GetKeyUp(dragKey))
        {
            ReleaseObject();
        }

        if (grabbedObject)
        {
            DragObjectWithMouse();
        }
    }

    void TryGrabObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, dragDistance))
        {
            if (hit.collider.CompareTag("Grabbable"))
            {
                grabbedObject = hit.collider.gameObject;

                if (grabbedObject.TryGetComponent<Rigidbody>(out Rigidbody rb))
                {
                    rb.freezeRotation = true; // Optional: prevents it from spinning
                                              // Do NOT set isKinematic = true!
                }

                objectZOffset = Vector3.Distance(Camera.main.transform.position, grabbedObject.transform.position);
            }
        }
    }


    void DragObjectWithMouse()
    {
        if (isSnapped) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 targetPosition = ray.GetPoint(objectZOffset);

        if (grabbedObject.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.MovePosition(targetPosition);
        }

        float distanceToSnap = Vector3.Distance(grabbedObject.transform.position, snapTarget.position);

        // ?? Show ghost vinyl if within ghostShowRange
        if (distanceToSnap <= ghostShowRange)
        {
            if (!ghostVinyl.activeSelf)
                ghostVinyl.SetActive(true);
        }
        else
        {
            if (ghostVinyl.activeSelf)
                ghostVinyl.SetActive(false);
        }

        // ? Snap if within snapRange
        if (distanceToSnap <= snapRange)
        {
            grabbedObject.transform.position = snapTarget.position;
            rb.isKinematic = true;
            isSnapped = true;

            ghostVinyl.SetActive(false);
        }
    }




    void ReleaseObject()
    {
        if (grabbedObject != null)
        {
            if (grabbedObject.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                rb.freezeRotation = false;
                rb.isKinematic = false;
            }

            grabbedObject = null;
            isSnapped = false;
        }
        if (ghostVinyl.activeSelf)
        {
            ghostVinyl.SetActive(false);
        }


    }

}
