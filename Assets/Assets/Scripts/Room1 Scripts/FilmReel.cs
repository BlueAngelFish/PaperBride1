using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FilmReel : MonoBehaviour
{
    public Transform snapPoint;
    public float snapDistance = 0.2f;
    public float insertDistance = 0.05f; //finish "inserting"
    public float rotationSpeed = 5f;
    public float insertSpeed = 0.5f; //reel rotate when snapping 

    private bool isSnapped = false;
    private bool isInserting = false;
    private Grab grabScript;

    private Quaternion targetRotation;

    void Start()
    {
        grabScript = GetComponent<Grab>();
    }

    void Update()
    {
        if (isSnapped || snapPoint == null || grabScript == null) return;

        if (!isInserting && !grabScript.IsBeingHeld) //if it's not insert or grabbing
        {
            float distance = Vector3.Distance(transform.position, snapPoint.position); //check distance from snap point
            if (distance <= snapDistance) //if near snap point, start "inserting" (animating in)
            {
                isInserting = true;

                Vector3 snapEuler = snapPoint.rotation.eulerAngles; //rotate film reel due to the long tape
                snapEuler.y += 70f;
                targetRotation = Quaternion.Euler(snapEuler);
            }
        }

        if (isInserting) //move slowly to snap point 
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
}
