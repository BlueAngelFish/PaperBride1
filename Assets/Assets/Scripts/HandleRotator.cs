using UnityEngine;

public class HandleRotator : MonoBehaviour
{
    public Transform crankHandle;       // Visual handle mesh (clicked to start)
    public Transform handlePivot;       // The pivot to rotate
    public float rotationToPlay = 360f; // Degrees required to trigger event
    public float sensitivity = 20f;     // How fast the handle spins
    public AudioSource crankAudio;      // The looping crank audio

    private bool isCranking = false;
    private Vector3 pivotScreenPos;
    private Vector2 lastMouseDir;
    private float totalRotation = 0f;
    private bool triggered = false;
    private float spinCooldown = 0.2f;   // Time after last rotation before pausing audio
    private float spinTimer = 0f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform == crankHandle)
            {
                isCranking = true;
                triggered = false;
                totalRotation = 0f;

                pivotScreenPos = Camera.main.WorldToScreenPoint(handlePivot.position);
                lastMouseDir = ((Vector2)Input.mousePosition - (Vector2)pivotScreenPos).normalized;

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isCranking = false;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (isCranking)
        {
            Vector2 currentMouseDir = ((Vector2)Input.mousePosition - (Vector2)pivotScreenPos).normalized;
            float angle = Vector2.SignedAngle(lastMouseDir, currentMouseDir);
            float rotationAmount = Mathf.Abs(angle);

            if (rotationAmount > 0.1f) // Only play audio when there's actual movement
            {
                handlePivot.Rotate(Vector3.right, -angle * sensitivity, Space.Self);

                totalRotation += rotationAmount;
                lastMouseDir = currentMouseDir;

                spinTimer = spinCooldown;

                if (crankAudio != null && !crankAudio.isPlaying)
                {
                    crankAudio.Play();
                }

                if (!triggered && totalRotation >= rotationToPlay)
                {
                    triggered = true;
                    Debug.Log("Rotation threshold reached!");
                }
            }
        }

        // Countdown and pause audio when not spinning
        if (crankAudio != null && crankAudio.isPlaying)
        {
            spinTimer -= Time.deltaTime;
            if (spinTimer <= 0f)
            {
                crankAudio.Pause();
            }
        }
    }
}
