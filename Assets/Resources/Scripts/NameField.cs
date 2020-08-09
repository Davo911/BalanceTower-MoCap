using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NameField : MonoBehaviour {

    ScoreManager sm;
    public InputField inputField;
    public GameObject goInputField;

	// Use this for initialization
	void Start () {
        
        sm = FindObjectOfType<ScoreManager>();
	}
	
	// Update is called once per frame
	void Update () {
        if (goInputField.activeSelf && Input.GetKey("escape"))
        {
            inputField.text = "";
            goInputField.SetActive(false);
        }
	}

    public void OnSubmit() {
        Debug.Log("Submit: "+inputField.text);
        goInputField.SetActive(false);

    }

    public void SaveScore()
    {
        inputField.text = "";
        goInputField.SetActive(true);
    }

}
