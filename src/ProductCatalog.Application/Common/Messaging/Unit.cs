namespace ProductCatalog.Application.Common.Messaging;

/// <summary>
/// Minimal Unit type (equivalent to MediatR.Unit)
/// </summary>
public readonly struct Unit
{
    public static readonly Unit Value = new Unit();
}