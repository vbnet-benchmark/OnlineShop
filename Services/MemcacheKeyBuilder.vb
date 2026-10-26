''' <summary>
''' Builds memcache keys for the product-recommendation cache
''' (Services/ProductRecommendationCacheService.vb, called from ProductDetails.aspx.vb). Kept as its
''' own class, one hop further from the page than the service itself, the same way this project
''' already splits OnlineShopContext out from the pages that use it.
''' </summary>
Public Class MemcacheKeyBuilder

    ' Keeps even a maximal-length stored field (ShopName is <MaxLength(100)>) comfortably inside
    ' memcached's own 250-byte key limit once the fixed key prefixes are added on top.
    Private Const MaxKeyComponentLength As Integer = 60

    ''' <summary>
    ''' VULNERABLE: username is passed straight through from the caller with no validation --
    ''' nothing here strips CR/LF, other control characters, or whitespace before the value becomes
    ''' part of a memcache key.
    ''' </summary>
    Public Function BuildRecommendationKey(username As String) As String
        Return "shop:reco:member:" & username
    End Function

    ''' <summary>
    ''' SAFE: the caller (ProductRecommendationCacheService.GetCachedOtherShopProductIds /
    ''' StoreOtherShopProductIds) is expected to have already run shopNameComponent through
    ''' SanitizeKeyComponent below before calling this.
    ''' </summary>
    Public Function BuildShopScopedKey(shopNameComponent As String) As String
        Return "shop:reco:othershop:" & shopNameComponent
    End Function

    ''' <summary>
    ''' Allow-list sanitizer for a memcache key component: keeps only letters, digits, "-" and "_",
    ''' dropping everything else -- including CR, LF, every other control character, and plain
    ''' whitespace, all of which are either directly protocol-significant to memcached's text
    ''' protocol or (for whitespace) act as a key terminator -- then bounds the result to
    ''' MaxKeyComponentLength characters.
    ''' </summary>
    Public Shared Function SanitizeKeyComponent(raw As String) As String
        If String.IsNullOrEmpty(raw) Then Return String.Empty

        Dim builder As New System.Text.StringBuilder()
        For Each c As Char In raw
            If Char.IsLetterOrDigit(c) OrElse c = "-"c OrElse c = "_"c Then
                builder.Append(c)
            End If
        Next

        Dim cleaned = builder.ToString()
        If cleaned.Length > MaxKeyComponentLength Then
            cleaned = cleaned.Substring(0, MaxKeyComponentLength)
        End If
        Return cleaned
    End Function

End Class
