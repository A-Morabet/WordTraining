using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using UnityEngine.EventSystems;
using System;
using SimpleJSON;
using System.Runtime.CompilerServices;


public class NewWordPacker : MonoBehaviour
{

// Setting Variables from both Unity Scene and inside script.

    GameSession gameSession;
    private SpeedHandler speedHandler;
    public TextMeshProUGUI displayWordClue;
    [SerializeField] TextMeshProUGUI displayWordText;
    [SerializeField] Animator displayWordAnim = null;
    [SerializeField] Animator displayPhraseAnim = null;

    // Audio clips and associated assets.
    
    AudioSource audioSource;
    [SerializeField] AudioClip letterSuccess;
    [SerializeField] AudioClip letterWrong;
    [SerializeField] AudioClip wordSuccess;
    [SerializeField] AudioClip crowdClap;
    [SerializeField] AudioClip crowdCheer;
    [SerializeField] AudioClip crowdBoo;
    [SerializeField] AudioClip letterClue;
    [SerializeField] AudioClip letterClueNo;
    [SerializeField] GameObject littleDevil;
    [SerializeField] GameObject jetPodObject;
    [SerializeField] GameObject circleLookObject;

    // App level scripts.

    private Timer timer;
    private DestroyerHandler destroyerHandler;
    private SpawnerDown spawnerDown;
    private WordChecker wordChecker;
    private ImagePacker imagePacker;
    private ScoreHandler scoreHandler;
    private CarryOvers carryOvers;
    private ParticleHandler particleHandler;
    private CountStart countStart;
    private ScrollBGCanvas scrollBGCanvas;
    
    // Fire particles from the Boss TV asset.
    ParticleSystem particleJetFire;

    // Hangman section (underscores and word letters)
    
    private int wordsCompleted;
    private int wordsToComplete;
    private string activeWord;
    private char letterChanged;
    private int spaceIndex;
    private bool spaceBool = false;

    public List<string> letterArray;
    public List<char> specialLetterArray;
    public List<char> wordChars;
    public List<char> wordCharsCheck;
    private List<char> wordCharsScores;
    private int wordCount;
    private int storeLetterIndex;
    private int randomWord;
    private string activeWordValue;

    private bool underScoresPresent;
    private int underScoresCount;

    // Boss and warning animations
    
    private BossBehaviour bossTV;
    private bool canGimmeScoreBoss;
        [SerializeField] GameObject warningObject;
    [SerializeField] Animator warningAnim;
    private WarningBehaviour warningBehaviour;

    // Clue button and score text
    
    [SerializeField] Sprite clueDisabled;
    [SerializeField] Sprite letterDisabled;
    [SerializeField] Sprite clueEnabled;
    [SerializeField] Sprite letterEnabled;
    [SerializeField] Animator scoreTextAnim;
    [SerializeField] Animator letterClueAnim;

    // Dictionary 

    public Dictionary<string, string> activeDictionary;
    private Dictionary<string, string> semanticFieldsDict;
    List<int> storedChoices;
    
    
    // Button 
    
    Button letterClueButton;
    Button phraseClueButton;
    

    // Feedback
    
    private float feedbackTime = 0.05f;
    private float feedbackClue = 0.05f;

    
    void Start()
    {

    // Setting References for variables in script when it starts.
        
        gameSession = FindObjectOfType<GameSession>();
        carryOvers = FindObjectOfType<CarryOvers>();
        timer = FindObjectOfType<Timer>();
        audioSource = GetComponent<AudioSource>();
        particleHandler = FindObjectOfType<ParticleHandler>();
        scrollBGCanvas = FindObjectOfType<ScrollBGCanvas>();
        wordsToComplete = 3;

        semanticFieldsDict = new Dictionary<string, string>();
        LoadDictionary("SemanticFields", semanticFieldsDict);
        
        activeDictionary = new Dictionary<string, string>();
        LoadDictionaryMOD(carryOvers.GetChoiceString(), activeDictionary);
        storedChoices = new List<int>();
        storedChoices.Add(0); 
        
        bossTV = FindObjectOfType<BossBehaviour>();      
        wordChecker = FindObjectOfType<WordChecker>();
        spawnerDown = FindObjectOfType<SpawnerDown>();
        imagePacker = FindObjectOfType<ImagePacker>();
        
        destroyerHandler = FindObjectOfType<DestroyerHandler>();
        scoreHandler = FindObjectOfType<ScoreHandler>();
        speedHandler = FindObjectOfType<SpeedHandler>();

        phraseClueButton = GameObject.Find("PhraseClueButton").GetComponent<Button>();
        letterClueButton = GameObject.Find("LetterClueButton").GetComponent<Button>();

        countStart = FindObjectOfType<CountStart>();


        NextWord();

        FillCharArray();
        CreateUnderscores();
        
        
    }


