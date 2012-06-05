#pragma strict
class Character {
	var level : int = 1;
	var exp : uint = 0;
	var hp : int = 1;
	var mp : int = 0;

	var strength : int = 10;
	var endurance : int = 10;
	var accuracy : int = 10;
	var speed : int = 10;
	var luck : int = 10;
	var intelligence : int = 10;
	var wisdom : int = 10;
	
	var activeSpell : Spell;

	function LevelUp() {
		level += 1;
	}
	
	function OnDefaultAction(playerTrans : Transform) {
		if (GameStateManager.dangerProximity == 3)
			MeleeAttack();
		else
			RangeAttack();
	}
	function OnQuickAction(playerTrans : Transform) {
		if (activeSpell)
			CastSpell(playerTrans);
		else
			OnDefaultAction(playerTrans);
	}
	
	function MeleeAttack() {
		Debug.Log('swing weapon');
	}
	function RangeAttack() {
		Debug.Log('shoot ranged');
	}
	function CastSpell(playerTrans : Transform) {
		if (activeSpell.CanCast(mp))
		{
			activeSpell.Cast(playerTrans);
		}
	}
}




@script RequireComponent (CharacterManager)
