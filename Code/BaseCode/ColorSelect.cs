using Godot;
using System;

public class ColorSelect : ColorRect
{
	DesignGrid designGrid;
	ColorRect colorSquare;

	ColorEnabler colorEnabler;


	private Color enabledColor = new Color("#ffffff");
	private Color disabledColor = new Color("#d0434343");

	public int ColorIndex { get; private set; }


	public override void _Ready()
	{
		designGrid = GetNode<DesignGrid>("/root/Create/CreateGrid");
		colorSquare = GetNode<ColorRect>("ColorRect");

		colorEnabler = GetParent<ColorEnabler>();

		ColorIndex = int.Parse(Name);
		Color = ColorEnabler.ColorIndexLookup(ColorIndex);
	}

	private void UserInput(object @event)
	{
		if (Input.IsActionJustPressed("AddClick") && colorEnabler.ColorIsEnabled)
		{
			colorEnabler.ChangeColor(ColorIndex);
			colorSquare.Color = enabledColor;
		}
	}

	public void DeselectColor()
	{
		colorSquare.Color = disabledColor;
	}
}