    public void setPackerReferences()
    {
        gameSession = FindObjectOfType<GameSession>();
        carryOvers = FindObjectOfType<CarryOvers>();
        timer = FindObjectOfType<Timer>();
        wordChecker = FindObjectOfType<WordChecker>();
        spawnerDown = FindObjectOfType<SpawnerDown>();
        destroyerHandler = FindObjectOfType<DestroyerHandler>();
        scoreHandler = FindObjectOfType<ScoreHandler>();
        speedHandler = FindObjectOfType<SpeedHandler>();
    }

    private void NextWord()
    {
        // Function gets word from dictionary and readies it for following functions.
    
        displayWordClue.text = "";

        randomWord = UnityEngine.Random.Range(0, activeDictionary.Count);
        
        activeWord = activeDictionary.ElementAt(randomWord).Key.ToUpper();
        
        // Image Packer gets image and updates canvas based on random word from JSON dictionary.
        imagePacker.GetImageKey(activeWord);
        imagePacker.GetImageValue();
        imagePacker.SetImageCanvas();

        activeDictionary.TryGetValue(activeWord.ToLower(), out activeWordValue);
        activeDictionary.Remove(activeDictionary.ElementAt(randomWord).Key);
        wordCount = activeWord.Length;

        
    }

    private void NextWordBoss()
    {
        // Function gets Boss word from dictionary and readies it for following functions.
    
        displayWordClue.text = "";
        activeDictionary.Clear();
        LoadDictionaryMOD(carryOvers.GetChoiceString() + "Boss", activeDictionary);
        

        imagePacker.LoadBossDictIMG();
        randomWord = UnityEngine.Random.Range(0, activeDictionary.Count);
        activeWord = activeDictionary.ElementAt(randomWord).Key.ToUpper();
        
        imagePacker.GetImageKey(activeWord);
        imagePacker.GetImageValue();
        imagePacker.SetImageCanvas();

        activeDictionary.TryGetValue(activeWord.ToLower(), out activeWordValue);
        activeDictionary.Remove(activeDictionary.ElementAt(randomWord).Key);
        wordCount = activeWord.Length;


    }

    private void FillCharArray()
    {
        // Based on word count, it fills several chars array used later for comparing and filling letters, and also for clue system.
    
        wordChars = activeWord.ToList();
        wordCharsCheck = activeWord.ToList();
        wordCharsScores = activeWord.ToList();
        specialLetterArray = activeWord.ToList();
        EliminateSpacesChar();
        EliminateSpacesCheck();
        EliminateSpacesSpecial();


    }
    private void CreateUnderscores()
    {
         // Creates undescores seen in game and takes spaces into account.
    
        letterArray = new List<string>();
        

        for (int letters = 0; letters < wordCount; letters++)
        {
            letterArray.Add("");
        }

        displayWordText.text = "";  // Clears word display.

        for (int letters = 0; letters < wordCount; letters++)
        {
            if(wordCharsScores[letters].ToString() == " ")
            {
                letterArray[letters] = " ";
                
            }
            if (wordCharsScores[letters].ToString() != " ")
            {
                letterArray[letters] = "_";
            }

            displayWordText.text += letterArray[letters];

        }
        destroyerHandler.EnableTouch();
    }

    public void CompareDestroyedLetter()
    {
        // Compares if letter destroyed matches or not one of the letters of the displayed word.
    
        if (gameSession.GetOnGoingBoss())
        {
            
            destroyerHandler.DisableTouch();
        }
        for (int letters = 0; letters < wordCharsCheck.Count; letters++)
        {
            if (carryOvers.GetLetterDH() == wordCharsCheck[letters].ToString())
            {
                storeLetterIndex = letters;
                wordCharsCheck[storeLetterIndex] = '0' ;
                StartCoroutine(WaitAndChange());

                return;
            }

            
        }
        StartCoroutine(WaitAndReduce());
    }



