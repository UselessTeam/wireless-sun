using System;
using Godot;

public class CooldownTween : Tween {
    PlayerControl playerControl;
    public override void _Ready () {
        GameRoot.Instance.Connect (nameof (GameRoot.GameplayStarted), this, nameof (OnGameplayStarted));
        Connect ("tween_all_completed", this, nameof (_OnTweenCompleted));
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
        GetParent<Node2D> ().Show ();
        InterpolateProperty (GetNode<Node2D> ("../CooldownBar"), "scale", new Vector2 (1, 1), new Vector2 (1, 0.1f), time, TransitionType.Linear, EaseType.In);
        Start ();
    }

    public void _OnTweenCompleted () {
        GetParent<Node2D> ().Hide ();
    }
}