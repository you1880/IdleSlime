using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager
{
    private const string BGM_PATH = "Sounds/BGM";
    private const string BUTTON_EFFECT_PATH = "Sounds/Button";
    private const string TOUCH_EFFECT_PATH = "Sounds/Touch";
    private AudioSource[] _audioSources = new AudioSource[(int)Define.SoundType.Count];
    private Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>();
    private float _bgmVolume = 1.0f;
    private float _effectVolume = 1.0f;

    public float BgmVolume
    {
        get { return _bgmVolume; }
    }

    public float EffectVolume
    {
        get { return _effectVolume; }
        set { _effectVolume = Mathf.Clamp01(value / 100.0f); }
    }

    public void Init()
    {
        GameObject root = GameObject.Find("@Sound");

        if (root == null)
        {
            root = new GameObject { name = "@Sound" };
            UnityEngine.Object.DontDestroyOnLoad(root);

            string[] soundNames = System.Enum.GetNames(typeof(Define.SoundType));

            for (int i = 0; i < soundNames.Length - 1; i++)
            {
                GameObject obj = new GameObject { name = soundNames[i] };
                _audioSources[i] = obj.AddComponent<AudioSource>();
                obj.transform.parent = root.transform;
            }

            _audioSources[(int)Define.SoundType.Bgm].loop = true;
        }
    }

    public void Clear()
    {
        foreach (AudioSource audioSource in _audioSources)
        {
            audioSource.clip = null;
            audioSource.Stop();
        }

        _audioClips.Clear();
    }

    public void SetBgmVolume(float volume)
    {
        AudioSource audioSource = _audioSources[(int)Define.SoundType.Bgm];
        if (audioSource == null)
        {
            return;
        }

        _bgmVolume = Mathf.Clamp01(volume / 100.0f);
        audioSource.volume = _bgmVolume;
    }

    public void Play(string path, Define.SoundType soundType = Define.SoundType.Effect, float pitch = 1.0f)
    {
        AudioClip audioClip = GetOrAddAudioClip(path, soundType);

        Play(audioClip, soundType, pitch);
    }

    public void Play(AudioClip audioClip, Define.SoundType soundType = Define.SoundType.Effect, float pitch = 1.0f)
    {
        if (audioClip == null)
        {
            return;
        }

        if (soundType == Define.SoundType.Bgm)
        {
            AudioSource audioSource = _audioSources[(int)Define.SoundType.Bgm];

            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }

            audioSource.pitch = pitch;
            audioSource.clip = audioClip;
            audioSource.Play();
        }
        else
        {
            AudioSource audioSource = _audioSources[(int)Define.SoundType.Effect];
            audioSource.pitch = pitch;
            audioSource.volume = _effectVolume;
            audioSource.PlayOneShot(audioClip);
        }
    }

    private AudioClip GetOrAddAudioClip(string path, Define.SoundType soundType = Define.SoundType.Effect)
    {
        if (path.Contains("Sounds/") == false)
        {
            path = $"Sounds/{path}";
        }

        AudioClip audioClip = null;

        if (soundType == Define.SoundType.Bgm)
        {
            audioClip = Managers.Resource.Load<AudioClip>(path);
        }
        else
        {
            if (!_audioClips.TryGetValue(path, out audioClip))
            {
                audioClip = Managers.Resource.Load<AudioClip>(path);
                _audioClips.Add(path, audioClip);
            }
        }

        if (audioClip == null)
        {
            Debug.Log($"AudioClip Missing {path}");
        }

        return audioClip;
    }

    public void PlayBGM()
    {
        Play(BGM_PATH, Define.SoundType.Bgm);
    }

    public void PlayEffectSound(Define.EffectSoundType effectSoundType)
    {
        string soundName = Enum.GetName(typeof(Define.EffectSoundType), effectSoundType);

        if (string.IsNullOrEmpty(soundName))
        {
            return;
        }

        string path = $"Sounds/{soundName}";
        Play(path, Define.SoundType.Effect);
    }
}
