using System.IO;
using UnityEditor;
using UnityEngine;

public static class GenerateFlappyBirdArt
{
    private const string ArtFolder = "Assets/Art";
    private const string AudioFolder = "Assets/Audio";
    private const string DataFolder = "Assets/Data";
    private const string PrefabFolder = "Assets/Prefabs";
    private const string ResourceArtFolder = "Assets/Resources/FlappyArt";
    private const string ResourceAudioFolder = "Assets/Resources/FlappyAudio";
    private const int Ppu = 32;

    [MenuItem("Flappy Bird/Generate Art And Audio")]
    public static void Generate()
    {
        Directory.CreateDirectory(ToAbs(ArtFolder));
        Directory.CreateDirectory(ToAbs(AudioFolder));
        Directory.CreateDirectory(ToAbs(DataFolder));
        Directory.CreateDirectory(ToAbs(PrefabFolder));
        Directory.CreateDirectory(ToAbs(ResourceArtFolder));
        Directory.CreateDirectory(ToAbs(ResourceAudioFolder));
        Directory.CreateDirectory(ToAbs("Assets/Scripts"));

        SaveSprite("background", DrawBackground(), Ppu, new Vector2(0.5f, 0.5f));
        SaveSprite("ground", DrawGround(), Ppu, new Vector2(0.5f, 1f));
        SaveSprite("pipe", DrawPipe(), Ppu, new Vector2(0.5f, 1f));
        SaveSprite("bird_0", DrawBird(0), Ppu, new Vector2(0.5f, 0.5f));
        SaveSprite("bird_1", DrawBird(1), Ppu, new Vector2(0.5f, 0.5f));
        SaveSprite("bird_2", DrawBird(2), Ppu, new Vector2(0.5f, 0.5f));
        SaveSprite("medal_bronze", DrawMedal(new Color32(205, 127, 50, 255)), Ppu, new Vector2(0.5f, 0.5f));
        SaveSprite("medal_silver", DrawMedal(new Color32(192, 192, 200, 255)), Ppu, new Vector2(0.5f, 0.5f));
        SaveSprite("medal_gold", DrawMedal(new Color32(255, 205, 40, 255)), Ppu, new Vector2(0.5f, 0.5f));
        SaveSprite("medal_platinum", DrawMedal(new Color32(220, 252, 255, 255)), Ppu, new Vector2(0.5f, 0.5f));
        SaveSprite("panel", DrawPanel(), Ppu, new Vector2(0.5f, 0.5f));

        SaveWav("flap", MakeTone(780, 0.07f, 0.28f, true));
        SaveWav("point", MakeTwoTone(980, 1320, 0.08f, 0.08f, 0.32f));
        SaveWav("hit", MakeNoise(0.12f, 0.45f, 140));
        SaveWav("die", MakeFallingTone(420, 110, 0.35f, 0.3f));

        GenerateResourceArt();
        CreateConfigAsset();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog(
            "Flappy Bird",
            "Art, audio and GameConfig were generated.",
            "OK");
    }

    /// <summary>
    /// Art and audio spawned at runtime must live under Resources so the game
    /// behaves the same in the editor and in a build on another machine.
    /// </summary>
    private static void GenerateResourceArt()
    {
        var altBody = new Color32(120, 200, 255, 255);
        var altBodyDark = new Color32(64, 150, 220, 255);

        SaveSpriteTo(ResourceArtFolder, "bird_0", DrawBird(0), Ppu, new Vector2(0.5f, 0.5f));
        SaveSpriteTo(ResourceArtFolder, "bird_1", DrawBird(1), Ppu, new Vector2(0.5f, 0.5f));
        SaveSpriteTo(ResourceArtFolder, "bird_2", DrawBird(2), Ppu, new Vector2(0.5f, 0.5f));
        SaveSpriteTo(ResourceArtFolder, "bird_alt_0", DrawBird(0, altBody, altBodyDark), Ppu, new Vector2(0.5f, 0.5f));
        SaveSpriteTo(ResourceArtFolder, "bird_alt_1", DrawBird(1, altBody, altBodyDark), Ppu, new Vector2(0.5f, 0.5f));
        SaveSpriteTo(ResourceArtFolder, "bird_alt_2", DrawBird(2, altBody, altBodyDark), Ppu, new Vector2(0.5f, 0.5f));
        SaveSpriteTo(ResourceArtFolder, "background", DrawBackground(), Ppu, new Vector2(0.5f, 0.5f));
        SaveSpriteTo(ResourceArtFolder, "ground", DrawGround(), Ppu, new Vector2(0.5f, 1f));
        SaveSpriteTo(ResourceArtFolder, "dragon", DrawDragon(), Ppu, new Vector2(0.5f, 0.5f));
        SaveSpriteTo(ResourceArtFolder, "fireball", DrawFireball(), Ppu, new Vector2(0.5f, 0.5f));
        SaveSpriteTo(ResourceArtFolder, "spark", DrawSpark(), Ppu, new Vector2(0.5f, 0.5f));
        SaveSpriteTo(ResourceArtFolder, "pipe", DrawPipe(), Ppu, new Vector2(0.5f, 1f));

        SaveWavTo(ResourceAudioFolder, "wow", MakeArpeggio(new[] { 660f, 880f, 1320f, 1760f }, 0.075f, 0.3f));
        SaveWavTo(ResourceAudioFolder, "firework", MakeFirework());
    }

