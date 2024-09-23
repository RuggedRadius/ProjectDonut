using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDonut.GameObjects.Doodads
{
    public enum InteractableState
    {
        Disabled,
        Inacessible,
        Acessible,
        Interacting,
        Interacted
    }

    public interface IInteractable
    {
        InteractableState State { get; set; }
        Rectangle InteractBounds { get; set; }

        bool PlayerInRange { get; set; }

        void Interact();
        void Update(GameTime gameTime);
        void Draw(GameTime gameTime);
    }
}
