using UnityEngine;

public class Grow : MonoBehaviour
{
    float target;
    float speed;

    public void Initialize(float t, float s)
    {
        target = t;
        speed = s;
    }

    void Update()
    {
        transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.one * target, Time.deltaTime * speed);
    }
}