    private static Pix DrawDragon()
    {
        const int w = 64;
        const int h = 48;
        var p = new Pix(w, h);

        var body = new Color32(168, 52, 74, 255);
        var bodyDark = new Color32(116, 30, 52, 255);
        var belly = new Color32(232, 170, 96, 255);
        var wing = new Color32(96, 24, 44, 255);
        var horn = new Color32(240, 226, 190, 255);
        var eye = new Color32(255, 232, 90, 255);
        var outline = new Color32(38, 12, 22, 255);

        p.FillCircle(34, 22, 13, body);
        p.Fill(30, 12, 24, 20, body);
        p.Fill(30, 12, 24, 5, bodyDark);
        p.FillCircle(30, 18, 8, belly);

        p.FillCircle(16, 26, 9, body);
        p.FillCircle(15, 24, 7, bodyDark);
        p.Fill(4, 22, 12, 7, body);
        p.Fill(4, 22, 12, 2, bodyDark);

        p.Fill(3, 23, 5, 4, horn);
        p.Fill(3, 25, 6, 1, outline);

        p.FillCircle(18, 30, 3, eye);
        p.FillCircle(18, 30, 1, outline);

        p.Fill(12, 32, 4, 3, horn);
        p.Fill(20, 34, 4, 3, horn);

        p.FillCircle(42, 34, 10, wing);
        p.FillCircle(46, 30, 9, wing);
        p.Rect(34, 24, 22, 18, outline);

        p.Fill(54, 18, 10, 5, body);
        p.Fill(60, 16, 4, 9, bodyDark);

        p.Rect(28, 10, 26, 24, outline);
        return p;
    }

    private static Pix DrawFireball()
    {
        var p = new Pix(20, 20);
        var core = new Color32(255, 246, 190, 255);
        var mid = new Color32(255, 168, 46, 255);
        var edge = new Color32(226, 74, 26, 255);

        p.FillCircle(10, 10, 9, edge);
        p.FillCircle(10, 10, 6, mid);
        p.FillCircle(10, 10, 3, core);
        return p;
    }

    private static Pix DrawSpark()
    {
        var p = new Pix(10, 10);
        var white = new Color32(255, 255, 255, 255);
        p.FillCircle(5, 5, 4, white);
        return p;
    }

    private static byte[] MakeArpeggio(float[] notes, float noteSeconds, float volume)
    {
        const int rate = 22050;
        int perNote = Mathf.RoundToInt(rate * noteSeconds);
        int tailSamples = Mathf.RoundToInt(rate * 0.18f);
        var samples = new float[perNote * notes.Length + tailSamples];

        for (int n = 0; n < notes.Length; n++)
        {
            for (int i = 0; i < perNote; i++)
            {
                int index = n * perNote + i;
                float t = i / (float)rate;
                float envelope = 1f - i / (float)perNote * 0.35f;
                samples[index] += Mathf.Sin(2f * Mathf.PI * notes[n] * t) * volume * envelope;
                samples[index] += Mathf.Sin(2f * Mathf.PI * notes[n] * 2f * t) * volume * 0.22f * envelope;
            }
        }

        int tailStart = perNote * notes.Length;
        float last = notes[notes.Length - 1];
        for (int i = 0; i < tailSamples; i++)
        {
            float t = i / (float)rate;
            float envelope = 1f - i / (float)tailSamples;
            samples[tailStart + i] = Mathf.Sin(2f * Mathf.PI * last * t) * volume * envelope;
        }

        return ToWav(samples, rate);
    }

    private static byte[] MakeFirework()
    {
        const int rate = 22050;
        int n = Mathf.RoundToInt(rate * 0.5f);
        var samples = new float[n];
        float prev = 0f;

        for (int i = 0; i < n; i++)
        {
            float t = i / (float)n;
            float raw = (Random.value * 2f - 1f);
            prev += (raw - prev) * 0.35f;

            float crackle = (Random.value < 0.02f) ? 0.9f : 0.25f;
            samples[i] = prev * crackle * 0.35f * (1f - t) * (1f - t);
        }

        return ToWav(samples, rate);
    }

