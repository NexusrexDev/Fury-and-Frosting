using Godot;
using Godot.Collections;
using System;

public enum MusicTitles
{
	None = -1,
	NexJingle,
	WinJingle,
	MainMenu,
	StoryScene,
	Gameplay,
	BossFight
}

public partial class AudioManager : Node
{
	public static AudioManager Instance { get; private set; }

	private Dictionary<MusicTitles, StringName> musicDictionary = new Dictionary<MusicTitles, StringName>
	{
		{ MusicTitles.NexJingle, new StringName("res://Music/Jingles/NexJingle.ogg") },
		{ MusicTitles.WinJingle, new StringName("res://Music/Jingles/WinJingle.ogg") },
		{ MusicTitles.MainMenu, new StringName("res://Music/Tunes/MainMenu.ogg") },
		{ MusicTitles.StoryScene, new StringName("res://Music/Tunes/StoryScene.ogg") },
		{ MusicTitles.Gameplay, new StringName("res://Music/Tunes/ForestTheme.ogg") },
		{ MusicTitles.BossFight, new StringName("res://Music/Tunes/WitchFight.ogg") }
	};

	private int _sfxPlayerCount = 16;
	private string _sfxBus = "SFX", _musicBus = "Music";

	private AudioStreamPlayer _musicPlayer;

	private Array<AudioStreamPlayer> _availableSFXPlayers = new Array<AudioStreamPlayer>(), _queueSFXPlayers = new Array<AudioStreamPlayer>();

	private Tween _tween;

	private MusicTitles _currentlyPlaying = MusicTitles.None;

	public override void _Ready()
	{
		Instance = this;
		ProcessMode = ProcessModeEnum.Always;

		for (int i = 0; i < _sfxPlayerCount; i++)
		{
			AudioStreamPlayer player = new AudioStreamPlayer();
			player.Bus = _sfxBus;
			player.Finished += () => { _availableSFXPlayers.Add(player); };
			_availableSFXPlayers.Add(player);
			AddChild(player);
		}

		_musicPlayer = new AudioStreamPlayer();
		_musicPlayer.Bus = _musicBus;
		AddChild(_musicPlayer);
	}

	public void PlayMusic(MusicTitles musicTrack, float fadeDuration = 0)
	{
		if (_currentlyPlaying == musicTrack)
			return;

		_currentlyPlaying = musicTrack;
		_musicPlayer.Stream = GD.Load<AudioStream>(musicDictionary[musicTrack]);
		_musicPlayer.Play();
		if (fadeDuration > 0)
		{
			_musicPlayer.VolumeDb = -80;
			if (_tween != null)
				_tween.Kill();
			_tween = CreateTween();
			_tween.TweenProperty(_musicPlayer, "volume_db", 0, fadeDuration);
		}
	}

	public void StopMusic(float fadeDuration = 0)
	{
		if (fadeDuration > 0)
		{
			if (_tween != null)
				_tween.Kill();
			_tween = CreateTween();
			_tween.TweenProperty(_musicPlayer, "volume_db", -80, fadeDuration);
			_tween.Finished += () => 
			{ 
				_musicPlayer.Stop();
				_musicPlayer.VolumeDb = 0;
			};
		}
		else
			_musicPlayer.Stop();

		_currentlyPlaying = MusicTitles.None;
	}

	public void PlaySFX(AudioStream audioStream)
	{
		AudioStreamPlayer player = _availableSFXPlayers[0];
		player.Stream = audioStream;
		player.Play();
		_availableSFXPlayers.RemoveAt(0);
	}
}
