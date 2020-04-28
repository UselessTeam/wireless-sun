using System;
using Godot;

public class Tile : Node2D {
	public static PackedScene template = (PackedScene) ResourceLoader.Load ("res://Nodes/Map/Tile.tscn");
	public static Tile Instance () {
		return (Tile) template.Instance ();
	}

	public const int HALF_WIDTH = 24;
	public const int HALF_HEIGHT = 16;
	public const int WIDTH = 2 * HALF_WIDTH;
	public const int HEIGHT = 2 * HALF_HEIGHT;
	private const int DEPTH = 8;

	private const int SPRITE_WIDTH = WIDTH;
	private const int SPRITE_HEIGHT = 64;

	private Sprite _sprite;
	private Sprite sprite {
		get {
			if (_sprite == null) {
				_sprite = GetNode ("Sprite") as Sprite;
			}
			return _sprite;
		}

	}

	public int typeId { get; private set; }

	public int u { get; private set; }
	public int v { get; private set; }
	public int w { get; private set; }

	public void SetCoord (int u, int v, int w) {
		this.u = u;
		this.v = v;
		this.w = w;
		this.Position = new Vector2 ((u - v) * HALF_WIDTH, (u + v) * HALF_HEIGHT - w * DEPTH);
		this.sprite.ZIndex = (u + v);
	}

	public void SetType (int typeId) {
		this.typeId = typeId;
		this.sprite.RegionRect = new Rect2 (typeId * SPRITE_WIDTH, 0, SPRITE_WIDTH, SPRITE_HEIGHT);
	}
}
