using Godot;
using System;

public class PlayingGrid : Grid
{
	public PuzzleDefinition CurrentPuzzle { get; private set; }
	private ColorEnabler colorEnabler;
	private Label useColors;

	private VerticalHintsColumn[] hintColumns;
	private HorizontalHintsRow[] hintRows;

	public override void _Ready()
	{
		base._Ready();
		CurrentPuzzle = variableHolder.CurrentPuzzleDefinition;
		useColors = GetNode<Label>("../UseColors");
		colorEnabler = GetNode<ColorEnabler>("../Colors");

		Control verticalHolder = GetNode<Control>("Vertical/Container");
		Control horizontalHolder = GetNode<Control>("Vertical/Container");

		for(int c = 0; c < verticalHolder.GetChildCount(); c++)
		{
			hintColumns[c] = verticalHolder.GetChild(c) as VerticalHintsColumn;
			hintColumns[c].Visible = c < CurrentPuzzle.GridSize;
		}

		for(int h = 0; h < horizontalHolder.GetChildCount(); h++)
		{
			hintRows[h] = horizontalHolder.GetChild(h) as HorizontalHintsRow;
			hintRows[h].Visible = h < CurrentPuzzle.GridSize;
		}

		LoadInPuzzle();
	}

	public override void SquareClicked(int x, int y)
	{
		if (CurrentPuzzle.GridDefinition[x, y] < 0)
		{
			// Squares with -1 should never be changed.
			return;
		}
	}

	public override void SquareClickedErase(int x, int y)
	{
		if (CurrentPuzzle.GridDefinition[x, y] < 0)
		{
			// Squares with -1 should never be changed.
			return;
		}
	}

	private void LoadInPuzzle()
	{
		//CurrentPuzzle
		for(int c = 0; c < CurrentPuzzle.VerticalHints.Length; c++)
		{
			ColorHint[] lineHints = CurrentPuzzle.VerticalHints[c];
			hintColumns
		}
	}
}
