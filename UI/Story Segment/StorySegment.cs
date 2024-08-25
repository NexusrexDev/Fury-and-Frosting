using Godot;
using Godot.Collections;
using System;
using static System.Net.Mime.MediaTypeNames;

public partial class StorySegment : Node2D
{
	[Export]
	public PackedScene _transitionScene;

	[Export]
	public PackedScene _nextScene;

	[Export]
	private AnimatedSprite2D _bardSprite;

	[Export]
	private RichTextLabel _textLabel;

	[Export]
	private Timer _timer;

	[Export(PropertyHint.MultilineText)]
	private string _fullText;

	private Array<string> _textArray;

	private int _currentTextIndex = 0;

	public override void _Ready()
	{
		_textArray = new Array<string>(_fullText.Split("\n"));
		_timer.Timeout += Start;
		_timer.Start();
	}

	private void Start()
	{
		if (_currentTextIndex < _textArray.Count)
		{
			_textLabel.VisibleCharacters = 0;
			_textLabel.Text = _textArray[_currentTextIndex];
			ReadText();
			_currentTextIndex++;
		}
		else
		{
			AudioManager.Instance.StopMusic(0.75f);
			Transition transitionNode = _transitionScene.Instantiate<Transition>();
			transitionNode.NextScene = _nextScene;
			AddChild(transitionNode);
		}
	}

	private async void ReadText()
	{
		if (_textLabel.VisibleCharacters >= _textLabel.Text.Length)
			return;
		
		StartAnimation();
		while (_textLabel.VisibleRatio < 1)
		{
			_textLabel.VisibleCharacters += 1;
			await ToSignal(GetTree().CreateTimer(0.035f), SceneTreeTimer.SignalName.Timeout);
		}
		StopAnimation();

		_timer.WaitTime = 1f;
		_timer.Start();
	}

	private void StartAnimation()
	{
		_bardSprite.Play("talk");
	}

	private void StopAnimation()
	{
		_bardSprite.Stop();
		_bardSprite.Frame = 0;
	}
}
