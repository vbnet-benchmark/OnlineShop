Imports Enyim.Caching.Memcached

''' <summary>
''' Memcached-backed lookups for ProductDetails.aspx's "you may also like" (per-member recommended
''' product ids) and "other products from this shop" widgets. Kept as a separate service class, the
''' same convention as OrderPricingService.vb, so the page code-behind doesn't have to know about the
''' memcached client directly. The actual key construction is one hop further still, in
''' MemcacheKeyBuilder.vb -- a genuine three-file chain from ProductDetails.aspx.vb through here and
''' into MemcacheKeyBuilder, back here for the actual cache call.
''' </summary>
Public Class ProductRecommendationCacheService

    Private ReadOnly _keyBuilder As New MemcacheKeyBuilder()

    ''' <summary>
    ''' VULNERABLE: username flows in from ProductDetails.aspx.vb's BindRecommendedProducts -- the
    ''' signed-in viewer's own raw, stored Username -- through MemcacheKeyBuilder.BuildRecommendationKey,
    ''' which concatenates it into the key with no validation at all, straight into the memcached
    ''' client call below.
    ''' </summary>
    Public Function GetCachedRecommendationIds(username As String) As List(Of Guid)
        If String.IsNullOrEmpty(username) Then Return Nothing
        Dim cacheKey As String = _keyBuilder.BuildRecommendationKey(username)
        Return MemcachedClientProvider.Client.Get(Of List(Of Guid))(cacheKey) ' SINK: PLANTED-VB-HR-683
    End Function

    Public Sub StoreRecommendationIds(username As String, productIds As List(Of Guid))
        If String.IsNullOrEmpty(username) Then Return
        Dim cacheKey As String = _keyBuilder.BuildRecommendationKey(username)
        MemcachedClientProvider.Client.Store(StoreMode.Set, cacheKey, productIds, TimeSpan.FromMinutes(10))
    End Sub

    ''' <summary>
    ''' SAFE: shopName is passed through MemcacheKeyBuilder.SanitizeKeyComponent (allow-list
    ''' alphanumerics/"-"/"_" only, length-bounded) before it is ever handed to BuildShopScopedKey, so
    ''' the value that actually lands in the memcache key can never carry CR/LF or any other
    ''' protocol-significant byte, regardless of what the shop owner originally typed as their
    ''' ShopName.
    ''' </summary>
    Public Function GetCachedOtherShopProductIds(shopName As String) As List(Of Guid)
        Dim sanitizedShopName = MemcacheKeyBuilder.SanitizeKeyComponent(shopName)
        Dim cacheKey As String = _keyBuilder.BuildShopScopedKey(sanitizedShopName)
        Return MemcachedClientProvider.Client.Get(Of List(Of Guid))(cacheKey) ' SAFE_SINK: PLANTED-VB-HR-683-safe
    End Function

    Public Sub StoreOtherShopProductIds(shopName As String, productIds As List(Of Guid))
        Dim sanitizedShopName = MemcacheKeyBuilder.SanitizeKeyComponent(shopName)
        Dim cacheKey As String = _keyBuilder.BuildShopScopedKey(sanitizedShopName)
        MemcachedClientProvider.Client.Store(StoreMode.Set, cacheKey, productIds, TimeSpan.FromMinutes(10))
    End Sub

End Class
