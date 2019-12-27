using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

public class BmsLoader
{
	// メインデータの#nnnxx:....を表す正規表現
	private static string MainDataPattern　 = @"#([0-9]{3})([0-9A-Z]{2}):(.*)";

	// ヘッダーの正規表現
	private static List<string> HeaderPatterns = new List<string>
	{
		@"#(PLAYER) (.*)",
		@"#(GENRE) (.*)",
		@"#(TITLE) (.*)",
		@"#(ARTIST) (.*)",
		@"#(BPM) (.*)",
		@"#(BPM[0-9A-Z]{2}) (.*)",
		@"#(PLAYLEVEL) (.*)",
		@"#(RANK) (.*)",
		@"#(WAV[0-9A-Z]{2}) (.*)" // #WAV00~ZZに対応
	};

	// 音源再生オフセット
	public float audioOffset = 0f;

	// BMS上のレーン番号とゲーム内のレーン番号の対応
	private static Dictionary<char, int> LanePairs = new Dictionary<char, int>
	{
		{ '1', 0 },
		{ '2', 1 },
		{ '3', 2 },
		{ '4', 3 },
		{ '5', 4 }
	};

	// ヘッダー名をキー、データを値とする辞書
	public Dictionary<string, string> headerData = new Dictionary<string, string>();
	// ノーツ情報
	public List<NoteProperty> noteProperties = new List<NoteProperty>();
	// テンポ変化情報
	public List<TempoChange> tempoChanges = new List<TempoChange>();

	// 各小節の長さ(beat単位、4で4分の4拍子)
	private float[] measureLengths = Enumerable.Repeat(4f, 1000).ToArray();
	// 各レーンで最後にロングノーツがONになったbeat
	// （OFFの時は負の値にしておく）
	private float[] longNoteBeginBuffers = new float[] {-1, -1, -1, -1, -1 };

	// (コンストラクタ) BMSファイルを読み込む
	public BmsLoader(string filePath)
	{
		// BMSファイルを読み込み、各行を配列に保持
		var lines = File.ReadAllLines(filePath, Encoding.UTF8);

		// ヘッダー読み込み
		foreach (var line in lines)
		{
			LoadHeaderLine(line);
		}

		// 基本BPMをbeat0の時のBPMとして設定
		tempoChanges.Add(
			new TempoChange(0, Convert.ToSingle(headerData["BPM"]))
		);

		// メインデータ読み込み
		foreach (var line in lines)
		{
			LoadMainDataLine(line);
		}

		// テンポ変化データを時系列順に並び替え
		tempoChanges = tempoChanges.OrderBy(x => x.beat).ToList();

		// 各ノーツに対して、secの設定を行う
		foreach (var noteProperty in noteProperties)
		{
			noteProperty.secBegin = Beatmap.ToSec(noteProperty.beatBegin, tempoChanges);
			noteProperty.secEnd = Beatmap.ToSec(noteProperty.beatEnd, tempoChanges);
		}
	}

	// ヘッダー行のみ読み込む
	private void LoadHeaderLine(string line)
	{
		// 各ヘッダー名に対して実行
		foreach (var headerPattern in HeaderPatterns)
		{
			Match match = Regex.Match(line, headerPattern);
			// ヘッダー行のパターンに一致すればデータを取得
			if (match.Success)
			{
				// ヘッダー名
				var headerName = match.Groups[1].Value;
				// データ本体
				var data = match.Groups[2].Value;
				headerData[headerName] = data;
				return;
			}
		}
	}

