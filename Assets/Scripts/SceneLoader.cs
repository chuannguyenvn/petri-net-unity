using UnityEngine;
using UnityEngine.SceneManagement;

// Simple class used to trigger button events to load scenes //
public class SceneLoader : MonoBehaviour
{
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
    
    public void GetSceneNetPlayground()
    {
        SceneManager.LoadScene(5);
    }
    
    public void GetSceneReachableMarkingsPlayground()
    {
        SceneManager.LoadScene(6);
    }

    public void GetSceneMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void GetSceneItem1b()
    {
        SceneManager.LoadScene(7);
    }
}