using System;
using System.Collections.Generic;
using System.Text;

namespace Convidad.TechnicalTest.Models.DTOs;
public record AssignReindeerToRouteRequest(
    Guid ReindeerId,
    int MaxDeliveries = 10
);
