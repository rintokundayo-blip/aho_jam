using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultManager : MonoBehaviour
{
    [Header("UI Display References")]
    [SerializeField] private Image resultImageUI;          // UI Image component to display the result image
    [SerializeField] private SpriteRenderer resultSpriteWorld; // Optional SpriteRenderer (if using 2D Sprite instead of UI)
    [SerializeField] private TMP_Text rankText;             // Text component to display the rank name (e.g. 蟲ｶ蝗ｽ邏・
    [SerializeField] private TMP_Text scoreText;            // Text component to display the total rub count

    [Header("Result Sprites (Assign in Inspector)")]
    [SerializeField] private Sprite islandSprite;   // 0 ~ 15: 蟲ｶ蝗ｽ邏・(Ship)
    [SerializeField] private Sprite pacificSprite;  // 16 ~ 25: 螟ｪ蟷ｳ豢狗ｴ・(Airplane)
    [SerializeField] private Sprite arcticSprite;   // 26 ~ 30: 蛹玲･ｵ邏・(Frozen sea)
    [SerializeField] private Sprite spaceSprite;    // 31 ~ 40: 螳・ｮ咏ｴ・(UFO)
    [SerializeField] private Sprite godSprite;      // 41 ~ : 逾樒ｴ・(God)

    void Start()
    {
        // Get total rub count saved during gameplay
        int rubCount = PlayerPrefs.GetInt("RubCount", 0);

        // Display score text if assigned
        if (scoreText != null)
        {
            scoreText.text = rubCount.ToString();
        }

        // Determine Rank and Image based on rub count
        string rankName = "";
        Sprite targetSprite = null;

        if (rubCount <= 15)
        {
            rankName = "蟲ｶ蝗ｽ邏・;
            targetSprite = islandSprite;
        }
        else if (rubCount <= 25)
        {
            rankName = "螟ｪ蟷ｳ豢狗ｴ・;
            targetSprite = pacificSprite;
        }
        else if (rubCount <= 30)
        {
            rankName = "蛹玲･ｵ邏・;
            targetSprite = arcticSprite;
        }
        else if (rubCount <= 40)
        {
            rankName = "螳・ｮ咏ｴ・;
            targetSprite = spaceSprite;
        }
        else
        {
            rankName = "逾樒ｴ・;
            targetSprite = godSprite;
        }

        // Update Rank Text
        if (rankText != null)
        {
            rankText.text = rankName;
        }

        // Apply Target Sprite to UI Image or SpriteRenderer
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
