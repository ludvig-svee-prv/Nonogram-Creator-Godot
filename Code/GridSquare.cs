using Godot;
using System;

public class GridSquare : ColorRect
{
	Grid connectedGrid;

	int x;
	int y;

	private Color standardColor = new Color(1,1,1);
	private TextureRect cross;

	public bool CrossIsShowing { 
		get {
			return cross.Visible;
		}
	}

	public bool ColorHasBeenChanged{
		get {
			return !this.Color.Equals(standardColor);
		}
	}

	public override void _Ready()
	{
		// All grids are always three steps up in hiearchy.
		connectedGrid = GetParent().GetParent().GetParent<Grid>();

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

	public void ToggleCross()
	{
		cross.Visible = !cross.Visible;
	}

	public void ResetColor()
	{
		SetColor(standardColor);
	}

	public void SetColor(Color c)
	{
		Color = c;
	}
}

