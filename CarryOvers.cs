using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CarryOvers : MonoBehaviour
{
   
// Script modifies and stores all dynamic variables in the App. 

    // Menu choices.

    private string choiceString = "";
    private string backgroundString = "";
    private string themeString = "";
    private string diffString = "";
    private string choiceStringSans = "";
    private int currentScene;
    
    private string activeDifficulty;
    private string storedTextDiff = "";
    private string[] splitChoiceCheck;

    // Miscellaneous0
    
    private string finalScore;
    bool comingFromLevel;

    // Audio Sources.

    AudioSource musicSource;
    AudioSource soundSource;

    // Options.

    Options options;
    private Toggle musicToggle;
    private Slider musicSlider;
    private Toggle soundToggle;  
    private Slider soundSlider;
    private Toggle leftHandedToggle;
    private Toggle rightHandedToggle;
    private bool enteringOptions;
    private bool enableDKeys = false;

    // DestroyerHandler variables.

    DestroyerHandler destroyerHandler;
    private string currentHitTag;
    private string currentLetter;
    private bool rightHanded;

    // Current game coins.

    SwitchCoins switchCoins;
    private int currentCoins;

    AdsScript adsscript;

    private void Awake()
    {
        // Prevents duplicate.
    
        GameObject[] objs = GameObject.FindGameObjectsWithTag("CarryOvers");

        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);

        
        destroyerHandler = FindObjectOfType<DestroyerHandler>();
        destroyerHandler.DisableTouch();



    }


    void Start()
    {
        
        LoadSettingsAtStart();
        
    }

    
    void Update()
    {
        
    }

    // Scene functions for index setting.


    public void ResetCurrentScene()

    {
        currentScene = 0;
    }

    public void SetGameScene()

    {
        currentScene = 4;
    }

    public void SetOptionsScene()

    {
        currentScene = 6;
    }

    public void SetGameCoinsScene()

    {
        currentScene = 7;
    }

    public void SetLeaderboardScene()
    {
        currentScene = 8;
    }

    public void SetLeaderboard2Scene()
    {
        currentScene = 9;
    }

    public void SetGameScreenScene()
    {
        currentScene = 10;
    }

    public void SetCreditsScene()
    {
        currentScene = 11;
    }

    public int GetCurrentScene()
    {
        return currentScene;
    }

    // Functions for getting or modifying menu choices.

    public string GetTheme()
    {
        return themeString;
    }

    public string GetDiff()
    {
        return diffString;
    }

    public void BackErase()
    {
        backgroundString = "";
        choiceString = "";
        themeString = "";
        diffString = "";
        currentScene--;
    }

    // Add theme and difficulty.

    public void AddBathroom()
    {
        backgroundString = "Bathroom";
        choiceString += "Bathroom";
        themeString = "Bathroom";
        currentScene = 2;
       
    }

    public void AddBedroom()
    {
        backgroundString = "Bedroom";
        choiceString += "Bedroom";
        themeString = "Bedroom";
        currentScene = 2;
        
    }

    public void AddKitchen()
    {
        backgroundString = "Kitchen";
        choiceString += "Kitchen";
        themeString = "Kitchen";
        currentScene = 2;
        
    }

    public void AddBeginner()
    {
        choiceString += "Beginner";
        diffString = "Beginner";
        currentScene = 3;
        choiceStringSans = choiceString;
    }

    public void AddIntermediate()
    {
        choiceString += "Intermediate";
        diffString = "Intermediate";
        currentScene = 3;
        choiceStringSans = choiceString;
    }

    public void AddAdvanced()
    {
        choiceString += "Advanced";
        diffString = "Advanced";
        currentScene = 3;
        
        choiceStringSans = choiceString;
    }

    public void BackEraseReview()
    {
        backgroundString = "";
        choiceString = "";
        diffString = "";
        choiceString += themeString;
        backgroundString += themeString;
        currentScene = 2;
    }

    public void SetSceneOne()
    {
        currentScene = 1;
    }

    public string GetChoiceString()
    {
        return choiceString;
        
    }

    public void ClearChoiceString()
    {
        backgroundString = "";
        choiceString ="";
    }

    public void ChangeDiffEasy()
    {
        activeDifficulty = "bgrLevel.LevelBeginner()";
        storedTextDiff = "Easy";
    }

    public void ChangeDiffMedium()
    {
        activeDifficulty = "imtLevel.LevelIntermediate()";
        storedTextDiff = "Medium";
    }

    public void ChangeDiffHard()
    {
        activeDifficulty = "advLevel.LevelAdvanced()";
        storedTextDiff = "Hard";
    }

    public string GetActiveDifficulty()
    {
        return activeDifficulty;
    }

    // Manages sound variables.

    public void SetOptionsReferences()
    {

        options = FindObjectOfType<Options>();
        musicSource = GameObject.Find("Background Music").GetComponent<AudioSource>();
        soundSource = GameObject.Find("Main Camera").GetComponent<AudioSource>();
        musicToggle = GameObject.Find("Music Toggle").GetComponent<Toggle>();
        musicSlider = GameObject.Find("Music Slider").GetComponent<Slider>();
        musicToggle.onValueChanged.AddListener(delegate { MusicTogglePress(); });
        musicSlider.onValueChanged.AddListener(delegate { ChangeMusicVolume(); });
        soundToggle = GameObject.Find("Sound Toggle").GetComponent<Toggle>();
        soundSlider = GameObject.Find("Sound Slider").GetComponent<Slider>();
        soundToggle.onValueChanged.AddListener(delegate { SoundTogglePress(); });
        soundSlider.onValueChanged.AddListener(delegate { ChangeSoundVolume(); });
        soundSource.clip = destroyerHandler.GetTestSound();
        leftHandedToggle = GameObject.Find("Left-Handed Toggle").GetComponent<Toggle>();
        rightHandedToggle = GameObject.Find("Right-Handed Toggle").GetComponent<Toggle>();
        StartCoroutine(SetHDListeners());

        //Debug.Log("The Music Name is: " + musicSource.clip.name);
        
    }

    private IEnumerator SetHDListeners()
    {
        yield return new WaitForSeconds(0.2f);
        leftHandedToggle.onValueChanged.AddListener(delegate { SetLeftHanded(); });
        rightHandedToggle.onValueChanged.AddListener(delegate { SetRightHanded(); });
    }

    public void MusicTogglePress()
    {
        //Debug.Log("The Music Name is: " + musicSource.clip.name);

        if (musicToggle.isOn == false)
        {
            musicSource.Pause(); 

        }

        if (musicToggle.isOn == true)
        {
            musicSource.UnPause();   

        }
    }

    public void SoundTogglePress()
    {
        //Debug.Log("The Sound Name is: " + soundSource.clip.name);

        if (soundToggle.isOn == false)
        {
            soundSource.Pause();
            
        }

        if (soundToggle.isOn == true)
        {
            soundSource.UnPause();
            soundSource.Play();

        }
    }

    public void ChangeMusicVolume()
    {
        musicSource.volume = musicSlider.value;
    }

    public void ChangeSoundVolume()
    {
        soundSource.volume = soundSlider.value;


        if(!soundSource.isPlaying && soundToggle.isOn == true && enteringOptions == false)
        {
            soundSource.Play();
        }
        
    }

    // Stores variable that changes app session based on user's dominant hand (Accessibility option).

    public void SetRightHanded()
    {
        if (rightHandedToggle.isOn == false)
        {
            rightHandedToggle.SetIsOnWithoutNotify(true);
            leftHandedToggle.SetIsOnWithoutNotify(false);
            rightHanded = true;
            //Debug.Log("Right-handed mode activated");
        }
        if (rightHandedToggle.isOn == true)
        {
            rightHandedToggle.SetIsOnWithoutNotify(true);
            leftHandedToggle.SetIsOnWithoutNotify(false);
            rightHanded = true;
            //Debug.Log("Right-handed mode activated");
        }

    }

    public void SetLeftHanded()
    {
        if (leftHandedToggle.isOn == false)
        {
            leftHandedToggle.SetIsOnWithoutNotify(true);
            rightHandedToggle.SetIsOnWithoutNotify(false);
            rightHanded = false;
            //Debug.Log("Left-handed mode activated");
        }
        if (leftHandedToggle.isOn == true)
        {
            leftHandedToggle.SetIsOnWithoutNotify(true);
            rightHandedToggle.SetIsOnWithoutNotify(false);
            rightHanded = false;
            //Debug.Log("Left-handed mode activated");
        }

    }


    // Section that loads and saves settings.

    public void LoadSettingsAtStart()
    {
        musicSource = GameObject.Find("Background Music").GetComponent<AudioSource>();
        soundSource = GameObject.Find("Main Camera").GetComponent<AudioSource>();

        

        if (!PlayerPrefs.HasKey("coinsRemaining"))
        {
            int defaultCoins = 3;
            PlayerPrefs.SetInt("coinsRemaining", defaultCoins);
            currentCoins = defaultCoins;
            //Debug.Log("Default coins set to " + currentCoins);

        }

        if (!PlayerPrefs.HasKey("musicmute"))
        {
            PlayerPrefs.SetInt("musicmute", 0);
        }

        if (!PlayerPrefs.HasKey("soundmute"))
        {
            PlayerPrefs.SetInt("soundmute", 0);
        }

        int storedMusicMute = PlayerPrefs.GetInt("musicmute");
        if (storedMusicMute != 0)
        {
            musicSource.Pause();
        }

        if (PlayerPrefs.GetInt("soundmute") != 0)
        {
            soundSource.Pause();
        }

        if (!PlayerPrefs.HasKey("musicVolume"))
        {
            PlayerPrefs.SetFloat("musicVolume", 0.8f);
        }

        if (!PlayerPrefs.HasKey("soundVolume"))
        {
            PlayerPrefs.SetFloat("soundVolume", 0.5f);
        }

        
        musicSource.volume = PlayerPrefs.GetFloat("musicVolume");
        soundSource.volume = PlayerPrefs.GetFloat("soundVolume");


        if (!PlayerPrefs.HasKey("activeLevelDifficulty"))
        {
            PlayerPrefs.SetString("activeLevelDifficulty", "bgrLevel.LevelBeginner()");
            PlayerPrefs.SetString("activeTextDifficulty", "Easy");
        }


        if (PlayerPrefs.GetString("activeLevelDifficulty") == "bgrLevel.LevelBeginner()")
        {
            ChangeDiffEasy();
            PlayerPrefs.SetString("activeTextDifficulty", "Easy");
            //Debug.Log("Difficulty changed to Easy");
        }

        if (PlayerPrefs.GetString("activeLevelDifficulty") == "imtLevel.LevelIntermediate()")
        {
            ChangeDiffMedium();
            PlayerPrefs.SetString("activeTextDifficulty", "Medium");
            //Debug.Log("Difficulty changed to Medium");
        }

        if (PlayerPrefs.GetString("activeLevelDifficulty") == "advLevel.LevelAdvanced()")
        {
            ChangeDiffHard();
            PlayerPrefs.SetString("activeTextDifficulty", "Hard");
            //Debug.Log("Difficulty changed to Hard");
        }

        //Debug.Log("the config level is " + PlayerPrefs.GetString("activeLevelDifficulty"));
        storedTextDiff = PlayerPrefs.GetString("activeTextDifficulty");
        

        if (!PlayerPrefs.HasKey("rightHanded"))
        {
            PlayerPrefs.SetInt("rightHanded", 1);
            rightHanded = true;
            
        }

        if (PlayerPrefs.GetInt("rightHanded") == 1)
        {
            rightHanded = true;
        }
        else
        {
            rightHanded = false;
        }

        //Debug.Log("Right-handed is " + PlayerPrefs.GetInt("rightHanded") + " and bool is " + rightHanded);
    }

    public void LoadSettingsAtOptions()
    {
       // Each time options menu is opened, last values from variables are loaded.

        if (!PlayerPrefs.HasKey("activeTextDifficulty"))
        {
            PlayerPrefs.SetString("activeTextDifficulty", "Easy");
        }

        storedTextDiff = PlayerPrefs.GetString("activeTextDifficulty");
        //Debug.Log("the difficulty text is " + storedTextDiff);
        options.SetDiffText(storedTextDiff);


        if (storedTextDiff == "Easy")
        {
            ChangeDiffEasy();
        }

        if (storedTextDiff == "Medium")
        {
            ChangeDiffMedium();
        }

        if (storedTextDiff == "Hard")
        {
            ChangeDiffHard();
        }

        enteringOptions = true;
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        soundSlider.value = PlayerPrefs.GetFloat("soundVolume");
        

        if (PlayerPrefs.GetInt("musicmute") == 0)
        {
            musicToggle.isOn = true;
        }
        else
        {
            musicToggle.isOn = false;
        }
        if (PlayerPrefs.GetInt("soundmute") == 0)
        {
            soundToggle.isOn = true;
            soundSource.mute = false;
        }
        else
        {
            soundToggle.isOn = false;
            
        }

        if (PlayerPrefs.GetInt("rightHanded") == 1)
        {
            rightHandedToggle.isOn = true;
            leftHandedToggle.isOn = false;
        }
        else
        {
            rightHandedToggle.isOn = false;
            leftHandedToggle.isOn = true;
        }

        enteringOptions = false ;

    }  

    public void SaveSettings()
    {
        PlayerPrefs.SetString("activeLevelDifficulty", activeDifficulty);
        PlayerPrefs.SetString("activeTextDifficulty", storedTextDiff);
        PlayerPrefs.SetFloat("musicVolume", musicSlider.value);
        PlayerPrefs.SetFloat("soundVolume", soundSlider.value);
        if (musicToggle.isOn == false)
        {
            PlayerPrefs.SetInt("musicmute", 1);
        }
        else
        {
            PlayerPrefs.SetInt("musicmute", 0);
        }
        if (soundToggle.isOn == false)
        {
            PlayerPrefs.SetInt("soundmute", 1);
        }
        else
        {
            PlayerPrefs.SetInt("soundmute", 0);
        }
        soundSource.clip = null;

        if (leftHandedToggle.isOn == true)
        {
            PlayerPrefs.SetInt("rightHanded", 0);
            //Debug.Log("Right-handed is " + PlayerPrefs.GetInt("rightHanded") + " and bool is " + rightHanded);
        }

        if (rightHandedToggle.isOn == true)
        {
            PlayerPrefs.SetInt("rightHanded", 1);
            //Debug.Log("Right-handed is " + PlayerPrefs.GetInt("rightHanded") + " and bool is " + rightHanded);
        }
    }

    public void SaveSettingsFromGame()
    {
        PlayerPrefs.SetString("activeLevelDifficulty", activeDifficulty);
        PlayerPrefs.SetString("activeTextDifficulty", storedTextDiff);
        PlayerPrefs.SetFloat("musicVolume", musicSlider.value);
        PlayerPrefs.SetFloat("soundVolume", soundSlider.value);
        if (musicToggle.isOn == false)
        {
            PlayerPrefs.SetInt("musicmute", 1);
        }
        else
        {
            PlayerPrefs.SetInt("musicmute", 0);
        }
        if (soundToggle.isOn == false)
        {
            PlayerPrefs.SetInt("soundmute", 1);
        }
        else
        {
            PlayerPrefs.SetInt("soundmute", 0);
        }
        soundSource.clip = null;

        if (leftHandedToggle.isOn == true)
        {
            PlayerPrefs.SetInt("rightHanded", 0);
            //Debug.Log("Right-handed is " + PlayerPrefs.GetInt("rightHanded") + " and bool is " + rightHanded);
        }

        if (rightHandedToggle.isOn == true)
        {
            PlayerPrefs.SetInt("rightHanded", 1);
            //Debug.Log("Right-handed is " + PlayerPrefs.GetInt("rightHanded") + " and bool is " + rightHanded);
        }
    }

    // Section handles settings loading and saving during app session.

    public void SetGameOptionsReferences()
    {
        musicSource = GameObject.Find("Background Music").GetComponent<AudioSource>();
        soundSource = GameObject.Find("Main Camera").GetComponent<AudioSource>();
        musicToggle = GameObject.Find("Music Toggle").GetComponent<Toggle>();
        musicSlider = GameObject.Find("Music Slider").GetComponent<Slider>();
        musicToggle.onValueChanged.AddListener(delegate { MusicTogglePress(); });
        musicSlider.onValueChanged.AddListener(delegate { ChangeMusicVolume(); });
        soundToggle = GameObject.Find("Sound Toggle").GetComponent<Toggle>();
        soundSlider = GameObject.Find("Sound Slider").GetComponent<Slider>();
        soundToggle.onValueChanged.AddListener(delegate { SoundTogglePress(); });
        soundSlider.onValueChanged.AddListener(delegate { ChangeSoundVolume(); });
        soundSource.clip = destroyerHandler.GetTestSound();

        //Debug.Log("The Music Name is: " + musicSource.clip.name);

    }

    public void LoadSettingsAtGameOptions()
    {

        if (PlayerPrefs.GetInt("musicmute") == 0)
        {
            musicToggle.isOn = true;
        }
        else
        {
            musicToggle.isOn = false;
        }
        if (PlayerPrefs.GetInt("soundmute") == 0)
        {
            soundToggle.isOn = true;
        }
        else
        {
            soundToggle.isOn = false;
        }

        musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        soundSlider.value = PlayerPrefs.GetFloat("soundVolume");

    }


    public void SaveSettingsAtGameOptions()
    {
        PlayerPrefs.SetFloat("musicVolume", musicSlider.value);
        PlayerPrefs.SetFloat("soundVolume", soundSlider.value);
        if (musicToggle.isOn == false)
        {
            PlayerPrefs.SetInt("musicmute", 1);
        }
        else
        {
            PlayerPrefs.SetInt("musicmute", 0);
        }
        if (soundToggle.isOn == false)
        {
            PlayerPrefs.SetInt("soundmute", 1);
        }
        else
        {
            PlayerPrefs.SetInt("soundmute", 0);
        }

        soundSource.clip = null;

    }

    public void SaveCoinsOnGet()
    {
        // Coin amount is saved on obtaining through minigame.
        
        switchCoins = FindObjectOfType<SwitchCoins>();

        if (switchCoins.GetCoinsInstantiated())
        {
            PlayerPrefs.SetInt("coinsRemaining", switchCoins.GetCoinsReward());
            //Debug.Log("Amount of coins stored is " + switchCoins.GetCoinsReward() + "coin/s");
            currentCoins = PlayerPrefs.GetInt("coinsRemaining");
        }
       

    }

    public void SubstractCoin()
    {
        currentCoins = PlayerPrefs.GetInt("coinsRemaining");
        currentCoins--;
        PlayerPrefs.SetInt("coinsRemaining", currentCoins);
    }

    public int passItOverCoins()
    {
        currentCoins = PlayerPrefs.GetInt("coinsRemaining");
        return currentCoins;
    }

    public bool GetRightHanded()
    {
        return rightHanded;
    }

    public void AddDiffToChoiceString()
    {
        choiceString += PlayerPrefs.GetString("activeTextDifficulty");
        //Debug.Log("The current entire difficulty is " + choiceString);

    }

    public void SubstractDiffToChoiceString()
    {
        choiceString = choiceStringSans;
        //Debug.Log("The current entire difficulty is " + choiceString);
    }

    public void SetSplitChoiceCheck(string[] splitChoice)
    {
        splitChoiceCheck = splitChoice;
        //Debug.Log("splitChoice has been passed to CarryOvers: " + splitChoiceCheck[0] + " " + splitChoiceCheck[1] + " " + splitChoiceCheck[2]);
        PlayerPrefs.SetString("CurrentSplitChoice0", splitChoiceCheck[0]);
        PlayerPrefs.SetString("CurrentSplitChoice1", splitChoiceCheck[1]);
        PlayerPrefs.SetString("CurrentSplitChoice2", splitChoiceCheck[2]);
    }

    public bool GetComingFromLevel()
    {
        return comingFromLevel;
    }

    public void SetComingFromLevel()
    {
        // Determines if last scene was the app session.
        
        comingFromLevel = true;
    }

    public void ResetComingFromLevel()
    {
        comingFromLevel = false;
    }

    public string[] GetSplitChoiceCheck()
    {
        return splitChoiceCheck;
    }

    public void SetFinalScore(string scoreFromLevel)
    {
        finalScore = scoreFromLevel;
        //Debug.Log("Level final score is " + finalScore);
    }

    public string GetFinalScore()
    {
        return finalScore;
    }

    // This section involves getting information from "destroyerHandler" script.
    
    public void TransferTagDH()
    {
        currentHitTag = destroyerHandler.GetDestroyedTag();
    }

    public string GetTagDH()
    {
        ////Debug.Log("the destroyed tag is " + currentHitTag);
        return currentHitTag;
    }

    public void TransferLetterDH()
    {
        currentLetter = destroyerHandler.GetDestroyedLetter();
    }

    public string GetLetterDH()
    {
        //Debug.Log("the destroyed letter is " + currentLetter);
        return currentLetter;
    }

    public string GetBackgroundString()
    {
        return backgroundString;
    }

    public int GetRemainingCoins()
    {
        return PlayerPrefs.GetInt("coinsRemaining");
    }

    // Hidden button section for debugging.

    public void DKeysInteract()
    {
        if (enableDKeys == false) { enableDKeys = true;  return; };
        if (enableDKeys == true) { enableDKeys = false;  return; };
       

    }
    public string DKeysState()
    {
        if (enableDKeys == false)
        {
            return "Dkeys are DOWN";
        }

        if (enableDKeys == true)
        {
            return "Dkeys are UP";
        }
        return null;
    }
}
