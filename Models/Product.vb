Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema
Imports System.Web.Mvc

Public Class Product
    <Key>
    Public Property ProductID As Guid

    <Required>
    Public Property ShopID As Guid

    <Required>
    Public Property CategoryID as GUID

    <Required>
    <MaxLength(150)>
    Public Property ProductName As String

    Public Property Description As String
    Public Property Price As Decimal
    Public Property StockQuantity As Integer
    Public Property Threshold As Integer
    Public Property CreatedAt As DateTime = DateTime.Now
    Public Property UpdatedAt As DateTime?
    Public Property IsActive As Boolean

    ' Navigation properties
    <ForeignKey("ShopID")>
    Public Overridable Property Shop As Shop

    <ForeignKey("CategoryID")>
    Public Overridable Property Category As Category

    Public Overridable Property ProductImages As ICollection(Of ProductImage)
    Public Overridable Property OrderItems As ICollection(Of OrderItem)
    Public Overridable Property Reviews As ICollection(Of Review)
    Public Overridable Property Favorites As ICollection(Of Favorite)
End Class
