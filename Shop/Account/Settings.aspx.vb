Partial Class Settings
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not User.Identity.IsAuthenticated Then
            ' Not logged in, redirect to login
            Response.Redirect("~/shop/account/Login.aspx")
            Return
        End If
        If Not IsPostBack Then
            mvSettings.ActiveViewIndex = 0
        End If
    End Sub

    Private Sub ResetNavButtons()
        btnProfile.CssClass = "w-100 mb-2 btn btn-outline-secondary"
        btnOrders.CssClass = "w-100 mb-2 btn btn-outline-secondary"
        btnPassword.CssClass = "w-100 mb-2 btn btn-outline-secondary"
        btnShop.CssClass = "w-100 mb-2 btn btn-outline-secondary"
    End Sub

    ' Profile
    Protected Sub btnProfile_Click(sender As Object, e As EventArgs) Handles btnProfile.Click
        ResetNavButtons()
        btnProfile.CssClass = "w-100 mb-2 btn btn-primary" ' highlight
        mvSettings.ActiveViewIndex = 0
    End Sub

    Protected Sub btnOrders_Click(sender As Object, e As EventArgs)
        ResetNavButtons()
        btnOrders.CssClass = "w-100 mb-2 btn btn-primary"
        mvSettings.ActiveViewIndex = 1
    End Sub

    Protected Sub btnPassword_Click(sender As Object, e As EventArgs)
        ResetNavButtons()
        btnPassword.CssClass = "w-100 mb-2 btn btn-primary"
        mvSettings.ActiveViewIndex = 2
    End Sub
    ' Shop
    Protected Sub btnShop_Click(sender As Object, e As EventArgs)
        ResetNavButtons()
        btnShop.CssClass = "w-100 mb-2 btn btn-primary"
        mvSettings.ActiveViewIndex = 3

        ' Always load CreateShop.aspx for now
        iframeShop.Attributes("src") = "~/shop/storefront/CreateShop.aspx"
    End Sub



    ' Logout
    Protected Sub btnLogout_Click(sender As Object, e As EventArgs)
        FormsAuthentication.SignOut()
        Response.Redirect("~/default.aspx")
    End Sub
End Class
