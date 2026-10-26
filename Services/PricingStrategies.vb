''' <summary>
''' Per-membership-tier pricing behavior. Checkout.aspx.vb resolves one of these two
''' concrete implementations at runtime and calls it polymorphically through the
''' IPricingStrategy interface -- see ResolvePricingStrategy / ResolvePricingStrategySafe
''' there for which value decides which concrete type gets instantiated.
''' </summary>
Public Interface IPricingStrategy
    Function ApplyMemberDiscount(subtotal As Decimal) As Decimal
End Interface

Public Class StandardPricingStrategy
    Implements IPricingStrategy

    Public Function ApplyMemberDiscount(subtotal As Decimal) As Decimal Implements IPricingStrategy.ApplyMemberDiscount
        Return subtotal
    End Function
End Class

Public Class VipPricingStrategy
    Implements IPricingStrategy

    Private Const VipDiscountRate As Decimal = 0.15D

    Public Function ApplyMemberDiscount(subtotal As Decimal) As Decimal Implements IPricingStrategy.ApplyMemberDiscount
        Return subtotal * (1D - VipDiscountRate)
    End Function
End Class
