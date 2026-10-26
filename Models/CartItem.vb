Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema
Public Class CartItem
    <Key>
    Public Property CartItemID As Integer
    Public Property UserID As Guid
    Public Property ProductID As Guid
    Public Property Quantity As Integer
    Public Property DateAdded As DateTime
    ' Navigation property
    Public Overridable Property Product As Product
End Class
