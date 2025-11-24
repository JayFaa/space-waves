using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioSource PlayClipAtPoint(AudioClip clip, Vector3 pos, float volume)
    {
        GameObject tempGO = new GameObject("TempAudio");
        tempGO.transform.position = pos;
        AudioSource source = tempGO.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume;
        source.Play();
        Destroy(tempGO, clip.length);
        return source;
    }

    void Awake()
    {
        if (FindAnyObjectByType<AudioManager>() != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }
}
