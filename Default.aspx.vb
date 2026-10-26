Imports System.Linq
Imports System.Data.Entity

Partial Class _Default
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            LoadHotProducts()
            LoadHotCategories()
            LoadHotShops()
        End If
    End Sub

    Private Sub LoadHotProducts()
        Using db As New OnlineShopContext()
            Dim products = db.Products.Include("ProductImages") _
                          .OrderByDescending(Function(p) p.ProductID).Take(20).ToList() _
                          .Where(Function(p) p.StockQuantity > 0 And p.IsActive = True) _
                          .Select(Function(p)
                                      Dim mainImg = p.ProductImages.FirstOrDefault(Function(img) img.IsMain)
                                      Return New With {
                                          .ProductID = p.ProductID,
                                          .ProductName = p.ProductName,
                                          .Price = p.Price,
                                          .ImageBase64 = If(mainImg IsNot Nothing, mainImg.ImageData, ""),
                                          .ImageMimeType = If(mainImg IsNot Nothing, mainImg.MimeType, "image/png")
                                      }
                                  End Function).ToList()

            If products.Any() Then
                pnlHotProducts.Visible = True
                rptHotProductsGroup.DataSource = products
                rptHotProductsGroup.DataBind()
            End If
        End Using
    End Sub

    Private Sub LoadHotCategories()
        Using db As New OnlineShopContext()
            Dim rootCategories = db.Categories _
                                 .Where(Function(c) c.ParentCategoryID Is Nothing AndAlso c.Products.Any()) _
                                 .ToList() _
                                 .Select(Function(c)
                                             Dim subCats = c.SubCategories.Where(Function(sc) sc.Products.Any()).ToList() _
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

            If rootCategories.Any() Then
                pnlHotCategories.Visible = True
                rptHotCategories.DataSource = rootCategories
                rptHotCategories.DataBind()
            End If
        End Using
    End Sub

    Private Sub LoadHotShops()
        Using db As New OnlineShopContext()
            Dim shops = db.Shops.Include("Owner") _
                        .OrderByDescending(Function(s) s.ShopID).Take(20).ToList() _
                        .Select(Function(s)
                                    Return New With {
                                        .ShopID = s.ShopID,
                                        .ShopName = s.ShopName,
                                        .LogoBase64 = If(String.IsNullOrEmpty(s.LogoBase64), "", s.LogoBase64),
                                        .LogoMimeType = If(String.IsNullOrEmpty(s.LogoMimeType), "image/png", s.LogoMimeType)
                                    }
                                End Function).ToList()

            If shops.Any() Then
                pnlHotShops.Visible = True
                rptHotShopsGroup.DataSource = shops
                rptHotShopsGroup.DataBind()
            End If
        End Using
    End Sub

End Class
