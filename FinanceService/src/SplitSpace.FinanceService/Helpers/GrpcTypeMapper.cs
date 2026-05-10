using Google.Protobuf.WellKnownTypes;
using Google.Type;
using Grpc.Core;
using DateTime = System.DateTime;

namespace SplitSpace.FinanceService.Helpers;

public static class GrpcTypeMapper
{
    public static Guid ToGuidOrThrow(this string value, string fieldName)
    {
        if (Guid.TryParse(value, out var guid) is false)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, $"Invalid UUID field {fieldName}"));
        }
        
        return guid;
    }

    public static Guid? ToNullableGuidOrThrow(this string? value, string fieldName)
    {
        if (value is null)
        {
            return null;
        }

        if (Guid.TryParse(value, out var guid) is false)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, $"Invalid UUID field {fieldName}"));
        }
        
        return guid;
    }
    
    public static decimal ToDecimalOrThrow(this Money value, string fieldName)
    {
        try
        {
            return value.DecimalValue;
        }
        catch 
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, $"Invalid Money field {fieldName}"));
        }
    }
    
    public static decimal? ToNullableDecimalOrThrow(this Money? value, string fieldName)
    {
        if (value is null)
        {
            return null;
        }

        try
        {
            return value.DecimalValue;
        }
        catch 
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, $"Invalid Money field {fieldName}"));
        }
    }
    
    public static Money ToMoney(this decimal value)
    {
        return new Money { DecimalValue = value };
    }
    
    public static Money? ToNullableMoney(this decimal? value)
    {
        if (value is null)
        {
            return null;
        }

        return new Money { DecimalValue = value.Value };
    }

    public static DateTimeOffset ToDateTimeOffsetOrThrow(this Date date, string? timeZone, string fieldName)
    {
        if (date.Year <= 0)
        {
            throw new RpcException(
                new Status(StatusCode.InvalidArgument, $"Date year must be greater than zero {fieldName}"));
        }
        if (date.Month <= 0)
        {
            throw new RpcException(
                new Status(StatusCode.InvalidArgument, $"Date month must be greater than zero {fieldName}"));
        }
        if (date.Day <= 0)
        {
            throw new RpcException(
                new Status(StatusCode.InvalidArgument, $"Date day must be greater than zero {fieldName}"));
        }

        if (timeZone is null)
        {
            return date.ToDateTimeOffset();
        }

        if (TimeZoneInfo.TryFindSystemTimeZoneById(timeZone, out var timeZoneId) is false)
        {
            throw new RpcException(
                new Status(StatusCode.InvalidArgument, $"Timezone must be in IANA format {fieldName}"));
        }
        
        var localDateTime = new DateTime(
            date.Year,
            date.Month,
            date.Day,
            0, 0, 0,
            DateTimeKind.Unspecified
        );

        var offset = timeZoneId.GetUtcOffset(localDateTime);

        return new DateTimeOffset(localDateTime, offset);
    }

    public static DateTimeOffset ToDateTimeOffsetOrThrow(this Timestamp timestamp, string fieldName)
    {
        if (timestamp.Nanos < 0)
        {
            throw new RpcException(
                new Status(StatusCode.InvalidArgument, $"Timestamp nanos must be greater than or equal zero {fieldName}"));
        }
        if (timestamp.Seconds <= 0)
        {
            throw new RpcException(
                new Status(StatusCode.InvalidArgument, $"Timestamp seconds must be greater than zero {fieldName}"));
        }
        
        return timestamp.ToDateTimeOffset();
    }
}
