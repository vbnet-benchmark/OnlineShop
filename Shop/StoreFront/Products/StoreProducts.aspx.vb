Imports System.Runtime.Remoting.Metadata.W3cXsd2001
Imports Newtonsoft.Json
Imports Enyim.Caching.Memcached

Public Class StoreProducts
    Inherits System.Web.UI.Page

    Private shopID As Guid
    Private currentShop As Shop
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not User.Identity.IsAuthenticated Then
            Response.Redirect("~/shop/account/Login.aspx")
        End If

        If Not IsPostBack Then
            'Request.QueryString("ShopID") = "44899911-9665-4AF0-868B-873891A51D97"
            ' Get ShopID from query string
            If Not Guid.TryParse(Request.QueryString("shopid"), shopID) Then
                Response.Redirect("~/shop/StoreFront/CreateShop.aspx")
            End If
            LoadProducts()
            LoadCategories()

        End If

    End Sub

    Private Sub LoadCategories()
        Using db As New OnlineShopContext()
            ' Get categories once
            Dim categories = db.Categories.OrderBy(Function(c) c.CategoryName).ToList()

            ' Bind to Add Product dropdown
            ddlCategory.DataSource = categories
            ddlCategory.DataTextField = "CategoryName"
            ddlCategory.DataValueField = "CategoryID"
            ddlCategory.DataBind()
            ddlCategory.Items.Insert(0, New ListItem("-- Select Category --", ""))
        End Using
    End Sub


    Protected Sub rptProducts_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptProducts.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim product = CType(e.Item.DataItem, Object)
            Dim ddlEditCategory As DropDownList = CType(e.Item.FindControl("ddlEditCategory"), DropDownList)

            Using db As New OnlineShopContext()
                Dim categories = db.Categories.OrderBy(Function(c) c.CategoryName).ToList()
                ddlEditCategory.DataSource = categories
                ddlEditCategory.DataTextField = "CategoryName"
                ddlEditCategory.DataValueField = "CategoryID"
                ddlEditCategory.DataBind()
            End Using

            ' Safely set the selected value after binding
            Dim categoryID = DataBinder.Eval(product, "CategoryID").ToString()
            Dim item As ListItem = ddlEditCategory.Items.FindByValue(categoryID)
            If item IsNot Nothing Then
                ddlEditCategory.SelectedValue = categoryID
            End If
        End If
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

        ' --- STOCK ---
        Dim stockQty As Integer = 0
        If Not String.IsNullOrWhiteSpace(txtStock.Text) Then
            If Not Integer.TryParse(txtStock.Text, stockQty) Then
                lblStockError.Text = "Stock must be a number."
                txtStock.Style("border") = "1px solid red"
                hasError = True
            Else
                lblStockError.Text = ""
                txtStock.Style("border") = "1px solid #ccc"
            End If
        End If

        ' --- THRESHOLD ---
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
        If String.IsNullOrWhiteSpace(ddlCategory.SelectedValue) Or ddlCategory.SelectedIndex = "0" Then
            lblCategoryError.Text = "Category is required."
            ddlCategory.Style("border") = "1px solid red"
            hasError = True
        Else
            lblCategoryError.Text = ""
            ddlCategory.Style("border") = "1px solid #ccc"
        End If

        ' --- IMAGES (from hidden field) ---
        Dim imagesJson As String = hfImages.Value
        Dim imageList As List(Of ImageItem) = Nothing
        Dim hasFiles As Boolean = False

        If Not String.IsNullOrWhiteSpace(imagesJson) Then
            Try
                imageList = Newtonsoft.Json.JsonConvert.DeserializeObject(Of List(Of ImageItem))(imagesJson)
                hasFiles = (imageList IsNot Nothing AndAlso imageList.Count > 0)
            Catch ex As Exception
                hasFiles = False
            End Try
        End If

        If Not hasFiles Then
            lblImageError.Text = "At least one image is required."
            fuProductImages.Style("border") = "1px solid red"
            hasError = True
        ElseIf imageList.Count > 4 Then
            lblImageError.Text = "Maximum 4 images allowed."
            fuProductImages.Style("border") = "1px solid red"
            hasError = True
        Else
            fuProductImages.Style("border") = "1px solid #ccc"
        End If

        If hasError Then
            ' Re-open modal without clearing the form
            LoadProducts()
            addProductModal.Style("display") = "flex"
            btnShowAddProductModal.Text = "Close New Product"
            ' Clear any edit selection
            hfEditProductID.Value = Nothing

            ' Hide all edit panels inside the Repeater
            For Each item As RepeaterItem In rptProducts.Items
                Dim pnlEdit As Panel = TryCast(item.FindControl("pnlEdit"), Panel)
                If pnlEdit IsNot Nothing Then
                    pnlEdit.Visible = False
                End If
            Next
            'ScriptManager.RegisterStartupScript(Me, Me.GetType(), "showModal", "window.openAddProductModal(false);", True)
            Return
        End If


        ' --- SAVE PRODUCT ---
        Dim userEmail = User.Identity.Name
        Using db As New OnlineShopContext()
            Dim user = db.Users.FirstOrDefault(Function(u) u.Email = userEmail)
            If user Is Nothing Then Return

            Dim shop = db.Shops.FirstOrDefault(Function(s) s.UserID = user.UserID)
            If shop Is Nothing Then Return

            ' ===== Memcache-backed "just submitted this name" duplicate-click guard =====
            ' VULNERABLE: the cache key is built from shopID plus the vendor's own raw free-text
            ' product name (BuildProductNameCacheKey below, no helper-level sanitization) -- nothing
            ' here strips CR/LF, other control characters, or whitespace before it becomes part of
            ' a memcache key. Models/Product.vb's ProductName has a <MaxLength(150)> but no charset
            ' restriction at all -- ample room for a CR/LF-carrying payload.
            Dim dupCheckKey = BuildProductNameCacheKey(shop.ShopID, txtProductName.Text.Trim())
            Dim recentlySubmitted = MemcachedClientProvider.Client.Get(Of Boolean?)(dupCheckKey) ' SINK: PLANTED-VB-HR-682
            If recentlySubmitted.HasValue AndAlso recentlySubmitted.Value Then
                lblNameError.Text = "This product name was just submitted -- please wait a moment before retrying."
                LoadProducts()
                Return
            End If

            ' Handle category: new or existing
            Dim categoryID As Guid
            Dim existingCategory = db.Categories.FirstOrDefault(Function(c) c.CategoryName.ToLower() = ddlCategory.SelectedItem.Text.Trim().ToLower())
            If existingCategory IsNot Nothing Then categoryID = existingCategory.CategoryID

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
            .UpdatedAt = DateTime.Now,
            .IsActive = True
        }

            db.Products.Add(product)

            ' Save images with Base64 from hidden field
            Dim mainImageIndex As Integer = Integer.Parse(hfMainImageIndex.Value)
            For i As Integer = 0 To imageList.Count - 1
                Dim imgItem = imageList(i)

                ' Split full data URL into MIME and Base64
                Dim base64Data As String = imgItem.data
                Dim mimeType As String = imgItem.mime

                ' If the data still contains the data URL prefix, strip it
                If Not String.IsNullOrWhiteSpace(base64Data) AndAlso base64Data.StartsWith("data:") Then
                    Dim parts() As String = base64Data.Split(New String() {","}, StringSplitOptions.None)
                    If parts.Length = 2 Then
                        ' Extract MIME type from the prefix
                        mimeType = parts(0).Replace("data:", "").Replace(";base64", "").Trim()
                        ' Keep only the actual Base64 string
                        base64Data = parts(1)
                    End If
                End If

                Dim img As New ProductImage With {
        .ImageID = Guid.NewGuid(),
        .ProductID = product.ProductID,
        .ImageData = base64Data,
        .MimeType = mimeType,
        .IsMain = (i + 1 = mainImageIndex)
    }
                db.ProductImages.Add(img)
            Next


            db.SaveChanges()

            MemcachedClientProvider.Client.Store(StoreMode.Set, dupCheckKey, True, TimeSpan.FromSeconds(30))

            ' Reset add product form server-side
            txtProductName.BorderColor = Drawing.Color.Black
            txtPrice.BorderColor = Drawing.Color.Black
            txtStock.BorderColor = Drawing.Color.Black
            txtThreshold.BorderColor = Drawing.Color.Black
            ddlCategory.BorderColor = Drawing.Color.Black
            txtProductDescription.BorderColor = Drawing.Color.Black


            ' Reset validation labels
            txtProductName.Text = ""
            txtPrice.Text = ""
            txtStock.Text = ""
            txtThreshold.Text = ""
            ddlCategory.SelectedIndex = 0
            txtProductDescription.Text = ""
            fuProductImages.Attributes.Clear()
            hfImages.Value = ""
            hfMainImageIndex.Value = "1"

            addProductModal.Style("display") = "none"
            btnShowAddProductModal.Text = "Add New Product"

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
                Where(Function(p) p.ShopID = shop.ShopID And p.IsActive = True).
                Select(Function(p) New With {
                    .ProductID = p.ProductID,
                    .ProductName = p.ProductName,
                    .Price = p.Price,
                    .StockQuantity = p.StockQuantity,
                    .ThresholdQuantity = p.Threshold,
                    .CategoryID = p.CategoryID,        ' <-- added
                    .CategoryName = p.Category.CategoryName,
                    .Description = p.Description,        ' << add this!
                    .Images = p.ProductImages.Select(Function(img) New With {
                        .ImageData = img.ImageData,
                        .IsMain = img.IsMain
                    }).ToList()
                }).ToList()

            rptProducts.DataSource = products
            rptProducts.DataBind()
        End Using
    End Sub

    Private Sub ClearAddProductForm()
        txtProductName.Text = ""
        txtPrice.Text = ""
        txtStock.Text = ""
        txtThreshold.Text = ""
        txtProductDescription.Text = ""
        ddlCategory.SelectedIndex = 0
        hfImages.Value = ""
        hfMainImageIndex.Value = "1"
        ' Clear FileUpload inputs via JS if needed
    End Sub

    ''' <summary>
    ''' VULNERABLE: productName is the vendor's own raw free-text input (Models/Product.vb's
    ''' ProductName has no MaxLength/charset restriction at all) concatenated directly -- nothing
    ''' here strips CR/LF, other control characters, or whitespace before it becomes part of a
    ''' memcache key.
    ''' </summary>
    Private Function BuildProductNameCacheKey(shopID As Guid, productName As String) As String
        Return "shop:" & shopID.ToString() & ":product:" & productName
    End Function

    ''' <summary>
    ''' SAFE: sanitizedProductName has already been passed through
    ''' MemcacheKeyBuilder.SanitizeKeyComponent by the caller before reaching here.
    ''' </summary>
    Private Function BuildProductNameCacheKeySafe(shopID As Guid, sanitizedProductName As String) As String
        Return "shop:" & shopID.ToString() & ":product:" & sanitizedProductName
    End Function

    Protected Sub rptProducts_ItemCommand(sender As Object, e As RepeaterCommandEventArgs)
        If e.CommandName = "EditProduct" Then
            ' Set the hidden field to the product being edited
            hfEditProductID.Value = e.CommandArgument.ToString()

            ' Reset add product form server-side
            txtProductName.BorderColor = Drawing.Color.Black
            txtPrice.BorderColor = Drawing.Color.Black
            txtStock.BorderColor = Drawing.Color.Black
            txtThreshold.BorderColor = Drawing.Color.Black
            ddlCategory.BorderColor = Drawing.Color.Black
            txtProductDescription.BorderColor = Drawing.Color.Black


            ' Reset validation labels
            txtProductName.Text = ""
            txtPrice.Text = ""
            txtStock.Text = ""
            txtThreshold.Text = ""
            ddlCategory.SelectedIndex = 0
            txtProductDescription.Text = ""
            fuProductImages.Attributes.Clear()
            hfImages.Value = ""
            hfMainImageIndex.Value = "1"

            addProductModal.Style("display") = "none"
            btnShowAddProductModal.Text = "Add New Product"
            LoadProducts()
        ElseIf e.CommandName = "CancelEdit" Then
            ' Clear hidden field to exit edit mode
            hfEditProductID.Value = ""
            LoadProducts()

        ElseIf e.CommandName = "SaveProduct" Then
            Dim editProductId As Guid = Guid.Parse(e.CommandArgument.ToString())
            SaveEditedProduct(e.Item, editProductId)
            hfEditProductID.Value = ""
            LoadProducts()
        End If
    End Sub



    Private Sub SaveEditedProduct(item As RepeaterItem, productId As Guid)
        Dim txtName As TextBox = CType(item.FindControl("txtEditName"), TextBox)
        Dim txtPrice As TextBox = CType(item.FindControl("txtEditPrice"), TextBox)
        Dim txtStock As TextBox = CType(item.FindControl("txtEditStock"), TextBox)
        Dim txtThreshold As TextBox = CType(item.FindControl("txtEditThreshold"), TextBox)
        Dim ddlCategory As DropDownList = CType(item.FindControl("ddlEditCategory"), DropDownList)

        Using db As New OnlineShopContext()
            Dim product = db.Products.FirstOrDefault(Function(p) p.ProductID = productId)
            If product Is Nothing Then Return

            ' ===== Memcache-backed "just submitted this name" duplicate-click guard (edit path) =====
            ' SAFE: the product-name component is passed through MemcacheKeyBuilder.SanitizeKeyComponent
            ' (allow-list alphanumerics/"-"/"_" only, length-bounded) via BuildProductNameCacheKeySafe
            ' below before it is ever concatenated into the cache key.
            Dim sanitizedName = MemcacheKeyBuilder.SanitizeKeyComponent(txtName.Text.Trim())
            Dim dupCheckKeySafe = BuildProductNameCacheKeySafe(product.ShopID, sanitizedName)
            Dim recentlyRenamed = MemcachedClientProvider.Client.Get(Of Boolean?)(dupCheckKeySafe) ' SAFE_SINK: PLANTED-VB-HR-682-safe

            product.ProductName = txtName.Text.Trim()
            product.Price = Decimal.Parse(txtPrice.Text)
            product.StockQuantity = Integer.Parse(txtStock.Text)
            product.Threshold = Integer.Parse(txtThreshold.Text)
            product.CategoryID = Guid.Parse(ddlCategory.SelectedValue)
            product.UpdatedAt = DateTime.Now

            db.SaveChanges()

            If Not (recentlyRenamed.HasValue AndAlso recentlyRenamed.Value) Then
                MemcachedClientProvider.Client.Store(StoreMode.Set, dupCheckKeySafe, True, TimeSpan.FromSeconds(30))
            End If
        End Using

        ' Rebind repeater and exit edit mode
        SetProductEditState(Guid.Empty, False)
    End Sub



    Private Sub SetProductEditState(productId As Guid, isEditing As Boolean)
        Using db As New OnlineShopContext()
            Dim products = db.Products.Where(Function(p) p.IsActive).ToList()

            ' Create a view model for repeater including edit state
            Dim vm = products.Select(Function(p) New With {
            .ProductID = p.ProductID,
            .ProductName = p.ProductName,
            .Price = p.Price,
            .StockQuantity = p.StockQuantity,
            .ThresholdQuantity = p.Threshold,
            .CategoryID = p.CategoryID,
            .CategoryName = p.Category.CategoryName
        }).ToList()

            rptProducts.DataSource = vm
            rptProducts.DataBind()
        End Using
    End Sub





End Class
Public Class ImageItem
    Public Property mime As String
    Public Property data As String
End Class