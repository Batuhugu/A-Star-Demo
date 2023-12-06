using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    private AudioSource _mySource;

    public static AudioManager Instance { get; private set; }

    [Header("Sound Effects")]
    [SerializeField] AudioClip buttonClick;
    [SerializeField] AudioClip snakeEat;

	[Header("Background Music"), Tooltip("Background music and its transition effects")]
    [SerializeField] private AudioMixerSnapshot onSnapshot;
    [SerializeField] private AudioMixerSnapshot offSnapshot;

    [SerializeField] private AudioSource musicSource;
    [SerializeField, Range(0, 2f)] private float pitchOnGame;
    [SerializeField, Range(0, 2f)] private float pitchOnGameOver;
    [SerializeField] private AudioLowPassFilter lowPassFilter;

    private bool _isMusicOn = true;

    private void Awake()
    {
        Instance = this;

        musicSource.pitch = pitchOnGameOver;
    }

    private void Start()
    {
        _mySource = GetComponent<AudioSource>();

        UpdateMusic();
    }

    public void ToggleMusic()
    {
        _isMusicOn = !_isMusicOn;
        UpdateMusic();
    }

    private void UpdateMusic()
    {
        var snapshot = _isMusicOn ? onSnapshot : offSnapshot;
        snapshot.TransitionTo(.2f);
    }

    public void PlaySFX(ClipType type, float volume = 1)
    {
		var clip = GetClip(type);
		_mySource.PlayOneShot(clip, volume);
    }

    private AudioClip GetClip(ClipType type)
    {
        return type switch
        {
            ClipType.Click => buttonClick,
            ClipType.Eat => snakeEat,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    public void UpdateMusicPitch(bool IsGameWorking, bool isMainMenu)
    {
        var target = IsGameWorking ? pitchOnGame : pitchOnGameOver;
        StartCoroutine(ChangePitch(musicSource, target, .3f));
        lowPassFilter.enabled = !(IsGameWorking || isMainMenu);
    }

    private IEnumerator ChangePitch(AudioSource source, float target, float time)
    {
        float passed = 0;
        var init = source.pitch;
        while (passed < time)
        {
            passed += Time.deltaTime;
            source.pitch = Mathf.Lerp(init, target, passed / time);
            yield return null;
        }
    }
}