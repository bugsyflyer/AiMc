using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void RestartScene()
    {
        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        GameObject[] dynamicObjects = GameObject.FindGameObjectsWithTag("DynamicObject");
        foreach (GameObject obj in dynamicObjects)
        {
            Destroy(obj);
        }
    }
}

