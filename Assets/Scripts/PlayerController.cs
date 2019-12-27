using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	[SerializeField] private GameObject prefabSingleNote; // 生成するPrefab
	[SerializeField] private GameObject prefabLongNote; // 生成するPrefab

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

		// 読み込む譜面があるディレクトリのパス
		var beatmapDirectory = Application.dataPath + "/../Beatmaps";
		// Beatmapクラスのインスタンスを作成
		beatmap = new Beatmap(beatmapDirectory + "/sample1.bms");

		// 直打ちしていたノーツは配置情報を削除した

		// ノーツの生成を行う
		foreach (var noteProperty in beatmap.noteProperties)
		{
			// beatmapのnotePropertiesの各要素の情報からGameObjectを生成
			GameObject objNote = null;
			switch (noteProperty.noteType)
			{
				case NoteType.Single:
					objNote = Instantiate(prefabSingleNote);
					break;
				case NoteType.Long:
					objNote = Instantiate(prefabLongNote);
					break;
			}
			objNote.GetComponent<NoteControllerBase>().noteProperty = noteProperty;
		}
	}

	void Update()
	{
		// 秒数を更新
		CurrentSec = Time.time - startOffset;
		// 拍を更新(ToBeatを使用)
		CurrentBeat = Beatmap.ToBeat(CurrentSec, beatmap.tempoChanges);
	}
}