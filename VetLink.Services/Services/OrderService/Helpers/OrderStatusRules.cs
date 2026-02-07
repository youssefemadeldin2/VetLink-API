using VetLink.Data.Enums;

namespace VetLink.Services.Services.OrderService.Helpers
{
    public static partial class OredrStatusRules
    {

        public static bool CanTransition(OredrStatus current, OredrStatus next, UserRole userRole)
        {
            if (current == OredrStatus.Paid && next == OredrStatus.Cancelled)
            {
                return userRole == UserRole.AdminOrSeller;
            }

            return userRole switch
            {
                UserRole.Buyer => CanBuyerTransition(current, next),
                UserRole.AdminOrSeller => CanAdminSellerTransition(current, next),
                _ => false
            };
        }

        private static bool CanBuyerTransition(OredrStatus current, OredrStatus next)
        {
            return current switch
            {
                OredrStatus.Draft => next == OredrStatus.Active,
                OredrStatus.Active => next == OredrStatus.PendingPayment || next == OredrStatus.Cancelled,
                OredrStatus.PendingPayment => next == OredrStatus.Cancelled,
                OredrStatus.Completed => next == OredrStatus.Returned,
                _ => false
            };
        }

        private static bool CanAdminSellerTransition(OredrStatus current, OredrStatus next)
        {
            return current switch
            {
                OredrStatus.Paid => next == OredrStatus.Processing || next == OredrStatus.Cancelled,
                OredrStatus.Processing => next == OredrStatus.PartiallyShipped || next == OredrStatus.Shipped,
                OredrStatus.PartiallyShipped => next == OredrStatus.Shipped,
                OredrStatus.Shipped => next == OredrStatus.Delivered,
                OredrStatus.Delivered => next == OredrStatus.Completed,
                _ => false
            };
        }


        public static bool IsTransitionForbidden(OredrStatus current, OredrStatus next)
        {
            return (current, next) switch
            {
                // forrpedden trans
                (OredrStatus.Paid, OredrStatus.Draft) => true,
                (OredrStatus.Paid, OredrStatus.Active) => true,
                (OredrStatus.Shipped, OredrStatus.Processing) => true,
                (OredrStatus.Completed, _) => true,
                (OredrStatus.Cancelled, _) => true,
                (OredrStatus.Returned, _) => true,
                _ => false
            };
        }
    }
}