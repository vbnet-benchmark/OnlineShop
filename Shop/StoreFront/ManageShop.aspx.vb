Imports System

Partial Class ManageShop
    Inherits System.Web.UI.Page

    Public Property shopID As Guid
    Private currentShop As Shop

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not User.Identity.IsAuthenticated Then
            Response.Redirect("~/shop/account/Login.aspx")
        End If




        If Not IsPostBack Then
            ' Get ShopID from query string
            If Not Guid.TryParse(Request.QueryString("shopid"), shopID) Then
                Response.Redirect("~/shop/StoreFront/CreateShop.aspx")
            Else
                'StoreProducts.ShopID = shopID 'Set shopID from query string or session
                iframeProducts.Attributes("src") = "Products/StoreProducts.aspx?shopid=" & shopID.ToString

            End If


            LoadShop()
            LoadShopSettings()

            ClearActiveLinks()
            lnkDashboard.CssClass = "sidebar-link active"
            mvShop.SetActiveView(viewDashboard)
        End If


    End Sub

    Private Sub LoadShopSettings()
        Dim userEmail As String = User.Identity.Name
        Using db As New OnlineShopContext()
            Dim user = db.Users.FirstOrDefault(Function(u) u.Email = userEmail)
            If user Is Nothing Then Return

            Dim shop = db.Shops.FirstOrDefault(Function(s) s.UserID = user.UserID)
            If shop Is Nothing Then Return

            txtEditShopName.Text = shop.ShopName
            txtEditDescription.Text = shop.Description
            txtThemeColor.Text = If(String.IsNullOrEmpty(shop.ThemeColor), "#ffffff", shop.ThemeColor)
            chkIsActive.Checked = shop.IsActive

            If Not String.IsNullOrEmpty(shop.LogoBase64) AndAlso Not String.IsNullOrEmpty(shop.LogoMimeType) Then
                imgLogoPreview.ImageUrl = $"data:{shop.LogoMimeType};base64,{shop.LogoBase64}"
            End If

            If Not String.IsNullOrEmpty(shop.BannerBase64) AndAlso Not String.IsNullOrEmpty(shop.BannerMimeType) Then
                imgBannerPreview.ImageUrl = $"data:{shop.BannerMimeType};base64,{shop.BannerBase64}"
            End If

        End Using
    End Sub

    Private Sub LoadShop()
        Using db As New OnlineShopContext()
            ' Get the current logged-in user
            Dim currentUser = db.Users.FirstOrDefault(Function(u) u.Email = User.Identity.Name)
            If currentUser Is Nothing Then
                Response.Redirect("~/shop/account/Login.aspx")
                Return
            End If

            ' Now get the shop for this user
            currentShop = db.Shops.FirstOrDefault(Function(s) s.ShopID = shopID AndAlso s.UserID = currentUser.UserID)
            If currentShop Is Nothing Then
                Response.Redirect("~/shop/account/CreateShop.aspx")
                Return
            End If

            ' Populate dashboard
            lblShopName.Text = currentShop.ShopName
            lblShopDescription.Text = currentShop.Description
            lblCreatedAt.Text = currentShop.CreatedAt.ToString("yyyy-MM-dd")

            ' Populate settings fields
            txtEditShopName.Text = currentShop.ShopName
            txtEditDescription.Text = currentShop.Description
        End Using
    End Sub


    ' Sidebar navigation
    Private Sub ClearActiveLinks()
        lnkDashboard.CssClass = "sidebar-link"
        lnkProducts.CssClass = "sidebar-link"
        lnkOrders.CssClass = "sidebar-link"
        lnkSettings.CssClass = "sidebar-link"
    End Sub

    Protected Sub lnkDashboard_Click(sender As Object, e As EventArgs)
        ClearActiveLinks()
        lnkDashboard.CssClass = "sidebar-link active"
        mvShop.SetActiveView(viewDashboard)
    End Sub

    Protected Sub lnkProducts_Click(sender As Object, e As EventArgs)
        ClearActiveLinks()
        lnkProducts.CssClass = "sidebar-link active"
        mvShop.SetActiveView(viewProducts)
    End Sub

    Protected Sub lnkOrders_Click(sender As Object, e As EventArgs)
        ClearActiveLinks()
        lnkOrders.CssClass = "sidebar-link active"
        mvShop.SetActiveView(viewOrders)
    End Sub

    Protected Sub lnkSettings_Click(sender As Object, e As EventArgs)
        ClearActiveLinks()
        lnkSettings.CssClass = "sidebar-link active"
        mvShop.SetActiveView(viewSettings)
    End Sub

    Private Function GetCurrentUser() As User
        If Not User.Identity.IsAuthenticated Then
            Return Nothing
        End If

        Dim email As String = User.Identity.Name
        Using db As New OnlineShopContext()
            Return db.Users.FirstOrDefault(Function(u) u.Email = email)
        End Using
    End Function

    Protected Sub btnSaveSettings_Click(sender As Object, e As EventArgs) Handles btnSaveSettings.Click
        Dim user = GetCurrentUser()
        If user Is Nothing Then
            Return
        End If

        Using db As New OnlineShopContext()
            Dim shop = db.Shops.FirstOrDefault(Function(s) s.UserID = user.UserID)
            If shop Is Nothing Then
                Return
            End If

            ' Validate and save theme color
            Dim selectedColor As String = txtThemeColor.Text.Trim()
            If Not System.Text.RegularExpressions.Regex.IsMatch(selectedColor, "^#([0-9A-Fa-f]{6})$") Then
                Return
            End If
            shop.ThemeColor = selectedColor

            ' Save name and description
            shop.ShopName = txtEditShopName.Text.Trim()
            shop.Description = txtEditDescription.Text.Trim()

            ' Save Logo as Base64
            If fuLogo.HasFile Then
                Dim contentType = fuLogo.PostedFile.ContentType ' e.g., "image/png" or "image/jpeg"
                Using ms As New IO.MemoryStream()
                    fuLogo.PostedFile.InputStream.CopyTo(ms)
                    shop.LogoBase64 = Convert.ToBase64String(ms.ToArray())
                    shop.LogoMimeType = contentType
                End Using
            End If

            If fuBanner.HasFile Then
                Dim contentType = fuBanner.PostedFile.ContentType
                Using ms As New IO.MemoryStream()
                    fuBanner.PostedFile.InputStream.CopyTo(ms)
                    shop.BannerBase64 = Convert.ToBase64String(ms.ToArray())
                    shop.BannerMimeType = contentType
                End Using
            End If


            ' Update IsActive based on toggle
            shop.IsActive = chkIsActive.Checked

            db.SaveChanges()
            LoadShopSettings()
        End Using
    End Sub




End Class
