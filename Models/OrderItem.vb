Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema

Public Class OrderItem
    <Key>
    Public Property OrderItemID As Integer

    <Required>
    Public Property OrderID As Guid

    <Required>
    Public Property ProductID As Guid

    Public Property Quantity As Integer
    Public Property UnitPrice As Decimal

    ' Navigation properties
    <ForeignKey("OrderID")>
    Public Overridable Property Order As Order

    <ForeignKey("ProductID")>
    Public Overridable Property Product As Product
End Class