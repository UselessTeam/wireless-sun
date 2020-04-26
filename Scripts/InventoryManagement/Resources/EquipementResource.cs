using System;
using Godot;

public class EquipementResource : ItemResource {
    [Export] public float armor = 0;
    [Export] public EquipementLocation location;
}

public enum EquipementLocation {
    Hand,
    Helmet,
    Torso,
    Boots
}