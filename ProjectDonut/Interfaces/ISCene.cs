using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace ProjectDonut.Interfaces
{
    /*
     * Convert Scene objects to this instead later!
     * */

    public interface IScene
    {
        int[,] DataMap { get; set; }

        Dictionary<string, Rectangle> ExitLocations { get; set; }

        // etc etc
    }
}
