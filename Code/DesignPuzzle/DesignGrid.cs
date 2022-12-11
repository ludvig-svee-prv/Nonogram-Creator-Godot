using Godot;
using System;

public class DesignGrid : Grid
{
	private CheckBox is5x5Button;

	private Control gridHolder5x5;
	private Control gridHolder10x10;

	private GridSquare[,] grid5x5;
	private GridSquare[,] grid10x10;

	private GridSquare[,] currentSquareGrid;
	private SavePuzzlePopup savePuzzlePopup;

	private CheckBox useColorsCheckbox;

	int currentSize = 5;


	public override void _Ready()
	{
		base._Ready();

		is5x5Button = GetNode<CheckBox>("../FieldSizes/5x5");
		savePuzzlePopup = GetNode<SavePuzzlePopup>("../Popup");
		useColorsCheckbox = GetNode<CheckBox>("../UseColors");

		gridHolder5x5 = GetNode<Control>("5x5");
		gridHolder10x10 = GetNode<Control>("10x10");

		grid5x5 = LoadInGrid(gridHolder5x5, 5);
		grid10x10 = LoadInGrid(gridHolder10x10, 10);

		if (variableHolder.CurrentPuzzleDefinition != null)
		{
			LoadInPreviousPuzzle();
		}
		else
		{
			SetGridSize();
		}
	}

	private GridSquare[,] LoadInGrid(Node gridHolder, int size)
	{
		GridSquare[,] gs = new GridSquare[size, size];
		for (int x = 0; x < size; x++)
		{
			Node xNode = gridHolder.GetNode("X_" + x);
			for (int y = 0; y < size; y++)
			{
				GridSquare square = xNode.GetNode<GridSquare>("Y_" + y);
				gs[x, y] = square;
			}
		}

		return gs;
	}

	public void SetGridSize()
	{
		bool is5x5 = is5x5Button.Pressed;
		currentSize = is5x5 ? 5 : 10;
		gridHolder5x5.Visible = is5x5 ? true : false;
		gridHolder10x10.Visible = is5x5 ? false : true;

		currentSquareGrid = is5x5 ? grid5x5 : grid10x10;
		ResetGrid(currentSize);
	}

	private void LoadInPreviousPuzzle()
	{
		is5x5Button.Pressed = variableHolder.CurrentPuzzleDefinition.GridSize == 5;
		SetGridSize();

		useColorsCheckbox.Pressed = variableHolder.CurrentPuzzleDefinition.IsColorGrid;
		for (int x = 0; x < variableHolder.CurrentPuzzleDefinition.GridSize; x++)
		{
			for (int y = 0; y < variableHolder.CurrentPuzzleDefinition.GridSize; y++)
			{
				int colorIndex = variableHolder.CurrentPuzzleDefinition.GridDefinition[x, y];
				if (colorIndex == -1)
				{
					colorIndex = 0;
				}
				currentSquareGrid[x, y].SetColor(colorIndex);
			}
		}
	}

	private void ResetGrid(int size)
	{
		for (int x = 0; x < currentSize; x++)
		{
			for (int y = 0; y < currentSize; y++)
			{
				currentSquareGrid[x, y].ResetSquare();
			}
		}
	}

	public override void SquareClicked(int x, int y)
	{
		if (ColorIsEnabled)
		{
			currentSquareGrid[x, y].SetColor(CurrentColorIndex);
		}
		else
		{
			currentSquareGrid[x, y].SetColor(1);
		}
	}

	public override void SquareClickedErase(int x, int y)
	{
		currentSquareGrid[x, y].ResetColor();
	}

	public override void ToggleColor(bool colorShouldBeActive)
	{
		if (ColorIsEnabled == colorShouldBeActive)
		{
			return;
		}

		ColorIsEnabled = colorShouldBeActive;
	}


	private void GenerateAndSavePuzzleLayout()
	{
		int[,] currentGrid = new int[currentSize, currentSize];
		for (int x = 0; x < currentSize; x++)
		{
			for (int y = 0; y < currentSize; y++)
			{
				currentGrid[x, y] = currentSquareGrid[x, y].CurrentColorIndex;
			}
		}
		PuzzleDefinition puzzleDefinition = new PuzzleDefinition(ColorIsEnabled, currentSquareGrid, currentSize);

		int[,] completedGrid = PossibleSolutionsManager.GetOneSolutionGrid(currentGrid, currentSize, puzzleDefinition.VerticalHints, puzzleDefinition.HorizontalHints);
		puzzleDefinition.UpdateGridDefinition(completedGrid);

		savePuzzlePopup.ShowPopup(puzzleDefinition);
	}


	private void GoToMainMenu()
	{
		GetTree().ChangeScene("res://Scenes/StartScene.tscn");
	}
}





