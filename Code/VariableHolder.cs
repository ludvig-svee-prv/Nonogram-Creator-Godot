using Godot;
using System;

public class VariableHolder : Node
{
    // Used as a way to pass variables between different scenes.

    public PuzzleDefinition CurrentPuzzleDefinition { get; set; }
}
