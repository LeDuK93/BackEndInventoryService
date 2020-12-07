using BackEndInventoryService.Model;
using FluentValidation;
using System.Collections.Generic;
using System.Linq;

namespace BackEndInventoryService.Validator
{
    public class ReservationValidator : AbstractValidator<Reservation>
    {
        public ReservationValidator()
        {
            RuleFor(r => r.OrdersLines)
                .Must(AllProductsAreNotOrderedMoreThanOnce);
        }

        private bool AllProductsAreNotOrderedMoreThanOnce(List<OrderLine> orders)
        {
            return orders.GroupBy(o => o.ProductId).All(g => g.Count() <= 1);
        }
    }
}
