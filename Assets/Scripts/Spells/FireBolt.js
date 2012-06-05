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
		var targetPos : Vector3 = (playerTrans.position + Vector3(0, 0, 15));
		if (Physics.Raycast(ray, hit, 100))
		{
			targetPos = hit.point;
			Debug.Log('raycast hit');
		}
	
		instance = GameObject.Instantiate(prefab, playerTrans.position, playerTrans.rotation) as GameObject;

		Debug.Log(targetPos);

		MoveToPoint(instance.transform, playerTrans.position, targetPos, speed);
	}
	
}
