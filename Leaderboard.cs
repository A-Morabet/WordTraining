using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms.Impl;
using Coffee.UIEffects;

public class Leaderboard : MonoBehaviour
{
// Script handles the local app leaderboard, populating default values, book keeping...etc.

    // Blueprint for Leaderboard.

    private GameObject entryContainer;
    private GameObject entryTemplate;
    private RectTransform templateTransform;
    private RectTransform containerTransform;
    public static RectTransform scoreToCenter;
    private List<Transform> highscoreEntryTransformList;
    private List<GameObject> highscoreCurrentTemplateList;
    private List<Transform> currentTransformList;
    private List<HighscoreEntry> highscoreEntryList;

    // Scores from end of session and saved ones.

    Highscores highscores;
    Highscores currentHighscore;

    //  Audio sources.

    AudioSource musicSource;
    AudioSource sfxSource;
    

    // Scene Objects.
    
    SceneLoader sceneLoader;
    TransitionHandler transitionHandler;
    CarryOvers carryOvers;
    ScrollRect scrollArea;
    Animator scoreTextAnimator;
    DropDown dropdown;
    [SerializeField] SeparateBHandler separateBHandler;
    
    
    
    int lastOptionValue; // Last option select in leaderboard.
    private string lastScoreSeen; // Last obtained score on app session.
    
    // Bools.
   
    private bool firstOpen;
    private bool templateListActive;
    private bool isWritten;

    // Values associated to score.

    private string lockedChars;
    private int scoreToSave;
    private string dateToSave;


    // Hidden button for Leaderboard resetting (debug).    
    Button deleteKeysButton;


    // Clips and buttons
    
    [SerializeField] AudioClip elevatorMusic;
    [SerializeField] AudioClip arrowsClick;
    [SerializeField] Button backToMenu;
    [SerializeField] Button restartButton;

    // Elements for initials writing
    
    TextMeshProUGUI currentInitialsText;
    char[] alphabet;
    int currentChar;

    

    void Awake()
    {
        // References for Leaderboard Container and "carryOvers" script.
        
        entryContainer = GameObject.Find("LeaderboardEntryContainer");
        containerTransform = entryContainer.GetComponent<RectTransform>();

        entryTemplate = GameObject.Find("LeaderboardEntryTemplate");
        templateTransform = entryTemplate.GetComponent<RectTransform>();
        carryOvers = FindObjectOfType<CarryOvers>();

        // Determines if hidden button will be enabled or not.

        if (carryOvers.DKeysState() == "Dkeys are UP") {
            GameObject deleteKeysObject;
            deleteKeysObject = GameObject.Find("Delete Keys (Hidden)");
            if (deleteKeysObject != null) { 
            deleteKeysObject.GetComponent<Image>().enabled = true;
            deleteKeysObject.GetComponent<Button>().enabled = true;
            deleteKeysObject.GetComponentInChildren<TextMeshProUGUI>().enabled = true;
        deleteKeysButton = GameObject.Find("Delete Keys (Hidden)").GetComponent<Button>();
        deleteKeysButton.onClick.AddListener(DeleteAllKeys);
            }
            if(deleteKeysObject == null)
            {
                //Debug.Log("deleteKeys was not found");
            }
        }
       
        dropdown = GameObject.Find("Dropdown").GetComponent<DropDown>();
        scrollArea = GameObject.Find("ScrollArea").GetComponent<ScrollRect>();
        musicSource = GameObject.Find("Background Music").GetComponent<AudioSource>();

        // Disables blueprint which is used to make a copy and populate it.
        
        entryTemplate.SetActive(false);

        // Alphabet array used for initial writing.
    
        alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ123456789".ToCharArray();
        lockedChars = "";


        // Commmand used for debug used alongside the hidden button.

        //PlayerPrefs.DeleteKey("FirstOpen");

        if (!PlayerPrefs.HasKey("FirstOpen"))
        {
            //Debug.Log("First Opening, setting bool to true...");
            firstOpen = true;
            PlayerPrefs.SetString("FirstOpen", "");
        }

        // By using PlayerPrefs, it checks if it's the first time the leaderboard is opened, populates it with default values if it is.
        // Retrieves saved database if it's not.

        if (firstOpen == true)
        {
            
            CheckFirstOpen();
            SortAndShow(); // Sorting algorithm for object from higher score to lower.
        }

        if (firstOpen == false)
        {
            dropdown = GameObject.Find("Dropdown").GetComponent<DropDown>();


            //Debug.Log("Not first time opening, opening last score seen: " + PlayerPrefs.GetString("LastScoreSeen"));
            lastScoreSeen = PlayerPrefs.GetString("LastScoreSeen");
            //lastOptionValue = PlayerPrefs.GetString("LastOptionValue");
            if (!carryOvers.GetComingFromLevel())
            {
                lastOptionValue = int.Parse(PlayerPrefs.GetString("LastOptionValue"));

                dropdown.ChangeOptionValue(lastOptionValue);

                //Debug.Log("Score Option Value changed to: " + lastOptionValue);
            }
            if (carryOvers.GetComingFromLevel())
            {
                //Debug.Log("Coming from a level, looking for choice and setting it...");
                StartCoroutine(WaitForReferenceDropDown());

                if (dropdown != null)
                {
                    //Debug.Log("There is a reference to dropdown apparently");
                }
                dropdown.FindLevelOption();

                carryOvers.ResetComingFromLevel();
            }
            
            string jsonString = PlayerPrefs.GetString(lastScoreSeen);
            highscores = JsonUtility.FromJson<Highscores>(jsonString);
            
            //CheckFirstOpen();
            SortAndShow();

        }
     

    }

