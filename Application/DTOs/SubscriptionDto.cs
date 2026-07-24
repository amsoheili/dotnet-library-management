public record PurchaseSubscriptionPlanDto(
    SubscriptionBillingPeriod billingPeriod,
    bool? autoRenewal
);