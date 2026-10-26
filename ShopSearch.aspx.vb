Public Class ShopSearch
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Using db As New OnlineShopContext()
                ' Include the User navigation property
                Dim shops = db.Shops.Include("Owner").ToList()

                ' ===== Batch-fetch each shop owner's "recently active" badge from memcache =====
                ' VULNERABLE: builds and looks up one cache key per shop in a loop, straight from
                ' each owner's own raw, stored Username (Models/User.vb, free text set at
                ' registration, no charset restriction) -- any one vendor who ever set a Username
                ' containing CR/LF could corrupt or collide with another vendor's badge entry in
                ' this same shared batch loop.
                Dim activeBadges As New Dictionary(Of Guid, Boolean)
                For Each s In shops
                    Dim badgeKey As String = "shop:owner:active:" & s.Owner.Username
                    Dim isActive = MemcachedClientProvider.Client.Get(Of Boolean?)(badgeKey) ' SINK: PLANTED-VB-HR-685
                    activeBadges(s.ShopID) = isActive.HasValue AndAlso isActive.Value
                Next

                ' ===== Batch-fetch each shop's "trending" badge, same shape, sanitized keys =====
                ' SAFE: each owner's Username is passed through
                ' MemcacheKeyBuilder.SanitizeKeyComponent before it becomes part of a memcache key,
                ' so this loop can never build a key carrying CR/LF or any other
                ' protocol-significant byte.
                Dim trendingBadges As New Dictionary(Of Guid, Boolean)
                For Each s In shops
                    Dim sanitizedOwnerName = MemcacheKeyBuilder.SanitizeKeyComponent(s.Owner.Username)
                    Dim trendingKey As String = "shop:owner:trending:" & sanitizedOwnerName
                    Dim isTrending = MemcachedClientProvider.Client.Get(Of Boolean?)(trendingKey) ' SAFE_SINK: PLANTED-VB-HR-685-safe
                    trendingBadges(s.ShopID) = isTrending.HasValue AndAlso isTrending.Value
                Next

                ' Project into an anonymous object with Username + Logo
                rptShops.DataSource = shops.Select(Function(s) New With {
                    .ShopID = s.ShopID,
                    .ShopName = s.ShopName,
                    .Username = s.Owner.Username,
                    .CreatedAt = s.CreatedAt,
                    .LogoBase64 = s.LogoBase64,
                    .LogoMimeType = s.LogoMimeType,
                    .IsRecentlyActive = activeBadges(s.ShopID),
                    .IsTrending = trendingBadges(s.ShopID)
                }).ToList()

                rptShops.DataBind()
            End Using
        End If
    End Sub




End Class