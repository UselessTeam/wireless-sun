using Godot;

public class WorldShader : ColorRect {
    private bool initialized = false;
    
    public override void _Process(float _delta) {
        if(!initialized) {
            initialized = true;
            var root = GetTree().Root;
            GetParent().RemoveChild(this);
            root.AddChild(this);
        }
        RectPosition = -0.5f*GetViewport().CanvasTransform.origin - new Vector2(4,4);
        RectSize = GetViewport().Size + new Vector2(8,8);
    }
}
