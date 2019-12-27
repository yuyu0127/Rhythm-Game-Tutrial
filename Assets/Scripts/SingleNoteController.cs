using UnityEngine;

public class SingleNoteController : NoteControllerBase
{
	void Update()
	{
		// ノーツの座標
		Vector2 position = new Vector2();
		position.x = noteProperty.lane - 2;
		position.y = (noteProperty.beatBegin - PlayerController.CurrentBeat) *
			PlayerController.ScrollSpeed;
		transform.localPosition = position;
	}
}