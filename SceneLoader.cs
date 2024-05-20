using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Runtime.CompilerServices;

// Script handles setup all references and variables of all Unity Scenes.

public class SceneLoader : MonoBehaviour
{
    // Setting up variables needed.

    // Scene Objects.
    
    CarryOvers carryOvers;
    ScrollBGCanvas scrollBGCanvas;
    DDBackAnimUI backgroundHandler;
    TransitionHandler transitionHandler;
    Camera mainCamera;
    GameObject bgNoirSprite;
    Canvas currentCanvas;
    Canvas assocCanvas;
    GameObject[] assocCanvasArr;

    // Options and Leaderboard menu.

    Button optionsConfirmButton;
    Button leaderConfirmButton;
    Button leader2ConfirmButton;

    // Theme Menu.

    Button bedroomButton;
    Button bathroomButton;
    Button kitchenButton;
    Button backThemeButton;

    // Difficulty Menu.

    Button beginnerButton;
    Button intermediateButton;
    Button advancedButton;
    Button backDiffButton;

    // Starter and Choice Menu.

    Button backRevButton;
    TextMeshProUGUI themeChoice;
    TextMeshProUGUI diffChoice;
    Button startGameButton;
    Button optionsButton;
    Button leaderButton;
    Button coinsButton;
    Button creditsButton;
    Button startLevelButton;
    Button howToPlayButton;
    Button leaveTutorialButton;
    Button backfromCreditsButton;
    
    // Exit level menu.
    
    Button exitLevelButton;
    
    [SerializeField] Sprite grayMenuButton;
    [SerializeField] Sprite grayCoinButton;

    TextMeshProUGUI coinNumberText;


     
    
