using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.EventSystems;
public class WallPainter : MonoBehaviour
{
    public ARPlaneManager planeManager;
    public ARRaycastManager raycastManager;
    public GameObject paintDotPrefab;
    public float dotSpacing = 0.01f;
    public float brushSize = 0.02f;
    public Color currentColor = Color.black;
    public static List<ARRaycastHit> rayHits = new List<ARRaycastHit>();
    public Vector3 lastPaintPos;
    public bool hasLastPaintPos = false;
    public AudioClip placeSound;
    public float placeVolume = 0.8f;
    public float placeDelay = 0.1f;
    public float lastPlaceTime = 0f;

    public void Update()
    {
        #if UNITY_EDITOR
            if (Input.GetMouseButton(0))
            {
                if (Input.GetMouseButton(0))
                {
                    if (IsPointerOverUI(Input.mousePosition))
                    {
                        return;
                    }

                    TryPaint(Input.mousePosition);
                }
                else
                {
                    hasLastPaintPos = false;
                }   
                TryPaint(Input.mousePosition);
            }
            else 
            {
                hasLastPaintPos = false;
            }
        #else
            if (Input.touchCount > 0)
            {
                Touch t = Input.GetTouch(0);
                if (t.phase == TouchPhase.Began || t.phase == TouchPhase.Moved || t.phase == TouchPhase.Stationary)
                {
                    TryPaint(t.position);
                }
                else
                {
                    hasLastPaintPos = false;
                }
            }
            else
            {
                hasLastPaintPos = false;
            }
        #endif
    }
    public void SetBrushSize(float newSize)
    {
        brushSize = newSize;
    }

    public bool IsPointerOverUI(Vector2 pos)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = pos;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        return results.Count > 0;
    }

    public void TryPaint(Vector2 screenPos)
    {
        if (!raycastManager.Raycast(screenPos, rayHits, TrackableType.PlaneWithinPolygon))
        {
            return;
        }

        var hit = rayHits[0];
        var plane = planeManager.GetPlane(hit.trackableId);
        Pose pose = hit.pose;

        if (hasLastPaintPos && Vector3.Distance(lastPaintPos, pose.position) < dotSpacing)
        {
            return;
        }

        Vector3 pos = pose.position;
        GameObject dot = Instantiate(paintDotPrefab, pos, pose.rotation);
        var renderer = dot.GetComponent<Renderer>();

        if (renderer != null)
        {
            renderer.material = new Material(renderer.material);
            renderer.material.color = currentColor;
        }

        dot.transform.localScale = Vector3.one * brushSize;
        lastPaintPos = pose.position;
        hasLastPaintPos = true;
        if (Time.time - lastPlaceTime < placeDelay)
        {
            return;
        }

        lastPlaceTime = Time.time;
        AudioSource.PlayClipAtPoint(placeSound, pose.position, placeVolume);
    }
}
