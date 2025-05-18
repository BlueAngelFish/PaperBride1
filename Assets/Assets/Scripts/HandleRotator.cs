using UnityEngine;

public class HandleRotator : MonoBehaviour
{
    public Transform crankHandle;       // The visible handle mesh to click
    public Transform handlePivot;       // The pivot object to rotate
    public float rotationToPlay = 360f; // Degrees to trigger event
    public float sensitivity = 100f;      // Rotation sensitivity multiplier

    private bool isCranking = false;
    private Vector3 pivotScreenPos;
    private Vector2 lastMouseDir;
    private float initialRotationX;
    private bool triggered = false;

    void Start()
    {
        if (handlePivot != null)
        {
            initialRotationX = handlePivot.localEulerAngles.x;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform == crankHandle)
            {
                isCranking = true;
                pivotScreenPos = Camera.main.WorldToScreenPoint(handlePivot.position);
                lastMouseDir = ((Vector2)Input.mousePosition - (Vector2)pivotScreenPos).normalized;

                // Reset starting rotation and triggered flag when starting to crank
                initialRotationX = handlePivot.localEulerAngles.x;
                triggered = false;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isCranking = false;
        }

        if (isCranking)
        {
            Vector2 currentMouseDir = ((Vector2)Input.mousePosition - (Vector2)pivotScreenPos).normalized;
            float angle = Vector2.SignedAngle(lastMouseDir, currentMouseDir);
            angle *= sensitivity;

            // Rotate pivot on local X axis (change Vector3.right if needed)
            handlePivot.Rotate(Vector3.right, -angle, Space.Self);

            lastMouseDir = currentMouseDir;

            // Calculate how much pivot rotated from initial rotation (handles wraparound)
            float currentRotationX = handlePivot.localEulerAngles.x;
            float deltaRotation = Mathf.DeltaAngle(initialRotationX, currentRotationX);

            // Check rotation threshold, trigger once
            if (!triggered && Mathf.Abs(deltaRotation) >= rotationToPlay)
            {
                triggered = true;
                Debug.Log("Rotation threshold reached!");
                // Do your triggered action here (play sound, start animation, etc)
            }
        }
    }
}
