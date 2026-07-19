using UnityEngine;
using System.Collections;

public class BGMManager : MonoBehaviour
{
    // BGMを再生するAudioSource
    [SerializeField] private AudioSource bgmSource;

    // フェードインに使う秒数
    [SerializeField] private float fadeInTime = 2f;

    // フェードアウトに使う秒数
    [SerializeField] private float fadeOutTime = 1f;

    // フェードアウトを開始するタイミング
    [SerializeField] private float fadeOutStartTime = 7f;

    private void Start()
    {
        // AudioSourceがセットされているか確認
        if (bgmSource != null)
        {
            // フェード付きループ再生を開始
            StartCoroutine(LoopWithFade());
        }
    }

    // フェード付きでループ再生する処理
    private IEnumerator LoopWithFade()
    {
        while (true)
        {
            // フェードイン
            yield return FadeInCoroutine(fadeInTime);

            // 曲の長さを取得
            float clipLength = bgmSource.clip.length;

            // フェードアウト開始地点まで待つ
            yield return new WaitForSeconds(fadeOutStartTime);

            // フェードアウト
            yield return FadeOutCoroutine(fadeOutTime);
        }
    }

    // フェードイン
    private IEnumerator FadeInCoroutine(float duration)
    {
        // フェードイン開始時は音量0
        bgmSource.volume = 0f;

        // 音量0のまま再生開始
        bgmSource.Play();
        Debug.Log("再生しました");

        // duration秒かけて音量を上げる
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            bgmSource.volume = Mathf.Lerp(0f, 0.125f, t / duration);
            yield return null;
        }

        // 最後に音量を指定する
        bgmSource.volume = 0.125f;
    }

    // フェードアウト
    private IEnumerator FadeOutCoroutine(float duration)
    {
        // フェードアウト開始時の音量を保存
        float startVolume = bgmSource.volume;

        // duration秒かけて音量を下げる
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            bgmSource.volume = Mathf.Lerp(startVolume, 0f, t / duration);
            yield return null;
        }

        // 最後に音量を0にする
        bgmSource.volume = 0f;

        // BGMを停止
        bgmSource.Stop();
    }
}
