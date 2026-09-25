using UnityEngine;

public class ScrollRepeater : MonoBehaviour
{
    [SerializeField] private Transform leftPiece;
    [SerializeField] private Transform rightPiece;
    [SerializeField] private float width = 10.5f;

    private bool scrolling = true;

    public void SetScrolling(bool enabled)
    {
        scrolling = enabled;
    }

    private void Update()
    {
        GameManager manager = GameManager.Instance;
        if (!scrolling || manager == null || leftPiece == null || rightPiece == null)
        {
            return;
        }

        if (manager.State == GameState.GameOver)
        {
            return;
        }

        float delta = manager.CurrentScrollSpeed * Time.deltaTime;
        leftPiece.Translate(Vector3.left * delta);
        rightPiece.Translate(Vector3.left * delta);

        if (leftPiece.localPosition.x <= -width)
        {
            leftPiece.localPosition = new Vector3(
                rightPiece.localPosition.x + width,
                leftPiece.localPosition.y,
                0f);
            Swap();
        }
    }

    private void Swap()
    {
        (leftPiece, rightPiece) = (rightPiece, leftPiece);
    }
}
