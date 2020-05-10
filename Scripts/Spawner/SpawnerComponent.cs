using System;
using Godot;

public abstract class SpawnerComponent : Node2D {
    int nonSpawneeChildren = 1;

    [Export] public float SpawnDelay = 2; // Time (in seconds) between two spawns
    [Export] public int RandomTimeSpread = 50; // Random spread of the time between two spawns
    [Export] public float MaxCount = 10; // Max number of spawned objects exising at the same time
    [Export] public float MaxTries = 1; // Number of times this node will try to spawn an object 

    // public double next_spawn_delay;

    public bool IsMaster { get { return !Network.IsConnectionStarted || IsNetworkMaster (); } }
    public bool IsTrueMaster { get { return Network.IsConnectionStarted && IsNetworkMaster (); } }

    public float Radius {
        get { return ((CircleShape2D) GetNode<CollisionShape2D> ("Area2D/CollisionShape2D").Shape).Radius; }
        set {
            ((CircleShape2D) GetNode<CollisionShape2D> ("Area2D/CollisionShape2D").Shape).Radius = value;
        }
    }

    double timeUntilNext;

    int spawnID = 0;

    public override void _Ready () {
        nonSpawneeChildren = GetChildCount ();
        if (!IsMaster)
            Rpc ("SendAllSpawnees", GetTree ().GetNetworkUniqueId ());
        if (IsMaster)
            AddToGroup ("SaveNodes");
        timeUntilNext = NextSpawnDelay ();
        this.UpdateZNode (+2);
    }

    public double NextSpawnDelay () { return SpawnDelay * (1 + General.rng.Next (-RandomTimeSpread, RandomTimeSpread) / 100.0); }

    public override void _Process (float delta) {
        if (IsMaster && IsActive ()) {
            timeUntilNext -= delta;
            if (timeUntilNext <= 0) {
                timeUntilNext = SpawnDelay * (1 + General.rng.Next (-RandomTimeSpread, RandomTimeSpread) / 100.0);
                Vector2 position;
                ushort numberTries = 0;
                do {
                    position = GenerateSpawnPosition (Radius);
                    if (GetWorld2d ().DirectSpaceState.IntersectPoint (GlobalPosition + position, 1, null, 1024, true, true).Count == 0) {
                        if (Map.Global == null || Map.Global.GetPositionFlavor(GlobalPosition + position).altitude > 0) {
                            // TODO: Instead of checking only altitude, check "score" of position, according to formula defined in spawned's data
                            if (Network.IsConnectionStarted)
                                Rpc ("SpawnOne", spawnID.ToString (), position);
                            else
                                SpawnOne (spawnID.ToString (), position);
                            spawnID++;
                            break;
                        }
                    }
                    if (numberTries > MaxTries) {
                        numberTries = 0;
                        break;
                    }
                    numberTries++;
                } while (numberTries < MaxTries);
            }
        }
    }

    int GetNumberSpawnees () {
        return GetChildCount () - nonSpawneeChildren;
    }

    // Weather the spawner is active
    bool IsActive () { return GetNumberSpawnees () < MaxCount; }

    public static Vector2 GenerateSpawnPosition (float Radius) {
        double theta = General.rng.NextDouble () * 2 * Math.PI;
        return new Vector2 ((float) Math.Cos (theta), (float) Math.Sin (theta)) * (float) General.rng.NextDouble () * Radius;

    }

    abstract protected IPiece GetSpawnee ();

    [PuppetSync]
    void SpawnOne (string name, Vector2 position) {
        var spawnNode = (Node2D) GetSpawnee ();
        spawnNode.Name = name;
        AddChild (spawnNode);
        spawnNode.Position = position;
        (spawnNode as IPiece).UpdateZ ();
    }

    [Master]
    void SendAllSpawnees (int id) {
        for (int i = 0; i < GetNumberSpawnees (); i++) {
            var spawnee = GetChild<Node2D> (i + nonSpawneeChildren);
            var name = spawnee.Name;
            GD.Print ("Spawning ", name);
            var position = spawnee.Position;
            RpcId (id, "SpawnOne", name, new Vector2 (0, 0));
        }
    }

    public Godot.Collections.Dictionary<string, object> MakeSave () {
        var saveObject = new Godot.Collections.Dictionary<string, object> () { { "Path", GetPath () }, {
                    "TimeUntilNext",
                    timeUntilNext
                    }, { "SpawnID", spawnID }, {
                    "PositionX",
                    Position.x
                    }, { "PositionY", Position.y }
            };
        return saveObject;
    }
    public void LoadData (Godot.Collections.Dictionary<string, object> saveObject) {
        Position = new Vector2 (Convert.ToSingle (saveObject["PositionX"]), Convert.ToSingle (saveObject["PositionY"]));
        timeUntilNext = Convert.ToSingle (saveObject["TimeUntilNext"]);
        spawnID = Convert.ToInt32 (saveObject["SpawnID"]);
    }
}