using UnityEngine;

// Fireball shot by the dragon. Reused from a pool.
public class Fireball : MonoBehaviour
{
    private float speed;
    private float despawnX;

    public void Launch(Vector3 position, float moveSpeed, float leftLimit)
    {
        speed = moveSpeed;
        despawnX = leftLimit;
        transform.position = position;
        gameObject.SetActive(true);
    }

    private void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.State != GameState.Playing)
        {
            gameObject.SetActive(false);
            return;
        }

        transform.Translate(Vector3.left * speed * Time.deltaTime, Space.World);
        transform.Rotate(0f, 0f, -540f * Time.deltaTime);

        if (transform.position.x < despawnX)
        {
            gameObject.SetActive(false);
        }
    }
}
