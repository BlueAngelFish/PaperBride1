using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FilmReel : MonoBehaviour
{
    public Transform snapPoint;
    public float snapDistance = 0.2f;
    public float insertDistance = 0.05f;
    public float rotationSpeed = 5f;
    public float insertSpeed = 0.5f;

    private bool isSnapped = false;
    private bool isInserting = false;
    private Grab grabScript;

    private Quaternion targetRotation;

    public GameObject filmOutline;

    void Start()
    {
        grabScript = GetComponent<Grab>();
        if (filmOutline != null)
            filmOutline.SetActive(false);
    }

    void Update()
    {
        // Disable outline if grabbed
        if (grabScript != null && grabScript.IsBeingHeld)
        {
            if (filmOutline != null && filmOutline.activeSelf)
                filmOutline.SetActive(false);
        }

        if (isSnapped || snapPoint == null || grabScript == null)
            return;

        if (!isInserting && !grabScript.IsBeingHeld)
        {
            float distance = Vector3.Distance(transform.position, snapPoint.position);
            if (distance <= snapDistance)
            {
                isInserting = true;
                Vector3 snapEuler = snapPoint.rotation.eulerAngles;
                snapEuler.y += 70f;
                targetRotation = Quaternion.Euler(snapEuler);
            }
        }

        if (isInserting)
        {
            transform.position = Vector3.MoveTowards(transform.position, snapPoint.position, Time.deltaTime * insertSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

            if (Vector3.Distance(transform.position, snapPoint.position) <= insertDistance)
            {
                transform.position = snapPoint.position;
                transform.rotation = targetRotation;
                isSnapped = true;
                isInserting = false;
            }
        }
    }

    void OnMouseEnter()
    {
        if (grabScript != null && !grabScript.IsBeingHeld && filmOutline != null)
            filmOutline.SetActive(true);
    }

    void OnMouseExit()
    {
        if (filmOutline != null)
            filmOutline.SetActive(false);
    }
}
