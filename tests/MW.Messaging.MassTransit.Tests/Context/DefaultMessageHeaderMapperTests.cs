using FluentAssertions;
using MW.Messaging.Headers;
using MW.Messaging.MassTransit.Context;
using MW.Messaging.Messaging;

namespace MW.Messaging.MassTransit.Tests.Context;

public class DefaultMessageHeaderMapperTests
{
    private readonly DefaultMessageHeaderMapper _mapper = new();

    [Fact]
    public void MapToHeaders_Should_MapAllFields()
    {
        var tenantId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var context = new PublishContextModel
        {
            CorrelationId = "corr-123",
            CausationId = "cause-456",
            TenantId = tenantId,
            UserId = userId,
            SourceService = "test-service",
            TraceId = "trace-789"
        };

        var headers = _mapper.MapToHeaders(context);

        headers[MessageHeaders.CorrelationId].Should().Be("corr-123");
        headers[MessageHeaders.CausationId].Should().Be("cause-456");
        headers[MessageHeaders.TenantId].Should().Be(tenantId.ToString());
        headers[MessageHeaders.UserId].Should().Be(userId.ToString());
        headers[MessageHeaders.SourceService].Should().Be("test-service");
        headers[MessageHeaders.TraceId].Should().Be("trace-789");
    }

    [Fact]
    public void MapToHeaders_Should_SkipNullAndEmptyFields()
    {
        var context = new PublishContextModel
        {
            CorrelationId = null,
            CausationId = "",
            SourceService = "test-service"
        };

        var headers = _mapper.MapToHeaders(context);

        headers.Should().ContainKey(MessageHeaders.SourceService);
        headers.Should().NotContainKey(MessageHeaders.CorrelationId);
        headers.Should().NotContainKey(MessageHeaders.CausationId);
        headers.Should().NotContainKey(MessageHeaders.TenantId);
        headers.Should().NotContainKey(MessageHeaders.UserId);
        headers.Should().NotContainKey(MessageHeaders.TraceId);
    }

    [Fact]
    public void MapToHeaders_Should_ThrowOnNull()
    {
        var act = () => _mapper.MapToHeaders(null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void MapFromHeaders_Should_MapAllFields()
    {
        var tenantId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var headers = new Dictionary<string, object>
        {
            [MessageHeaders.CorrelationId] = "corr-123",
            [MessageHeaders.CausationId] = "cause-456",
            [MessageHeaders.TenantId] = tenantId.ToString(),
            [MessageHeaders.UserId] = userId.ToString(),
            [MessageHeaders.SourceService] = "test-service",
            [MessageHeaders.TraceId] = "trace-789",
            [MessageHeaders.EventName] = "OrderCreated"
        };

        var context = _mapper.MapFromHeaders(headers);

        context.CorrelationId.Should().Be("corr-123");
        context.CausationId.Should().Be("cause-456");
        context.TenantId.Should().Be(tenantId);
        context.UserId.Should().Be(userId);
        context.SourceService.Should().Be("test-service");
        context.TraceId.Should().Be("trace-789");
        context.EventName.Should().Be("OrderCreated");
    }

    [Fact]
    public void MapFromHeaders_Should_HandleEmptyHeaders()
    {
        var headers = new Dictionary<string, object>();

        var context = _mapper.MapFromHeaders(headers);

        context.CorrelationId.Should().BeNull();
        context.CausationId.Should().BeNull();
        context.TenantId.Should().BeNull();
        context.UserId.Should().BeNull();
        context.SourceService.Should().BeNull();
        context.TraceId.Should().BeNull();
        context.EventName.Should().BeNull();
    }

    [Fact]
    public void MapFromHeaders_Should_HandleInvalidGuidGracefully()
    {
        var headers = new Dictionary<string, object>
        {
            [MessageHeaders.TenantId] = "not-a-guid",
            [MessageHeaders.UserId] = "also-not-a-guid"
        };

        var context = _mapper.MapFromHeaders(headers);

        context.TenantId.Should().BeNull();
        context.UserId.Should().BeNull();
    }

    [Fact]
    public void MapFromHeaders_Should_ThrowOnNull()
    {
        var act = () => _mapper.MapFromHeaders(null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void MapToHeaders_And_MapFromHeaders_Should_Roundtrip()
    {
        var tenantId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var publishContext = new PublishContextModel
        {
            CorrelationId = "corr-roundtrip",
            CausationId = "cause-roundtrip",
            TenantId = tenantId,
            UserId = userId,
            SourceService = "roundtrip-service",
            TraceId = "trace-roundtrip"
        };

        var headers = _mapper.MapToHeaders(publishContext);
        var consumerContext = _mapper.MapFromHeaders(headers);

        consumerContext.CorrelationId.Should().Be(publishContext.CorrelationId);
        consumerContext.CausationId.Should().Be(publishContext.CausationId);
        consumerContext.TenantId.Should().Be(publishContext.TenantId);
        consumerContext.UserId.Should().Be(publishContext.UserId);
        consumerContext.SourceService.Should().Be(publishContext.SourceService);
        consumerContext.TraceId.Should().Be(publishContext.TraceId);
    }
}
