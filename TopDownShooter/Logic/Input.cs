using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using TopDownShooter.GameObjects;

namespace TopDownShooter.Utils
{
    public class Input
    {
        public Keys Left { get; set; }
        public Keys Right { get; set; }
        public Keys Up { get; set; }
        public Keys Down { get; set; }

        public Keys Throw { get; set; }
        public Keys Teleport { get; set; }
        public Keys Reload { get; set; }
        public Keys Punch { get; set; }

        public Cursor Cursor;

        public List<Keys> GetKeys()
        {
            // return list of the movement keys
            return new List<Keys>() {Left, Right, Up, Down };
        } 
    }
}
