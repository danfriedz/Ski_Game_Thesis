using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static UnityEngine.UIElements.UxmlAttributeDescription;
using System.Security.Cryptography;

public class Objective_Spawner : MonoBehaviour
{
   // public static Objective_Spawner Instance { get; private set; }

    //from options menu, global perhaps
    static int numLeftStreches = 50;
    static int numRightStreches;//= PersistentManagerScript.Singleton_numRightStreches;
    static int numUpStreches;// = PersistentManagerScript.Singleton_numUpStreches;
    static int numDownStreches;

    static int numPro = PersistentManagerScript.Singleton_numOfPronation;
    static int numSup = PersistentManagerScript.Singleton_numOfSupination;

    private float elapsedTime = 0.0f;
    private float holdDuration = 3.0f;
    private bool startTimerFlag = false;
    private bool countOnceFlag = false;

    private int localCurrentLocation = -1;

    [SerializeField] public TextMeshProUGUI holdTimeTextUI;
    [SerializeField] public TextMeshProUGUI LeftCountUI;
    [SerializeField] public TextMeshProUGUI RightCountUI;
    [SerializeField] public TextMeshProUGUI UpCountUI;
    [SerializeField] public TextMeshProUGUI DownCountUI;

    [SerializeField] public SpriteRenderer LeftSprite;
    [SerializeField] public SpriteRenderer RightSprite;
    [SerializeField] public SpriteRenderer UpSprite;
    [SerializeField] public SpriteRenderer DownSprite;
    [SerializeField] public SpriteRenderer AmazingUI;

    [SerializeField] public Animator Photo_LHS;
    [SerializeField] public SpriteRenderer Photo_LSH_SR;
    [SerializeField] public Animator Crowd_RHS;
    [SerializeField] public Sprite[] SkiPhotos;

    //[SerializeField] public GameObject PersistanceManager;
    private PersistentManagerScript _manager;

    private Color StrechGo = new Color(1,1,1,1);

    //Array strechNames = ["Left","Right","Up","Down"];
    private int[] strechCount = {numLeftStreches, numRightStreches,numUpStreches,numDownStreches};
    //private int[] strechCount;

    void Start()
    {
        //PersistanceManager = GameObject.FindGameObjectWithTag("Singelton").GetComponent<PersistentManagerScript>();

        //persistenceScript = PersistanceManager.GetComponent<PersistentManagerScript>();
        //strechCount = {numLeftStreches, numRightStreches, numUpStreches, numDownStreches };
        _manager = GameObject.FindFirstObjectByType<PersistentManagerScript>();

        numLeftStreches = _manager.Singleton_numLeftStreches;
        numRightStreches = _manager.Singleton_numRightStreches;
        numUpStreches = _manager.Singleton_numUpStreches;
        numDownStreches = _manager.Singleton_numDownStreches;

        LeftCountUI.text = "Remaining Left Streches: " + numLeftStreches;
        RightCountUI.text = "Remaining Right Streches: " + numRightStreches;
        UpCountUI.text = "Remaining Down Streches: " + numUpStreches;
        DownCountUI.text = "Remaining Up Streches: " + numDownStreches;
        holdTimeTextUI.text = "Hold that strech!";
        AmazingUI.enabled = false;

        strechCount[0] = _manager.Singleton_numLeftStreches;
        strechCount[1] = _manager.Singleton_numRightStreches;
        strechCount[2] = _manager.Singleton_numUpStreches;
        strechCount[3] = _manager.Singleton_numDownStreches;
    }
    
    void Update()
    {
        
        //visual timer updates UI element. When done lowers strech count
        visualTimer();
        numLeftStreches = _manager.Singleton_numLeftStreches;
        numRightStreches = _manager.Singleton_numRightStreches;
        numUpStreches = _manager.Singleton_numUpStreches;
        numDownStreches = _manager.Singleton_numDownStreches;

        //strechCount =[numLeftStreches,numRightStreches,numUpStreches,numDownStreches];
        /*
        LeftCountUI.text = "Remaining Left Streches: " + numLeftStreches;
        RightCountUI.text = "Remaining Right Streches: " + numRightStreches;
        UpCountUI.text = "Remaining Down Streches: " + numUpStreches;
        DownCountUI.text = "Remaining Up Streches: " + numDownStreches;

        strechCount[0] = _manager.Singleton_numLeftStreches;
        strechCount[1] = _manager.Singleton_numRightStreches;
        strechCount[2] = _manager.Singleton_numUpStreches;
        strechCount[3] = _manager.Singleton_numDownStreches;*/

    }

