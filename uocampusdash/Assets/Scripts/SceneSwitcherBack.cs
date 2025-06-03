using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcherBack : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            SceneManager.LoadScene("BRP Sample Scene");
        }

        if (SceneManager.GetActiveScene().name == "BRP Sample Scene" && Input.GetKeyDown(KeyCode.H))
        {
            SceneManager.LoadScene("campus");
        }
    }
}