using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultManager : MonoBehaviour
{
    [Header("UI Display References")]
    [SerializeField] private Image resultImageUI;          // UI Image
    [SerializeField] private SpriteRenderer resultSpriteWorld; // SpriteRenderer (if 2D Sprite)
    [SerializeField] private TMP_Text rankText;             // Rank Name Text
    [SerializeField] private TMP_Text scoreText;            // Rub Count Text

    [Header("Result Sprites (Assign in Inspector)")]
    [SerializeField] private Sprite islandSprite;   // 0 ~ 15: 島国級
    [SerializeField] private Sprite pacificSprite;  // 16 ~ 25: 太平洋級
    [SerializeField] private Sprite arcticSprite;   // 26 ~ 30: 北極級
    [SerializeField] private Sprite spaceSprite;    // 31 ~ 40: 宇宙級
    [SerializeField] private Sprite godSprite;      // 41 ~ : 神級

    void Start()
    {
        int rubCount = PlayerPrefs.GetInt("RubCount", 0);

        // 「SCORE: 25」の形式で表示
        if (scoreText != null)
        {
            scoreText.text = "SCORE: " + rubCount.ToString();
        }

        string rankName = "";
        Sprite targetSprite = null;

        if (rubCount <= 60)
        {
            rankName = "シマグニキュウ";
            targetSprite = islandSprite;
        }
        else if (rubCount <= 70)
        {
            rankName = "タイヘイヨウキュウ";
            targetSprite = pacificSprite;
        }
        else if (rubCount <= 90)
        {
            rankName = "ホッキョクキュウ";
            targetSprite = arcticSprite;
        }
        else if (rubCount <= 100)
        {
            rankName = "ウチュウキュウ";
            targetSprite = spaceSprite;
        }
        else
        {
            rankName = "ゴッドキュウ";
            targetSprite = godSprite;
        }

        // 「RANK: 島国級」の形式で表示
        if (rankText != null)
        {
            rankText.text = "RANK: " + rankName;
        }

        if (targetSprite != null)
        {
            if (resultImageUI != null)
            {
                resultImageUI.sprite = targetSprite;
            }
            if (resultSpriteWorld != null)
            {
                resultSpriteWorld.sprite = targetSprite;
            }
        }
    }
}