using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.EventSystems;

public class CubePainter : MonoBehaviour
{
    public ARRaycastManager raycastManager;
    public ARPlaneManager planeManager;
    public GameObject cubePrefab;
    public float gridSize = 0.1f;
    public Color cubeColor = Color.white;
    private static readonly List<ARRaycastHit> rayHits = new();
    private Vector3 lastSnappedPos;
    private bool hasLastSnapped = false;
    public AudioClip placeSound;
    public float placeVolume = 0.8f;
    public float placeDelay = 0.1f;
    private float lastPlaceTime = 0f;

    private void Update()
    {
    #if UNITY_EDITOR
        if (Input.GetMouseButton(0))
        {
            if (!IsPointerOverUI(Input.mousePosition))
                TryPaint(Input.mousePosition);
        }
        else hasLastSnapped = false;
    #else
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);

            if (IsPointerOverUI(t.position))
                return;

            if (t.phase == TouchPhase.Began ||
                t.phase == TouchPhase.Moved ||
                t.phase == TouchPhase.Stationary)
            {
                TryPaint(t.position);
            }
        }
        else hasLastSnapped = false;
    #endif
    }

    private bool IsPointerOverUI(Vector2 pos)
    {
        PointerEventData eventData = new(EventSystem.current);
        eventData.position = pos;

        List<RaycastResult> results = new();
        EventSystem.current.RaycastAll(eventData, results);

        return results.Count > 0;
    }

    private void TryPaint(Vector2 screenPos)
    {

        if (!raycastManager.Raycast(screenPos, rayHits, TrackableType.PlaneWithinPolygon)) 
        {
            return;
        }

        var hit = rayHits[0];
        var plane = planeManager.GetPlane(hit.trackableId);

        if (plane == null)
            return;

        Pose pose = hit.pose;
        Vector3 pos = pose.position;

        Vector3 snapped = new(
            Mathf.Round(pos.x / gridSize) * gridSize,
            Mathf.Round(pos.y / gridSize) * gridSize,
            Mathf.Round(pos.z / gridSize) * gridSize
        );


        if (hasLastSnapped && snapped == lastSnappedPos)
            return;

        GameObject cube = Instantiate(cubePrefab, snapped, pose.rotation);
        Renderer r = cube.GetComponent<Renderer>();
        if (r != null)
        {
            r.material = new Material(r.material);
            r.material.color = cubeColor;
        }

        lastSnappedPos = snapped;
        hasLastSnapped = true;
        
        if (Time.time - lastPlaceTime < placeDelay)
        {
            return;
        }

        lastPlaceTime = Time.time;
        AudioSource.PlayClipAtPoint(placeSound, pos, placeVolume);
    
    }
}
