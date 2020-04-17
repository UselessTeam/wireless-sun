using System;
using Craft;
using Godot;

public class CraftGUI : PanelContainer {
    private PackedScene packedCraftableItem = (PackedScene) ResourceLoader.Load ("res://Nodes/GUI/GuiItem/CraftableItem.tscn");
    private PackedScene packedIngredientItem = (PackedScene) ResourceLoader.Load ("res://Nodes/GUI/GuiItem/IngredientItem.tscn");
    private PackedScene packedCraftingArrow = (PackedScene) ResourceLoader.Load ("res://Nodes/GUI/GuiItem/CraftingArrow.tscn");

    public GridContainer myGrid { get { return GetNode<GridContainer> ("Grid"); } }

    public void Display (CraftData craft) {

        foreach (var ingredient in craft.ingredients) {
            var ingredientItem = (IngredientItem) packedIngredientItem.Instance ();
            myGrid.AddChild (ingredientItem);
            ingredientItem.Display (ingredient.ToItemSlot ());
        }
        var arrow = packedCraftingArrow.Instance ();
        myGrid.AddChild (arrow);
        var result = (CraftableItem) packedCraftableItem.Instance ();
        myGrid.AddChild (result);
        result.Display (craft);

    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}