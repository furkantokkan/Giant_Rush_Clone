using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class LevelManager : MonoBehaviour
{
    private static LevelManager instance;
    public static LevelManager Instance { get { return instance; } }

    [SerializeField] private List<GameObject> tutorialLevels = new List<GameObject>();
    [SerializeField] private List<GameObject> orderedLevels = new List<GameObject>();
    [SerializeField] private List<GameObject> randomLevels = new List<GameObject>();

    private int currentLevelIndex;
    private int lastIndex = -1;
    private int playedLevelCount;

    private bool isRestarted = false;

    private GameObject currentLevel;

#pragma warning disable
    public event Action onNewLevelLoaded;
    public event Action beforeNewLevelLoad;
    public Action onWinEvent;
    public Action onLoseEvent;
#pragma warning restore
    private bool nextLevelIndexTaked = false;
    private enum levelState
    {
        Tutorial,
        Ordered,
        Random,
        None
    }
    private levelState currentLevelState = levelState.Tutorial;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        currentLevelIndex = PlayerPrefs.GetInt("CurrentLevel");
        playedLevelCount = PlayerPrefs.GetInt("LevelCount");
        currentLevelState = (levelState)PlayerPrefs.GetInt("CurrentState");
    }
    void Start()
    {
        if (currentLevelState != levelState.Random)
        {
            GetNewLevel(false);
        }
        else
        {
            //oyun kapatýlý açýnca farklý level gelsin diyorsanýz true yapýn
            GetNewLevel(false);
        }
    }
    private void OnEnable()
    {
        onWinEvent += SaveNextLevelIndex;
    }
    private void OnDisable()
    {
        onWinEvent -= SaveNextLevelIndex;
    }
    private void GetNewLevel(bool newLevel)
    {
        bool finished = false;

        while (!finished)
        {
            switch (currentLevelState)
            {
                case levelState.Tutorial:
                    if (tutorialLevels.Count > 0 && currentLevelIndex <= tutorialLevels.Count - 1)
                    {
                        SpawnNewLevel(tutorialLevels[currentLevelIndex]);
                        finished = true;
                    }
                    else
                    {
                        SetKeys(0, 1);
                    }
                    break;
                case levelState.Ordered:
                    if (orderedLevels.Count > 0 && currentLevelIndex <= orderedLevels.Count - 1)
                    {
                        SpawnNewLevel(orderedLevels[currentLevelIndex]);
                        finished = true;
                    }
                    else
                    {
                        SetKeys(0, 2);
                    }
                    break;
                case levelState.Random:
                    if (randomLevels.Count > 0 && currentLevelIndex <= randomLevels.Count - 1)
                    {
                        if (!newLevel)
                        {
                            SpawnNewLevel(randomLevels[currentLevelIndex]);
                        }
                        else
                        {
                            if (randomLevels.Count > 1)
                            {
                                SpawnNewLevel(randomLevels[GetRandomLevelIndex()]);
                            }
                            else
                            {
                                SpawnNewLevel(randomLevels[currentLevelIndex]);
                            }
                        }

                        finished = true;
                    }
                    else
                    {
                        if (randomLevels.Count <= 0 && orderedLevels.Count <= 0)
                        {
                            finished = true;
                        }
                        else
                        {
                            SetKeys(0, 1);
                        }
                    }
                    break;
                case levelState.None:
                    print("Levels Not Founded");
                    SetKeys(0, 2);
                    finished = true;
                    break;
                default:
                    print("Levels Not Founded");
                    SetKeys(0, 2);
                    finished = true;
                    break;
            }
        }
    }
    public GameObject GetCurrentLevelPrefab()
    {
        return currentLevel;
    }
    public int GetLevel()
    {
        return playedLevelCount + 1;
    }
    public void NextLevel()
    {
        GetNewLevel(true);
        beforeNewLevelLoad?.Invoke();
        isRestarted = false;
        nextLevelIndexTaked = false;
    }
    [ContextMenu("Next Level Debug")]
    private void NextLevelDebug()
    {
        isRestarted = false;
        nextLevelIndexTaked = false;
        beforeNewLevelLoad?.Invoke();
        SaveNextLevelIndex();
        GetNewLevel(true);
    }
    [ContextMenu("Restart Level Debug")]
    public void RestartLevel()
    {
        beforeNewLevelLoad?.Invoke();
        GetNewLevel(false);
        isRestarted = true;
    }
    private void SaveNextLevelIndex()
    {
        if (isRestarted)
        {
            return;
        }
        if (!nextLevelIndexTaked)
        {
            int index = currentLevelIndex += 1;
            int levelCount = playedLevelCount += 1;
            PlayerPrefs.SetInt("LevelCount", levelCount);
            PlayerPrefs.SetInt("CurrentLevel", index);
            currentLevelIndex = index;
            nextLevelIndexTaked = true;
        }
    }
    private int GetRandomLevelIndex()
    {
        int index;
        do
        {
            index = UnityEngine.Random.Range(0, randomLevels.Count);

            if (lastIndex != index)
            {
                lastIndex = index;
                currentLevelIndex = index;
                PlayerPrefs.SetInt("CurrentLevel", index);
                break;
            }
        } while (lastIndex == index);

        return currentLevelIndex;
    }
    private void SpawnNewLevel(GameObject newLevel)
    {
        if (currentLevel != null)
        {
            Destroy(currentLevel);
        }
        onNewLevelLoaded?.Invoke();
        currentLevel = Instantiate(newLevel, Vector3.zero, Quaternion.identity);
    }
    private void SetKeys(int levelIndex, int stateIndex)
    {
        PlayerPrefs.SetInt("CurrentLevel", levelIndex);
        currentLevelIndex = levelIndex;
        PlayerPrefs.SetInt("CurrentState", stateIndex);
        currentLevelState = (levelState)stateIndex;
    }
    [ContextMenu("Delete All Keys")]
    private void ResetKeys()
    {
        PlayerPrefs.DeleteAll();
        currentLevelIndex = 0;
        currentLevelState = levelState.Tutorial;
    }
}
