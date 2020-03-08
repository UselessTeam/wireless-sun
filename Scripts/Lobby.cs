using System;
using Godot;

public class Lobby : Control {

	public static Lobby Instance { get { return instance; } }
	static Lobby instance = null;

	public override void _Ready () {
		instance = this;
		GetTree ().Connect ("connected_to_server", this, "_OnJoinedAServer");
	}

	string name = "";
	string ipAdress = "";

	void _OnNameChanged (string new_text) {
		name = new_text;
		Global.username = name;
	}

	void _OnIpChanged (string new_text) {
		ipAdress = new_text;
	}

	void _OnHostPressed () {
		Network.Host ();
		Global.LoadGameScene ();
	}

	void _OnJoinPressed () {
		Network.Join (ipAdress);
	}

	void _OnJoinedAServer () {
		Global.LoadGameScene ();
	}

}
