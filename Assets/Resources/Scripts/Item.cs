using UnityEngine;
using System.Collections;


public class Item : MonoBehaviour {

    Rigidbody rb;
    ConstantForce cf;
    GameManager manager;
    bool point = false;

    void Start() {
        rb = GetComponent<Rigidbody>();
        cf = GetComponent<ConstantForce>();
        //cf.force.Set(50f, 0, 0);
        //Debug.Log(rb);
        rb.AddForce(Physics.gravity);//, ForceMode.Acceleration
        //Debug.Log("Items script has been activated.");
    }
	
	// Update is called once per frame
	void Update () {
        if (manager == null)
            manager = FindObjectOfType<GameManager>();

	    if((transform.position.x > 5) || (!manager.runningGame && transform.position.x < 0)){
            Destroy(gameObject);
        }
        
	}

   


    public void OnCollisionEnter(Collision collisionInfo)
    {
        
        if (collisionInfo.collider.tag == "Player")
        {
            if (!point)
            {
                manager.addExtralife(1);
                manager.addScore(3);
                
            }
            point = true;
            Destroy(gameObject);
            
        }

    }
}