    private IEnumerator WaitAndChange()
    {
        // Checks first if the app is currently in a Boss Level or not.
        // If compared letter matches, it provides audio and visual feedback (sfx and letter appears).
        // It also checks if it was the last letter, which then proceeds to complete the word and call all pertinent functions.
    
        yield return new WaitForSecondsRealtime(feedbackTime);
        PlayScoreGood();

        CountUnderscores();
        
        if (carryOvers.GetTagDH() == "BossTV")
        {
            StartCoroutine(bossTV.PlaySuccessAnimation());
            
        }

        if (underScoresCount == 1)
        {
            ChangeFinalChar();
            yield break;
        }
        
        
        if (underScoresCount > 1)
        { 
        
        ChangeCorrectChar();
        audioSource.PlayOneShot(letterSuccess);
        scoreHandler.IncreasePoints();
        scoreHandler.EnableSendIt();
        yield return new WaitForSeconds(0.001f);
        scoreHandler.IncreaseMultiBar();
        
        CountUnderscores();
        if (carryOvers.GetTagDH() == "BossTV" && underScoresCount == 1)
          {
                letterClueButton.enabled = false;
                Image letterClueImage = GameObject.Find("LetterClueButton").GetComponent<Image>();
                letterClueImage.sprite = letterDisabled;
          }

                scoreHandler.DisableSendIt();
        }
    }

    private IEnumerator WaitAndReduce()
    {
        // If compared letter is incorrect feedback is provided and score is reduced.
    
        yield return new WaitForSecondsRealtime(feedbackTime);
        PlayScoreBad();
        
        if (carryOvers.GetTagDH() == "BossTV")
        {
            StartCoroutine(bossTV.PlayFailAnimation());
        }
        audioSource.PlayOneShot(letterWrong);
        scoreHandler.DecreasePoints();
        scoreHandler.EnableSendIt();
        yield return new WaitForSeconds(0.001f);
        scoreHandler.DecreaseMultiBar();
        scoreHandler.DisableSendIt();
    }



    private void ChangeCorrectChar()
    {

        // When changing letters in char array after successful letter it checks for spaces.
        // Spacebool determines if the current word has a space in between or not.

        if (spaceBool == true)
        {

            if (storeLetterIndex >= spaceIndex)
            {
                letterArray[storeLetterIndex+1] = wordChars[storeLetterIndex].ToString();
                //Debug.Log("the underscore(true) has now become " + letterArray[storeLetterIndex + 1]);
                letterChanged = wordChars[storeLetterIndex];
            }
            if (storeLetterIndex < spaceIndex)
            {
                letterArray[storeLetterIndex] = wordChars[storeLetterIndex].ToString();
                //Debug.Log("the underscore(true) has now become " + letterArray[storeLetterIndex]);
                letterChanged = wordChars[storeLetterIndex];
            }

        }

        if (spaceBool == false) 
        {
            //Debug.Log("space bool was not true!");

            letterArray[storeLetterIndex] = wordChars[storeLetterIndex].ToString();
            //Debug.Log("the underscore(false) has now become " + letterArray[storeLetterIndex]);
            letterChanged = wordChars[storeLetterIndex];
        }


        specialLetterArray.Remove(letterChanged);
        //Debug.Log("letter " + letterChanged + " was removed.");
        UpdateSlots();


    }

    private void ChangeFinalChar()
    {
        // Same as previous function but called when filling last word letter.
    
        if (spaceBool == true)
        {
            //Debug.Log("space bool was true!");
            if (storeLetterIndex >= spaceIndex)
            {
                letterArray[storeLetterIndex + 1] = wordChars[storeLetterIndex].ToString();
                //Debug.Log("the underscore(true) has now become " + letterArray[storeLetterIndex + 1]);
                letterChanged = wordChars[storeLetterIndex];
                
                
            }

            if (storeLetterIndex < spaceIndex)
            {
                letterArray[storeLetterIndex] = wordChars[storeLetterIndex].ToString();
                //Debug.Log("the underscore(true) has now become " + letterArray[storeLetterIndex]);
                letterChanged = wordChars[storeLetterIndex];
                

            }
            
        }

        if (spaceBool != true)
        {
            //Debug.Log("space bool was not true!");
            letterArray[storeLetterIndex] = wordChars[storeLetterIndex].ToString();
            //Debug.Log("the underscore(false) has now become " + letterArray[storeLetterIndex]);
            letterChanged = wordChars[storeLetterIndex];
        }

        specialLetterArray.Remove(letterChanged);
        //Debug.Log("letter " + letterChanged + " was removed.");
        
        UpdateSlots();

        if (carryOvers.GetTagDH() != "BossTV")
        {
            
            StartCoroutine(WaitAndComplete());
        }
    
        if (carryOvers.GetTagDH() == "BossTV")
        {
            bossTV.EnableItIsDone();
            
            StartCoroutine(WaitAndCompleteBossKill());
        }

    }

