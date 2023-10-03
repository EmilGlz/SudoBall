using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


/// <summary>
/// Common GameObjects or Variables and Processes or Methods which can be accessed from anywhere.
/// UI methods are also included here.
/// </summary>
public class CommonDataAndPros : MonoBehaviour
{
    #region Singleton
    private static CommonDataAndPros _instance;
    public static CommonDataAndPros Instance { get { return _instance; } }
    private void Awake()
    {
        _instance = this;
    }
    #endregion

    private const string PP_DIAMOND = "dmnd";
    private const string PP_LEVEL = "lvl";
    private const string PP_SOUND = "snd";

    private Camera mainCamera;

    [SerializeField] private GameObject perfectText;
    [SerializeField] private GameObject goodText;
    [SerializeField] private GameObject missedText;

    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject inGameCanvas;
    [SerializeField] private GameObject endGameCanvas;
    [SerializeField] private GameObject loseGameCanvas;

    #region UI
    [SerializeField] private GameObject touchToPlayPanel;
    [SerializeField] private TMP_Text levelLeft;
    [SerializeField] private TMP_Text levelRight;
    [SerializeField] private TMP_Text[] diamondTexts;
    [SerializeField] private RectTransform diamondAnimRect;
    [SerializeField] private RectTransform diamondOnScoreTable;
    [SerializeField] private Vector2 diamondOnScoreTablePos;
    [SerializeField] private GameObject comboGO;
    [SerializeField] private TMP_Text comboNum;
    [SerializeField] private TMP_Text levelWinText;
    #endregion

    #region Settings
    [SerializeField] private GameObject soundRect;
    [SerializeField] private float soundRectUpHeight = 140f;
    [SerializeField] private GameObject soundRedCross;
    private bool soundOn = true;
    [SerializeField] private GameObject optionsRect;
    private bool settingsOpen = false;
    #endregion

    [HideInInspector] public int diamond;
    [HideInInspector] public int currentLevel;

    private void Start()
    {
        mainCamera = Camera.main;
        if (!PlayerPrefs.HasKey(PP_LEVEL)) // First Time
        {
            diamond = 0;
            currentLevel = 1;
            soundOn = true;
            PlayerPrefs.SetInt(PP_LEVEL, currentLevel);
            PlayerPrefs.SetInt(PP_DIAMOND, diamond);
            PlayerPrefs.SetInt(PP_SOUND, 1);
        }
        else
        {
            diamond = PlayerPrefs.GetInt(PP_DIAMOND);
            currentLevel = PlayerPrefs.GetInt(PP_LEVEL);
            soundOn = PlayerPrefs.GetInt(PP_SOUND) == 1;
            soundRedCross.SetActive(!soundOn);
            AudioManager.Instance.soundOn = soundOn;
        }
        UpdateLevelInfo();
        UpdateDiamond();
    }

    private void UpdateLevelInfo()
    {
        levelLeft.text = currentLevel.ToString();
        levelRight.text = (currentLevel + 1).ToString();
    }

    //public void NextLevel()
    //{
    //    PlayerPrefs.SetInt(PP_LEVEL,++currentLevel);
    //    levelLeft.text = currentLevel.ToString();
    //    levelRight.text = (currentLevel + 1).ToString();
    //}

    private void UpdateDiamond()
    {
        PlayerPrefs.SetInt(PP_DIAMOND, diamond);
        for (int i = 0; i < diamondTexts.Length; i++)
        {
            diamondTexts[i].text = diamond.ToString();
        }
    }

    public void AddDiamond(int add, Vector2 screenPos)
    {
        diamond += add;
        PlayerPrefs.SetInt(PP_DIAMOND, diamond);
        diamondAnimRect.gameObject.SetActive(true);
        diamondAnimRect.position = screenPos;
        LeanTween.scale(diamondAnimRect, Vector2.one * 1.5f, .6f).setEasePunch();
        LeanTween.value(diamondAnimRect.gameObject, diamondAnimRect.anchoredPosition, diamondOnScoreTable.anchoredPosition, .6f).setEaseInOutBack()
            .setOnUpdateVector2(value =>
            {
                diamondAnimRect.anchoredPosition = value;
            })
            .setOnComplete(() =>
            {
                for (int i = 0; i < diamondTexts.Length; i++)
                {
                    diamondTexts[i].text = diamond.ToString();
                }
                diamondAnimRect.gameObject.SetActive(false);
                LeanTween.scale(diamondOnScoreTable, Vector2.one * 1.3f, .5f).setEasePunch();
            });
    }

    public void PerfectShot(int combo)
    {
        if (combo > 1)
        {
            comboGO.SetActive(true);
            comboNum.text = string.Format("{0}x", combo.ToString());
            LeanTween.scale(comboNum.gameObject, Vector2.one * 1.5f, .5f).setEasePunch();
        }
        else
        {
            comboGO.SetActive(false);
        }

        perfectText.SetActive(true);
        LeanTween.scale(perfectText, Vector2.one * 1.5f, .5f).setEasePunch().setOnComplete(() => perfectText.SetActive(false));
    }

    public void GoodShot()
    {
        comboGO.SetActive(false);
        goodText.SetActive(true);
        LeanTween.scale(goodText, Vector2.one * 1.5f, .5f).setEasePunch().setOnComplete(() => goodText.SetActive(false));
    }

    public void MissedShot()
    {
        comboGO.SetActive(false);
        missedText.SetActive(true);
        LeanTween.scale(missedText, Vector2.one * 1.5f, .5f).setEasePunch().setOnComplete(() => missedText.SetActive(false));
    }

    public void EndGame()
    {
        levelWinText.text = string.Format("level {0} passed", currentLevel);
        PlayerPrefs.SetInt(PP_LEVEL, ++currentLevel);


        inGameCanvas.SetActive(false);
        endGameCanvas.SetActive(true);
    }

    public void LoseGame()
    {
        inGameCanvas.SetActive(false);
        loseGameCanvas.SetActive(true);
    }

    public void OnClick_Settings()
    {
        if (settingsOpen)
        {
            LeanTween.moveLocalY(soundRect, 0, .3f);
            settingsOpen = false;
        }
        else
        {
            LeanTween.moveLocalY(soundRect, soundRectUpHeight, .3f);
            settingsOpen = true;
        }
    }

    public void OnClick_Sound()
    {
        if (soundOn)
        {
            soundRedCross.SetActive(true);
            AudioManager.Instance.soundOn = false;
            soundOn = false;
        }
        else
        {
            soundRedCross.SetActive(false);
            AudioManager.Instance.soundOn = true;
            soundOn = true;
        }
        PlayerPrefs.SetInt(PP_SOUND,soundOn ? 1 : 0);
    }

    public void OnClick_StartGameUI()
    {
        touchToPlayPanel.SetActive(false);
        optionsRect.SetActive(false);
        if (settingsOpen)
        {
            soundRect.transform.localPosition = Vector2.zero;
            settingsOpen = false;
        }
    }

    public void LoadingComplete()
    {
        loadingScreen.SetActive(false);
    }

}
