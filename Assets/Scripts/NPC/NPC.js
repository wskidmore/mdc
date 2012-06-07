#pragma strict
var hp : float = 3;
var lootable : boolean = false;
var dead : boolean = false;
var hitAudio : AudioClip;
var dieAudio : AudioClip;
var attackAudio : AudioClip;

function Start () {

}

function Update () {
	if (hp <= 0 && !dead)
	{
		dead = true;
		// die, change to corpse to loot
		gameObject.audio.clip = dieAudio;
		gameObject.audio.Play();
		transform.gameObject.animation.Play("die");
		Destroy(gameObject, transform.gameObject.animation["die"].length);
	}

}

function OnSpellCast(spellInfo: SpellInfo) {
	Debug.Log("in OnSpellCast for NPC");
	hp -= spellInfo.damage;
	if (hp > 0)
	{
		transform.gameObject.animation.Play("gethit");
		if (hp % 2 == 0)
		{
			gameObject.audio.clip = hitAudio;
			gameObject.audio.Play();
		}
	}
}