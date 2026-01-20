namespace Convidad.TechnicalTest.Models.DTOs
{ 
    public record DeliveryDto(
        Guid Id,
        Guid ChildId,
        Guid RouteId,
        string Status,
       DateTimeOffset CreatedAt
    );
}