    private void Start()
    {
        // Establishes Music, Camera and disables "destroyerHandler" script used in app gameplay session.
    
        musicSource.clip = elevatorMusic;
        musicSource.loop = true;
        musicSource.Play();

        sfxSource = GameObject.Find("Main Camera").GetComponent<AudioSource>();

        DestroyerHandler destroyerHandler = FindObjectOfType<DestroyerHandler>();
        if(destroyerHandler != null) { destroyerHandler.DisableTouch(); }
        
    }


    private void CreateHighscoreEntryTransform (HighscoreEntry highscoreEntry, Transform container, List<Transform> transformList, List<GameObject> templateList)
    {

        // Function that populates Score Container from a series of HighScoreEntry objects
        
        float templateHeight = 80f;

        RectTransform entryTransform = Instantiate(templateTransform, container);
        RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
        entryRectTransform.anchoredPosition = new Vector2(-240, -templateHeight * transformList.Count);
        entryTransform.gameObject.SetActive(true);

        int rank = transformList.Count + 1;
        string rankString;
        rankString = rank.ToString() + ".";
        entryTransform.Find("PositionText").GetComponent<TextMeshProUGUI>().text = rankString;

        string name = highscoreEntry.name;
        entryTransform.Find("NameText").GetComponent<TextMeshProUGUI>().text = name;
        
        GameObject letterUpObject = entryTransform.Find("LetterUpButton").gameObject;
        GameObject letterDownObject = entryTransform.Find("LetterDownButton").gameObject;
        GameObject okButtonObject = entryTransform.Find("OkButton").gameObject;
        GameObject initialsTextObject = entryTransform.Find("InitialsText").gameObject;

        // The following if statements determine which score on the Container is the latest one.
        
        if (highscoreEntry.recent == false)
        {
            
            letterUpObject.SetActive(false);
            letterDownObject.SetActive(false);
            okButtonObject.SetActive(false);
            initialsTextObject.SetActive(false);
        }

        if (highscoreEntry.recent == true)
        {
            // If it's the latest then adds initial writing buttons for saving purposes.
            
            scrollArea.enabled = false;
            
            TextMeshProUGUI initialsText = initialsTextObject.GetComponent<TextMeshProUGUI>();
            currentInitialsText = initialsText;
            initialsText.text = "A";

            backToMenu.onClick.AddListener(delegate { carryOvers.ClearChoiceString(); });
            backToMenu.onClick.AddListener(delegate { separateBHandler.ButtonAnimation(backToMenu.gameObject); });
            


            backToMenu.interactable = false;
            if (restartButton != null) { restartButton.interactable = false; }
            
        }

        if (highscoreEntry.name == "___")
        {
            // Sets character spacing for underscores underneath initials.
        
            RectTransform underscores = entryTransform.Find("NameText").GetComponent<RectTransform>();
            underscores.anchoredPosition += new Vector2(0, 10f);
            entryTransform.Find("NameText").GetComponent<TextMeshProUGUI>().characterSpacing = 9;
        }

        int score = highscoreEntry.score;
        entryTransform.Find("ScoreText").GetComponent<TextMeshProUGUI>().text = score.ToString();

        if (score.ToString() == carryOvers.GetFinalScore() && highscoreEntry.recent == true)
        {

            // Automatically scores to most recent score for initial writing.
            
            scoreToCenter = entryTransform.Find("ScoreText").GetComponent<RectTransform>();
            ScrollViewFocusFunctions.FocusOnItem(scrollArea, scoreToCenter);
            //Debug.Log("Centering around added score!");
            scoreTextAnimator = entryTransform.Find("ScoreText").GetComponent<Animator>();
            scoreTextAnimator.Play("ScoreGlowing");
            scoreToSave = score;
            dateToSave = highscoreEntry.date;
            highscoreEntry.recent = false;
        }

        string date = highscoreEntry.date;
        entryTransform.Find("DateText").GetComponent<TextMeshProUGUI>().text = date.ToString();

        transformList.Add(entryTransform);
        templateList.Add(entryTransform.gameObject);

    }

