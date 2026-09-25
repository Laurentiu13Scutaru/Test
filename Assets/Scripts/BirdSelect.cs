using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Lets the player pick a bird color on the ready screen.
public class BirdSelect : MonoBehaviour
{
    private static readonly Color[] Palette =
    {
        Color.white,
        new Color(1f, 0.42f, 0.42f),
        new Color(1f, 0.68f, 0.22f),
        new Color(0.45f, 0.95f, 0.5f),
        new Color(0.4f, 0.78f, 1f),
        new Color(0.92f, 0.48f, 1f)
    };

    private BirdController bird;
    private GameObject root;
    private Image[] outlines;

    public static BirdSelect Create(Canvas canvas, BirdController birdController, TMP_FontAsset font)
    {
        var go = new GameObject("BirdSelect");
        go.transform.SetParent(canvas.transform, false);
        BirdSelect select = go.AddComponent<BirdSelect>();
        select.Build(canvas, birdController, font);
        return select;
    }

    private void Build(Canvas canvas, BirdController birdController, TMP_FontAsset font)
    {
        bird = birdController;
        root = gameObject;

        var rootRect = root.AddComponent<RectTransform>();
        rootRect.anchorMin = new Vector2(0.5f, 0f);
        rootRect.anchorMax = new Vector2(0.5f, 0f);
        rootRect.pivot = new Vector2(0.5f, 0f);
        rootRect.anchoredPosition = new Vector2(0f, 48f);
        rootRect.sizeDelta = new Vector2(720f, 150f);

        CreateLabel(font);

        outlines = new Image[Palette.Length];
        float spacing = 92f;
        float startX = -((Palette.Length - 1) * spacing) * 0.5f;
        Color current = LoadTint();

        if (bird != null)
        {
            bird.SetTint(current);
        }

        for (int i = 0; i < Palette.Length; i++)
        {
            int index = i;
            Color preview = i == 0 ? new Color(1f, 0.88f, 0.28f) : Palette[i];
            outlines[i] = CreateSwatch(startX + i * spacing, preview, () => Select(index));
        }

        RefreshOutlines(IndexOf(current));
    }

    private void CreateLabel(TMP_FontAsset font)
    {
        var go = new GameObject("Label");
        go.transform.SetParent(root.transform, false);

        var rect = go.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 1f);
        rect.anchorMax = new Vector2(0.5f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.anchoredPosition = new Vector2(0f, 8f);
        rect.sizeDelta = new Vector2(700f, 40f);

        var text = go.AddComponent<TextMeshProUGUI>();
        text.text = "CHOOSE BIRD";
        text.fontSize = 28f;
        text.alignment = TextAlignmentOptions.Center;
        text.enableWordWrapping = false;
        text.raycastTarget = false;
        if (font != null)
        {
            text.font = font;
        }
    }

    private Image CreateSwatch(float x, Color fill, UnityEngine.Events.UnityAction onClick)
    {
        var go = new GameObject("Color");
        go.transform.SetParent(root.transform, false);

        var rect = go.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0f);
        rect.anchorMax = new Vector2(0.5f, 0f);
        rect.pivot = new Vector2(0.5f, 0f);
        rect.anchoredPosition = new Vector2(x, 8f);
        rect.sizeDelta = new Vector2(78f, 78f);

        Image ring = go.AddComponent<Image>();
        ring.color = Color.white;
        ring.raycastTarget = true;

        var button = go.AddComponent<Button>();
        button.targetGraphic = ring;
        button.onClick.AddListener(onClick);

        var inner = new GameObject("Fill");
        inner.transform.SetParent(go.transform, false);
        var innerRect = inner.AddComponent<RectTransform>();
        innerRect.anchorMin = Vector2.zero;
        innerRect.anchorMax = Vector2.one;
        innerRect.offsetMin = new Vector2(8f, 8f);
        innerRect.offsetMax = new Vector2(-8f, -8f);
        inner.AddComponent<Image>().color = fill;
        inner.GetComponent<Image>().raycastTarget = false;

        return ring;
    }

    private void Select(int index)
    {
        Color color = Palette[index];
        SaveTint(color);
        if (bird != null)
        {
            bird.SetTint(color);
        }

        RefreshOutlines(index);
    }

    private void RefreshOutlines(int selected)
    {
        for (int i = 0; i < outlines.Length; i++)
        {
            outlines[i].color = i == selected
                ? new Color(1f, 1f, 1f, 1f)
                : new Color(1f, 1f, 1f, 0.25f);
        }
    }

    public void SetVisible(bool visible)
    {
        if (root != null)
        {
            root.SetActive(visible);
        }
    }

    private static int IndexOf(Color color)
    {
        for (int i = 0; i < Palette.Length; i++)
        {
            if (Almost(Palette[i], color))
            {
                return i;
            }
        }

        return 0;
    }

    private static bool Almost(Color a, Color b)
    {
        return Mathf.Abs(a.r - b.r) < 0.05f &&
               Mathf.Abs(a.g - b.g) < 0.05f &&
               Mathf.Abs(a.b - b.b) < 0.05f;
    }

    public static Color LoadTint()
    {
        return new Color(
            PlayerPrefs.GetFloat("FlappyBird.TintR", 1f),
            PlayerPrefs.GetFloat("FlappyBird.TintG", 1f),
            PlayerPrefs.GetFloat("FlappyBird.TintB", 1f),
            1f);
    }

    public static void SaveTint(Color color)
    {
        PlayerPrefs.SetFloat("FlappyBird.TintR", color.r);
        PlayerPrefs.SetFloat("FlappyBird.TintG", color.g);
        PlayerPrefs.SetFloat("FlappyBird.TintB", color.b);
        PlayerPrefs.Save();
    }
}
