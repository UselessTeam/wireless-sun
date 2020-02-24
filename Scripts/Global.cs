using Godot;

public class Global : Node {
	public static Inventory inventory;
	public static Global global;
	
	public override void _Ready() {
		Item.Manager.Load();
	}
}
