using Managment.Pooling;
using UnityEngine;

namespace Management.SoundScripts
{
    public class AudioPool : MonoPool<AudioPool, AudioSourcePoolable>
    {
        public new static AudioPool Instance => (AudioPool)MonoPool<AudioPool, AudioSourcePoolable>.Instance;
    }
}