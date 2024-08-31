using Godot;
using Godot.Collections;
using System;
using static System.Net.Mime.MediaTypeNames;

public partial class StorySegment : Node2D
{
	[Export]
	public PackedScene _transitionScene;

	[Export]
	public SceneNames _nextScene;

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

	private Tween _tween;

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
			ReadText(_textArray[_currentTextIndex]);
		}
		else
		{
			AudioManager.Instance.StopMusic(0.75f);
			Transition transitionNode = _transitionScene.Instantiate<Transition>();
			transitionNode.NextScene = _nextScene;
			AddChild(transitionNode);
		}
	}

	private async void ReadText(string text)
	{
		_textLabel.VisibleRatio = 0;
		_textLabel.Text = text;
		int fullCount = _textLabel.GetTotalCharacterCount();

		StartAnimation();

		if (_tween != null)
			_tween.Kill();
		_tween = CreateTween();
		_tween.TweenProperty(_textLabel, "visible_ratio", 1, 0.035f * fullCount);

		await ToSignal(_tween, Tween.SignalName.Finished);

		StopAnimation();
		_currentTextIndex++;

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
