using UnityEngine;
using UnityEngine.Video;

public class Projector : MonoBehaviour
{
    public Transform crankHandle;               // The crank mesh that user clicks
    public Transform handlePivot;               // The pivot point to rotate around
    public VideoPlayer videoPlayer;             // Video player component
    public AudioSource videoAudio;              // Audio that plays with the video
    public float rotationToPlay = 360f;         // Total rotation needed to trigger playback
    public float sensitivity = 10f;             // Crank sensitivity
    public float spinCooldown = 1.5f;           // Time to wait before stopping video/audio after no spin

    private float totalCrankRotation = 0f;
    private bool isCranking = false;
    private Vector3 pivotScreenPos;
    private Vector2 lastMouseDir;
    private float spinTimer = 0f;

    void Update()
    {
        // Start cranking
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform == crankHandle)
            {
                isCranking = true;
                pivotScreenPos = Camera.main.WorldToScreenPoint(handlePivot.position);
                lastMouseDir = ((Vector2)Input.mousePosition - (Vector2)pivotScreenPos).normalized;

                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }

        // Stop cranking
        if (Input.GetMouseButtonUp(0))
        {
            isCranking = false;
        }

        if (isCranking)
        {
            Vector2 currentMouseDir = ((Vector2)Input.mousePosition - (Vector2)pivotScreenPos).normalized;
            float angle = Vector2.SignedAngle(lastMouseDir, currentMouseDir);
            angle *= sensitivity;

            if (Mathf.Abs(angle) > 0.1f)
            {
                crankHandle.RotateAround(handlePivot.position, handlePivot.forward, -angle);
                totalCrankRotation += Mathf.Abs(angle);
                lastMouseDir = currentMouseDir;

                spinTimer = spinCooldown;

                if (totalCrankRotation >= rotationToPlay && !videoPlayer.isPlaying)
                {
                    videoPlayer.Play();
                    if (videoAudio != null && !videoAudio.isPlaying)
                        videoAudio.Play();

                    Debug.Log("🎬 Video and audio started!");
                }
            }
        }

        // Countdown spin timer
        if (videoPlayer.isPlaying)
        {
            spinTimer -= Time.deltaTime;

            if (spinTimer <= 0f)
            {
                videoPlayer.Pause();
                if (videoAudio != null && videoAudio.isPlaying)
                    videoAudio.Pause();

                Debug.Log("⏸️ Video and audio paused due to no spin.");
            }
        }
    }
}
