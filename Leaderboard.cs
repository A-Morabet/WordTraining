using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms.Impl;
using Coffee.UIEffects;

public class Leaderboard : MonoBehaviour
{
    private GameObject entryContainer;
    private GameObject entryTemplate;
    private RectTransform templateTransform;
    private RectTransform containerTransform;
    public static RectTransform scoreToCenter;
    private List<Transform> highscoreEntryTransformList;
    private List<GameObject> highscoreCurrentTemplateList;
    private List<Transform> currentTransformList;
    private List<HighscoreEntry> highscoreEntryList;

    

    Highscores highscores;
    Highscores currentHighscore;
    CarryOvers carryOvers;
    AudioSource musicSource;
    AudioSource sfxSource;
    ScrollRect scrollArea;
    Animator scoreTextAnimator;
    SceneLoader sceneLoader;
    TransitionHandler transitionHandler;
    [SerializeField] SeparateBHandler separateBHandler;
    [SerializeField] AudioClip elevatorMusic;
    
    
    
    private string lastScoreSeen;
    private bool firstOpen;
    private bool templateListActive;
    
    private bool isWritten;
    //private string inputScore;

    private string lockedChars;
    //[SerializeField]  Button upButton;
    //[SerializeField]  Button downButton;
    //[SerializeField]  Button okButton;
    //[SerializeField] TextMeshProUGUI initialsText;

    private int scoreToSave;
    private string dateToSave;

    DropDown dropdown;
    int lastOptionValue;
    Button deleteKeysButton;

    [SerializeField] Button backToMenu;
    [SerializeField] Button restartButton;
    [SerializeField] AudioClip arrowsClick;
    

    TextMeshProUGUI currentInitialsText;
    char[] alphabet;
    int currentChar;


