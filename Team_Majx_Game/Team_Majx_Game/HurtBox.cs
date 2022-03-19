using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Team_Majx_Game
{
    // Basic information for the
    // hurtbox of projectiles / damaging moves
    class HurtBox
    {
        private Rectangle position;

        public HurtBox(Rectangle position)
        {
            this.position = position;
        }
    }
}
