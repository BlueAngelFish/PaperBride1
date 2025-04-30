using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzlePiece : MonoBehaviour
{
    public float snapDistance = 0.3f;       // How close pieces must be to snap
    public List<Vector3> validOffsets;      // Predefined offsets that define valid snap positions (optional)

    private bool isDragging = false;
    private Vector3 offset;

    private static List<PuzzlePiece> allPieces;

    void Start()
    {
        if (allPieces == null)
            allPieces = new List<PuzzlePiece>();

        allPieces.Add(this);
    }

    void OnDestroy()
    {
        allPieces.Remove(this);
    }

    void OnMouseDown()
    {
        offset = transform.position - GetMouseWorldPos();
        isDragging = true;
    }

    void OnMouseDrag()
    {
        if (isDragging)
        {
            transform.position = GetMouseWorldPos() + offset;
            TrySnapToNearbyPieces();    
        }
    }

    void OnMouseUp()
    {
        isDragging = false;
    }

    void TrySnapToNearbyPieces()
    {
        foreach (PuzzlePiece other in allPieces)
        {
            if (other == this) continue;

            foreach (Vector3 snapOffset in validOffsets)
            {
                Vector3 expectedPos = transform.position + snapOffset;
                float dist = Vector3.Distance(other.transform.position, expectedPos);

                if (dist < snapDistance)
                {
                    other.transform.position = expectedPos;
                    other.transform.parent = this.transform;
                }
            }
        }
    }

    Vector3 GetMouseWorldPos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, 0);
        float distance;
        if (plane.Raycast(ray, out distance))
        {
            return ray.GetPoint(distance);
        }
        return Vector3.zero;
    }
}
