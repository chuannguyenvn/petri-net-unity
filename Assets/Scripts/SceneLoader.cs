using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) SceneManager.LoadScene(0);
    }

    public void GetSceneItem1()
    {
        SceneManager.LoadScene(1);
    }

    public void GetSceneItem2()
    {
        SceneManager.LoadScene(2);
    }

    public void GetSceneItem3()
    {
        SceneManager.LoadScene(3);
    }

    public void GetSceneItem4()
    {
        SceneManager.LoadScene(4);
    }
    
    public void GetSceneDIY()
    {
        SceneManager.LoadScene(5);
    }
}