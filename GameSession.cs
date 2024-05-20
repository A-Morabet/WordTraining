using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameSession : MonoBehaviour
{

    private bool activateBoss = false;
    private bool onGoingBoss = false;
    private bool isAlive;

    DestroyerHandler destroyerHandler;
    CarryOvers carryOvers;
    BackgroundHandler backgroundHandler;
    CountStart countStart;
    AdsScript adsscript;
    
    NewWordPacker wordPacker;
    SpawnerDown spawnerDown;
    SpawnerLeft spawnerLeft;
    SpawnerRight spawnerRight;
    BGRLevel bgrLevel;
    IMTLevel imtLevel;
    ADVLevel advLevel;
    RectTransform devilPosition;
    Timer timer;
    DDBackAnimUI ddBackAnimUI;
    SpeedHandler speedHandler;
   
    int redCount;
    int timeCount;


    TextMeshProUGUI displayWordText;
    TextMeshProUGUI scoreText;
    TextMeshProUGUI multiplierText;
    RectTransform multiplierBar;
    RectTransform clueButtons;
    TextMeshProUGUI wordBarText;
    RectTransform wordBar;
    GameObject tv;

    AudioSource musicSource;
    AudioSource sfxSource;
    [SerializeField] AudioClip easyClip;
    [SerializeField] AudioClip mediumClip;
    [SerializeField] AudioClip hardClip;
    [SerializeField] AudioClip bossClip;
    [SerializeField] AudioClip defeatClip;
    [SerializeField] AudioClip bulletClip;
    [SerializeField] AudioClip tickScoreClip;
    [SerializeField] AudioClip hornClip;
    [SerializeField] AudioClip noRecordClip;
    [SerializeField] AudioClip yeahClip;
    [SerializeField] AudioClip crowdClip;

    [SerializeField] AudioClip jumpClip;
    [SerializeField] AudioClip tinkerClip;
    [SerializeField] AudioClip turnOnClip;
    [SerializeField] AudioClip laughClip;
    [SerializeField] AudioClip jetpackClip;
    [SerializeField] AudioClip fireClip;
    [SerializeField] AudioClip exploClip;
    [SerializeField] AudioClip gaugeClip;
    [SerializeField] AudioClip popClip;
    [SerializeField] AudioClip fallClip;
    [SerializeField] AudioClip peaceClip;
    [SerializeField] AudioClip pauseClip;

    [SerializeField] AudioClip clayClip;
    [SerializeField] AudioClip clayBClip;
    [SerializeField] AudioClip timeClip;
    





    void Start()
    {
        countStart = FindObjectOfType<CountStart>();
        countStart.DisablePause();
        //activeDifficulty = "bgrLevel.LevelBeginner()";
        //activateBoss = true;
        destroyerHandler = destroyerHandler = GameObject.Find("Main Camera").GetComponent<DestroyerHandler>();
        backgroundHandler = FindObjectOfType<BackgroundHandler>();
        carryOvers = FindObjectOfType<CarryOvers>();
        backgroundHandler.ChooseYourBackground(carryOvers.GetBackgroundString());
        //destroyerHandler.EnableTouch();
        //adsscript = GameObject.Find("GoogleAds").GetComponent<AdsScript>();
        wordPacker = FindObjectOfType<NewWordPacker>();
        countStart = FindObjectOfType<CountStart>();
        spawnerDown = FindObjectOfType<SpawnerDown>();
        spawnerLeft = FindObjectOfType<SpawnerLeft>();
        spawnerRight = FindObjectOfType<SpawnerRight>();
        bgrLevel = FindObjectOfType<BGRLevel>();
        imtLevel = FindObjectOfType<IMTLevel>();
        advLevel = FindObjectOfType<ADVLevel>();
        timer = FindObjectOfType<Timer>();
        ddBackAnimUI = FindObjectOfType<DDBackAnimUI>();
        musicSource = GameObject.Find("Background Music").GetComponent<AudioSource>();
        sfxSource = GameObject.Find("Main Camera").GetComponent<AudioSource>();


        //NO OLVIDAR QUE AL TERMINAR LA PARTIDA DEBE NO DEJARTE EXPLOTAR MÁS COSAS MIENTRAS MIRAS EL SCORE

        //Usar Raycast de fondo para impedir tocar el resto de cosas al empezar partida!
        ddBackAnimUI.EnableBlock();

        //tkharbi9a para testeo
        //destroyerHandler.EnableTouch();

        speedHandler = FindObjectOfType<SpeedHandler>();
        speedHandler.WordBarDisable();




        displayWordText = GameObject.Find("Display Word Text Mesh Pro").GetComponent<TextMeshProUGUI>();
        scoreText = GameObject.Find("ScoreText").GetComponent<TextMeshProUGUI>();
        multiplierText = GameObject.Find("MultiplierText").GetComponent<TextMeshProUGUI>();
        multiplierBar = GameObject.Find("MultiplierBar").GetComponent<RectTransform>();
        clueButtons = GameObject.Find("ClueButtons").GetComponent<RectTransform>();
        wordBarText = GameObject.Find("WordBarText").GetComponent<TextMeshProUGUI>();
        wordBar = GameObject.Find("WordBar").GetComponent<RectTransform>();
        tv = GameObject.Find("TV");
        devilPosition = GameObject.Find("SpawnDevilRight").GetComponent<RectTransform>();
        //Debug.Log("the position where devil will spawn is " + devilPosition.localPosition);






        if (carryOvers.GetRightHanded() == false)
        {
            Vector2 tvNewPosition = new Vector2(-5.25f, 2.75f);
            tv.transform.position = tvNewPosition;
            Vector2 displayTextNewPosition = new Vector2(-525f, 45f);
            displayWordText.rectTransform.localPosition = displayTextNewPosition;
            Vector2 scoreTextNewPosition = new Vector2(-515f, -48f);
            scoreText.rectTransform.localPosition = scoreTextNewPosition;
            scoreText.alignment = TextAlignmentOptions.Left;
            Vector2 multiplierTextNewPosition = new Vector2(-395f, -128f);
            multiplierText.rectTransform.localPosition = multiplierTextNewPosition;
            multiplierText.alignment = TextAlignmentOptions.Left;
            Vector2 multiplierBarNewPosition = new Vector2(-880f, -225f);
            multiplierBar.localPosition = multiplierBarNewPosition;
            Vector2 clueButtonsNewPosition = new Vector2(-1425f, -4f);
            clueButtons.localPosition = clueButtonsNewPosition;
            Vector2 wordBarTextNewPosition = new Vector2(-595f, -430f);
            wordBarText.rectTransform.localPosition = wordBarTextNewPosition;
            Vector2 wordBarNewPosition = new Vector2(-905f, -500f);
            wordBar.localPosition = wordBarNewPosition;
            devilPosition = GameObject.Find("SpawnDevilLeft").GetComponent<RectTransform>();
        }


        //StartCoroutine(spawnerDown.TwoTwoFromLeft3());

        //if (carryOvers.GetActiveDifficulty() == "bgrLevel.LevelBeginner()")
        //{
        //    StartCoroutine(bgrLevel.LevelBeginner());
        //}

        //if (carryOvers.GetActiveDifficulty() == "imtLevel.LevelIntermediate()")
        //{
        //    StartCoroutine(imtLevel.LevelIntermediate());
        //}

        //if (carryOvers.GetActiveDifficulty() == "advLevel.LevelAdvanced()")
        //{
        //    StartCoroutine(advLevel.LevelAdvanced());
        //}

        //adsscript.ShowInterstitial(); //SHOWS INTERSTITIAL AD!

        //StartCoroutine(WaitThenStartCountDown()); TEST QUE HICE PARA QUE FUNCIONE ADS EN MOVIL
    }

    //MUY PROBABLE QUE META PALABRAS DE NIVEL INTERMEDIO EN BOSS Y QUE NO SEAN MUY LARGAS, EN AVANZADO METER PALABRAS LARGAS PARA BOSS
    //AUDIO FEEDBACK DE OBJETOS CUANDO SPAWNEAN

    // Update is called once per frame
    void Update()
    {
        if (timer.GetCurrentTimer() <=10)
        {
            countStart.DisablePause();
        }

        if (timer.GetCurrentTimer() > 10)
        {
            countStart.EnablePause();
        }

    }

    public void StartTheGame()
    {
        PlayMusic();
        ddBackAnimUI.DisableBlock();
        countStart.DisableBlock();
        destroyerHandler.EnableTouch();
        speedHandler.WordBarEnable();
        timer.EnableTimer();
        StartSpawning();

        
    }

    public void PlayMusic()
    {
        switch (PlayerPrefs.GetString("activeTextDifficulty"))
        {
            case "Easy":
                musicSource.clip = easyClip;
                break;
            case "Medium":
                musicSource.clip = mediumClip;
                break;
            case "Hard":
                musicSource.clip = hardClip;
                break;
            default:
                break;
        }

        musicSource.volume = PlayerPrefs.GetFloat("musicVolume");
        musicSource.Play();
        musicSource.loop = true;
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void PauseMusic()
    {
        musicSource.Pause();
       
    }

    public void UnPauseMusic()
    {
        musicSource.UnPause();
    }

    public void PlayBossMusic()
    {
        musicSource.clip = bossClip;
        musicSource.Play();
    }

    public void PlayDefeatMusic()
    {
        musicSource.loop = false;
        musicSource.clip = defeatClip;
        musicSource.Play();
    }

    public void PlayMusicSpecific(string str)
    {
        switch (str)
        {
            case "Easy":
                musicSource.clip = easyClip;
                break;
            case "Medium":
                musicSource.clip = mediumClip;
                break;
            case "Hard":
                musicSource.clip = hardClip;
                break;
            
            default:
                break;
        }

        musicSource.volume = PlayerPrefs.GetFloat("musicVolume");
        musicSource.Play();
        musicSource.loop = true;
    }

    public void PlaySFXOnce(string clip)
    {

        switch (clip)
        {
            case "jumpClip":
                sfxSource.clip = jumpClip;
                break;
            case "tinkerClip":
                sfxSource.clip = tinkerClip;
                sfxSource.loop = true;
                break;
            case "turnOnClip":
                sfxSource.loop = false;
                sfxSource.clip = turnOnClip;
                break;
            case "laughClip":
                sfxSource.loop = false;
                sfxSource.clip = laughClip;
                break;
            case "jetpackClip":
                sfxSource.clip = jetpackClip;
                break;
            case "fireClip":
                sfxSource.clip = fireClip;
                break;
            case "exploClip":
                sfxSource.clip = exploClip;
                sfxSource.loop = true;
                sfxSource.pitch = Random.Range(0.6f, 1.2f);
                break;
            case "gaugeClip":
                sfxSource.pitch = 1f;
                sfxSource.clip = gaugeClip;
                break;
            case "popClip":
                sfxSource.loop = false;
                sfxSource.clip = popClip;
                break;
            case "fallClip":
                sfxSource.clip = fallClip;
                break;
            case "peaceClip":
                sfxSource.clip = peaceClip;
                break;
            case "clayClip":
                sfxSource.PlayOneShot(clayClip);
                return;
            case "clayBClip":
                sfxSource.PlayOneShot(clayBClip);
                return;
            case "timeClip":
                sfxSource.PlayOneShot(timeClip);
                return;
            case "bulletClip":
                sfxSource.clip = bulletClip;
                sfxSource.Play();
                break;
                //sfxSource.PlayOneShot(bulletClip);
                //return;
            case "tickScoreClip":
                sfxSource.clip = tickScoreClip;
                break;
            case "hornClip":
                sfxSource.clip = hornClip;
                break;
            case "yeahClip":
                sfxSource.clip = yeahClip;
                break;
            case "noRecordClip":
                musicSource = GameObject.Find("Background Music").GetComponent<AudioSource>();
                musicSource.clip = noRecordClip;
                musicSource.loop = true;
                musicSource.Play();
                return;
            case "pauseClip":
                sfxSource.clip = pauseClip;
                break;
            case "crowdClip":
                sfxSource.PlayOneShot(crowdClip);
                return;
            case "welcomeClip":
                sfxSource.PlayOneShot(crowdClip);
                return;
            case "selectThemeClip":
                sfxSource.PlayOneShot(crowdClip);
                return;
            case "selectDiffClip":
                sfxSource.PlayOneShot(crowdClip);
                return;
            default:
                break;
        }

        sfxSource.Play();
    }

    public void StopSFX()
    {
        sfxSource.Stop();
    }



    private void StartSpawning()
    {
        if (carryOvers.GetActiveDifficulty() == "bgrLevel.LevelBeginner()")
        {
            StartCoroutine(bgrLevel.LevelBeginner());
        }

        if (carryOvers.GetActiveDifficulty() == "imtLevel.LevelIntermediate()")
        {
            StartCoroutine(imtLevel.LevelIntermediate());
        }

        if (carryOvers.GetActiveDifficulty() == "advLevel.LevelAdvanced()")
        {
            StartCoroutine(advLevel.LevelAdvanced());
        }
    }

    

    public IEnumerator CheckForBoss()
    {
        if (onGoingBoss == true)
        {
            yield return new WaitUntil(() => onGoingBoss == false);
        }
    }

    public IEnumerator HoldWaves()
    {

        if (GetOnGoingBoss() == true)
        {
            yield return new WaitUntil(() => GetOnGoingBoss() == false);
        }
    }

    public IEnumerator HoldWavesAfterWordCompletion()
    {
        //FALTA RELLENARSE PARA AÑADIRLA LUEGO
        yield return new WaitForSeconds(0f);
    }


    public bool GetActivateBoss()
    {
        return activateBoss;
    }

    public void DisableActivateBoss()
    {
        activateBoss = false;
    }

    public bool GetOnGoingBoss()
    {
        return onGoingBoss;
    }

    public void ActivateOnGoingBoss()
    {
        onGoingBoss = true;
    }

    public void DeactivateOnGoingBoss()
    {
        onGoingBoss = false;
    }

    public void ActivateBoss()
    {
       activateBoss = true;
    }

    public void SetAlive()
    {
        isAlive = true;
    }

    public bool IsAlive()
    {
        return isAlive;
    }

    public IEnumerator StopUntilDestroyed()
    {
        yield return new WaitUntil(() => isAlive == false);

        yield return null;
    }

    public void SetDestroyed()
    {
        isAlive = false;
    }

    public int GetRedCount()
    {
        return redCount;
    }

    public void ResetRedCount()
    {
        redCount = 0;
    }

    public void OneLessCount()
    {
        redCount--;
    }

    public void OneMoreCount()
    {
        redCount++;
    }

    public int GetTimeCount()
    {
        return timeCount;
    }

    public void ResetTimeCount()
    {
        timeCount = 0;
    }

    public void OneLessTimeCount()
    {
        timeCount--;
    }

    public void OneMoreTimeCount()
    {
        timeCount++;
    }


    public void RestartGame()
    {

    }

    public Vector2 GetDevilPosition()
    {
        //Debug.Log("devil spawned in position" + devilPosition.position);
        return devilPosition.position;
    }


    public IEnumerator WaitThenStartCountDown()
    {
        yield return new WaitForSeconds(0.5f);
        countStart = FindObjectOfType<CountStart>();
        StartCoroutine(countStart.CountDownAnim());
    }

}
