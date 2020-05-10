using Godot;
using System;

public class ChangableSprite : Sprite, ICustomizablePart {
    [Export]
    private int firstIndex;
    [Export]
    private int totalIndex;
    
    [Signal]
    delegate void changed();

    public override void _Ready () {
        Frame = firstIndex + key;
    }

    public int key { protected set; get; } = 0;

    public void Change(int to) {
        if (key == to) {
            return;
        }
        to = key;
        Frame = firstIndex + to;
        EmitSignal(nameof(changed));
    }

    public virtual void RandomSetup() {
        int new_value = General.rng.Next(totalIndex);
        Change(new_value);
    }
}
