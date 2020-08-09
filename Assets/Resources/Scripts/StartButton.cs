using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StartButton : MonoBehaviour {

    public Vector3 posAdd;

	// Use this for initialization
    GameManager manager;
	void Start () {
        manager = FindObjectOfType<GameManager>();
	}
	
	// Update is called once per frame
    void Update()
    {
        
    }

    public void OnCollisionEnter(Collision collisionInfo)
    {
        if (collisionInfo.collider.tag == "Player")
        {
            manager.newGame();
        }
    }
}
