using Godot;
using System;

public class CompletedPopup : Panel
{
	private PlayingGrid playGrid;
	private Control viewGrid;

	public override void _Ready()
	{
		playGrid = GetNode<PlayingGrid>("../Grid");
		viewGrid = GetNode<Control>("ViewGrid");
		Visible = false;
	}

	public void ShowCompletedPopup()
	{
		viewGrid.Visible = false;
		this.Visible = true;
	}

	private void GoToMainMenu()
	{
		GetTree().ChangeScene("res://Scenes/StartScene.tscn");
	}

	private void ShowCompletedPuzzle()
	{
		PuzzleDefinition puzzle = playGrid.CurrentPuzzle;
		string gridPath =  puzzle.GridSize == 5 ? "5x5" : "10x10";

		Control currentGrid = viewGrid.GetNode<Control>("Grid/" + gridPath);
		for (int x = 0; x < puzzle.GridSize; x++)
		{
			for (int y = 0; y < puzzle.GridSize; y++)
			{
				int color = puzzle.GridDefinition[x,y];
				if (color < 0)
				{
					color = 0;
				}
				currentGrid.GetNode<ColorRect>("X_" + x + "/Y_" + y).Color = ColorEnabler.ColorIndexLookup(color);
			}
		}

		currentGrid.Visible = true;
		viewGrid.Visible = true;
	}
}
