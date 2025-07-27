using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class AudioController : MonoBehaviour
{
    public static AudioController Instance;

    [SerializeField] private AudioSource AudioSource;
    [SerializeField] private List<AudioData> AudioDatas = new();

    private void Awake()
    {
        Instance = this;

        if (AudioSource == null)
            AudioSource = GetComponent<AudioSource>();

        AudioSource.playOnAwake = false;
    }

    public void PlayAudio(AudioType audioType)
    {
        if (AudioDatas.Exists(x => x.AudioName.Equals(audioType)))
        {
            AudioData data = AudioDatas.Find(x => x.AudioName.Equals(audioType));
            if (data != null)
            {
                AudioSource.clip = data.Clip;
                AudioSource.volume = data.Volume;
                if (AudioSource.clip != null)
                {
                    AudioSource.PlayOneShot(data.Clip);
                }
            }
        }
    }

    public void StopAudio()
    {
        if (AudioSource.isPlaying)
            AudioSource.Stop();
    }
}

[System.Serializable]
public class AudioData
{
    public AudioType AudioName;
    public AudioClip Clip;
    public float Volume;
}

[System.Serializable]
public enum AudioType
{
    Flip,
    PerfectMatch,
    WrongMatch,
    GameOver
}
