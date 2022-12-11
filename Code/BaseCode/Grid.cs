using Godot;
using System;

public abstract class Grid : Control
{
    protected VariableHolder variableHolder;

    public override void _Ready()
    {
		variableHolder = GetNode<VariableHolder>("/root/VariableHolder");
    }

    public abstract void SquareClicked(int x, int y);

    public abstract void SquareClickedErase(int x, int y);
}
