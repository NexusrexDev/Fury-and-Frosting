using Godot;
using System;

[Tool]
public partial class Background : Node2D
{
	[Export]
	private DayNightEffect _foregroundEffect, _backgroundEffect;

	[Export]
	private Sprite2D _sky;

	[Export]
	private GradientTexture1D _skyGradient;

	private float _value;

	[Export(PropertyHint.Range, "0, 1")]
	private float Value
	{
		get => _value;
		set
		{
			_value = Mathf.Clamp(value, 0, 1);
			if (_foregroundEffect != null)
				_foregroundEffect.Value = _value;
			if (_backgroundEffect != null)
				_backgroundEffect.Value = _value;
			if (_sky != null)
				_sky.Modulate = _skyGradient.Gradient.Sample(_value);
		}
	}
}
