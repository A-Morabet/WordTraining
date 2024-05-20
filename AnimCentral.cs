using System.Collections;
using System.Collections.Generic;
//using UnityEditorInternal; me hace error de build
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class AnimCentral : MonoBehaviour
{
    //ScrollTitle2 scrollTitle2;
    ScrollBGCanvas scrollBGCanvas;
    SeparateBHandler separateBHandler;
    SceneLoader sceneLoader;
    CarryOvers carryOvers;
    DDBackAnimUI ddBackAnimUI;
    TransitionHandler transitionHandler;

    bool canPause;


    [SerializeField] GameObject toThemeCanvas;
    [SerializeField] GameObject toDiffCanvas;
    [SerializeField] GameObject toRevCanvas;
    [SerializeField] GameObject toOptionsCanvas;
    [SerializeField] GameObject toLeaderCanvas;




    [SerializeField] GameObject backThemeCanvas;
    [SerializeField] GameObject backDiffCanvas;
    [SerializeField] GameObject backRevCanvas;
    [SerializeField] GameObject backOptionsCanvas;
    [SerializeField] GameObject backLeaderCanvas;



    [SerializeField] Animator toThemeAnim;
    [SerializeField] Animator toThemeAnimCo;
    [SerializeField] AnimationClip toThemeClip;
    [SerializeField] AnimationClip toThemeCoClip;

    [SerializeField] Animator toDiffAnim;
    [SerializeField] Animator toDiffAnimCo;
    [SerializeField] AnimationClip toDiffClip;
    [SerializeField] AnimationClip toDiffCoClip;

    [SerializeField] Animator toRevAnim;
    [SerializeField] Animator toRevAnimCo;
    [SerializeField] AnimationClip toRevCoClip;
    [SerializeField] Animator themeAnim;
    [SerializeField] Animator diffAnim;
    [SerializeField] AnimationClip themeRevAppear;
    [SerializeField] AnimationClip diffRevAppear;

    [SerializeField] Animator toOptionsAnim;
    [SerializeField] Animator toOptionsAnimCo;
    [SerializeField] AnimationClip toOptionsCoClip;

    [SerializeField] Animator toLeaderAnim;
    [SerializeField] Animator toLeaderAnimCo;
    [SerializeField] AnimationClip toLeaderCoClip;


    [SerializeField] Animator backThemeAnim;
    [SerializeField] Animator backThemeAnimCo;
    [SerializeField] AnimationClip backThemeClip;
    [SerializeField] AnimationClip backThemeCoClip;

    [SerializeField] Animator backDiffAnim;
    [SerializeField] Animator backDiffAnimCo;
    [SerializeField] AnimationClip backDiffCoClip;

    [SerializeField] Animator backRevAnim;
    [SerializeField] Animator backRevAnimCo;
    [SerializeField] AnimationClip backRevCoClip;

    [SerializeField] Animator backOptionsAnim;
    [SerializeField] Animator backOptionsAnimCo;
    [SerializeField] AnimationClip backOptionsCoClip;

    [SerializeField] Animator backLeaderAnim;
    [SerializeField] Animator backLeaderAnimCo;
    [SerializeField] AnimationClip backLeaderCoClip;

    [SerializeField] AnimationClip startToTutorialClip;
    [SerializeField] AnimationClip startToTutorialBWClip;

    VideoPlayer videoPlayer;
    [SerializeField] Image pauseImage;
    [SerializeField] AudioClip pauseSound;

    [SerializeField] GameObject darkTutorialObject;
    [SerializeField] Image bgAnimImage;
    



    private void Awake()
    {
        
        
    }

    // Start is called before the first frame update
    void Start()
    {
        ddBackAnimUI = FindObjectOfType<DDBackAnimUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (canPause)
        {

            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                Vector3 pos = Camera.main.ScreenToWorldPoint(Input.touches[0].position);
                RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);
                //Debug.Log("I'm hitting " + hit.collider.name);
                if (hit.collider.name != "Leave Tutorial Button") 
                
                { 

                    if (!videoPlayer.isPaused)
                    {
                        videoPlayer.Pause();
                        AudioSource mainSource = GameObject.Find("Main Camera").GetComponent<AudioSource>();
                        mainSource.PlayOneShot(pauseSound);
                        //Debug.Log("video paused");
                        pauseImage.gameObject.SetActive(true);
                        return;
                    }

                    if (videoPlayer.isPaused)
                    {
                        videoPlayer.Play();
                        AudioSource mainSource = GameObject.Find("Main Camera").GetComponent<AudioSource>();
                        mainSource.PlayOneShot(pauseSound);
                        pauseImage.gameObject.SetActive(false);
                        //Debug.Log("video unpaused");
                        return;
                    }


                }

            }
        }
    }


    public IEnumerator AnimationCentral()
    {
        scrollBGCanvas = FindObjectOfType<ScrollBGCanvas>();
        separateBHandler = FindObjectOfType<SeparateBHandler>();

        scrollBGCanvas.PlayWhoosh();
        //if (scrollBGCanvas.ButtonClicked() == null)
        //{
        //    if (separateBHandler.ButtonClicked() == "Back To Menu Button")
        //    {
        //        Debug.Log("back to Start from Leader2");

        //        backLeaderCanvas.SetActive(true);
        //        backLeaderAnim.Play("BackLeader2");
        //        backLeaderAnimCo.Play("BackLeaderCo");
        //        yield return new WaitForSeconds(backLeaderCoClip.length);
        //        sceneLoader = GetComponent<SceneLoader>();
        //        sceneLoader.GoToStartEndScreen(0);
        //        yield break;
        //    }
        //}

        if (scrollBGCanvas.ButtonClicked() == "Start Game Button")
        {
            //Debug.Log("going from start to theme");
            toThemeCanvas.SetActive(true);
            transitionHandler = FindObjectOfType<TransitionHandler>();
            StartCoroutine(transitionHandler.PlayThemeAnnouncer());
            toThemeAnim.Play("StartToTheme");
            toThemeAnimCo.Play("StartToThemeCo");
            yield return new WaitForSeconds(toThemeCoClip.length);
            sceneLoader = GetComponent<SceneLoader>();
            sceneLoader.GoToTheme();
            ddBackAnimUI.DisableBlock();
            yield break;
        }

        if (scrollBGCanvas.ButtonClicked() == "Back Theme Button")
        {
            //Debug.Log("back from theme to start");
            backThemeCanvas.SetActive(true);
            backThemeAnim.Play("BackTheme");
            backThemeAnimCo.Play("BackThemeCo");
            yield return new WaitForSeconds(backThemeCoClip.length);
            sceneLoader = GetComponent<SceneLoader>();
            sceneLoader.GoToStart();
            ddBackAnimUI.DisableBlock();
            yield break;
        }

        if (scrollBGCanvas.ButtonClicked() == "Bathroom Button" | scrollBGCanvas.ButtonClicked() == "Bedroom Button" | scrollBGCanvas.ButtonClicked() == "Kitchen Button")
        {
            //Debug.Log("to difficulty screen");
            toDiffCanvas.SetActive(true);
            transitionHandler = FindObjectOfType<TransitionHandler>();
            StartCoroutine(transitionHandler.PlayDifficultyAnnouncer());
            toDiffAnim.Play("ThemeToDifficulty");
            toDiffAnimCo.Play("ThemeToDifficultyCo");
            yield return new WaitForSeconds(toDiffCoClip.length);
            sceneLoader = GetComponent<SceneLoader>();
            
            sceneLoader.GoToDifficulty();
            ddBackAnimUI.DisableBlock();
            yield break;
        }

        if (scrollBGCanvas.ButtonClicked() == "Back Difficulty Button")
        {
            //Debug.Log("back to theme from diff");
            backDiffCanvas.SetActive(true);
            backDiffAnim.Play("BackDifficulty");
            backDiffAnimCo.Play("BackDifficultyCo");
            yield return new WaitForSeconds(backDiffCoClip.length);
            sceneLoader = GetComponent<SceneLoader>();
            sceneLoader.GoToTheme();
            ddBackAnimUI.DisableBlock();
            yield break;
        }

        if (scrollBGCanvas.ButtonClicked() == "Beginner Button" | scrollBGCanvas.ButtonClicked() == "Intermediate Button" | scrollBGCanvas.ButtonClicked() == "Advanced Button")
        {
            //Debug.Log("to review screen");
            toRevCanvas.SetActive(true);
            toRevAnim.Play("DiffToReview");
            toRevAnimCo.Play("DiffToReviewCo");
            yield return new WaitForSeconds(toRevCoClip.length);
            sceneLoader = GetComponent<SceneLoader>();
            sceneLoader.GoToReview();
            ddBackAnimUI.DisableBlock();
            yield break;
            //sceneLoader = GetComponent<SceneLoader>();
            //yield return new WaitForSeconds(0.5f);
            //themeAnim.Play("ThemeRevAppear");
            //diffAnim.Play("DiffRevAppear");
        }

        if (scrollBGCanvas.ButtonClicked() == "Back Review Button")
        {
            //Debug.Log("back to diff from rev");
            backRevCanvas.SetActive(true);
            backRevAnim.Play("BackReview");
            backRevAnimCo.Play("BackReviewCo");
            yield return new WaitForSeconds(backRevCoClip.length);
            sceneLoader = GetComponent<SceneLoader>();
            sceneLoader.GoToDifficulty();
            ddBackAnimUI.DisableBlock();
            yield break;
        }

        if (scrollBGCanvas.ButtonClicked() == "Options Button")
        {
            //Debug.Log("back to start from opt");
            toOptionsCanvas.SetActive(true);
            toOptionsAnim.Play("StartToOptions");
            toOptionsAnimCo.Play("ToOptionsCo");
            yield return new WaitForSeconds(toOptionsCoClip.length);
            sceneLoader = GetComponent<SceneLoader>();
            sceneLoader.GoToOptions();
            ddBackAnimUI.DisableBlock();
            yield break;
        }

        if (scrollBGCanvas.ButtonClicked() == "Confirm Options Button")
        {

            //Debug.Log("back to start from opt");
            carryOvers = FindObjectOfType<CarryOvers>();
            carryOvers.SaveSettings();
            backOptionsCanvas.SetActive(true);
            backOptionsAnim.Play("BackOptions");
            backOptionsAnimCo.Play("BackOptionsCo");
            yield return new WaitForSeconds(backOptionsCoClip.length);
            sceneLoader = GetComponent<SceneLoader>();
            sceneLoader.GoToStartMenu();
            ddBackAnimUI.DisableBlock();
            yield break;
        }

        if (scrollBGCanvas.ButtonClicked() == "Leaderboard Button")
        {

            //Debug.Log("going to leaderboard");
            
            toLeaderCanvas.SetActive(true);
            toLeaderAnim.Play("StartToLeader");
            toLeaderAnimCo.Play("StartToLeaderCo");
            yield return new WaitForSeconds(toLeaderCoClip.length);
            sceneLoader = GetComponent<SceneLoader>();
            sceneLoader.GoToLeaderboard();
            ddBackAnimUI.DisableBlock();
            yield break;
        }

        if (scrollBGCanvas.ButtonClicked() == "Back To Menu")
        {
            //Debug.Log("back to Start from Leader");

            backLeaderCanvas.SetActive(true);
            backLeaderAnim.Play("BackLeader");
            backLeaderAnimCo.Play("BackLeaderCo");
            yield return new WaitForSeconds(backLeaderCoClip.length);
            sceneLoader = GetComponent<SceneLoader>();
            sceneLoader.GoToStart();
            ddBackAnimUI.DisableBlock();
            transitionHandler = FindObjectOfType<TransitionHandler>();
            transitionHandler.PlayMenuMusicPublic();
            yield break;
        }

        if (scrollBGCanvas.ButtonClicked() == "Back To Menu 2")
        {
            //Debug.Log("back to Start from Leader");

            backLeaderCanvas.SetActive(true);
            backLeaderAnim.Play("BackLeader");
            backLeaderAnimCo.Play("BackLeaderCo2");
            yield return new WaitForSeconds(backLeaderCoClip.length);
            sceneLoader = GetComponent<SceneLoader>();
            sceneLoader.GoToStart();
            ddBackAnimUI.DisableBlock();
            transitionHandler = FindObjectOfType<TransitionHandler>();
            transitionHandler.PlayMenuMusicPublic();
            yield break;
        }

        if (scrollBGCanvas.ButtonClicked() == "How To Play Button")
        {
            //Debug.Log("going to tutorial video");
            darkTutorialObject.SetActive(true);
            toThemeAnim.Play("StartToTutorial");
            yield return new WaitForSeconds(startToTutorialClip.length);
            videoPlayer = GameObject.Find("Video Player").GetComponent<VideoPlayer>();
            Camera mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
            videoPlayer.renderMode = VideoRenderMode.CameraFarPlane;
            videoPlayer.targetCamera = mainCamera;
            videoPlayer.Prepare();
            while (!videoPlayer.isPrepared)
            {
                yield return null;
                yield return new WaitForSeconds(0.1f);
                //Debug.Log("video tutorial is not yet prepared");
            }
            yield return new WaitForSeconds(0.35f);
            canPause = true;
            darkTutorialObject.SetActive(false);
            bgAnimImage = GameObject.Find("BackgroundAnim").GetComponent<Image>();
            bgAnimImage.enabled = false;
            videoPlayer.Play();
            ddBackAnimUI.DisableBlock();
            ulong videoFrame = (ulong)videoPlayer.frame;
            //Debug.Log("video framecount is " + videoPlayer.frameCount);
            while (videoFrame != 2169)
            {
                if (videoPlayer.isPlaying == false)
                {
                    yield break;
                }
                yield return null;
                yield return new WaitForSeconds(0.01f);
                //Debug.Log("video frame is " + videoFrame);
                videoFrame = (ulong)videoPlayer.frame;

            }
            //Debug.Log("got out of the while loop!!!");
            BackToMenu();


            

            yield break;
        }

        if (scrollBGCanvas.ButtonClicked() == "Leave Tutorial Button")
        {
            //Debug.Log("leaving tutorial");
            StopAllCoroutines();
            ddBackAnimUI.EnableBlock();
            videoPlayer = GameObject.Find("Video Player").GetComponent<VideoPlayer>();
            //videoPlayer.loopPointReached -= BackToMenu;
            videoPlayer.Stop();
            darkTutorialObject.SetActive(true);
            bgAnimImage.enabled = true;
            pauseImage.gameObject.SetActive(false);
            canPause = false;
            toThemeAnim.Play("StartToTutorialBW");
            yield return new WaitForSeconds(startToTutorialBWClip.length);
            ddBackAnimUI.DisableBlock();
            //Debug.Log("blargh");

            yield break;
        }

    }

    private void BackToMenu()
    {
        StartCoroutine(BackToMenuRoutine());
    }

    private IEnumerator BackToMenuRoutine()
    {
        videoPlayer = GameObject.Find("Video Player").GetComponent<VideoPlayer>();
        videoPlayer.Stop();
        darkTutorialObject.SetActive(true);
        bgAnimImage.enabled = true;
        canPause = false;
        ddBackAnimUI.EnableBlock();
        toThemeAnim.Play("StartToTutorialBW");
        yield return new WaitForSeconds(startToTutorialBWClip.length);
        ddBackAnimUI.DisableBlock();
        //Debug.Log("blargh");
    }
}
