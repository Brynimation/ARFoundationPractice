using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/*#if UNITY_EDITOR
        faceRenderer = GameObject.FindGameObjectWithTag("Player").GetComponent<Renderer>();
#endif*/

public class BalloonUI : MonoBehaviour
{
    [Header("UI Text")]
    [SerializeField] TMP_Text ScoreText;
    [SerializeField] TMP_Text QuestionText;
    [SerializeField] TMP_Text TimerText;

    [Header("Buttons")]
    [SerializeField] Button RestartButton;
    [SerializeField] Button QuitButton;
    private int score = 0;
    private BalloonSpawner balloonSpawner;
    void Awake()
    {
        balloonSpawner = FindObjectOfType<BalloonSpawner>();

        balloonSpawner.OnSpawnNextQuestion += UpdateQuestionText;
        balloonSpawner.OnUpdateTimeRemaining += UpdateTimeRemainingText;

        Balloon.OnDestroyBalloon += UpdateScoreText;
        UpdateScoreText(0);
     }

    private void Start()
    {
        RestartButton.onClick.AddListener(Restart);
        QuitButton.onClick.AddListener(Quit);
    }

    private void Restart() 
    {
        SceneManager.LoadScene(0);
    }
    private void Quit()
    {
        Application.Quit();
    }
    private void UpdateQuestionText(string text) 
    {
        Debug.Log(text);
        QuestionText.SetText(text);
    }
    private void UpdateTimeRemainingText(string text) 
    {
        TimerText.SetText(text);
    }
    private void UpdateScoreText(int value) 
    {
        score += value;
        ScoreText.SetText("Score: "+score.ToString());
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
