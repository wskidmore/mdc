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

