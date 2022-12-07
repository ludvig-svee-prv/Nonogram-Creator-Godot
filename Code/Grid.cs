using Godot;
using System;

public abstract class Grid : Control
{
    public override void _Ready()
    {
        
    }

    public abstract void SquareClicked(int x, int y);

    public abstract void SquareClickedErase(int x, int y);
}
