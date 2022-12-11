using Godot;
using System;

public class HintNumber : ColorRect
{
	private Label numberText;

	public override void _Ready()
	{
		numberText = GetNode<Label>("Label");
		Visible = false;
	}

	public void SetHintNumber(int number, int color)
	{
		numberText.Text = number.ToString();
		numberText.Modulate = ColorEnabler.ColorIndexLookup(color);
		this.Visible = true;
	}
}
