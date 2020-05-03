using System;
using Godot;

public interface IPiece {
    void LoadData (Godot.Collections.Dictionary<string, object> saveObject);
}