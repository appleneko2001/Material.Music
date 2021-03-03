using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Text;

namespace Material.Music.Core.IPCPipes
{
    public class WindowsIPCPipe : IPCPipeInterface, IDisposable
    {
        private NamedPipeServerStream pipeServer;
        private NamedPipeClientStream pipeClient;
        private bool isServer = false;
        public WindowsIPCPipe(string PipeName, bool server = false)
        {
            isServer = server;
            if (server)
            {
                pipeServer = new NamedPipeServerStream(PipeName);
                initIPCPipeServer();
            }
            else
            {
                pipeClient = new NamedPipeClientStream(PipeName);
                pipeClient.Connect();
            }
        }

        public event EventHandler<byte[]> OnReceivedData;

        public void Dispose()
        {
            pipeServer?.Close();
            pipeServer = null;
            pipeClient?.Close();
            pipeClient = null;
        }

        public bool SendData(byte[] data)
        {
            try
            {
                if(isServer == false)
                { 
                    if (pipeClient == null)
                        return false;
                    if (!pipeClient.IsConnected)
                        return false;

                        pipeClient.Write(data);
                }
                else
                {
                    pipeServer.Write(data);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void _ConnectionHandler(IAsyncResult result)
        {
            var srv = result.AsyncState as NamedPipeServerStream;
            srv.EndWaitForConnection(result);

            using(var stream = new MemoryStream())
            {
                srv.CopyTo(stream);
                OnReceivedData?.Invoke(null, stream.ToArray());
            }
            srv.Disconnect(); 
            initIPCPipeServer();
        }

        private void initIPCPipeServer() => pipeServer.BeginWaitForConnection(new AsyncCallback(_ConnectionHandler), pipeServer);
    }
}
