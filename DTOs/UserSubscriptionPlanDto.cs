public record ActivateUserSubscriptionPlanDto(
    string userId,
    string subscriptionPlanId,
    SubscriptionBillingPeriod billingPeriod,
    bool? autoRenewal
);