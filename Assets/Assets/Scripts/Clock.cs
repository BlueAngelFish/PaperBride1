using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock : MonoBehaviour
{
    public Transform centerPivot;
    public Transform hourPivot;       
    public Transform minutePivot;    
    public GameObject compartment;
    public Animator compartmentAnimator; 

    private bool isDragging = false; //checks which clock hand being dragged 
    private Transform selectedPivot = null;
    private Vector3 lastDirection; //how much player rotates the clock hand (mouse cursor) around centre

    private bool hasTriggered = false;
    void Start()
    {
        compartmentAnimator = compartment.GetComponent<Animator>();
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform.CompareTag("ClockHand"))
                {
                    selectedPivot = hit.transform.parent; //get parent's (pivot) transform 
                    isDragging = true;

                    //calculate direction from centre pivot to mouse position 
                    Vector3 worldPos = GetMouseWorldPositionOnPlane(centerPivot.position, Vector3.forward);
                    lastDirection = (worldPos - centerPivot.position).normalized;
                }
            }
        }

        if (isDragging && selectedPivot != null)
        {
            Vector3 currentPos = GetMouseWorldPositionOnPlane(centerPivot.position, Vector3.forward);
            Vector3 currentDir = (currentPos - centerPivot.position).normalized;

            float angle = Vector3.SignedAngle(lastDirection, currentDir, Vector3.forward); //calculate angle and which direction to rotate 
            selectedPivot.Rotate(0, 0, angle);

            lastDirection = currentDir; //update direction 
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            selectedPivot = null;
        }

        CheckUnlockCondition();
    }

    void CheckUnlockCondition()
    {
        if (hasTriggered) return;

        float hourAngle = NormalizeAngle(hourPivot.eulerAngles.z); //normalise to 0-360 degree 
        float minuteAngle = NormalizeAngle(minutePivot.eulerAngles.z);

        // adjust based on hour and min pivot 
        if (Mathf.Abs(hourAngle - 270f) < 10f && Mathf.Abs(minuteAngle - 240f) < 10f)
        {
            hasTriggered = true;
            compartmentAnimator.SetTrigger("Pull");
            Debug.Log("Unlocked!");
        }
    }

    float NormalizeAngle(float angle) //angle stays within 0-360 degree 
    {
        angle %= 360f;
        if (angle < 0f) angle += 360f;
        return angle;
    }

    Vector3 GetMouseWorldPositionOnPlane(Vector3 planeOrigin, Vector3 planeNormal) //convert mouse position to world position  
    {
        Plane plane = new Plane(planeNormal, planeOrigin);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        plane.Raycast(ray, out float distance);
        return ray.GetPoint(distance);
    }
}
