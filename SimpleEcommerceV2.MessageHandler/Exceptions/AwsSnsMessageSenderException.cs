﻿using System.Runtime.Serialization;

namespace SimpleEcommerceV2.MessageHandler.Exceptions;

[Serializable]
public class AwsSnsMessageSenderException : Exception
{
    public AwsSnsMessageSenderException()
    {
    }

    public AwsSnsMessageSenderException(string message) : base(message)
    {
    }

    public AwsSnsMessageSenderException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected AwsSnsMessageSenderException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
