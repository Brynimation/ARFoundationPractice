using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class StartUI : MonoBehaviour
{
    [SerializeField] Button startButton;
    [SerializeField] float transitionTime;
    [SerializeField] CanvasGroup canvasGroup;
    private float timeElapsed;
    void Start()
    {
        startButton.onClick.AddListener(StartChangeScene);
    }
    void StartChangeScene() 
    {
        StartCoroutine(ChangeSceneTransition());
    }

    IEnumerator ChangeSceneTransition() 
    {
        while (timeElapsed < transitionTime) 
        {
            canvasGroup.alpha = timeElapsed / transitionTime;
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        SceneManager.LoadScene(1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
