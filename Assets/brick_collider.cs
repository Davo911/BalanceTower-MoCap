using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class brick_collider : MonoBehaviour
{
	[SerializeField] public RotationState rotationState;


	GameObject Tablet;
    float OldHeight, NewHeight;
    bool IsFalling;
    private bool touched;
	Rigidbody RigidbodyBrick;
	Animator animator;
	private bool IsPlaying;
	ParticleSystem ParticleSystem;


	// Start is called before the first frame update
	void Start()
    {
        IsFalling = true;
        Tablet = GameObject.FindGameObjectWithTag("Tablet");
        OldHeight = transform.position.y;
        touched = false;
		RigidbodyBrick = GetComponent<Rigidbody>();
		animator = GetComponent<Animator>();
		ParticleSystem = GetComponent<ParticleSystem>();
		if( ParticleSystem != null )
		{
			var emission = ParticleSystem.emission.enabled;
			emission = false;
			Debug.Log( "ParticleSystem found" );
		}

	}

    // Update is called once per frame
    void Update()
    {
            if (Tablet != null) {
                if (transform.position.y < Tablet.transform.position.y)
                {
                    transform.parent = null;
                }   
                //safety
                if(tag == "Tower")
                    transform.parent = Tablet.transform.parent;
            }
            

    }

	private void OnCollisionEnter(Collision collision)
    {
        //Destroy brick when hitting floor
        if (collision.collider.tag != "Player" && collision.collider.tag == "Floor" && (tag == "Brick" || tag == "Tower"))
        {
            tag = "Floor";
            StartCoroutine(DestroyObjectWithDelay(0.5f));
            
            GameObject.Find("GameManager").GetComponent<GameManager>().takeExtralife(1);

        }

        //Play Animation dependinung on velocity, rotation
        IsPlaying = animator.GetCurrentAnimatorStateInfo( 0 ).IsTag( "animation" );

		#region Animation
		//don't play animation if animation is still playing
		if( IsPlaying == false )
		{
			//Only play animation when colliding with certain speed
			if( collision.relativeVelocity.y > 0.5f )
			{
				if( collision.collider.gameObject.tag == "Tower" || collision.collider.gameObject.tag == "Tablet" )
				{
					if( tag == "Tower" ) //own tag then it is not the falling object but the on already in tower
					{
						if( rotationState == RotationState.Rotation0 || rotationState == RotationState.Rotation180 )
						{
							animator.Play( "AlreadInTower", 0, 0 );
							//Debug.Log( "Playing: AlreadInTower" );
						}
						if( rotationState == RotationState.Rotation90 || rotationState == RotationState.Rotation270 )
						{
							animator.Play( "AlreadInTowerRotate", 0, 0 );
							//Debug.Log( "Playing: AlreadInTowerRotate" );
						}
					}
					else
					{
						if( rotationState == RotationState.Rotation0 || rotationState == RotationState.Rotation180 )
						{
							animator.Play( "stick1_animation", 0, 0 );
							//Debug.Log( "Playing: stick1_animation" );
							//Debug.Log( ParticleSystem.ToString() );
							//Debug.Log( ParticleSystem.emission.enabled.ToString() );
							if( ParticleSystem != null )
							{
								var emission = ParticleSystem.emission.enabled;
								emission = true;
								//Debug.Log( ParticleSystem.emission.enabled.ToString() );
							}
						}
						if( rotationState == RotationState.Rotation90 || rotationState == RotationState.Rotation270 )
						{
							animator.Play( "stick1_animationRotate", 0, 0 );
							//Debug.Log( "Playing: stick1_animationRotate" );
							if( ParticleSystem != null )
							{
								var emission = ParticleSystem.emission.enabled;
								emission = true;
							}
						}
					}
                    if(Tablet == null)
                        Tablet = GameObject.FindGameObjectWithTag("Tablet");
                    transform.parent = Tablet.transform.parent;
                    /*
                    if (transform.parent == null)
                    {
                        Tablet.transform.parent = transform;
                    }
                    else { 
                        Tablet.transform.parent = transform.parent;
                    }
                    */
                }
				else
				{
					if( rotationState == RotationState.Rotation0 || rotationState == RotationState.Rotation180 )
						animator.Play( "AlreadInTower", 0, 0 );
					if( rotationState == RotationState.Rotation90 || rotationState == RotationState.Rotation270 )
						animator.Play( "AlreadInTowerRotate", 0, 0 );
				}
			}
		}
		#endregion Animation



	}

    private void OnCollisionExit(Collision collision)
    {
        if (tag == "Tower")
            tag = "Brick";
    }

    private void OnCollisionStay(Collision collision)
    {
		if (collision.gameObject.tag == "Tablet" || collision.gameObject.tag == "Tower")
        {
            tag = "Tower";
        }	
	}

	private Vector3 CalculatePassingVelocity()
	{
		Vector3 NewVeloctiy = new Vector3();
		return NewVeloctiy;
	}

	IEnumerator DestroyObjectWithDelay( float seconds )
	{
        GameObject.Instantiate(Resources.Load("Prefabs/smoke"), transform.position, Quaternion.Euler(-90,0,0));

        //yield return new WaitForSeconds(seconds);
        yield return new WaitForSeconds(seconds);
        if ( gameObject.transform.parent != null )
			gameObject.transform.parent = null;

		Destroy( gameObject );
	}
}

public enum RotationState
{
	Rotation0 = 0,
	Rotation90 = 1,
	Rotation180 = 2,
	Rotation270 = 3
}
