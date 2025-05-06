using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock : MonoBehaviour
{
    [Header("Clock Hands")]
    public Transform hourHand;
    public Transform minuteHand;

    [Header("Pivot Center")]
    public Transform clockPivot;

    [Header("Hidden Compartment")]
    public Animation compartmentAnimator;

    [Header("Correct Time (in Degrees)")]
    public float correctHourAngle = 240f;  // 4:00
    public float correctMinuteAngle = 0f;

    [Header("Tolerance")]
    public float angleTolerance = 2f;

    private bool isDragging;
    private Transform currentHand;
    private Vector3 lastDirection;
    private bool solved = false;

    void Update()
    {
        HandleMouseInput();
        CheckForCorrectTime();
    }

    void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform == hourHand || hit.transform == minuteHand)
                {
                    currentHand = hit.transform;
                    lastDirection = GetMouseDirectionToPivot();
                    isDragging = true;
                }
            }
        }

        if (Input.GetMouseButton(0) && isDragging && currentHand != null)
        {
            Vector3 currentDirection = GetMouseDirectionToPivot();
            float angleDelta = Vector3.SignedAngle(lastDirection, currentDirection, Vector3.forward);

            if (angleDelta > 0f) // Only allow clockwise
            {
                currentHand.RotateAround(clockPivot.position, Vector3.forward, angleDelta);
                lastDirection = currentDirection;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            currentHand = null;
        }
    }

    Vector3 GetMouseDirectionToPivot()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 worldMouse = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, clockPivot.position.z - Camera.main.transform.position.z));
        return (worldMouse - clockPivot.position).normalized;
    }

    void CheckForCorrectTime()
    {
        if (solved) return;

        float hourZ = NormalizeAngle(hourHand.eulerAngles.z);
        float minuteZ = NormalizeAngle(minuteHand.eulerAngles.z);

        bool hourCorrect = Mathf.Abs(hourZ - correctHourAngle) <= angleTolerance;
        bool minuteCorrect = Mathf.Abs(minuteZ - correctMinuteAngle) <= angleTolerance;

        if (hourCorrect && minuteCorrect)
        {
            solved = true;
            compartmentAnimator.Play();
            Debug.Log("Puzzle solved! Compartment opening.");
        }
    }

    float NormalizeAngle(float angle)
    {
        angle %= 360;
        if (angle < 0) angle += 360;
        return angle;
    }
}
