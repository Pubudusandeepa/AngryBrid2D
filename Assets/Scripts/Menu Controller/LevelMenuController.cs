using UnityEngine;
using System.Collections;

public class LevelMenuController : MonoBehaviour {

	public void PlayGame(){
		Application.LoadLevel ("Gameplay");
	}

	public void GoBack(){
		Application.LoadLevel ("MainMenu");
	}
}
