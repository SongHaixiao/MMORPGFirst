using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoSingleton<SoundManager>
{
    public AudioMixer audioMixer;
    public AudioSource musicAudioSource;
    public AudioSource soundAudioSOurce;

    const string MusicPath = "Music/";
    const string SoundPath = "Sound/";

    private bool musicOn;
    public bool MusicOn
    {
        get { return musicOn; }
        set
        {
            musicOn = value;
            this.MusicMute(!musicOn);
        }
    }

    private bool soundOn;
    public bool SoundOn
    {
        get { return soundOn; }
        set
        {
            soundOn = value;
            this.SoundMute(!soundOn);
        }
    }

    private int musicVolume;
    public int MusicVolume
    {
        get { return this.musicVolume; }
        set
        {
            if (this.musicVolume != value)
            {
                this.musicVolume = value;
                if (this.musicOn) this.SetVolume("MusicVolume", this.musicVolume);
            }
        }
    }

    private int soundVolume;
    public int SoundVolume
    {
        get { return this.soundVolume; }
        set
        {
            if (this.soundVolume != value)
            {
                this.soundVolume = value;
                if (this.soundOn) this.SetVolume("SoundVolume", this.soundVolume);
            }
        }
    }

    void Start()
    {
        this.MusicVolume = Config.MusicVolume;
        this.SoundVolume = Config.SoundVolume;
        this.MusicOn = Config.MusicOn;
        this.SoundOn = Config.SoundOn;
    }

    public void MusicMute(bool mute)
    {
        this.SetVolume("MusicVolume", mute ? 0 : musicVolume);
    }

    public void SoundMute(bool mute)
    {
        this.SetVolume("SoundVolume", mute ? 0 : soundVolume);
    }

    private void SetVolume(string name, int value)
    {
        float volume = value;

        if(value == 0)
            volume = value - 80;
        else
           volume = value;

        this.audioMixer.SetFloat(name, volume);
    }

    public void PlayMusic(string name)
    {
        AudioClip clip = Resloader.Load<AudioClip>(MusicPath + name);
        if (clip == null)
        {
            Debug.LogWarningFormat("PlayMusic : {0} not existed.", name);
            return;
        }

        if (musicAudioSource.isPlaying)
        {
            musicAudioSource.Stop();
        }

        musicAudioSource.clip = clip;
        musicAudioSource.Play();
    }

    public void PlaySound(string name)
    {
        AudioClip clip = Resloader.Load<AudioClip>(SoundPath + name);
        if (clip == null)
        {
            Debug.LogWarningFormat("PlaySound : {0} not existed.", name);
            return;
        }
        soundAudioSOurce.PlayOneShot(clip);
    }

    protected void PlayClipOnAudioSource(AudioSource source, AudioClip clip, bool isLoop)
    {
        
    }
}