    public void CharDown()
    {
        // Event function for Char Down button.
    
        sfxSource.pitch = 1;
        sfxSource.PlayOneShot(arrowsClick);

        if (currentChar == 34)
        {
            // When reaching end of alphabet it starts from first letter
            
            //Debug.Log("Resetting the whole loop from char down");
            currentChar = 0;
            currentInitialsText.text = lockedChars + alphabet[currentChar].ToString();
            return;
        }

        if (currentChar <= 33)
        {
            currentChar++;
            //Debug.Log("current char is" + currentChar);
            currentInitialsText.text = lockedChars + alphabet[currentChar].ToString();
        }

    }

    public void SetChar()
    {
        // Event function for char set.

        sfxSource.pitch = 1.5f;
        sfxSource.PlayOneShot(arrowsClick);

        if (currentInitialsText.textInfo.characterCount <= 3)
        {
            lockedChars += alphabet[currentChar].ToString();
            //Debug.Log("the saved chars are" + lockedChars);

            if (currentInitialsText.textInfo.characterCount < 3)
            { UpdateInitialsText(); }

            if ( currentInitialsText.textInfo.characterCount == 3)
            {
                // When setting all chars, char buttons are disabled and buttons for either exiting to main menu,
                // or playing another game are activated.
            
                currentInitialsText.text = lockedChars;

                Button letterUp = GameObject.Find("LetterUpButton").GetComponent<Button>();
                Button letterDown = GameObject.Find("LetterDownButton").GetComponent<Button>();
                Button okButton = GameObject.Find("OkButton").GetComponent<Button>();

                letterUp.interactable = false;
                letterDown.interactable = false;
                okButton.interactable = false;

                //Debug.Log("Max count achieved, activating buttons...");

                backToMenu = GameObject.Find("Back To Menu Button").GetComponent<Button>();
                backToMenu.interactable = true;
                
                if (restartButton != null) {
                UIEffect restartGameEffect = restartButton.gameObject.GetComponent<UIEffect>();
                if (carryOvers.GetRemainingCoins() <= 0) { restartButton.interactable = false; restartGameEffect.effectMode = EffectMode.Grayscale; }
                restartButton.onClick.AddListener(delegate { carryOvers.SubstractDiffToChoiceString(); });
                restartButton.onClick.AddListener(delegate { separateBHandler.ButtonAnimation(restartButton.gameObject); });
                transitionHandler = FindObjectOfType<TransitionHandler>();
                restartButton.onClick.AddListener(delegate { transitionHandler.GoToGameFromReview(); });
                restartButton.interactable = true;
                }

                //Debug.Log("Saving and sorting...");

                sfxSource.pitch = 1f;
                SaveRealRecordText();
                OnlySort();
                SaveAfterShowing();
            }
        }
        
    }

    public void CharUp()
    {
        // Event function for Char Up Button.

        sfxSource.pitch = 1;
        sfxSource.PlayOneShot(arrowsClick);

        if (currentChar == 0)
        {
            //Debug.Log("Resetting the whole loop from char up");
            currentChar = 34;
            currentInitialsText.text = lockedChars + alphabet[currentChar].ToString();
            return;
        }

        if (currentChar >= 1)
        {
            //Debug.Log("current char is" + currentChar);
            currentChar--;
            currentInitialsText.text = lockedChars + alphabet[currentChar].ToString();
        }

    }

