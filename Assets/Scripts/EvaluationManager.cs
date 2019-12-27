using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EvaluationManager : MonoBehaviour
{
	// スコア理論値
	public static readonly float TheoreticalScore = 1000000;
	// 各判定の時のスコア増加率(Perfectの時を1とする)
	private static readonly Dictionary<JudgementType, float> ScoreAddRates =
		new Dictionary<JudgementType, float>()
		{
			{ JudgementType.Perfect, 1.0f },
			{ JudgementType.Good, 0.5f },
			{ JudgementType.Bad, 0.2f },
			{ JudgementType.Miss, 0.0f }
		};
	// 現在のスコア
	public static float Score;
	// 現在のコンボ数
	public static int Combo;
	// プレイ中の最大コンボ数
	public static int MaxCombo;
	// コンボ数理論値
	public static int TheoreticalCombo;
	// 各判定種別の個数
	public static Dictionary<JudgementType, int> JudgementCounts =
		new Dictionary<JudgementType, int>();

	// 各値を初期化
	void Start()
	{
		Score = 0f;
		Combo = 0;
		MaxCombo = 0;
		JudgementCounts[JudgementType.Miss] = 0;
		JudgementCounts[JudgementType.Bad] = 0;
		JudgementCounts[JudgementType.Good] = 0;
		JudgementCounts[JudgementType.Perfect] = 0;

		// ロングノーツは始点と終点で別々にカウントするので、
		// 理論コンボ数は
		// (シングルノーツの個数) + (ロングノーツの個数) × 2
		TheoreticalCombo =
			PlayerController.beatmap.noteProperties
			.Count(x => x.noteType == NoteType.Single) +
			PlayerController.beatmap.noteProperties
			.Count(x => x.noteType == NoteType.Long) * 2;
	}

	void Update()
	{
		// 最大コンボ数を更新
		MaxCombo = Mathf.Max(Combo, MaxCombo);
	}

	// ノーツを処理した際に呼び出される
	public static void OnHit(JudgementType judgementType)
	{
		// コンボ数をインクリメント
		Combo++;

		// 1ノーツあたりのPerfect時のスコア増加量
		// (スコア理論値) ÷ (コンボ数理論値)
		var addValue = TheoreticalScore / TheoreticalCombo;
		// 判定に応じてスコアを増やす
		Score += addValue * ScoreAddRates[judgementType];

		// 対応するJudgementCountをインクリメント
		JudgementCounts[judgementType]++;
	}

	// ノーツを見逃した際に呼び出される
	public static void OnMiss()
	{
		// コンボ数を0に
		Combo = 0;
		// 見逃し回数をインクリメント
		JudgementCounts[JudgementType.Miss]++;
	}
}