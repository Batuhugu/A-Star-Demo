using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public bool IsGameWorking { get; set; }

    [SerializeField] GameObject menuPanel;
    [SerializeField] GameObject gameoverPanel;
    [SerializeField] GameObject colorsPanel;
    [SerializeField] Snake snakeScript;
    [SerializeField] ButtonStateHandler buttonStateHandler;

    [Header("Texts")]
    [SerializeField] TextMeshProUGUI gameOver_scoreText;
    [SerializeField] TextMeshProUGUI inGame_scoreText;
    [SerializeField] TextMeshProUGUI highestScoreText;

    [Header("Volume")]
    [SerializeField] GameObject soundButton;
    [SerializeField] AudioSource audioSource;
    [SerializeField] Image imageMusicOn;
    [SerializeField] Image imageMusicOff;

    [HideInInspector] public int score = 0;
    [SerializeField] private string URL;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Application.targetFrameRate = 60;
        // This was for testing color options. They unlocked according to score.
        // PlayerPrefs.SetInt("Score", 0);
    }

    public void StartGame()
    {
        IsGameWorking = true;
        menuPanel.SetActive(false);
        soundButton.SetActive(false);
        inGame_scoreText.gameObject.SetActive(true);
        snakeScript.ResetState(false);
        UpdateScore();
    }

    public void GameOver()
    {
        AudioManager.Instance.UpdateMusicPitch(false, false);
        UpdateScore();
        SetScore();
        IsGameWorking = false;
        inGame_scoreText.gameObject.SetActive(false);
        gameoverPanel.SetActive(true);
        soundButton.SetActive(true);
    }

    public void Menu()
    {
        AudioManager.Instance.UpdateMusicPitch(false, true);
        inGame_scoreText.gameObject.SetActive(false);
        gameoverPanel.SetActive(false);
        menuPanel.SetActive(true);
        soundButton.SetActive(true);
        snakeScript.ResetState(true);
    }

    public void RestartGame()
    {
        IsGameWorking = true;
        inGame_scoreText.gameObject.SetActive(true);
        gameoverPanel.SetActive(false);
        soundButton.SetActive(false);
        snakeScript.ResetState(false);
        UpdateScore();
    }

    public void ColorsPanel()
    {
        colorsPanel.SetActive(!colorsPanel.activeSelf);
        if (colorsPanel.activeSelf)
        {
            //Debug.Log("Highest score: " + PlayerPrefs.GetInt("Score"));
            buttonStateHandler.CheckScore(PlayerPrefs.GetInt("Score"));
            buttonStateHandler.SetObjectsAlpha();
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void UpdateScore()
    {
        gameOver_scoreText.text = "Score: " + score;
        inGame_scoreText.text = score.ToString();
    }

    public void OnClick_Music()
    {
        audioSource.volume = audioSource.volume == 0 ? 1 : 0;
        AudioManager.Instance.ToggleMusic();
        imageMusicOff.enabled = !imageMusicOff.enabled;
        imageMusicOn.enabled = !imageMusicOn.enabled;
    }

    public void OnClick_Link()
    {
        Application.OpenURL(URL);
    }

    private void SetScore()
    {
        if (PlayerPrefs.HasKey("Score"))
        {
            int oldScore = PlayerPrefs.GetInt("Score");
            if (score > oldScore)
            {
                PlayerPrefs.SetInt("Score", score);
                PlayerPrefs.Save();
            }
        }
        else
        {
            PlayerPrefs.SetInt("Score", score);
            PlayerPrefs.Save();
        }
        highestScoreText.SetText("Highest Score: " + PlayerPrefs.GetInt("Score"));
    }
}
