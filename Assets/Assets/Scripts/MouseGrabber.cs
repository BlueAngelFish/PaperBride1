using UnityEngine;

public class MouseGrabber : MonoBehaviour
{
    public float dragDistance = 3f;           // Distance from camera when dragging
    public KeyCode dragKey = KeyCode.Mouse0;  // Right mouse button
    private GameObject grabbedObject = null;
    private float objectZOffset;
    public Transform snapTarget;   // The point to snap to
    public float snapRange = 0.5f; // Distance within which the object will snap
    private bool isSnapped = false;


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
                    rb.isKinematic = true; // Optional: disable physics
                }

                objectZOffset = Vector3.Distance(Camera.main.transform.position, grabbedObject.transform.position);
            }
        }
    }

    void DragObjectWithMouse()
    {
        if (isSnapped) return; // Don’t move it if already snapped

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 targetPosition = ray.GetPoint(objectZOffset);

        // Lock the Y-axis
        Vector3 lockedPosition = new Vector3(targetPosition.x, grabbedObject.transform.position.y, targetPosition.z);
        grabbedObject.transform.position = lockedPosition;

        // Snap logic
        if (Vector3.Distance(grabbedObject.transform.position, snapTarget.position) <= snapRange)
        {
            grabbedObject.transform.position = snapTarget.position;
            isSnapped = true;
        }
    }


    void ReleaseObject()
    {
        if (grabbedObject != null)
        {
            if (grabbedObject.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                rb.isKinematic = false;
            }

            grabbedObject = null;
            isSnapped = false;
        }
    }
}
