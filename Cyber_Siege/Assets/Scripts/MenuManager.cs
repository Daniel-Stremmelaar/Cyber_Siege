using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public List<GameObject> windows = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AssignScene (int i)
    {
        SceneManager.LoadScene(i);
    }

    public void AssignWindow (int i)
    {
        windows[i].SetActive(!windows[i].active);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
