using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grab : MonoBehaviour
{
    private Camera cam;
    private bool isDragging = false;
    private Transform grabbedObject;
    private Vector3 offset;
    private float distanceToCamera;
    public bool IsBeingHeld { get; private set; } = false;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform == transform)
                {
                    grabbedObject = hit.transform;
                    isDragging = true;

                    //distance from camera to hit point 
                    distanceToCamera = Vector3.Distance(cam.transform.position, hit.point);
                    //convert 2d mouse position to 3d and offset between 3D mouse world position and object's actual position 
                    Vector3 worldPos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distanceToCamera));
                    offset = grabbedObject.position - worldPos;
                }
            }
        }

        if (Input.GetMouseButton(0) && isDragging && grabbedObject != null)
        {
            Vector3 screenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distanceToCamera);
            Vector3 worldPos = cam.ScreenToWorldPoint(screenPoint) + offset;

            // Lock the Y position
            worldPos.y = grabbedObject.position.y;

            grabbedObject.position = worldPos;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            grabbedObject = null;
        }
    }
}
