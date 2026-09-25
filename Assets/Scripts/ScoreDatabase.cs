using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public static class ScoreDatabase
{
    // Replace this with your real backend endpoint.
    // Example: https://your-api.example.com/api/scores
    private const string ApiUrl = "https://your-api.example.com/api/scores";

    private static ApiSender sender;

    public static void SaveScore(int score, string playerName = "Guest")
    {
        if (score <= 0)
        {
            return;
        }

        if (sender == null)
        {
            GameObject go = new GameObject("ScoreApiSender");
            Object.DontDestroyOnLoad(go);
            sender = go.AddComponent<ApiSender>();
        }

        sender.Enqueue(score, playerName);
    }

    private sealed class ApiSender : MonoBehaviour
    {
        private readonly Queue<ScoreRequest> queue = new Queue<ScoreRequest>();
        private bool isSending;

        public void Enqueue(int score, string playerName)
        {
            queue.Enqueue(new ScoreRequest
            {
                Score = score,
                PlayerName = playerName
            });

            if (!isSending)
            {
                StartCoroutine(SendNext());
            }
        }

        private IEnumerator SendNext()
        {
            isSending = true;

            while (queue.Count > 0)
            {
                ScoreRequest request = queue.Dequeue();
                yield return SendRequest(request);
            }

            isSending = false;
        }

        private IEnumerator SendRequest(ScoreRequest request)
        {
            WWWForm form = new WWWForm();
            form.AddField("playerName", request.PlayerName);
            form.AddField("score", request.Score.ToString());

            using UnityWebRequest webRequest = UnityWebRequest.Post(ApiUrl, form);
            webRequest.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogWarning($"Failed to save score via API: {webRequest.error}");
            }
            else
            {
                Debug.Log("Score saved successfully via API.");
            }
        }
    }

    private struct ScoreRequest
    {
        public int Score;
        public string PlayerName;
    }
}
