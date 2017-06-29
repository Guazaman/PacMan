﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour {

	public float moveSpeed = 3.9f;

	public int pinkyReleaseTimer = 5;
	public int blueReleaseTimer = 14;
	public int orangeReleaseTimer = 21;
	public float ghostReleaseTimer = 0;

	public bool isInGhostHouse = false;

	public Node startingPosition;
	public Node homeNode;

	public int scatterModeTimer1 = 7;
	public int chaseModeTimer1 = 20;
	public int scatterModeTimer2 = 7;
	public int chaseModeTimer2 = 20;
	public int scatterModeTimer3 = 5;
	public int chaseModeTimer3 = 20;
	public int scatterModeTimer4 = 5;
	//public int chaseModeTimer4 = 20;

	private int modeChangeIteration = 1;
	private float modeChangeTimer = 0;

	public enum Mode{
		Chase,
		Scatter,
		Frightened
	}

	Mode currentMode = Mode.Scatter;
	Mode previousMode;

	public enum GhostType{
		Red,
		Pink,
		Blue,
		Orange
	}

	public GhostType ghostType = GhostType.Red;

	private GameObject pacMan;

	private Node currentNode, targetNode, previousNode;
	private Vector2 direction, nextDirection;

	// Use this for initialization
	void Start () {
		pacMan = GameObject.FindGameObjectWithTag ("PacMan");
		//Debug.Log (pacMan);
		Node node = GetNodeAtPosition (transform.position);

		if (node != null) {
			currentNode = node;
		}

		if (isInGhostHouse) {
			direction = Vector2.up;
			targetNode = currentNode.neighbors [0];
		} else {
			direction = Vector2.left;
			targetNode = ChooseNextNode ();
		}

		//direction = Vector2.right;

		previousNode = currentNode;

		//Vector2 pacmanPosition = pacMan.transform.position;
		//Vector2 targetTile = new Vector2 (Mathf.RoundToInt(pacmanPosition.x), Mathf.RoundToInt(pacmanPosition.y));
		//targetNode = GetNodeAtPosition (targetTile);
		//Debug.Log ("nombre de TargetNode: " + targetNode.transform.position);
		//Debug.Log ("nombre de local: " + previousNode.transform.position);
	}
	
	// Update is called once per frame
	void Update () {
		ModeUpdate ();
		Move ();
		ReleaseGhost ();
	}

	void Move(){
		if(targetNode != currentNode && targetNode != null && !isInGhostHouse){
			if (OverShotTarget ()) {
				currentNode = targetNode;
				transform.localPosition = currentNode.transform.position;
				GameObject otherPortal = GetPortal (currentNode.transform.position);

				if (otherPortal != null) {
					transform.localPosition = otherPortal.transform.position;
					currentNode = otherPortal.GetComponent<Node> ();
				}

				targetNode = ChooseNextNode ();
				previousNode = currentNode;
				currentNode = null;
			} else {
				transform.localPosition += (Vector3)direction * moveSpeed * Time.deltaTime;
			}
		}
	}

	void ModeUpdate(){
		if(currentMode != Mode.Frightened){
			modeChangeTimer += Time.deltaTime;

			if(modeChangeIteration == 1){
				if(currentMode == Mode.Scatter && modeChangeTimer > scatterModeTimer1){
					ChangeMode (Mode.Chase);
					modeChangeTimer = 0;
				}

				if(currentMode == Mode.Chase && modeChangeTimer > chaseModeTimer1){
					modeChangeIteration = 2;
					ChangeMode (Mode.Scatter);
					modeChangeTimer = 0;
				}
			}else if(modeChangeIteration == 2){
				if(currentMode == Mode.Scatter && modeChangeTimer > scatterModeTimer2){
					ChangeMode (Mode.Chase);
					modeChangeTimer = 0;
				}

				if(currentMode == Mode.Chase && modeChangeTimer > chaseModeTimer2){
					modeChangeIteration = 3;
					ChangeMode (Mode.Scatter);
					modeChangeTimer = 0;
				}
			}
			else if(modeChangeIteration == 3){
				if(currentMode == Mode.Scatter && modeChangeTimer > scatterModeTimer3){
					ChangeMode (Mode.Chase);
					modeChangeTimer = 0;
				}

				if(currentMode == Mode.Chase && modeChangeTimer > chaseModeTimer3){
					modeChangeIteration = 4;
					ChangeMode (Mode.Scatter);
					modeChangeTimer = 0;
				} //
			}
			else if(modeChangeIteration == 4){
				if(currentMode == Mode.Scatter && modeChangeTimer > scatterModeTimer4){
					ChangeMode (Mode.Chase);
					modeChangeTimer = 0;
				}
			}
		}else if (currentMode == Mode.Frightened){
			
		} 
	}

	void ChangeMode(Mode m){
		currentMode = m;
	}

	Vector2 GetRedGhostTargetTile(){
		Vector2 pacManPosition = pacMan.transform.localPosition;
		Vector2 targetTile = new Vector2 (Mathf.RoundToInt(pacManPosition.x), Mathf.RoundToInt(pacManPosition.y));

		return targetTile;
	}

	Vector2 GetPinkGhostTargetTile(){ //4 Tiles ahead of Pacman (according to him pos and direction)
		Vector2 pacManPosition = pacMan.transform.position;
		Vector2 pacManOrientation = pacMan.GetComponent<PacMan> ().orientation;

		int pacManPositionX = Mathf.RoundToInt (pacManPosition.x);
		int pacManPositionY = Mathf.RoundToInt (pacManPosition.y);

		Vector2 pacManTile = new Vector2 (pacManPositionX, pacManPositionY);
		Vector2 targetTile = pacManTile + (4 * pacManOrientation);

		return targetTile;
	}

	Vector2 GetBlueGhostTargetTile(){
		// (Vector FROM BlueGhost TO Two tiles in front of Pacman) *2
		Vector2 pacManPosition = pacMan.transform.localPosition;
		Vector2 pacManOrientation = pacMan.GetComponent<PacMan> ().orientation;

		int pacManPositionX = Mathf.RoundToInt (pacManPosition.x);
		int pacManPositionY = Mathf.RoundToInt (pacManPosition.y);

		Vector2 pacManTile = new Vector2 (pacManPositionX, pacManPositionY);

		Vector2 targetTile = pacManTile + (2 * pacManOrientation);

		// Temporary BlueGhost position
		Vector2 tempBlueGhostPosition = GameObject.Find("ghost_blue").transform.localPosition;

		int blueGhostPositionX = Mathf.RoundToInt (tempBlueGhostPosition.x);
		int blueGhostPositionY = Mathf.RoundToInt (tempBlueGhostPosition.y);

		tempBlueGhostPosition = new Vector2 (blueGhostPositionX, blueGhostPositionY);

		float distance = GetDistance (tempBlueGhostPosition, targetTile);
		distance *= 2;

		targetTile = new Vector2 (tempBlueGhostPosition.x + distance, tempBlueGhostPosition.y + distance);

		return targetTile;
	}

	Vector2 GetOrangeGhostTargetTile(){
		//calculates distance from pacman.
		//if (distance > 8 tiles){ targeting = blueGhost} else {targeting = SCATTER}
		Vector2 pacManPosition = pacMan.transform.position;

		float distance = GetDistance (transform.localPosition, pacManPosition);
		Vector2 targetTile = Vector2.zero;

		if(distance > 8){
			targetTile = new Vector2 (Mathf.RoundToInt(pacManPosition.x), Mathf.RoundToInt(pacManPosition.y));
		}else if(distance < 8){
			targetTile = homeNode.transform.position;
		}

		return targetTile;
	}

	Vector2 GetTargetTile(){
		Vector2 targetTile = Vector2.zero;

		if(ghostType == GhostType.Red){
			targetTile = GetRedGhostTargetTile ();
		}else if(ghostType == GhostType.Pink){
			targetTile = GetPinkGhostTargetTile();
		}else if(ghostType == GhostType.Blue){
			targetTile = GetBlueGhostTargetTile();
		}else if(ghostType == GhostType.Orange){
			targetTile = GetOrangeGhostTargetTile();
		}

		return targetTile;
	}

	void ReleasePinkGhost(){
		if(ghostType == GhostType.Pink && isInGhostHouse){
			isInGhostHouse = false;
		}
	}

	void ReleaseBlueGhost(){
		if(ghostType == GhostType.Blue && isInGhostHouse){
			isInGhostHouse = false;
		}
	}

	void ReleaseOrangeGhost(){
		if(ghostType == GhostType.Orange && isInGhostHouse){
			isInGhostHouse = false;
		}
	}

	void ReleaseGhost(){
		ghostReleaseTimer += Time.deltaTime;
		if(ghostReleaseTimer > pinkyReleaseTimer){
			ReleasePinkGhost ();
		}
		if(ghostReleaseTimer > blueReleaseTimer){
			ReleaseBlueGhost ();
		}
		if(ghostReleaseTimer > orangeReleaseTimer){
			ReleaseOrangeGhost ();
		}
	}

	Node ChooseNextNode(){
		Vector2 targetTile = Vector2.zero;

		if (currentMode == Mode.Chase) {
			targetTile = GetTargetTile ();
		} else if(currentMode == Mode.Scatter){
			targetTile = homeNode.transform.position;
		}

		//Vector2 pacmanPosition = pacMan.transform.position;
		//targetTile = new Vector2 (Mathf.RoundToInt(pacmanPosition.x), Mathf.RoundToInt(pacmanPosition.y));
		//targetTile = GetTargetTile();

		Node moveToNode = null;

		Node[] foundNodes = new Node[4];
		Vector2[] foundNodesDirection = new Vector2[4];

		int nodeCounter = 0;

		for(int i=0; i<currentNode.neighbors.Length; i++){
			if(currentNode.validDirections[i] != direction * -1){
				foundNodes [nodeCounter] = currentNode.neighbors [i];
				foundNodesDirection [nodeCounter] = currentNode.validDirections [i];
				nodeCounter++;
			}
		}

		if(foundNodes.Length == 1){
			moveToNode = foundNodes [0];
			direction = foundNodesDirection [0];
		}

		if(foundNodes.Length > 1){
			float leastDistance = 100000f;

			for (int i = 0; i < foundNodes.Length; i++) {
				if(foundNodesDirection [i] != Vector2.zero){
					float distance = GetDistance (foundNodes[i].transform.position, targetTile);
					if(distance < leastDistance){
						leastDistance = distance;
						moveToNode = foundNodes [i];
						direction = foundNodesDirection [i];
					}
				}
			}
		}

		return moveToNode;
	}

	Node GetNodeAtPosition(Vector2 pos){
		GameObject tile = GameObject.Find("Game").GetComponent<GameBoard> ().board [(int)pos.x, (int)pos.y];

		if(tile != null){
			if(tile.GetComponent<Node> () != null){
				return tile.GetComponent<Node> ();
			}
		}

		return null;
	}

	GameObject GetPortal(Vector2 pos){
		GameObject tile = GameObject.Find ("Game").GetComponent<GameBoard> ().board [(int)pos.x, (int)pos.y];

		if (tile != null) {
			if (tile.GetComponent<Tile>() != null) {
				if (tile.GetComponent<Tile> ().isPortal) {
					GameObject otherPortal = tile.GetComponent<Tile> ().portalReceiver;
					return otherPortal;
				}
			}
		}

		return null;
	}

	float LengthFromNode (Vector2 targetPosition){
		Vector2 vec = targetPosition - (Vector2)previousNode.transform.position;
		return vec.sqrMagnitude;
	}

	bool OverShotTarget(){
		float nodeToTarget = LengthFromNode (targetNode.transform.position);
		float nodeToSelf = LengthFromNode (transform.localPosition);

		return nodeToSelf > nodeToTarget;
	}

	float GetDistance(Vector2 posA, Vector2 posB){
		float dx = posA.x - posB.x;
		float dy = posA.y - posB.y;

		float distance = Mathf.Sqrt (dx*dx + dy*dy);

		return distance;
	}
}
