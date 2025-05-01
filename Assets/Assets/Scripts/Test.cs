using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 offset;
    private Camera cam;
    private float initialY; // Store the initial Y position

    public float snapDistance = 0.5f;

    void Start()
    {
        cam = Camera.main;
        initialY = transform.position.y; // Save the Y position when the object starts
    }

    void OnMouseDown()
    {
        isDragging = true;
        offset = transform.position - GetMouseWorldPos();
    }

    void OnMouseUp()
    {
        isDragging = false;
        TrySnap();
    }

    void Update()
    {
        if (isDragging)
        {
            Vector3 mousePos = GetMouseWorldPos();
            Vector3 newPos = mousePos + offset;
            newPos.y = initialY; // Lock Y position
            transform.position = newPos;
        }
    }

    Vector3 GetMouseWorldPos()
    {
        Vector3 mouseScreen = Input.mousePosition;
        mouseScreen.z = cam.WorldToScreenPoint(transform.position).z;
        return cam.ScreenToWorldPoint(mouseScreen);
    }

    void TrySnap()
    {
        Test[] allPieces = FindObjectsOfType<Test>();
        Transform closestSnapPoint = null;
        float closestDist = float.MaxValue;

        foreach (Test other in allPieces)
        {
            // Skip self-object
            if (other == this) continue;

            foreach (Transform mySnap in transform)
            {
                if (!mySnap.name.StartsWith("Snap")) continue;

                foreach (Transform theirSnap in other.transform)
                {
                    if (!theirSnap.name.StartsWith("Snap")) continue;

                    // Calculate distance between snap points
                    float dist = Vector3.Distance(mySnap.position, theirSnap.position);

                    // If the distance is smaller than the snap threshold and it's the closest one found
                    if (dist < snapDistance && dist < closestDist)
                    {
                        closestDist = dist;
                        closestSnapPoint = theirSnap;
                    }
                }
            }
        }

        if (closestSnapPoint != null)
        {
            // Simply snap to the closest snap point
            Vector3 targetPosition = closestSnapPoint.position;
            targetPosition.y = initialY; // Ensure the Y position stays fixed

            transform.position = targetPosition;
        }
    }
}
