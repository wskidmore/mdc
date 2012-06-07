#pragma strict
class Spell {
	function Cast(playerTrans : Transform){
		Debug.Log('casting spell from: '+playerTrans.position.x+', '+playerTrans.position.y);
	}
	
	function CanLearn(player) {
		return true;
	}
	
	function CanCast(currentMP) {
		return true;
	}
}

class SpellInfo {
	var type : String = "Damage"; // Damage, Buff, Debuff
	var duration : float = 0; // 0 immediate/none, otherwise in seconds
	var damage : float = 0; // before defenses applied, after any plus mods
	var element : String; // fire, ice, water, etc for resistence checks
}

