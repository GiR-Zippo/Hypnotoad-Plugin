using H.Formatters;
using H.Pipes;
using System.ComponentModel.Design;

namespace HypnotoadPlugin
{
    internal static class Pipe
    {
        internal static PipeClient<Message> Client { get; private set; }

        internal static void Initialize()
        {
            Client = new PipeClient<Message>("Hypnotoad", formatter: new NewtonsoftJsonFormatter());
        }

        internal static void Dispose()
        {

        }
    }
}