    private void UpdateInitialsText()
    {
        currentInitialsText.text = lockedChars + alphabet[currentChar].ToString();
    }
    


    private void CheckFirstOpen()
    {
        // Populates default values and chooses default dropdown option.
        
        if (!PlayerPrefs.HasKey("BedroomIntermediateMedium"))
        {
            

            //Debug.Log("There's not BedroomIntermediateMedium, Initializing table with default values...");
            PlayerPrefs.SetString("BedroomIntermediateMedium", "");

            highscoreEntryList = new List<HighscoreEntry>()
            {
                new HighscoreEntry{ score = 110730, name = "CMK", date= "04/03/2024", recent = false  },
                new HighscoreEntry{ score = 21500, name = "JOE", date= "09/10/2023", recent = false  },
                new HighscoreEntry{ score = 19648, name = "DAV", date= "10/11/2023", recent = false  },
                new HighscoreEntry{ score = 34200, name = "MAG", date= "21/11/2023", recent = false  },
                new HighscoreEntry{ score = 88630, name = "MAX", date= "27/12/2023", recent = false  },
                new HighscoreEntry{ score = 15000, name = "LEN", date= "05/03/2024", recent = false  },
                new HighscoreEntry{ score = 32648, name = "RDY", date= "08/03/2024", recent = false  },
                new HighscoreEntry{ score = 6590, name = "LIS", date= "04/09/2023", recent = false  },
                new HighscoreEntry{ score = 10084, name = "RON", date= "14/10/2023", recent = false  },
                new HighscoreEntry{ score = 36543, name = "POE", date= "28/11/2023", recent = false  },
            };
            
            highscores = new Highscores { highscoreEntryList = highscoreEntryList };
            currentHighscore = highscores;
            string json = JsonUtility.ToJson(highscores);
            PlayerPrefs.SetString("BedroomIntermediateMedium", json);
            //Debug.Log(PlayerPrefs.GetString("BedroomIntermediateMedium"));
            lastScoreSeen = "BedroomIntermediateMedium";
            PlayerPrefs.SetString("LastScoreSeen", lastScoreSeen);
            //Debug.Log(PlayerPrefs.GetString("LastScoreSeen"));
            dropdown.ChangeOptionValue(13);
            PlayerPrefs.SetString("LastOptionValue", dropdown.GetOptionValue().ToString());
            //Debug.Log("Option Nº " + PlayerPrefs.GetString("LastOptionValue") + " saved.");
            PlayerPrefs.Save();
                        
        }
    }

