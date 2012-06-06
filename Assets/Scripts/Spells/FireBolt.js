#pragma strict
class FireBolt extends Spell {
	var speed : float = 2;
	var prefab : GameObject = Resources.Load("FireBolt", GameObject);
	
	private var instance : GameObject;
	
	function Cast(playerTrans : Transform) {
		Debug.Log('firing firebolt..');
	
		// test for collision
		var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		var hit : RaycastHit;
		var targetPos : Vector3 = (playerTrans.position + (playerTrans.forward * 100));
		if (Physics.Raycast(ray, hit, 100))
		{
			targetPos = hit.point;
		}
		Debug.Log(targetPos.x + ', '+ targetPos.y + ', '+ targetPos.z);
	
		// create the particles/objects
		instance = GameObject.Instantiate(prefab, playerTrans.position, playerTrans.rotation) as GameObject;
		var instStraightScript : Straight = instance.GetComponent(Straight);
		instStraightScript.speed = speed;
		instStraightScript.target = targetPos;
		instStraightScript.moving = true;
	}
	
}
