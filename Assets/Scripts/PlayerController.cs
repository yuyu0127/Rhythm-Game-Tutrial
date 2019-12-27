using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public static float ScrollSpeed = 1.0f; // 譜面のスクロール速度
	public static float CurrentSec = 0f; // 現在の経過時間(秒)
	public static float CurrentBeat = 0f; // 現在の経過時間(beat)

	public static Beatmap beatmap; // 譜面データを管理する
	private float startOffset = 1.0f; // 譜面のオフセット(秒)

	void Awake()
	{
		// 値を初期化
		CurrentSec = 0f;
		CurrentBeat = 0f;

		// TODO: ここで譜面の読み込みを行う

		// TODO: ここでノーツの生成を行う
	}

	void Update()
	{
		// 秒数を更新
		CurrentSec = Time.time - startOffset;
		// 拍を更新(ToBeatを使用)
		CurrentBeat = Beatmap.ToBeat(CurrentSec, beatmap.tempoChanges);
	}
}