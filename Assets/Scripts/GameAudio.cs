using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GameAudio : MonoBehaviour
{
    [SerializeField] private AudioClip flapClip;
    [SerializeField] private AudioClip pointClip;
    [SerializeField] private AudioClip hitClip;
    [SerializeField] private AudioClip dieClip;

    private AudioSource source;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 0f;
    }

    public void PlayFlap() => Play(flapClip);
    public void PlayPoint() => Play(pointClip);
    public void PlayHit() => Play(hitClip);
    public void PlayDie() => Play(dieClip);

    private void Play(AudioClip clip)
    {
        if (clip != null)
        {
            source.PlayOneShot(clip);
        }
    }
}
