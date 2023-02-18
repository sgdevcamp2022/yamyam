using System;
namespace StompHelper
{
    public enum StompCommand
    {
        //Client Command
        CONNECT,
        DISCONNECT,
        SUBSCRIBE,
        UNSUBSCRIBE,
        SEND,

        //Server Response
        CONNECTED,
        MESSAGE,
        ERROR
    }
}