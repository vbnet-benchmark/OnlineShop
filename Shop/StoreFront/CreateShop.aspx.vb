Imports System
Imports System.Linq

Partial Class CreateShop
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not User.Identity.IsAuthenticated Then
            ' Not logged in, redirect to login
            Response.Redirect("~/shop/account/Login.aspx")
            Return
        End If

        If Not IsPostBack Then
            Using db As New OnlineShopContext()
                Dim userEmail As String = User.Identity.Name
                Dim currentUser = db.Users.FirstOrDefault(Function(u) u.Email = userEmail)

                If currentUser IsNot Nothing Then
                    ' Check if user already has a shop
                    Dim existingShop = db.Shops.FirstOrDefault(Function(s) s.UserID = currentUser.UserID)
                    If existingShop IsNot Nothing Then
                        ' Redirect to ManageShop if shop exists
                        Response.Redirect("~/shop/storefront/ManageShop.aspx?ShopID=" & existingShop.ShopID.ToString())
                    End If
                End If
            End Using
        End If
    End Sub


    Protected Sub btnCreateShop_Click(sender As Object, e As EventArgs) Handles btnCreateShop.Click
        ' Ensure the user is logged in
        If Not User.Identity.IsAuthenticated Then
            Response.Redirect("~/shop/account/Login.aspx")
            Return
        End If

        Using db As New OnlineShopContext()
            ' Get email from the authentication cookie
            Dim userEmail As String = User.Identity.Name
            Dim currentUser = db.Users.FirstOrDefault(Function(u) u.Email = userEmail)

            If currentUser Is Nothing Then
                lblMessage.Text = "User not found. Please log in again."
                Return
            End If

            ' Check if the user already has a shop
            Dim existingShop = db.Shops.FirstOrDefault(Function(s) s.UserID = currentUser.UserID)
            If existingShop IsNot Nothing Then
                Response.Redirect("~/shop/storefront/ManageShop.aspx?ShopID=" & existingShop.ShopID.ToString())
                Return
            End If

            ' Create new shop
            Dim newShop As New Shop With {
                .ShopID = Guid.NewGuid(),
                .UserID = currentUser.UserID,
                .ShopName = txtShopName.Text.Trim(),
                .Description = txtDescription.Text.Trim(),
                .CreatedAt = DateTime.Now,
                .IsActive = False
            }

            db.Shops.Add(newShop)
            db.SaveChanges()

            ' Redirect to manage shop for the newly created shop
            Response.Redirect("~/shop/storefront/ManageShop.aspx?ShopID=" & newShop.ShopID.ToString())
        End Using
    End Sub

End Class
