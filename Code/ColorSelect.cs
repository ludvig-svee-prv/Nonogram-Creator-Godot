using Godot;
using System;

public class ColorSelect : HBoxContainer
{
	DesignGrid designGrid;
	OptionButton selectButton;
	ColorRect colorSquare;

	public bool IsSelected { get; private set; }

	public override void _Ready()
	{
		designGrid = GetNode<DesignGrid>("/root/Create/CreateGrid");
		selectButton = GetNode<OptionButton>("OptionButton");
		colorSquare = GetNode<ColorRect>("ColorRect");
	}

	private void OptionPressed(bool button_pressed)
	{
		if (IsSelected)
		{
			// Ignore activation if already pressed. One color must always be active while color is enabled.
			selectButton.SetPressedNoSignal(true);
			return;
		}

		IsSelected = true;
		designGrid.SelectSquareColor(colorSquare.Color);
	}

	public void DeselectColor()
	{
		IsSelected = false;
		selectButton.SetPressedNoSignal(false);
	}
}


