using Godot;
using System;
using System.Collections.Generic;

public static class General {
    public static readonly Random rng = new Random();
}
public static class NodeExtension {
    public static List<T> GetChildren<T>(this Node node) where T : Node {
        List<T> result = new List<T>();
        foreach(Node child in node.GetChildren()) {
            T tChild = child as T;
            if(tChild != null) {
                result.Add(tChild);
            }
        }
        return result;
    }
}
