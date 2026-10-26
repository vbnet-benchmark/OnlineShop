Imports System.Linq
Imports System.Data.Entity

Public Class Categories
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Using db As New OnlineShopContext()
                ' Fetch root categories that have products
                Dim categories = db.Categories.
                    Where(Function(c) c.ParentCategoryID Is Nothing AndAlso
                                      c.Products.Any()).
                    ToList().
                    Select(Function(c)
                               ' Fetch subcategories that have products
                               Dim subCats = c.SubCategories.
                                             Where(Function(sc) sc.Products.Any()).ToList().
                                             Select(Function(sc) New With {
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

                rptCategories.DataSource = categories
                rptCategories.DataBind()
            End Using
        End If
    End Sub
End Class
