using Godot;
using Godot.Collections;
using System;

public partial class StorySegment : Node2D
{
	[Export]
	private PackedScene _transitionScene;

	[Export]
	private SceneNames _nextScene;

	[Export]
	private AnimatedSprite2D _bardSprite;

	[Export]
	private RichTextLabel _textLabel;

	[Export]
	private Timer _timer;

	[Export(PropertyHint.MultilineText)]
	private string _fullText;

	private Array<string> _textArray;

	private int _currentLineIndex = 0;

	private Tween _tween;

	[Export]
	private Dictionary<int, NodePath> _fadeInQueue, _fadeOutQueue; //key is line index, value is scene index

	[Export]
	private Button _skipButton;

	[Export]
	private AudioStream _selectSFX;

	public override void _Ready()
	{
		_textArray = new Array<string>(_fullText.Split("\n"));

		_skipButton.Pressed += () => { End(true); };
		_skipButton.GrabFocus();

		_timer.Timeout += Start;
		_timer.Start();
	}

	public override void _ExitTree()
	{
		_timer.Timeout -= Start;
	}

	private void Start()
	{
		if (_currentLineIndex < _textArray.Count)
		{
			ReadText(_textArray[_currentLineIndex]);
		}
		else
		{
			End(false);
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

		if (_fadeInQueue.ContainsKey(_currentLineIndex))
		{
			_tween.SetParallel();
			Sprite2D scene = GetNode<Sprite2D>(_fadeInQueue[_currentLineIndex]);
			_tween.TweenProperty(scene, "modulate", new Color(scene.Modulate, 1), 0.6f);
		}
		if (_fadeOutQueue.ContainsKey(_currentLineIndex))
		{
			_tween.SetParallel();
			Sprite2D scene = GetNode<Sprite2D>(_fadeOutQueue[_currentLineIndex]);
			_tween.TweenProperty(scene, "modulate", new Color(scene.Modulate, 0), 0.6f);
		}

		await ToSignal(_tween, Tween.SignalName.Finished);

		StopAnimation();
		_currentLineIndex++;

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

	private void End(bool skipped)
	{
		_skipButton.ReleaseFocus();
		AudioManager.Instance.StopMusic(0.75f);
		
		if (skipped)
			AudioManager.Instance.PlaySFX(_selectSFX);
		
		Transition transitionNode = _transitionScene.Instantiate<Transition>();
		transitionNode.NextScene = _nextScene;
		AddChild(transitionNode);
	}
}