    public void CheckDropDownChange(string dropDownOption)
    {
        
        if (PlayerPrefs.HasKey(dropDownOption))
        {
            //Debug.Log("Dropdown option IS populated, getting it now...");
            string jsonString = PlayerPrefs.GetString(dropDownOption);
            highscores = JsonUtility.FromJson<Highscores>(jsonString);
            PlayerPrefs.SetString("LastOptionValue", dropdown.GetOptionValue().ToString());
            //Debug.Log("Option Nº " + PlayerPrefs.GetString("LastOptionValue") + " saved.");
            SortAndShow();
        }

        if (!PlayerPrefs.HasKey(dropDownOption))
        {

            //Debug.Log("Dropdown option not populated, doing it now...");
            PlayerPrefs.SetString(dropDownOption, "");

            if (dropDownOption.Contains("Easy"))
            { 
                highscoreEntryList = new List<HighscoreEntry>()
            {
                new HighscoreEntry{ score = 52000, name = "HOM", date= "03/09/2023"  },
                new HighscoreEntry{ score = 38462, name = "BIL", date= "06/09/2023"  },
                new HighscoreEntry{ score = 16548, name = "POT", date= "15/10/2023"  },
                new HighscoreEntry{ score = 28671, name = "JAY", date= "18/11/2023"  },
                new HighscoreEntry{ score = 5801, name = "MIL", date= "08/09/2023"  },
                new HighscoreEntry{ score = 9623, name = "SOU", date= "20/09/2023"  },
                new HighscoreEntry{ score = 5466, name = "MCL", date= "28/10/2023"  },
                new HighscoreEntry{ score = 42351, name = "PYR", date= "06/12/2023"  },
                new HighscoreEntry{ score = 30571, name = "RIT", date= "12/05/2024"  },
                new HighscoreEntry{ score = 11688, name = "SOP", date= "03/03/2024"  },
            };

            }

            if (dropDownOption.Contains("Medium"))
            {

                highscoreEntryList = new List<HighscoreEntry>()
            {
                new HighscoreEntry{ score = 102320, name = "LOR", date= "08/10/2023"  },
                new HighscoreEntry{ score = 66513, name = "TIM", date= "04/02/2024"  },
                new HighscoreEntry{ score = 45210, name = "FAN", date= "09/12/2023"  },
                new HighscoreEntry{ score = 34200, name = "JRY", date= "15/11/2023"  },
                new HighscoreEntry{ score = 7800, name = "OLG", date= "20/04/2024"  },
                new HighscoreEntry{ score = 15000, name = "SIG", date= "13/11/2023"  },
                new HighscoreEntry{ score = 16300, name = "JMS", date= "15/09/2023"  },
                new HighscoreEntry{ score = 78660, name = "SON", date= "13/01/2024"  },
                new HighscoreEntry{ score = 10084, name = "TOR", date= "05/10/2023"  },
                new HighscoreEntry{ score = 11540, name = "GNT", date= "09/11/2023"  },
            };

            }

            if (dropDownOption.Contains("Hard"))
            {

                highscoreEntryList = new List<HighscoreEntry>()
            {
                new HighscoreEntry{ score = 323180, name = "PHI", date= "01/01/2024"  },
                new HighscoreEntry{ score = 290220, name = "HNS", date= "05/03/2024"  },
                new HighscoreEntry{ score = 186381, name = "DMC", date= "06/06/2026"  },
                new HighscoreEntry{ score = 95000, name = "DRK", date= "04/02/2024"  },
                new HighscoreEntry{ score = 46540, name = "RNG", date= "23/09/2023"  },
                new HighscoreEntry{ score = 32508, name = "LOT", date= "04/11/2023"  },
                new HighscoreEntry{ score = 20310, name = "ORC", date= "02/06/2023"  },
                new HighscoreEntry{ score = 18272, name = "GNM", date= "05/09/2023"  },
                new HighscoreEntry{ score = 16340, name = "SEF", date= "02/04/2023"  },
                new HighscoreEntry{ score = 55870, name = "SEC", date= "05/02/2024"  },
            };

            }
            // Reload
            highscores = new Highscores { highscoreEntryList = highscoreEntryList };
            currentHighscore = highscores;
            string json = JsonUtility.ToJson(highscores);
            PlayerPrefs.SetString(dropDownOption, json);
            //Debug.Log(PlayerPrefs.GetString(dropDownOption));
            lastScoreSeen = dropDownOption;
            PlayerPrefs.SetString("LastScoreSeen", dropDownOption);
            PlayerPrefs.SetString("LastOptionValue", dropdown.GetOptionValue().ToString());
            //Debug.Log("Option Nº " + PlayerPrefs.GetString("LastOptionValue") + " saved.");
            PlayerPrefs.Save();
            SortAndShow();
        }
        
    }

    private void SortAndShow()
    {
        // Function that sorts list by highest score, and proceeds to update Container interface.

        for (int i = 0; i < highscores.highscoreEntryList.Count; i++)
        {
            for (int j = i + 1; j < highscores.highscoreEntryList.Count; j++)
            {
                if (highscores.highscoreEntryList[j].score > highscores.highscoreEntryList[i].score)
                {
                    //swap
                    HighscoreEntry tmp = highscores.highscoreEntryList[i];
                    highscores.highscoreEntryList[i] = highscores.highscoreEntryList[j];
                    highscores.highscoreEntryList[j] = tmp;
                }
            }
        }

        highscoreEntryTransformList = new List<Transform>();

        if (templateListActive)
        {
            for (int i = 0; i < highscoreCurrentTemplateList.Count; i++)
            {
                Destroy(highscoreCurrentTemplateList[i].gameObject);
            }
            templateListActive = false;
        }

        if (!templateListActive)
        {
            highscoreCurrentTemplateList = new List<GameObject>();
            templateListActive = true;
        }

        foreach (HighscoreEntry highscoreEntry in highscores.highscoreEntryList)
        {
            CreateHighscoreEntryTransform(highscoreEntry, containerTransform, highscoreEntryTransformList, highscoreCurrentTemplateList);

        }

        SaveAfterShowing();
    }

