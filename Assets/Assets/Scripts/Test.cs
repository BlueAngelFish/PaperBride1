using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private bool isDragging = false;
    private bool isMouseDown = false;
    private Vector3 offset;
    private Camera cam;
    private float initialY;
    private Vector3 mouseDownPos;
    public float snapDistance = 0.5f;
    public float dragThreshold = 0.1f; // Threshold for detecting drag

    // Static: globally occupied snap points
    private static HashSet<Transform> occupiedSnapPoints = new HashSet<Transform>();

    // Instance: snap point currently used by THIS object
    private Transform currentSnapPoint = null;

    [System.Serializable]
    public class PuzzleConnection
    {
        public string mySnapName;              // e.g., "Snap1"
        public GameObject targetPiece;         // e.g., Article 2 GameObject
        public string targetSnapName;          // e.g., "Snap2"
    }

    [Header("Correct Position Settings")]
    public List<PuzzleConnection> correctConnections;

    void Start()
    {
        cam = Camera.main;
        initialY = transform.position.y;
    }

    void OnMouseDown()
    {
        isMouseDown = true;
        mouseDownPos = GetMouseWorldPos();
        offset = transform.position - mouseDownPos;

        // Free previously used snap point, if any
        if (currentSnapPoint != null)
        {
            occupiedSnapPoints.Remove(currentSnapPoint);
            currentSnapPoint = null;
        }
    }

    void OnMouseUp()
    {
        if (isDragging)
        {
            TrySnap();

            // Check correctness after snapping
            if (IsInCorrectPosition())
            {
                Debug.Log(name + " is in the correct position!");
            }
            else
            {
                Debug.Log(name + " is NOT in the correct position!");
            }
        }

        isDragging = false;
        isMouseDown = false;
    }

    void Update()
    {
        if (isMouseDown)
        {
            Vector3 currentMousePos = GetMouseWorldPos();
            float dragDistance = Vector3.Distance(currentMousePos, mouseDownPos);

            // Only start dragging after the mouse has moved beyond the threshold
            if (!isDragging && dragDistance > dragThreshold)
            {
                isDragging = true;
            }

            if (isDragging)
            {
                Vector3 newPos = currentMousePos + offset;
                newPos.y = initialY;
                transform.position = newPos;
            }
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
            if (other == this) continue;

            foreach (Transform mySnap in transform)
            {
                if (!mySnap.name.StartsWith("Snap")) continue;

                foreach (Transform theirSnap in other.transform)
                {
                    if (!theirSnap.name.StartsWith("Snap")) continue;

                    if (occupiedSnapPoints.Contains(theirSnap)) continue;

                    float dist = Vector3.Distance(mySnap.position, theirSnap.position);
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
            Vector3 targetPosition = closestSnapPoint.position;
            targetPosition.y = initialY;

            transform.position = targetPosition;

            currentSnapPoint = closestSnapPoint;
            occupiedSnapPoints.Add(currentSnapPoint);
        }
    }

    public bool IsInCorrectPosition()
    {
        if (correctConnections == null || correctConnections.Count == 0)
        {
            Debug.LogWarning($"{gameObject.name} has no correctConnections defined.");
            return false;
        }

        foreach (var connection in correctConnections)
        {
            if (connection == null || connection.targetPiece == null)
            {
                Debug.LogWarning($"Invalid connection in {gameObject.name}. Skipping.");
                return false;
            }

            Transform mySnap = transform.Find(connection.mySnapName);
            Transform theirSnap = connection.targetPiece.transform.Find(connection.targetSnapName);

            if (mySnap == null || theirSnap == null)
            {
                Debug.LogWarning($"Snap point not found: {connection.mySnapName} or {connection.targetSnapName} on {gameObject.name}");
                return false;
            }

            float dist = Vector3.Distance(mySnap.position, theirSnap.position);

            if (dist > snapDistance)
            {
                Debug.LogWarning($"{gameObject.name} incorrect: {connection.mySnapName} not correctly aligned with {connection.targetPiece.name}.{connection.targetSnapName}. Distance: {dist}");
                return false;
            }
        }

        Debug.Log($"{gameObject.name} is in the correct position!");
        return true;
    }

}
