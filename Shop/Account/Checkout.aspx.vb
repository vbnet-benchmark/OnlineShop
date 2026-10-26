Imports System.Data.Entity

Public Class Checkout
    Inherits System.Web.UI.Page

    Private db As New OnlineShopContext()

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim userGuid As Guid = GetCurrentUserGuid()
            BindCheckoutSummary(userGuid)
        End If
    End Sub

    Private Function GetCurrentUserGuid() As Guid
        If Not User.Identity.IsAuthenticated Then
            Response.Redirect("Login.aspx")
            Return Guid.Empty
        End If

        Using db As New OnlineShopContext()
            Dim userModel = db.Users.FirstOrDefault(Function(u) u.Email = User.Identity.Name)
            If userModel Is Nothing Then
                Response.Redirect("Login.aspx")
                Return Guid.Empty
            End If
            Return userModel.UserID
        End Using
    End Function

    Private Function GetCartItemsWithProducts(userGuid As Guid) As List(Of CartItem)
        Return db.UserCartItems.Include("Product").Where(Function(c) c.UserID = userGuid).ToList()
    End Function

    Private Function CalculateSubtotal(cartItems As List(Of CartItem)) As Decimal
        Return cartItems.Sum(Function(c) c.Product.Price * c.Quantity)
    End Function

    Private Function ComputeBulkTierCodeFromQuantity(totalQty As Integer) As String
        If totalQty >= 20 Then
            Return "TIER3"
        ElseIf totalQty >= 10 Then
            Return "TIER2"
        ElseIf totalQty >= 5 Then
            Return "TIER1"
        Else
            Return "TIER0"
        End If
    End Function

    Private Sub BindCheckoutSummary(userGuid As Guid)
        Dim cartItems = GetCartItemsWithProducts(userGuid)
        Dim subtotal = CalculateSubtotal(cartItems)

        lblSubtotal.InnerText = "$" & subtotal.ToString("F2")
        lblTotal.InnerText = "$" & subtotal.ToString("F2")

        ' Pre-populate the hidden "client total" field with the correct, server-computed
        ' value, same as the real total shown above. A hidden field is still an ordinary
        ' posted form field -- nothing stops the client from editing it before the order is
        ' actually placed.
        hfClientTotal.Value = subtotal.ToString("F2")

        ' Pre-populate the free-shipping-eligible hidden field from the real subtotal.
        hfFreeShippingEligible.Value = (subtotal >= 50D).ToString()

        ' Pre-populate the membership-tier hidden field from the user's real, stored tier.
        Dim userModel = db.Users.FirstOrDefault(Function(u) u.UserID = userGuid)
        hfMembershipTier.Value = If(userModel IsNot Nothing, userModel.MembershipTier, "Standard")

        ' Pre-populate the bulk-discount tier code from the real cart quantity.
        Dim totalQty = cartItems.Sum(Function(c) c.Quantity)
        hfBulkTierCode.Value = ComputeBulkTierCodeFromQuantity(totalQty)
    End Sub

    ' ===================== Scenario 1 (direct): order total =====================

    Protected Sub btnPlaceOrder_Click(sender As Object, e As EventArgs) Handles btnPlaceOrder.Click
        Dim userGuid As Guid = GetCurrentUserGuid()
        Dim cartItems = GetCartItemsWithProducts(userGuid)
        If Not cartItems.Any() Then
            lblCheckoutMessage.InnerText = "Your cart is empty."
            Return
        End If

        ' VULNERABLE: the order total that gets charged is taken straight from the posted
        ' hidden field -- the same field the page itself populated with the correct amount
        ' at render time, but nothing stops the client from editing it before clicking
        ' "Place Order".
        Dim clientTotal As Decimal
        If Not Decimal.TryParse(hfClientTotal.Value, clientTotal) Then
            lblCheckoutMessage.InnerText = "Invalid order total."
            Return
        End If

        Dim order As New Order With {
            .OrderID = Guid.NewGuid(),
            .UserID = userGuid,
            .TotalAmount = clientTotal ' SINK: PLANTED-VB-HR-116
        }
        db.Orders.Add(order)

        For Each item In cartItems
            db.OrderItems.Add(New OrderItem With {
                .OrderID = order.OrderID,
                .ProductID = item.ProductID,
                .Quantity = item.Quantity,
                .UnitPrice = item.Product.Price
            })
        Next

        db.Payments.Add(New Payment With {
            .PaymentID = Guid.NewGuid(),
            .OrderID = order.OrderID,
            .Amount = clientTotal,
            .PaymentMethod = "CreditCard"
        })

        db.SaveChanges()
        lblCheckoutMessage.InnerText = $"Order placed! Charged ${clientTotal:F2}."
    End Sub

    Protected Sub btnPlaceOrderSafe_Click(sender As Object, e As EventArgs) Handles btnPlaceOrderSafe.Click
        Dim userGuid As Guid = GetCurrentUserGuid()
        Dim cartItems = GetCartItemsWithProducts(userGuid)
        If Not cartItems.Any() Then
            lblCheckoutMessage.InnerText = "Your cart is empty."
            Return
        End If

        ' SAFE: the order total is always recomputed server-side from each product's
        ' authoritative, database-stored price -- the posted hidden field is never
        ' consulted.
        Dim serverTotal As Decimal = CalculateSubtotal(cartItems)

        Dim order As New Order With {
            .OrderID = Guid.NewGuid(),
            .UserID = userGuid,
            .TotalAmount = serverTotal ' SAFE_SINK: PLANTED-VB-HR-116-safe
        }
        db.Orders.Add(order)

        For Each item In cartItems
            db.OrderItems.Add(New OrderItem With {
                .OrderID = order.OrderID,
                .ProductID = item.ProductID,
                .Quantity = item.Quantity,
                .UnitPrice = item.Product.Price
            })
        Next

        db.Payments.Add(New Payment With {
            .PaymentID = Guid.NewGuid(),
            .OrderID = order.OrderID,
            .Amount = serverTotal,
            .PaymentMethod = "CreditCard"
        })

        db.SaveChanges()
        lblCheckoutMessage.InnerText = $"Order placed! Charged ${serverTotal:F2}."
    End Sub

    ' ===================== Scenario 2 (indirect): promo discount percent =====================

    Private Function ApplyDiscountCode(subtotal As Decimal, discountPercentText As String) As Decimal
        Dim discountPercent As Decimal
        If Not Decimal.TryParse(discountPercentText, discountPercent) Then
            discountPercent = 0D
        End If
        ' VULNERABLE: discountPercent came straight from the client (hfPromoPercent, written
        ' client-side after a one-time AJAX promo-code validation, but re-submitted
        ' unchecked on the actual order postback) and is applied to real money with no
        ' re-validation against the promo code itself.
        Return subtotal - (subtotal * discountPercent / 100D) ' SINK: PLANTED-VB-HR-117
    End Function

    Private Shared ReadOnly ValidPromoCodes As New Dictionary(Of String, Decimal) From {
        {"SAVE10", 10D},
        {"SAVE20", 20D},
        {"WELCOME5", 5D}
    }

    Private Function ApplyDiscountCodeSafe(subtotal As Decimal, promoCode As String) As Decimal
        Dim discountPercent As Decimal = 0D
        If Not String.IsNullOrWhiteSpace(promoCode) Then
            ValidPromoCodes.TryGetValue(promoCode.Trim().ToUpperInvariant(), discountPercent)
        End If
        ' SAFE: the discount percentage is always looked up server-side from a fixed,
        ' trusted table keyed by the promo CODE -- the client only ever supplies the code,
        ' never the percentage itself, so there is nothing here for a client to tamper with.
        Return subtotal - (subtotal * discountPercent / 100D) ' SAFE_SINK: PLANTED-VB-HR-117-safe
    End Function

    Protected Sub btnApplyPromo_Click(sender As Object, e As EventArgs) Handles btnApplyPromo.Click
        Dim userGuid As Guid = GetCurrentUserGuid()
        Dim cartItems = GetCartItemsWithProducts(userGuid)
        Dim subtotal = CalculateSubtotal(cartItems)

        Dim finalTotal As Decimal
        If chkUsePromo.Checked Then
            finalTotal = ApplyDiscountCode(subtotal, hfPromoPercent.Value)
        Else
            finalTotal = subtotal
        End If

        lblCheckoutMessage.InnerText = $"Total after promo: ${finalTotal:F2}"
    End Sub

    Protected Sub btnApplyPromoSafe_Click(sender As Object, e As EventArgs) Handles btnApplyPromoSafe.Click
        Dim userGuid As Guid = GetCurrentUserGuid()
        Dim cartItems = GetCartItemsWithProducts(userGuid)
        Dim subtotal = CalculateSubtotal(cartItems)

        Dim finalTotal As Decimal
        If chkUsePromo.Checked Then
            finalTotal = ApplyDiscountCodeSafe(subtotal, txtPromoCode.Text)
        Else
            finalTotal = subtotal
        End If

        lblCheckoutMessage.InnerText = $"Total after promo: ${finalTotal:F2}"
    End Sub

    ' ===================== Scenario 3 (interprocedural): free-shipping flag =====================

    Protected Sub btnPlaceOrderWithShipping_Click(sender As Object, e As EventArgs) Handles btnPlaceOrderWithShipping.Click
        Dim userGuid As Guid = GetCurrentUserGuid()
        Dim cartItems = GetCartItemsWithProducts(userGuid)
        Dim subtotal = CalculateSubtotal(cartItems)

        Dim freeShippingRequested As Boolean
        Boolean.TryParse(hfFreeShippingEligible.Value, freeShippingRequested)

        Dim pricingService As New OrderPricingService()
        Dim shipping = pricingService.CalculateShippingCost(subtotal, cartItems, freeShippingRequested)
        Dim finalTotal = subtotal + shipping

        lblCheckoutMessage.InnerText = $"Total with shipping: ${finalTotal:F2} (shipping ${shipping:F2})"
    End Sub

    Protected Sub btnPlaceOrderWithShippingSafe_Click(sender As Object, e As EventArgs) Handles btnPlaceOrderWithShippingSafe.Click
        Dim userGuid As Guid = GetCurrentUserGuid()
        Dim cartItems = GetCartItemsWithProducts(userGuid)
        Dim subtotal = CalculateSubtotal(cartItems)

        Dim pricingService As New OrderPricingService()
        Dim shipping = pricingService.CalculateShippingCostSafe(subtotal, cartItems)
        Dim finalTotal = subtotal + shipping

        lblCheckoutMessage.InnerText = $"Total with shipping: ${finalTotal:F2} (shipping ${shipping:F2})"
    End Sub

    ' ===================== Scenario 4 (type/polymorphism-dependent): membership tier =====================

    Private Function ResolvePricingStrategy(membershipTier As String) As IPricingStrategy
        ' VULNERABLE: the concrete strategy is picked from a value the client controls
        ' (hfMembershipTier), never re-checked against the authenticated user's own stored
        ' membership tier.
        If String.Equals(membershipTier, "VIP", StringComparison.OrdinalIgnoreCase) Then
            Return New VipPricingStrategy() ' SINK: PLANTED-VB-HR-119
        End If
        Return New StandardPricingStrategy()
    End Function

    Private Function ResolvePricingStrategySafe(userGuid As Guid) As IPricingStrategy
        ' SAFE: the concrete strategy is picked from the authenticated user's own
        ' database-stored membership tier -- the client's hidden field is never consulted.
        Dim userModel = db.Users.FirstOrDefault(Function(u) u.UserID = userGuid)
        Dim realTier = If(userModel IsNot Nothing, userModel.MembershipTier, "Standard")
        If String.Equals(realTier, "VIP", StringComparison.OrdinalIgnoreCase) Then
            Return New VipPricingStrategy() ' SAFE_SINK: PLANTED-VB-HR-119-safe
        End If
        Return New StandardPricingStrategy()
    End Function

    Protected Sub btnPlaceOrderMember_Click(sender As Object, e As EventArgs) Handles btnPlaceOrderMember.Click
        Dim userGuid As Guid = GetCurrentUserGuid()
        Dim cartItems = GetCartItemsWithProducts(userGuid)
        Dim subtotal = CalculateSubtotal(cartItems)

        Dim strategy As IPricingStrategy = ResolvePricingStrategy(hfMembershipTier.Value)
        Dim finalTotal = strategy.ApplyMemberDiscount(subtotal)

        lblCheckoutMessage.InnerText = $"Member total: ${finalTotal:F2}"
    End Sub

    Protected Sub btnPlaceOrderMemberSafe_Click(sender As Object, e As EventArgs) Handles btnPlaceOrderMemberSafe.Click
        Dim userGuid As Guid = GetCurrentUserGuid()
        Dim cartItems = GetCartItemsWithProducts(userGuid)
        Dim subtotal = CalculateSubtotal(cartItems)

        Dim strategy As IPricingStrategy = ResolvePricingStrategySafe(userGuid)
        Dim finalTotal = strategy.ApplyMemberDiscount(subtotal)

        lblCheckoutMessage.InnerText = $"Member total: ${finalTotal:F2}"
    End Sub

    ' ===================== Scenario 5 (indirect, repeat tier / different construction shape): =====================
    ' ===================== bulk-quantity discount tier code =====================

    Private Function GetBulkDiscountMultiplier(tierCode As String) As Decimal
        ' VULNERABLE: tierCode came straight from the client (hfBulkTierCode, computed once
        ' from the real cart quantity at page load, but re-submitted unchecked on every
        ' postback) -- a multi-branch Select Case builds the multiplier across several
        ' statements rather than a single expression, but the untrusted value still drives
        ' every branch.
        Dim multiplier As Decimal
        Select Case tierCode
            Case "TIER3"
                multiplier = 0.75D
            Case "TIER2"
                multiplier = 0.85D
            Case "TIER1"
                multiplier = 0.95D
            Case Else
                multiplier = 1D
        End Select
        Return multiplier ' SINK: PLANTED-VB-HR-120
    End Function

    Private Function GetBulkDiscountMultiplierSafe(cartItems As List(Of CartItem)) As Decimal
        ' SAFE: the tier code is always re-derived server-side from the real,
        ' database-backed cart quantity -- the client's hidden field is never consulted.
        Dim totalQty = cartItems.Sum(Function(c) c.Quantity)
        Dim tierCode = ComputeBulkTierCodeFromQuantity(totalQty)

        Dim multiplier As Decimal
        Select Case tierCode
            Case "TIER3"
                multiplier = 0.75D
            Case "TIER2"
                multiplier = 0.85D
            Case "TIER1"
                multiplier = 0.95D
            Case Else
                multiplier = 1D
        End Select
        Return multiplier ' SAFE_SINK: PLANTED-VB-HR-120-safe
    End Function

    Protected Sub btnApplyBulkDiscount_Click(sender As Object, e As EventArgs) Handles btnApplyBulkDiscount.Click
        Dim userGuid As Guid = GetCurrentUserGuid()
        Dim cartItems = GetCartItemsWithProducts(userGuid)

        Dim multiplier = GetBulkDiscountMultiplier(hfBulkTierCode.Value)
        Dim finalTotal As Decimal = 0D
        For Each item In cartItems
            finalTotal += item.Product.Price * item.Quantity * multiplier
        Next

        lblCheckoutMessage.InnerText = $"Total with bulk discount: ${finalTotal:F2}"
    End Sub

    Protected Sub btnApplyBulkDiscountSafe_Click(sender As Object, e As EventArgs) Handles btnApplyBulkDiscountSafe.Click
        Dim userGuid As Guid = GetCurrentUserGuid()
        Dim cartItems = GetCartItemsWithProducts(userGuid)

        Dim multiplier = GetBulkDiscountMultiplierSafe(cartItems)
        Dim finalTotal As Decimal = 0D
        For Each item In cartItems
            finalTotal += item.Product.Price * item.Quantity * multiplier
        Next

        lblCheckoutMessage.InnerText = $"Total with bulk discount: ${finalTotal:F2}"
    End Sub

End Class
