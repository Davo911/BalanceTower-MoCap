//Copyright 2013 Daniel Pots s64716 HTW Dresden

using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;

public class OMUnityPlugin : MonoBehaviour{
	public GameObject [] sphere; //for Example
	public Quaternion [] rotation;	
	public Vector3 [] scale;
	public Vector3 [] position;
	public string ip = "192.168.1.151";
	public int port = 999;
	public int boneNum = 21;
	
	void Start(){
		initMoCap(ip,port);
		boneNum = getBoneCount();
		rotation = new Quaternion[boneNum];
		position = new Vector3[boneNum];
		scale = new Vector3[boneNum];
		sphere = new GameObject[boneNum];
		
		for (int i = 0; i < boneNum; i++){ 
			sphere[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere); //for Example
		}
	}
	
	void Update(){
		update();
		updateSkeleton();
	}
	
	void updateSkeleton(){
		for (int i=0; i<boneNum;i++){
			//Marshaling of C++'s float[4] into Unity's Vector4
			IntPtr copyPointer;
			float [] copyVector = new float[4];
			copyPointer = getRotationQuaternion(i);
			Marshal.Copy( copyPointer, copyVector, 0, 4);
			rotation[i].Set(copyVector[0],copyVector[1],copyVector[2],copyVector[3]);
			
			copyPointer = getPositionVector(i);
			Marshal.Copy( copyPointer, copyVector, 0, 3);
			position[i].Set(copyVector[1],copyVector[0],copyVector[2]);
			copyPointer = getScale(i);
			Marshal.Copy( copyPointer, copyVector, 0, 3);
			scale[i].Set(copyVector[2],copyVector[1],copyVector[0]);
		
			//Example figure
			sphere[i].transform.position = position[i];
			sphere[i].transform.rotation = rotation[i];
			scale[i].Set(0.1f,0.1f,0.1f);
			sphere[i].transform.localScale = scale[i];
		}
	}
	
	//Import Functions from C++ OMSDK Wrapper
	[DllImport ("OMUnityPlugin")]
	//Initialising of MoCap-Constructor and connecting
    private static extern void initMoCap(string ip, int port); 
	[DllImport ("OMUnityPlugin")]
	//Updates Frame (Bonematrices)
    private static extern void update();
    [DllImport ("OMUnityPlugin")]
	//reads out OMBOOL isTracking
    private static extern bool checkTracking(); 
	[DllImport ("OMUnityPlugin")]
	//returns float of matrixposition x,y in Bone boneNum
    private static extern float getBone(int boneNum, int x, int y); 
    [DllImport ("OMUnityPlugin")]
	//disconnects and deletes the MoCap Instance
    private static extern void DeleteSomeClass();
	[DllImport ("OMUnityPlugin")]
	//returns OMBoneNum
    private static extern int getBoneCount();
	[DllImport ("OMUnityPlugin")]
	//returns a float[4] Array with an in C++ calculated Quaternion from Bone boneNum
    private static extern IntPtr getRotationQuaternion(int boneNum);
	[DllImport ("OMUnityPlugin")]
	//returns a float[3] Array with the Position of the Bone boneNum
    private static extern IntPtr getPositionVector(int boneNum);
	[DllImport ("OMUnityPlugin")]
	//returns a float[3] Array with the Scale of the Bone boneNum
    private static extern IntPtr getScale(int boneNum);
}