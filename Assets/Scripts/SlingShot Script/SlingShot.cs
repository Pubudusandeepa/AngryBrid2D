using UnityEngine;
using System.Collections;

public class SlingShot : MonoBehaviour {

	private Vector3 slingShootMiddleVector;

	[HideInInspector]
	public SlingshotState slingShootState;

	public Transform leftslingShootOrigin, rightslingShootOrigin;

	public LineRenderer slingShootLineRenderer1, slingShootLineRenderer2, trajectoryLineRenderer;

	[HideInInspector]
	public GameObject birdToThrow;

	public Transform birdWaitPosition;

	public float throwSpeed;

	[HideInInspector]
	public float timeSinceThrown;

	public delegate void BirdThrown ();
	public event BirdThrown birdThrown;

	// Use this for initialization
	void Awake () {
		slingShootLineRenderer1.sortingLayerName = "Foreground";
		slingShootLineRenderer2.sortingLayerName = "Foreground";
		trajectoryLineRenderer.sortingLayerName = "Foreground";

		slingShootState = SlingshotState.Idel;
		slingShootLineRenderer1.SetPosition (0, leftslingShootOrigin.position);
		slingShootLineRenderer2.SetPosition (0, rightslingShootOrigin.position);

		slingShootMiddleVector = new Vector3 ((leftslingShootOrigin.position.x + rightslingShootOrigin.position.x) / 2, (leftslingShootOrigin.position.y + leftslingShootOrigin.position.y) / 2, 0);
	}
	
	// Update is called once per frame
	void Update () {
		switch(slingShootState){
		case SlingshotState.Idel:
			initializeBird ();
			DisplaySlingshootLineRenderers ();

			if (Input.GetMouseButtonDown (0)) {
				Vector3 location = Camera.main.ScreenToWorldPoint (Input.mousePosition);

				if (birdToThrow.GetComponent<CircleCollider2D> () == Physics2D.OverlapPoint (location)) {
					slingShootState = SlingshotState.UserPulling;
				}
					
			}
			break;

		case SlingshotState.UserPulling:

			DisplaySlingshootLineRenderers ();

			if (Input.GetMouseButton (0)) {
				Vector3 location = Camera.main.ScreenToWorldPoint (Input.mousePosition);
				location.z = 0f;

				if (Vector3.Distance (location, slingShootMiddleVector) > 2f) {
					var maxPosition = (location - slingShootMiddleVector).normalized * 2f + slingShootMiddleVector;
					birdToThrow.transform.position = maxPosition;
				} else {
					birdToThrow.transform.position = location;
				}
				var distance = Vector3.Distance (slingShootMiddleVector, birdToThrow.transform.position);
				DisplayTrajectoryLineRenderer (distance);
			} else {
				DisplaySlingshootLineRenderers ();
				timeSinceThrown = Time.time;
				float distance = Vector3.Distance (slingShootMiddleVector, birdToThrow.transform.position);
				if (distance > 1) {
					SetSlingshotLinerenderersActive (false);
					slingShootState = SlingshotState.BirdFlying;
					ThrowBird (distance);
				} else {
					birdToThrow.transform.positionTo (distance / 10, birdWaitPosition.position);
					initializeBird ();  
				}

			}
			break;
		}
	}

	private void initializeBird() {
		birdToThrow.transform.position = birdWaitPosition.position;
		slingShootState = SlingshotState.Idel;
		SetSlingshotLinerenderersActive (true);
	}

	void SetSlingshotLinerenderersActive(bool active) {
		slingShootLineRenderer1.enabled = active;
		slingShootLineRenderer2.enabled = active;

	}

	void DisplaySlingshootLineRenderers() {
		slingShootLineRenderer1.SetPosition (1, birdToThrow.transform.position);
		slingShootLineRenderer2.SetPosition (1, birdToThrow.transform.position);
	}

	void SetTrajectoryLineRendererActive(bool active){
		trajectoryLineRenderer.enabled = active;
	}

	void DisplayTrajectoryLineRenderer(float distance) {
		SetTrajectoryLineRendererActive (true);

		Vector3 v2 = slingShootMiddleVector - birdToThrow.transform.position;
		int segmentCount = 15;

		Vector2[] segments = new Vector2[segmentCount];

		segments [0] = birdToThrow.transform.position;

		Vector2 segVelocity = new Vector2 (v2.x, v2.y) * throwSpeed * distance;

		for (int i=1; i< segmentCount; i++){
			float time = i * Time.fixedDeltaTime * 5f;
			segments [i] = segments [0] + segVelocity * time + 0.5f * Physics2D.gravity * Mathf.Pow (time, 2);

		}

		trajectoryLineRenderer.SetVertexCount (segmentCount);
		for (int i = 0; i < segmentCount; i++) {
			trajectoryLineRenderer.SetPosition (i, segments [i]);
		}
			

	}
	private void ThrowBird(float distance){
		Vector3 velocity = slingShootMiddleVector - birdToThrow.transform.position;

		birdToThrow.GetComponent<Bird> ().OnThrow ();

		birdToThrow.GetComponent<Rigidbody2D> ().velocity = new Vector2 (velocity.x, velocity.y) * throwSpeed * distance;

		if (birdThrown != null)
			birdThrown ();

	}


		

}
