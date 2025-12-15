using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.EventSystems;

public class TreePainter : MonoBehaviour
{
    public GameObject[] treePrefabs;
    public ARRaycastManager raycastManager;
    public ARPlaneManager planeManager;
    public AudioClip placeSound;
    public float placeVolume = 0.8f;
    public float placeDelay = 0.1f;
    private float lastPlaceTime = 0f;
    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);

            if (IsOverUI(t.position))
                return;

            if (t.phase == TouchPhase.Began)
                TryPlantTree(t.position);
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (!IsOverUI(Input.mousePosition))
                TryPlantTree(Input.mousePosition);
        }
    }

    void TryPlantTree(Vector2 screenPos)
    {

        if (raycastManager.Raycast(screenPos, hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;
            ARPlane plane = planeManager.GetPlane(hits[0].trackableId);

            if (plane.alignment == PlaneAlignment.HorizontalUp)
            {
                GameObject chosenTree = treePrefabs[Random.Range(0, treePrefabs.Length)];
                Instantiate(chosenTree, hitPose.position, Quaternion.identity);
                if (Time.time - lastPlaceTime < placeDelay)
                {
                    return;
                }

                lastPlaceTime = Time.time;
                AudioSource.PlayClipAtPoint(placeSound, hitPose.position, placeVolume);
            }
        }
    }

    bool IsOverUI(Vector2 pos)
    {
        PointerEventData e = new PointerEventData(EventSystem.current);
        e.position = pos;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(e, results);
        return results.Count > 0;
    }
}
