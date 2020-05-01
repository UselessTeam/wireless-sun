using System;
using Godot;

public class CooldownTween : Tween {
    PlayerControl playerControl;
    public override void _Ready () {
        GameRoot.Instance.Connect (nameof (GameRoot.GameplayStarted), this, nameof (OnGameplayStarted));
    }
    public void OnGameplayStarted () {
        Gameplay.Instance.Connect (nameof (Gameplay.PlayerRespawn), this, nameof (OnPlayerRespawn));
        OnPlayerRespawn (); // The first spawn doesn't  have time to reach 
    }
    public void OnPlayerRespawn () {
        playerControl = Gameplay.myPlayer.GetNode<PlayerControl> ("Control");
        playerControl.Connect (nameof (PlayerControl.StartCooldown), this, nameof (_OnStartCooldown));
    }

    public void _OnStartCooldown (string locationName, float time) {
        if (GetParent ().GetParent ().Name != locationName)
            return;
        InterpolateProperty (GetParent<Node2D> (), "scale", new Vector2 (2, 2), new Vector2 (2, 0), time, TransitionType.Linear, EaseType.In);
        Start ();
    }
}