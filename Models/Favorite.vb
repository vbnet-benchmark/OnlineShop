Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema

Public Class Favorite
    <Key>
    Public Property FavoriteID As Guid

    <Required>
    Public Property UserID As Guid

    <Required>
    Public Property ProductID As Guid

    Public Property CreatedAt As DateTime = DateTime.Now

    ' Navigation properties
    <ForeignKey("UserID")>
    Public Overridable Property User As User

    <ForeignKey("ProductID")>
    Public Overridable Property Product As Product
End Class