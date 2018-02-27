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
}