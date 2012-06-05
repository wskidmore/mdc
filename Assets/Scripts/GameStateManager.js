#pragma strict

class GameStateManager extends MonoBehaviour {

	static var paused : boolean = false;
	static var dangerProximity : int = 0;

	function Update () {
		if (Input.GetButtonUp('Pause'))
		{
			paused = !paused;
			
		}
		
	}

}