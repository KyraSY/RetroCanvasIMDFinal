using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BubblePainter : MonoBehaviour
{
    public GameObject bubblePrefab;
    public float distanceFromCamera = 0.5f;
    public float bubbleSize = 0.05f;
    public float sizeRandomness = 0.02f;
    public AudioClip placeSound;
    public float placeVolume = 0.8f;
    public float placeDelay = 0.1f;
    private float lastPlaceTime = 0f;
    Camera cam;
    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        #if UNITY_EDITOR
            if (Input.GetMouseButton(0))
            {
                if (!IsOverUI(Input.mousePosition))
                    SpawnBubble(Input.mousePosition);
            }
        #else
            if (Input.touchCount > 0)
            {
                Touch t = Input.GetTouch(0);
                if (!IsOverUI(t.position))
                {
                    if (t.phase == TouchPhase.Began ||
                        t.phase == TouchPhase.Moved ||
                        t.phase == TouchPhase.Stationary)
                    {
                        SpawnBubble(t.position);
                    }
                }
            }
        #endif
    }

    bool IsOverUI(Vector2 pos)
    {
        PointerEventData e = new(EventSystem.current) { position = pos };
        List<RaycastResult> results = new();
        EventSystem.current.RaycastAll(e, results);
        return results.Count > 0;
    }

    void SpawnBubble(Vector2 screenPos)
    {

        Vector3 worldPos = cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, distanceFromCamera));
        GameObject bubble = Instantiate(bubblePrefab, worldPos, Quaternion.identity);
        float finalSize = bubbleSize + Random.Range(-sizeRandomness, sizeRandomness);
        bubble.transform.localScale = Vector3.one * finalSize;

        Rigidbody rb = bubble.GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.AddForce(Vector3.up * 0.1f, ForceMode.Impulse);

        Destroy(bubble, 3f);

        if (Time.time - lastPlaceTime < placeDelay)
        {
            return;
        }

        lastPlaceTime = Time.time;
        AudioSource.PlayClipAtPoint(placeSound, worldPos, placeVolume);
    }

    public void SetBrushSize(float size)
    {
        bubbleSize = size;
    }
}
