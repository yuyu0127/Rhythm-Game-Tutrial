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
