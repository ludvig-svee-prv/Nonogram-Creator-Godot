using Godot;
using System;
using System.Text;
using System.Collections.Generic;

public class PossibleSolutionsManager
{
	private static Dictionary<int, string[]> AlreadyCalculatedVerticalLines = new Dictionary<int, string[]>();


	// Build all possible states recursively for current vertical line. Goes backwards from last square to highest for an easier flow to follow.
	// Uses a list of ints as the current state
	private static void BuildSolutionsRecursive(ColorHint[] hints, int hintIndex, int highestSquareCurrentlyPossibly, int filledSquaresLeft, int[] oldState, HashSet<string> completedStates)
	{
		if (filledSquaresLeft > oldState.Length - highestSquareCurrentlyPossibly)
		{
			return; // Not possible
		}


		ColorHint currentHint = hints[hintIndex];

		// Since this loop is reversed, it will try from highest possible amount of unused space, to directly after in color puzzles and
		// one cross in monochrome puzzles.
		for (int s = oldState.Length - 1; s >= highestSquareCurrentlyPossibly; s--)
		{
			int[] currentState = new int[oldState.Length];
			Array.Copy(oldState, currentState, currentState.Length);

			int startSqr = s;
			bool isPossible = true;

			// Ensure in monochrome puzzles, that each hint has atleast 1 space filled with a cross.
			if (currentHint.IsEmptyHint)
			{
				currentState[s] = 0;
				int updatedHintIndex = hintIndex + 1;
				int highSquareAfterEmpty = s + 1;

				BuildSolutionsRecursive(hints, updatedHintIndex, highSquareAfterEmpty, filledSquaresLeft, currentState, completedStates);
				continue;
			}

			for (int f = 0; f < hints[hintIndex].Amount; f++)
			{
				int index = f + startSqr;
				if (index >= currentState.Length)
				{
					isPossible = false;
					break; // hint needs more squares 
				}
				currentState[index] = hints[hintIndex].ColorIndex;
			}

			if (!isPossible)
			{
				continue;
			}

			int leftToFill = filledSquaresLeft - hints[hintIndex].Amount;
			int updateHintIndex = hintIndex + 1;
			int newHighestSquare = startSqr + hints[hintIndex].Amount;

			if (leftToFill > 0)
			{
				BuildSolutionsRecursive(hints, updateHintIndex, newHighestSquare, leftToFill, currentState, completedStates);
				continue;
			}

			if (newHighestSquare < currentState.Length - 1)
			{
				//Append empty squares at the end of the line.
				for (int fill = newHighestSquare; fill < currentState.Length; fill++)
				{
					currentState[fill] = 0;
				}
			}

			StringBuilder sb = new StringBuilder();
			for (int c = 0; c < currentState.Length; c++)
			{
				sb.Append(currentState[c] == -1 ? 0 : currentState[c]);
			}
			string solution = sb.ToString();
			if (!completedStates.Contains(solution))
			{
				completedStates.Add(solution);
			}
		}
	}

	private static HashSet<string> BuildPossibleSolutionsForVerticalLine(ColorHint[] hints, int gridSize)
	{
		int filledSquares = 0;

		foreach (ColorHint hint in hints)
		{
			if (!hint.IsEmptyHint) // To avoid empty monochrome hints.
			{
				filledSquares += hint.Amount;
			}
		}

		HashSet<string> completedLines = new HashSet<string>();

		// Skip calculation if it's one hint and it fills the line (either zero squares or all squares)
		if ((filledSquares == 0 || filledSquares == gridSize) && hints.Length == 1)
		{
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < gridSize; i++)
			{
				sb.Append(hints[0].ColorIndex);
			}
			completedLines.Add(sb.ToString());
			return completedLines;
		}


		int[] currentState = new int[gridSize];
		for (int cs = 0; cs < gridSize; cs++)
		{
			currentState[cs] = -1;
		}

		//Go up from lowest square. This makes it build up states in a pattern similar to how increases in different bases works for easier redability.
		for (int i = gridSize - 1; i >= 0; i--)
		{
			BuildSolutionsRecursive(hints, 0, i, filledSquares, currentState, completedLines);
		}

