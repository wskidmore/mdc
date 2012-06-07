#pragma strict
class FireBolt extends Spell {
	var cost : float = 1; // cost is required for all spells

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
			if (hit.collider.gameObject.tag == "mob")
			{
				var info = new SpellInfo();
				info.damage = GetDamage();
				info.element = "Fire";
				hit.collider.gameObject.SendMessage("OnSpellCast", info);
			}
		}
	
		// create the particles/objects
		instance = GameObject.Instantiate(prefab, playerTrans.position, playerTrans.rotation) as GameObject;
		instance.audio.Play();
		var instStraightScript : Straight = instance.GetComponent(Straight);
		instStraightScript.speed = speed;
		instStraightScript.target = targetPos;
		instStraightScript.moving = true;
		
	}
	
	function GetDamage(){
		return 1;
	}
}
