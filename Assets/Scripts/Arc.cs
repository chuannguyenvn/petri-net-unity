using UnityEngine;

// The arc linking states and transitions //
public class Arc : MonoBehaviour
{
    public Transform origin; // Holds the transform of the "tail" of the arc
    public Transform target; // Holds the transform of the "tail" of the arc

    public GameObject arrowHead;
    public float thickness = 5f;

    private RectTransform rectTransform;

    void Start()
    {
        // The arc is actually a thin rectangle, rotated as will
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        // Destroy the arc immediately if missing any point
        if (origin == null || target == null || origin.gameObject.activeSelf == false ||
            target.gameObject.activeSelf == false) Destroy(gameObject);
        
        transform.SetSiblingIndex(0); // Arcs should always be behind other objects
        
        if (origin == null || target == null) return;

        Vector2 pointA = origin.position * 1920 / Screen.width; // Position of the origin
        Vector2 pointB = target.position * 1920 / Screen.width; // Position of the target
        // Multiply by 1920/[screen width] to get the correct screen space positions

        Vector2 direction = (pointB - pointA).normalized;
        float distance = Vector2.Distance(pointA, pointB);

        // Adjusting the rectangle
        rectTransform.sizeDelta = new Vector2(distance, thickness);
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.zero;
        rectTransform.anchoredPosition = (pointB - pointA) / 2 + pointA;

        // Calculating the rotation
        float rotateDeg = Mathf.Rad2Deg * Mathf.Atan(direction.y / direction.x);
        rectTransform.localEulerAngles = rotateDeg * Vector3.forward;

        if (target.GetComponent<Destination>() != null)
        {
            // Only enable target's collider when raycasting to avoid hitting unwanted colliders
            target.GetComponent<Collider2D>().enabled = true;
            RaycastHit2D ray = Physics2D.Raycast(origin.position, direction);
            target.GetComponent<Collider2D>().enabled = false;

            // Position the head to the raycast's hit position
            arrowHead.transform.position = ray.point;
        }
        else arrowHead.transform.position = target.transform.position; // Else position it to mouse

        if (Mathf.Abs(Vector3.SignedAngle(direction, Vector3.right, Vector3.forward)) > 90)
            rotateDeg += 180; // Flip the arrow if needed

        arrowHead.transform.eulerAngles = Vector3.forward * rotateDeg; // Finally set the rotation
    }
}