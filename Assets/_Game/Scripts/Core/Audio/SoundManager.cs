using System.Linq;
using UnityEngine;

namespace Ibit.Core.Audio
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance;

        public Sound[] Sounds => sounds;

        [SerializeField]
        private Sound[] sounds;

        private Sound bgm;

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
            Instance = null;
        }

        public void PlaySound(string soundName, bool oneshot = false)
        {
            var sound = sounds.First(snd => snd.name.Equals(soundName));

            if (soundName.Contains("BGM"))
            {
                bgm = sound;
            }

            sound.Play(oneshot);
        }

        public void PlayAnotherBgm()
        {
            bgm?.Pause();

            var playables = sounds.Where(sound => sound.name.Contains("BGM"));
            var music = playables.ElementAt(Random.Range(0, playables.Count() - 1));
            bgm = music;
            bgm.Play();
        }
    }
}