using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [Header("Ready")]
    [SerializeField] private GameObject readyPanel;
    [SerializeField] private TextMeshProUGUI readyHintText;

    [Header("Playing")]
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Game Over")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI bestScoreText;
    [SerializeField] private Image medalImage;
    [SerializeField] private Sprite medalBronze;
    [SerializeField] private Sprite medalSilver;
    [SerializeField] private Sprite medalGold;
    [SerializeField] private Sprite medalPlatinum;
    [SerializeField] private GameObject restartButton;

    private BirdSelect birdSelect;
    private TextMeshProUGUI cheerText;
    private float cheerTime;
    private Color cheerColor = Color.white;
    private const float CheerDuration = 1.15f;

    private void Awake()
    {
        HidePanelBackground(readyPanel);
        HidePanelBackground(gameOverPanel);
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    private void Start()
    {
        LayoutHud();
        CreateBirdSelect();
        Button button = FindRestartButton();
        if (button != null)
        {
            button.onClick.AddListener(() =>
            {
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.Restart();
                }
            });
        }
    }

    // Put score, hint and restart in the right places on screen.
    private void LayoutHud()
    {
        PlaceText(scoreText, new Vector2(0.5f, 1f), new Vector2(0f, -90f), new Vector2(640f, 160f), 92f);
        PlaceText(readyHintText, new Vector2(0.5f, 0.5f), new Vector2(0f, 90f), new Vector2(900f, 220f), 48f);
        PlaceText(finalScoreText, new Vector2(0.5f, 0.5f), new Vector2(0f, 130f), new Vector2(700f, 120f), 72f);
        PlaceText(bestScoreText, new Vector2(0.5f, 0.5f), new Vector2(0f, 40f), new Vector2(700f, 80f), 42f);
        PlaceRect(restartButton, new Vector2(0.5f, 0.5f), new Vector2(0f, -160f), new Vector2(320f, 80f));
        PlaceRect(medalImage != null ? medalImage.gameObject : null, new Vector2(0.5f, 0.5f), new Vector2(0f, -40f), new Vector2(90f, 90f));

        StyleButtonLabel();
        EnsureCheerText();
    }

    private void Update()
    {
        AnimateCheer();
    }

    private void StyleButtonLabel()
    {
        if (restartButton == null)
        {
            return;
        }

        TextMeshProUGUI label = restartButton.GetComponent<TextMeshProUGUI>();
        if (label == null)
        {
            label = restartButton.GetComponentInChildren<TextMeshProUGUI>(true);
        }

        if (label == null)
        {
            return;
        }

        label.enabled = true;
        label.text = "RESTART";
        label.fontSize = 44f;
        label.alignment = TextAlignmentOptions.Center;
        label.enableWordWrapping = false;
        label.overflowMode = TextOverflowModes.Overflow;
        label.raycastTarget = true;
    }

    private static void PlaceText(TextMeshProUGUI text, Vector2 anchor, Vector2 anchoredPos, Vector2 size, float fontSize)
    {
        if (text == null)
        {
            return;
        }

        PlaceRect(text.gameObject, anchor, anchoredPos, size);
        text.fontSize = fontSize;
        text.alignment = TextAlignmentOptions.Center;
        text.enableWordWrapping = false;
        text.overflowMode = TextOverflowModes.Overflow;
        text.raycastTarget = false;
    }

    private static void PlaceRect(GameObject target, Vector2 anchor, Vector2 anchoredPos, Vector2 size)
    {
        if (target == null)
        {
            return;
        }

        RectTransform rect = target.GetComponent<RectTransform>();
        if (rect == null)
        {
            return;
        }

        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = anchoredPos;
        rect.sizeDelta = size;
    }

    private static void HidePanelBackground(GameObject panel)
    {
        if (panel == null)
        {
            return;
        }

        Image image = panel.GetComponent<Image>();
        if (image != null)
        {
            Color color = image.color;
            color.a = 0f;
            image.color = color;
            image.raycastTarget = false;
        }
    }

    private void CreateBirdSelect()
    {
        Canvas canvas = GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = GetComponentInParent<Canvas>();
        }

        BirdController bird = FindFirstObjectByType<BirdController>();
        TMP_FontAsset font = readyHintText != null ? readyHintText.font : null;
        if (canvas != null && bird != null)
        {
            birdSelect = BirdSelect.Create(canvas, bird, font);
        }
    }

    private Button FindRestartButton()
    {
        if (restartButton == null)
        {
            return null;
        }

        Button button = restartButton.GetComponent<Button>();
        if (button == null)
        {
            button = restartButton.GetComponentInChildren<Button>(true);
        }

        if (button == null)
        {
            button = restartButton.AddComponent<Button>();
        }

        return button;
    }

    public void ShowReady(int bestScore)
    {
        if (readyPanel != null)
        {
            readyPanel.SetActive(true);
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        if (scoreText != null)
        {
            scoreText.gameObject.SetActive(false);
        }

        if (finalScoreText != null)
        {
            finalScoreText.gameObject.SetActive(false);
        }

        if (bestScoreText != null)
        {
            bestScoreText.gameObject.SetActive(false);
        }

        if (restartButton != null)
        {
            restartButton.SetActive(false);
        }

        if (medalImage != null)
        {
            medalImage.gameObject.SetActive(false);
        }

        if (readyHintText != null)
        {
            readyHintText.gameObject.SetActive(true);
            readyHintText.text = bestScore > 0
                ? $"BEST {bestScore}\nTAP / SPACE"
                : "TAP / SPACE";
        }

        if (birdSelect != null)
        {
            birdSelect.SetVisible(true);
        }

        HideCheer();
    }

    public void ShowPlaying(int score)
    {
        if (readyPanel != null)
        {
            readyPanel.SetActive(false);
        }

        if (readyHintText != null)
        {
            readyHintText.gameObject.SetActive(false);
        }

        if (birdSelect != null)
        {
            birdSelect.SetVisible(false);
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        if (finalScoreText != null)
        {
            finalScoreText.gameObject.SetActive(false);
        }

        if (bestScoreText != null)
        {
            bestScoreText.gameObject.SetActive(false);
        }

        if (restartButton != null)
        {
            restartButton.SetActive(false);
        }

        if (medalImage != null)
        {
            medalImage.gameObject.SetActive(false);
        }

        if (scoreText != null)
        {
            scoreText.gameObject.SetActive(true);
            UpdateScore(score);
        }

        HideCheer();
    }

    public void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }
    }

    public void ShowGameOver(int score, int best)
    {
        if (readyPanel != null)
        {
            readyPanel.SetActive(false);
        }

        if (readyHintText != null)
        {
            readyHintText.gameObject.SetActive(false);
        }

        if (birdSelect != null)
        {
            birdSelect.SetVisible(false);
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        if (scoreText != null)
        {
            scoreText.gameObject.SetActive(true);
            scoreText.text = score.ToString();
        }

        if (finalScoreText != null)
        {
            finalScoreText.gameObject.SetActive(true);
            finalScoreText.text = "GAME OVER";
        }

        if (bestScoreText != null)
        {
            bestScoreText.gameObject.SetActive(true);
            bestScoreText.text = $"SCORE  {score}    BEST  {best}";
        }

        if (restartButton != null)
        {
            restartButton.SetActive(true);
        }

        if (medalImage != null)
        {
            medalImage.gameObject.SetActive(true);
            medalImage.sprite = MedalFor(score);
            medalImage.enabled = medalImage.sprite != null;
        }

        HideCheer();
    }

    public void ShowCheerForScore(int score)
    {
        EnsureCheerText();
        if (cheerText == null)
        {
            return;
        }

        string title = CheerTitle(score);
        if (string.IsNullOrEmpty(title))
        {
            return;
        }

        cheerColor = CheerColor(score);
        cheerText.text = title;
        cheerText.color = cheerColor;
        cheerText.gameObject.SetActive(true);
        cheerTime = CheerDuration;
        cheerText.rectTransform.localScale = Vector3.one * 0.35f;
        cheerText.rectTransform.anchoredPosition = new Vector2(0f, 64f);
    }

    private void EnsureCheerText()
    {
        if (cheerText != null)
        {
            return;
        }

        Canvas canvas = GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = GetComponentInParent<Canvas>();
        }

        if (canvas == null)
        {
            return;
        }

        TMP_FontAsset font = scoreText != null
            ? scoreText.font
            : (readyHintText != null ? readyHintText.font : null);

        var go = new GameObject("CheerText");
        go.transform.SetParent(canvas.transform, false);
        RectTransform rect = go.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(0f, 64f);
        rect.sizeDelta = new Vector2(720f, 120f);

        cheerText = go.AddComponent<TextMeshProUGUI>();
        if (font != null)
        {
            cheerText.font = font;
        }

        cheerText.fontSize = 54f;
        cheerText.fontStyle = FontStyles.Bold;
        cheerText.alignment = TextAlignmentOptions.Center;
        cheerText.enableWordWrapping = false;
        cheerText.overflowMode = TextOverflowModes.Overflow;
        cheerText.raycastTarget = false;
        cheerText.outlineWidth = 0.22f;
        cheerText.outlineColor = new Color(0.2f, 0.08f, 0.02f, 0.9f);
        go.SetActive(false);
    }

    private void AnimateCheer()
    {
        if (cheerText == null || !cheerText.gameObject.activeSelf)
        {
            return;
        }

        cheerTime -= Time.deltaTime;
        if (cheerTime <= 0f)
        {
            HideCheer();
            return;
        }

        float u = 1f - Mathf.Clamp01(cheerTime / CheerDuration);
        float scale;
        if (u < 0.16f)
        {
            scale = Mathf.Lerp(0.35f, 1.22f, u / 0.16f);
        }
        else if (u < 0.3f)
        {
            scale = Mathf.Lerp(1.22f, 1f, (u - 0.16f) / 0.14f);
        }
        else
        {
            scale = 1f;
        }

        cheerText.rectTransform.localScale = Vector3.one * scale;

        float rise = u > 0.55f ? (u - 0.55f) * 90f : 0f;
        cheerText.rectTransform.anchoredPosition = new Vector2(0f, 64f + rise);

        Color color = cheerColor;
        color.a = u > 0.68f ? 1f - (u - 0.68f) / 0.32f : 1f;
        cheerText.color = color;
    }

    private void HideCheer()
    {
        cheerTime = 0f;
        if (cheerText != null)
        {
            cheerText.gameObject.SetActive(false);
        }
    }

    private static string CheerTitle(int score)
    {
        if (score >= 40)
        {
            return "MYTHIC";
        }

        if (score >= 30)
        {
            return "GODLIKE";
        }

        if (score >= 25)
        {
            return "UNSTOPPABLE";
        }

        if (score >= 20)
        {
            return "LEGENDARY";
        }

        if (score >= 15)
        {
            return "EPIC";
        }

        if (score >= 10)
        {
            return "WOW";
        }

        if (score >= 5)
        {
            return "NICE";
        }

        return null;
    }

    private static Color CheerColor(int score)
    {
        if (score >= 40)
        {
            return new Color(0.85f, 0.55f, 1f);
        }

        if (score >= 30)
        {
            return new Color(0.55f, 1f, 0.45f);
        }

        if (score >= 25)
        {
            return new Color(0.4f, 0.95f, 1f);
        }

        if (score >= 20)
        {
            return new Color(1f, 0.84f, 0.2f);
        }

        if (score >= 15)
        {
            return new Color(1f, 0.42f, 0.82f);
        }

        if (score >= 10)
        {
            return new Color(1f, 0.55f, 0.2f);
        }

        return new Color(1f, 0.92f, 0.35f);
    }

    private Sprite MedalFor(int score)
    {
        if (score >= 40)
        {
            return medalPlatinum;
        }

        if (score >= 30)
        {
            return medalGold;
        }

        if (score >= 20)
        {
            return medalSilver;
        }

        if (score >= 10)
        {
            return medalBronze;
        }

        return null;
    }
}
