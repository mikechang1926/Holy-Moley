using UnityEngine;
using System.Collections;

public class CameraZoom : MonoBehaviour
{
    public static CameraZoom Instance; // Singleton for easy access
    private Camera mainCamera;
    private float originalFOV;
    private Vector3 originalPosition;

    // 🔹 Mole Zoom Settings
    public float moleZoomFOVOffset = 15f; // Strong zoom for mole reveal
    public float moleZoomDepth = -2f; // Move slightly closer to mole

    // 🔹 Dialogue Zoom Settings (More Pronounced)
    public float dialogueZoomFOVOffset = 14f; // 🔥 Increased zoom for dialogue
    public float dialogueShiftX = 4f; // 🔥 More shift to center focus

    // 🔹 NPC Selection Zoom Settings
    public float npcZoomFOVOffset = 10f; // Dramatic zoom-in when selecting NPC
    public float npcZoomDepth = -3f; // Move closer for a stronger effect

    public float zoomSpeed = 1.5f; // Smooth transition speed

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        mainCamera = Camera.main;
        originalFOV = mainCamera.fieldOfView;
        originalPosition = mainCamera.transform.position;
    }

    // 🔥 Zoom in dramatically when selecting an NPC
    public void ZoomToNPC(Transform npcTransform)
    {
        if (mainCamera == null)
        {
            Debug.LogError("❌ CameraZoom: No Camera found!");
            return;
        }

        float targetFOV = Mathf.Clamp(originalFOV - npcZoomFOVOffset, 40f, originalFOV);
        Vector3 targetPosition = new Vector3(
            npcTransform.position.x, // Align X-coordinate with NPC
            mainCamera.transform.position.y, // Keep camera height
            mainCamera.transform.position.z + npcZoomDepth // 🔥 Move dramatically closer
        );

        Debug.Log($"🔍 Zooming in on NPC: Target FOV = {targetFOV}, Target X = {targetPosition.x}");
        StartCoroutine(ZoomIn(targetFOV, targetPosition));
    }

    // 🔥 Zoom in on the mole when found
    public void ZoomToMole(Transform moleTransform)
    {
        if (mainCamera == null)
        {
            Debug.LogError("❌ CameraZoom: No Camera found!");
            return;
        }

        float targetFOV = Mathf.Clamp(originalFOV - moleZoomFOVOffset, 40f, originalFOV);
        Vector3 targetPosition = new Vector3(
            moleTransform.position.x, // Align X-coordinate with mole
            mainCamera.transform.position.y, // Keep camera height
            mainCamera.transform.position.z + moleZoomDepth // Move slightly closer
        );

        Debug.Log($"🔍 Zooming in on mole: Target FOV = {targetFOV}, Target X = {targetPosition.x}");
        StartCoroutine(ZoomIn(targetFOV, targetPosition));
    }

    // 🔥 More pronounced zoom & shift for dialogue
    public void ZoomForDialogue(Transform npcTransform)
    {
        if (mainCamera == null)
        {
            Debug.LogError("❌ CameraZoom: No Camera found!");
            return;
        }

        float targetFOV = Mathf.Clamp(originalFOV - dialogueZoomFOVOffset, 45f, originalFOV);
        Vector3 targetPosition = new Vector3(
            npcTransform.position.x + dialogueShiftX, // 🔥 More shift in X for a stronger effect
            mainCamera.transform.position.y, // Keep camera height
            mainCamera.transform.position.z // Maintain depth
        );

        Debug.Log($"🔍 Zooming for dialogue: Target FOV = {targetFOV}, Target X = {targetPosition.x}");
        StartCoroutine(ZoomIn(targetFOV, targetPosition));
    }

    // 🔄 Reset camera when dialogue closes
    public void ResetCamera()
    {
        if (mainCamera == null) return;
        Debug.Log("🔄 Resetting camera to original position & zoom.");
        StartCoroutine(ZoomIn(originalFOV, originalPosition)); // Reset to original state
    }

    private IEnumerator ZoomIn(float targetFOV, Vector3 targetPosition)
    {
        float time = 0;
        float startFOV = mainCamera.fieldOfView;
        Vector3 startPosition = mainCamera.transform.position;

        while (time < 1)
        {
            time += Time.deltaTime * zoomSpeed;
            mainCamera.fieldOfView = Mathf.Lerp(startFOV, targetFOV, time);
            mainCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, time);
            yield return null;
        }

        Debug.Log("✅ Camera zoom complete.");
    }
}
