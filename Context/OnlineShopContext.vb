Imports System.Data.Entity

Public Class OnlineShopContext
    Inherits DbContext

    Public Sub New()
        MyBase.New("name=OnlineShop")
    End Sub

    Public Property Users As DbSet(Of User)
    Public Property Shops As DbSet(Of Shop)
    Public Property Categories As DbSet(Of Category)
    Public Property Products As DbSet(Of Product)
    Public Property ProductImages As DbSet(Of ProductImage)
    Public Property Orders As DbSet(Of Order)
    Public Property OrderItems As DbSet(Of OrderItem)
    Public Property Reviews As DbSet(Of Review)
    Public Property Payments As DbSet(Of Payment)
    Public Property Favorites As DbSet(Of Favorite)
    Public Property Conversations As DbSet(Of Conversation)
    Public Property Messages As DbSet(Of Message)
    Public Property UserProductViews As DbSet(Of UserProductView)
    Public Property UserCartItems As DbSet(Of CartItem)

End Class
