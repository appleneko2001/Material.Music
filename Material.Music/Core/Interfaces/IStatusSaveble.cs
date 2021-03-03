using System;
using System.Collections.Generic;
using System.Text;

namespace Material.Music.Core.Interfaces
{
    public interface IStatusSaveble
    {
        bool RequestSaveStatus();
        bool RequestLoadStatus();
    }
}
