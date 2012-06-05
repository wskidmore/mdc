#pragma strict
var characters : Character[];
var activeIndex : int = 0;

function Start () {
	/*
	Load chars from save state
	*/
	
	
	// For debug only
	characters = new Character[3];
	characters[0] = new Warrior();
	characters[1] = new Warrior();
	characters[2] = new Character();
	
	characters[0].activeSpell = new FireBolt();

	// End Debug
	
}

function Update() {
	// detect danger
	GameStateManager.dangerProximity = 3;

	if (Input.GetButtonUp('Tab'))
		activeIndex = (activeIndex + 1 >= characters.length) ? 0 : activeIndex + 1;

	if (Input.GetButtonUp('Fire1'))
		characters[activeIndex].OnDefaultAction(transform);
	
	if (Input.GetButtonUp('Fire2'))
		characters[activeIndex].OnQuickAction(transform);

}

