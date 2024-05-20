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
    GameSession gameSession;
    
    [SerializeField] TextMeshProUGUI displayWordText;
    [SerializeField] Animator displayWordAnim = null;
    [SerializeField] Animator displayPhraseAnim = null;
    public TextMeshProUGUI displayWordClue;
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
    ParticleSystem particleJetFire;



    private int wordsCompleted;
    private int wordsToComplete;
    private string activeWord;
    private char letterChanged;
    private int spaceIndex;
    private bool spaceBool = false;

    private BossBehaviour bossTV;
    private bool canGimmeScoreBoss;
    [SerializeField] GameObject warningObject;
    [SerializeField] Animator warningAnim;
    private WarningBehaviour warningBehaviour;
    [SerializeField] Sprite clueDisabled;
    [SerializeField] Sprite letterDisabled;
    [SerializeField] Sprite clueEnabled;
    [SerializeField] Sprite letterEnabled;
    [SerializeField] Animator scoreTextAnim;
    [SerializeField] Animator letterClueAnim;

    public Dictionary<string, string> activeDictionary;


    private Dictionary<string, string> semanticFieldsDict;
    List<int> storedChoices;



    public List<string> letterArray;
    public List<char> specialLetterArray;
    public List<char> wordChars;
    public List<char> wordCharsCheck;
    private List<char> wordCharsScores;

    private int wordCount;
    private int storeLetterIndex;
    private int randomWord;
    private string activeWordValue;

    private float feedbackTime = 0.05f;

    Button letterClueButton;
    Button phraseClueButton;
    
    
    private float feedbackClue = 0.05f;

    private bool underScoresPresent;
    private int underScoresCount;

    private SpeedHandler speedHandler;

    void Start()
    {
        //semanticFieldsDict.ElementAt(0).Key
        gameSession = FindObjectOfType<GameSession>();
        carryOvers = FindObjectOfType<CarryOvers>();
        timer = FindObjectOfType<Timer>();
        audioSource = GetComponent<AudioSource>();
        particleHandler = FindObjectOfType<ParticleHandler>();
        scrollBGCanvas = FindObjectOfType<ScrollBGCanvas>();
        wordsToComplete = 3;

        semanticFieldsDict = new Dictionary<string, string>();
        LoadDictionary("SemanticFields", semanticFieldsDict);
        //Debug.Log(semanticFieldsDict.ElementAt(3).Key);
        activeDictionary = new Dictionary<string, string>();
        LoadDictionaryMOD(carryOvers.GetChoiceString(), activeDictionary);
        storedChoices = new List<int>();
        storedChoices.Add(0);
        //Debug.Log(activeDictionary.ElementAt(3).Key);
        bossTV = FindObjectOfType<BossBehaviour>();

        //bedroom = FindObjectOfType<Bedroom>();
        wordChecker = FindObjectOfType<WordChecker>();
        spawnerDown = FindObjectOfType<SpawnerDown>();

        imagePacker = FindObjectOfType<ImagePacker>();
        //activeDictionary2 = bedroom.GetDictionaryBeginner();
        //Debug.Log(activeDictionary.ElementAt(0).Key);

        destroyerHandler = FindObjectOfType<DestroyerHandler>();
        scoreHandler = FindObjectOfType<ScoreHandler>();
        speedHandler = FindObjectOfType<SpeedHandler>();

        phraseClueButton = GameObject.Find("PhraseClueButton").GetComponent<Button>();
        letterClueButton = GameObject.Find("LetterClueButton").GetComponent<Button>();

        countStart = FindObjectOfType<CountStart>();

        NextWord();

        FillCharArray();
        CreateUnderscores();
        //Debug.Log(wordChecker.GetAlphabet());
        
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
        displayWordClue.text = "";

        randomWord = UnityEngine.Random.Range(0, activeDictionary.Count);
        //Debug.Log("There are " + activeDictionary.Count + "Words");
        activeWord = activeDictionary.ElementAt(randomWord).Key.ToUpper();
        //Método para el Image Packer
        imagePacker.GetImageKey(activeWord);
        imagePacker.GetImageValue();
        imagePacker.SetImageCanvas();

        activeDictionary.TryGetValue(activeWord.ToLower(), out activeWordValue);
        //Debug.Log("The Word Value is " + activeWordValue);
        activeDictionary.Remove(activeDictionary.ElementAt(randomWord).Key);
        wordCount = activeWord.Length;

        
    }

    private void NextWordBoss()
    {
        displayWordClue.text = "";
        activeDictionary.Clear();
        LoadDictionaryMOD(carryOvers.GetChoiceString() + "Boss", activeDictionary);
        

        imagePacker.LoadBossDictIMG();
        randomWord = UnityEngine.Random.Range(0, activeDictionary.Count);
        //Debug.Log("There are " + activeDictionary.Count + "Words");
        activeWord = activeDictionary.ElementAt(randomWord).Key.ToUpper();
        //Método para el Image Packer
        imagePacker.GetImageKey(activeWord);
        imagePacker.GetImageValue();
        imagePacker.SetImageCanvas();

        activeDictionary.TryGetValue(activeWord.ToLower(), out activeWordValue);
        //Debug.Log("The Word Value is " + activeWordValue);
        activeDictionary.Remove(activeDictionary.ElementAt(randomWord).Key);
        wordCount = activeWord.Length;


    }

    private void FillCharArray()
    {
        wordChars = activeWord.ToList();
        wordCharsCheck = activeWord.ToList();
        wordCharsScores = activeWord.ToList();
        specialLetterArray = activeWord.ToList();
        EliminateSpacesChar();
        EliminateSpacesCheck();
        EliminateSpacesSpecial();
        //Debug.Log(new string(wordChars.ToArray())); Log para convertir lista de chars a string completo


    }
    private void CreateUnderscores()
    {
        letterArray = new List<string>();
        

        for (int letters = 0; letters < wordCount; letters++)
        {
            letterArray.Add("");
        }

        displayWordText.text = ""; //CLEAR EL TEXTO

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
            

            //if (letters > 0)
            //{
            //    displayWordText.text += " ";
            //}

            displayWordText.text += letterArray[letters];

        }
        destroyerHandler.EnableTouch();
    }

    public void CompareDestroyedLetter()
    {
        if (gameSession.GetOnGoingBoss())
        {
            //Debug.Log("ongoingBoss is " + gameSession.GetOnGoingBoss());
            destroyerHandler.DisableTouch();
        }
        for (int letters = 0; letters < wordCharsCheck.Count; letters++)
        {
            if (carryOvers.GetLetterDH() == wordCharsCheck[letters].ToString())
            {
                storeLetterIndex = letters;
                wordCharsCheck[storeLetterIndex] = '0' ;
                //scoreHandler.EnableSendIt();
                StartCoroutine(WaitAndChange());

                return;
            }

            
        }
        //Debug.Log("Not the same!");
        //scoreHandler.EnableSendIt();
        StartCoroutine(WaitAndReduce());
    }



    private IEnumerator WaitAndChange()
    {
        yield return new WaitForSecondsRealtime(feedbackTime);
        PlayScoreGood();

        CountUnderscores();
        
        if (carryOvers.GetTagDH() == "BossTV")
        {
            StartCoroutine(bossTV.PlaySuccessAnimation());
            
            
        }

        if (underScoresCount == 1)
        {
            //Debug.Log("Changing final Char.");
            ChangeFinalChar();
            yield break;
        }
        //CHEQUEO SI FALTA UN UNDERSCORE EN ESTE MOMENTO, SI HAY MÁS DE UNO CHANGECORRECTCHAR, SI SOLO HAY UNO PASAMOS A OTRO MÉTODO
        
        if (underScoresCount > 1)
        { 
        //Debug.Log("Changing not final Char.");
        
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

        if (spaceBool == true)
        {
            //Debug.Log("space bool was true!");
            //Debug.Log("the stored index is " + storeLetterIndex + " and the space index is " + spaceIndex);

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

        
        //CheckUnderScores();


    }

    private void ChangeFinalChar()
    {
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
            //scoreHandler.EnableSendIt();
            StartCoroutine(WaitAndComplete());
        }

        //StartCoroutine(WaitAndComplete());
        
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

            //if (letters > 0)
            //{
            //    displayWordText.text += " ";
            //}

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
        yield return new WaitForSecondsRealtime(0); //estaba a feedback time pero lo he puesto a 0, no debe esperar dos veces
        underScoresCount = 0;
        audioSource.PlayOneShot(wordSuccess);
        audioSource.PlayOneShot(crowdClap);
        scoreHandler.IncreasePoints();
        scoreHandler.AddLetterPoints();
        scoreHandler.EnableSendIt();
        yield return new WaitForSeconds(0.0001f);
        destroyerHandler.DisableTouch();
        //AÑADO MULTIPLIERBARINCREASE AQUÍ??
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
        if (wordsCompleted == wordsToComplete )
        {
            ResetWordsCompleted();
            gameSession.ActivateBoss();
            StartCoroutine(gameSession.HoldWaves());
        }
    }

    private IEnumerator WaitAndCompleteBossKill()
    {
        //yield return new WaitForSecondsRealtime(0); //estaba a feedback time pero lo he puesto a 0, no debe esperar dos veces
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
        

        displayWordAnim.Play("wordSuccess", 0, 0.0f);
        wordChecker.DisableChecks();

        yield return new WaitForSeconds(3);
        
        displayWordAnim.Play("idle", 0, 0.0f);

        gameSession.DisableActivateBoss();
        gameSession.ActivateOnGoingBoss();

        yield return StartCoroutine(RealBossIntro());
        
        //AQUÍ OCURRE CAMBIO HACIA BOSS STAGE
        //StartCoroutine(bossTV.BetweenWordsAnimation());
        //yield return new WaitForSeconds(bossTV.GetAnimTime());

        //StartCoroutine(bossTV.PlayFailAnimationOnly());

        bossTV.GetReadyForBoss();
        //gameSession.DisableActivateBoss();
        //gameSession.ActivateOnGoingBoss();
        while (bossTV.GetReadyBool() == true)
        {
            yield return null;
        }
        bossTV.DisableFailEmote();
        StartCoroutine(bossTV.BetweenWordsAnimation());
        yield return new WaitForSeconds(bossTV.GetAnimTime());
        
        //yield return new WaitForSeconds(2f); //AQUÍ OCURRE ALGO RARO, EL STATIC DEJA DE VERSE Y EL NOISE SIGUE ACTIVO, AL AÑADIR MÁS ESPERA SE ARREGLA
        bossTV.EnableImageSlot();
        spaceBool = false;
        NextWordBoss();
        
        //Debug.Log("I finished NextWordBoss");
        //TODO PARECE FUNCIONAR BIEN, FALTA DESACTIVAR TODOS LOS SPAWNS, HACER QUE EL BOSS FUNCIONE, 
        //Y QUE AL DERROTARLO VUELVA A SU SITIO Y LA PARTIDA SIGA
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
        
        //add event at animation end on interface, increase counter from warningBehaviour
        //pause speed bar
        //set counter limit based on current speed
        //once counter limit is reached, disable game object and proceed with rest of animation.
        //playOneShotSound of Siren and Warning Voice on loop based on own counter too, at end of sound play check counter limit and stop if reach
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
        //FALTAN:
        //ANIMACIÓN TVSTILL, EL JET VUELVE A SU SITIO, APAGAR FUEGO JET
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
        timer.ActivateTimerWait();
        gameSession.PlayDefeatMusic();
        bossTV.PlayJetStill();
        displayWordAnim.Play("wordSuccess", 0, 0.0f);
        bossTV.EnableCanMoveCenter();

        while (bossTV.GetMoveCenter())
        {
            ShakeAnything shakerTV = bossTV.gameObject.GetComponent<ShakeAnything>();
            //shakerTV.MiniShakeTV();

            Vector3 randomPos = bossTV.gameObject.transform.position +
            new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));
            particleHandler.SpawnParticles("TinkerNoise", randomPos);
            gameSession.PlaySFXOnce("exploClip");
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.2f, 0.8f));
            //yield return null;
        }
        gameSession.StopSFX();
        yield return new WaitForSeconds(bossTV.PlayFadeInSuccess());
        bossTV.ResetOnScreen();
        yield return new WaitForSeconds(bossTV.PlayShakeOff()); //AQUÍ METÍ LOS SONIDOS DENTRO DE LA ANIMACIÓN COMO EVENTOS
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
        //var count = 0;

        //for (int letters = 0; letters < wordChars.Count; letters++)
        //{
        //    if (letterArray[letters] == "_")
        //    {
        //        count++;
        //    }
        //}

        //underScoresCount = count;
        //Debug.Log(underScoresCount + " Underscores Remaining");
    }


    public void ShowPhraseClue()
    {
        if (scoreHandler.DecreaseClueMulti() == true)
        {
            
            StartCoroutine(PhraseClueCoroutine());
        }
        else
        {
            // play audio que dice no se puede por no tener multiplier
        }
    }

    public void ShowLetterClue()
    {
        if (scoreHandler.DecreaseCluePoints() == true)
        {

            scrollBGCanvas.ButtonAnimation(letterClueButton.gameObject);
            StartCoroutine(LetterClueCoroutine()); 
        }
        else
        {
            scrollBGCanvas.ButtonAnimation(letterClueButton.gameObject);
            audioSource.PlayOneShot(letterClueNo);
            // Aquí meter la animación de no se puede, se puede poner grayscale y ya está
        }
    }


    private IEnumerator LetterClueCoroutine()
    {
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

    private IEnumerator PhraseClueCoroutine()
    {
        yield return new WaitForSecondsRealtime(feedbackTime);
        StartCoroutine(PlayPhraseClueAnimation());
        displayWordClue.text = activeWordValue;
    }


    private IEnumerator PlayPhraseClueAnimation()
    {
        displayPhraseAnim.Play("PhraseClue", 0, 0.0f);
        
        yield return new WaitForSeconds(5);

        displayPhraseAnim.Play("PhraseClueFadeOut", 0, 0.0f);

        
    }


    private IEnumerator WaitAndCompleteLetterClue()
    {
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

    private void SetPreviousDictionary() //PROPORCIONAR LOS NOMBRES DE TODOS LOS DICCIONARIOS A TRAVÉS DE UN ARCHIVO, Y CUANDO QUIERA VACIAR Y LLENAR,
        //SEÑALO HACIA UN ÍNDICE
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



            //int charToCheck = 0;
            //while (charToCheck<27)
            //{
            //    if (wordChars[letters] != wordChecker.GetAlphabet()[charToCheck])
            //    {
            //        charToCheck++;
            //    }
            //    if (wordChars[letters] == wordChecker.GetAlphabet()[charToCheck])
            //    {
            //        charToCheck = 0;
            //        break;
            //    }
            //}

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

    //COSAS DE JSON

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
