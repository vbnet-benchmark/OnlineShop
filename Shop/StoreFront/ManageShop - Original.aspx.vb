Imports System

Partial Class ManageShop
    Inherits System.Web.UI.Page

    Private shopID As Guid
    Private currentShop As Shop

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not User.Identity.IsAuthenticated Then
            Response.Redirect("~/shop/account/Login.aspx")
        End If




        If Not IsPostBack Then
            ' Get ShopID from query string
            If Not Guid.TryParse(Request.QueryString("shopid"), shopID) Then
                Response.Redirect("~/shop/StoreFront/CreateShop.aspx")
            End If


            LoadShop()
            LoadShopSettings()
            LoadProducts()

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

    '''Products part
    Protected Sub btnAddProduct_Click(sender As Object, e As EventArgs) Handles btnAddProduct.Click
        ' Clear previous errors
        lblNameError.Text = ""
        lblPriceError.Text = ""
        lblStockError.Text = ""
        lblThresholdError.Text = ""
        lblCategoryError.Text = ""
        lblImageError.Text = ""

        Dim hasError As Boolean = False

        ' --- PRODUCT NAME ---
        If String.IsNullOrWhiteSpace(txtProductName.Text) Then
            lblNameError.Text = "Product name is required."
            txtProductName.Style("border") = "1px solid red"
            hasError = True
        Else
            txtProductName.Style("border") = "1px solid #ccc"
        End If

        ' --- PRICE ---
        Dim priceDecimal As Decimal
        If String.IsNullOrWhiteSpace(txtPrice.Text) OrElse Not Decimal.TryParse(txtPrice.Text, priceDecimal) Then
            lblPriceError.Text = "Enter a valid price."
            txtPrice.Style("border") = "1px solid red"
            hasError = True
        Else
            lblPriceError.Text = ""
            txtPrice.Style("border") = "1px solid #ccc"
        End If

        ' Stock
        Dim stockQty As Integer = 0
        If Not String.IsNullOrWhiteSpace(txtStock.Text) OrElse txtStock.Text = "" Then
            If Not Integer.TryParse(txtStock.Text, stockQty) Then
                lblStockError.Text = "Stock must be a number."
                txtStock.Style("border") = "1px solid red"
                hasError = True
            Else
                lblStockError.Text = ""
                txtStock.Style("border") = "1px solid #ccc"
            End If
        End If

        ' Threshold
        Dim thresholdQty As Integer = 0
        If Not String.IsNullOrWhiteSpace(txtThreshold.Text) Then
            If Not Integer.TryParse(txtThreshold.Text, thresholdQty) Then
                lblThresholdError.Text = "Threshold must be a number."
                txtThreshold.Style("border") = "1px solid red"
                hasError = True
            Else
                lblThresholdError.Text = ""
                txtThreshold.Style("border") = "1px solid #ccc"
            End If
        End If

        ' --- CATEGORY ---
        If String.IsNullOrWhiteSpace(txtCategory.Text) Then
            lblCategoryError.Text = "Category is required."
            txtCategory.Style("border") = "1px solid red"
            hasError = True
        Else
            txtCategory.Style("border") = "1px solid #ccc"
        End If

        ' --- IMAGES ---
        Dim files = fuProductImages.PostedFiles
        Dim hasFiles As Boolean = False

        For Each f As HttpPostedFile In files
            If f.ContentLength > 0 Then
                hasFiles = True
                Exit For
            End If
        Next

        If Not hasFiles Then
            lblImageError.Text = "At least one image is required."
            fuProductImages.Style("border") = "1px solid red"
            hasError = True
        ElseIf files.Count > 4 Then
            lblImageError.Text = "Maximum 4 images allowed."
            fuProductImages.Style("border") = "1px solid red"
            hasError = True
        Else
            fuProductImages.Style("border") = "1px solid #ccc"
        End If

        If hasError Then
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "showModal", "document.getElementById('addProductModal').style.display='flex';", True)
            Return
        End If

        ' --- SAVE PRODUCT ---
        Dim userEmail = User.Identity.Name
        Using db As New OnlineShopContext()
            Dim user = db.Users.FirstOrDefault(Function(u) u.Email = userEmail)
            If user Is Nothing Then
                Return
            End If

            Dim shop = db.Shops.FirstOrDefault(Function(s) s.UserID = user.UserID)
            If shop Is Nothing Then
                Return
            End If

            ' Handle category: new or existing
            Dim categoryID As Guid
            Dim existingCategory = db.Categories.FirstOrDefault(Function(c) c.CategoryName.ToLower() = txtCategory.Text.Trim().ToLower())
            If existingCategory IsNot Nothing Then
                categoryID = existingCategory.CategoryID
            Else
                Dim newCategory As New Category With {
                .CategoryID = Guid.NewGuid(),
                .CategoryName = txtCategory.Text.Trim()
            }
                db.Categories.Add(newCategory)
                categoryID = newCategory.CategoryID
            End If

            ' Create product
            Dim product As New Product With {
            .ProductID = Guid.NewGuid(),
            .ShopID = shop.ShopID,
            .CategoryID = categoryID,
            .ProductName = txtProductName.Text.Trim(),
            .Description = txtProductDescription.Text.Trim(),
            .Price = priceDecimal,
            .StockQuantity = stockQty,
            .Threshold = thresholdQty,
            .CreatedAt = DateTime.Now,
            .UpdatedAt = DateTime.Now
        }

            db.Products.Add(product)

            ' Save images with Base64
            Dim mainImageIndex As Integer = Integer.Parse(hfMainImageIndex.Value)
            For i As Integer = 0 To files.Count - 1
                Dim file = files(i)
                Using ms As New IO.MemoryStream()
                    file.InputStream.CopyTo(ms)
                    Dim imgBase64 As String = Convert.ToBase64String(ms.ToArray())

                    Dim img As New ProductImage With {
                    .ImageID = Guid.NewGuid(),
                    .ProductID = product.ProductID,
                    .ImageData = imgBase64,
                    .MimeType = file.ContentType,
                    .IsMain = (i + 1 = mainImageIndex)
                }
                    db.ProductImages.Add(img)
                End Using
            Next

            db.SaveChanges()

            ' Clear form
            txtProductName.Text = ""
            txtPrice.Text = ""
            txtStock.Text = ""
            txtThreshold.Text = ""
            txtCategory.Text = ""
            txtProductDescription.Text = ""
            fuProductImages.Attributes.Clear()
            hfMainImageIndex.Value = "1"

            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "closeModal", "document.getElementById('addProductModal').style.display='none';", True)

            ' Reload repeater
            LoadProducts()
        End Using
    End Sub


    Private Sub LoadProducts()
        Dim userEmail = User.Identity.Name
        Using db As New OnlineShopContext()
            Dim user = db.Users.FirstOrDefault(Function(u) u.Email = userEmail)
            If user Is Nothing Then Return

            Dim shop = db.Shops.FirstOrDefault(Function(s) s.UserID = user.UserID)
            If shop Is Nothing Then Return

            Dim products = db.Products.
                Where(Function(p) p.ShopID = shop.ShopID).
                Select(Function(p) New With {
                    .ProductID = p.ProductID,
                    .ProductName = p.ProductName,
                    .Price = p.Price,
                    .StockQuantity = p.StockQuantity,
                    .ThresholdQuantity = p.Threshold,
                    .CategoryName = p.Category.CategoryName,
                    .Images = p.ProductImages.Select(Function(img) New With {
                        .ImageData = img.ImageData,
                        .IsMain = img.IsMain
                    }).ToList()
                }).ToList()

            rptProducts.DataSource = products
            rptProducts.DataBind()
        End Using
    End Sub


End Class
