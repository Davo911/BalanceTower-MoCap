using System;
using UnityEngine;
using UnityEngine.UI;

public class ObstaclesSpawn : MonoBehaviour {
    [SerializeField] private Image previewImage;
    public Transform[] spawnPoint;
    public GameObject collider;
    public GameObject item;
    public GameObject colliderPole;
    GameObject block;
    public Transform BrickSpawnPoint;
    float startTime;
    public float spawnTime = 0f;
    public float startWaveTime = 2f;
    public int spawncounter = 0;
    public int upwardsForce = 0;
    float waveTime;
    public float timeincrease = 0.04f;
    public float FootTurnGestureHeight = 0.15f;
    int randindex, randindex_old = 0, spawnlimit;
    GameManager manager;
    bool itemspawned,polespawned,brickturned;
    bool[] spawnslotused;
    Vector3 spawnpos;
    Sprite[] TetroSprite;
    GameObject FallShadow;
	GameObject CurrentBlock;
    GameObject rightFoot, leftFoot;
    float SpawnIntervalLowerEnd = -0.9f;
	float SpawnIntervalUpperEnd = 0.9f;
	float SpawnStepWidth = 0.3f;
	float[] PossibleRotations = { 0f, 90f, 180f, 270f };

	string[] TetrisBlocks =
	{
		"TetrisI",
		"TetrisL",
		"TetrisLMirrored",
		"TetrisQuadrat",
		"TetrisT",
		"TetrisZMirrored",
		"TetrisZ"
	};
    

	void Start () {
        manager = FindObjectOfType<GameManager>();
        waveTime = startWaveTime;
        startTime = Time.time;
        manager.runningGame = false;
        brickturned = false;
        TetroSprite= Resources.LoadAll<Sprite>("Sprites/tetrominoes_sprite");
    }
	
	// Update is called once per frame
    void FixedUpdate()
    {
        if (manager.runningGame)
        {
            leftFoot  = GameObject.Find("Avatar").GetComponent<MoCapAnsteuerung>().leftFoot;
            rightFoot = GameObject.Find("Avatar").GetComponent<MoCapAnsteuerung>().rightFoot;

            float footheightdiff = leftFoot.transform.position.y - rightFoot.transform.position.y;
            //if (footheightdiff > -FootTurnGestureHeight && footheightdiff < FootTurnGestureHeight)
                brickturned = false;
            if (!brickturned && tag != "Tower" && (Input.GetKeyDown(KeyCode.Comma) || footheightdiff < -FootTurnGestureHeight))
            {
                brickturned = true;
                Debug.Log("Rotate Counterclockwise");
                block.transform.Rotate(new Vector3(90f, 0, 0));

            }
            if (!brickturned && tag != "Tower" && ((Input.GetKeyDown(KeyCode.Minus) || footheightdiff > FootTurnGestureHeight)))
            {
                brickturned = true;
                Debug.Log("Rotate Clockwise");
                block.transform.Rotate(new Vector3(-90f, 0, 0));
            }
            if ((Time.time - startTime) >= spawnTime)//spawn stuff over time
            {
                SpawnBricks();
                //SpawnColliders();
                
                //Instantiate(collider, new Vector3(0, 5, 0), Quaternion.identity);
                spawnTime += waveTime;
                //Time.timeScale += timeincrease;
            }
        }else if (FallShadow != null)
        {
            Destroy(FallShadow);
            FallShadow = null;
        }
        else
        {
            previewImage.sprite = TetroSprite[0];
        }
	}

    public void newGame() {
        FindObjectOfType<ObstaclesSpawn>().startTime = Time.time; // equal to this.startTime = Time.time?
        waveTime = startWaveTime;
        spawnTime = 0f;
        startTime = Time.time;
		//Instantiate( Resources.Load( "Prefabs/FallShadow" ), new Vector3( 0,0,0 ), Quaternion.identity );
		//FallShadow = GameObject.FindGameObjectWithTag( "FallShadow" );
        FallShadow = GameObject.Instantiate(Resources.Load("Prefabs/FallBlitz"), new Vector3(0, 5.67f, 0), Quaternion.identity) as GameObject;
    }

