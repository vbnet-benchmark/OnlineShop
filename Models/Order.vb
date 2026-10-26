Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema

Public Class Order
    <Key>
    Public Property OrderID As Guid

    <Required>
    Public Property UserID as GUID

    Public Property OrderDate As DateTime = DateTime.Now
    Public Property TotalAmount As Decimal
    <MaxLength(50)>
    Public Property Status As String = "Pending"

    ' Navigation properties
    <ForeignKey("UserID")>
    Public Overridable Property User As User
    Public Overridable Property OrderItems As ICollection(Of OrderItem)
    Public Overridable Property Payments As ICollection(Of Payment)
End Class