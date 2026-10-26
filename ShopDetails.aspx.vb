Imports System.Data.Entity

Public Class ShopDetails
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim shopIdString As String = Request.QueryString("ShopID")
            Dim shopID As Guid

            If Not String.IsNullOrEmpty(shopIdString) AndAlso Guid.TryParse(shopIdString, shopID) Then
                LoadShop(shopID)
            Else
                Response.Write("Invalid or missing shop ID.")
            End If
        End If
    End Sub

    Private Sub LoadShop(shopID As Guid)
        Using db As New OnlineShopContext()
            Dim shop = db.Shops.Find(shopID)
            If shop IsNot Nothing Then
                ' ===== Shop Info =====
                lblShopName.Text = shop.ShopName
                lblOwner.Text = db.Users.Find(shop.UserID)?.Username
                lblCreatedAt.Text = shop.CreatedAt.ToString("yyyy-MM-dd")
                lblDescription.Text = shop.Description

                ' Theme
                If Not String.IsNullOrEmpty(shop.ThemeColor) Then
                    btnFollow.CssClass = "btn btn-sm"
                    btnFollow.Style("background-color") = shop.ThemeColor
                    btnFollow.Style("border-color") = shop.ThemeColor
                    btnFollow.Style("color") = "#fff"
                End If


                ' Logo
                If Not String.IsNullOrEmpty(shop.LogoBase64) Then
                    imgShopLogo.ImageUrl = $"data:{shop.LogoMimeType};base64,{shop.LogoBase64}"
                End If

                ' Banner
                If Not String.IsNullOrEmpty(shop.BannerBase64) Then
                    divShopBanner.Style("background-image") = $"url('data:{shop.BannerMimeType};base64,{shop.BannerBase64}')"
                    divShopBanner.Style("background-size") = "cover"
                    divShopBanner.Style("background-position") = "center"
                End If


                ' ===== Shop Products (cached per-viewer) =====
                ' The concrete cache implementation is resolved by ResolveViewerRecommendationCache
                ' below -- MemberViewerRecommendationCache (VULNERABLE: keys off the viewer's own raw
                ' stored Username, PLANTED-VB-HR-684) for a signed-in member, or
                ' GuestViewerRecommendationCache (SAFE: keys off a server-generated session GUID,
                ' PLANTED-VB-HR-684-safe) for an anonymous visitor. This call site itself has no
                ' branch at all -- see Services/RecommendationCacheStrategies.vb.
                Dim viewerCache As IViewerRecommendationCache = ResolveViewerRecommendationCache()
                Dim viewerIdentifier As String = GetViewerIdentifier(db)
                Dim cacheScopePrefix As String = "shop:" & shopID.ToString()

                Dim cachedProductIds = viewerCache.GetCachedProductIds(cacheScopePrefix, viewerIdentifier)

                Dim products As List(Of Product)
                If cachedProductIds IsNot Nothing Then
                    products = db.Products.Where(Function(p) cachedProductIds.Contains(p.ProductID)) _
                                          .Include("ProductImages") _
                                          .ToList()
                Else
                    products = db.Products.Where(Function(p) p.ShopID = shopID) _
                                          .Include("ProductImages") _
                                          .ToList()
                    viewerCache.StoreProductIds(cacheScopePrefix, viewerIdentifier, products.Select(Function(p) p.ProductID).ToList())
                End If

                Dim productList = products.Select(Function(p)
                                                      Dim mainImage = p.ProductImages.FirstOrDefault()
                                                      Dim imageBase64 As String = Nothing

                                                      If mainImage IsNot Nothing AndAlso Not String.IsNullOrEmpty(mainImage.ImageData) Then
                                                          ' Directly embed the base64 data like on ProductDetails.aspx
                                                          imageBase64 = $"data:image/png;base64,{mainImage.ImageData}"
                                                      End If

                                                      Return New With {
                                                          .ProductID = p.ProductID,
                                                          .ProductName = p.ProductName,
                                                          .Price = p.Price,
                                                          .ImageBase64 = imageBase64
                                                      }
                                                  End Function).ToList()

                rptShopProducts.DataSource = productList
                rptShopProducts.DataBind()
            Else
                Response.Write("Shop not found.")
            End If
        End Using
    End Sub

    Private Function ResolveViewerRecommendationCache() As IViewerRecommendationCache
        If User.Identity.IsAuthenticated Then
            Return New MemberViewerRecommendationCache()
        End If
        Return New GuestViewerRecommendationCache()
    End Function

    Private Function GetViewerIdentifier(db As OnlineShopContext) As String
        If User.Identity.IsAuthenticated Then
            Dim userModel = db.Users.FirstOrDefault(Function(u) u.Email = User.Identity.Name)
            Return If(userModel IsNot Nothing, userModel.Username, "")
        End If

        If Session("AnonUserID") Is Nothing Then Session("AnonUserID") = Guid.NewGuid()
        Return CType(Session("AnonUserID"), Guid).ToString()
    End Function

End Class
