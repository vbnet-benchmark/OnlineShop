Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema

Public Class Payment
    <Key>
    Public Property PaymentID As Guid

    <Required>
    Public Property OrderID as GUID

    Public Property PaymentDate As DateTime = DateTime.Now
    Public Property Amount As Decimal
    <MaxLength(50)>
    Public Property PaymentMethod As String
    <MaxLength(50)>
    Public Property Status As String = "Completed"

    ' Navigation property
    <ForeignKey("OrderID")>
    Public Overridable Property Order As Order
End Class