using System.Linq;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public Sound[] Sounds => sounds;

    [SerializeField]
    private Sound[] sounds;

    private void Awake()
    {
        Instance = this;

        for (var i = 0; i < sounds.Length; i++)
        {
            var go = new GameObject("Sound_" + i + "_" + sounds[i].name);
            go.transform.SetParent(this.transform);
            sounds[i].SetSource(go.AddComponent<AudioSource>());

            if (sounds[i].playOnAwake)
                sounds[i].Play();
        }
    }

    private void OnDestroy()
    {
        Destroy(this.gameObject);
        Instance = null;
    }

    public void PlaySound(string soundName) => sounds.First(sound => sound.name.Equals(soundName)).Play();
}

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;

    [Range(0f, 1.5f)]
    public float volume = 0.7f;

    [Range(0f, 1.5f)]
    public float pitch = 1f;

    [Range(0f, 0.5f)]
    public float randomVolume = 0.1f;

    [Range(0f, 0.5f)]
    public float randomPitch = 0.1f;

    public bool loop;
    public bool playOnAwake;

    private AudioSource audioSource;

    public void SetSource(AudioSource source)
    {
        audioSource = source;
        audioSource.clip = clip;
    }

    public void Play()
    {
        audioSource.volume = volume * (1f + Random.Range(-randomVolume / 2f, randomVolume / 2f));
        audioSource.pitch = pitch * (1f + Random.Range(-randomPitch / 2f, randomPitch / 2f));
        audioSource.loop = loop;
        audioSource.Play();
    }

    public void Pause()
    {
        if (!audioSource.isPlaying)
            return;

        audioSource.Pause();
    }

    public void Resume()
    {
        if (audioSource.isPlaying)
            return;

        audioSource.UnPause();
    }
}