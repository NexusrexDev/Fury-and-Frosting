using Godot;
using Godot.Collections;
using System;

public partial class AudioManager : Node
{
	private int _sfxPlayerCount = 16;
	private string _sfxBus = "SFX", _musicBus = "Music";

	private AudioStreamPlayer _musicPlayer;

	private Array<AudioStreamPlayer> _availableSFXPlayers = new Array<AudioStreamPlayer>(), _queueSFXPlayers = new Array<AudioStreamPlayer>();

	public override void _Ready()
	{
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

	public void PlayMusic(string fileName)
	{
		_musicPlayer.Stream = GD.Load<AudioStream>(fileName);
		_musicPlayer.Play();
	}

	public void StopMusic()
	{
		_musicPlayer.Stop();
	}

	public void PlaySFX(string fileName)
	{
		AudioStreamPlayer player = _availableSFXPlayers[0];
		player.Stream = GD.Load<AudioStream>(fileName);
		player.Play();
		_availableSFXPlayers.RemoveAt(0);
	}
}
