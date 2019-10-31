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

    float addedTimeStreak;
    [SerializeField] float resetDelay;
    Coroutine resetStreakTimer;

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
        if(currentTimer == null)
        {
            currentTimer = StartCoroutine(Timer());
        }
    }
    public void StartCountdown(float timeGiven)
    {
        if(currentTimer == null)
        {
            currentTimer = StartCoroutine(Countdown(timeGiven));
        }
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

    IEnumerator Countdown(float time)
    {
        uiManager.timerText.gameObject.SetActive(true);
        totalTime = time;
        uiManager.timerText.text = totalTime.ToString("F2");
        while (totalTime > 0)
        {
            yield return null;
            totalTime -= Time.deltaTime;
            if (totalTime < 0)
            {
                totalTime = 0;
            }
            uiManager.timerText.text = totalTime.ToString("F2");
        }
        currentTimer = null;
        uiManager.timerText.gameObject.SetActive(false);
    }
    public void StopTimer()
    {
        StopCoroutine(currentTimer);
        currentTimer = null;
    }
    public void ResetTimer()
    {
        totalTime = 0;
        uiManager.timerText.text = totalTime.ToString("F2");
    }

    public void ChangeTime(float amount)
    {
        if (totalTime + amount < 0)
        {
            amount = -totalTime;
        }
        if (amount != 0)
        {
            addedTimeStreak += amount;
            totalTime += amount;
            uiManager.timerText.text = totalTime.ToString("F2");
            uiManager.timeChangeText.text = addedTimeStreak.ToString("F2");
            uiManager.animator.SetTrigger("Next");
            uiManager.animator.Play("TimeChangeAnim");
            if(resetStreakTimer != null)
            {
                StopCoroutine(resetStreakTimer);
            }
            resetStreakTimer = StartCoroutine(StreakReset());
        }
    }

    IEnumerator StreakReset()
    {
        yield return new WaitForSeconds(resetDelay);
        addedTimeStreak = 0;
    }
}
