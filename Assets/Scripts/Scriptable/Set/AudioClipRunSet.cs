using UnityEngine;

namespace OMWGame.Scriptable {
    [CreateAssetMenu(fileName = "AudioClip Set", menuName = "Set/AudioClip")]
    public class AudioClipRunSet : RuntimeSet<AudioClip> {
        public AudioClip GetRandom() {
            return Items[Random.Range(0, Items.Count)];
        }
    }
}