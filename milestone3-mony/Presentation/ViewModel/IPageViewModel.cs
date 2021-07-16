using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Presentation
{
    public interface IPageViewModel
    {
        string Name { get; }

        void Reload()
        {
        }
    }
}
