using Godot;
using System;

public class PlayingGrid : Grid
{
	public PuzzleDefinition CurrentPuzzle { get; private set; }
	private ColorEnabler colorEnabler;
	private Label useColors;
	private GridSquare[,] currentSquareGrid;

	private int gridSize;

	private HintColumnRow[] hintColumns;
	private HintColumnRow[] hintRows;

	private CompletedPopup completedPopup;

	public override void _Ready()
	{
		base._Ready();
		completedPopup = GetNode<CompletedPopup>("../CompletedPopup");

		CurrentPuzzle = variableHolder.CurrentPuzzleDefinition;
		useColors = GetNode<Label>("../UseColors");
		colorEnabler = GetNode<ColorEnabler>("../Colors");

		ColorIsEnabled = CurrentPuzzle.IsColorGrid;
		colorEnabler.ToggleColor(ColorIsEnabled);
		useColors.Text = "Is color puzzle: " + ColorIsEnabled;

		gridSize = CurrentPuzzle.GridSize;

		Control verticalHolder = GetNode<Control>("Vertical/Container");
		Control horizontalHolder = GetNode<Control>("Horizontal/Container");

		hintColumns = new HintColumnRow[verticalHolder.GetChildCount()];
		hintRows = new HintColumnRow[horizontalHolder.GetChildCount()];

		for (int c = 0; c < verticalHolder.GetChildCount(); c++)
		{
			hintColumns[c] = verticalHolder.GetChild(c) as HintColumnRow;
			hintColumns[c].Visible = c < CurrentPuzzle.GridSize;
		}

		for (int h = 0; h < horizontalHolder.GetChildCount(); h++)
		{
			hintRows[h] = horizontalHolder.GetChild(h) as HintColumnRow;
			hintRows[h].Visible = h < CurrentPuzzle.GridSize;
		}

		LoadInPuzzle();
	}

	public override void ToggleColor(bool colorShouldBeActive)
	{
		// Player should at this point not be able to change between color and monochrome puzzle versions during play.
	}

	public override void SquareClicked(int x, int y)
	{
		if (CurrentPuzzle.GridDefinition[x, y] < 0 || currentSquareGrid[x, y].CrossIsShowing)
		{
			// Squares with -1 should never be changed and crosses block input.
			return;
		}

		if (ColorIsEnabled)
		{
			currentSquareGrid[x, y].SetColor(CurrentColorIndex);
		}
		else
		{
			currentSquareGrid[x, y].SetColor(1);
		}

		CheckPuzzleStatus();
	}

	public override void SquareClickedErase(int x, int y)
	{
		if (CurrentPuzzle.GridDefinition[x, y] < 0)
		{
			// Squares with -1 should never be changed.
			return;
		}

		if (ColorIsEnabled)
		{
			if (currentSquareGrid[x, y].CrossIsShowing || currentSquareGrid[x, y].CurrentColorIndex == 0)
			{
				currentSquareGrid[x, y].ToggleCross();
			}
			else
			{
				currentSquareGrid[x, y].SetColor(0);
			}
		}
		else if (currentSquareGrid[x, y].CurrentColorIndex == 1) // Black color for monochrome
		{
			currentSquareGrid[x, y].SetColor(0);
		}
		else
		{
			currentSquareGrid[x, y].ToggleCross();
		}

		CheckPuzzleStatus();
	}

	private void LoadInPuzzle()
	{
		for (int c = 0; c < CurrentPuzzle.VerticalHints.Length; c++)
		{
			ColorHint[] lineHints = CurrentPuzzle.VerticalHints[c];
			hintColumns[c].LoadInHints(lineHints);
		}

		for (int r = 0; r < CurrentPuzzle.HorizontalHints.Length; r++)
		{
			ColorHint[] lineHints = CurrentPuzzle.HorizontalHints[r];
			hintRows[r].LoadInHints(lineHints);
		}

		Control currentGrid;

		if (gridSize == 5)
		{
			currentGrid = GetNode<Control>("Grid/5x5");
			GetNode<Control>("Grid/10x10").Visible = false;
		}
		else
		{
			currentGrid = GetNode<Control>("Grid/10x10");
			GetNode<Control>("Grid/5x5").Visible = false;
		}

		currentSquareGrid = new GridSquare[gridSize, gridSize];
		for (int x = 0; x < gridSize; x++)
		{
			for (int y = 0; y < gridSize; y++)
			{
				string path = "X_" + x + "/Y_" + y;
				currentSquareGrid[x, y] = currentGrid.GetNode<GridSquare>(path);

				if (CurrentPuzzle.GridDefinition[x, y] == -1)
				{
					currentSquareGrid[x, y].ShowCross();
				}
				else
				{
					currentSquareGrid[x, y].ResetSquare();
				}
			}
		}
		currentGrid.Visible = true;
	}

	private bool PuzzleIsComplete()
	{
		for (int x = 0; x < gridSize; x++)
		{
			for (int y = 0; y < gridSize; y++)
			{
				if (CurrentPuzzle.GridDefinition[x, y] < 1)
				{
					continue;
				}

				if (currentSquareGrid[x, y].CurrentColorIndex != CurrentPuzzle.GridDefinition[x, y])
				{
					return false;
				}
			}
		}

		return true;
	}

	private void CheckPuzzleStatus()
	{
		if (!PuzzleIsComplete())
		{
			return;
		}

		completedPopup.Visible = true;
	}
	
		
	private void ResetPuzzle()
	{
		for (int x = 0; x < gridSize; x++)
		{
			for (int y = 0; y < gridSize; y++)
			{
				if (CurrentPuzzle.GridDefinition[x, y] == -1)
				{
					currentSquareGrid[x, y].ShowCross();
				}
				else
				{
					currentSquareGrid[x, y].ResetSquare();
				}
			}
		}
	}
}


