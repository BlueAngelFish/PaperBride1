using UnityEngine;
using System.Collections;

public class Teapot : MonoBehaviour
{
    public float dragDistance = 3f;           // Distance from camera when dragging
    public KeyCode dragKey = KeyCode.Mouse0;  // Left mouse button
    private GameObject grabbedObject = null;
    private float objectZOffset;
    public float pourRange = 1.0f; // Distance to detect nearby teacups
    private bool isTilting = false;
    public GameObject flame;
    public GameObject liquid1;
    public GameObject liquid2;

    IEnumerator TiltTeapotTemporarily(Transform teapotTransform)
    {
        if (isTilting) yield break; // Prevent overlapping tilts
        isTilting = true;

        Quaternion originalRotation = teapotTransform.rotation;
        Quaternion tiltRotation = originalRotation * Quaternion.Euler(-15f, 0f, 0f); // Tilt 30 degrees on X-axis

        float t = 0f;
        float duration =0.5f;

        while (t < duration)
        {
            t += Time.deltaTime;
            teapotTransform.rotation = Quaternion.Slerp(originalRotation, tiltRotation, t / duration);
            yield return null;
        }

        // Wait a moment then reset
        yield return new WaitForSeconds(0.2f);

        // Reset the rotation
        t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            teapotTransform.rotation = Quaternion.Slerp(tiltRotation, originalRotation, t / duration);
            yield return null;
        }

        isTilting = false;
    }


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
            CheckForTeacups(); // Check for nearby cups while dragging
        }
    }

    void CheckForTeacups()
    {
        Collider[] hitColliders = Physics.OverlapSphere(grabbedObject.transform.position, pourRange);
        foreach (Collider col in hitColliders)
        {
            if (col.CompareTag("Teacup"))
            {
                Transform liquid = col.transform.Find("Liquid");
                if (liquid != null && !liquid.gameObject.activeSelf)
                {
                    liquid.gameObject.SetActive(true);
                    StartCoroutine(TiltTeapotTemporarily(grabbedObject.transform));
                }
            }
        }

        // Check if both liquids are active to light the flame
        if (liquid1.activeSelf && liquid2.activeSelf && !flame.activeSelf)
        {
            flame.SetActive(true);
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
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 targetPosition = ray.GetPoint(objectZOffset);

        // Lock the Y-axis to the object's current Y position
        Vector3 lockedPosition = new Vector3(targetPosition.x, grabbedObject.transform.position.y, targetPosition.z);
        grabbedObject.transform.position = lockedPosition;
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
        }
    }


}

