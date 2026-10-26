Imports System.Linq
Imports System.Data.Entity
Imports Enyim.Caching.Memcached

Partial Class SearchResults
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            BindSearch()
        End If
    End Sub

    Private Sub BindSearch()
        Dim query As String = Request.QueryString("q")
        lblQuery.Text = "Search results for: " & Server.HtmlEncode(query)

        Dim hasResults As Boolean = False

        Using db As New OnlineShopContext()
            ' Bind each section individually
            If BindProducts(db, query) Then hasResults = True
            If BindCategories(db, query) Then hasResults = True
            If BindShops(db, query) Then hasResults = True
        End Using

        ' Show "no results" if nothing found
        lblNoResults.Visible = Not hasResults
    End Sub

    ' -------------------------------
    ' Products
    ' -------------------------------
    Private Function BindProducts(db As OnlineShopContext, query As String) As Boolean
        ' ===== Memcached-backed product search-results cache =====
        ' VULNERABLE: the cache key is built straight from the raw, unmodified search query string
        ' -- no charset check, no CR/LF rejection, no length bound -- and handed directly to the
        ' memcached client. Any visitor can request SearchResults.aspx?q=<anything>, including a
        ' value containing CR/LF or other control characters.
        Dim cacheKey As String = "search:products:" & query
        Dim productList = MemcachedClientProvider.Client.Get(Of List(Of Object))(cacheKey) ' SINK: PLANTED-VB-HR-681

        If productList IsNot Nothing Then
            If productList.Any() Then
                pnlProducts.Visible = True
                rptProducts.DataSource = productList
                rptProducts.DataBind()
                Return True
            End If
            Return False
        End If

        Dim productsQuery = db.Products.Include("ProductImages").AsQueryable()
        If Not String.IsNullOrEmpty(query) Then
            productsQuery = productsQuery.Where(Function(p) p.ProductName.Contains(query) Or p.Description.Contains(query))
        End If

        productList = productsQuery.ToList().Select(Function(p)
                                                            Dim mainImage = p.ProductImages.FirstOrDefault(Function(img) img.IsMain)
                                                            Dim imgBase64 As String = Nothing
                                                            Dim imgMime As String = Nothing

                                                            If mainImage IsNot Nothing AndAlso mainImage.ImageData IsNot Nothing AndAlso mainImage.MimeType IsNot Nothing Then
                                                                imgBase64 = mainImage.ImageData
                                                                imgMime = mainImage.MimeType
                                                            End If

                                                            Return New With {
                                                                .ProductID = p.ProductID,
                                                                .ProductName = p.ProductName,
                                                                .Description = p.Description,
                                                                .Price = p.Price,
                                                                .ImageBase64 = imgBase64,
                                                                .ImageMimeType = imgMime
                                                            }
                                                        End Function).ToList()

        MemcachedClientProvider.Client.Store(StoreMode.Set, cacheKey, productList, TimeSpan.FromMinutes(5))

        If productList.Any() Then
            pnlProducts.Visible = True
            rptProducts.DataSource = productList
            rptProducts.DataBind()
            Return True
        End If

        Return False
    End Function

    ' -------------------------------
    ' Categories
    ' -------------------------------
    Private Function BindCategories(db As OnlineShopContext, query As String) As Boolean
        ' Start with root categories
        Dim categoriesQuery = db.Categories.AsQueryable()

        ' Only root categories
        categoriesQuery = categoriesQuery.Where(Function(c) c.ParentCategoryID Is Nothing)

        ' Only categories that have products
        categoriesQuery = categoriesQuery.Where(Function(c) c.Products.Any())

        ' Apply search filter if query exists
        If Not String.IsNullOrEmpty(query) Then
            categoriesQuery = categoriesQuery.Where(Function(c) c.CategoryName.Contains(query))
        End If

        ' Fetch categories and include subcategories with products
        Dim categoryList = categoriesQuery.ToList().Select(Function(c)
                                                               Dim subCats = c.SubCategories _
                                                                        .Where(Function(sc) sc.Products.Any()) _
                                                                        .ToList() _
                                                                        .Select(Function(sc) New With {
                                                                            .CategoryID = sc.CategoryID,
                                                                            .CategoryName = sc.CategoryName
                                                                        }).ToList()

                                                               Return New With {
                                                              .CategoryID = c.CategoryID,
                                                              .CategoryName = c.CategoryName,
                                                              .ImageData = If(c.ImageData IsNot Nothing, c.ImageData, ""),
                                                              .MimeType = If(c.MimeType IsNot Nothing, c.MimeType, "image/png"),
                                                              .SubCategories = If(subCats.Any(), subCats, New List(Of Object))
                                                          }
                                                           End Function).ToList()

        If categoryList.Any() Then
            pnlCategories.Visible = True
            rptMainCategories.DataSource = categoryList
            rptMainCategories.DataBind()
            Return True
        End If

        Return False
    End Function


    ' -------------------------------
    ' Shops
    ' -------------------------------
    Private Function BindShops(db As OnlineShopContext, query As String) As Boolean
        ' ===== Memcached-backed shop search-results cache =====
        ' SAFE: the cache key component is passed through MemcacheKeyBuilder.SanitizeKeyComponent
        ' (allow-list alphanumerics/"-"/"_" only, length-bounded) before it is ever concatenated into
        ' the cache key, so the value handed to the memcached client can never carry CR/LF or any
        ' other protocol-significant byte, no matter what the visitor typed into the search box.
        Dim sanitizedQuery = MemcacheKeyBuilder.SanitizeKeyComponent(query)
        Dim cacheKey As String = "search:shops:" & sanitizedQuery
        Dim shopList = MemcachedClientProvider.Client.Get(Of List(Of Object))(cacheKey) ' SAFE_SINK: PLANTED-VB-HR-681-safe

        If shopList IsNot Nothing Then
            If shopList.Any() Then
                pnlShops.Visible = True
                rptShops.DataSource = shopList
                rptShops.DataBind()
                Return True
            End If
            Return False
        End If

        Dim shopsQuery = db.Shops.Include("Owner").AsQueryable()
        If Not String.IsNullOrEmpty(query) Then
            shopsQuery = shopsQuery.Where(Function(s) s.ShopName.Contains(query) Or s.Owner.Username.Contains(query))
        End If

        shopList = shopsQuery.ToList().Select(Function(s)
                                                      Return New With {
                                                          .ShopID = s.ShopID,
                                                          .ShopName = s.ShopName,
                                                          .Username = s.Owner.Username,
                                                          .LogoBase64 = s.LogoBase64,
                                                          .LogoMimeType = s.LogoMimeType
                                                      }
                                                  End Function).ToList()

        MemcachedClientProvider.Client.Store(StoreMode.Set, cacheKey, shopList, TimeSpan.FromMinutes(5))

        If shopList.Any() Then
            pnlShops.Visible = True
            rptShops.DataSource = shopList
            rptShops.DataBind()
            Return True
        End If

        Return False
    End Function
End Class
