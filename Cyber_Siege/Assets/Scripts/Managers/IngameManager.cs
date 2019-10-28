using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameManager : MonoBehaviour
{
    public int maxBulletHoleAmount;
    List<GameObject> bulletHoles = new List<GameObject>();

    public Coroutine currentTimer;
    float totalTime;

    [SerializeField] IngameUIManager uiManager;

    [SerializeField] TriggerEvent entrance, exit;

    public List<Target> targets;

    private void Awake()
    {
        if (entrance)
        {
            entrance.onTrigger += StartGame;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            ChangeTime(-5);
        }
    }


    public void AddBulletHole(GameObject bulletHole)
    {
        if(maxBulletHoleAmount > 0)
        {
            if (bulletHoles.Count == maxBulletHoleAmount)
            {
                Destroy(bulletHoles[0]);
                bulletHoles.RemoveAt(0);
            }
            bulletHoles.Add(bulletHole);
        }
        else
        {
            Destroy(bulletHole);
        }
    }

    public void StartGame()
    {
        print("BET");
        uiManager.timerText.gameObject.SetActive(true);
        StartTimer();
        entrance.onTrigger -= StartGame;
        exit.onTrigger += FinishGame;
    }
    public void FinishGame()
    {
        StopTimer();
        exit.onTrigger -= FinishGame;

        float penaltyTime = 0;
        foreach(Target target in targets)
        {
            penaltyTime += target.timePenaltyOnAlive;
        }
        if(penaltyTime != 0)
        {
            ChangeTime(-penaltyTime);
        }
    }



    public void StartTimer()
    {
        currentTimer = StartCoroutine(Timer());
    }

    IEnumerator Timer()
    {
        totalTime = 0;
        uiManager.timerText.text = totalTime.ToString("F2");
        while (true)
        {
            yield return null;
            totalTime += Time.deltaTime;
            uiManager.timerText.text = totalTime.ToString("F2");
        }
    }

    public void StopTimer()
    {
        StopCoroutine(currentTimer);
    }
    public void ResetTimer()
    {
        totalTime = 0;
        uiManager.timerText.text = totalTime.ToString("F2");
    }
    public void ChangeTime(float amount)
    {
        if(totalTime + amount < 0)
        {
            amount = -totalTime;
        }
        totalTime += amount;
        uiManager.timerText.text = totalTime.ToString("F2");
        uiManager.timeChangeText.text = amount.ToString("F2");
        uiManager.animator.SetTrigger("Next");
        uiManager.animator.Play("TimeChangeAnim");
    }
}