    void SpawnBricks() //Unsere Methode zum spawnen von Tetris steinen
    {
		//Random Position within interval and stepwidth
		Vector3 SpawnPos = new Vector3( 0.5f, 
			BrickSpawnPoint.transform.position.y, 
			UnityEngine.Random.Range( SpawnIntervalLowerEnd, SpawnIntervalUpperEnd ) );

		BrickSpawnPoint.transform.position = SpawnPos;
		//Debug.Log( "Spawn Position: " + BrickSpawnPoint.transform.position.ToString() );

		//Random Rotation
		int randNumber = UnityEngine.Random.Range( 0, PossibleRotations.Length );
		float angle = PossibleRotations[ randNumber ];
		Quaternion rotation = Quaternion.Euler( angle, 0, 0 );

        //Random Form
        int blocknr = UnityEngine.Random.Range(0, TetrisBlocks.Length);
        string blockToInstantiate = TetrisBlocks[blocknr];

        //Brick preview
        setBlockPreview(blocknr);

		//Spawn Brick
		block = GameObject.Instantiate(Resources.Load("Prefabs/Tetris/"+ blockToInstantiate ), 
			BrickSpawnPoint.transform.position, rotation ) as GameObject;

		brick_collider block_script = block.GetComponent<brick_collider>();
		block_script.rotationState = (RotationState)randNumber; //same index as in PossibleRotations

		block.transform.position = BrickSpawnPoint.transform.position;
        //	Debug.Log( "Block Position:" + block.transform.position.ToString() );

        //Set FallShadow and increment spawncounter
        //FallShadow.transform.position = BrickSpawnPoint.transform.position;
        FallShadow.transform.position = new Vector3(BrickSpawnPoint.position.x, FallShadow.transform.position.y, BrickSpawnPoint.position.z);
		spawncounter++;
    }
    
    void setBlockPreview(int blocknr)
    {
        if (blocknr == 0)
        {
            previewImage.sprite = TetroSprite[5];
        }
        else if (blocknr == 1 || blocknr == 2)
        {
            previewImage.sprite = TetroSprite[1];
        }
        else if (blocknr == 3)
        {
            previewImage.sprite = TetroSprite[4];
        }
        else if (blocknr == 4)
        {
            previewImage.sprite = TetroSprite[3];
        }
        else if (blocknr == 5 || blocknr == 6)
        {
            previewImage.sprite = TetroSprite[2];
        }
        //Sprite sprite = Resources.Load<Sprite>("Sprites/tetrominoes_sprites_1");
        //previewImage.sprite = TetroSprite[1];
    }


    void SpawnColliders()
    {
        int[] spawnslotused = new int[3];
        spawnslotused[0] = 0; 
        spawnslotused[1] = 0; 
        spawnslotused[2] = 0;
        int blocksspawned = 0;

        itemspawned = false;
        polespawned = false;
        spawnlimit = 2;
        for (int i = 0; i <= manager.turn / 10 && i < spawnlimit; i++) //i represents the number of obstacles or items to spawn in a single wave
        {
            randindex_old = randindex;
            do
            {
                randindex = UnityEngine.Random.Range(0, spawnPoint.Length);
            } while (spawnslotused[randindex] == 1);
            //while(randindex_old == randindex);
        
            if (manager.checkforitem() && !itemspawned) //Spawn an Item
            {
                Instantiate(item, spawnPoint[randindex].position, Quaternion.identity);
                itemspawned = true;
                spawnlimit++;
                spawnslotused[randindex] = 1;

            }
            else
            {
                if ((UnityEngine.Random.value <= manager.propability() || blocksspawned > 1 ) && !polespawned) //Spawn a High Pole
                {
                    spawnpos = spawnPoint[randindex].position;
                    spawnpos.y = 1.4f;
                    spawnpos.x--;
                    Instantiate(colliderPole, spawnpos, Quaternion.AngleAxis(90, new Vector3(0, 1, 0)));
                    polespawned = true;
                    spawnslotused[randindex] = 1;
                }
                else //Spawn a Block
                {
                    Instantiate(collider, spawnPoint[randindex].position, Quaternion.AngleAxis(90,new Vector3(0,1,0)));
                    spawnslotused[randindex] = 1;
                    blocksspawned++;
                }
            }
        }
        manager.turn++;
    }

    public void countCounter(int plusminusone)
    {
        spawncounter = spawncounter + plusminusone;
    }
}