	// メインデータ行のみ読み込む
	private void LoadMainDataLine(string line)
	{
		var match = Regex.Match(line, MainDataPattern);
		// match.Groups[1～3]に()で囲った部分(nnn, xx, ....)が代入されている
		// #nnnxx:....というフォーマットに一致していれば，データを処理
		if (match.Success)
		{
			// 小節番号
			int measureNum = Convert.ToInt32(match.Groups[1].Value);
			// チャンネル番号
			string channel = match.Groups[2].Value;
			// データ本体
			string body = match.Groups[3].Value;
			// データ種別
			DataType dataType = GetDataType(channel);

			// 対応可能なデータでない場合は処理を終了
			if (dataType == DataType.Unsupported)
			{
				return;
			}
			// 拍子変化
			if (dataType == DataType.MeasureChange)
			{
				measureLengths[measureNum] = Convert.ToSingle(body) * 4f;
			}
			// ノーツ・BPM変化
			else if (dataType == DataType.SingleNote ||
				dataType == DataType.LongNote ||
				dataType == DataType.DirectTempoChange ||
				dataType == DataType.IndexedTempoChange ||
				dataType == DataType.Bgm)
			{
				// 小節の開始beat (measureLengthsの先頭からmeasureNum個分の合計)
				float measureStartBeat = measureLengths.Take(measureNum).Sum();
				// オブジェクトの個数(分割数)
				int objCount = body.Length / 2;
				// データ本体を2桁ごとに区切って処理
				for (int i = 0; i < objCount; i++)
				{
					// オブジェクト番号
					string objNum = body.Substring(i * 2, 2);
					// 休符の場合は処理をスキップ
					if (objNum == "00")
					{
						continue;
					}

					// 4分の4拍子の場合、小節の開始beatに
					// (現在小節の長さ(beat) / objCount)を加えることで
					// 現在見ているオブジェクトのbeatが出る。
					float beat = measureStartBeat +
						(i * measureLengths[measureNum] / objCount);

					// ノーツの場合
					if (dataType == DataType.SingleNote ||
						dataType == DataType.LongNote)
					{
						// レーン番号(チャンネル番号の一の位で決まる)
						int lane = LanePairs[channel[1]];
						// チャンネル番号の十の位でノーツの種類を判定
						switch (dataType)
						{
							case DataType.SingleNote: // シングルノーツ
								// シングルノーツとしてnotePropertiesに追加
								noteProperties.Add(
									new NoteProperty(beat, beat, lane, NoteType.Single)
								);
								break;
							case DataType.LongNote: // ロングノーツ
								// このレーンのロングノーツがOFFの時
								if (longNoteBeginBuffers[lane] < 0)
								{
									// ロングノーツがONになったことにし、
									// ロングノーツの始点のbeatを保持
									longNoteBeginBuffers[lane] = beat;
								}
								// このレーンのロングノーツがONの時
								else
								{
									// 始点のbeat情報はバッファから読み込み、
									// ロングノーツをnotePropertiesに追加
									noteProperties.Add(new NoteProperty(
										longNoteBeginBuffers[lane], beat, lane, NoteType.Long
									));
									// バッファを適当な負の値に設定
									// （ロングノーツがOFFになったことを示す）
									longNoteBeginBuffers[lane] = -1;
								}
								break;
						}
					}
					// 直接指定タイプのテンポ変化の場合
					else if (dataType == DataType.DirectTempoChange)
					{
						// 16進数のBPMを10進数に変換
						float tempo = Convert.ToInt32(objNum, 16);
						// テンポ変化情報をtempoChangesに追加
						tempoChanges.Add(new TempoChange(beat, tempo));
					}
					// インデックス指定タイプのテンポ変化の場合
					else if (dataType == DataType.IndexedTempoChange)
					{
						// headerDataにある"BPMnn"というキーを持つ値を実数に変換
						float tempo
							= Convert.ToSingle(headerData["BPM" + objNum]);
						// テンポ変化情報をtempoChangesに追加
						tempoChanges.Add(new TempoChange(beat, tempo));
					}
					// BGMの場合
					else if (dataType == DataType.Bgm)
					{
						// 音源再生オフセット(秒単位) を設定
						audioOffset = Beatmap.ToSec(beat, tempoChanges);
					}
				}
			}
		}
	}

	// チャンネル番号からデータの種類を求める
	private DataType GetDataType(string channel)
	{
		// チャンネルの十の位が1のとき
		if (channel[0] == '1')
		{
			// シングルノーツ
			return DataType.SingleNote;
		}
		// チャンネルが02のとき
		else if (channel == "02")
		{
			// 拍子変化
			return DataType.MeasureChange;
		}
		// チャンネルが01のとき
		else if (channel == "01")
		{
			// BGM
			return DataType.Bgm;
		}
		// チャンネルが03のとき
		else if (channel == "03")
		{
			// BPM直接指定型テンポ変化
			return DataType.DirectTempoChange;
		}
		// チャンネルが08のとき
		else if (channel == "08")
		{
			// BPMインデックス指定型BPM変化
			return DataType.IndexedTempoChange;
		}
		// チャンネルの十の位が5のとき
		else if (channel[0] == '5')
		{
			// ロングノーツ
			return DataType.LongNote;
		}
		// それ以外のとき
		else
		{
			// 未対応とする
			return DataType.Unsupported;
		}
	}
}

public enum DataType
{
	Unsupported, // 未対応の種別
	SingleNote, // シングルノーツ
	LongNote, // ロングノーツ
	DirectTempoChange, // BPM直接指定型テンポ変化
	IndexedTempoChange, // BPMインデックス指定型BPM変化
	MeasureChange, // 拍子変化
	Bgm // BGM
}