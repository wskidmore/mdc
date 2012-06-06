#pragma strict
var speed : float = 4;
var moving : boolean = false;
var finished : boolean = false;
var target : Vector3;

private var incr : float = 0;

function Update () {
	if (finished)
		Destroy(gameObject, .01);
	
	if (moving)
	{
		if(incr <=1)
		    incr += speed/100;
		else
			finished = true;
		    
		transform.position = Vector3.Lerp(transform.position, target, incr);
	}

}