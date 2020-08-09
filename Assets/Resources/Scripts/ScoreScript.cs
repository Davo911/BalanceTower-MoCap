using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreScript : MonoBehaviour {

    Text scoreTxt;
	// Use this for initialization
	void Start () {
        scoreTxt = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void setScore(float scoreVal) {
        scoreTxt.text = scoreVal.ToString();
    }
        
}
