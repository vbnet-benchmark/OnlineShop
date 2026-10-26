Imports Enyim.Caching.Memcached

''' <summary>
''' Per-viewer-type caching behavior for a shop's product-badge widget. ShopDetails.aspx.vb resolves
''' one of these two concrete implementations at runtime and calls it polymorphically through the
''' IViewerRecommendationCache interface -- see ResolveViewerRecommendationCache /
''' GetViewerIdentifier there for which value decides which concrete type gets instantiated, and
''' what identifier each one receives.
'''
''' This is a deliberately NEW hierarchy, separate from Services/PricingStrategies.vb's
''' IPricingStrategy -- that one is already used by Shop/Account/Checkout.aspx.vb's own
''' membership-tier scenario (PLANTED-VB-HR-119 / -119-safe), so it was not reused or overloaded for
''' this unrelated feature.
''' </summary>
Public Interface IViewerRecommendationCache
    Function GetCachedProductIds(scopePrefix As String, identifier As String) As List(Of Guid)
    Sub StoreProductIds(scopePrefix As String, identifier As String, productIds As List(Of Guid))
End Interface

''' <summary>
''' Resolved for an authenticated, signed-in shopper. identifier is their own raw, stored Username.
''' </summary>
Public Class MemberViewerRecommendationCache
    Implements IViewerRecommendationCache

    ''' <summary>
    ''' VULNERABLE: identifier is the member's own raw Username (free text set at registration,
    ''' Models/User.vb, no charset restriction beyond a 50-character max length) concatenated
    ''' directly -- nothing here validates it before it becomes part of a memcache key.
    ''' </summary>
    Public Function GetCachedProductIds(scopePrefix As String, identifier As String) As List(Of Guid) Implements IViewerRecommendationCache.GetCachedProductIds
        Dim cacheKey As String = scopePrefix & ":member:" & identifier
        Return MemcachedClientProvider.Client.Get(Of List(Of Guid))(cacheKey) ' SINK: PLANTED-VB-HR-684
    End Function

    Public Sub StoreProductIds(scopePrefix As String, identifier As String, productIds As List(Of Guid)) Implements IViewerRecommendationCache.StoreProductIds
        Dim cacheKey As String = scopePrefix & ":member:" & identifier
        MemcachedClientProvider.Client.Store(StoreMode.Set, cacheKey, productIds, TimeSpan.FromMinutes(10))
    End Sub
End Class

''' <summary>
''' Resolved for an anonymous visitor. identifier is always a server-generated anonymous session
''' GUID string (Session("AnonUserID"), standard "D"-format hex-and-hyphens only) -- never free
''' text, so there is nothing for an attacker to inject here regardless of how this class builds its
''' key.
''' </summary>
Public Class GuestViewerRecommendationCache
    Implements IViewerRecommendationCache

    Public Function GetCachedProductIds(scopePrefix As String, identifier As String) As List(Of Guid) Implements IViewerRecommendationCache.GetCachedProductIds
        Dim cacheKey As String = scopePrefix & ":guest:" & identifier
        Return MemcachedClientProvider.Client.Get(Of List(Of Guid))(cacheKey) ' SAFE_SINK: PLANTED-VB-HR-684-safe
    End Function

    Public Sub StoreProductIds(scopePrefix As String, identifier As String, productIds As List(Of Guid)) Implements IViewerRecommendationCache.StoreProductIds
        Dim cacheKey As String = scopePrefix & ":guest:" & identifier
        MemcachedClientProvider.Client.Store(StoreMode.Set, cacheKey, productIds, TimeSpan.FromMinutes(10))
    End Sub
End Class
