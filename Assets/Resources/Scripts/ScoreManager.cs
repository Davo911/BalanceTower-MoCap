using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;


public class ScoreManager : MonoBehaviour {

    GameManager manager;
    int highscore = 0;
    int bestscore = 0;
    public Text highscoreTxt;
    public Text bestscoreTxt;

    // Use this for initialization
	void Start () {
        highscore = PlayerPrefs.GetInt("Highscore");
        manager = FindObjectOfType<GameManager>();
	}

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            ResetHighscore();
        SetHighscore(manager.score);
        highscoreTxt.text = "Highscore: " + PlayerPrefs.GetInt("Highscore").ToString();
        bestscoreTxt.text = "Your Best: " + bestscore.ToString();
    }

    void ResetHighscore() {
        PlayerPrefs.SetInt("Highscore", 0);
        highscore = 0;
        bestscore = 0;
    }

    

    void SetHighscore(int val) {
        if (val > highscore)
        {
            PlayerPrefs.SetInt("Highscore", val);
            highscore = val;
        }
        if (val > bestscore)
        {
            bestscore = val;
        }
    }

    public void resetScore()
    {
        Debug.Log(bestscore);
        bestscore = 0;
        Debug.Log("--"+bestscore);
        bestscoreTxt.text = "Your Best: " + bestscore.ToString();
    }
	
}
