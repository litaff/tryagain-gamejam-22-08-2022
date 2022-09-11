using System;

namespace _Scripts
{
    public abstract class Npc : Character
    {
        protected Random Random;

        public void Init(float gravity, float maxJumpVelocity, float minJumpVelocity, Random random)
        {
            base.Init(gravity, maxJumpVelocity, minJumpVelocity);
            Random = random;
        }
        public abstract void Act();
    }
}