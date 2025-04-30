using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperPiece : MonoBehaviour
{
    public PaperPiece rightSide; //right piece 
    public Vector3 rightOffset; //offset of the right piece       

    public PaperPiece leftSide;
    public Vector3 leftOffset;

    public PaperPiece bottomSide; //bottom piece 
    public Vector3 bottomOffset;

    public PaperPiece topSide;
    public Vector3 topOffset;

    public bool isDragging = false;
    private Vector3 offset;

    private Vector3 lastPosition;

    void OnMouseDown()
    {
        offset = transform.position - GetMouseWorldPos(); //offset between mouse and obejct 
        isDragging = true;
    }

    void OnMouseDrag()
    {
        if (isDragging) //paper will follow mouse position 
        {
            transform.position = GetMouseWorldPos() + offset;
        }
    }

    void OnMouseUp()
    {
        isDragging = false; //when mouse release, stop dragging 
    }

    void Update()
    {
        if (!isDragging) return;

        SnapToSide(rightSide, rightOffset);
        SnapToSide(leftSide, leftOffset);
        SnapToSide(bottomSide, bottomOffset);
        SnapToSide(topSide, topOffset);
    }

    void SnapToSide(PaperPiece neighbor, Vector3 offsetToNeighbor)
    {
        if (neighbor == null || neighbor.isDragging) return;

        Vector3 expectedPos = transform.position + offsetToNeighbor;
        float dist = Vector3.Distance(neighbor.transform.position, expectedPos);

        if (dist < 0.3f) // Snap threshold
        {
            neighbor.transform.position = expectedPos;
            neighbor.transform.parent = this.transform;

            //Debug.Log($"{name} snapped {neighbor.name} at distance {dist}");
            //neighbor.enabled = false;
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
