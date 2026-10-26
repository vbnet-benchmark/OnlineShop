''' <summary>
''' Order-level pricing calculations shared by the checkout flow. Kept as a separate
''' service class (mirroring the project's own Context/ split) so checkout page code
''' doesn't have to duplicate shipping-cost logic.
''' </summary>
Public Class OrderPricingService

    ''' <summary>
    ''' Computes the shipping cost for an order.
    ''' VULNERABLE: trusts the caller-supplied freeShippingRequested flag outright, with
    ''' no re-check of real eligibility (e.g. a minimum subtotal) against the order's own
    ''' data -- the caller is expected to have decided this honestly, but nothing here
    ''' verifies that.
    ''' </summary>
    Public Function CalculateShippingCost(subtotal As Decimal, cartItems As List(Of CartItem), freeShippingRequested As Boolean) As Decimal
        If freeShippingRequested Then ' SINK: PLANTED-VB-HR-118
            Return 0D
        End If

        ' Same placeholder shipping logic as Cart.aspx's own CalculateShipping: $5 per
        ' distinct shop represented in the cart.
        Dim shops = cartItems.Select(Function(c) c.Product.ShopID).Distinct().Count()
        Return shops * 5D
    End Function

    ''' <summary>
    ''' SAFE variant: free-shipping eligibility is always derived from the order's own
    ''' subtotal, server-side -- there is no caller-supplied flag to tamper with at all.
    ''' </summary>
    Public Function CalculateShippingCostSafe(subtotal As Decimal, cartItems As List(Of CartItem)) As Decimal
        Const FreeShippingThreshold As Decimal = 50D
        If subtotal >= FreeShippingThreshold Then ' SAFE_SINK: PLANTED-VB-HR-118-safe
            Return 0D
        End If

        Dim shops = cartItems.Select(Function(c) c.Product.ShopID).Distinct().Count()
        Return shops * 5D
    End Function

End Class
