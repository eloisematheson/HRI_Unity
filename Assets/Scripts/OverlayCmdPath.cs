using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlayCmdPath : MonoBehaviour {

	//Overlay information for the commanded path (points sent in via ROS)
	private List<Vector3> cmdPath = new List<Vector3>();

	// Use this for initialization
	void Start() {

		//Create a repeating invoked function after 5.0 seconds at a set period of 0.04 seconds
		InvokeRepeating("DrawOverlay",5.0f, 0.04f);
	}

	void DrawOverlay () {

		//First Check Tunnel Steering has been chosen by the user
		if (SetupScene.TunnelSteeringBool & SetupScene.StartProcedure) {

			//Check information has been received over ros
			if (CmdPathOverlay.overlayCmdRosReceived) {

				//Determine current position of the needle
				GameObject needleCurrent = GameObject.Find ("needle");
				if (needleCurrent == null)
					Debug.Log ("Fcn NeedleOffsets Callback: Can't find the needle object in the scene");
				else {

					//Get the line rendered from the game object (defined in Unity interface)
					LineRenderer cmdOverlayRenderer = gameObject.GetComponent<LineRenderer> ();
                    //Debug.Log ("Number of positions in overlay from ROS is: " + CmdPathOverlay.overlayCmdRosPos.GetLength (0));

                    //GLOBAL REFERENCE
                    for (int i = 0; i < CmdPathOverlay.overlayCmdRosPos.GetLength(0); i++)
                    {
                        //Get overlay path points from the array
                        Vector3 posInt = CmdPathOverlay.overlayCmdRosPos[i];
                        Quaternion quatInt = CmdPathOverlay.overlayCmdRosQuat[i];
                        //Debug.Log("Cmd Overlay Time step: " + i + ", ROS pos is: " + posInt);

                        //Change to Unity frame by negating y axis and mirroring y axis
                        Vector3 deltaPosGlobal = new Vector3(posInt.x, -1 * posInt.y, posInt.z);
                        Quaternion deltaQuatGlobal = new Quaternion(-1 * quatInt.x, quatInt.y, -1 * quatInt.z, quatInt.w);
                        //Debug.Log("Cmd Overlay Time step: " + i + ", Unity pos is: " + deltaPosGlobal);

                        //Add the vector to the rendered list (currently orientation ignored)
                        cmdPath.Add(deltaPosGlobal);
                    }

                    /*
                    //LOCAL SETTINGS
                    //Define transformation between ROS LH and Unity LH Frames
                    //Done by examination ROS(RH)[X Y Z] -> UNITY(LH)[Y -Z X], 
                    Quaternion ros2unityQuat = Quaternion.Euler (0, -90, 90);	//Order
					Quaternion ros2unityQuatInverse = Quaternion.Inverse (ros2unityQuat);

					for (int i = 0; i < CmdPathOverlay.overlayCmdRosPos.GetLength (0); i++) {

						//Get starting point as the current pose of the needle tip
						Vector3 globalNeedlePos = GameObject.Find ("needle").transform.position;
						Quaternion globalNeedleQuat = GameObject.Find ("needle").transform.localRotation;

						//Get overlay path points from the array
						Vector3 posInt = CmdPathOverlay.overlayCmdRosPos [i];
						Quaternion quatInt = CmdPathOverlay.overlayCmdRosQuat [i];
						//Debug.Log ("Time step: " + i + ", ROS pos is: " + posInt);
								
						//Convert ROS RH quaternion to ROS LH quaternion by mirroring the Y axis, and translation by negating y
						Vector3 deltaPosLocalROS = new Vector3 (posInt.x, posInt.y, -1 * posInt.z);
						Quaternion deltaQuatLocalROS = new Quaternion (-1 * quatInt.x, -1 * quatInt.y, quatInt.z, quatInt.w);

						//Transform the overlay position in the local Unity LH frame
						//Vector3 deltaPosLocal = ros2unityQuatInverse * (ros2unityQuat * deltaPosLocalROS);
						Quaternion deltaQuatLocal = deltaQuatLocalROS * ros2unityQuatInverse;

						//TEST - direct transformatin ROS(RH)[X Y Z] -> UNITY(LH)[Y -Z X]
						Vector3 deltaPosLocal = new Vector3 (posInt.z, posInt.x, -1*posInt.y);
						deltaPosLocal = Quaternion.Euler(0, 180, 0) * deltaPosLocal; //Seems to be required for the transparent shader
						//Debug.Log ("And Local Unity Pos is: " + deltaPosLocal);

						//Change to global frame
						Vector3 deltaPosGlobal = globalNeedlePos + (globalNeedleQuat * deltaPosLocal);		//deltaPosLocal transferred to global frame first
						Quaternion deltaQuatGlobal = globalNeedleQuat * deltaQuatLocal;						//deltaQuatLocal is locally applied with this order of ops

						//Add the vector to the rendered list
						//cmdPath.Add (deltaPosGlobal);			//Currently only the position of the path matters (rotation ignored)
						cmdPath.Add (deltaPosLocal);

					}*/

					//Change number of points to match that in the list
					cmdOverlayRenderer.positionCount = cmdPath.Count;
					//Debug.Log ("Number of positions in overlay is: " + cmdPath.Count);

					//Draw the actual path the needle will follow with respect to the needle tip
					for (int j = 0; j < cmdPath.Count; j++) {
						//Change the postion of the lines
						cmdOverlayRenderer.SetPosition (j, cmdPath [j]);
						//Debug.Log ("Setting overlay position " + j + " as: " + cmdPath [j]);
					}

					//Clear the list
					cmdPath.Clear ();
				}
			}
		}
	}
		
	void OnDestroy()
	{
		//Destroy the instance
		//Destroy(cmdMaterial);
	}

}
