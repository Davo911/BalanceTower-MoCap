using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    [SerializeField] private Image LifeImage;
    public float speed = 1f;
	private float startTime, journeyLength;
    public bool runningGame = false;
    public bool noclip = false;
    public int score = 0;
    public int turn = 0;
    ScoreScript scoreTxt;
    headlineTxt headlineTxt;
    public Text extralives;
    public Text textinvulnarebility;
    StartButton button1;
    public Random rng;
    public int extralife;
    public int remaining_invulnarebility;
    private Sprite[] LifeSprite;
    //public GameObject startbutton;

    void Awake() {
        scoreTxt = FindObjectOfType<ScoreScript>();
        button1 = FindObjectOfType<StartButton>();
        headlineTxt = FindObjectOfType<headlineTxt>();
        
    }
	// Use this for initialization
	void Start () {
        //newGame();
	}
	
	// Update is called once per frame
    void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time); //To set the speed, just multiply the Time.
        if (Input.GetKeyDown(KeyCode.T)){
            noclip = !noclip;
        }
        if(Input.GetKey(KeyCode.N) && !runningGame)
            newGame();

        if (remaining_invulnarebility > 0)
            remaining_invulnarebility--;
        //if(runningGame)
        //Time.timeScale += 0.0001f; //speeding up the game

        setScore(GameObject.FindGameObjectsWithTag("Tower").Length);
	}

    public void newGame(){
        Debug.Log("New Game(Manager)");
        try
        {
            FindObjectOfType<ObstaclesSpawn>().newGame();
            LifeSprite = Resources.LoadAll<Sprite>("Sprites/hearts");
            addScore(-score);
            turn = 0;
            extralife = 0;
            setExtralife(3);
            runningGame = true;
            button1.transform.position = new Vector3(0.5f, -0.5f, -0.3f);
            Time.timeScale = 1;
            LifeImage.sprite = LifeSprite[extralife];
            //headlineTxt.changeState(false);

        }
        catch (System.Exception)
        {

            throw;
        }
    }
    public void addScore(int i){
        score += i;
        scoreTxt.setScore(score);

    }

    public void setScore(int i)
    {
        score = i;
        scoreTxt.setScore(score);
        foreach (var item in GameObject.FindGameObjectsWithTag("Wall"))
        {
            item.transform.localScale = new Vector3(1, 0.1f+0.1f * score, 1);
        }
    }
    public void takeExtralife(int i)
    {
        if (extralife > 0)
        {
            extralife -= i;
        }
        else {
            EndGame();
        }
        LifeImage.sprite = LifeSprite[extralife];
    }
    public void setExtralife(int l)
    {
        extralife = l;
        LifeImage.sprite = LifeSprite[extralife];
        //extralives.text = "Extra Lives: " + extralife;
    }

    public void addExtralife(int l)
    {
        extralife += l;
        LifeImage.sprite = LifeSprite[extralife];
        //extralives.text = "Extra Lives: " + extralife;
    }


    public bool checkforitem()
    {
        //  Zusammenfassung:
        //Returns true, if there is an item going to be spawned
        if ((turn) % 3 == 0 && turn != 0)
            return true;
        return false;
    }

    public float propability()
    {
        //Returns a propability whats the chance of an high tier obstacle to spawn
        if (turn < 5)
            return 0;
        if (turn < 10)
            return 0.2f;
        return 0.4f;
    }

    public void increaseInvulnarebility(int i) {
        remaining_invulnarebility += i;
    }

    public void setInvulnarebility(int i){
        remaining_invulnarebility = i;
    }

    public void EndGame() {
        try
        {
            runningGame = false;
            //headlineTxt.changeState(true);
            button1.transform.position = new Vector3(0.5f, 0.16f, -1.1f);
            StartCoroutine(FindObjectOfType<MoCapAnsteuerung>().collapseSkeleton());

            foreach (var item in GameObject.FindGameObjectsWithTag("Tower"))
            {
                Destroy(item);
            }
            foreach (var item in GameObject.FindGameObjectsWithTag("Brick"))
            {
                Destroy(item);
            }
            foreach (var item in GameObject.FindGameObjectsWithTag("FallShadow"))
            {
                Destroy(item);
            }
        }
        catch (System.Exception)
        {

            throw;
        }

    }
}
