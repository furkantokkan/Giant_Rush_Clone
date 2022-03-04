using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Menu")]
    [SerializeField] private GameObject menuObject;
    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI fpsText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI praiseText;
    [Header("Praise Settings")]
    [SerializeField] private string[] praiseWords;
    [Header("Sliders")]
    [SerializeField] private Slider progressBar;
    [Header("Events")]
    [SerializeField] private UnityEvent onWinUI;
    [SerializeField] private UnityEvent onLoseUI;
    [SerializeField] private UnityEvent onGameStartUI;
    [SerializeField] private UnityEvent forceToCloseOnNewLevel;

    private float deltaTime;
    private int inputCounter = 0;
    private bool isUpdating = false;
    private void Start()
    {
        onGameStartUI?.Invoke();
        UpdateLevelText();

        if (Application.isEditor && !menuObject.activeInHierarchy)
        {
            GameManager.Instance.GameStarted = true;
        }
    }
    private void OnEnable()
    {
        LevelManager.Instance.onWinEvent += ExecuteOnWin;
        LevelManager.Instance.onNewLevelLoaded += UpdateLevelText;
        LevelManager.Instance.onLoseEvent += ExecuteOnLose;
        LevelManager.Instance.onNewLevelLoaded += ForceToClose;
        InputManager.Instance.onTouchStart += CloseTheMenu;
    }

    private void OnDisable()
    {
        LevelManager.Instance.onWinEvent -= ExecuteOnWin;
        LevelManager.Instance.onLoseEvent -= ExecuteOnLose;
        LevelManager.Instance.onNewLevelLoaded -= UpdateLevelText;
        LevelManager.Instance.onNewLevelLoaded -= ForceToClose;
        InputManager.Instance.onTouchStart -= CloseTheMenu;
    }

    public void SetProgresBarMaxValue(float maxValue)
    {
        progressBar.maxValue = maxValue;
    }
    public void UpdateProgressBar(float newTargetValue, float duration = 1f)
    {
        if (isUpdating)
        {
            return;
        }
        StartCoroutine(ProgressBar(newTargetValue, duration));
    }
    private IEnumerator ProgressBar(float newTargetValue, float duration = 1f)
    {
        float startingValue = progressBar.value;
        float targetValue = newTargetValue;
        float elapsedTime = 0f;
        isUpdating = true;

        while (elapsedTime < duration)
        {
            progressBar.value = Mathf.Lerp(startingValue, targetValue, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        progressBar.value = targetValue;
        isUpdating = false;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ScreenshotHandler.TakeScreenshotOnGame();
        }

        ShowFPS();
    }

    /// <summary>
    /// Karýþýk gelmesini istiyorsanýz parametreyi boþ "" yada null býrakýn
    /// </summary>
    /// <param name="newText"></param>
    public void ShowPraiseText(string newText)
    {
        Animator praiseAnim = praiseText.GetComponent<Animator>();

        if (!praiseAnim.IsInTransition(0) && praiseAnim.GetCurrentAnimatorStateInfo(0).IsName("PraiseTextAnimation"))
        {
            return;
        }

        if (String.IsNullOrEmpty(newText))
        {
            praiseText.text = GetRandomWord();
        }
        else
        {
            praiseText.text = newText;
        }

        praiseAnim.SetTrigger("Text");
    }
    public string GetRandomWord()
    {
        int index = UnityEngine.Random.Range(0, praiseWords.Length);
        return praiseWords[index];
    }
    private void ForceToClose()
    {
        inputCounter = 0;
        forceToCloseOnNewLevel?.Invoke();
    }
    private void CloseTheMenu()
    {
        inputCounter++;

        if (inputCounter >= 1 && menuObject.activeInHierarchy)
        {
            menuObject.gameObject.SetActive(false);
        }
        if (inputCounter >= 1)
        {
            GameManager.Instance.GameStarted = true;
        }
        else if (inputCounter >= 2)
        {
            GameManager.Instance.GiveInputToUser = true;
        }
    }
    private void ShowFPS()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        fpsText.text = Mathf.Ceil(fps).ToString();
    }
    private void UpdateLevelText()
    {
        levelText.text = "Level" + " " + LevelManager.Instance.GetLevel().ToString();
    }
    private void ExecuteOnWin()
    {
        onWinUI?.Invoke();
    }
    private void ExecuteOnLose()
    {
        onLoseUI?.Invoke();
    }
}
