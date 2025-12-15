using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class StickerPainter : MonoBehaviour
{
    public ARRaycastManager raycastManager;
    public ARPlaneManager planeManager;
    public GameObject[] stickerPrefabs;
    public AudioClip placeSound;
    public float placeVolume = 0.8f;
    public float placeDelay = 0.1f;
    private float lastPlaceTime = 0f;
    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private void Update()
    {
        #if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
            {
                if (!IsOverUI(Input.mousePosition))
                    TryPlaceSticker(Input.mousePosition);
            }
        #else
            if (Input.touchCount > 0)
            {
                Touch t = Input.GetTouch(0);

                if (IsOverUI(t.position))
                    return;

                if (t.phase == TouchPhase.Began)
                    TryPlaceSticker(t.position);
            }
        #endif
    }

    void TryPlaceSticker(Vector2 screenPosition)
    {
        if (!raycastManager.Raycast(screenPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            return;
        }

        Pose pose = hits[0].pose;
        Quaternion rot = Quaternion.LookRotation(pose.up, Vector3.up);
        GameObject chosenSticker = stickerPrefabs[Random.Range(0, stickerPrefabs.Length)];
        Instantiate(chosenSticker, pose.position, rot);
        if (Time.time - lastPlaceTime < placeDelay)
        {
            return;
        }

        lastPlaceTime = Time.time;
        AudioSource.PlayClipAtPoint(placeSound, pose.position, placeVolume);
    }

    bool IsOverUI(Vector2 screenPos)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = screenPos;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        return results.Count > 0;
    }
}