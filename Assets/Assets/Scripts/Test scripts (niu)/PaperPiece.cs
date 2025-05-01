using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperPiece : MonoBehaviour
{
    //public PaperPiece rightSide; //right piece 
    //public Vector3 rightOffset; //offset of the right piece       

    //public PaperPiece leftSide;
    //public Vector3 leftOffset;

    //public PaperPiece bottomSide; //bottom piece 
    //public Vector3 bottomOffset;

    //public PaperPiece topSide;
    //public Vector3 topOffset;

    //public bool isDragging = false;
    //private Vector3 offset;

    //private Vector3 lastPosition;

    //private Transform parentTransform;
    //void Start()
    //{
    //    // Get the parent object containing all the pieces
    //    parentTransform = transform.parent;
    //}
    //void OnMouseDown()
    //{
    //    offset = transform.position - GetMouseWorldPos(); //offset between mouse and obejct 
    //    isDragging = true;

    //    if (parentTransform != null)
    //    {
    //        parentTransform.Rotate(0f, 0f, -180f);
    //    }
    //}

    //void OnMouseDrag()
    //{
    //    if (isDragging) //paper will follow mouse position 
    //    {
    //        transform.position = GetMouseWorldPos() + offset;
    //    }
    //}

    //void OnMouseUp()
    //{
    //    isDragging = false; //when mouse release, stop dragging 
    //}

    //void Update()
    //{
    //    if (!isDragging) return;

    //    TrySnapToNeighbor(rightSide, rightOffset);
    //    TrySnapToNeighbor(leftSide, leftOffset);
    //    TrySnapToNeighbor(bottomSide, bottomOffset);
    //    TrySnapToNeighbor(topSide, topOffset);
    //}

    //void TrySnapToNeighbor(PaperPiece neighbor, Vector3 offsetToNeighbor)
    //{
    //    if (neighbor == null || neighbor.isDragging) return;

    //    Vector3 expectedPos = transform.position + offsetToNeighbor;
    //    float dist = Vector3.Distance(neighbor.transform.position, expectedPos);

    //    if (dist < 0.3f) // Snap threshold
    //    {
    //        neighbor.transform.position = expectedPos;
    //        neighbor.transform.parent = this.transform;

    //        //Debug.Log($"{name} snapped {neighbor.name} at distance {dist}");
    //        //neighbor.enabled = false;
    //    }
    //}

    //Vector3 GetMouseWorldPos()
    //{
    //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //    Plane plane = new Plane(Vector3.up, 0); 
    //    float distance;
    //    if (plane.Raycast(ray, out distance))
    //    {
    //        return ray.GetPoint(distance);
    //    }
    //    return Vector3.zero;
    //}
    public PaperPiece rightSide; // right piece 
    public Vector3 rightOffset;  // offset of the right piece       

    public PaperPiece leftSide;
    public Vector3 leftOffset;

    public PaperPiece bottomSide; // bottom piece 
    public Vector3 bottomOffset;

    public PaperPiece topSide;
    public Vector3 topOffset;

    public bool isDragging = false;
    private Vector3 offset;

    private Transform parentTransform;

    private bool isFlipping = false;

    void Start()
    {
        // Get the parent object containing all the pieces
        parentTransform = transform.parent;
    }

    void OnMouseDown()
    {
        if (isFlipping) return; // Prevent interaction during flip

        offset = transform.position - GetMouseWorldPos(); // offset between mouse and object 
        isDragging = true;

        StartCoroutine(FlipPiece()); // Animate the flip
    }

    void OnMouseDrag()
    {
        if (isDragging && !isFlipping) // piece will follow mouse position 
        {
            transform.position = GetMouseWorldPos() + offset;
        }
    }

    void OnMouseUp()
    {
        isDragging = false; // when mouse is released, stop dragging 
    }

    void Update()
    {
        if (!isDragging) return;

        TrySnapToNeighbor(rightSide, rightOffset);
        TrySnapToNeighbor(leftSide, leftOffset);
        TrySnapToNeighbor(bottomSide, bottomOffset);
        TrySnapToNeighbor(topSide, topOffset);
    }

    void TrySnapToNeighbor(PaperPiece neighbor, Vector3 offsetToNeighbor)
    {
        if (neighbor == null || neighbor.isDragging) return;

        Vector3 expectedPos = transform.position + offsetToNeighbor;
        float dist = Vector3.Distance(neighbor.transform.position, expectedPos);

        if (dist < 0.3f) // Snap threshold
        {
            neighbor.transform.position = expectedPos;
            neighbor.transform.parent = this.transform;
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

    IEnumerator FlipPiece()
    {
        isFlipping = true;

        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = startRotation * Quaternion.Euler(0f, 0f, 180f);
        float t = 0f;
        float duration = 0.3f; // Flip duration

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, t);
            yield return null;
        }

        isFlipping = false;
    }
}
