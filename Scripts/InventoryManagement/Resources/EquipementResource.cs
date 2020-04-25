using System;
using Godot;

public class EquipementResource : ItemResource {
    [Export] public float armor = 0;
    [Export] public string type = "none";
    [Export] public string location = "hand";
}