    private static void CreateConfigAsset()
    {
        string path = DataFolder + "/GameConfig.asset";
        GameConfig config = AssetDatabase.LoadAssetAtPath<GameConfig>(path);
        if (config == null)
        {
            config = ScriptableObject.CreateInstance<GameConfig>();
            AssetDatabase.CreateAsset(config, path);
        }
    }

    private static Pix DrawBackground()
    {
        const int w = 576;
        const int h = 384;
        var p = new Pix(w, h);
        var skyTop = new Color32(78, 192, 202, 255);
        var skyBot = new Color32(112, 210, 214, 255);
        for (int y = 0; y < h; y++)
        {
            float t = y / (float)(h - 1);
            var row = Color32.Lerp(skyBot, skyTop, t);
            for (int x = 0; x < w; x++)
            {
                p.Set(x, y, row);
            }
        }

        var cloud = new Color32(250, 250, 250, 255);
        DrawCloud(p, 40, 250, 70, cloud);
        DrawCloud(p, 210, 290, 86, cloud);
        DrawCloud(p, 400, 240, 74, cloud);
        DrawCloud(p, 500, 310, 60, cloud);

        var city = new Color32(219, 205, 146, 255);
        var cityDark = new Color32(196, 180, 122, 255);
        int baseY = 118;
        for (int i = 0; i < 18; i++)
        {
            int bx = 8 + i * 32;
            int bw = 22 + (i * 7) % 10;
            int bh = 28 + (i * 13) % 42;
            p.Fill(bx, baseY, bw, bh, (i % 2 == 0) ? city : cityDark);
        }

        var bushDark = new Color32(86, 158, 44, 255);
        var bush = new Color32(116, 191, 46, 255);
        for (int x = 0; x < w; x++)
        {
            int hill = 78 + Mathf.RoundToInt(Mathf.Sin(x * 0.045f) * 16f + Mathf.Sin(x * 0.11f) * 8f);
            for (int y = 0; y < hill; y++)
            {
                p.Set(x, y + 92, y > hill - 8 ? bush : bushDark);
            }
        }

        return p;
    }

    private static void DrawCloud(Pix p, int cx, int cy, int r, Color32 c)
    {
        p.FillCircle(cx, cy, r / 2, c);
        p.FillCircle(cx + r / 2, cy - 4, r / 3, c);
        p.FillCircle(cx - r / 2, cy - 2, r / 3, c);
        p.FillCircle(cx + r / 5, cy + r / 4, r / 3, c);
    }

    private static Pix DrawGround()
    {
        const int w = 336;
        const int h = 112;
        var p = new Pix(w, h);
        var dirt = new Color32(222, 216, 149, 255);
        var dirtDark = new Color32(208, 179, 102, 255);
        var grass = new Color32(92, 176, 36, 255);
        var grassDark = new Color32(74, 148, 28, 255);
        var outline = new Color32(82, 54, 22, 255);
        p.Fill(0, 0, w, h, dirt);
        for (int y = 0; y < 72; y++)
        {
            for (int x = 0; x < w; x++)
            {
                if (((x / 8) + (y / 8)) % 2 == 0)
                {
                    p.Set(x, y, dirtDark);
                }
            }
        }

        p.Fill(0, 84, w, 28, grass);
        p.Fill(0, 84, w, 4, grassDark);
        p.Fill(0, 108, w, 4, outline);
        for (int x = 0; x < w; x += 16)
        {
            p.Fill(x, 88, 8, 16, grassDark);
        }

        return p;
    }

    private static Pix DrawPipe()
    {
        const int w = 52;
        const int h = 320;
        var p = new Pix(w, h);
        var body = new Color32(115, 191, 46, 255);
        var dark = new Color32(73, 128, 28, 255);
        var light = new Color32(168, 224, 86, 255);
        var outline = new Color32(47, 84, 16, 255);
        var lip = new Color32(98, 168, 36, 255);

        p.Fill(6, 0, 40, 292, body);
        p.Fill(6, 0, 8, 292, light);
        p.Fill(38, 0, 8, 292, dark);
        p.Rect(6, 0, 40, 292, outline);

        p.Fill(0, 292, 52, 28, lip);
        p.Fill(0, 292, 10, 28, light);
        p.Fill(42, 292, 10, 28, dark);
        p.Rect(0, 292, 52, 28, outline);
        p.Fill(2, 296, 48, 4, light);
        return p;
    }

