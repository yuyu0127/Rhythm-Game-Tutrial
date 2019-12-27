using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	[SerializeField] private GameObject prefabSingleNote; // 生成するPrefab

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
		// 現段階では手打ち

		// Beatmapクラスのインスタンスを作成
		beatmap = new Beatmap();

		// ノーツ配置情報を設定
		beatmap.noteProperties = new List<NoteProperty>
		{
			new NoteProperty(0, 0, 0, NoteType.Single),
			new NoteProperty(1, 1, 1, NoteType.Single),
			new NoteProperty(2, 2, 2, NoteType.Single),
			new NoteProperty(3, 3, 1, NoteType.Single),
			new NoteProperty(4, 4, 0, NoteType.Single),
			new NoteProperty(4, 4, 4, NoteType.Single),
			new NoteProperty(5, 5, 3, NoteType.Single),
			new NoteProperty(6, 6, 2, NoteType.Single),
			new NoteProperty(7, 7, 3, NoteType.Single),
			new NoteProperty(8, 8, 4, NoteType.Single)
		};

		// テンポ変化を設定
		beatmap.tempoChanges = new List<TempoChange>
		{
			new TempoChange(0, 60f),
			new TempoChange(2, 120f),
			new TempoChange(4, 60f),
			new TempoChange(6, 120f)
		};

		// ノーツの生成を行う
		foreach (var noteProperty in beatmap.noteProperties)
		{
			// beatmapのnotePropertiesの各要素の情報からGameObjectを生成
			var objNote = Instantiate(prefabSingleNote);
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