    // Start is called before the first frame update
    void Awake()
    {
        entryContainer = GameObject.Find("LeaderboardEntryContainer");
        containerTransform = entryContainer.GetComponent<RectTransform>();

        entryTemplate = GameObject.Find("LeaderboardEntryTemplate");
        templateTransform = entryTemplate.GetComponent<RectTransform>();
        carryOvers = FindObjectOfType<CarryOvers>();



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

        carryOvers = FindObjectOfType<CarryOvers>();
        dropdown = GameObject.Find("Dropdown").GetComponent<DropDown>();
        scrollArea = GameObject.Find("ScrollArea").GetComponent<ScrollRect>();
        musicSource = GameObject.Find("Background Music").GetComponent<AudioSource>();

        



        entryTemplate.SetActive(false);

        alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ123456789".ToCharArray();
        lockedChars = "";


        // Comando para forzar default

        //PlayerPrefs.DeleteKey("FirstOpen");

        if (!PlayerPrefs.HasKey("FirstOpen"))
        {
            //Debug.Log("First Opening, setting bool to true...");
            firstOpen = true;
            PlayerPrefs.SetString("FirstOpen", "");
        }

        // LO QUE HAY QUE HACER:
        // A) CUANDO SE ENCIENDA ESTE SCRIPT VA A COMPROBAR EL ULTIMO STRING VISTO EN DROPDOWN,
        // HACER UN CHEQUEO DE SI EL ULTIMO STRING VISTO EXISTE, SI NO, POBLARLO
        // LOADEAR HIGHSCORE BASADO EN ULTIMO STRING VISTO
        // QUE APAREZCA OPCIÓN ULTIMO STRING VISTO SELECCIONADA EN DROPDOWN
        // B) SI EL ÚLTIMO STRING VISTO ES NULL, AÑADIR STRING HIGHSCOREDEFAULT LIVINGROOM INTERMEDIATE
        // HACER UN CHEQUEO DE SI EL LIVINGROOM INTERMEDIATE EXISTE, SI NO, POBLARLO
        // LOADEAR HIGHSCORE BASADO EN DEFAULT
        // QUE APAREZCA OPCIÓN LIVINGROOM INTERMEDIATE SELECCIONADA EN DROPDOWN
        // C) SI CAMBIO OPCIÓN EN DROPDOWN A OTRA,
        // HACER UN CHEQUEO DE SI EL STRING SELECCIONADO EXISTE, SI NO, POBLARLO
        // LOADEAR HIGHSCORE BASADO EN STRING SELECCIONADO
        // QUE APAREZCA OPCIÓN ULTIMO STRING VISTO SELECCIONADA EN DROPDOWN

        // ASOCIAR EL DICCIONARIO Y DIFICULTAD ESCOGIDOS AL HIGHSCORE CORRESPONDIENTE
        // QUE AL FINAL DE LA PARTIDA COMPRUEBE SI LA PUNTUACIÓN ES MAYOR QUE CUALQUIERA DE LOS SCORES EN EL HIGHSCORE ASOCIADO
        // SI LA PUNTUACIÓN ES MENOR SALE MENU COMO PAUSA CON EL SCORE OBTENIDO CON LOS BOTONES VOLVER AL MENU O RESTART
        // SI ES EL CASO EN LA VENTANA DE FINAL DE PARTIDA DONDE ENSEÑE EL SCORE OBTENIDO MOSTRAR NEW RECORD
        // LLEVARTE A ESCENA COPIA DE LEADERBOARD EN LA QUE TE MUESTRA DONDE TE HAN ASIGNADO Y SALE TECLADO PARA PONER TUS INICIALES
        // ELEGIR ABAJO Y VOLVER AL MENU O RESTART.

        
        // Comando para forzar por defecto

        
        //PlayerPrefs.DeleteKey("LastScoreSeen");

        if (firstOpen == true)
        {
            // Comando para forzar por defecto
            //PlayerPrefs.DeleteKey("BedroomIntermediateMedium");
            // Command json get list
            
            //string jsonString = PlayerPrefs.GetString("BedroomIntermediateMedium");
            //Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);
            CheckFirstOpen();
            SortAndShow();
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
        musicSource.clip = elevatorMusic;
        musicSource.loop = true;
        musicSource.Play();

        sfxSource = GameObject.Find("Main Camera").GetComponent<AudioSource>();

        DestroyerHandler destroyerHandler = FindObjectOfType<DestroyerHandler>();
        if(destroyerHandler != null) { destroyerHandler.DisableTouch(); }
        
    }


    private void CreateHighscoreEntryTransform (HighscoreEntry highscoreEntry, Transform container, List<Transform> transformList, List<GameObject> templateList)
    {
  
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
        //GameObject inputObject = entryTransform.Find("InputFieldScore").gameObject;
        //
        GameObject letterUpObject = entryTransform.Find("LetterUpButton").gameObject;
        GameObject letterDownObject = entryTransform.Find("LetterDownButton").gameObject;
        GameObject okButtonObject = entryTransform.Find("OkButton").gameObject;
        GameObject initialsTextObject = entryTransform.Find("InitialsText").gameObject;


        if (highscoreEntry.recent == false)
        {
            
            //Debug.Log("this entry is not the record, disabling input field. Recent is " + highscoreEntry.recent);
            //inputObject.SetActive(false);
            //
            letterUpObject.SetActive(false);
            letterDownObject.SetActive(false);
            okButtonObject.SetActive(false);
            initialsTextObject.SetActive(false);
        }

        if (highscoreEntry.recent == true)
        {
            //StartCoroutine(SelectNewHighScoreInput());
            //TMP_InputField inputObjectScore = entryTransform.Find("InputFieldScore").GetComponent<TMP_InputField>();
            scrollArea.enabled = false;
            //inputObjectScore.ActivateInputField();
            //inputObjectScore.Select();
            //inputObjectScore.contentType = TMP_InputField.ContentType.Name;
            //Debug.Log("this entry is the record, enabling input field. Recent is " + highscoreEntry.recent);
            //
            
            TextMeshProUGUI initialsText = initialsTextObject.GetComponent<TextMeshProUGUI>();
            currentInitialsText = initialsText;
            initialsText.text = "A";




            //backToMenu = GameObject.Find("Back To Menu Button").GetComponent<Button>();
            //restart = GameObject.Find("Restart Button").GetComponent<Button>();


            backToMenu.onClick.AddListener(delegate { carryOvers.ClearChoiceString(); });
            backToMenu.onClick.AddListener(delegate { separateBHandler.ButtonAnimation(backToMenu.gameObject); });
            


            backToMenu.interactable = false;
            if (restartButton != null) { restartButton.interactable = false; }
            


        }

        if (highscoreEntry.name == "___")
        {
            RectTransform underscores = entryTransform.Find("NameText").GetComponent<RectTransform>();
            underscores.anchoredPosition += new Vector2(0, 10f);
            entryTransform.Find("NameText").GetComponent<TextMeshProUGUI>().characterSpacing = 9;
        }

            //ESTOY AQUÍ, ARREGLAR POSICIONAMIENTO DE _ _ _ PONERLE SPACING Y MIRAR LO DE QUE TE HAGA EDITAR DIRECTAMENTE
            //POR QUÉ SALE VARIAS VECES EL INPUT FIELD

        int score = highscoreEntry.score;
        entryTransform.Find("ScoreText").GetComponent<TextMeshProUGUI>().text = score.ToString();

        if (score.ToString() == carryOvers.GetFinalScore() && highscoreEntry.recent == true)
        {
            
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



        // Set background visible odds and evens, easier to read
        //entryTransform.Find("background").gameObject.SetActive(rank % 2 == 1);

        transformList.Add(entryTransform);
        templateList.Add(entryTransform.gameObject);

    }

    public void CharDown()
    {
        sfxSource.pitch = 1;
        sfxSource.PlayOneShot(arrowsClick);
        //TextMeshProUGUI initialsText = GameObject.Find("LetterDownButton").GetComponent<TextMeshProUGUI>();

        if (currentChar == 34)
        {
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

        sfxSource.pitch = 1.5f;
        sfxSource.PlayOneShot(arrowsClick);
        //TextMeshProUGUI initialsText = GameObject.Find("LetterDownButton").GetComponent<TextMeshProUGUI>();
        if (currentInitialsText.textInfo.characterCount <= 3)
        {
            lockedChars += alphabet[currentChar].ToString();
            //Debug.Log("the saved chars are" + lockedChars);

            if (currentInitialsText.textInfo.characterCount < 3)
            { UpdateInitialsText(); }

            if ( currentInitialsText.textInfo.characterCount == 3)
            {
                currentInitialsText.text = lockedChars;

                Button letterUp = GameObject.Find("LetterUpButton").GetComponent<Button>();
                Button letterDown = GameObject.Find("LetterDownButton").GetComponent<Button>();
                Button okButton = GameObject.Find("OkButton").GetComponent<Button>();

                letterUp.interactable = false;
                letterDown.interactable = false;
                okButton.interactable = false;

                //Debug.Log("Max count achieved, activating buttons...");

                backToMenu = GameObject.Find("Back To Menu Button").GetComponent<Button>();
                //restartButton = GameObject.Find("Restart Button").GetComponent<Button>();
                backToMenu.interactable = true;
                //sceneLoader = FindObjectOfType<SceneLoader>();
                //sceneLoader.SetListenerButton(backToMenu);
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
            // Reload
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
            //jsonString = PlayerPrefs.GetString("BedroomIntermediate");
            //highscores = JsonUtility.FromJson<Highscores>(jsonString);
            
        }
    }

    public void CheckDropDownChange(string dropDownOption)
    {
        //PENSAR EN HACER UNA LISTA DE EASY MEDIUM Y HARD, Y QUE CUANDO SE CREE POR PRIMERA VEZ UNA LISTA DE CADA COSA
        //QUE COJA ITEMS DE LA LISTA DE MANERA RANDOM Y ASÍ CADA LISTA ES DISTINTA DE LA OTRA

        // Comando para forzar default

        //PlayerPrefs.DeleteKey(dropDownOption);

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
            //jsonString = PlayerPrefs.GetString("BedroomIntermediate");
            //highscores = JsonUtility.FromJson<Highscores>(jsonString);
            SortAndShow();
        }
        
    }

    private void SortAndShow()
    {
        // Sort list by score

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
        // Instance Entry
        HighscoreEntry highscoreEntry = new HighscoreEntry { score = score, name = name, date = date };

        // Load Entire Highscore
        string jsonString = PlayerPrefs.GetString("BedroomIntermediate");
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);

        // Add new entry to it
        highscores.highscoreEntryList.Add(highscoreEntry);

        // Save changes
        string json = JsonUtility.ToJson(highscores);
        PlayerPrefs.SetString("BedroomIntermediate", json);
        PlayerPrefs.Save();
    }

    private void AddHighscoreEntry(int score, string name, string date)
    {
        // Instance Entry
        HighscoreEntry highscoreEntry = new HighscoreEntry { score = score, name = name, date = date};

        // Load Entire Highscore
        string jsonString = PlayerPrefs.GetString("highscoretable");
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);

        // Add new entry to it
        highscores.highscoreEntryList.Add(highscoreEntry);

        // Save changes
        string json = JsonUtility.ToJson(highscores);
        PlayerPrefs.SetString("highscoreTable", json);
        PlayerPrefs.Save();
    }

    private class Highscores
    {
        public List<HighscoreEntry> highscoreEntryList;
    }

    [System.Serializable]
    private class HighscoreEntry
    {
        //just one entry
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

    //public void ReadStringInput(string str)
    //{
    //    inputScore = str;
    //    Debug.Log("you have written " + inputScore + " inside the inputfield.");

    //    if (inputScore.Length == 3)
    //    {
    //        inputFieldScore.interactable = false;
    //        isWritten = true;
    //        scrollArea.enabled = true;

    //        Button backToMenu = GameObject.Find("Back To Menu Button").GetComponent<Button>();
    //        Button restart = GameObject.Find("Restart Button").GetComponent<Button>();
    //        backToMenu.enabled = true;
    //        restart.enabled = true;

    //        Debug.Log("Deactivating inputfield.");
    //        SaveRealRecord();
    //        OnlySort();
    //        SaveAfterShowing();
    //    } 
        
    // }

    //private void SaveRealRecord()
    //{
    //    for (int i = 0; i < highscores.highscoreEntryList.Count; i++)
    //    {
    //        if (highscores.highscoreEntryList[i].name == "___")
    //        {
    //            Debug.Log("Found the blank entry, deleting it...");
    //            highscores.highscoreEntryList.RemoveAt(i);
    //            Debug.Log("Blank entry deleted, adding new real entry...");
    //            highscores.highscoreEntryList.Add(new HighscoreEntry { score = scoreToSave, name = inputScore.ToUpper(), date = dateToSave, recent = false });
    //        }

    //    }
        
    //}

    private void SaveRealRecordText()
    {
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

    //public void ReselectInputfield()
    //{
    //    if (isWritten == false)
    //    {
    //        TMP_InputField inputObjectScore = GameObject.Find("InputFieldScore").GetComponent<TMP_InputField>();
    //        inputObjectScore.ActivateInputField();
    //        inputObjectScore.Select();

    //    }

    //}

    
}


