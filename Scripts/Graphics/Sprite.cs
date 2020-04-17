using Godot;

namespace Graphics {
    public struct Sprite {
        public byte sheet;
        public byte x;
        public byte y;

        public Sprite (byte sheet, byte x, byte y) { this.sheet = sheet; this.x = x; this.y = y; }

        public Rect2 GetRect () {
            return new Rect2 (x * 12, y * 12, 12, 12);
        }
    }
}