    public void Awake()
    {
        // Setting up initial references.
        
        carryOvers = GameObject.Find("CarryOvers").GetComponent<CarryOvers>();
        scrollBGCanvas = FindObjectOfType<ScrollBGCanvas>();
        backgroundHandler = FindObjectOfType<DDBackAnimUI>();
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        transitionHandler = FindObjectOfType<TransitionHandler>();


        // On Script "Awake State" gets Scene index from script "carryOvers" and sets references according to that scene.

        if (carryOvers.GetCurrentScene() == 0)
        {
            if (transitionHandler.GetCanIntro())
            {
                // Game Intro
                DestroyAllDontDestroyOnLoadObjects();
                StartCoroutine(StartFromIntro());
                transitionHandler.DisableIntro();
                return;
            }
            currentCanvas = GameObject.FindGameObjectWithTag("CurrentCanvas").GetComponent<Canvas>();
            
            currentCanvas.worldCamera = mainCamera;
            bgNoirSprite = GameObject.Find("BGNoir").gameObject;
            
            if (bgNoirSprite != null)
            { 
            bgNoirSprite.SetActive(false);
            }

            assocCanvasArr = GameObject.FindGameObjectsWithTag("AssocCanvas");

            for (int i = 0; i < assocCanvasArr.Length; i++)
            {
                // Establishes world camera for all involved canvases in scene.
            
                assocCanvas = assocCanvasArr[i].GetComponent<Canvas>();
                assocCanvas.worldCamera = mainCamera;
            }

                // Add event listeners for all menu buttons through script, which prevents it from being cleared on scene change.

            startGameButton = GameObject.Find("Start Game Button").GetComponent<Button>();
            startGameButton.onClick.AddListener(carryOvers.SetSceneOne);
            startGameButton.onClick.AddListener(delegate { scrollBGCanvas.ButtonAnimation(startGameButton.gameObject); });
            
            optionsButton = GameObject.Find("Options Button").GetComponent<Button>();
            optionsButton.onClick.AddListener(delegate { scrollBGCanvas.ButtonAnimation(optionsButton.gameObject); });
            
            leaderButton = GameObject.Find("Leaderboard Button").GetComponent<Button>();
            leaderButton.onClick.AddListener(delegate { scrollBGCanvas.ButtonAnimation(leaderButton.gameObject); });
            
            coinsButton = GameObject.Find("Get Game Coins Button").GetComponent<Button>();
            coinsButton.onClick.AddListener(delegate { scrollBGCanvas.ButtonAnimation(coinsButton.gameObject); });
            coinsButton.onClick.AddListener(delegate { transitionHandler.GoToCoinsFromMenu(); });
            
            creditsButton = GameObject.Find("Credits Button").GetComponent<Button>();
            creditsButton.onClick.AddListener(delegate { scrollBGCanvas.ButtonAnimation(creditsButton.gameObject); });
            creditsButton.onClick.AddListener(delegate { transitionHandler.GoToCreditsFromMenu(); });
            
            howToPlayButton = GameObject.Find("How To Play Button").GetComponent<Button>();
            howToPlayButton.onClick.AddListener(delegate { scrollBGCanvas.ButtonAnimation(howToPlayButton.gameObject); });
            
            leaveTutorialButton = GameObject.Find("Leave Tutorial Button").GetComponent<Button>();
            leaveTutorialButton.onClick.AddListener(delegate { scrollBGCanvas.ButtonAnimation(leaveTutorialButton.gameObject); });

        }

            // Same procedure as previous for the rest of the scenes.

        if (carryOvers.GetCurrentScene() == 1)
        {
            currentCanvas = GameObject.FindGameObjectWithTag("CurrentCanvas").GetComponent<Canvas>();
            currentCanvas.worldCamera = mainCamera;

            assocCanvasArr = GameObject.FindGameObjectsWithTag("AssocCanvas");

            for (int i = 0; i < assocCanvasArr.Length; i++)
            {
                assocCanvas = assocCanvasArr[i].GetComponent<Canvas>();
                assocCanvas.worldCamera = mainCamera;
            }

            bathroomButton = GameObject.Find("Bathroom Button").GetComponent<Button>();
            bathroomButton.onClick.AddListener(carryOvers.AddBathroom);
            bathroomButton.onClick.AddListener(delegate { scrollBGCanvas.ButtonAnimation(bathroomButton.gameObject); });
            
            bedroomButton = GameObject.Find("Bedroom Button").GetComponent<Button>();
            bedroomButton.onClick.AddListener(carryOvers.AddBedroom);
            bedroomButton.onClick.AddListener(delegate { scrollBGCanvas.ButtonAnimation(bedroomButton.gameObject); });
            
            kitchenButton = GameObject.Find("Kitchen Button").GetComponent<Button>();
            kitchenButton.onClick.AddListener(carryOvers.AddKitchen);
            kitchenButton.onClick.AddListener(delegate { scrollBGCanvas.ButtonAnimation(kitchenButton.gameObject); });
            
            backThemeButton = GameObject.Find("Back Theme Button").GetComponent<Button>();
            backThemeButton.onClick.AddListener(carryOvers.BackErase);
            backThemeButton.onClick.AddListener(delegate { scrollBGCanvas.ButtonAnimation(backThemeButton.gameObject); });
        }

        if (carryOvers.GetCurrentScene() == 2)
        {
            currentCanvas = GameObject.FindGameObjectWithTag("CurrentCanvas").GetComponent<Canvas>();
            currentCanvas.worldCamera = mainCamera;

            assocCanvasArr = GameObject.FindGameObjectsWithTag("AssocCanvas");

            for (int i = 0; i < assocCanvasArr.Length; i++)
            {
                assocCanvas = assocCanvasArr[i].GetComponent<Canvas>();
                assocCanvas.worldCamera = mainCamera;
            }

            beginnerButton = GameObject.Find("Beginner Button").GetComponent<Button>();
            beginnerButton.onClick.AddListener(carryOvers.AddBeginner);
            beginnerButton.onClick.AddListener(delegate { scrollBGCanvas.ButtonAnimation(beginnerButton.gameObject); });
            
            intermediateButton = GameObject.Find("Intermediate Button").GetComponent<Button>();
            intermediateButton.onClick.AddListener(carryOvers.AddIntermediate);
            intermediateButton.onClick.AddListener(delegate { scrollBGCanvas.ButtonAnimation(intermediateButton.gameObject); });
            
            advancedButton = GameObject.Find("Advanced Button").GetComponent<Button>();
            advancedButton.onClick.AddListener(carryOvers.AddAdvanced);
            advancedButton.onClick.AddListener(delegate { scrollBGCanvas.ButtonAnimation(advancedButton.gameObject); });
            
            backDiffButton = GameObject.Find("Back Difficulty Button").GetComponent<Button>();
            backDiffButton.onClick.AddListener(carryOvers.BackErase);
            backDiffButton.onClick.AddListener(delegate { scrollBGCanvas.ButtonAnimation(backDiffButton.gameObject); });
        }

        if (carryOvers.GetCurrentScene() == 3)
        {
            currentCanvas = GameObject.FindGameObjectWithTag("CurrentCanvas").GetComponent<Canvas>();
            currentCanvas.worldCamera = mainCamera;

            assocCanvasArr = GameObject.FindGameObjectsWithTag("AssocCanvas");

            for (int i = 0; i < assocCanvasArr.Length; i++)
            {
                assocCanvas = assocCanvasArr[i].GetComponent<Canvas>();
                assocCanvas.worldCamera = mainCamera;
            }

            startLevelButton = GameObject.Find("Start Level Button").GetComponent<Button>();

            themeChoice = GameObject.Find("Theme Choice").GetComponent<TextMeshProUGUI>();
            diffChoice = GameObject.Find("Difficulty Choice").GetComponent<TextMeshProUGUI>();
            
            themeChoice.text = "Theme: " + carryOvers.GetTheme();
            diffChoice.text = "Word Difficulty: " + carryOvers.GetDiff();

            backRevButton = GameObject.Find("Back Review Button").GetComponent<Button>();
            backRevButton.onClick.AddListener(carryOvers.BackEraseReview);
            backRevButton.onClick.AddListener(delegate { scrollBGCanvas.ButtonAnimation(backRevButton.gameObject); });

         
            startLevelButton.onClick.AddListener(delegate { scrollBGCanvas.ButtonAnimation(startLevelButton.gameObject); });
            startLevelButton.onClick.AddListener(delegate { transitionHandler.GoToGameFromReview(); });
           
        }

        if (carryOvers.GetCurrentScene() == 4)
        {
            currentCanvas = GameObject.FindGameObjectWithTag("CurrentCanvas").GetComponent<Canvas>();
            currentCanvas.worldCamera = mainCamera;

        }

        if (carryOvers.GetCurrentScene() == 6)
        {

            currentCanvas = GameObject.FindGameObjectWithTag("CurrentCanvas").GetComponent<Canvas>();
            currentCanvas.worldCamera = mainCamera;

            assocCanvasArr = GameObject.FindGameObjectsWithTag("AssocCanvas");

            for (int i = 0; i < assocCanvasArr.Length; i++)
            {
                assocCanvas = assocCanvasArr[i].GetComponent<Canvas>();
                assocCanvas.worldCamera = mainCamera;
            }

            carryOvers.SetOptionsReferences();
            carryOvers.LoadSettingsAtOptions();
            
            optionsConfirmButton = GameObject.Find("Confirm Options Button").GetComponent<Button>();
            optionsConfirmButton.onClick.AddListener(delegate { scrollBGCanvas.ButtonAnimation(optionsConfirmButton.gameObject); });

        }

        if (carryOvers.GetCurrentScene() == 7)
        {
            currentCanvas = GameObject.FindGameObjectWithTag("CurrentCanvas").GetComponent<Canvas>();
            currentCanvas.worldCamera = mainCamera;

        }

        if (carryOvers.GetCurrentScene() == 8)
        {
            currentCanvas = GameObject.FindGameObjectWithTag("CurrentCanvas").GetComponent<Canvas>();
            currentCanvas.worldCamera = mainCamera;

            assocCanvasArr = GameObject.FindGameObjectsWithTag("AssocCanvas");

            for (int i = 0; i < assocCanvasArr.Length; i++)
            {
                assocCanvas = assocCanvasArr[i].GetComponent<Canvas>();
                assocCanvas.worldCamera = mainCamera;
            }
            
            leaderConfirmButton = GameObject.Find("Back To Menu 2").GetComponent<Button>();
            leaderConfirmButton.onClick.AddListener(delegate { scrollBGCanvas.ButtonAnimation(leaderConfirmButton.gameObject); });

        }

        if (carryOvers.GetCurrentScene() == 9)
        {
            backgroundHandler.ActivateBackground();
            
            currentCanvas = GameObject.FindGameObjectWithTag("CurrentCanvas").GetComponent<Canvas>();
            currentCanvas.worldCamera = mainCamera;

            assocCanvasArr = GameObject.FindGameObjectsWithTag("AssocCanvas");

            for (int i = 0; i < assocCanvasArr.Length; i++)
            {
                assocCanvas = assocCanvasArr[i].GetComponent<Canvas>();
                assocCanvas.worldCamera = mainCamera;
            }
            
        }

        if (carryOvers.GetCurrentScene() == 11)
        {
            currentCanvas = GameObject.FindGameObjectWithTag("CurrentCanvas").GetComponent<Canvas>();
            currentCanvas.worldCamera = mainCamera;

            backfromCreditsButton = GameObject.Find("Back from Credits").GetComponent<Button>();

        }

    }

