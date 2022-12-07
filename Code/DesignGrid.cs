using Godot;
using System;

public class DesignGrid : Grid
{
	public bool ColorIsEnabled { get; private set; }
	public override void _Ready()
	{

	}

	public override void SquareClicked(int x, int y)
	{

	}

	public override void SquareClickedErase(int x, int y)
	{

	}

	public void SelectSquareColor(Color c)
	{

	}

	public void ToggleColor(bool colorShouldBeActive)
	{
		if (ColorIsEnabled == colorShouldBeActive){
			return;
		}

		ColorIsEnabled = colorShouldBeActive;
	}
}
