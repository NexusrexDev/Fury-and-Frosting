using Godot;
using System;

[Tool]
public partial class DayNightEffect : CanvasModulate
{
	[Export]
	private GradientTexture1D _gradientTexture;

	private float _value;

	[Export(PropertyHint.Range, "0, 1")]
	public float Value
	{
		get => _value;
		set
		{
			_value = Mathf.Clamp(value, 0, 1);
			if (_gradientTexture != null)
				Color = _gradientTexture.Gradient.Sample(_value);
		}
	}

	public override void _Ready()
	{
		if (_gradientTexture != null)
			Color = _gradientTexture.Gradient.Sample(_value);
	}
}