    private void Start()
    
    {
        // Gets saved coins from script "carryOvers", and determines if Coin Button is enabled or disabled.
    
        if (carryOvers.GetCurrentScene() == 0)
        {
            Button getCoins = GameObject.Find("Get Game Coins Button").GetComponent<Button>();
            GetCoins();
            if (carryOvers.passItOverCoins() > 0)
            {
                getCoins.enabled = false;
            }


            if (getCoins.enabled == true)
            {
                Animator coinsButtonAnimator = getCoins.gameObject.GetComponent<Animator>();
                coinsButtonAnimator.Play("CoinGlow");
            }
        }
    }

    public void SetListenerButton(Button buttonToSet)
    {
        buttonToSet.onClick.AddListener(delegate { scrollBGCanvas.ButtonAnimation(buttonToSet.gameObject); });
        //Debug.Log("SceneLoader_listener added to button");
    }

    private void GetCoins()
    {
        coinNumberText = GameObject.Find("Coin Text").GetComponent<TextMeshProUGUI>();
        coinNumberText.text = "= " + carryOvers.passItOverCoins();

        CompareCoins();

    }

     // If coins are equal to 0, game can't be started until additional coins are obtained

    private void CompareCoins()
    {
        Image startGameImage = GameObject.Find("Start Game Button").GetComponent<Image>();
        startGameButton = GameObject.Find("Start Game Button").GetComponent<Button>();
        coinsButton = GameObject.Find("Get Game Coins Button").GetComponent<Button>();
        Image gameCoinsImage = GameObject.Find("Get Game Coins Button").GetComponent<Image>();

        if (carryOvers.passItOverCoins() == 0)
        {
            startGameImage.sprite = grayMenuButton;
            startGameButton.interactable = false;
        }
        else if(carryOvers.passItOverCoins() > 0)
        {
            gameCoinsImage.sprite = grayCoinButton;
            coinsButton.interactable = false;
        }
        else if (carryOvers.passItOverCoins() < 0)
        {
            coinNumberText.text = "0";
            startGameImage.sprite = grayMenuButton;
            startGameButton.interactable = false;
        }
    }

