using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzlePiece : MonoBehaviour
{
    private bool isDragging = false;
    private bool isMouseDown = false;
    private Vector3 offset; //offset between mouse and drag 
    private Camera cam;
    private float initialY;
    private Vector3 mouseDownPos; //first mouse click position
    public float snapDistance = 0.5f;
    public float dragThreshold = 0.1f; //prevent accidental drag || min distance mouse must move

    private static HashSet<Transform> occupiedSnapPoints = new HashSet<Transform>(); //track occupied snap points
    private Transform currentSnapPoint = null; //snap point that article is snap to 

    [System.Serializable]
    public class PuzzleConnection
    {
        public string mySnapName;
        public GameObject targetPiece; //target article it should be connected to 
        public string targetSnapName;
    }

    [Header("Correct Position Settings")]
    public List<PuzzleConnection> correctConnections; //correct connections

    private bool isCorrectlyConnected = false; //track placement state
    private static int correctArticleCount = 0; //track how many pieces are correct placed
    private static bool allArticlesConnectedLogged = false; //tracks debuglog for all article connected correctly
    private static int totalArticles = 0; //track total puzzle piece 

    private static List<PuzzlePiece> allArticles = new List<PuzzlePiece>(); //check for overlap and flip 

    void Start()
    {
        cam = Camera.main;
        initialY = transform.position.y;

        if (totalArticles == 0) //check total article piece once 
        {
            totalArticles = FindObjectsOfType<PuzzlePiece>().Length;
        }

        if (!allArticles.Contains(this))
        {
            allArticles.Add(this);
        }
    }

    void OnMouseDown()
    {
        isMouseDown = true;
        mouseDownPos = GetMouseWorldPos();
        offset = transform.position - mouseDownPos;

        if (currentSnapPoint != null) //prevent articles from snapping to same snap point
        {
            occupiedSnapPoints.Remove(currentSnapPoint);
            currentSnapPoint = null;
        }

        if (correctArticleCount == totalArticles) //check if all articles are correctly palced, if yes, flip 
        {
            FlipAllArticles();
        }
    }

    void OnMouseUp()
    {
        if (isDragging)
        {
            SnapPieces();

            bool nowCorrect = IsInCorrectPosition();

            if (nowCorrect && !isCorrectlyConnected) //check if connected correctly 
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

                if (PuzzleManager.Instance != null)
                    PuzzleManager.Instance.isTopPuzzleComplete = true;

                PuzzleManager.Instance?.CheckCompletionStatus();
            }
            else if (correctArticleCount < totalArticles)
            {
                allArticlesConnectedLogged = false; //reset if any is incorrect
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

            if (!isDragging && dragDistance > this.dragThreshold)
            {
                isDragging = true;
            }

            if (isDragging) //move article 
            {
                Vector3 newPos = currentMousePos + offset;
                newPos.y = initialY;
                transform.position = newPos;
            }
        }
    }

    Vector3 GetMouseWorldPos() //convert to world position 
    {
        Vector3 mouseScreen = Input.mousePosition;
        mouseScreen.z = cam.WorldToScreenPoint(transform.position).z;
        return cam.ScreenToWorldPoint(mouseScreen);
    }

    void SnapPieces()
    {
        PuzzlePiece[] allPieces = FindObjectsOfType<PuzzlePiece>();
        Transform closestSnapPoint = null;
        float closestDist = float.MaxValue;

        foreach (PuzzlePiece other in allPieces)
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
                        // Check if the target position overlaps with another piece
                        if (!IsPositionOverlapping(theirSnap.position))
                        {
                            closestDist = dist;
                            closestSnapPoint = theirSnap;
                        }
                    }
                }
            }
        }

        if (closestSnapPoint != null) //snap to closet point
        {
            Vector3 targetPosition = closestSnapPoint.position;
            targetPosition.y = initialY;

            transform.position = targetPosition;
            currentSnapPoint = closestSnapPoint;
            occupiedSnapPoints.Add(currentSnapPoint);
        }
    }

    bool IsPositionOverlapping(Vector3 position) //check if pieces are overlapping 
    {
        float overlapBuffer = 0.1f;

        foreach (PuzzlePiece other in allArticles)
        {
            if (other == this) continue;

            float dist = Vector3.Distance(other.transform.position, position);
            if (dist < overlapBuffer)
            {
                return true;
            }
        }

        return false;
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

    void FlipAllArticles()
    {
        foreach (var article in allArticles)
        {
            article.transform.Rotate(0, 0, 180);
        }
        foreach (PhotoPuzzle puzzle in FindObjectsOfType<PhotoPuzzle>())
        {
            puzzle.ResetInitialY();
        }
        Debug.Log("All articles flipped!");
    }
}
