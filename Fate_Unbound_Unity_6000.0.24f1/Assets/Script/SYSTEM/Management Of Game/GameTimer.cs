using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameTimer : SimpleSingleton<GameTimer>, ISceneTargetProvider
{
    public TMP_Text timerText; // Assign this in the Inspector
    public string sceneAfterWinning = "putSceneName";
    public float timeRemaining = 60f; // 1 minute in seconds
    private bool timerIsRunning = true;

    public Player pm;

    protected virtual void Start()
    {
        pm = FindObjectOfType<Player>();
    }

    void Update()
    {
        if (timerIsRunning)
        {
            if (pm.CurrentHP <= 0)
            {
                timerIsRunning = false;
                DisplayTime(timeRemaining);
                UIManager.Instance.OpenReplace(GameUIID.YouLose);
                Destroy(pm.gameObject);
                Time.timeScale = 0f;
            }
            else
            {
                if (timeRemaining > 0 && pm != null)
                {
                    timeRemaining -= Time.deltaTime;
                    DisplayTime(timeRemaining);
                }
                else
                {
                    Debug.Log("Time has run out!");
                    timeRemaining = 0;
                    timerIsRunning = false;
                    UIManager.Instance.OpenReplace(GameUIID.YouWin);
                    DestroyAllEnemies();
                    Time.timeScale = 0f;
                    // DisplayTime(0);
                }
            }
        }
    }

    public string GetSceneName()
    {
        return sceneAfterWinning;
    }

    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay = Mathf.Clamp(timeToDisplay, 0, Mathf.Infinity);

        int minutes = Mathf.FloorToInt(timeToDisplay / 60);
        int seconds = Mathf.FloorToInt(timeToDisplay % 60);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void DestroyAllEnemies()
    {
        Enemy[] allEnemies = FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in allEnemies)
        {
            Destroy(enemy.gameObject);
        }
    }
}