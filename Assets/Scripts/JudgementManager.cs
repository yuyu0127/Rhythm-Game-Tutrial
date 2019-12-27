using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class JudgementManager : MonoBehaviour
{
	public static Dictionary<JudgementType, float> JudgementWidth =
		new Dictionary<JudgementType, float>
		{
			{ JudgementType.Perfect, 0.05f }, // PERFECTの判定幅
			{ JudgementType.Good, 0.20f }, // GOODの判定幅
			{ JudgementType.Bad, 0.30f } // BADの判定幅
		};

	// 各レーンに対応するキー
	private static KeyCode[] InputKeys = new KeyCode[]
	{
		KeyCode.C,
		KeyCode.V,
		KeyCode.B,
		KeyCode.N,
		KeyCode.M,
	};

	private void Update()
	{
		// 各レーンに対して処理
		for (int lane = 0; lane < InputKeys.Length; lane++)
		{
			// レーンに対応するキー
			var inputKey = InputKeys[lane];
			// レーンに対応するキーが押された時
			if (Input.GetKeyDown(inputKey))
			{
				// 最近傍のノーツのNoteControllerBaseを取得
				var nearest = GetNearestNoteControllerBaseInLane(lane);
				// ノーツがレーンに無ければスキップ
				if (!nearest) continue;

				// 最近傍のノーツを処理すべきタイミング(sec)
				var noteSec = nearest.noteProperty.secBegin;
				// 処理すべきタイミングと
				// 実際にキーが押されたタイミングの差の絶対値
				var differenceSec = Mathf.Abs(noteSec - PlayerController.CurrentSec);
				// 最近傍のノーツのOnKeyDownを呼び出し
				nearest.OnKeyDown(GetJudgementType(differenceSec));
			}
			// レーンに対応するキーが離されたとき
			else if (Input.GetKeyUp(inputKey))
			{
				// 処理中のノーツ
				var processed = GetProcessedNoteControllerBaseInLane(lane);
				// 処理中のノーツが存在しなければスキップ
				if (!processed) continue;

				// ノーツの終点を処理すべきタイミング(sec)
				var noteSec = processed.noteProperty.secEnd;
				// 処理すべきタイミングと
				// 実際にキーが離されたタイミングの差の絶対値
				var differenceSec = Mathf.Abs(noteSec -
					PlayerController.CurrentSec);
				// ノーツのOnKeyUpを呼び出し
				processed.OnKeyUp(GetJudgementType(differenceSec));
			}
			
		}
	}

	// タイミングの差の絶対値から判定種別を取得する
	private JudgementType GetJudgementType(float differenceSec)
	{
		// PERFECT幅以内のとき
		if (differenceSec <= JudgementWidth[JudgementType.Perfect])
		{
			return JudgementType.Perfect;
		}
		// GOOD幅以内のとき
		else if (differenceSec <= JudgementWidth[JudgementType.Good])
		{
			return JudgementType.Good;
		}
		// BAD幅以内のとき
		else if (differenceSec <= JudgementWidth[JudgementType.Bad])
		{
			return JudgementType.Bad;
		}
		// それ以外のとき
		else
		{
			return JudgementType.Miss;
		}
	}

	// 指定したレーン内の最近傍のノーツを取得（存在しなければnull）
	private NoteControllerBase GetNearestNoteControllerBaseInLane(int lane)
	{
		// 指定したレーン内のノーツ
		var noteControllers =
			PlayerController.ExistingNoteControllers
			.Where(x => x.noteProperty.lane == lane);
		// 指定したレーン内にノーツが存在するとき
		if (noteControllers.Any())
		{
			// beatの差の絶対値が小さい順に並び替え→ 先頭の要素を取得して返す
			return noteControllers
				.OrderBy(x => Mathf.Abs(
					x.noteProperty.beatBegin - PlayerController.CurrentBeat
				))
				.First();
		}
		// 指定したレーン内にノーツが存在しないとき
		else
		{
			// nullを返す
			return null;
		}
	}

	// 指定したレーン内の処理中のノーツを取得（存在しなければnull）
	private NoteControllerBase GetProcessedNoteControllerBaseInLane(int lane)
	{
		// 指定したレーン内のノーツ
		var noteControllers =
			PlayerController.ExistingNoteControllers
			.Where(x => x.noteProperty.lane == lane && x.isProcessed);
		// 指定したレーン内にノーツが存在するとき
		if (noteControllers.Any())
		{
			// beatの差の絶対値が小さい順に並び替え
			// → 先頭の要素を取得して返す
			return noteControllers
				.OrderBy(x => Mathf.Abs(x.noteProperty.beatBegin -
					PlayerController.CurrentBeat))
				.First();
		}
		// 指定したレーン内にノーツが存在しないとき
		else
		{
			// nullを返す
			return null;
		}
	}
}

// 判定種別
public enum JudgementType
{
	Miss,
	Perfect,
	Good,
	Bad
}