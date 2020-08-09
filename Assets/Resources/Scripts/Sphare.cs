using UnityEngine;
using System.Collections;


public class Sphare : MonoBehaviour {

    Rigidbody rb;
    ConstantForce cf;
    GameManager manager;
    bool point = false;

    void Start() {
        rb = GetComponent<Rigidbody>();
        cf = GetComponent<ConstantForce>();
        //cf.force.Set(Physics.gravity, ForceMode.Acceleration);
        //Debug.Log(rb);
        //rb.AddForce(Physics.gravity, ForceMode.Acceleration);
    }
	
	// Update is called once per frame
	void Update () {
        if (manager == null)
            manager = FindObjectOfType<GameManager>();

        if (manager.runningGame && transform.position.x > 2 && !point)
        {
            manager.addScore(1);
            point = true;
        }
	    if((transform.position.x > 5) || (!manager.runningGame && transform.position.x < 0)){
            Destroy(gameObject);
        }
	}

   


    public void OnCollisionEnter(Collision collisionInfo)
    {
        if (collisionInfo.collider.tag == "Player" && manager.remaining_invulnarebility > 0)
            Destroy(gameObject);
        if (collisionInfo.collider.tag == "Player" && !manager.noclip)
        {
            if (manager.remaining_invulnarebility == 0) {
                if (manager.extralife == 0){
                    manager.EndGame(); 
                    return;
                }
                manager.addExtralife(-1);
                manager.setInvulnarebility(20);
                Destroy(gameObject);
            }
        }
        
    }
}
