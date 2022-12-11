using Godot;
using System;

public class GridSquare : ColorRect
{
	Grid connectedGrid;

	int x;
	int y;

	private TextureRect cross;

	public bool CrossIsShowing
	{
		get
		{
			return cross.Visible;
		}
	}

	public int CurrentColorIndex { get; private set; } = 0;

	public override void _Ready()
	{
		// All grids are always three steps up in hiearchy.
		connectedGrid = GetNode<Grid>("/root/Base/Grid");

		cross = GetNode<TextureRect>("Cross");

		// Name is in format "X_NUMBER/Y_NUMBER"
		x = int.Parse(GetParent().Name.Split("_")[1]);
		y = int.Parse(Name.Split("_")[1]);
	}



	public void InputToSquare(InputEvent @event)
	{
		if (@event is InputEventMouse)
		{
			if (Input.IsActionJustPressed("AddClick"))
			{
				ClickedAdd();
			}
			else if (Input.IsActionJustPressed("EraseClick"))
			{
				ClickedRemove();
			}
		}
	}


	// Gets called when a square in a grid is left clicked.
	public void ClickedAdd()
	{
		connectedGrid.SquareClicked(x, y);
	}

	// Gets called when a square in a grid is right clicked.
	public void ClickedRemove()
	{
		connectedGrid.SquareClickedErase(x, y);
	}

	public void ShowCross()
	{
		cross.Visible = true;
	}

	public void HideCross()
	{
		cross.Visible = false;
	}

	public void ToggleCross()
	{
		cross.Visible = !cross.Visible;
	}

	public void ResetSquare()
	{
		HideCross();
		ResetColor();
	}

	public void ResetColor()
	{
		SetColor(0);
	}

	public void SetColor(int colorIndex)
	{
		CurrentColorIndex = colorIndex;
		Color = ColorEnabler.ColorIndexLookup(CurrentColorIndex);
	}
}

