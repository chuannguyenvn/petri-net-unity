using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashScreen : MonoBehaviour
{
    public GameObject[] stripes;
    public float speed;
    public bool hiding;

    void OnEnable()
    {
        // foreach (GameObject stripe in stripes)
        // {
        //     stripe.transform.localScale = new Vector3(6, 10, 1);
        // }

        hiding = false;
    }

    void Update()
    {
        if (hiding)
        {
            foreach (GameObject stripe in stripes)
            {
                if (stripe.transform.localScale.x < 5f)
                    stripe.transform.localScale += Vector3.right * Time.deltaTime * speed;
            }
        }
        else
        {
            foreach (GameObject stripe in stripes)
            {
                if (stripe.transform.localScale.x > 0)
                    stripe.transform.localScale -= Vector3.right * Time.deltaTime * speed;
                else
                    stripe.transform.localScale = new Vector3(0, 10, 1);
            }
        }
    }
}