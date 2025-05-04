using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance;

    public bool isTopPuzzleComplete = false;
    public bool isBottomPuzzleComplete = false;

    public Transform fullPuzzle; //parent of both puzzles

    private bool canDragFullPuzzle = false;
    private bool isDragging = false;
    private Vector3 offset;
    private Camera cam;

    public GameObject candle; 
    private void Awake()
    {
        Instance = this;
        cam = Camera.main;
    }

    private void Update()
    {
        if (canDragFullPuzzle && isDragging)
        {
            Vector3 mouseWorld = GetMouseWorldPos();
            fullPuzzle.position = mouseWorld + offset;
        }
    }

    public void CheckCompletionStatus()
    {
        if (isTopPuzzleComplete && isBottomPuzzleComplete && !canDragFullPuzzle)
        {
            Debug.Log("Both puzzles completed! You can now drag them together.");
            canDragFullPuzzle = true;
            FitCollider();

            Rigidbody rb = fullPuzzle.gameObject.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = fullPuzzle.gameObject.AddComponent<Rigidbody>();
                rb.useGravity = false;
                rb.isKinematic = true; 
            }
        }
    }

    public void StartDrag()
    {
        if (!canDragFullPuzzle) return;
        isDragging = true;
        offset = fullPuzzle.position - GetMouseWorldPos(); //distance between mouse and puzzle position  
    }

    public void StopDrag()
    {
        isDragging = false;
    }

    Vector3 GetMouseWorldPos()
    {
        Vector3 mouseScreen = Input.mousePosition;
        mouseScreen.z = cam.WorldToScreenPoint(fullPuzzle.position).z;
        return cam.ScreenToWorldPoint(mouseScreen);
    }
    void OnMouseDown()
    {
        StartDrag();
    }

    void OnMouseUp()
    {
        StopDrag();
    }
    public void FitCollider() //fit a box collider around the child object (both puzzle and photo puzzle) of full puzzle 
    {
        var renderers = GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return;

        Bounds bounds = renderers[0].bounds; 
        foreach (Renderer r in renderers) 
        {
            bounds.Encapsulate(r.bounds);
        }

        BoxCollider collider = GetComponent<BoxCollider>();
        if (collider == null) collider = gameObject.AddComponent<BoxCollider>();

        Vector3 center = bounds.center - transform.position; //converts world space to local space to align with collider 
        collider.center = center; 
        collider.size = bounds.size;

        collider.isTrigger = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == candle)
        {
            if (isTopPuzzleComplete && isBottomPuzzleComplete)
            {
                fullPuzzle.gameObject.SetActive(false);
                Debug.Log("Puzzle completed and collided with candle. Hiding puzzle.");
            }
        }
    }
}
