Imports System.Data.Entity

Public Class Cart
    Inherits System.Web.UI.Page

    Private db As New OnlineShopContext()

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim userGuid As Guid = GetCurrentUserGuid()
            BindCart(userGuid, db)
        End If
    End Sub

    Private Sub BindCart(userGuid As Guid, db As OnlineShopContext)
        ' Load all cart items for this user including product images
        Dim cartItems = db.UserCartItems.Include("Product.ProductImages") _
                        .Where(Function(c) c.UserID = userGuid) _
                        .OrderBy(Function(c) c.DateAdded) _
                        .ToList()

        ' Prepare data for Repeater
        Dim cartData = cartItems.Select(Function(c) New With {
                                        c.CartItemID,
                                        c.Quantity,
                                        .ProductName = c.Product.ProductName,
                                        .Price = c.Product.Price * c.Quantity,
                                        .ImageUrls = c.Product.ProductImages.Select(Function(img) "data:image/png;base64," & img.ImageData).ToList()
                                    }).ToList()

        rptCartItems.DataSource = cartData
        rptCartItems.DataBind()

        ' Show/hide empty panel
        pnlCartEmpty.Visible = Not cartItems.Any()
        btnCheckout.Visible = cartItems.Any()

        ' Calculate totals
        Dim subtotal As Decimal = cartItems.Sum(Function(c) c.Product.Price * c.Quantity)
        Dim shipping As Decimal = CalculateShipping(cartItems) ' placeholder function
        Dim tax As Decimal = CalculateTax(subtotal) ' placeholder function
        Dim total As Decimal = subtotal + shipping + tax

        ' Update labels in right-hand summary panel
        lblSubtotal.InnerText = "$" & subtotal.ToString("F2")
        lblShipping.InnerText = "$" & shipping.ToString("F2")
        lblTax.InnerText = "$" & tax.ToString("F2")
        lblTotal.InnerText = "$" & total.ToString("F2")
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

    Protected Sub rptCartItems_ItemCommand(source As Object, e As RepeaterCommandEventArgs) Handles rptCartItems.ItemCommand
        If e.CommandName = "Remove" Then
            Dim cartId As Integer = Convert.ToInt32(e.CommandArgument)
            Dim userGuid As Guid = GetCurrentUserGuid()

            Dim item = db.UserCartItems.FirstOrDefault(Function(c) c.CartItemID = cartId AndAlso c.UserID = userGuid)
            If item IsNot Nothing Then
                db.UserCartItems.Remove(item)
                db.SaveChanges()
            End If

            BindCart(userGuid, db)
            Dim master As Site = CType(Me.Master, Site)
            master.UpdateCartCount()
        End If
    End Sub

    Protected Sub btnCheckout_Click(sender As Object, e As EventArgs) Handles btnCheckout.Click
        Response.Redirect("Checkout.aspx")
    End Sub

    ' Placeholder function for shipping calculation
    ' Placeholder function for shipping calculation
    Private Function CalculateShipping(cartItems As List(Of CartItem)) As Decimal
        ' Example: $5 per shop
        Dim shops = cartItems.Select(Function(c) c.Product.ShopID).Distinct().Count()
        Return shops * 5D
    End Function


    ' Placeholder function for tax calculation
    Private Function CalculateTax(subtotal As Decimal) As Decimal
        ' Example: 8% tax
        Return subtotal * 0.08D
    End Function
End Class
