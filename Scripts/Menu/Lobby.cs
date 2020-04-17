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
		GameRoot.username = name;
	}

	void _OnIpChanged (string new_text) {
		ipAdress = new_text;
	}

	void _OnSoloPressed () {
		this.Hide ();
		GetNode<Control> ("../SaveMenu").Show ();
	}

	void _OnHostPressed () {
		Network.Host ();
		_OnSoloPressed ();
	}

	void _OnJoinPressed () {
		Network.Join (ipAdress);
	}

	void _OnJoinedAServer () {
		GameRoot.LoadGameScene ("");
	}

	// void _StartGame () {
	// 	//
	// 	Global.LoadGameScene ("");
	// }
}
