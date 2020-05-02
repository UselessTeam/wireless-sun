using System;
using Godot;

public class ChargeBar : TextureProgress {
    PlayerControl playerControl;
    public override void _Ready () {
        GetParent<PlayerControl> ().Connect (nameof (PlayerControl.StartCharge), this, nameof (_OnStartCharge));
        GetParent<PlayerControl> ().Connect (nameof (PlayerControl.StopCharge), this, nameof (_OnStopCharge));
    }

    public void _OnStartCharge (float chargeMax) {
        MaxValue = chargeMax;
        Value = 0;
        this.Show ();
    }

    public void _OnStopCharge () {
        this.Hide ();
        MaxValue = 0;
        Modulate = new Color (1, 1, 1);
    }

    public override void _Process (float delta) {
        if (MaxValue > 0) {
            if (Value < MaxValue)
                Value += delta;
            if (Value >= MaxValue)
                Modulate = new Color (1, 0.5f, 0);
        }
    }
}