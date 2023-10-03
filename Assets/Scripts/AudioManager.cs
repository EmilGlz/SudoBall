using UnityEngine;

public class AudioManager : MonoBehaviour
{

    #region Singleton
    private static AudioManager _instance;
    public static AudioManager Instance { get { return _instance; } }

    void Awake()
    {
        if (Instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }
    #endregion

    [HideInInspector] public bool soundOn = true;

    private const string savedSoundVolume = "sv";

    public Sound[] sounds;

    private void Start()
    {
        if (PlayerPrefs.HasKey(savedSoundVolume))
        {
            soundOn = PlayerPrefs.GetInt(savedSoundVolume) == 1;
        }
        else
        {
            PlayerPrefs.SetInt(savedSoundVolume, 1);
        }
    }

    public void Play(int soundIndex)
    {
        //Sound s = Array.Find(sounds, sound => sound.name == name); // find in sounds array a sound whose name is name
        Sound s = sounds[soundIndex];
        if (s == null)
        {
            return;
        }

        if (soundOn)
        {
            s.source.Play();
        }
    }

    public void Stop(int soundIndex)
    {
        //Sound s = Array.Find(sounds, sound => sound.name == name); // find in sounds array a sound whose name is name
        Sound s = sounds[soundIndex];
        if (s == null)
        {
            return;
        }
        s.source.Stop();
    }

    public void SetVolume(int soundIndex, float soundVolume)
    {
        //Sound s = Array.Find(sounds, sound => sound.name == name); // find in sounds array a sound whose name is name
        Sound s = sounds[soundIndex];
        if (s == null)
        {
            return;
        }
        s.source.volume = soundVolume;
    }

    public void TurnOnOffSound(bool soundOn)
    {
        this.soundOn = soundOn;
        PlayerPrefs.SetInt(savedSoundVolume, soundOn ? 1 : 0);
    }

    public void SetVolumePitch(int index, float volume, float pitch)
    {
        sounds[index].source.volume = volume;
        sounds[index].source.pitch = pitch;
    }
}
