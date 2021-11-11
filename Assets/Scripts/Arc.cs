using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arc : MonoBehaviour
{
    public Transform origin;
    public Transform target;

    public GameObject arrowHead;
    public float thickness = 5f;
    
    private RectTransform rectTransform;
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    
    void Update()
    {
        if (origin == null || target == null) Destroy(gameObject);
        transform.SetSiblingIndex(0);
        if (origin == null || target == null)return;

        Vector2 pointA = origin.position * 1920 / Screen.width;
        Vector2 pointB = target.position* 1920 / Screen.width;
        Vector2 direction = (pointB - pointA).normalized;
        float distance = Vector2.Distance(pointA, pointB);
        
        rectTransform.sizeDelta = new Vector2(distance, thickness);
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.zero;
        rectTransform.anchoredPosition = (pointB - pointA) / 2 + pointA;
        
        float rotateDeg = Mathf.Rad2Deg * Mathf.Atan(direction.y / direction.x);
        
        rectTransform.localEulerAngles =  rotateDeg * Vector3.forward;
        
        if (target.GetComponent<State>() != null)
        {
            target.GetComponent<Collider2D>().enabled = true;
            RaycastHit2D ray = Physics2D.Raycast(origin.position, direction);
            target.GetComponent<Collider2D>().enabled = false;
            arrowHead.transform.position = ray.point;            if (Mathf.Abs(Vector3.SignedAngle(direction, Vector3.right, Vector3.forward)) > 90 &&
                arrowHead.transform.localScale.x > 0)
            {
                rotateDeg += 180;
            }
            
            arrowHead.transform.eulerAngles = Vector3.forward * rotateDeg;
        }
        else if (target.GetComponent<Transition>() != null)
        {
            target.GetComponent<Collider2D>().enabled = true;
            RaycastHit2D ray = Physics2D.Raycast(origin.position, direction);
            target.GetComponent<Collider2D>().enabled = false;
            arrowHead.transform.position = ray.point;
            if (Mathf.Abs(Vector3.SignedAngle(direction, Vector3.right, Vector3.forward)) > 90 &&
                arrowHead.transform.localScale.x > 0)
            {
                rotateDeg += 180;
            }
            
            arrowHead.transform.eulerAngles = Vector3.forward * rotateDeg;
        }
        else
        {
            arrowHead.transform.position = target.transform.position;
            if (Mathf.Abs(Vector3.SignedAngle(direction, Vector3.right, Vector3.forward)) > 90 &&
                arrowHead.transform.localScale.x > 0)
            {
                rotateDeg += 180;
            }
            
            arrowHead.transform.eulerAngles = Vector3.forward * rotateDeg;

        }
    }
}
