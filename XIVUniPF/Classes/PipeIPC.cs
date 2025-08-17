using System.IO;
using System.IO.Pipes;
using System.Text;

namespace XIVUniPF.Classes
{
    public class PipeIPC : IDisposable
    {
        public delegate void MessageReceivedEventHandler(string message);
        public event MessageReceivedEventHandler? MessageReceived;

        private readonly string _pipeName;
        private CancellationTokenSource? _cts;
        private Task? _serverTask;

        public PipeIPC(string pipeName)
        {
            _pipeName = pipeName;
        }

        public void StartServer()
        {
            _cts = new CancellationTokenSource();
            _serverTask = Task.Run(() => ServerLoop(_cts.Token));
        }

        private async Task ServerLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                using var server = new NamedPipeServerStream(
                    _pipeName, 
                    PipeDirection.InOut, 
                    1,
                    PipeTransmissionMode.Message, 
                    PipeOptions.Asynchronous
                );
                try
                {
                    await server.WaitForConnectionAsync(token);
                    using var reader = new StreamReader(server, Encoding.UTF8);
                    string? line;
                    while ((line = await reader.ReadLineAsync()) != null)
                        MessageReceived?.Invoke(line);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch { }
            }
        }

        public async Task SendMessageAsync(string message)
        {
            try
            {
                using var client = new NamedPipeClientStream(".", _pipeName, PipeDirection.Out);
                await client.ConnectAsync(1000);
                using var writer = new StreamWriter(client, Encoding.UTF8) { AutoFlush = true };
                await writer.WriteLineAsync(message);
            }
            catch { }
        }

        ~PipeIPC()
        {
            _cts?.Cancel();
            try { _serverTask?.Wait(1000); } catch { }
            _cts?.Dispose();
        }

        public void Dispose()
        {
            _cts?.Cancel();
            try { _serverTask?.Wait(1000); } catch { }
            _cts?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
