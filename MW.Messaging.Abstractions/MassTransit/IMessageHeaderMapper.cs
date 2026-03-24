namespace MW.Messaging.MassTransit;

/// <summary>
/// Abstraction responsible for mapping shared metadata models into MassTransit headers
/// and extracting them back when consuming. This keeps shared metadata handling centralized
/// so that header mapping is not repeated in each service.
/// </summary>
public interface IMessageHeaderMapper
{
    /// <summary>
    /// Maps the properties of a <see cref="MW.Messaging.Messaging.PublishContextModel"/>
    /// into a dictionary of message headers suitable for MassTransit publish/send operations.
    /// </summary>
    /// <param name="context">The publish context model containing metadata to map.</param>
    /// <returns>A dictionary of header name-value pairs.</returns>
    IDictionary<string, object> MapToHeaders(Messaging.PublishContextModel context);

    /// <summary>
    /// Extracts message headers from a consumed message and maps them into a
    /// <see cref="MW.Messaging.Messaging.ConsumerContextModel"/>.
    /// </summary>
    /// <param name="headers">The message headers from the consumed message.</param>
    /// <returns>A populated <see cref="MW.Messaging.Messaging.ConsumerContextModel"/>.</returns>
    Messaging.ConsumerContextModel MapFromHeaders(IDictionary<string, object> headers);
}
