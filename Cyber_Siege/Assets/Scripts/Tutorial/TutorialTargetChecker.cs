using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTargetChecker : MonoBehaviour
{
    private Tutorial tutorial;
    bool enable = true;
    public List<TutorialTarget> targets = new List<TutorialTarget>();
    // Start is called before the first frame update
    void Start()
    {
        tutorial = GameObject.FindGameObjectWithTag("Tutorial").GetComponent<Tutorial>();
    }

    // Update is called once per frame
    void Update()
    {
        if(targets.Count < 1 && enable)
        {
            enable = false;
            GameObject.FindGameObjectWithTag("Manager").GetComponent<IngameManager>().StopTimer();
            tutorial.NextStage();
        }
    }
}
