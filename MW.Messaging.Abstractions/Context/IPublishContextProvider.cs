namespace MW.Messaging.Context;

/// <summary>
/// Creates a <see cref="Messaging.PublishContextModel"/> from the current execution flow.
/// <para>
/// Implementations may gather correlation, service identity, user/tenant, and tracing metadata
/// from ambient state (e.g., scoped services, async-local context) without depending on
/// HTTP-specific types such as <c>HttpContext</c>.
/// </para>
/// <para>
/// This abstraction is kept synchronous because the metadata sources it reads
/// (correlation context, service identity, scoped user/tenant values, trace context)
/// are in-memory and do not require async I/O.
/// </para>
/// </summary>
public interface IPublishContextProvider
{
    /// <summary>
    /// Creates a new publish context model populated from the current execution flow.
    /// A new instance is returned on each invocation.
    /// </summary>
    /// <returns>A new <see cref="Messaging.PublishContextModel"/> containing current metadata.</returns>
    Messaging.PublishContextModel Create();
}
