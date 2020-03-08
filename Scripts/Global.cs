using Godot;

public class Global : Node {
	public static Inventory inventory;
	public static Global global;
	public static string username = "";

	public override void _Ready () {
		global = this;
		Item.Manager.Load ();
	}

	public static void LoadGameScene () {
		global.GetTree ().ChangeScene ("res://Scenes/Beach.tscn");
	}

	public static void LoadLobbyScene () {
		global.GetTree ().ChangeScene ("res://Scenes/Lobby.tscn");
	}
}