using System;
using System.Collections.Generic;
using System.Text;

namespace Convidad.TechnicalTest.Data.DTOs
{
    public record WishDto
    (
        Guid Id,
        string Category,
        int Priority
    );
}