    private void UpdateSlots()
    {
        displayWordText.text = "";
        for (int letters = 0; letters < wordCount; letters++)
        {

            displayWordText.text += letterArray[letters];
        }
    }

    private void CheckUnderScores()

    {
        underScoresPresent = true;
        
        for (int letters = 0; letters < wordChars.Count; letters++)
        {
            underScoresPresent = letterArray[letters].Contains("_");
            
            if ( underScoresPresent == true)
            {
                break;
            }

        }
        
        if (underScoresPresent == false)
        {
            
            StartCoroutine(WaitAndComplete());
        }
    }

    private IEnumerator WaitAndComplete()
    {
        // Function that contributes to setting up the next word, it also checks if the requirements have been met to enter Boss Level.
    
        yield return new WaitForSecondsRealtime(0);
        underScoresCount = 0;
        audioSource.PlayOneShot(wordSuccess);
        audioSource.PlayOneShot(crowdClap);
        scoreHandler.IncreasePoints();
        scoreHandler.AddLetterPoints();
        scoreHandler.EnableSendIt();
        yield return new WaitForSeconds(0.0001f);
        destroyerHandler.DisableTouch();
        scoreHandler.IncreaseMultiBar(); //ESTO
        speedHandler.IncreaseWordBar();
        speedHandler.CheckSpeedZone();
        scoreHandler.DisableSendIt();

        wordsCompleted++;
        CheckWordsCompleted();

        if (gameSession.GetActivateBoss())
        {
            StartCoroutine(PlayTextAnimationBossIntro());
        }
        if (gameSession.GetActivateBoss() == false)
        {
            StartCoroutine(PlayTextAnimation());
        }
        if (gameSession.GetOnGoingBoss())
        {
            StartCoroutine(PlayTextAnimationBossOutro());
        }

    }

    private void CheckWordsCompleted()
    {
        // Determines if Boss level will start based on amount of completed words.
        
        if (wordsCompleted == wordsToComplete )
        {
            ResetWordsCompleted();
            gameSession.ActivateBoss();
            StartCoroutine(gameSession.HoldWaves());
        }
    }

    private IEnumerator WaitAndCompleteBossKill()
    {
        // Runs when the last letter in Boss Level is completed.
        
        destroyerHandler.DisableTouch();
        //Debug.Log("You Killed the Boss!!!");
        underScoresCount = 0;
        audioSource.PlayOneShot(wordSuccess);
        audioSource.PlayOneShot(crowdClap);
        audioSource.PlayOneShot(crowdCheer);
        scoreHandler.IncreasePoints();
        scoreHandler.AddLetterPoints();
        EnableGimmeScoreBoss();
        scoreHandler.EnableSendIt();
        yield return new WaitForSeconds(0.0001f);
        DisableGimmeScoreBoss();
        scoreHandler.DisableSendIt();
        
        StartCoroutine(PlayTextAnimationBossOutro());


    }


    private IEnumerator PlayTextAnimation()
    {
        // Animation played when finishing a word, during the animation certain features are disabled to prevent any exploits.
    
        displayWordAnim.Play("wordSuccess", 0, 0.0f);
        wordChecker.DisableChecks();

        DisableClueButtons();

        yield return new WaitForSeconds(3);
        
        displayWordAnim.Play("idle", 0, 0.0f);

        StartCoroutine (bossTV.BetweenWordsAnimation());
        spaceBool = false;
        NextWord();
        //Debug.Log("I finished NextWord");

        FillCharArray();
        CreateUnderscores();
        wordChecker.EnableChecks();

        yield return new WaitForSeconds(1.25f);

        EnableClueButtons();
    }

