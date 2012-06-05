#pragma strict
class Warrior extends Character {

function LevelUp() {
	super.LevelUp();
	
	hp += 10;
	strength += 5;
	endurance += 5;
	accuracy += 3;
	speed += 2;
}

}


