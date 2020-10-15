using UnityEngine;
using System.Collections;

public enum SlingshotState {
	Idel,
	UserPulling,
	BirdFlying
}

public enum GameState {
	Start,
	BirdMovingToSligshot,
	Playing,
	Won,
	Lost

}

public enum BirdState {
	BeforeThrown,
	Thrown
}

