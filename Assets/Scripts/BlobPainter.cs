using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.EventSystems;

public class BlobPainter : MonoBehaviour
{
    public ARRaycastManager raycastManager;
    public ARPlaneManager planeManager;
    public GameObject blobPrefab;
    public float finalScale = 0.2f;
    public float growSpeed = 1.5f;
    public float spacing = 0.05f;
    private static List<ARRaycastHit> rayHits = new();
    private Vector3 lastPos;
    private bool hasLast = false;
    public float minFinalScale = 0.1f;
    public float maxFinalScale = 0.35f;
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
        else hasLast = false;
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
        else hasLast = false;
    #endif
    }

    bool IsPointerOverUI(Vector2 pos)
    {
        PointerEventData d = new(EventSystem.current);
        d.position = pos;

        List<RaycastResult> r = new();
        EventSystem.current.RaycastAll(d, r);

        return r.Count > 0;
    }

    private void TryPaint(Vector2 screenPos)
    {
        Vector3 paintPos;
        Quaternion paintRot;

        if (raycastManager.Raycast(screenPos, rayHits, TrackableType.All))
        {
            var hit = rayHits[0];
            Pose pose = hit.pose;

            paintPos = pose.position;
            paintRot = pose.rotation;
        }
        else
        {
            Ray ray = Camera.main.ScreenPointToRay(screenPos);
            if (!Physics.Raycast(ray, out RaycastHit hitInfo))
                return;

            paintPos = hitInfo.point;
            paintRot = Quaternion.LookRotation(hitInfo.normal, Vector3.up);
        }

        if (hasLast && Vector3.Distance(lastPos, paintPos) < spacing)
            return;

        GameObject blob = Instantiate(blobPrefab, paintPos, paintRot);
        blob.transform.localScale = Vector3.zero;

        float randomScale = Random.Range(minFinalScale, maxFinalScale);
        blob.AddComponent<Grow>().Initialize(randomScale, growSpeed);

        if (Time.time - lastPlaceTime < placeDelay)
        {
        return;
        }

        lastPlaceTime = Time.time;
        AudioSource.PlayClipAtPoint(placeSound, paintPos, placeVolume);
    

    }
}
