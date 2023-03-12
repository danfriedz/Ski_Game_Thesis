using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Objective_Spawner : MonoBehaviour
{
    //from options menu, global perhaps
    static int numLeftStreches = 3;
    static int numRightStreches = 3;
    static int numUpStreches = 3;
    static int numDownStreches = 3;

    private float elapsedTime = 0.0f;
    private float holdDuration = 5.0f;
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

    private Color StrechGo = new Color(1,1,1,1);

    //Array strechNames = ["Left","Right","Up","Down"];
    private int[] strechCount = {numLeftStreches,numRightStreches,numUpStreches,numDownStreches};

    void Start()
    {
        LeftCountUI.text = "";
        RightCountUI.text = "";
        UpCountUI.text = "";
        DownCountUI.text = "";
    }
    
    void Update()
    {
        //visual timer updates UI element. When done lowers strech count
        visualTimer();
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
                lowerStrechCounter();
            }
        }
        else {elapsedTime = 0;}
        //UI element for visual feedback
        holdTimeTextUI.text = elapsedTime.ToString("#.00");
    }

    void lowerStrechCounter()
    {
        //we only want to lower it once therefore check this flag
        if(countOnceFlag == true)
        {
            strechCount[localCurrentLocation] -= 1;
            print("Direction count:"+strechCount[localCurrentLocation]);
            countOnceFlag = false;
            updateUI();
        }
    }

    void updateUI()
    {
        if(localCurrentLocation == 0){
            LeftCountUI.text = strechCount[localCurrentLocation].ToString();
        }
        if(localCurrentLocation == 1){
            RightCountUI.text = strechCount[localCurrentLocation].ToString();
        }
        if(localCurrentLocation == 2){
            UpCountUI.text = strechCount[localCurrentLocation].ToString();
        }
        if(localCurrentLocation == 3){
            DownCountUI.text = strechCount[localCurrentLocation].ToString();
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
}
