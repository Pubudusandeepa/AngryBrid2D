using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class GameManager : MonoBehaviour {

	public CameraFollow cameraFollow;

	int currentBirdIndex;

	public SlingShot slingShot;

	[HideInInspector]
	public static GameState gameState;

	private List<GameObject> bricks;
	private List<GameObject> birds;
	private List<GameObject> pigs;

	// Use this for initialization
	void Awake () {
		gameState = GameState.Start;
		slingShot.enabled = false;

		bricks = new List<GameObject> (GameObject.FindGameObjectsWithTag ("Brick"));
		birds = new List<GameObject> (GameObject.FindGameObjectsWithTag("Bird"));
		pigs = new List<GameObject> (GameObject.FindGameObjectsWithTag("Pig"));
	}

	void OnEnable() {
		slingShot.birdThrown += SlingShotBirdThrown;
	}

	void OnDisable() {
		slingShot.birdThrown -= SlingShotBirdThrown;
	}


	// Update is called once per frame
	void Update () {
		switch(gameState) {
		case  GameState.Start:
			if (Input.GetMouseButtonUp (0)) {
				AnimateBirdToSlingshot ();
			}
			break;
		case GameState.Playing:
			if (slingShot.slingShootState == SlingshotState.BirdFlying &&
			   (BricksBirdsPigsStoppedMoving () || Time.time - slingShot.timeSinceThrown > 5f)) {
				slingShot.enabled = false;
				AnimateCameraToStartPosition ();
				gameState = GameState.BirdMovingToSligshot;
			}
			break;

		case GameState.Won:
		case GameState.Lost:
			if (Input.GetMouseButtonDown (0)) {
				Application.LoadLevel (Application.loadedLevelName);
			}
			break;

		}
	}

	void AnimateBirdToSlingshot() {
		gameState = GameState.BirdMovingToSligshot;
		birds [currentBirdIndex].transform.positionTo (Vector2.Distance (birds [currentBirdIndex].transform.position / 10, slingShot.birdWaitPosition.position) / 10, 
			slingShot.birdWaitPosition.position).
		setOnInitHandler ((x) => {
			x.complete ();
			x.destroy ();

			gameState = GameState.Playing;
			slingShot.enabled = true;

			slingShot.birdToThrow = birds [currentBirdIndex];
		});          
	}

	bool BricksBirdsPigsStoppedMoving() {
		foreach(var item in bricks.Union(birds).Union(pigs)) {
			if(item != null && item.GetComponent<Rigidbody2D>().velocity.sqrMagnitude > GameVariables.MinVelocity) {
				return false;
			}
		}
		return true;
	}

	private bool AllPigsAreDestroyed() {
		return pigs.All (x => x == null);
	}

	private void AnimateCameraToStartPosition() {
		float duration = Vector2.Distance (Camera.main.transform.position, cameraFollow.startingPosition) / 10f;
		if (duration == 0.0f)
			duration = 0.1f;

		Camera.main.transform.positionTo (duration, cameraFollow.startingPosition).
		setOnCompleteHandler ((x) => {
			cameraFollow.isFollowing = false;
			if (AllPigsAreDestroyed ()) {
				gameState = GameState.Won;
			} else if (currentBirdIndex == birds.Count - 1) {
				gameState = GameState.Lost;
			} else {
				slingShot.slingShootState = SlingshotState.Idel;
				currentBirdIndex++;
				AnimateBirdToSlingshot ();
			}
		});
	}

	private void SlingShotBirdThrown() {
		cameraFollow.birdToFollow = birds [currentBirdIndex].transform;
		cameraFollow.isFollowing = true;
	}
}