    public void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + 1);
    }

    // Following functions are called to switch from one scene to another.

    public void GoToStart()
    {
        carryOvers = GameObject.Find("CarryOvers").GetComponent<CarryOvers>();
        SceneManager.LoadScene(0);
        carryOvers.ResetCurrentScene();
        carryOvers.ClearChoiceString();
    }

    public void GoToTheme()
    {
        SceneManager.LoadScene(1);
    }

    public void GoToDifficulty()
    {
        SceneManager.LoadScene(2);

    }

    public void GoToReview()
    {
        SceneManager.LoadScene(3);

    }

    public void GoToGame()
    {
        carryOvers.SubstractCoin();
        backgroundHandler.DeactivateBackground();
        SceneManager.LoadScene(4);
        carryOvers.SetGameScene();
    }

    public void GoToOptions()
    {
        carryOvers = GameObject.Find("CarryOvers").GetComponent<CarryOvers>();
        SceneManager.LoadScene(6);
        carryOvers.SetOptionsScene();
        //Debug.Log("the current scene is " + carryOvers.GetCurrentScene());
        
    }

    public void GoToLeaderboard()
    {
        carryOvers = GameObject.Find("CarryOvers").GetComponent<CarryOvers>();
        SceneManager.LoadScene(8);
        carryOvers.SetLeaderboardScene();
        
        //Debug.Log("the current scene is " + carryOvers.GetCurrentScene());

    }

    public void GoToLeaderboard2()
    {
        carryOvers = GameObject.Find("CarryOvers").GetComponent<CarryOvers>();
        SceneManager.LoadScene(9);
        carryOvers.SetLeaderboard2Scene();
        //Debug.Log("the current scene is " + carryOvers.GetCurrentScene());
        
    }

    public void GoToGameCoins()
    {
        carryOvers = GameObject.Find("CarryOvers").GetComponent<CarryOvers>();
        SceneManager.LoadScene(7);
        carryOvers.SetGameCoinsScene();
        GameObject.Find("BackgroundAnim").SetActive(false);
        //Debug.Log("the current scene is " + carryOvers.GetCurrentScene());

    }

    public void GoToStartMenu()
    {
        //carryOvers = GameObject.Find("CarryOvers").GetComponent<CarryOvers>();
        //carryOvers.SaveSettings();
        SceneManager.LoadScene(0);
        carryOvers.ResetCurrentScene();
        backgroundHandler.ActivateBackground();
        //Debug.Log("the current scene is " + carryOvers.GetCurrentScene());

    }

    public void GoToStartMenu2()
    {
        
        carryOvers.SaveCoinsOnGet();
        SceneManager.LoadScene(0);
        backgroundHandler.ActivateBackground();
        carryOvers.ResetCurrentScene();
        //Debug.Log("the current scene is " + carryOvers.GetCurrentScene());

    }

    public void GoToCredits()
    {
        SceneManager.LoadScene(11);
        carryOvers.SetCreditsScene();
        backgroundHandler.DeactivateBackground();
        
        //Debug.Log("the current scene is " + carryOvers.GetCurrentScene());

    }

    public void GoToStartEndScreen(int SceneID)
    {
        carryOvers = GameObject.Find("CarryOvers").GetComponent<CarryOvers>();
        //carryOvers.SetOptionsReferences();
        //carryOvers.SaveSettingsAtGameOptions();
        Time.timeScale = 1.0f;
        //Debug.Log("I exited Menu");
        SceneManager.LoadScene(SceneID);
        backgroundHandler.ActivateBackground();
        carryOvers.ResetCurrentScene();
        carryOvers.ClearChoiceString();
    }

    public void QuitGame()
    {
        //System.Diagnostics.Process.GetCurrentProcess().Kill();
        Application.Quit();
    }

    public void GoToGameOver()
    {
        SceneManager.LoadScene(2);
    }

    public void GoToRetry()
    {
        SceneManager.LoadScene(1);
    }

    public void GoToIntro()
    {
        SceneManager.LoadScene(10);
        Destroy(GameObject.Find("BackgroundAnim"));
    }

    private IEnumerator StartFromIntro()
    {
        yield return new WaitForSeconds(0.5f);
        GoToIntro();
    }

    
    public void DestroyAllDontDestroyOnLoadObjects()
    {

        var go = new GameObject("Sacrificial Lamb");
        DontDestroyOnLoad(go);

        foreach (var root in go.scene.GetRootGameObjects())
            Destroy(root);

    }
    
}
