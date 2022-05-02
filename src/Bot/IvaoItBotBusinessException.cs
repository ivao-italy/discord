using System.Runtime.Serialization;

namespace Ivao.It.DiscordBot;

[Serializable]
public class IvaoItBotBusinessException : Exception
{
    public IvaoItBotBusinessException() { }
    public IvaoItBotBusinessException(string message) : base(message) { }
    public IvaoItBotBusinessException(string message, Exception inner) : base(message, inner) { }

    protected IvaoItBotBusinessException(
        SerializationInfo info,
        StreamingContext context) : base(info, context)
    {
    }
}
