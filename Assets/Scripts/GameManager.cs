using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static GameManager Instance { get; private set; }

    private GroundPiece[] _pieces;

    private bool isFinished;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if(Instance != this)
        {
            Destroy(gameObject);
            DontDestroyOnLoad(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    private void Start()
    {
        SetUpNextLevel();
    }
    void SetUpNextLevel()
    {
        _pieces = FindObjectsOfType<GroundPiece>();
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        SetUpNextLevel();
    }


    public void CheckComplete()
    {
        isFinished = true;

        if(_pieces.ToList().Exists(x => x.isColored == false))
        {
            isFinished = false;
        }

        if (isFinished)
        {
            NextLevel();
        }
    }

    void NextLevel()
    {
        var sceneCount = SceneManager.sceneCountInBuildSettings;
        int nextScene = SceneManager.GetActiveScene().buildIndex == sceneCount - 1
            ? default : SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(nextScene);
    }
}
