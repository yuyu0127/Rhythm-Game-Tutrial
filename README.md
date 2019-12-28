# Rhythm-Game-Tutrial
このリポジトリには、書籍「Unityで作るリズムゲーム」（以下、書籍）第１章のサンプルプロジェクトを格納しています。
書籍のダウンロード版はこちら( https://ecml.booth.pm/items/1739359 )

# Dependency
Unity2019.2.17f1にて制作しました。

# Setup
緑のボタン「Clone or download」を押したら表示される
「Download zip」を押して、zipファイルをダウンロードした後適当な場所に解凍してください。

解凍したフォルダ名は「Rhythm-Game-Tutrial-master」(もしくはそれに類するもの)になるはずなので、
Unityで「Rhythm-Game-Tutrial-master」フォルダを開いてください。

# Usage
## スクリプトを閲覧する場合
/Assets/Scripts 以下に、書籍で使っているUnity C#スクリプトを配置しています。

## ゲームの動作を確認する場合
まず、Assets/Scenes/SelectScene (選曲画面)を開いてください。
Unityエディタの画面上部にある再生アイコンをクリックするとゲームが開始します。

選曲画面では、以下の操作方法が適用されます。
```
←→: 譜面の選択
↑↓: ハイスピード(演奏画面の譜面スクロール速度)の選択
Space: 譜面の決定(GameSceneに遷移)
```

GameScene(演奏画面)では、以下の操作方法が適用されます。
```
Space: 譜面の再生
Esc: 選曲画面に戻る(曲が終了したら押してください)
C: 最も左のレーンをタップ
V: 左から2番目のレーンをタップ
B: 左から3番目のレーンをタップ
N: 左から4番目のレーンをタップ
M: 左から5番目のレーンをタップ
```
# License
Copyright (C) 2019  yuyu0127

このリポジトリ及びreleaseにアップロードされている全てのファイル(以下、「コンテンツ」)を、著作権法で認められている権利者の許諾を得ずに、個人的な範囲を超える使用目的で複製すること及びネットワーク等を通じて「コンテンツ」を送信できる状態にすることを禁じます。
「コンテンツ」の利用は、必ずご自身の責任と判断によって行ってください。「コンテンツ」を使用した結果生じたいかなる直接的・間接的損害も、長崎大学マルチメディア研究会、yuyu0127を含むプログラム開発者および「コンテンツ」の制作に関わったすべての個人と団体は、いっさいその責任を負いかねます。

# Authors

/Beatmaps/blackSteps.ogg: ゲッポウ( https://mobile.twitter.com/guepmoo )

/Beatmaps/nightShadow.ogg: 鷹ピー( https://mobile.twitter.com/takapi130 )

/Beatmaps/nightShadow-hyper.bms, nightShadow-normal.bms, blackSteps-hyper.bms, blackSteps-normal.bms: Boltz( https://github.com/septem48 )

上記以外のスクリプトやアセット: yuyu0127( https://github.com/yuyu0127 )

# 書籍版の正誤表
## 修正点1：BmsLoader.csのコンストラクタ内に、テンポ変化データを時系列順に並び替える処理を追加 (P.51)
(誤) (コンストラクタのみ抜粋)
```BmsLoader.cs
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
    }
```
(正) (コンストラクタのみ抜粋)
```BmsLoader.cs
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
        
        // テンポ変化データを時系列順に並び替え                           この行を追加
        tempoChanges = tempoChanges.OrderBy(x => x.beat).ToList();  // この行を追加
    }
```

## 修正点2 : NoteProperty.csのコンストラクタの引数から`secBegin`, `secEnd`を削除し、代入箇所も削除 (P.68-69)
(誤)
```NoteProperty.cs
public class NoteProperty
{
    public float beatBegin; // 始点が判定ラインと重なるbeat
    public float beatEnd; // 終点が判定ラインと重なるbeat
    public float secBegin; // 始点が判定ラインと重なるsec
    public float secEnd; // 終点が判定ラインと重なるsec
    public int lane; // レーン
    public NoteType noteType; // ノーツ種別

    // コンストラクタ
    public NoteProperty(
        float beatBegin, float beatEnd,
        float secBegin, float secEnd,                    // この行を追加
        int lane, NoteType noteType
    )
    {
        this.beatBegin = beatBegin;
        this.beatEnd = beatEnd;
        this.secBegin = secBegin;                        // この行を削除
        this.secEnd = secEnd;                            // この行を削除
        this.lane = lane;
        this.noteType = noteType;
    }
}

public enum NoteType
{
    Single, // シングルノーツ
    Long // ロングノーツ
}
```

(正)
```NoteProperty.cs
public class NoteProperty
{
    public float beatBegin; // 始点が判定ラインと重なるbeat
    public float beatEnd; // 終点が判定ラインと重なるbeat
    public float secBegin; // 始点が判定ラインと重なるsec
    public float secEnd; // 終点が判定ラインと重なるsec
    public int lane; // レーン
    public NoteType noteType; // ノーツ種別

    // コンストラクタ
    public NoteProperty(
        float beatBegin, float beatEnd,
        int lane, NoteType noteType
    )
    {
        this.beatBegin = beatBegin;
        this.beatEnd = beatEnd;
        this.lane = lane;
        this.noteType = noteType;
    }
}

public enum NoteType
{
    Single, // シングルノーツ
    Long // ロングノーツ
}
```

## 修正点3 : BmsLoaderに関する説明文・修正用のソースコードを変更 (P.69-70)
(誤)

`NoteProperty`の変更に伴い、BMSを読み込む際の処理も変更する必要があります。
譜面を読み込んでノーツのデータを追加する際、ノーツが降ってくる秒数も計算しておき、
その値をコンストラクタに渡すようにします。
```BmsLoader.cs
                    // ノーツの場合
                    if (dataType == DataType.SingleNote ||
                        dataType == DataType.LongNote)
                    {
                        // レーン番号(チャンネル番号の一の位で決まる)
                        int lane = LanePairs[channel[1]];
                        // チャンネル番号の十の位でノーツの種類を判定
                              ……(ここまで同様につき省略)……
                        switch (dataType)
                        {
                            case DataType.SingleNote: // シングルノーツ
                                // beatを秒に変換
                                var sec = Beatmap.ToSec(beat, tempoChanges);
                                // シングルノーツとしてnotePropertiesに追加
                                noteProperties.Add(new NoteProperty(
                                    beat, beat, sec, sec, lane, NoteType.Single
                                ));
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
                                    // 始点のbeat情報はバッファから読み込み
                                    var beatBegin = longNoteBeginBuffers[lane];
                                    var secBegin = Beatmap.ToSec(beatBegin, tempoChanges);
                                    // 終点
                                    var beatEnd = beat;
                                    var secEnd = Beatmap.ToSec(beatEnd, tempoChanges);
                                    // ロングノーツをnotePropertiesに追加
                                    noteProperties.Add(new NoteProperty(
                                        beatBegin, beatEnd, secBegin, secEnd, lane, NoteType.Long
                                    ));
                                    // バッファを適当な負の値に設定
                               ……(以下同様につき省略)……
```

(正)

`NoteProperty`の変更に伴い、BMSを読み込む際の処理も変更する必要があります。
譜面を読み込んでノーツのデータを取得した後、ノーツが降ってくる秒数を計算し、値を設定します。
```BmsLoader.cs
    // 各レーンで最後にロングノーツがONになったbeat
    // （OFFの時は負の値にしておく）
    private float[] longNoteBeginBuffers = new float[] {-1, -1, -1, -1, -1 };
                              ……(ここまで同様につき省略)……
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
                               ……(以下同様につき省略)……
```

## 修正点4 : 判定を画面に表示する際の手順に関して、手順4と5の間に1つ手順を追加 (P.106)
(誤)
1. Hierarchyビューで右クリックし、**UI**→**Text**より2つのTextを作成します。
2. 作成したTextの名称を、1つは「**Text Label**」、もう1つは「**Text Value**」に変更します。
3. **Canvas**にアタッチされた`Canvas Scaler`コンポーネントの`UI Scale Mode`を`Scale With Screen Size`に設定します。
4. **Canvas**にアタッチされた`Canvas Scaler`コンポーネントの`Reference Resolution`を`X: 200, Y: 300`に設定します。
5. **Text Label**の`RectTransform`を以下のように設定します。
(以下省略)
 
(正)
1. Hierarchyビューで右クリックし、**UI**→**Text**より2つのTextを作成します。
2. 作成したTextの名称を、1つは「**Text Label**」、もう1つは「**Text Value**」に変更します。
3. **Canvas**にアタッチされた`Canvas Scaler`コンポーネントの`UI Scale Mode`を`Scale With Screen Size`に設定します。
4. **Canvas**にアタッチされた`Canvas Scaler`コンポーネントの`Reference Resolution`を`X: 200, Y: 300`に設定します。
5. **Canvas**にアタッチされた`Canvas Scaler`コンポーネントの`Screen Match Mode`を`Expand`に設定します。
6. **Text Label**の`RectTransform`を以下のように設定します。
(以下省略)

## 修正点5 : UIManagerを作成する際、**Player**にアタッチするような記載を削除 (P.108)
(誤)

しかし、これだけでは実際の値に対応した文字が表示されません。
そこで、スクリプトを用いてTextの内容を変更できるようにしましょう。
今回は、Textにスクリプトをアタッチするのではなく、`UIManager`というクラスで管理することにします。
新たに**UIMangaer.cs**を以下の内容で作成し、先程と同様にHierarchy上のGameObject「**Player**」にアタッチしてください。
  
(正)

しかし、これだけでは実際の値に対応した文字が表示されません。
そこで、スクリプトを用いてTextの内容を変更できるようにしましょう。
今回は、Textにスクリプトをアタッチするのではなく、`UIManager`というクラスで管理することにします。
新たに**UIMangaer.cs**を以下の内容で作成してください。

## 修正点6 : 楽曲選択画面を制作する際の手順に関して、手順7と8の間に1つ手順を追加 (P.111)
(誤)
1. 新たなシーンを作成し、名前を**SelectScene**とします。
2. **SelectScene**を開きます。
3. **Main Camera**の`Background`を白（`RGB=(255, 255, 255)`）にします。
4. Hierarchyビューで右クリックし、**UI**→**Text**より2つのTextを作成します。
5. 作成したTextの名称を、1つは「**Text Information**」、もう1つは「**Text Scroll Speed**」に変更します。
6. **Canvas**にアタッチされた`Canvas Scaler`コンポーネントの`UI Scale Mode`を`Scale With Screen Size`に設定します。
7. **Canvas**にアタッチされた`Canvas Scaler`コンポーネントの`Reference Resolution`を`X: 200, Y: 300`に設定します。
8. **Text Information**の`RectTransform`を以下のように設定します。
(以下省略)
 
(正)
1. 新たなシーンを作成し、名前を**SelectScene**とします。
2. **SelectScene**を開きます。
3. **Main Camera**の`Background`を白（`RGB=(255, 255, 255)`）にします。
4. Hierarchyビューで右クリックし、**UI**→**Text**より2つのTextを作成します。
5. 作成したTextの名称を、1つは「**Text Information**」、もう1つは「**Text Scroll Speed**」に変更します。
6. **Canvas**にアタッチされた`Canvas Scaler`コンポーネントの`UI Scale Mode`を`Scale With Screen Size`に設定します。
7. **Canvas**にアタッチされた`Canvas Scaler`コンポーネントの`Reference Resolution`を`X: 200, Y: 300`に設定します。
8. **Canvas**にアタッチされた`Canvas Scaler`コンポーネントの`Screen Match Mode`を`Expand`に設定します。
9. **Text Information**の`RectTransform`を以下のように設定します。
(以下省略)

## 修正点7 : SelectorControllerの修正にて、`using UnityEngine.SceneManagement;`の1行を追加 (P.117)
(誤)
```SelectorController.cs
                              ……(ここまで同様につき省略)……
	private void Update()
	{
		// 譜面ID変更
		if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			ChangeSelectedIndex(selectedIndex - 1);
		}
		if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			ChangeSelectedIndex(selectedIndex + 1);
		}

		// スクロール速度変更
		if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			ChangeScrollSpeed(scrollSpeed - 0.1f);
		}
		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			ChangeScrollSpeed(scrollSpeed + 0.1f);
		}

		// 決定処理
		if (Input.GetKeyDown(KeyCode.Space))
		{
			// スクロール速度を設定
			PlayerController.ScrollSpeed = scrollSpeed;
			// 譜面を設定
			PlayerController.beatmap = new Beatmap(beatmapPaths[selectedIndex]);
			// シーン切り替え
			SceneManager.LoadScene("GameScene");
		}
	}
                               ……(以下同様につき省略)……
```


(正)
```SelectorController.cs
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement; // シーン遷移に必要
using UnityEngine.UI;
                              ……(この間同様につき省略)……
	private void Update()
	{
		// 譜面ID変更
		if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			ChangeSelectedIndex(selectedIndex - 1);
		}
		if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			ChangeSelectedIndex(selectedIndex + 1);
		}

		// スクロール速度変更
		if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			ChangeScrollSpeed(scrollSpeed - 0.1f);
		}
		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			ChangeScrollSpeed(scrollSpeed + 0.1f);
		}

		// 決定処理
		if (Input.GetKeyDown(KeyCode.Space))
		{
			// スクロール速度を設定
			PlayerController.ScrollSpeed = scrollSpeed;
			// 譜面を設定
			PlayerController.beatmap = new Beatmap(beatmapPaths[selectedIndex]);
			// シーン切り替え
			SceneManager.LoadScene("GameScene");
		}
	}
                               ……(以下同様につき省略)……
```