    private IEnumerator PlayTextAnimationBossIntro()
    {
        // Changes needed before entering Boss Level, some of the changes include pausing the wave and the countdown timer.

        displayWordAnim.Play("wordSuccess", 0, 0.0f);
        wordChecker.DisableChecks();

        yield return new WaitForSeconds(3);
        
        displayWordAnim.Play("idle", 0, 0.0f);

        gameSession.DisableActivateBoss();
        gameSession.ActivateOnGoingBoss();

        yield return StartCoroutine(RealBossIntro());
        
        // Here starts change to Boss Level.

        bossTV.GetReadyForBoss();
        
        while (bossTV.GetReadyBool() == true)
        {
            yield return null;
        }
        bossTV.DisableFailEmote();
        StartCoroutine(bossTV.BetweenWordsAnimation());
        yield return new WaitForSeconds(bossTV.GetAnimTime());
        
        bossTV.EnableImageSlot();
        spaceBool = false;
        NextWordBoss();
        
        //Debug.Log("I finished NextWordBoss");
        
        FillCharArray();
        CreateUnderscores();
        wordChecker.EnableChecks();
        circleLookObject.SetActive(true);
        circleLookObject.transform.position = bossTV.transform.position;
        yield return new WaitForSeconds(4f);
        circleLookObject.SetActive(false);
        scoreHandler.ClueBossMode();
        StartCoroutine(bossTV.BetweenWordsAnimation());
        yield return new WaitForSeconds(bossTV.GetAnimTime());
        bossTV.BossEnable();
        destroyerHandler.EnableTouch();
        letterClueButton.interactable = true;
        timer.DeactivateTimerWait();
        bossTV.DisableImageSlot();
        bossTV.DisableColDevil();
        
    }

    private IEnumerator RealBossIntro()
    {
        // The Boss Animation intro is a mix of unity animation tools and fine-tuned scripting.
        // It also prevents any exploits or stability issues that may happen during the intro.
    
        timer.ActivateTimerWait();
        letterClueButton.interactable = false;
        destroyerHandler.DisableTouch();
        speedHandler.WordBarDisable();
        warningObject.SetActive(true);
        warningBehaviour = FindObjectOfType<WarningBehaviour>();
        int counterLimit = speedHandler.SetCounterLimitWarning(speedHandler.GetTimeScaleValue());
        warningBehaviour.SetCounterLimit(counterLimit);
        gameSession.StopMusic();
        warningAnim.Play("WarningFlash");
        
        while (warningObject.activeSelf)
        {
            yield return null;
        }
        RectTransform canvasObject = GameObject.Find("Game Scene Canvas").GetComponent<RectTransform>();
        GameObject littleDevilInst = Instantiate(littleDevil, gameSession.GetDevilPosition(), transform.rotation);
        gameSession.PlaySFXOnce("jumpClip");

        while (littleDevilInst != null)
        {
            yield return null;
        }

        counterLimit = speedHandler.SetCounterLimitTV(speedHandler.GetTimeScaleValue());
        bossTV.SetCounterLimit(counterLimit);
        bossTV.playTinkerAnim();
        StartCoroutine(bossTV.BetweenWordsAnimationTinker());
        gameSession.PlaySFXOnce("tinkerClip");
        while (bossTV.GetTinkerCompletion() == false)
        {
            Vector3 randomPos = bossTV.gameObject.transform.position +
            new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));
            