		return completedLines;
	}


	private static bool CurrentHorizontalIsPossible(string[] verticalLines, ColorHint[] lineHints, int horziontalIndex)
	{
		int currentSquare = 0;

		for (int h = 0; h < lineHints.Length; h++)
		{
			ColorHint hint = lineHints[h];
			if (currentSquare >= verticalLines.Length){
				return false;
			}

			if (hint.IsEmptyHint)
			{
				if (verticalLines[currentSquare][horziontalIndex] != '0')
				{
					return false;
				}
			}
			else
			{
				int squaresLeftToFillFromHint = hint.Amount;
				while (squaresLeftToFillFromHint > 0)
				{
					if (verticalLines.Length < currentSquare + squaresLeftToFillFromHint)
					{
						return false;
					}

					if (verticalLines[currentSquare][horziontalIndex] == '0')
					{
						currentSquare++;
						continue;
					}
					else if (verticalLines[currentSquare][horziontalIndex] != hint.ColorIndex.ToString()[0])
					{
						return false;
					}

					squaresLeftToFillFromHint--;
					currentSquare++;
				}
			}
		}

		return true;
	}


	private static bool TryCurrentVerticalLineCombination(string[] verticalLines, ColorHint[][] horizontalHints)
	{
		for (int i = 0; i < horizontalHints.Length; i++)
		{
			ColorHint[] currentHints = horizontalHints[i];

			if (!CurrentHorizontalIsPossible(verticalLines, currentHints, i))
			{
				return false;
			}
		}

		return true;
	}

	private static int[,] BuildSolutionFromVerticalLines(string[] verticalLines, int gridSize)
	{
		int[,] constructedGrid = new int[gridSize, gridSize];
		for (int x = 0; x < gridSize; x++)
		{
			for (int y = 0; y < gridSize; y++)
			{
				constructedGrid[x, y] = int.Parse(verticalLines[y][x].ToString());
			}
		}

		return constructedGrid;
	}

	private static int[][,] GetAllPossibleCombinationsOfVerticalLines(string[][] verticalLines, ColorHint[][] horizontalHints, int gridSize, int[,] correctSolution)
	{
		// Convert to array for quicker access

		List<int[,]> possibleSolutions = new List<int[,]>();

		int totalAmountOfCombinations = 1;
		for (int i = 0; i < verticalLines.Length; i++)
		{
			totalAmountOfCombinations *= verticalLines[i].Length;
		}

		int[] currentStates = new int[verticalLines.Length];
		for (int f = 0; f < currentStates.Length; f++)
		{
			currentStates[f] = 0;
		}

		for (int i = 0; i < totalAmountOfCombinations; i++)
		{
			string[] selectedLines = new string[currentStates.Length];
			for (int sl = 0; sl < selectedLines.Length; sl++)
			{
				int selectedIndexForCurrentVert = currentStates[sl];
				selectedLines[sl] = verticalLines[sl][selectedIndexForCurrentVert];
			}

			if (TryCurrentVerticalLineCombination(selectedLines, horizontalHints))
			{
				int[,] builtGrid = BuildSolutionFromVerticalLines(selectedLines, gridSize);
				if (!builtGrid.Equals(correctSolution))
				{
					possibleSolutions.Add(builtGrid);
				}
			}

			// Increase the state going from right to left based on the positions given base.
			for (int c = currentStates.Length - 1; c >= 0; c--)
			{
				currentStates[c] += 1;
				if (currentStates[c] < verticalLines[c].Length)
				{
					// State has been updated and can therefore continue processing.
					break;
				}

				// Reset state to zero to next loop increase the left index state.
				currentStates[c] = 0;
			}
		}

		return possibleSolutions.ToArray();
	}

	// Add -1 (Locked Crosses) to the puzzle to garauntee a single possible solution remains. 
	private static int[,] ProccessGridToReduceToASingleSolution(int[][,] possibleSolutions, int[,] correctSolution, int gridSize)
	{
		List<Vector2> changedPoints = new List<Vector2>();

		Queue<int[,]> solutionsStillLeft = new Queue<int[,]>();
		foreach (int[,] solution in possibleSolutions)
		{
			solutionsStillLeft.Enqueue(solution);
		}

		while (solutionsStillLeft.Count > 0)
		{
			int[,] currentSolution = solutionsStillLeft.Dequeue();
			bool stillWorks = true;
			foreach (Vector2 v in changedPoints)
			{
				int x = (int)v.x;
				int y = (int)v.y;
				if (currentSolution[x, y] != 0)
				{
					stillWorks = false;
					break;
				}

				currentSolution[x, y] = -1;
			}

			if (stillWorks)
			{
				bool addedPoint = false;
				//Generate a point that this grid has and that the correct grid doesn't and place it in grid. 

				for (int x = 0; x < gridSize; x++)
				{
					if (addedPoint)
					{
						break;
					}

					for (int y = 0; y < gridSize; y++)
					{
						if (currentSolution[x, y] > 0 && correctSolution[x, y] <= 0)
						{
							correctSolution[x, y] = -1;
							changedPoints.Add(new Vector2(x, y));
							addedPoint = true;
							break;
						}
					}
				}
			}
		}

		return correctSolution;
	}

	// Calculates all possible solutions given the provided hints and places immovable crosses to ensure only one solution is possible.
	public static int[,] GetOneSolutionGrid(int[,] currentGrid, int gridSize, ColorHint[][] verticalHints, ColorHint[][] horizontalHints)
	{
		// Get all vertical line states.
		string[][] verticalSolutions = new string[gridSize][];

		for (int i = 0; i < verticalHints.Length; i++)
		{
			int currentValue = 1;
			ColorHint[] currentLine = verticalHints[i];

			foreach (ColorHint hint in currentLine)
			{
				currentValue *= hint.GetHashCode();
			}

			currentValue *= gridSize;
			if (!AlreadyCalculatedVerticalLines.ContainsKey(currentValue))
			{
				HashSet<string> solutions = BuildPossibleSolutionsForVerticalLine(currentLine, gridSize);
				string[] arraySolutions = new string[solutions.Count];
				int counter = 0;
				foreach (string s in solutions)
				{
					arraySolutions[counter] = s;
					counter++;
				}

				AlreadyCalculatedVerticalLines.Add(currentValue, arraySolutions);
			}

			verticalSolutions[i] = AlreadyCalculatedVerticalLines[currentValue];
		}

		foreach (string[] list in verticalSolutions)
		{
			if (list == null)
			{
				GD.PrintErr("Missing list in vertical solutinos");
				return new int[0, 0];
			}

			if (list.Length == 0)
			{
				GD.PrintErr("Missing vertical solution for one line");
				return new int[0, 0];
			}
		}

		int[][,] possibleGrids = GetAllPossibleCombinationsOfVerticalLines(verticalSolutions, horizontalHints, gridSize, currentGrid);
		int[,] singleSolution = ProccessGridToReduceToASingleSolution(possibleGrids, currentGrid, gridSize);

		return singleSolution;
	}
}
