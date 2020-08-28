using System;
using Godot;
using Newtonsoft.Json;

public class PlayerStats : Node {
    public float Hp = 100;
    public float Attack = 0;
    public float AttackSpeed = 1;
    public float CriticalHits = 5;
    public float Speed = 1;

    public string Serialize () {
        var saveObject = new Godot.Collections.Dictionary<string, object> () {};
        saveObject[nameof (Hp)] = Hp;
        saveObject[nameof (Attack)] = Attack;
        saveObject[nameof (AttackSpeed)] = AttackSpeed;
        saveObject[nameof (CriticalHits)] = CriticalHits;
        saveObject[nameof (Speed)] = Speed;
        return JSON.Print (saveObject);
    }
    public void Deserialize (string serializedData) {
        var data = JsonConvert.DeserializeObject<Godot.Collections.Dictionary<string, object>> (serializedData);
        GetNode<HealthComponent> ("../../Health").MaxHp = Hp;
    }
}