            particleHandler.SpawnParticles("TinkerNoise", randomPos);
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.2f,1f));
            yield return null;
        }
        bossTV.ResetTinker();
        gameSession.StopSFX();
       
        bossTV.DisableImageSlot();
        yield return new WaitForSeconds(0.25f);
        gameSession.PlaySFXOnce("turnOnClip");
        yield return StartCoroutine(bossTV.FadeInFailAnimation());
        yield return new WaitForSeconds(0.2f);
        gameSession.PlaySFXOnce("laughClip");
        yield return new WaitForSeconds(0.9f);
        gameSession.PlaySFXOnce("jetpackClip");
        yield return StartCoroutine(bossTV.JetSet());
        Vector3 jetPos = jetPodObject.transform.position;
        particleHandler.SpawnParticles("JetFire", jetPos);
        gameSession.PlaySFXOnce("fireClip");
        particleJetFire = particleHandler.GetCachedParticleSystem();
        yield return new WaitForSeconds(1f);
        gameSession.PlayBossMusic();
        bossTV.PlayStandingAnimation();
        

    }

    private IEnumerator PlayTextAnimationBossOutro()
    {
        // Animation created along the manners of the previous function for when the Boss Level is completed.
    
        timer.ActivateTimerWait();
        gameSession.PlayDefeatMusic();
        bossTV.PlayJetStill();
        displayWordAnim.Play("wordSuccess", 0, 0.0f);
        bossTV.EnableCanMoveCenter();

        while (bossTV.GetMoveCenter())
        {
            ShakeAnything shakerTV = bossTV.gameObject.GetComponent<ShakeAnything>();
          

            Vector3 randomPos = bossTV.gameObject.transform.position +
            new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));
            particleHandler.SpawnParticles("TinkerNoise", randomPos);
            gameSession.PlaySFXOnce("exploClip");
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.2f, 0.8f));
            
        }
        gameSession.StopSFX();
        yield return new WaitForSeconds(bossTV.PlayFadeInSuccess());
        bossTV.ResetOnScreen();
        yield return new WaitForSeconds(bossTV.PlayShakeOff()); 
        yield return new WaitForSeconds(1f);
        bossTV.ShowVictory();
        gameSession.PlaySFXOnce("peaceClip");
        yield return new WaitForSeconds(1f);
        bossTV.EnableCanMoveBack();
        
        wordChecker.DisableChecks();

        yield return new WaitForSeconds(3);
        particleJetFire.emissionRate = 0f;
        yield return new WaitForSeconds(1f);
        Destroy(particleJetFire);
        bossTV.ResetOffScreen();
        bossTV.PlayJetHide();
        gameSession.PlaySFXOnce("jetpackClip");
        yield return new WaitForSeconds(1);
        displayWordAnim.Play("idle", 0, 0.0f);
        gameSession.DeactivateOnGoingBoss();
        bossTV.EnableColDevil();
        timer.IncreaseTimerAfterBoss();
        StartCoroutine(bossTV.BetweenWordsAnimation());
        yield return new WaitForSeconds(bossTV.GetAnimTime());
        bossTV.DisableItIsDone();
        
        SetPreviousDictionary();
        NextWord();
        //Debug.Log("I finished NextWord");

        FillCharArray();
        CreateUnderscores();
        wordChecker.EnableChecks();
        scoreHandler.ClueNormalMode();
        speedHandler.WordBarEnable();
        gameSession.PlayMusic();
        timer.DeactivateTimerWait();
        Image letterClueImage = GameObject.Find("LetterClueButton").GetComponent<Image>();
        letterClueImage.sprite = letterEnabled;
        letterClueButton.enabled = true;
        wordsToComplete += 2;
    }


    public int GetWordCount()
    {
        return wordCount;
    }

    private void CountUnderscores()
    {

        underScoresCount = specialLetterArray.Count;
         //Debug.Log(underScoresCount + " Underscores Remaining");
    }

    public void ShowLetterClue()
    {
        //When pressing on the Letter Clue button, it shows one of the word's letters at the cost of the user's score.
    
        if (scoreHandler.DecreaseCluePoints() == true)
        {

            scrollBGCanvas.ButtonAnimation(letterClueButton.gameObject);
            StartCoroutine(LetterClueCoroutine()); 
        }
        else
        {
            scrollBGCanvas.ButtonAnimation(letterClueButton.gameObject);
            audioSource.PlayOneShot(letterClueNo);
           
        }
    }


    private IEnumerator LetterClueCoroutine()
    {
        // Function that handles filling one letter in the word.
        // It also checks for spaces when doing so.

        CountUnderscores();
        yield return new WaitForSecondsRealtime(feedbackClue);

        PlayScoreBad();

        for (int letters = 0; letters < letterArray.Count; letters++)
        {
            if (letterArray[letters] == "_")
            {
                bool dontCheckBelow = false;

                if (underScoresCount == 1)
                {
                    //Debug.Log("Last Underscore condition true");
                    dontCheckBelow = true;
                    int offset = 1;
                    
                    underScoresCount = 0;

                    if (spaceBool == true)
                    {
                        //Debug.Log("spaceBool is true!");
                        if (letters >= spaceIndex)
                        {
                            letterArray[letters] = wordChars[letters - offset].ToString();
                            letterChanged = wordChars[letters - offset];
                            wordCharsCheck[letters - offset] = '0';
                            specialLetterArray.Remove(letterChanged);
                            UpdateSlots();
                            StartCoroutine(WaitAndCompleteLetterClue());
                            //Debug.Log("I started the wait and complete");
                            yield break;

                        }

                        if (letters < spaceIndex)
                        {
                            letterArray[letters] = wordChars[letters].ToString();
                            letterChanged = wordChars[letters];
                            wordCharsCheck[letters] = '0';
                            specialLetterArray.Remove(letterChanged);
                            UpdateSlots();
                            StartCoroutine(WaitAndCompleteLetterClue());
                            //Debug.Log("I started the wait and complete");
                            yield break;
                        }

                        
                    }

                    if (spaceBool == false)
                        //Debug.Log("spaceBool is not true!");
                    {
                        letterArray[letters] = wordChars[letters].ToString();
                        letterChanged = wordChars[letters];
                        wordCharsCheck[letters] = '0';
                        specialLetterArray.Remove(letterChanged);
                        UpdateSlots();
                        StartCoroutine(WaitAndCompleteLetterClue());
                        //Debug.Log("I started the wait and complete");

                        yield break;
                    }

                }

                //Debug.Log("there are " + underScoresCount + " underscores.");

                if (underScoresCount > 1)
                {
                    //Debug.Log("Last Underscore condition NOT true");

                    if (spaceBool == true && dontCheckBelow == false)
                    {
                        //Debug.Log("spaceBool is true and Below is false");
                        int offset = 1;

                        if (letters >= spaceIndex)
                        {
                            //Debug.Log("space index is more than index letter");
                            letterArray[letters] = wordChars[letters - offset].ToString();
                            letterChanged = wordChars[letters - offset];
                            wordCharsCheck[letters - offset] = '0';
                            specialLetterArray.Remove(letterChanged);
                            UpdateSlots();
                            audioSource.PlayOneShot(letterClue);
                            CountUnderscores();
                            if (gameSession.GetOnGoingBoss() && underScoresCount == 1)
                            {
                                letterClueButton.enabled = false;
                                Image letterClueImage = GameObject.Find("LetterClueButton").GetComponent<Image>();
                                letterClueImage.sprite = letterDisabled;
                            }
                            yield break;

                        }

                        if (letters < spaceIndex)
                        {
                            //Debug.Log("space index is less than index letter");
                            letterArray[letters] = wordChars[letters].ToString();
                            letterChanged = wordChars[letters];
                            wordCharsCheck[letters] = '0';
                            specialLetterArray.Remove(letterChanged);
                            UpdateSlots();
                            audioSource.PlayOneShot(letterClue);
                            CountUnderscores();
                            if (gameSession.GetOnGoingBoss() && underScoresCount == 1)
                            {
                                letterClueButton.enabled = false;
                                Image letterClueImage = GameObject.Find("LetterClueButton").GetComponent<Image>();
                                letterClueImage.sprite = letterDisabled;
                            }
                            dontCheckBelow = false;
                            yield break;
                        }
                        
                    }


                    if (spaceBool == false && dontCheckBelow == false)
                    {
                        //Debug.Log("spaceBool is false and Below is false");
                            letterArray[letters] = wordChars[letters].ToString();
                            letterChanged = wordChars[letters];
                            wordCharsCheck[letters] = '0';
                            specialLetterArray.Remove(letterChanged);
                            UpdateSlots();
                            audioSource.PlayOneShot(letterClue);
                            CountUnderscores();
                        if (gameSession.GetOnGoingBoss() && underScoresCount == 1)
                        {
                            letterClueButton.enabled = false;
                            Image letterClueImage = GameObject.Find("LetterClueButton").GetComponent<Image>();
                            letterClueImage.sprite = letterDisabled;
                        }
                        //dontCheckBelow = false;
                        yield break;
                    }

                }
            }
        }
    }

