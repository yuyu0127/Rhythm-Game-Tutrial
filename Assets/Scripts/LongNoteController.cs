using UnityEngine;

public class LongNoteController : NoteControllerBase
{
	[SerializeField] GameObject objBegin; // 始点部分のGameObject
	[SerializeField] GameObject objTrail; // 軌跡部分のGameObject
	[SerializeField] GameObject objEnd; // 終点部分のGameObject

	void Update()
	{
		// 始点の座標（beatBeginによる指定）
		Vector2 positionBegin = new Vector2();
		positionBegin.x = noteProperty.lane - 2;
		positionBegin.y =
			(noteProperty.beatBegin - PlayerController.CurrentBeat) *
			PlayerController.ScrollSpeed;
		objBegin.transform.localPosition = positionBegin;

		// 終点の座標（beatEndによる指定）
		Vector2 positionEnd = new Vector2();
		positionEnd.x = noteProperty.lane - 2;
		positionEnd.y =
			(noteProperty.beatEnd - PlayerController.CurrentBeat) *
			PlayerController.ScrollSpeed;
		objEnd.transform.localPosition = positionEnd;

		// 軌跡部分の座標は始点と終点の中心に設定
		Vector2 positionTrail = (positionBegin + positionEnd) / 2f;
		objTrail.transform.localPosition = positionTrail;

		// 軌跡部分のY拡大率は終点の始点の座標の差に設定
		Vector2 scale = objTrail.transform.localScale;
		scale.y = positionEnd.y - positionBegin.y;
		objTrail.transform.localScale = scale;
	}
}