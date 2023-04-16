using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    float lastTimeScale;
    [SerializeField] GameObject pauseMenuUI;
    public bool gameCanBePaused;
    [SerializeField] GameObject textManager;
    // Update is called once per frame

    static PauseMenu instance;
    private void Start()
    {
        textManager.SetActive(false);
        pauseMenuUI.SetActive(false);
        GameAudioManager.GetInstance().PlayMusic1();
    }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }
    
    public static PauseMenu GetInstance()
    {
        return instance;
    }

        void Update()
        {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }


    public void Resume()
    {
        textManager.SetActive(false);
        pauseMenuUI.SetActive(false);
        GameAudioManager.GetInstance().NormalMusicPitch();
        Time.timeScale = 1;   //segurament s ha de fer d un altre manera
        GameIsPaused = false;
    }

    void Pause()
    {
        if (gameCanBePaused)
        {
            
            EventSystem.current.SetSelectedGameObject(null);
            GameAudioManager.GetInstance().PausedMusicPitch();
            textManager.SetActive(true);
            pauseMenuUI.SetActive(true);
            lastTimeScale = Time.timeScale;
            Time.timeScale = 0;   //segurament s ha de fer d un altre manera
            GameIsPaused = true;
        }
    }

    public void LoadMenu()
    {
        Time.timeScale = 1;
        SceneLoader.GetInstance().StartLoadMainMenu();
        pauseMenuUI.SetActive(false);
    }
}
