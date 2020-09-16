using System;
using DirectionHelper;
using Godot;

public class MovementComponent : Node2D {
    [Export] public float IMPACT_FACTOR = 800;
    [Export] public float WALK_SPEED = 100;
    [Export] public bool HAS_8_DIRECTION = false;
    public static Vector2 IsometricMultiplier = new Vector2 (1.5f, 1);

    public bool IsMaster { get { return !Network.IsConnectionStarted || IsNetworkMaster (); } }
    public bool IsTrueMaster { get { return Network.IsConnectionStarted && IsNetworkMaster (); } }

    public KinematicPiece MyBody;
    public Node2D MyPiece { get { return GetParent<Node2D> (); } }
    public ControlComponent MyControl {
        get { return MyPiece.GetNode<ControlComponent> ("Control"); }
    }

    KinematicCollision2D collInfo = null;
    [Signal] public delegate void BodyCollision (KinematicCollision2D collInfo);
    [Signal] public delegate void EndImpact ();

    public bool isImpact = false;
    private float impactTime = 0;
    private Vector2 impactDirection;

    [Puppet] private Direction currentDirection;
    [Master] public Direction CurrentDirection {
        get { return currentDirection; }
        set {
            if (value != currentDirection) {
                currentDirection = value;
                GD.Print ("Direction: ", value.ToString ());
                // MyControl.EmitSignal (nameof (ControlComponent.UpdateAnimation));
                MyControl.SetAnimation (value);
                if (Network.IsConnectionStarted)
                    Rset ("currentDirection", value);
            }
        }
    }

    [Puppet] private Vector2 nextMovement = new Vector2 (0, 0);
    public Vector2 NextMovement {
        get { return nextMovement; }
        set {
            nextMovement = value;
            if (IsTrueMaster)
                RsetUnreliable ("nextMovement", nextMovement);
            if (value != Vector2.Zero)
                CurrentDirection = (HAS_8_DIRECTION) ? DirectionHelper.DirMethods.ToDirection8 (value) : DirectionHelper.DirMethods.ToDirection4 (value);
        }
    }

    [Puppet] Vector2 PuppetPosition = new Vector2 (0, 0);

    public bool CanMove { get { return !isImpact; } }

    public override void _Ready () { MyBody = GetParent<KinematicPiece> (); }

    // Call whenever the piece is hit
    public void StartImpact (Vector2 direction, float time) {
        isImpact = true;
        impactTime = time;
        impactDirection = direction * IMPACT_FACTOR;
    }

    public override void _PhysicsProcess (float delta) {
        if (isImpact) { // Movement in case of impact
            impactTime -= delta;
            collInfo = MyBody.MoveAndCollide (impactDirection * IsometricMultiplier * delta);
            MyBody.UpdateZ ();
            if (impactTime <= 0) {
                impactTime = 0;
                isImpact = false;
                EmitSignal (nameof (EndImpact));
            }
        } else if (NextMovement != Vector2.Zero) { // Normal Movement
            collInfo = MyBody.MoveAndCollide (NextMovement * IsometricMultiplier * WALK_SPEED * delta);
            MyBody.UpdateZ ();
        }
        if (IsMaster) {
            NextMovement = new Vector2 (0, 0);
            if (collInfo != null)
                EmitSignal (nameof (BodyCollision), collInfo);
        }
        if (IsTrueMaster) //Master Code
            RsetUnreliable ("PuppetPosition", Position);
        else { //Puppet Code
            if (PuppetPosition != Vector2.Zero)
                Position = PuppetPosition;
        }

    }

}