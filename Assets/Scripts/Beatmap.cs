using System.Collections.Generic;
using System.IO; // FileInfoを使うために必須
using System.Linq;

public class Beatmap
{
	public List<NoteProperty> noteProperties = new List<NoteProperty>();
	public List<TempoChange> tempoChanges = new List<TempoChange>();
	public string audioFilePath = ""; // 楽曲ファイルの絶対パス
	public float audioOffset; // 音源再生オフセット

	// (コンストラクタ) BMSファイルを読み込む
	public Beatmap(string filePath)
	{
		// BMSファイルの入っているフォルダの絶対パス
		var bmsDirectory = new FileInfo(filePath).DirectoryName;
		var bmsLoader = new BmsLoader(filePath);

		noteProperties = bmsLoader.noteProperties;
		tempoChanges = bmsLoader.tempoChanges;

		// 音源が指定されているとき
		if (bmsLoader.headerData.ContainsKey("WAV01"))
		{
			// 楽曲ファイルの絶対パス（フォルダ名とファイル名をつなげる）
			audioFilePath = bmsDirectory + "/" + bmsLoader.headerData["WAV01"];
		}
		// 音源再生オフセットを設定
		audioOffset = bmsLoader.audioOffset;
	}

	// 指定されたテンポで、 beatをsecへ変換する
	public static float ToSecWithFixedTempo(float beat, float tempo)
	{
		return beat / (tempo / 60f);
	}

	// 指定されたテンポで、secをbeatへ変換する
	public static float ToBeatWithFixedTempo(float sec, float tempo)
	{
		return sec * (tempo / 60f);
	}

	// テンポ変化情報を基に、beatをsecへ変換する
	public static float ToSec(float beat, List<TempoChange> tempoChanges)
	{
		// accumulatedSec: 累計の秒数
		float accumulatedSec = 0f;
		// i: テンポ変化番号
		int i = 0;
		// n: 変換するbeatの直前までのテンポ変化の回数
		var n = tempoChanges.Count(x => x.beat <= beat);

		// 変換するbeatの直前にあるテンポ変化までのsecを求める
		while (i < n - 1)
		{
			accumulatedSec += ToSecWithFixedTempo(
				tempoChanges[i + 1].beat - tempoChanges[i].beat,
				tempoChanges[i].tempo
			);
			i++;
		}
		// 残りのbeat分を足す
		accumulatedSec += ToSecWithFixedTempo(
			beat - tempoChanges[i].beat, tempoChanges[i].tempo
		);
		return accumulatedSec;
	}

	// テンポ変化情報を基に、secをbeatへ変換する
	public static float ToBeat(float sec, List<TempoChange> tempoChanges)
	{
		// accumulatedSec: 累計の秒数
		float accumulatedSec = 0;
		// i: テンポ変化番号
		int i = 0;
		// n: 全てのテンポ変化の回数
		var n = tempoChanges.Count;

		// 最後から1つ前のテンポ変化までループ
		while (i < n - 1)
		{
			// tmpSec: i回目のテンポ変化地点での秒数
			var tmpSec = accumulatedSec;
			// 次(i+1回目)のテンポ変化タイミング(秒)を計算する
			accumulatedSec += ToSecWithFixedTempo(
				tempoChanges[i + 1].beat - tempoChanges[i].beat,
				tempoChanges[i].tempo
			);
			if (accumulatedSec >= sec)
			{
				// 次のテンポ変化タイミングが変換するsecを超えた場合、
				//「超える直前のテンポ変化があるbeat + 残りのbeat」を返す
				return tempoChanges[i].beat +
					ToBeatWithFixedTempo(
						sec - tmpSec, tempoChanges[i].tempo
					);
			}
			i++;
		}

		// 変換するsecが最後のテンポ変化よりも後にある時、
		//「最後のテンポ変化があるbeat + 残りのbeat」を返す
		return tempoChanges[n - 1].beat +
			ToBeatWithFixedTempo(
				sec - accumulatedSec, tempoChanges[n - 1].tempo
			);
	}
}