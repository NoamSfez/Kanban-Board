using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation
{

    /// <summary>
    /// Interface indicating a closable object.
    /// Main purpose is to be able to close a window from a viewmodel cleanly.
    /// Idea from https://stackoverflow.com/questions/16172462/close-window-from-viewmodel
    /// </summary>
    interface ICloseable
    {
        void Close();
    }
}