// Here should be a phrase clue function that handles phrases that help the user remember the word such as "I eat the soup with the _____",
// This helps the user remember that the word in question is "spoon", it's called Semantic cueing.
// This function will be developped after publish process.
       
    }


    private IEnumerator WaitAndCompleteLetterClue()
    {
        // If the user completes the word with the clue button, he doesn't get as much score and a different sfx.
        
        yield return new WaitForSecondsRealtime(0);
        PlayScoreBad();
        underScoresCount = 0;
        audioSource.PlayOneShot(letterClue);
        audioSource.PlayOneShot(letterSuccess);
        audioSource.PlayOneShot(crowdBoo);
        scoreHandler.AddLetterPoints();
        destroyerHandler.DisableTouch();
        wordsCompleted++;

        if (gameSession.GetActivateBoss() == true)
        {
            StartCoroutine(PlayTextAnimationBossIntro());
        }
        if (gameSession.GetActivateBoss() == false)
        { 
            StartCoroutine(PlayTextAnimation());
        //Debug.Log("I played the Text Animation");

        }
    }

 // Handles enabling and disabling clue button and updating interface.

    private void DisableClueButtons()
    {
       

        phraseClueButton.enabled = false;
        letterClueButton.enabled = false;
        Image clueImage = phraseClueButton.gameObject.GetComponent<Image>();
        Image letterImage = letterClueButton.gameObject.GetComponent<Image>();
        clueImage.sprite = clueDisabled;
        letterImage.sprite = letterDisabled;
    }

    private void EnableClueButtons()
    {

        phraseClueButton.enabled = true;
        letterClueButton.enabled = true;
        Image clueImage = phraseClueButton.gameObject.GetComponent<Image>();
        Image letterImage = letterClueButton.gameObject.GetComponent<Image>();
        clueImage.sprite = clueEnabled;
        letterImage.sprite = letterEnabled;
    }

    private void PlayScoreGood()
    {
        scoreTextAnim.Play("ScoreGood", -1, 0.0f);
    }

    private void PlayScoreBad()
    {
        scoreTextAnim.Play("ScoreBad", -1, 0.0f);
    }

    private void SetPreviousDictionary() 
    {
        activeDictionary.Clear();
        LoadDictionaryMOD(carryOvers.GetChoiceString(), activeDictionary);
    }

    private void EliminateSpacesChar()
    {
        for (int letters = 0; letters < wordChars.Count; letters++)
        {
            if (wordChars[letters].ToString() == " ")
            {
                wordChars.RemoveAt(letters);
                //Debug.Log("I removed it");
                spaceIndex = letters;
                spaceBool = true;
            }


        }
    }

    private void EliminateSpacesCheck()
    {
        for (int letters = 0; letters < wordCharsCheck.Count; letters++)
        {
            if (wordCharsCheck[letters].ToString() == " ")
            {
                wordCharsCheck.RemoveAt(letters);
                //Debug.Log("I removed it");

            }         

        }
    }

    private void EliminateSpacesSpecial()
    {
        for (int letters = 0; letters < specialLetterArray.Count; letters++)
        {
            if (specialLetterArray[letters].ToString() == " ")
            {
                specialLetterArray.RemoveAt(letters);
                //Debug.Log("I removed it");

            }

        }
    }

    public int GetWordsCompleted()
    {
        return wordsCompleted;

    }

    public void ResetWordsCompleted()
    {
        wordsCompleted = 0;
    }

    // Functions that handle dictionary loading, they contain all the words that will be used during the level, the words do not repeat and in the event that all words are completed (very unlikely) the level ends.

    private void LoadDictionary(string dictFileName, Dictionary<string, string> outputDict)
    {
        TextAsset ta = Resources.Load(dictFileName) as TextAsset;
        //Debug.Log(dictFileName);
        JSONObject jsonObj = (JSONObject)JSON.Parse(ta.text);
        foreach (var key in jsonObj.GetKeys()) { outputDict[key] = jsonObj[key]; }
    }

    private void LoadDictionaryMOD(string dictFileName, Dictionary<string, string> outputDict)
    {
        TextAsset ta = Resources.Load("Dictionaries/" + carryOvers.GetTheme() + "/" + dictFileName) as TextAsset;
        //Debug.Log("the path is: " + "Dictionaries/" + carryOvers.GetTheme() + "/" + dictFileName);
        JSONObject jsonObj = (JSONObject)JSON.Parse(ta.text);
        foreach (var key in jsonObj.GetKeys()) { outputDict[key] = jsonObj[key]; }
    }

    private void EnableGimmeScoreBoss()
    {
        canGimmeScoreBoss = true;
    }

    private void DisableGimmeScoreBoss()
    {
        canGimmeScoreBoss = false;
    }

    public bool GetGimmeScoreBoss()
    {
        return canGimmeScoreBoss;
    }
}
