using System;
using System.Collections.Generic;

namespace StompHelper
{
    public class StompMessage
    {
        public StompCommand Command { get; private set; }
        public string Body { get; private set; }
        public Dictionary<string, string> Headers {get { return _headers; }}

        private readonly Dictionary<string, string> _headers = new Dictionary<string, string>();

        public StompMessage(StompCommand command): this(command, string.Empty)
        {
        }

        public StompMessage(StompCommand command, string body)
            : this(command, body, new Dictionary<string, string>())
        {
        }

        internal StompMessage(StompCommand command, string body, Dictionary<string, string> headers)
        {

            Command = command;
            Body = body;
            _headers = headers;

            this["content-length"] = body.Length.ToString();
        }

        public string this[string header]
        {
            get { return _headers.ContainsKey(header) ? _headers[header] : string.Empty; }
            set { _headers[header] = value; }
        }
    }
}