    private static Pix DrawBird(int frame)
    {
        return DrawBird(frame, new Color32(247, 226, 107, 255), new Color32(224, 186, 48, 255));
    }

    private static Pix DrawBird(int frame, Color32 body, Color32 bodyDark)
    {
        const int w = 34;
        const int h = 24;
        var p = new Pix(w, h);
        var white = new Color32(255, 255, 255, 255);
        var beak = new Color32(240, 90, 40, 255);
        var beakDark = new Color32(196, 52, 24, 255);
        var outline = new Color32(40, 28, 16, 255);
        var wingUp = new Color32(247, 248, 230, 255);
        var wingMid = new Color32(244, 160, 64, 255);
        var wingDown = new Color32(232, 96, 40, 255);

        p.FillCircle(16, 11, 8, body);
        p.FillCircle(16, 10, 8, body);
        p.Fill(10, 6, 14, 10, body);
        p.Fill(12, 5, 10, 3, bodyDark);

        p.FillCircle(23, 14, 4, white);
        p.FillCircle(24, 14, 2, outline);
        p.FillCircle(25, 15, 1, white);

        p.Fill(24, 9, 9, 4, beak);
        p.Fill(24, 8, 9, 1, beakDark);
        p.Fill(24, 12, 9, 1, beakDark);
        p.Set(33, 10, beak);

        p.FillCircle(8, 10, 4, white);
        p.Rect(3, 4, 28, 17, outline);

        int wingY = frame == 0 ? 14 : frame == 1 ? 10 : 6;
        Color32 wing = frame == 0 ? wingUp : frame == 1 ? wingMid : wingDown;
        p.FillCircle(12, wingY, 5, wing);
        p.Rect(8, wingY - 4, 9, 9, outline);
        return p;
    }

    private static Pix DrawMedal(Color32 metal)
    {
        var p = new Pix(48, 48);
        var outline = new Color32(60, 40, 16, 255);
        var inner = Color32.Lerp(metal, new Color32(255, 255, 255, 255), 0.25f);
        p.FillCircle(24, 24, 20, outline);
        p.FillCircle(24, 24, 17, metal);
        p.FillCircle(20, 28, 6, inner);
        p.Fill(20, 18, 8, 12, outline);
        return p;
    }

    private static Pix DrawPanel()
    {
        var p = new Pix(220, 140);
        var paper = new Color32(222, 196, 126, 255);
        var dark = new Color32(86, 56, 22, 255);
        p.Fill(0, 0, 220, 140, dark);
        p.Fill(6, 6, 208, 128, paper);
        return p;
    }

    private static void SaveSprite(string name, Pix pix, int ppu, Vector2 pivot)
    {
        SaveSpriteTo(ArtFolder, name, pix, ppu, pivot);
    }

    private static void SaveSpriteTo(string folder, string name, Pix pix, int ppu, Vector2 pivot)
    {
        string assetPath = folder + "/" + name + ".png";
        File.WriteAllBytes(ToAbs(assetPath), pix.EncodePng());
        AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);

        var importer = (TextureImporter)AssetImporter.GetAtPath(assetPath);
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.spritePixelsPerUnit = ppu;
        importer.filterMode = FilterMode.Point;
        importer.mipmapEnabled = false;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.alphaIsTransparency = true;
        importer.npotScale = TextureImporterNPOTScale.None;

