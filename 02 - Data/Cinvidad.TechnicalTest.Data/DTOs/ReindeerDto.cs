using System;
using System.Collections.Generic;
using System.Text;

namespace Convidad.TechnicalTest.Data.DTOs
{
    public record ReindeerDto
    (
        Guid Id,
        string Name,
        string PlateNumber,
        double Weight,
        int Packets
    );
}
