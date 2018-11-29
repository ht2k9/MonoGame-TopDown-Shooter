using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TopDownShooter.Utilities
{
    public static class OwnMath
    {
        public static float CalculateAngleBetweenPoints(Point p1, Point p2)
        {
            // get the difference x and y from the points
            float deltaX = p2.X - p1.X;
            float deltaY = p2.Y - p1.Y;
            // calculate the angle
            float res = (float)(Math.Atan2(deltaY, deltaX));
            return res;
        }

        public static Vector2 GetDirectionToPoint(Point p1, Point p2)
        {
            // get the difference x and y from the points
            float diffX = p2.X - p1.X;
            float diffY = p2.Y - p1.Y;

            // returns a new vector from the difference
            Vector2 res = new Vector2(diffX, diffY);

            return res;
        }
    }
}
