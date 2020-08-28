using System;
using Godot;

public class SaveMenu : Control {
    [Export] PackedScene loadButtonPackedScene;

    public static SaveMenu Instance { get { return instance; } }
    static SaveMenu instance = null;

    public Control MainSaveMenu;
    public Control ChooseNewName { get { return GetNode<Control> ("ChooseNewName"); } }
    public Control OpenedSave { get { return GetNode<Control> ("OpenedSave"); } }

    public string newName = "";
    public string currentSave;

    int initialChildCount = 0;

    public override void _Ready () {
        instance = this;
        MainSaveMenu = GetNode<Control> ("MainSaveMenu");
        Connect ("draw", this, nameof (UpdateSaveList));
        initialChildCount = GetChildCount ();
    }

    public void UpdateSaveList () {
        ChooseNewName.GetNode<Label> ("NameError").Text = "";
        for (int i = initialChildCount; i < MainSaveMenu.GetChildCount (); i++) {
            MainSaveMenu.GetChild (i).QueueFree ();
        }
        foreach (var save in Save.GetSaveList ()) {
            var loadButton = (LoadButton) loadButtonPackedScene.Instance ();
            loadButton.Name = save;
            loadButton.Text = save;
            if (GetNodeOrNull (save) != null)
                GD.Print ("Save already exists :", save);
            else {
                MainSaveMenu.AddChild (loadButton);
                loadButton.Connect (nameof (LoadButton.LoadPressed), this, nameof (_OnLoadPressed));
            }
        }
    }

    void _OnNewPressed () {
        MainSaveMenu.Hide ();
        ChooseNewName.Show ();
    }
    void _OnBackPressed () {
        this.Hide ();
        GetNode<Control> ("../BaseMenu").Show ();
    }
    public void _OnLoadPressed (string name) {
        currentSave = name;
        MainSaveMenu.Hide ();
        OpenedSave.Show ();
    }

    public void _OnPlayPressed () {
        GameRoot.LoadGameScene (currentSave);
    }
    void _OnBackToSaveMenuPressed () {
        ChooseNewName.Hide ();
        OpenedSave.Hide ();
        MainSaveMenu.Show ();
        currentSave = "";
    }
    void _OnDeletePressed () { GetNode<PopupCheck> ("/root/Menu/PopupCheck").StartPopupCheck (this, nameof (_OnDeleteConfirmed)); }

    void _OnDeleteConfirmed () {
        Save.DeleteSave (currentSave);
        _OnBackToSaveMenuPressed ();
        UpdateSaveList ();
    }

    void _OnNewNameEdit (string line) {
        newName = line;
    }
    void _OnMakeSavePressed () {
        if (newName == "")
            ChooseNewName.GetNode<Label> ("NameError").Text = "You need to choose a name";

        else if (!NameIsNew ())
            ChooseNewName.GetNode<Label> ("NameError").Text = "Name already exists";
        else {
            GameRoot.BuildNewWorld (newName);
            ChooseNewName.Hide ();
            MainSaveMenu.Show ();
            UpdateSaveList ();
        }
    }
    bool NameIsNew () {
        foreach (var save in Save.GetSaveList ()) {
            if (save.ToLower () == newName.ToLower ())
                return false;
        }
        return true;

    }

}