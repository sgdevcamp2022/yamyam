using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters;
using System.Text;

namespace StompHelper
{
    public class StompMessageParser
    {
        public string Serialize(StompMessage message)
        {
            var buffer = new StringBuilder();

            buffer.Append(message.Command + "\n");

            if (message.Headers != null)
            {
                foreach (var header in message.Headers)
                {
                    buffer.Append(header.Key)
                        .Append(":")
                        .Append(header.Value)
                        .Append("\n");
                }
            }

            buffer.Append("\n");
            buffer.Append(message.Body);
            buffer.Append('\0');

            return buffer.ToString();
        }

        public StompMessage Deserialize(string message)
        {
            var reader = new StringReader(message);
            var command = reader.ReadLine();
            var headers = new Dictionary<string, string>();
            var header = reader.ReadLine();

            // If Don't Work Try using IsNullOrWhiteSpace
            while (!string.IsNullOrEmpty(header))
            {
                
                var split = header.Split(':');
                if (split.Length == 2) headers.Add(split[0].Trim(), split[1].Trim());
                header = reader.ReadLine() ?? string.Empty;
            }

            var body = reader.ReadToEnd() ?? string.Empty;
            body = body.TrimEnd('\r', '\n', '\0');
            return new StompMessage(EnumParser<StompCommand>.toEnum(command), body, headers);
        }
    }
}