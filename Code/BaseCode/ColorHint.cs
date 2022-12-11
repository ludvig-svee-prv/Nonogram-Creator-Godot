using Godot;
using System;

using GodotDictionary = Godot.Collections.Dictionary;

public class ColorHint : Node
{
    public int Amount { get; private set; }
    public int ColorIndex { get; private set; }
    public bool IsEmptyHint { get; private set; }

    public GodotDictionary StructuredJSON { get; private set; }

    public ColorHint(GodotDictionary loadedDict)
    {
        StructuredJSON = loadedDict;

        Amount = int.Parse(loadedDict["Amount"].ToString());
        ColorIndex = int.Parse(loadedDict["ColorIndex"].ToString());
        IsEmptyHint = bool.Parse(loadedDict["IsEmptyHint"].ToString());
    }

    public ColorHint(int amount, int colorIndex)
    {
        Amount = amount;
        ColorIndex = colorIndex;
        IsEmptyHint = false;

        GenerateJSON();
    }

    public ColorHint()
    {
        IsEmptyHint = true;
        
        GenerateJSON();
    }

    public override bool Equals(object obj)
    {
        if (!(obj is ColorHint))
        {
            return false;
        }

        ColorHint hint = (ColorHint)obj;

        return hint.Amount == Amount && hint.ColorIndex == ColorIndex && hint.IsEmptyHint == IsEmptyHint;
    }

    private void GenerateJSON()
    {
        GodotDictionary jsonDict = new GodotDictionary();

        jsonDict.Add("Amount", Amount);
        jsonDict.Add("ColorIndex", ColorIndex);
        jsonDict.Add("IsEmptyHint", IsEmptyHint);

        StructuredJSON = jsonDict;
    }

    public override int GetHashCode()
    {
        int value = 17;
        value *= Amount + 17;
        value *= ColorIndex + 17;
        value *= IsEmptyHint ? 17 : 1;
        return value;
    }
}
