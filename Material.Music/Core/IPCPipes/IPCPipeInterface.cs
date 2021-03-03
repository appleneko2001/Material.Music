using System;
using System.Collections.Generic;
using System.Text;

namespace Material.Music.Core.IPCPipes
{
    public interface IPCPipeInterface
    {
        bool SendData(byte[] data);
        event EventHandler<byte[]> OnReceivedData;
    }
}
