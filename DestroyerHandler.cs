using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DestroyerHandler : MonoBehaviour
{
    // Script handles app session logic on prop destruction through touch controls and what ensues:
    // p.e. score increase, prop destruction animation, letter filled in hangman section, boss level activation...etc.


    // Variables for Scene Objects, destroyerHandler references itself so it's destroyed later in case there's duplicates.
    
    private static DestroyerHandler destroyerHandler;
    private CarryOvers carryOvers;
    NewWordPacker wordPacker;
    SpeedHandler speedHandler;
    ScoreHandler scoreHandler;
    ParticleHandler particleHandler;
    GameSession gameSession;
    SpawnerLeft spawnerLeft;
    SpawnerRight spawnerRight;

    // Props and associated audioclips.

    [SerializeField] GameObject balloonEffect;
    [SerializeField] AudioClip balloonBurst;
    [SerializeField] GameObject clayLightEffect;
    [SerializeField] AudioClip clayBurst;
    [SerializeField] AudioClip clayBrownBurst;
    [SerializeField] AudioClip clayRedBurst;
    [SerializeField] GameObject clayBrownEffect;
    [SerializeField] GameObject clayRedEffect;
    [SerializeField] AudioClip clayDamage;
    [SerializeField] GameObject clayBrownDestroyEffect;
    [SerializeField] GameObject clayRedDestroyEffect;
    [SerializeField] GameObject voidEffect;
    [SerializeField] AudioClip tvHit;
    [SerializeField] AudioClip tvFinalHit;
    [SerializeField] AudioClip testSound;
    [SerializeField] GameObject floatPlus;
    [SerializeField] GameObject floatMinus;
    [SerializeField] GameObject timeBurst;

    private float feedbackTime = 0.21f;

    AudioSource audioSource;

    // Scripts that dictate prop behaviour.

    private BalloonBehaviour balloon;
    private BalloonRedBehaviour balloonRed;
    private BalloonBlueBehaviour balloonBlue;
    private ClayBehaviour clay;
    private ClayBrownBehaviour clayBrown;
    private ClayRedBehaviour clayRed;
    private ShakeAnything shaker;

    // Timer and Boss.

    private Timer timer;
    private BossBehaviour bossTV;
    

    // Miscellaneous.

    private bool touchEnabled;
    private string destroyedLetter;
    private string currentHitTag; 
    private bool referencesSet;


    private void Awake()
    {
        // Duplicate delete.
    
        if (destroyerHandler == null)
        {
            destroyerHandler = this;
            DontDestroyOnLoad(destroyerHandler);
        }

        else
        {
            Destroy(gameObject);
        }

    }

    void Start()
    {
        // Sets up references to variables.
        
        carryOvers = FindObjectOfType<CarryOvers>();
        wordPacker = FindObjectOfType<NewWordPacker>();
        
        speedHandler = FindObjectOfType<SpeedHandler>();
        gameSession = FindObjectOfType<GameSession>();
        spawnerLeft = FindObjectOfType<SpawnerLeft>();
        spawnerRight = FindObjectOfType<SpawnerRight>();
        audioSource = GameObject.Find("Main Camera").GetComponent<AudioSource>();
        timer = FindObjectOfType<Timer>();
        currentHitTag = "";
        
        
    }

    
    void Update()
    {
        // Main function that handles destruction logic for all props during app session.
        // Determines hit collider from touch controls and proceeds to apply specific prop logic.
    
        if ( touchEnabled )
        {
            if (!referencesSet){
            // When touch is enabled references are set up again and bool prevents it to be loaded each frame
            
            wordPacker = FindObjectOfType<NewWordPacker>();
            particleHandler = FindObjectOfType<ParticleHandler>();
            speedHandler = FindObjectOfType<SpeedHandler>();
            gameSession = FindObjectOfType<GameSession>();
            spawnerLeft = FindObjectOfType<SpawnerLeft>();
            spawnerRight = FindObjectOfType<SpawnerRight>();
            scoreHandler = FindObjectOfType<ScoreHandler>();
            audioSource = GameObject.Find("Main Camera").GetComponent<AudioSource>();
            timer = FindObjectOfType<Timer>();

            wordPacker.setPackerReferences();

            referencesSet = true;
            }

            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {

            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.touches[0].position);
            RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);
            Vector3 voidPosition = new Vector3(pos.x, pos.y, 0);
            if (hit != null && hit.collider != null)
            {
                //Debug.Log("I'm hitting " + hit.collider.name);
                if (hit.transform.tag == "Balloon")
                {
                        currentHitTag = hit.transform.tag;
                        carryOvers.TransferTagDH();

                        Instantiate(balloonEffect, hit.transform.position, Quaternion.identity);/*.GetComponent<Animator>().SetFloat("speedMulti", speedHandler.ChangeAnimationSpeed());*/
                    
                    balloon = hit.transform.GetComponent<BalloonBehaviour>();
                    
                    destroyedLetter = balloon.textLetter.text;
                    carryOvers.TransferLetterDH();

                    //Debug.Log("The letter destroyed is " + balloon.textLetter.text);
                    audioSource.PlayOneShot(balloonBurst);
                    
                    particleHandler.SpawnParticles("Balloon", hit.transform.position);
                    Destroy(hit.transform.gameObject);
                    
                    wordPacker.CompareDestroyedLetter();
                    GimmeScore(hit.transform.position);

                        return;
                }
                if (hit.transform.tag == "BalloonRed")
                {

                    Instantiate(balloonEffect, hit.transform.position, Quaternion.identity);
                    balloonRed = hit.transform.GetComponent<BalloonRedBehaviour>();
                    //Debug.Log("I accessed the script");
                    destroyedLetter = balloonRed.textLetter.text;
                    carryOvers.TransferLetterDH();
                    currentHitTag = hit.transform.tag;
                    carryOvers.TransferTagDH();

                    //Debug.Log("The letter destroyed is " + balloonRed.textLetter.text);
                    audioSource.PlayOneShot(balloonBurst);
                    Destroy(hit.transform.gameObject);
                    particleHandler.SpawnParticles("BalloonRed", hit.transform.position);
                    GimmeScore(hit.transform.position);
                    wordPacker.CompareDestroyedLetter();
                        
                        return;
                    }
                if (hit.transform.tag == "BalloonBlue")
                {

                    Instantiate(balloonEffect, hit.transform.position, Quaternion.identity);
                    balloonBlue = hit.transform.GetComponent<BalloonBlueBehaviour>();
                    //Debug.Log("I accessed the script");
                    destroyedLetter = balloonBlue.textLetter.text;
                    carryOvers.TransferLetterDH();
                    currentHitTag = hit.transform.tag;
                    carryOvers.TransferTagDH();
                    //Debug.Log("The letter destroyed is " + balloonBlue.textLetter.text);
                    audioSource.PlayOneShot(balloonBurst);
                    Destroy(hit.transform.gameObject);
                    particleHandler.SpawnParticles("BalloonBlue", hit.transform.position);
                    GimmeScore(hit.transform.position);
                    wordPacker.CompareDestroyedLetter();
                        
                        return;
                    }
                if (hit.transform.tag == "Clay")
                {

                    //Instantiate(balloonEffect, hit.transform.position, Quaternion.identity);
                    clay = hit.transform.GetComponent<ClayBehaviour>();
                    destroyedLetter = clay.textLetter.text;
                    carryOvers.TransferLetterDH();
                    currentHitTag = hit.transform.tag;
                    carryOvers.TransferTagDH();
                    //Debug.Log("The letter destroyed is " + clay.textLetter.text);
                    audioSource.PlayOneShot(clayBurst);
                    particleHandler.SpawnParticles("Clay", hit.transform.position);
                    Destroy(hit.transform.gameObject);
                    GimmeScore(hit.transform.position);
                    wordPacker.CompareDestroyedLetter();
                        
                        return;

                    }
                if (hit.transform.tag == "ClayBrown")
                {
                    //Clay props move the opposite side on hit collision.
                    
                    clayBrown = hit.transform.GetComponent<ClayBrownBehaviour>();

                    if (hit.collider.name == "ClayBrownColliderRight" && clayBrown.GetDamageCount() < 3)
                    {
                        audioSource.PlayOneShot(clayDamage);
                        shaker = hit.transform.GetComponent<ShakeAnything>();
                        shaker.ShakeClayBrown();
                        particleHandler.SpawnParticles("ClayBrown", hit.transform.position);
                        clayBrown.FractureClayLeft();
                            return;
                    }

                    if (hit.collider.name == "ClayBrownColliderLeft" && clayBrown.GetDamageCount() < 3)
                    {
                        audioSource.PlayOneShot(clayDamage);
                        shaker = hit.transform.GetComponent<ShakeAnything>();
                        shaker.ShakeClayBrown();
                        particleHandler.SpawnParticles("ClayBrown", hit.transform.position);
                        clayBrown.FractureClayRight();
                            return;
                    }

                    if (clayBrown.GetDamageCount() == 3)
                    {
                        audioSource.PlayOneShot(clayBrownBurst);
                        destroyedLetter = clayBrown.textLetter.text;
                        carryOvers.TransferLetterDH();
                        currentHitTag = hit.transform.tag;
                        carryOvers.TransferTagDH();
                        //Debug.Log("The letter destroyed is " + clayBrown.textLetter.text);
                        particleHandler.SpawnParticles("ClayBrownDestroyed", hit.transform.position);
                        Destroy(hit.transform.gameObject);
                        GimmeScore(hit.transform.position);
                        wordPacker.CompareDestroyedLetter();
                            
                        }
                        return;
                    }

                    if (hit.transform.tag == "ClayRed")
                    {

                        Instantiate(balloonEffect, hit.transform.position, Quaternion.identity);
                        clayRed = hit.transform.GetComponent<ClayRedBehaviour>();

                        if (hit.collider.name == "ClayRedColliderRight" && clayRed.GetDamageCount() < 5)
                        {
                            audioSource.PlayOneShot(clayDamage);
                            shaker = hit.transform.GetComponent<ShakeAnything>();
                            shaker.ShakeClayRed();
                            particleHandler.SpawnParticles("ClayRed", hit.transform.position);
                            clayRed.FractureClayLeft();
                            return;
                        }

                        if (hit.collider.name == "ClayRedColliderLeft" && clayRed.GetDamageCount() < 5)
                        {
                            audioSource.PlayOneShot(clayDamage);
                            shaker = hit.transform.GetComponent<ShakeAnything>();
                            shaker.ShakeClayRed();
                            particleHandler.SpawnParticles("ClayRed", hit.transform.position);
                            clayRed.FractureClayRight();
                            return;
                        }

                        if (clayRed.GetDamageCount() == 5)
                        {
                            audioSource.PlayOneShot(clayRedBurst);
                            destroyedLetter = clayRed.textLetter.text;
                            carryOvers.TransferLetterDH();
                            currentHitTag = hit.transform.tag;
                            carryOvers.TransferTagDH();
                            //Debug.Log("The letter destroyed is " + clayRed.textLetter.text);
                            gameSession.OneLessCount();
                            if (gameSession.GetRedCount() <= 0)
                            {
                                gameSession.SetDestroyed();
                                gameSession.ResetRedCount();
                                
                            }
                            particleHandler.SpawnParticles("ClayRedDestroyed", hit.transform.position);
                            Destroy(hit.transform.gameObject);
                            GimmeScore(hit.transform.position);
                            wordPacker.CompareDestroyedLetter();
                            
                        }
                        return;
                    }

                    if (hit.transform.tag == "PowerUpTimer")
                    {
                        // Increases time on hit.

                        Instantiate(balloonEffect, hit.transform.position, Quaternion.identity);
                        Instantiate(timeBurst, hit.transform.position, Quaternion.identity);
                        gameSession.PlaySFXOnce("timeClip");
                        
                        currentHitTag = hit.transform.tag;
                        carryOvers.TransferTagDH();
                        timer.IncreaseTimer();
                        
                        Destroy(hit.transform.gameObject);
                        return;
                        

                    }

                    if (hit.transform.tag == "BossTV" && gameSession.GetOnGoingBoss() == true)
                    {

                        // Handles boss hit collision

                        Instantiate(balloonEffect, hit.transform.position, Quaternion.identity);
                        bossTV = GameObject.Find("TV").GetComponent<BossBehaviour>();
                        destroyedLetter = bossTV.GetTVLetter();
                        carryOvers.TransferLetterDH();
                        //Debug.Log("The letter destroyed is " + destroyedLetter);
                        audioSource.PlayOneShot(tvHit);
                        shaker = hit.transform.parent.GetComponent<ShakeAnything>();
                        shaker.ShakeTV();
                        if (bossTV.GetResetWait() == false) { bossTV.EnableWait(); }
                        currentHitTag = hit.transform.tag;
                        carryOvers.TransferTagDH();
                        wordPacker.CompareDestroyedLetter();

                        GimmeScore(hit.transform.position);

                        return;
                    }

                    if (hit.transform.tag != "BossTV" && hit.transform.tag != "PowerUpTimer" && hit.transform.tag != "ClayBrown" && hit.transform.tag != "ClayRed" && hit.transform.tag != "Clay" && hit.transform.tag != "BalloonRed" && hit.transform.tag != "BalloonBlue" && hit.transform.tag != "Balloon" && hit.transform.tag != "LetterClueButton")
                    {

                        Instantiate(voidEffect, voidPosition, Quaternion.identity).GetComponent<Animator>();/*.SetFloat("speedMulti", speedHandler.ChangeAnimationSpeed());*/
                        //Debug.Log("Nothing here");
                        return;
                    }
            }          

        }
        }
    }

    

    public string GetDestroyedLetter()
    {
        return destroyedLetter;
    }

    public string GetDestroyedTag()
    {
        //Debug.Log("the destroyed tag is " + currentHitTag);
        return currentHitTag;
    }

    public void EnableTouch()
    {
        touchEnabled = true;
    }

    public void DisableTouch()
    {    
    // Disables touch controls and readies references to be set up again.
    
        touchEnabled = false;
        referencesSet = false;
    }

    public AudioClip GetTestSound()
    {
        return testSound;
    }

    private void GimmeScore(Vector3 hitPosition)
    {
        StartCoroutine(GimmeTheScore(hitPosition));
        
    }

    private IEnumerator GimmeTheScore(Vector3 hitPosition)
    {
        // Waits for score to be processed from "scoreHandler" script and receives it,
        // determines if score is positive or negative, and then modifies accordingly.
    
        int scoreToShow = 0;
        yield return new WaitUntil(() => scoreHandler.getSendIt() == true);
        scoreToShow = scoreHandler.SendScore();
        //Debug.Log("getSendIt became true! the score to show is " + scoreToShow);
        //Debug.Log("the score to show is " + scoreToShow);
        if (scoreToShow > 0)
        {
            GameObject floatScoreObject = Instantiate(floatPlus, hitPosition + new Vector3(0f,0.6f), transform.rotation);
            TextMeshProUGUI floatScoreText = floatScoreObject.GetComponentInChildren<TextMeshProUGUI>();
            floatScoreText.text = "+" + scoreToShow.ToString();

            floatScoreObject.transform.SetParent(GameObject.Find("Game Scene Canvas").GetComponent<Transform>());
            floatScoreObject.transform.SetAsFirstSibling();
            floatScoreObject.transform.localScale = new Vector3(1.5f, 1.5f);
            if (wordPacker.GetGimmeScoreBoss()) { floatScoreObject.transform.localScale = new Vector3(4f, 4f); }
            yield break;
        }
        if (scoreToShow < 0)
        {
            GameObject floatScoreObject = Instantiate(floatMinus, hitPosition + new Vector3(0f, 0.6f), transform.rotation);
            TextMeshProUGUI floatScoreText = floatScoreObject.GetComponentInChildren<TextMeshProUGUI>();
            floatScoreText.text = scoreToShow.ToString();

            floatScoreObject.transform.SetParent(GameObject.Find("Game Scene Canvas").GetComponent<Transform>());
            floatScoreObject.transform.SetAsFirstSibling();
            floatScoreObject.transform.localScale = new Vector3(1.5f, 1.5f);
            yield break;
        }

        
        
    }

    public bool GetTouchState()
    {
        return touchEnabled;
    }
}

