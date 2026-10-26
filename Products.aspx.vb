Imports System.Data.Entity

Public Class Products
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        BindProducts()
    End Sub
    Private Sub BindProducts()
        Dim categoryId As Guid
        If Not Guid.TryParse(Request.QueryString("CategoryID"), categoryId) Then
            ' handle invalid categoryID
            Return
        End If

        Using db As New OnlineShopContext()
            ' Get the selected category
            Dim mainCategory = db.Categories.Include("SubCategories").FirstOrDefault(Function(c) c.CategoryID = categoryId)
            If mainCategory Is Nothing Then Return

            ' Get IDs of main + subcategories
            Dim categoryIds As New List(Of Guid)
            categoryIds.Add(mainCategory.CategoryID)
            categoryIds.AddRange(mainCategory.SubCategories.Select(Function(sc) sc.CategoryID))

            ' Get products in any of these categories
            Dim products = db.Products.
                        Where(Function(p) categoryIds.Contains(p.CategoryID) And p.StockQuantity > 0 And p.IsActive = True).
                        Include("ProductImages").
                        ToList().
                        Select(Function(p)
                                   Dim mainImage = p.ProductImages.FirstOrDefault(Function(img) img.IsMain)
                                   Dim imgData As String = If(mainImage IsNot Nothing, mainImage.ImageData, "")
                                   Dim imgMime As String = If(mainImage IsNot Nothing, mainImage.MimeType, "image/png")

                                   Return New With {
                                       .ProductID = p.ProductID,
                                       .ProductName = p.ProductName,
                                       .Price = p.Price,
                                       .Description = p.Description,
                                       .ImageBase64 = imgData,
                                       .ImageMimeType = imgMime
                                   }
                               End Function).ToList()

            rptAllProducts.DataSource = products
            rptAllProducts.DataBind()
        End Using
    End Sub


    'Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
    '    If Not IsPostBack Then
    '        Using db As New OnlineShopContext()

    '            ' Check for CategoryID in query string
    '            Dim categoryIdStr As String = Request.QueryString("CategoryID")
    '            Dim productsQuery = db.Products.Include("ProductImages").AsQueryable()

    '            If Not String.IsNullOrEmpty(categoryIdStr) Then
    '                Dim categoryId As Guid
    '                If Guid.TryParse(categoryIdStr, categoryId) Then
    '                    ' Filter products by CategoryID
    '                    productsQuery = productsQuery.Where(Function(p) p.CategoryID = categoryId)
    '                End If
    '            End If

    '            Dim products = productsQuery.ToList()

    '            Dim productList = products.Select(Function(p)
    '                                                  Dim mainImage = p.ProductImages.FirstOrDefault(Function(i) i.IsMain)
    '                                                  Dim base64 As String

    '                                                  If mainImage IsNot Nothing AndAlso
    '                                                     mainImage.ImageData IsNot Nothing AndAlso
    '                                                     mainImage.MimeType IsNot Nothing Then

    '                                                      base64 = "data:" & mainImage.MimeType &
    '                                                               ";base64," & mainImage.ImageData
    '                                                  Else
    '                                                      ' fallback image if no main image exists
    '                                                      base64 = "Images/sample-product.png"
    '                                                  End If

    '                                                  Return New With {
    '                                                      .ProductID = p.ProductID,
    '                                                      .ProductName = p.ProductName,
    '                                                      .Price = p.Price,
    '                                                      .ImageBase64 = base64
    '                                                  }
    '                                              End Function).ToList()

    '            rptAllProducts.DataSource = productList
    '            rptAllProducts.DataBind()
    '        End Using
    '    End If
    'End Sub


End Class
