using Godot;
using System;
using System.Text;
using System.Collections.Generic;

using GodotDictionary = Godot.Collections.Dictionary;
using GodotArray = Godot.Collections.Array;

public class PuzzleDefinition
{
    public bool IsColorGrid { get; private set; }

    public int GridSize { get; private set; }

    public int[,] GridDefinition { get; private set; }

    public string JSONString { get; private set; }

    public ColorHint[][] VerticalHints { get; private set; }
    public ColorHint[][] HorizontalHints { get; private set; }

    public PuzzleDefinition(bool isColor, GridSquare[,] currentSquareGrid, int currentSize)
    {
        IsColorGrid = isColor;
        GridSize = currentSize;
        if (IsColorGrid)
        {
            VerticalHints = GenerateColorHints(currentSquareGrid, currentSize, true);
            HorizontalHints = GenerateColorHints(currentSquareGrid, currentSize, false);
        }
        else
        {
            VerticalHints = GenerateBWHints(currentSquareGrid, currentSize, true);
            HorizontalHints = GenerateBWHints(currentSquareGrid, currentSize, false);
        }
    }

    public PuzzleDefinition(string jsonDefinition)
    {
        JSONString = jsonDefinition;
        GodotDictionary jsonDict = JSON.Parse(jsonDefinition).Result as GodotDictionary;

        VerticalHints = LoadInHints(jsonDict["VerticalHints"] as GodotArray);
        HorizontalHints = LoadInHints(jsonDict["HorizontalHints"] as GodotArray);

        IsColorGrid = bool.Parse(jsonDict["IsColor"].ToString());
        GridSize = int.Parse(jsonDict["GridSize"].ToString());

        string gridJson = jsonDict["Grid"].ToString();
        string[] lines = gridJson.Split("|");

        GridDefinition = new int[GridSize, GridSize];
        
        for(int x = 0; x < GridSize; x++)
        {
            string[] lineSplit = lines[x].Split(",");
            for (int y = 0; y < GridSize; y++)
            {
                GridDefinition[x,y] = int.Parse(lineSplit[y]);
            }
        }
    }

    private ColorHint[][] LoadInHints(GodotArray directionHints)
    {
        ColorHint[][] loadedHints = new ColorHint[directionHints.Count][];
        for (int i = 0; i < directionHints.Count; i++)
        {
            GodotArray currentLine = directionHints[i] as GodotArray;
            string s = directionHints[i].ToString();

            ColorHint[] currentLoadedLineHints = new ColorHint[currentLine.Count];
            for (int h = 0; h < currentLoadedLineHints.Length; h++)
            {
                GodotDictionary hint = currentLine[h] as GodotDictionary;
                currentLoadedLineHints[h] = new ColorHint(hint);
            }

            loadedHints[i] = currentLoadedLineHints;
        }

        return loadedHints;
    }

    public void UpdateGridDefinition(int[,] newGrid)
    {
        GridDefinition = newGrid;
        GeneratePuzzleJSON();
    }

    private GodotArray ConvertHintsToGodotArray(ColorHint[][] orgHintArray)
    {
        GodotArray convertedArray = new GodotArray();
        for (int a = 0; a < orgHintArray.Length; a++)
        {
            GodotArray hintArray = new GodotArray();
            ColorHint[] currentHints = orgHintArray[a];
            for (int h = 0; h < currentHints.Length; h++)
            {
                hintArray.Add(currentHints[h].StructuredJSON);
            }
            convertedArray.Add(hintArray);
        }

        return convertedArray;
    }

    private void GeneratePuzzleJSON()
    {
        GodotDictionary jsonDictionary = new GodotDictionary();
        StringBuilder gridDefintionStringBuilder = new StringBuilder();

        for (int x = 0; x < GridSize; x++)
        {
            for (int y = 0; y < GridSize; y++)
            {
                gridDefintionStringBuilder.Append(GridDefinition[x, y]);
                if (y < GridSize - 1)
                {
                    gridDefintionStringBuilder.Append(",");
                }
            }

            if (x < GridSize - 1)
            {
                gridDefintionStringBuilder.Append('|');
            }
        }

        jsonDictionary.Add("VerticalHints", ConvertHintsToGodotArray(VerticalHints));
        jsonDictionary.Add("HorizontalHints", ConvertHintsToGodotArray(HorizontalHints));
        jsonDictionary.Add("Grid", gridDefintionStringBuilder.ToString());
        jsonDictionary.Add("IsColor", IsColorGrid);
        jsonDictionary.Add("GridSize", GridSize);

        JSONString = JSON.Print(jsonDictionary);
    }


    // Uses mainLine and secLine instead of X and Y to allow repeat of code for both vertical and horizontal hint generation
    // Used to generate hints for multi-color puzzles
    private ColorHint[][] GenerateColorHints(GridSquare[,] currentSquareGrid, int size, bool isVertical)
    {
        List<ColorHint[]> lines = new List<ColorHint[]>();
        for (int mainLine = 0; mainLine < size; mainLine++)
        {
            List<ColorHint> currentLine = new List<ColorHint>();
            int currentCount = 0;
            int currentColorIndex = 0;

            for (int secLine = 0; secLine < size; secLine++)
            {
                GridSquare g = isVertical ? currentSquareGrid[mainLine, secLine] : currentSquareGrid[secLine, mainLine];

                if (g.CurrentColorIndex != 0)
                {
                    if (currentCount > 0 && currentColorIndex == g.CurrentColorIndex || currentCount == 0)
                    {
                        currentCount += 1;
                        currentColorIndex = g.CurrentColorIndex;
                    }
                    else
                    {
                        currentLine.Add(new ColorHint(currentCount, currentColorIndex));
                        currentCount = 1;
                        currentColorIndex = g.CurrentColorIndex;
                    }
                }
                else if (currentCount > 0)
                {
                    currentLine.Add(new ColorHint(currentCount, currentColorIndex));
                    currentCount = 0;
                    currentColorIndex = 0;
                }
            }

            if (currentColorIndex != 0)
            {
                currentLine.Add(new ColorHint(currentCount, currentColorIndex));
            }

            if (currentLine.Count == 0)
            {
                currentLine.Add(new ColorHint(0, 0));
            }

            lines.Add(currentLine.ToArray());
        }
        return lines.ToArray();
    }

    // Uses mainLine and secLine instead of X and Y to allow repeat of code for both vertical and horizontal hint generation
    // Used to generate hints for monochromatic puzzles
    private ColorHint[][] GenerateBWHints(GridSquare[,] currentSquareGrid, int size, bool isVertical)
    {
        List<ColorHint[]> lines = new List<ColorHint[]>();
        for (int mainLine = 0; mainLine < size; mainLine++)
        {
            List<ColorHint> currentLine = new List<ColorHint>();
            int currentCount = 0;

            for (int secLine = 0; secLine < size; secLine++)
            {
                GridSquare g = isVertical ? currentSquareGrid[mainLine, secLine] : currentSquareGrid[secLine, mainLine];

                if (g.CurrentColorIndex > 0)
                {
                    currentCount += 1;
                }
                else if (currentCount > 0)
                {
                    currentLine.Add(new ColorHint(currentCount, 1));
                    currentLine.Add(new ColorHint()); // Ensure that empty spaces are added for monochrome calculations
                    currentCount = 0;
                }
            }

            if (currentLine.Count == 0)
            {
                currentLine.Add(new ColorHint(0, 0));
            }

            lines.Add(currentLine.ToArray());
        }
        return lines.ToArray();
    }
}
