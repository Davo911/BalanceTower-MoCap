using UnityEngine;
using System.Collections;

public class SignMove : MonoBehaviour {
	private Vector3 startPosition;
	public float speed = 0.03f;
	public float distance = 0.0005f;
	private float startTime, journeyLength;
	private Vector3 startMarker, endMarker;

	// Use this for initialization
	void Start () {
		startPosition = transform.position;
		newPosition();
		distance *= Mathf.Pow(Mathf.Abs(transform.position.z),1.5f);
		speed *= Mathf.Abs(transform.position.z / 30f);
	}
	
	// Update is called once per frame
	void Update () {
		float distCovered = (Time.time - startTime) * speed;
		float fracJourney = distCovered / journeyLength;
		transform.position = Vector3.Lerp(startMarker, endMarker, fracJourney);
		if(fracJourney > 1)
			newPosition();
	}

	private void newPosition(){
		startMarker = transform.position;
		endMarker = new Vector3(startPosition.x + Random.Range(-distance,distance), startPosition.y + Random.Range(-distance,distance), startPosition.z);
		startTime = Time.time;
		journeyLength = Vector3.Distance(startMarker, endMarker);
	}
}
