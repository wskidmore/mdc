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

function MoveToPoint(objTransform : Transform, startPos : Vector3, endPos : Vector3, time : float) {
	var i = 0.0;
	var rate = 1.0/time;
	if (i < 1.0)
	{
		i += Time.deltaTime * rate;
		Debug.Log('moving at rate '+ i + ' from '+startPos + ' to '+endPos);
		objTransform.position = Vector3.Lerp(startPos, endPos, i);
		yield WaitForEndOfFrame();
		Move(objTransform, startPos, endPos, time - Time.deltaTime);
	}
}
}

function Move(objTransform : Transform, startPos : Vector3, endPos : Vector3, time : float) {
	MoveToPoint(objTransform, startPos, endPos, time);
}