    void visualTimer()
    {
        if (startTimerFlag == true){
            if(elapsedTime < holdDuration)
            {
                elapsedTime += Time.deltaTime;
            }
            if((int)elapsedTime == holdDuration)
            {
                print("Hold Done");
                AmazingUI.enabled = true;

                Photo_LHS.Play("snapshot");
                Photo_LSH_SR.sprite = SkiPhotos[0];
                Crowd_RHS.Play("Crowd_Rise");

                lowerStrechCounter();
            }
        }
        else {elapsedTime = 0;}
        //UI element for visual feedback
        holdTimeTextUI.text = elapsedTime.ToString("0.00");
    }

    void lowerStrechCounter()
    {
        //we only want to lower it once therefore check this flag
        if(countOnceFlag == true)
        {
            strechCount[localCurrentLocation] -= 1;

            if (localCurrentLocation == 0) _manager.Singleton_numLeftStreches -= 1;
            if (localCurrentLocation == 1) _manager.Singleton_numRightStreches -= 1;
            if (localCurrentLocation == 2) _manager.Singleton_numDownStreches -= 1;
            if (localCurrentLocation == 3) _manager.Singleton_numUpStreches -= 1;

            print("Direction count:"+strechCount[localCurrentLocation]);
            countOnceFlag = false;
            updateUI();
        }
    }

    void updateUI()
    {
        if(localCurrentLocation == 0){
            LeftCountUI.text = "Remaining Left Streches: " + strechCount[localCurrentLocation].ToString();
        }
        if(localCurrentLocation == 1){
            RightCountUI.text = "Remaining Right Streches: " +strechCount[localCurrentLocation].ToString();
        }
        if(localCurrentLocation == 2){
            UpCountUI.text = "Remaining Down Streches: " +strechCount[localCurrentLocation].ToString();
        }
        if(localCurrentLocation == 3){
            DownCountUI.text = "Remaining Up Streches: " +strechCount[localCurrentLocation].ToString();
        }
    }

    //character controller calls this function
    public void startStrechRoutine(string locationStr)
    {
        //updates when player touches a strech direction
        if(locationStr == "Left"){
            localCurrentLocation = 0;
            }
        if(locationStr == "Right"){
            localCurrentLocation = 1;
            }
        if(locationStr == "Up"){
            localCurrentLocation = 2;
            }
        if(locationStr == "Down"){
            localCurrentLocation = 3;
            }

        updateUI();
        
        //if strechs remain in this direction begin countdown
        if(strechCount[localCurrentLocation] >= 1){
            startTimerFlag = true;
            countOnceFlag = true;
            visualFeedback(localCurrentLocation);
        }

        //not implemented currently
        if (locationStr == "Stop")
        {
            AmazingUI.enabled = false;
            startTimerFlag = false;
            localCurrentLocation = 10;
            visualFeedback(localCurrentLocation);
        }
    }

    void visualFeedback(int areaToHighlight)
    {
        float time = 0;
        //0=left,1=right,2=up,3=down
        if(areaToHighlight == 0)
        {
            LeftSprite.color = StrechGo;
        }
        if(areaToHighlight == 1)
        {
            RightSprite.color = StrechGo;
        }
        if(areaToHighlight == 2)
        {
            UpSprite.color = StrechGo;
        }
        if(areaToHighlight == 3)
        {
            DownSprite.color = StrechGo;
        }
        if(areaToHighlight == 10)
        {
            LeftSprite.color = Color.clear;
            RightSprite.color = Color.clear;
            UpSprite.color = Color.clear;
            DownSprite.color = Color.clear;
        }
        time += Time.deltaTime;
    }
    /*
    private void Awake()
    {
        //ensures we keep the first OG version around.
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        //get a refrence to the dropdown buttons
        //on the level select screen
        

    }*/

}
