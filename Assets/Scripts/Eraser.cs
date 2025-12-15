using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Eraser : MonoBehaviour
{
    public float maxDistance = 5f;
    public AudioClip placeSound;
    public float placeVolume = 0.8f;
    public float placeDelay = 0.1f;
    private float lastPlaceTime = 0f;

    private void Update()
    {
        #if UNITY_EDITOR
            if (Input.GetMouseButton(0))
            {
                if (!IsPointerOverUI(Input.mousePosition)) {
                    TryErase(Input.mousePosition);
                }
            }
        #else
            if (Input.touchCount > 0)
            {
                Touch t = Input.GetTouch(0);
                if (!IsPointerOverUI(t.position)) {
                    TryErase(t.position);
                }
            }
        #endif
    }

    bool IsPointerOverUI(Vector2 pos)
    {
        PointerEventData data = new(EventSystem.current) { position = pos };
        List<RaycastResult> results = new();
        EventSystem.current.RaycastAll(data, results);
        return results.Count > 0;
    }

    void TryErase(Vector2 screenPos)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, maxDistance))
        {
            GameObject target = hitInfo.collider.gameObject;

            if (target.CompareTag("PaintObject"))
            {
                Destroy(target);
                if (Time.time - lastPlaceTime < placeDelay) {
                    return;
                }

                lastPlaceTime = Time.time;
                AudioSource.PlayClipAtPoint(placeSound, target.transform.position, placeVolume);
                return;
            }
        }
    }
}
