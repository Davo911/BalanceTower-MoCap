using System;
using System.Collections;
using UnityEngine;
using Ventuz.OSC;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class MoCapAnsteuerung : MonoBehaviour
{
	public static Matrix4x4[] matrix = new Matrix4x4[21]; //Knochenmatrizen aus MoCap abspeichern
	private Transform[] bones;
	private GameObject[] sphere; //Sphären um Knochenpositionen anzuzeigen
    private GameObject tablet, tabletEmptyGameObject;
	private	static UdpReader reader;
	private OscBundle bundle;
	//Handmesh bei Fernglas an und Ausschalten
	public SkinnedMeshRenderer[] hands;
    public GameManager manager;
	private bool mocap, mocapcontrolling;
	private float[] skeleton;
	public float handabstand, handlinks, handrechts = 0;
	public Transform _00UpperChest;
	public Transform _01MidTorso;
	public Transform _02LowerTorso;
	public Transform _03LeftUpperLeg;
	public Transform _04LeftLowerLeg;
	public Transform _05LeftFoot;
	public Transform _06RightUpperLeg;
	public Transform _07RightLowerLeg;
	public Transform _08RightFoot;
	public Transform _09Neck;
	public Transform _10Head;
	public Transform _11RightClavicle;
	public Transform _12RightShoulder;
	public Transform _13RightUpperArm;
	public Transform _14RightLowerArm;
	public Transform _15RightHand;
	public Transform _16LeftClavicle;
	public Transform _17LeftShoulder;
	public Transform _18LeftUpperArm;
	public Transform _19LeftLowerArm;
	public Transform _20LeftHand;
    public GameObject rightFoot, leftFoot;
    public float maxFernglas = 0.3f;
    public float maxclap = 0.3f;
	public static bool glas;
    public float TabSpawnDistance = 0.5f;
    public float HandsOutDistance;
	private Vector3 initPosition = Vector3.zero;
	private Transform AvatarRoot;
    private int posenumber = 0, previouspose = 0;
    private bool TabSpawned;
    private bool PoseHandsTogether, PoseHandsOut, TabDisappear;
    private bool gameover = false;
	public void Awake ()
	{
		bones = new Transform[21];
		
		bones [0] = _00UpperChest;
		bones [1] = _01MidTorso;
		bones [2] = _02LowerTorso;
		bones [3] = _03LeftUpperLeg;
		bones [4] = _04LeftLowerLeg;
		bones [5] = _05LeftFoot;
		bones [6] = _06RightUpperLeg;
		bones [7] = _07RightLowerLeg;
		bones [8] = _08RightFoot;
		bones [9] = _09Neck;
		bones [10] = _10Head;
		bones [11] = _11RightClavicle;
		bones [12] = _12RightShoulder;
		bones [13] = _13RightUpperArm;
		bones [14] = _14RightLowerArm;
		bones [15] = _15RightHand;
		bones [16] = _16LeftClavicle;
		bones [17] = _17LeftShoulder;
		bones [18] = _18LeftUpperArm;
		bones [19] = _19LeftLowerArm;
		bones [20] = _20LeftHand;
	}
    GameObject objA, objB;

    void Start ()
	{
        TabSpawned = false;
		if(reader == null)
		reader = new UdpReader (4000);
		sphere = new GameObject[21];
		skeleton = new float[336];
		AvatarRoot = GameObject.FindGameObjectWithTag("AvatarRoot").transform;
		for (int i = 0; i < 21; i++){
            if (i == 5 /*|| i == 10 || i == 15 || i == 20*/)
            {
                sphere[i] = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/GemSphereCollider"), new Vector3(0f, 0f, 0f), Quaternion.identity); //Sphären Instanzieren und Erstellen
                leftFoot = sphere[i];
            }
            else if (i == 8)
            {
                sphere[i] = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/GemSphereCollider"), new Vector3(0f, 0f, 0f), Quaternion.identity); //Sphären Instanzieren und Erstellen
                rightFoot = sphere[i];
            }
            else if (i == 10)
            {
                sphere[i] = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/GemDiamond")); //Sphären Instanzieren und Erstellen
            }
            else
                sphere[i] = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/GemSphere"), new Vector3(0f, 0f, 0f), Quaternion.identity); //Sphären Instanzieren und Erstellen
            sphere[i].transform.parent = AvatarRoot;
		}
        TabSpawnDistance = 0.5f;
        HandsOutDistance = 1.1f;
    }
	
	// Update is called once per frame
	void Update ()
	{

        
        //change scale depending on distance betweeen hands
        

        /*	BoneInfos
		Index	Bone Name		Position in Hierarchy
		0		Upper Chest 	Root
		1		Mid Torso		Child of Upper Chest
		2		Lower Torso		Child of Mid Torso
		3		Left Upper Leg	Child of Lower Torso
		4		Left Lower Leg	Child of Left Upper Leg
		5		Left Foot		Child of Left Lower Leg
		6		Right Upper Leg	Child of Lower Torso
		7		Right Lower Leg	Child of Right Upper Leg
		8		Right Foot 		Child of Right Lower Leg
		9		Neck			Child of Upper Chest
		10		Head 			Child of Neck
		11		Right Clavicle	Child of Upper Chest
		12		Right Shoulder	Child of Clavicle
		13		Right Upper Arm	Child of Shoulder
		14		Right Lower Arm	Child of Upper Arm
		15		Right Hand		Child of Lower Arm
		16		Left Clavicle	Child of Upper Chest
		17		Left Shoulder	Child of Left Clavicle
		18		Left Upper Arm	Child of Left Shoulder
		19		Left Lower Arm	Child of Left Upper Arm
		20		Left Hand		Child of Left Lower Arm
		*/

        #region MoCap Ansteuerung
        if (reader != null)
			bundle = (OscBundle)reader.Receive (); //Aktuelle MoCap Position empfangen
        
        bundle = null; //for testing not connected environment
        
        if (bundle != null)
        {
            int i = 0;
            foreach (OscElement m in bundle.Elements)
            {
                skeleton[i] = (float)m.Args[0];
                i++;
            }
            print("Recieved Bundle");

            //Save for once on pressed button
            if (Input.GetKeyDown(KeyCode.F1))
            {
                print("Saving F1");
                saveSkeleton("pose1");
            }

			#region Saving Multiple Positions

			if (Input.GetKeyDown(KeyCode.F2))
            {
                saveSkeleton("pose2");
            }
			if (Input.GetKeyDown(KeyCode.F3))
			{
				saveSkeleton("pose3");
			}
			if (Input.GetKeyDown(KeyCode.F4))
			{
				saveSkeleton("pose4");
			}
			if (Input.GetKeyDown(KeyCode.F5))
			{
				saveSkeleton("pose5");
			}
			if (Input.GetKeyDown(KeyCode.F6))
			{
				saveSkeleton("pose6");
			}
			if (Input.GetKeyDown(KeyCode.F7))
			{
				saveSkeleton("pose7");
			}
			if (Input.GetKeyDown(KeyCode.F8))
			{
				saveSkeleton("pose8");
            }
            if (Input.GetKeyDown(KeyCode.F9))
            {
                saveSkeleton("pose9");
            }
            if (Input.GetKeyDown(KeyCode.F10))
            {
                saveSkeleton("pose10");
            }							
			
			#endregion

            mocap = true;
            mocapcontrolling = false;
        }
        else
        {
            //Load for once if no bundle found
            //if successfull, set mocap true, else quit

            if(posenumber == 0) //no pose has been set yet
            {
                mocap = loadSkeleton(skeleton, "pose6");
                posenumber = 6;
            }
            
            #region LoadPoses

            if (Input.GetKeyDown(KeyCode.Alpha1) && posenumber != 1)
            {
                mocap = loadSkeleton(skeleton, "pose1");
                posenumber = 1;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2) && posenumber != 2)
            {
                mocap = loadSkeleton(skeleton, "pose2");
                posenumber = 2;
            }
            if (Input.GetKeyDown(KeyCode.Alpha3) && posenumber != 3)
            {
                mocap = loadSkeleton(skeleton, "pose3");
                posenumber = 3;
            }
            if (Input.GetKeyDown(KeyCode.Alpha4) && posenumber != 4)
            {
                mocap = loadSkeleton(skeleton, "pose4");
                posenumber = 4;
            } 
            if (Input.GetKeyDown(KeyCode.Alpha5) && posenumber != 5)
            {
                mocap = loadSkeleton(skeleton, "pose5");
                posenumber = 5;
            }
            if (Input.GetKeyDown(KeyCode.Alpha6) && posenumber != 6)
            {
                mocap = loadSkeleton(skeleton, "pose6");
                posenumber = 6;
            } 
            if (Input.GetKeyDown(KeyCode.Alpha7) && posenumber != 7)
            {
                mocap = loadSkeleton(skeleton, "pose7");
                posenumber = 7;
            }
            if (Input.GetKeyDown(KeyCode.Alpha8) && posenumber != 8)
            {
                mocap = loadSkeleton(skeleton, "pose8");
                posenumber = 8;
            }
            if (Input.GetKeyDown(KeyCode.Alpha9) && posenumber != 9)
            {
                mocap = loadSkeleton(skeleton, "pose9");
                posenumber = 9;
            }
            if (Input.GetKeyDown(KeyCode.Alpha0) && posenumber != 10)
            {
                mocap = loadSkeleton(skeleton, "pose10");
                posenumber = 10;
            }
            #endregion

            mocapcontrolling = true;

            if (Input.GetKeyDown(KeyCode.C))
            {
                if (posenumber != 7)
                {
                    previouspose = posenumber;
                    loadSkeleton(skeleton, "pose7");
                    posenumber = 7;
                }
            }
            if (Input.GetKeyUp(KeyCode.C))
            {
                mocap = loadSkeleton(skeleton, "pose" + previouspose);
                posenumber = previouspose;    
            }
        }
		

        if (mocap) {
		
            //Speichern in Matrizen zur Weiterverarbeitung
			int x = 0;
			for (int anz = 0; anz <21; anz++) {
				matrix [anz].m00 = skeleton [x++];
				matrix [anz].m01 = skeleton [x++];
				matrix [anz].m02 = skeleton [x++];
				matrix [anz].m03 = skeleton [x++];
				matrix [anz].m10 = skeleton [x++];
				matrix [anz].m11 = skeleton [x++];
				matrix [anz].m12 = skeleton [x++];
				matrix [anz].m13 = skeleton [x++];
				matrix [anz].m20 = skeleton [x++];
				matrix [anz].m21 = skeleton [x++];
				matrix [anz].m22 = skeleton [x++];
				matrix [anz].m23 = skeleton [x++];
				matrix [anz].m30 = skeleton [x++];
				matrix [anz].m31 = skeleton [x++];
				matrix [anz].m32 = skeleton [x++];
				matrix [anz].m33 = skeleton [x++];
			}

			for (int anz = 0; anz <21; anz++) {
				//Rotation nicht für den Kopf
				if (anz != 10 && !gameover)	
					bones [anz].transform.localRotation = MoCapUtils.GetRotation (matrix [anz]);
					sphere[anz].transform.localPosition = MoCapUtils.GetPosition (matrix [anz]);

			}

            //Manual Controlling
            if (mocapcontrolling)
            {
                if (Input.GetKey(KeyCode.C))
                {
                    AvatarRoot.transform.Translate(new Vector3(0 , 0, Input.GetAxis("Horizontal") * 0.03f));
                }
                else
                {   
                    AvatarRoot.transform.Translate(new Vector3(0 , 0, Input.GetAxis("Horizontal") * 0.06f));
                }

                if (Input.GetKey(KeyCode.Q))
                {
                    AvatarRoot.Rotate(new Vector3(0, 1, 0), -5f);
                }
                if (Input.GetKey(KeyCode.E))
                {
                    AvatarRoot.Rotate(new Vector3(0, 1, 0), 5f);
                }
                
                #region LimittoPlayarea
                float avatarx, avatarz;
                if (AvatarRoot.position.z > 1.7f)
                {
                    avatarz = 1.7f;
                }else{    
                    if (AvatarRoot.position.z < -1.5f)
                    {
                        avatarz = -1.5f;
                    }else{
                        avatarz = AvatarRoot.position.z;
                    }
                }
                if (AvatarRoot.position.x > 1.3f)
                {
                    avatarx = 1.3f;
                }
                else
                {
                    if (AvatarRoot.position.x < -1.3f)
                    {
                        avatarx = -1.3f;
                    }
                    else
                    {
                        avatarx = AvatarRoot.position.x;
                    }
                }

                AvatarRoot.transform.position = new Vector3(avatarx, 0, avatarz);
                #endregion 
            }
            else //controlling via MoCap, locking manual controlling#########
            {
                AvatarRoot.transform.position = new Vector3(0,0,0);

                //Rotation ausblenden?
                if (initPosition == Vector3.zero) initPosition = sphere[1].transform.localPosition;
                //Rotation im Stillstand
                if (sphere[1].transform.localPosition.x > initPosition.x - 0.01f && sphere[1].transform.localPosition.x < initPosition.x + 0.01f &&
                    sphere[1].transform.localPosition.y > initPosition.y - 0.01f && sphere[1].transform.localPosition.y < initPosition.y + 0.01f &&
                    sphere[1].transform.localPosition.z > initPosition.z - 0.01f && sphere[1].transform.localPosition.z < initPosition.z + 0.01f)
                {
                    //AvatarRoot.rotation = Quaternion.Euler(0f,90f,0f);
                    //Debug.Log("Still");
                }
                else
                {
                    AvatarRoot.rotation = Quaternion.Euler(0f, 0f, 0f);
                    //Debug.Log ("Bewegt");
                }
            }

            /*
			//Head and Neck
			bones [9].Rotate (180, 180, 0);
			//bones[10].Rotate(180,180,0);
			//left Arm
			for (int i=16; i<=20; i++)
				bones [i].Rotate (0, 90, 90);
			//right Arm
			for (int i=11; i<=15; i++)
				bones [i].Rotate (0, -90, -90);
			//Foots
			bones [5].Rotate (90, 0, 0);
			bones [8].Rotate (90, 0, 0);



	

			//Fernglastest
			//9= Nacken 10 = Kopf 15=Handrechts 20=Handlinks
			if (Math.Abs ((MoCapUtils.GetPosition (matrix [10]) - MoCapUtils.GetPosition (matrix [15])).magnitude) < maxFernglas && 
                Math.Abs ((MoCapUtils.GetPosition (matrix [10]) - MoCapUtils.GetPosition (matrix [20])).magnitude) < maxFernglas && 
                Math.Abs ((MoCapUtils.GetPosition (matrix [15]) - MoCapUtils.GetPosition (matrix [20])).magnitude) < maxFernglas) {
				glas = true; //Fernglas Aktiv
				for (int i = 0; i < hands.Length; i++)
					hands [i].enabled = false;
			} else {
				glas = false; //Fernglas nicht aktiv
				for (int i = 0; i < hands.Length; i++)
					hands [i].enabled = true;
				
			}
            */


        }
        #endregion
        if (tablet == null)
            TabSpawned = false;
        //
        if (!TabSpawned && Input.GetKeyDown(KeyCode.T) || !TabSpawned && PoseHandsTogether == true && PoseHandsOut == true)
        {
			/*Erklärung: Tablet hat als Parent ein leeres GameObject. Das wird benötigt damit die Tetrissteine
			nicht umher fliegen.
			Das Tablet mit Collider usw. hat das Tag "Tablet"
			Der Parent von Tablet hat den Tag "TabletParent"
			Tablet hier im Code ist das Tablet mit tag "tablet"
			tabletEmptyGameObject im Code ist Parent mit Tag "TabletParent"
			*/

			Debug.Log("Tablet Spawned");
            tabletEmptyGameObject = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Tablet"), (sphere[15].transform.position + sphere[20].transform.position) / 2, Quaternion.identity);
			tablet = GameObject.FindGameObjectWithTag( "Tablet" );
			tablet.GetComponentInChildren<Animator>().Play("TabletSpawn2", 0, 0);
            Debug.Log(tablet.GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).IsTag("Spawn").ToString());

            TabSpawned = true;
            PoseHandsTogether = false;
            PoseHandsOut = false;
        }

        if (TabSpawned && PoseHandsTogether == true)
        {
            Debug.Log("Tablet Destroyed");
			Destroy( tabletEmptyGameObject.gameObject );
            TabSpawned = false;
            PoseHandsTogether = false;
        }

        //Tablet Spawn Pose
        if (Vector3.Distance(sphere[15].transform.position, sphere[20].transform.position) <= TabSpawnDistance)
        {
            PoseHandsTogether = true;
        }

        if (Vector3.Distance(sphere[15].transform.position, sphere[20].transform.position) >= HandsOutDistance)
        {
            PoseHandsOut = true;
        }

        if (TabSpawned)
        {
            tablet.transform.localScale = new Vector3(tablet.transform.localScale.x, 
                                                        tablet.transform.localScale.y, 
                                                        Vector3.Distance(sphere[15].transform.position, sphere[20].transform.position)+1);
           
			Vector3 tabletpos = (sphere[15].transform.position + sphere[20].transform.position) / 2;
            //tabletpos.Set(tabletpos.x-0.2f, tabletpos.y+0.2f, tabletpos.z);
            tabletEmptyGameObject.transform.position = tabletpos;
			tabletEmptyGameObject.transform.LookAt(sphere[15].transform);
        }

        //Move Camera Slightly
        GameObject cam = GameObject.FindGameObjectWithTag("MainCamera");
        cam.transform.LookAt(new Vector3(transform.position.x, transform.position.y+2.7f, transform.position.z/2));
        try
        {
            if (manager.runningGame && gameover == true)
                gameover = false;
        }
        catch (Exception)
        {}

            
    }


    //collapsing effekt after losing game
    public IEnumerator collapseSkeleton()
    {
        GameObject[] fakesphere = new GameObject[21];
        gameover = true;
        for (int i = 0; i < sphere.Length; i++)
        {
            sphere[i].SetActive(false);
            fakesphere[i] = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/GemSphereBounce Variant"), sphere[i].transform.position, sphere[i].transform.rotation);
            
        }

        yield return new WaitForSeconds(5);

        for (int i = 0; i < fakesphere.Length; i++)
        {
            Destroy(fakesphere[i].gameObject);
            sphere[i].SetActive(true);
            yield return new WaitForSeconds(0.1f);
        }

    }

	//Allowing to Save/Load Avatars Base Position from MoCap to OfflineVersion
    public void saveSkeleton(string filename){
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.dataPath + "/" + filename + ".dat");

        AvatarData data = new AvatarData();
        data.skeleton = new float[336];

        int length = this.skeleton.Length;
        for (int i = 0; i < length; i++)
        {
            data.skeleton[i] = this.skeleton[i];
        }

        bf.Serialize(file, data);
        file.Close();
        print("File " + filename + " has been saved");
    }

    public bool loadSkeleton(float[] skeletonarray, String filename) {
        if (File.Exists(Application.dataPath + "/" + filename + ".dat")) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.dataPath + "/" + filename + ".dat", FileMode.Open);
            AvatarData data = (AvatarData)bf.Deserialize(file);
            file.Close();

            int length = data.skeleton.Length;
            for (int i = 0; i < length; i++) {
                skeletonarray[i] = data.skeleton[i];
            }

            return true;
        }
        return false;
    }
}

//Allowing to Save/Load Avatars Base Position from MoCap to OfflineVersion

[Serializable]
class AvatarData { 
    public float[] skeleton;
}