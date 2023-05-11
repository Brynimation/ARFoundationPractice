using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using System;
using UnityEngine.XR.ARFoundation;

[System.Serializable]
public struct QuestionAnswer
{
    public string question;

    public List<string> answers;
    public List<int> correctAnswerIndices;
}
public class BalloonSpawner : MonoBehaviour
{
    public float timePerQuestion;
    public List<QuestionAnswer> questionAnswers;
    public Balloon balloonPrefab;
    public Action<string> OnSpawnNextQuestion;
    public Action<string> OnUpdateTimeRemaining;

    private float remainingTime;
    private bool started = false;
    private ARFace ARFace;
    private Renderer faceRenderer;
    int currentQuestionIndex;
    void Start()
    {
        remainingTime = timePerQuestion;
        StartCoroutine(DelayStart());

    }

    private float GetScreenSpaceDiagonal(Bounds bounds) 
    {
        Vector3 min = Camera.main.WorldToScreenPoint(bounds.min);
        Vector3 max = Camera.main.WorldToScreenPoint(bounds.max);
        return (max - min).magnitude;
    }

    IEnumerator DelayStart()
    {
        while (ARFace == null) 
        {
            ARFace = FindObjectOfType<ARFace>();
            yield return null;
        }
        faceRenderer = ARFace.GetComponent<Renderer>();
        //Tell the player to stand further back
        OnSpawnNextQuestion?.Invoke("Move away from the camera");

        //Only start the game once the player is far enough away from the camera
        while (GetScreenSpaceDiagonal(faceRenderer.bounds) > 500) 
        {
            yield return null;
        }
        OnSpawnNextQuestion?.Invoke("Starting in 3");
        yield return new WaitForSeconds(1f);
        OnSpawnNextQuestion?.Invoke("Starting in 2");
        yield return new WaitForSeconds(1f);
        OnSpawnNextQuestion?.Invoke("Starting in 1");
        yield return new WaitForSeconds(1f);

        started = true;
        OnSpawnNextQuestion?.Invoke(questionAnswers[currentQuestionIndex].question);
        OnUpdateTimeRemaining?.Invoke(timePerQuestion.ToString());
        SpawnAllAnswers();
    }

    void SpawnAllAnswers() 
    {

        //If we've reached the end of the list
        if (currentQuestionIndex == questionAnswers.Count) 
        {
            OnSpawnNextQuestion?.Invoke("Finished!");
            return;
        }

        List<string> answers = questionAnswers[currentQuestionIndex].answers;
        List<int> correctIndices = questionAnswers[currentQuestionIndex].correctAnswerIndices;
        //Spawn balloons containing the answers
        for (int i = 0; i < answers.Count; i++)
        {
            string answer = answers[i];
            Vector2 randomViewportPos = new Vector2(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));
            SpawnBalloonAnswer(randomViewportPos, answer, i, correctIndices);
        }

        //Destroy all balloons created for the previous question


        //Send event to UI to change the question
        OnSpawnNextQuestion?.Invoke(questionAnswers[currentQuestionIndex].question);
       
        //Move to next quesetion
        currentQuestionIndex++;
    }
    void SpawnBalloonAnswer(Vector2 viewportPosition, string answerText, int curIndex, List<int> correctIndices) 
    {
        Balloon balloon = Instantiate(balloonPrefab, transform.position, Quaternion.identity);
        balloon.InitialiseAnswer(viewportPosition, answerText, curIndex, correctIndices);

    }

    private void DestroyPreviousAnswers() 
    {
        Balloon[] balloons = FindObjectsOfType<Balloon>();
        foreach (Balloon balloon in balloons) 
        {
            Destroy(balloon.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (ARFace == null && started) 
        {
            started = false;
            ARFace = FindObjectOfType<ARFace>();
            if(ARFace != null) 
            {
                faceRenderer = ARFace.GetComponent<Renderer>();
                started = true;
            }
        }
        if (!started) return;
        Debug.Log("Diagonal length: " + GetScreenSpaceDiagonal(faceRenderer.bounds));
        remainingTime -= Time.deltaTime;
        if (remainingTime <= 0) 
        {
            remainingTime = timePerQuestion;
            DestroyPreviousAnswers();
            SpawnAllAnswers();
        }
        float roundedRemainingTime = (float) Math.Round(remainingTime, 2);
        OnUpdateTimeRemaining?.Invoke(roundedRemainingTime.ToString());
    }
}