        var settings = new TextureImporterSettings();
        importer.ReadTextureSettings(settings);
        settings.spriteMeshType = SpriteMeshType.FullRect;
        settings.spriteAlignment = (int)SpriteAlignment.Custom;
        settings.spritePivot = pivot;
        importer.SetTextureSettings(settings);
        importer.SaveAndReimport();
    }

    private static void SaveWav(string name, byte[] wav)
    {
        SaveWavTo(AudioFolder, name, wav);
    }

    private static void SaveWavTo(string folder, string name, byte[] wav)
    {
        string assetPath = folder + "/" + name + ".wav";
        File.WriteAllBytes(ToAbs(assetPath), wav);
        AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
        var importer = (AudioImporter)AssetImporter.GetAtPath(assetPath);
        var settings = importer.defaultSampleSettings;
        settings.loadType = AudioClipLoadType.DecompressOnLoad;
        settings.compressionFormat = AudioCompressionFormat.PCM;
        importer.defaultSampleSettings = settings;
        importer.SaveAndReimport();
    }

    private static byte[] MakeTone(float freq, float seconds, float volume, bool drop)
    {
        const int rate = 22050;
        int n = Mathf.RoundToInt(rate * seconds);
        var samples = new float[n];
        for (int i = 0; i < n; i++)
        {
            float t = i / (float)rate;
            float env = drop ? 1f - i / (float)n : 1f;
            samples[i] = Mathf.Sin(2f * Mathf.PI * freq * t) * volume * env;
        }

        return ToWav(samples, rate);
    }

    private static byte[] MakeTwoTone(float a, float b, float secA, float secB, float volume)
    {
        const int rate = 22050;
        int nA = Mathf.RoundToInt(rate * secA);
        int nB = Mathf.RoundToInt(rate * secB);
        var samples = new float[nA + nB];
        for (int i = 0; i < nA; i++)
        {
            samples[i] = Mathf.Sin(2f * Mathf.PI * a * (i / (float)rate)) * volume;
        }

        for (int i = 0; i < nB; i++)
        {
            samples[nA + i] = Mathf.Sin(2f * Mathf.PI * b * (i / (float)rate)) * volume;
        }

        return ToWav(samples, rate);
    }

    private static byte[] MakeFallingTone(float start, float end, float seconds, float volume)
    {
        const int rate = 22050;
        int n = Mathf.RoundToInt(rate * seconds);
        var samples = new float[n];
        for (int i = 0; i < n; i++)
        {
            float t = i / (float)(n - 1);
            float freq = Mathf.Lerp(start, end, t);
            samples[i] = Mathf.Sin(2f * Mathf.PI * freq * (i / (float)rate)) * volume * (1f - t);
        }

        return ToWav(samples, rate);
    }

    private static byte[] MakeNoise(float seconds, float volume, float lowpass)
    {
        const int rate = 22050;
        int n = Mathf.RoundToInt(rate * seconds);
        var samples = new float[n];
        float prev = 0f;
        for (int i = 0; i < n; i++)
        {
            float raw = (Random.value * 2f - 1f) * volume;
            prev = prev + (raw - prev) * (lowpass / rate);
            samples[i] = prev * (1f - i / (float)n);
        }

        return ToWav(samples, rate);
    }

    private static byte[] ToWav(float[] samples, int rate)
    {
        int dataLen = samples.Length * 2;
        using var ms = new MemoryStream(44 + dataLen);
        using var bw = new BinaryWriter(ms);
        bw.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"));
        bw.Write(36 + dataLen);
        bw.Write(System.Text.Encoding.ASCII.GetBytes("WAVEfmt "));
        bw.Write(16);
        bw.Write((short)1);
        bw.Write((short)1);
        bw.Write(rate);
        bw.Write(rate * 2);
        bw.Write((short)2);
        bw.Write((short)16);
        bw.Write(System.Text.Encoding.ASCII.GetBytes("data"));
        bw.Write(dataLen);
        for (int i = 0; i < samples.Length; i++)
        {
            float v = Mathf.Clamp(samples[i], -1f, 1f);
            bw.Write((short)Mathf.RoundToInt(v * 32767f));
        }

        return ms.ToArray();
    }

    private static string ToAbs(string assetPath)
    {
        return Path.GetFullPath(Path.Combine(Application.dataPath, "..", assetPath));
    }

    private sealed class Pix
    {
        private readonly int w;
        private readonly int h;
        private readonly Color32[] px;

        public Pix(int width, int height)
        {
            w = width;
            h = height;
            px = new Color32[w * h];
        }

        public void Set(int x, int y, Color32 c)
        {
            if ((uint)x >= (uint)w || (uint)y >= (uint)h)
            {
                return;
            }

            px[y * w + x] = c;
        }

        public void Fill(int x, int y, int width, int height, Color32 c)
        {
            for (int yy = y; yy < y + height; yy++)
            {
                for (int xx = x; xx < x + width; xx++)
                {
                    Set(xx, yy, c);
                }
            }
        }

        public void Rect(int x, int y, int width, int height, Color32 c)
        {
            Fill(x, y, width, 1, c);
            Fill(x, y + height - 1, width, 1, c);
            Fill(x, y, 1, height, c);
            Fill(x + width - 1, y, 1, height, c);
        }

        public void FillCircle(int cx, int cy, int r, Color32 c)
        {
            int r2 = r * r;
            for (int y = -r; y <= r; y++)
            {
                for (int x = -r; x <= r; x++)
                {
                    if (x * x + y * y <= r2)
                    {
                        Set(cx + x, cy + y, c);
                    }
                }
            }
        }

        public byte[] EncodePng()
        {
            var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            tex.SetPixels32(px);
            tex.Apply();
            byte[] png = tex.EncodeToPNG();
            Object.DestroyImmediate(tex);
            return png;
        }
    }
}
