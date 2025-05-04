using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages a group of focus targets and controls the camera accordingly. Implements a singleton for global access.
/// Automatically adjusts position and zoom based on target spread, clamped to default.
/// </summary>
public class CameraManager : MonoBehaviour
{
    /// <summary>
    /// Global singleton instance.
    /// </summary>
    public static CameraManager Instance { get; private set; }

    [Tooltip("List of Transforms the camera should focus on.")]
    [SerializeField]
    private List<Transform> focusTargets = new List<Transform>();

    [Tooltip("The Camera component to control.")]
    [SerializeField]
    private Camera targetCamera;

    [Tooltip("Multiplier applied to the maximum focus spread to compute zoom-out distance.")]
    [SerializeField]
    private float zoomOutMultiplier = 0.5f;

    // Distance along the camera's forward axis to the ground plane at start (default zoom)
    private float defaultIntersectionDistance;

    private void Awake()
    {
        // Enforce singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Compute default intersection distance if camera assigned
        if (targetCamera != null)
        {
            defaultIntersectionDistance = CalculateIntersectionDistance();
        }
        else
        {
            Debug.LogWarning("CameraManager: No camera assigned. Zoom limits won't be initialized.");
        }

        // Uncomment to persist across scenes
        // DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Provides global access to the CameraManager instance.
    /// </summary>
    public static CameraManager GetInstance()
    {
        if (Instance == null)
        {
            Debug.LogError("CameraManager instance is null. Make sure one exists in the scene.");
        }
        return Instance;
    }

    /// <summary>
    /// Adds a Transform to the focus group if not already present.
    /// </summary>
    public void AddFocus(Transform target)
    {
        if (target == null)
        {
            Debug.LogWarning("Tried to add a null Transform to focus group.");
            return;
        }
        if (!focusTargets.Contains(target))
        {
            focusTargets.Add(target);
        }
    }

    /// <summary>
    /// Removes a Transform from the focus group.
    /// </summary>
    public void RemoveFocus(Transform target)
    {
        if (target == null)
        {
            Debug.LogWarning("Tried to remove a null Transform from focus group.");
            return;
        }
        focusTargets.Remove(target);
    }

    private void LateUpdate()
    {
        if (targetCamera == null)
        {
            Debug.LogWarning("No camera assigned to CameraManager.");
            return;
        }

        // Compute average XZ position of all focus targets
        Vector3 focusPoint = CalculateCenterPoint();

        // Get camera position and forward direction
        Vector3 camPos = targetCamera.transform.position;
        Vector3 camDir = targetCamera.transform.forward;

        // Compute intersection with Y=0 plane
        if (Mathf.Approximately(camDir.y, 0f))
        {
            Debug.LogWarning("Camera direction is parallel to ground; cannot compute intersection.");
            return;
        }
        float t = -camPos.y / camDir.y;
        Vector3 currentIntersection = camPos + camDir * t;

        // Horizontal translation to center focus
        Vector3 translation = focusPoint - currentIntersection;
        translation.y = 0f; // Only move in XZ plane
        targetCamera.transform.position += translation;

        // Adjust zoom based on focus spread
        float maxSpread = CalculateMaxDistance(focusPoint);
        float additionalDistance = maxSpread * zoomOutMultiplier;
        float desiredDistance = defaultIntersectionDistance + additionalDistance;
        // Clamp to not zoom closer than default
        desiredDistance = Mathf.Max(defaultIntersectionDistance, desiredDistance);

        float zoomDelta = desiredDistance - t;
        // Apply zoom: positive zoomDelta moves camera back, negative zoomDelta moves it forward
        targetCamera.transform.position -= camDir * zoomDelta;

        // Optional: Make the camera look at the focus point (if rotation should update)
        // targetCamera.transform.LookAt(new Vector3(focusPoint.x, 0f, focusPoint.z));
    }

    /// <summary>
    /// Calculates the average XZ position of all focus targets on the Y=0 plane.
    /// </summary>
    private Vector3 CalculateCenterPoint()
    {
        if (focusTargets == null || focusTargets.Count == 0)
        {
            return Vector3.zero;
        }

        Vector3 sum = Vector3.zero;
        foreach (Transform t in focusTargets)
        {
            Vector3 pos = t.position;
            sum += new Vector3(pos.x, 0f, pos.z);
        }
        return sum / focusTargets.Count;
    }

    /// <summary>
    /// Computes intersection distance along forward axis from the camera to the Y=0 plane.
    /// </summary>
    private float CalculateIntersectionDistance()
    {
        Vector3 camPos = targetCamera.transform.position;
        Vector3 camDir = targetCamera.transform.forward;
        return -camPos.y / camDir.y;
    }

    /// <summary>
    /// Calculates the maximum XZ-plane distance from the focus center to any target.
    /// </summary>
    private float CalculateMaxDistance(Vector3 centerPoint)
    {
        float maxDist = 0f;
        foreach (Transform t in focusTargets)
        {
            Vector3 posXZ = new Vector3(t.position.x, 0f, t.position.z);
            float dist = Vector3.Distance(posXZ, centerPoint);
            if (dist > maxDist) maxDist = dist;
        }
        return maxDist;
    }
}
