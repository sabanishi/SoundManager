# SoundManager

## 概要
Unityでの音声操作が簡単に行えるようになります。

バージョン2020.3.24f1のみ動作確認が完了しています。

## 利用方法
1.Soundフォルダを自分のプロジェクトのAssetsフォルダ直下にインポートする。

<img width="194" alt="image1" src="https://user-images.githubusercontent.com/84651801/162545285-82afe71b-3346-4e87-9221-37f875388d70.png">

2.アクティブなシーン上に、Soundフォルダ内のSoundManagerプレハブを追加。

3.追加したSoundManagerを右クリックし、プレハブ > 展開 によりプレハとの紐付けを解除する。

<img width="612" alt="image2" src="https://user-images.githubusercontent.com/84651801/162545380-a0c62c9d-6d59-4e0f-86e5-d6ff6bbe7c40.png">

4.SEフォルダ、BGMフォルダ内に使用する音声をインポートする。

5.音声を鳴らしたい場所にSoundManager.PlaySE()またはSoundManager.PlayBGM()を記述する。

<br>

より詳しい説明はManual.pdfをご覧ください。

## ライセンス
MIT
