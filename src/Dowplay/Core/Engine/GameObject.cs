namespace Coreblock;

using Microsoft.Xna.Framework.Graphics;

public abstract class GameObject : UniversalPhysicsBody
{
    protected GameObject(Texture2D texture) : base(texture){

    }

    public virtual void Update() {
        base.Update(Globals.Time);
    }
}