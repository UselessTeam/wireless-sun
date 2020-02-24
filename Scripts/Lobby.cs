using System;
using Godot;

public class Lobby : Control {

	string PlayerName = "";
	string IpAdress = "";

	void _OnNameChanged (string new_text) {
		PlayerName = new_text;
	}

	void _OnIpChanged (string new_text) {
		IpAdress = new_text;
	}

	void _OnHostPressed () {
		LoadGame ();
	}

	void _OnJoinPressed () {
		LoadGame ();
	}

	void LoadGame () {
		GetTree ().ChangeScene ("res://Scenes/Beach.tscn");
	}
}
