using UnityEngine;

public class LongNoteController : NoteControllerBase
{
	[SerializeField] GameObject objBegin; // 始点部分のGameObject
	[SerializeField] GameObject objTrail; // 軌跡部分のGameObject
	[SerializeField] GameObject objEnd; // 終点部分のGameObject

	void Update()
	{
		SetTransform();
		CheckMiss();
	}

	// ノーツの座標設定
	private void SetTransform()
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

	// 見逃し検出
	private void CheckMiss()
	{
		// 処理中でない状態で始点が判定ラインを通過し、
		// BADの判定幅よりも離れるとノーツを削除
		if (!isProcessed &&
			noteProperty.secBegin - PlayerController.CurrentSec <
			-JudgementManager.JudgementWidth[JudgementType.Bad])
		{
			// リストから削除
			PlayerController.ExistingNoteControllers.Remove(
				GetComponent<NoteControllerBase>()
			);
			// GameObject自体も削除
			Destroy(gameObject);
		}

		// 処理中の状態で終点が判定ラインを通過し、
		// BADの判定幅よりも離れるとノーツを削除
		if (isProcessed &&
			noteProperty.secEnd - PlayerController.CurrentSec <
			-JudgementManager.JudgementWidth[JudgementType.Bad])
		{
			// 処理中フラグを解除
			isProcessed = false;
			// リストから削除
			PlayerController.ExistingNoteControllers.Remove(
				GetComponent<NoteControllerBase>()
			);
			// GameObject自体も削除
			Destroy(gameObject);
		}
	}

	// キーが押された時
	public override void OnKeyDown(JudgementType judgementType)
	{
		// コンソールに判定を表示
		Debug.Log(judgementType);
		if (judgementType != JudgementType.Miss)
		{
			// 処理中フラグを付ける
			isProcessed = true;
		}
	}

	// キーが離された時
	public override void OnKeyUp(JudgementType judgementType)
	{
		// コンソールに判定を表示
		Debug.Log(judgementType);
		// 処理中フラグを解除
		isProcessed = false;
		//リストから削除
		PlayerController.ExistingNoteControllers.Remove(
			GetComponent<NoteControllerBase>()
		);
		// GameObject自体も削除
		Destroy(gameObject);
	}
}