using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Texture2D handCursor;          // 手カーソル画像
    public GameObject explanationPanel;   // 説明パネル

    void Start()
    {
        // カーソルを手に変更
        Cursor.SetCursor(handCursor, Vector2.zero, CursorMode.Auto);

        // 説明パネルを表示
        explanationPanel.SetActive(true);

        // ゲーム停止
        Time.timeScale = 0f;
    }

    public void StartGame()
    {
        // 説明パネルを非表示
        explanationPanel.SetActive(false);

        // ゲーム再開
        Time.timeScale = 1f;
    }
}
