using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Projector : MonoBehaviour
{
    public Transform crankHandle;               // The visual crank that rotates
    public GameObject handlePivot;              // The object acting as the center of rotation
    public VideoPlayer videoPlayer;
    public float rotationToPlay = 360f;

    private float totalCrankRotation = 0f;
    private bool isCranking = false;
    private Vector3 pivotScreenPos;
    private Vector2 lastMouseDir;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform == crankHandle)
            {
                isCranking = true;

                // Get screen-space position of the pivot
                pivotScreenPos = Camera.main.WorldToScreenPoint(handlePivot.transform.position);
                lastMouseDir = (Input.mousePosition - pivotScreenPos).normalized;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isCranking = false;
        }

        if (isCranking)
        {
            Vector2 currentMouseDir = (Input.mousePosition - pivotScreenPos).normalized;

            // Signed angle between last and current direction
            float angle = Vector2.SignedAngle(lastMouseDir, currentMouseDir);

            // Rotate crank handle around the pivot's world position and forward axis
            crankHandle.RotateAround(handlePivot.transform.position, handlePivot.transform.forward, -angle);

            totalCrankRotation += Mathf.Abs(angle);
            lastMouseDir = currentMouseDir;

            if (totalCrankRotation >= rotationToPlay && !videoPlayer.isPlaying)
            {
                videoPlayer.Play();
            }
        }
    }
}
