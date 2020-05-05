using System;
using Godot;

public class PopupCheck : Popup {
    Node targetNode;
    string targetMethod;
    object[] parameters;
    public void StartPopupCheck (Node targetNode, string targetMethod, params object[] parameters) {
        this.targetMethod = targetMethod;
        this.targetNode = targetNode;
        this.parameters = parameters;
        Popup_ ();
    }

    public void _OnYesPressed () {
        targetNode.Call (targetMethod, parameters);
        _OnNoPressed ();
    }

    public void _OnNoPressed () {
        this.Hide ();
        targetNode = null;
        targetMethod = "";
    }
}