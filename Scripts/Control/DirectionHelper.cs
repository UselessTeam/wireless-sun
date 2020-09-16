using System;
using Godot;

namespace DirectionHelper {
    public static class DirMethods {
        static readonly double TANPI_8 = Math.Sqrt (2) - 1;
        public static Vector2 InputDirection () {
            Vector2 inputDirection = new Vector2 (0, 0);
            if (Input.IsActionPressed ("ui_up"))
                inputDirection.y = -1;
            if (Input.IsActionPressed ("ui_down"))
                inputDirection.y = 1;
            if (Input.IsActionPressed ("ui_left"))
                inputDirection.x = -1;
            if (Input.IsActionPressed ("ui_right"))
                inputDirection.x = 1;
            return inputDirection.Normalized ();
        }
        public static Direction ToDirection8 (Vector2 v) { //SENSITIVE CODE! RUN EVERY FRAME
            short num;
            if (v.x < -TANPI_8 * Math.Abs (v.y)) //LEFT
                num = 0;
            else if (v.x > TANPI_8 * Math.Abs (v.y)) //RIGHT
                num = 3;
            else
                num = 5;
            if (v.y < -TANPI_8 * Math.Abs (v.x)) //BACK
                num += 1;
            else if (v.y > TANPI_8 * Math.Abs (v.x)) //FRONT
                num += 2;

            return (Direction) num;
        }
        public static Direction ToDirection4 (Vector2 v) { //SENSITIVE CODE! RUN EVERY FRAME
            short num;
            if (v.x < 0) //LEFT
                num = 0;
            else //RIGHT
                num = 3;
            if (v.y < 0) //BACK
                num += 1;
            else //FRONT
                num += 2;

            return (Direction) num;
        }
    }

    public enum Direction {
        right,
        back_right,
        front_right,
        left,
        back_left,
        front_left,
        back,
        front,
    }
}