    private void OnlySort()
    {
        for (int i = 0; i < highscores.highscoreEntryList.Count; i++)
        {
            for (int j = i + 1; j < highscores.highscoreEntryList.Count; j++)
            {
                if (highscores.highscoreEntryList[j].score > highscores.highscoreEntryList[i].score)
                {
                    //swap
                    HighscoreEntry tmp = highscores.highscoreEntryList[i];
                    highscores.highscoreEntryList[i] = highscores.highscoreEntryList[j];
                    highscores.highscoreEntryList[j] = tmp;
                }
            }
        }

        
    }

    private IEnumerator WaitForReferenceDropDown()
    {
        yield return new WaitUntil(() => dropdown != null);
    }

    private void SaveAfterShowing()
    {
        string json = JsonUtility.ToJson(highscores);
        PlayerPrefs.SetString(carryOvers.GetChoiceString(), json);
        //Debug.Log(PlayerPrefs.GetString(carryOvers.GetChoiceString()));
        PlayerPrefs.SetString("LastOptionValue", dropdown.GetOptionValue().ToString());
        //Debug.Log("Option Nº " + PlayerPrefs.GetString("LastOptionValue") + " saved.");
        PlayerPrefs.Save();

        
    }


    private void AddHighscoreEntryDefault(int score, string name, string date)
    {
        // Instances entry, then proceeds to load entire highscore to add the entry to it and finally save.
        HighscoreEntry highscoreEntry = new HighscoreEntry { score = score, name = name, date = date };

        
        string jsonString = PlayerPrefs.GetString("BedroomIntermediate");
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);

        
        highscores.highscoreEntryList.Add(highscoreEntry);

       
        string json = JsonUtility.ToJson(highscores);
        PlayerPrefs.SetString("BedroomIntermediate", json);
        PlayerPrefs.Save();
    }

    private void AddHighscoreEntry(int score, string name, string date)
    {
        
        HighscoreEntry highscoreEntry = new HighscoreEntry { score = score, name = name, date = date};

        string jsonString = PlayerPrefs.GetString("highscoretable");
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);
       
        highscores.highscoreEntryList.Add(highscoreEntry);

        string json = JsonUtility.ToJson(highscores);
        PlayerPrefs.SetString("highscoreTable", json);
        PlayerPrefs.Save();
    }

    private class Highscores
    {
        // Class that holds a list of classes/objects.
    
        public List<HighscoreEntry> highscoreEntryList;
    }

    [System.Serializable]
    private class HighscoreEntry
    {
        // Class/object holding one entry.
        
        public int score;
        public string name;
        public string date;
        public bool recent;
    }

    public bool GetFirstOpen()
    {
        return firstOpen;
    }

    

    public void DeleteAllKeys()
    { 
    // Button that resets all Leaderboard values.
    
        PlayerPrefs.DeleteKey("FirstOpen");
        PlayerPrefs.DeleteKey("LastScoreSeen");
        PlayerPrefs.DeleteKey("LastOptionValue");
        PlayerPrefs.DeleteKey("BedroomIntermediateMedium");
        PlayerPrefs.DeleteKey("coinsRemaining");
        PlayerPrefs.DeleteKey("activeLevelDifficulty");

        dropdown.RemoveListener();
        for (int i = 0; i < dropdown.GetDropDownCount(); i++)
        {
            //Debug.Log("Deleting Option Nº " + i +"...");
            PlayerPrefs.DeleteKey(dropdown.GiveOptionName(i));
        }
        dropdown.RestoreListener();
    }


    private void SaveRealRecordText()
    {
        // Function that saves record entry replacing the dummy one.
        
        for (int i = 0; i < highscores.highscoreEntryList.Count; i++)
        {
            if (highscores.highscoreEntryList[i].name == "___")
            {
                //Debug.Log("Found the blank entry, deleting it...");
                highscores.highscoreEntryList.RemoveAt(i);
                //Debug.Log("Blank entry deleted, adding new real entry...");
                highscores.highscoreEntryList.Add(new HighscoreEntry { score = scoreToSave, name = lockedChars.ToUpper(), date = System.DateTime.Now.ToString("dd/MM/yyyy"), recent = false });
            }

        }

    }

   
}


