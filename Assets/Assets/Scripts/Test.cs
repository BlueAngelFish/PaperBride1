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
    public float dragThreshold = 0.1f;

    private static HashSet<Transform> occupiedSnapPoints = new HashSet<Transform>();
    private Transform currentSnapPoint = null;

    [System.Serializable]
    public class PuzzleConnection
    {
        public string mySnapName;
        public GameObject targetPiece;
        public string targetSnapName;
    }

    [Header("Correct Position Settings")]
    public List<PuzzleConnection> correctConnections;

    private bool isCorrectlyConnected = false;
    private static int correctArticleCount = 0;
    private static bool allArticlesConnectedLogged = false;
    private static int totalArticles = 0;

    void Start()
    {
        cam = Camera.main;
        initialY = transform.position.y;

        // Count total articles once at start
        if (totalArticles == 0)
        {
            totalArticles = FindObjectsOfType<Test>().Length;
        }
    }

    void OnMouseDown()
    {
        isMouseDown = true;
        mouseDownPos = GetMouseWorldPos();
        offset = transform.position - mouseDownPos;

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

            bool nowCorrect = IsInCorrectPosition();

            if (nowCorrect && !isCorrectlyConnected)
            {
                isCorrectlyConnected = true;
                correctArticleCount++;
            }
            else if (!nowCorrect && isCorrectlyConnected)
            {
                isCorrectlyConnected = false;
                correctArticleCount--;
            }

            if (nowCorrect)
            {
                Debug.Log(name + " is in the correct position!");
            }
            else
            {
                Debug.Log(name + " is NOT in the correct position!");
            }

            if (correctArticleCount == totalArticles && !allArticlesConnectedLogged)
            {
                Debug.Log("All articles are in the correct position!");
                allArticlesConnectedLogged = true;
            }
            else if (correctArticleCount < totalArticles)
            {
                allArticlesConnectedLogged = false; // Reset when something becomes incorrect
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
            return false;

        foreach (var connection in correctConnections)
        {
            if (connection == null || connection.targetPiece == null)
                return false;

            Transform mySnap = transform.Find(connection.mySnapName);
            Transform theirSnap = connection.targetPiece.transform.Find(connection.targetSnapName);

            if (mySnap == null || theirSnap == null)
                return false;

            float dist = Vector3.Distance(mySnap.position, theirSnap.position);
            if (dist > snapDistance)
                return false;
        }

        return true;
    }
}
