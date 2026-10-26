Imports System.Linq
Imports System.Web.UI.HtmlControls

Public Class ProductDetails
    Inherits System.Web.UI.Page

    Protected ShopThemeColor As String = "#007bff"

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        ' ===== Get Product ID =====
        Dim productId As Guid
        If Not Guid.TryParse(Request.QueryString("ID"), productId) Then Return

        ' ===== Load All Product Info =====
        Using db As New OnlineShopContext()
            Dim product = db.Products.Include("ProductImages").Include("Shop").FirstOrDefault(Function(p) p.ProductID = productId)
            If product Is Nothing Then Return

            ' ===== Determine User =====
            Dim userGuid = GetCurrentUserGuid(db)

            ' ===== Log Product View (only once, optional) =====
            If Not IsPostBack Then
                LogUserProductView(db, userGuid, productId)
            End If

            ' ===== Bind Product & Shop Info =====
            ' These can remain static, bind only once
            If Not IsPostBack Then
                BindProductInfo(product)
                BindShopInfo(product.Shop)
            End If

            ' ===== Bind Dynamic Content =====
            ' Must bind on every page load (postbacks included)
            BindMainImageAndThumbnails(product)
            BindOtherShopProducts(db, product)
            BindRecommendedProducts(db, productId, userGuid)
        End Using
    End Sub


    Private Function GetCurrentUserGuid(db As OnlineShopContext) As Guid
        Dim userGuid As Guid

        If User.Identity.IsAuthenticated Then
            Dim userModel = db.Users.FirstOrDefault(Function(u) u.Email = User.Identity.Name)
            userGuid = If(userModel IsNot Nothing, userModel.UserID, Guid.NewGuid())

            ' Migrate anonymous views
            If Session("AnonUserID") IsNot Nothing Then
                Dim anonId As Guid = CType(Session("AnonUserID"), Guid)
                Dim anonViews = db.UserProductViews.Where(Function(v) v.UserID = anonId).ToList()
                For Each v In anonViews
                    v.UserID = userGuid
                Next
                db.SaveChanges()
                Session.Remove("AnonUserID")
            End If
        Else
            If Session("AnonUserID") Is Nothing Then Session("AnonUserID") = Guid.NewGuid()
            userGuid = CType(Session("AnonUserID"), Guid)
        End If

        Return userGuid
    End Function

    Private Sub LogUserProductView(db As OnlineShopContext, userGuid As Guid, productId As Guid)
        Dim view As New UserProductView With {
        .UserID = userGuid,
        .ProductID = productId,
        .ViewedAt = DateTime.Now
    }
        db.UserProductViews.Add(view)
        db.SaveChanges()
    End Sub

    Private Sub BindProductInfo(product As Product)
        lblProductName.Text = product.ProductName
        lblPrice.Text = product.Price.ToString("F2")
        lblDescription.Text = product.Description

        ' Set max quantity for Add to Cart
        txtQuantity.Attributes("max") = product.StockQuantity.ToString()
        txtQuantity.Text = If(product.StockQuantity > 0, "1", "0")
        btnAddToCart.Enabled = product.StockQuantity > 0
    End Sub

    Private Sub BindShopInfo(shop As Shop)
        Dim themeColor = If(String.IsNullOrEmpty(shop.ThemeColor), "#007bff", shop.ThemeColor)
        divShop.Style("border-left") = $"8px solid {themeColor}"

        hlShopName.Text = shop.ShopName
        hlShopName.NavigateUrl = $"ShopDetails.aspx?ShopID={shop.ShopID}"

        If Not String.IsNullOrEmpty(shop.LogoBase64) Then
            imgShopLogo.ImageUrl = $"data:{shop.LogoMimeType};base64,{shop.LogoBase64}"
        End If
        If Not String.IsNullOrEmpty(shop.BannerBase64) Then
            divShopBanner.Style("background-image") = $"url('data:{shop.BannerMimeType};base64,{shop.BannerBase64}')"
            divShopBanner.Style("background-size") = "cover"
            divShopBanner.Style("background-position") = "center"
        End If
    End Sub

    Private Sub BindMainImageAndThumbnails(product As Product)
        If product.ProductImages.Any() Then
            imgMainProduct.Src = $"data:image/png;base64,{product.ProductImages.First().ImageData}"
            imgModal.Src = imgMainProduct.Src
        End If

        divThumbnails.Controls.Clear()
        For Each img In product.ProductImages
            Dim thumb As New HtmlGenericControl("img")
            thumb.Attributes("src") = $"data:image/png;base64,{img.ImageData}"
            thumb.Attributes("class") = "img-thumbnail rounded me-2 mb-2"
            thumb.Style("width") = "75px"
            thumb.Style("height") = "75px"
            thumb.Style("cursor") = "pointer"
            thumb.Attributes("onclick") = $"document.getElementById('{imgMainProduct.ClientID}').src=this.src; document.getElementById('{imgModal.ClientID}').src=this.src;"
            divThumbnails.Controls.Add(thumb)
        Next
    End Sub

    Private Sub BindOtherShopProducts(db As OnlineShopContext, currentProduct As Product)
        ' ===== Memcached-backed "other products from this shop" cache =====
        ' SAFE: currentProduct.Shop.ShopName is passed through
        ' MemcacheKeyBuilder.SanitizeKeyComponent (allow-list, length-bounded) inside
        ' ProductRecommendationCacheService before it is ever concatenated into a memcache key --
        ' see Services/ProductRecommendationCacheService.vb / Services/MemcacheKeyBuilder.vb (a
        ' genuine three-file chain: this page, the service, and the key builder).
        Dim cacheService As New ProductRecommendationCacheService()
        Dim otherProductIds = cacheService.GetCachedOtherShopProductIds(currentProduct.Shop.ShopName)

        Dim otherProducts As List(Of Product)
        If otherProductIds IsNot Nothing Then
            otherProducts = db.Products.Where(Function(p) otherProductIds.Contains(p.ProductID)).ToList()
        Else
            otherProducts = db.Products.Where(Function(p) p.ShopID = currentProduct.ShopID AndAlso p.ProductID <> currentProduct.ProductID).ToList()
            cacheService.StoreOtherShopProductIds(currentProduct.Shop.ShopName, otherProducts.Select(Function(p) p.ProductID).ToList())
        End If

        otherProductsDiv.Controls.Clear()
        For Each op In otherProducts
            Dim card As New HtmlGenericControl("div")
            card.Attributes("class") = "card flex-shrink-0 me-2 mb-3"
            card.Style("width") = "150px"
            card.Style("cursor") = "pointer"

            Dim imgCtrl As New HtmlGenericControl("img")
            If op.ProductImages.Any() Then imgCtrl.Attributes("src") = $"data:image/png;base64,{op.ProductImages.First().ImageData}"
            imgCtrl.Attributes("class") = "card-img-top"
            imgCtrl.Style("height") = "150px"
            card.Controls.Add(imgCtrl)

            Dim body As New HtmlGenericControl("div")
            body.Attributes("class") = "card-body p-2"
            body.InnerHtml = $"<h6 class='card-title mb-1'>{op.ProductName}</h6><p class='card-text text-primary mb-0'>${op.Price:F2}</p>"
            card.Controls.Add(body)

            card.Attributes("onclick") = $"window.location='ProductDetails.aspx?ID={op.ProductID}';"
            otherProductsDiv.Controls.Add(card)
        Next
    End Sub

    Private Sub BindRecommendedProducts(db As OnlineShopContext, productId As Guid, userGuid As Guid)
        ' ===== Memcached-backed "you may also like" cache =====
        ' VULNERABLE: for a signed-in member, the cache lookup is keyed off their own raw, stored
        ' Username (Models/User.vb, free text set at registration, no charset restriction) -- see
        ' Services/ProductRecommendationCacheService.vb / Services/MemcacheKeyBuilder.vb (a genuine
        ' three-file chain: this page, the service, and the key builder, where the actual memcached
        ' client call lives back in the service).
        Dim username As String = Nothing
        If User.Identity.IsAuthenticated Then
            Dim userModel = db.Users.FirstOrDefault(Function(u) u.UserID = userGuid)
            username = If(userModel IsNot Nothing, userModel.Username, Nothing)
        End If

        Dim cacheService As New ProductRecommendationCacheService()
        Dim recommendedProductIDs As List(Of Guid) = cacheService.GetCachedRecommendationIds(username)

        If recommendedProductIDs Is Nothing Then
            ' Get recommended product IDs
            Dim recentViews = db.UserProductViews _
            .Where(Function(v) v.UserID = userGuid) _
            .OrderByDescending(Function(v) v.ViewedAt) _
            .Select(Function(v) v.ProductID) _
            .Distinct() _
            .Take(20).ToList()

            If recentViews.Any() Then
                recommendedProductIDs = db.UserProductViews _
                .Where(Function(v) recentViews.Contains(v.ProductID) AndAlso v.ProductID <> productId) _
                .GroupBy(Function(v) v.ProductID) _
                .Select(Function(g) g.Key) _
                .Take(20).ToList()
            Else
                recommendedProductIDs = db.Products _
                .Where(Function(p) p.ProductID <> productId) _
                .OrderBy(Function(p) Guid.NewGuid()) _
                .Take(20) _
                .Select(Function(p) p.ProductID).ToList()
            End If

            cacheService.StoreRecommendationIds(username, recommendedProductIDs)
        End If

        Dim products = db.Products.Include("ProductImages") _
                    .Where(Function(p) recommendedProductIDs.Contains(p.ProductID)).ToList()

        recommendedProductsSlider.Controls.Clear()

        For Each p In products
            Dim card As New HtmlGenericControl("div")
            card.Attributes("class") = "card"
            card.Style("cursor") = "pointer"

            Dim img As New HtmlGenericControl("img")
            If p.ProductImages.Any() Then
                img.Attributes("src") = $"data:image/png;base64,{p.ProductImages.First().ImageData}"
            End If
            img.Attributes("class") = "card-img-top"
            img.Style("height") = "150px"
            card.Controls.Add(img)

            Dim body As New HtmlGenericControl("div")
            body.Attributes("class") = "card-body p-2"
            body.InnerHtml = $"<h6 class='card-title mb-1'>{p.ProductName}</h6><p class='card-text text-primary mb-0'>${p.Price:F2}</p>"
            card.Controls.Add(body)

            card.Attributes("onclick") = $"window.location='ProductDetails.aspx?ID={p.ProductID}';"
            recommendedProductsSlider.Controls.Add(card)
        Next
    End Sub







    Protected Sub btnAddToCart_Click(sender As Object, e As EventArgs) Handles btnAddToCart.Click
        Dim qty As Integer
        If Not Integer.TryParse(txtQuantity.Text, qty) OrElse qty <= 0 Then
            lblCartMessage.Text = "Please enter a valid quantity."
            Return
        End If

        Using db As New OnlineShopContext()
            ' Get product ID from query string
            Dim productId As Guid = Guid.Parse(Request.QueryString("id"))

            ' Load the product to check stock
            Dim product = db.Products.FirstOrDefault(Function(p) p.ProductID = productId)
            If product Is Nothing Then
                lblCartMessage.Text = "Product not found."
                Return
            End If

            ' Determine user ID (logged in or anon)
            Dim userGuid As Guid
            If User.Identity.IsAuthenticated Then
                Dim userModel = db.Users.FirstOrDefault(Function(u) u.Email = User.Identity.Name)
                If userModel Is Nothing Then
                    lblCartMessage.Text = "Error: user not found."
                    Return
                End If
                userGuid = userModel.UserID
            Else
                If Session("AnonUserID") Is Nothing Then Session("AnonUserID") = Guid.NewGuid()
                userGuid = CType(Session("AnonUserID"), Guid)
            End If

            ' Check if item already in cart
            Dim existingItem = db.UserCartItems.FirstOrDefault(Function(c) c.UserID = userGuid AndAlso c.ProductID = productId)
            Dim totalRequested = qty
            If existingItem IsNot Nothing Then
                totalRequested += existingItem.Quantity
            End If

            ' Check against available stock
            If totalRequested > product.StockQuantity Then
                lblCartMessage.Text = $"Sorry, only {product.StockQuantity} unit(s) are available to order."
                Return
            End If

            ' Add/update cart item
            If existingItem IsNot Nothing Then
                existingItem.Quantity += qty
            Else
                Dim newItem As New CartItem With {
                    .UserID = userGuid,
                    .ProductID = productId,
                    .Quantity = qty,
                    .DateAdded = DateTime.Now
                }
                db.UserCartItems.Add(newItem)
            End If

            db.SaveChanges()
            lblCartMessage.Text = "Item added to cart!"
            ' Update the master page cart count
            Dim master As Site = CType(Me.Master, Site)
            master.UpdateCartCount()
        End Using
    End Sub



    'Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
    '    If IsPostBack Then Return

    '    ' ===== Get Product ID =====
    '    Dim productId As Guid
    '    If Not Guid.TryParse(Request.QueryString("ID"), productId) Then Return

    '    ' ===== Open DB Context =====
    '    Using db As New OnlineShopContext()
    '        ' ===== Load Product =====
    '        Dim product = db.Products.Include("ProductImages").Include("Shop").FirstOrDefault(Function(p) p.ProductID = productId)
    '        If product Is Nothing Then Return

    '        Dim shop = product.Shop

    '        ' ===== Determine User Identifier =====
    '        Dim userGuid As Guid
    '        If User.Identity.IsAuthenticated Then
    '            Dim usermodel = db.Users.FirstOrDefault(Function(u) u.Email = User.Identity.Name)
    '            userGuid = If(User IsNot Nothing, usermodel.UserID, Guid.NewGuid())

    '            ' Migrate anonymous views to logged-in user
    '            If Session("AnonUserID") IsNot Nothing Then
    '                Dim anonId As Guid = CType(Session("AnonUserID"), Guid)
    '                Dim anonViews = db.UserProductViews.Where(Function(v) v.UserID = anonId).ToList()
    '                For Each v In anonViews
    '                    v.UserID = userGuid
    '                Next
    '                db.SaveChanges()
    '                Session.Remove("AnonUserID")
    '            End If
    '        Else
    '            ' Anonymous user: use session GUID
    '            If Session("AnonUserID") Is Nothing Then Session("AnonUserID") = Guid.NewGuid()
    '            userGuid = CType(Session("AnonUserID"), Guid)
    '        End If

    '        ' ===== Log the View =====
    '        Dim view As New UserProductView With {
    '            .UserID = userGuid,
    '            .ProductID = productId,
    '            .ViewedAt = DateTime.Now
    '        }
    '        db.UserProductViews.Add(view)
    '        db.SaveChanges()

    '        ' ===== Bind Main Product Info =====
    '        lblProductName.Text = product.ProductName
    '        lblPrice.Text = product.Price.ToString("F2")
    '        lblDescription.Text = product.Description

    '        ' ===== Shop Info =====
    '        ShopThemeColor = If(String.IsNullOrEmpty(shop.ThemeColor), "#007bff", shop.ThemeColor)
    '        divShop.Style("border-left") = $"8px solid {ShopThemeColor}"

    '        hlShopName.Text = shop.ShopName
    '        hlShopName.NavigateUrl = $"ShopDetails.aspx?ShopID={shop.ShopID}"

    '        If Not String.IsNullOrEmpty(shop.LogoBase64) Then
    '            imgShopLogo.ImageUrl = $"data:{shop.LogoMimeType};base64,{shop.LogoBase64}"
    '        End If

    '        If Not String.IsNullOrEmpty(shop.BannerBase64) Then
    '            divShopBanner.Style("background-image") = $"url('data:{shop.BannerMimeType};base64,{shop.BannerBase64}')"
    '            divShopBanner.Style("background-size") = "cover"
    '            divShopBanner.Style("background-position") = "center"
    '        End If

    '        ' ===== Main Product Image =====
    '        If product.ProductImages.Any() Then
    '            imgMainProduct.Src = $"data:image/png;base64,{product.ProductImages.First().ImageData}"
    '            imgModal.Src = imgMainProduct.Src
    '        End If

    '        ' ===== Thumbnails =====
    '        divThumbnails.Controls.Clear()
    '        For Each img In product.ProductImages
    '            Dim thumb As New HtmlGenericControl("img")
    '            thumb.Attributes("src") = $"data:image/png;base64,{img.ImageData}"
    '            thumb.Attributes("class") = "img-thumbnail rounded me-2 mb-2"
    '            thumb.Style("width") = "75px"
    '            thumb.Style("height") = "75px"
    '            thumb.Style("cursor") = "pointer"
    '            thumb.Attributes("onclick") = $"document.getElementById('{imgMainProduct.ClientID}').src=this.src; document.getElementById('{imgModal.ClientID}').src=this.src;"
    '            divThumbnails.Controls.Add(thumb)
    '        Next

    '        ' ===== Other Products in Shop =====
    '        Dim otherProducts = db.Products.Where(Function(p) p.ShopID = shop.ShopID AndAlso p.ProductID <> productId).ToList()
    '        otherProductsDiv.Controls.Clear()
    '        For Each op In otherProducts
    '            Dim card As New HtmlGenericControl("div")
    '            card.Attributes("class") = "card flex-shrink-0 me-2 mb-3"
    '            card.Style("width") = "150px"
    '            card.Style("cursor") = "pointer"

    '            Dim imgCtrl As New HtmlGenericControl("img")
    '            If op.ProductImages.Any() Then
    '                imgCtrl.Attributes("src") = $"data:image/png;base64,{op.ProductImages.First().ImageData}"
    '            End If
    '            imgCtrl.Attributes("class") = "card-img-top"
    '            imgCtrl.Style("height") = "150px"
    '            card.Controls.Add(imgCtrl)

    '            Dim body As New HtmlGenericControl("div")
    '            body.Attributes("class") = "card-body p-2"
    '            body.InnerHtml = $"<h6 class='card-title mb-1'>{op.ProductName}</h6><p class='card-text text-primary mb-0'>${op.Price:F2}</p>"
    '            card.Controls.Add(body)

    '            card.Attributes("onclick") = $"window.location='ProductDetails.aspx?ID={op.ProductID}';"
    '            otherProductsDiv.Controls.Add(card)
    '        Next

    '        ' =========================================
    '        ' You May Also Like - Smart Recommendations
    '        ' =========================================

    '        ' 1️⃣ Get recent views for this user
    '        Dim recentViews = db.UserProductViews _
    '            .Where(Function(v) v.UserID = userGuid) _
    '            .OrderByDescending(Function(v) v.ViewedAt) _
    '            .Select(Function(v) v.ProductID) _
    '            .Distinct() _
    '            .Take(10) _
    '            .ToList()

    '        Dim recommendedProductIDs As List(Of Guid)

    '        If recentViews.Any() Then
    '            ' 2️⃣ Find products viewed by users who also viewed these products
    '            Dim recommendedProductsQuery = db.UserProductViews _
    '                .Where(Function(v) recentViews.Contains(v.ProductID) AndAlso v.ProductID <> productId) _
    '                .GroupBy(Function(v) v.ProductID) _
    '                .Select(Function(g) New With {Key .ProductID = g.Key, Key .ViewCount = g.Count()}) _
    '                .OrderByDescending(Function(x) x.ViewCount) _
    '                .Take(6)

    '            recommendedProductIDs = recommendedProductsQuery.Select(Function(x) x.ProductID).ToList()
    '        Else
    '            ' 3️⃣ Fallback: Random 6 products excluding current
    '            recommendedProductIDs = db.Products _
    '                .Where(Function(p) p.ProductID <> productId) _
    '                .OrderBy(Function(p) Guid.NewGuid()) _
    '                .Take(6) _
    '                .Select(Function(p) p.ProductID).ToList()
    '        End If

    '        ' 4️⃣ Load actual product objects
    '        Dim youMayAlsoLike = db.Products.Include("ProductImages") _
    '            .Where(Function(p) recommendedProductIDs.Contains(p.ProductID)).ToList()

    '        ' 5️⃣ Bind to carousel div
    '        youMayAlsoLikeDiv.Controls.Clear()
    '        For Each op In youMayAlsoLike
    '            Dim card As New HtmlGenericControl("div")
    '            card.Attributes("class") = "card flex-shrink-0 me-2 mb-3"
    '            card.Style("width") = "150px"
    '            card.Style("cursor") = "pointer"

    '            ' Image
    '            Dim imgCtrl As New HtmlGenericControl("img")
    '            If op.ProductImages.Any() Then
    '                imgCtrl.Attributes("src") = $"data:image/png;base64,{op.ProductImages.First().ImageData}"
    '            End If
    '            imgCtrl.Attributes("class") = "card-img-top"
    '            imgCtrl.Style("height") = "150px"
    '            card.Controls.Add(imgCtrl)

    '            ' Card body
    '            Dim body As New HtmlGenericControl("div")
    '            body.Attributes("class") = "card-body p-2"
    '            body.InnerHtml = $"<h6 class='card-title mb-1'>{op.ProductName}</h6><p class='card-text text-primary mb-0'>${op.Price:F2}</p>"
    '            card.Controls.Add(body)

    '            ' Click to product
    '            card.Attributes("onclick") = $"window.location='ProductDetails.aspx?ID={op.ProductID}';"
    '            youMayAlsoLikeDiv.Controls.Add(card)
    '        Next


    '    End Using
    'End Sub
End Class
