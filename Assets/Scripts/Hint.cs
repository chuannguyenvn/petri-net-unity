using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Class used to slowly fade the hint //
public class Hint : MonoBehaviour
{
    private TextMeshProUGUI hintText;

    void Start()
    {
        hintText = GetComponent<TextMeshProUGUI>();
        StartCoroutine(WaitAndFade_CO());
    }

    IEnumerator WaitAndFade_CO()
    {
        yield return new WaitForSeconds(5f);

        while (hintText.alpha > 0)
        {
            hintText.alpha -= 0.003f;
            yield return new WaitForSeconds(0.003f);
        }

        Destroy(gameObject);
    }
}