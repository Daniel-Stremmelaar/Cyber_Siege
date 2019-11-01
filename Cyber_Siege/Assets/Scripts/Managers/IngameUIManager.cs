using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IngameUIManager : MonoBehaviour
{
    public GameObject crosshair;
    public Image hitMarker;
    public float hitmarkerFadeSpeed;
    public float hitmarkerDurationBeforeFade;
    public Coroutine hitmarkerRoutine;

    public TextMeshProUGUI clipAmmo, storedAmmo, weaponName;

    public TextMeshProUGUI timerText, timeChangeText;
    public GameObject timerHolder;

    public TextMeshProUGUI interactionText;

    public Animator animator;

    public GameObject pauseMenu;
    public List<GameObject> gameUI = new List<GameObject>();
    public Player player;

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (pauseMenu.active == false)
            {
                foreach (GameObject g in gameUI)
                {
                    g.SetActive(false);
                }
                pauseMenu.SetActive(true);
                Time.timeScale = 0;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                ResumeGame();
            }
        }
    }

    public IEnumerator Hitmarker()
    {
        Color resetColor = hitMarker.color;
        resetColor.a = 1;
        hitMarker.color = resetColor;
        hitMarker.gameObject.SetActive(true);
        yield return new WaitForSeconds(hitmarkerDurationBeforeFade);
        Color hitmarkerColor = hitMarker.color;
        while (hitmarkerColor.a > 0)
        {
            hitmarkerColor.a -= hitmarkerFadeSpeed * Time.deltaTime;
            hitmarkerColor.a = Mathf.Max(0, hitmarkerColor.a);
            hitMarker.color = hitmarkerColor;
            yield return null;
        }
        hitMarker.gameObject.SetActive(false);
        hitmarkerRoutine = null;
    }

    public void ResumeGame()
    {
        foreach (GameObject g in gameUI)
        {
            g.SetActive(true);
        }
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
