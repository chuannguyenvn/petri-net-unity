using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Destination : MonoBehaviour
{
    public List<Token> tokens;
    protected Mouse mouse;
    protected Background background;

    public string identifier;
    [SerializeField] InputField inputField;
    public virtual void Start()
    {
        mouse = ProgramManager.Instance.mouse;
        background = ProgramManager.Instance.canvas.GetComponent<Background>();

        StartCoroutine(Spawn_CO());
        inputField.ActivateInputField();
    }

    protected void ClearTokens()
    {
        foreach (Token token in tokens)
        {
        }
        
        tokens.Clear();
    }

    IEnumerator Spawn_CO()
    {
        float spawnTime = 0.3f;
        float coeff = 0.05f;
        while (spawnTime > 0)
        {
            transform.localScale = Vector3.one * (1 + Mathf.Sin(spawnTime * 30) * coeff * spawnTime / 0.5f);
            spawnTime -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        transform.localScale = Vector3.one;
    }
}
