using System;
using Godot;

public class BaseMenu : Control {
	void _OnSoloPressed () {
		this.Hide ();
		GetNode<Control> ("../SaveMenu").Show ();
	}
	void _OnMultiplayerPressed () {
		this.Hide ();
		GetNode<Control> ("../Lobby").Show ();
	}
	void _OnQuitPressed () {
		GetTree ().Quit ();
	}
}
