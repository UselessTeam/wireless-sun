using System;
using Craft;
using Godot;

public class CraftGUI : PanelContainer {
    private PackedScene packedInventoryItem = (PackedScene) ResourceLoader.Load ("res://Nodes/GUI/InventoryItem.tscn");
    private PackedScene packedCraftingArrow = (PackedScene) ResourceLoader.Load ("res://Nodes/GUI/CraftingArrow.tscn");

    public GridContainer myGrid { get { return GetNode<GridContainer> ("Grid"); } }

    public void Display (CraftData craft) {

        foreach (var ingredient in craft.ingredients) {
            var ingredientItem = (InventoryItem) packedInventoryItem.Instance ();
            myGrid.AddChild (ingredientItem);
            ingredientItem.Display (ingredient.ToItemStack ());
            GD.Print (ingredient.item);
        }
        var arrow = packedCraftingArrow.Instance ();
        myGrid.AddChild (arrow);
        var result = (InventoryItem) packedInventoryItem.Instance ();
        myGrid.AddChild (result);
        result.Display (craft.ToItemStack ());

    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}