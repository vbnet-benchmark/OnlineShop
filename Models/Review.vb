Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema

Public Class Review
    <Key>
    Public Property ReviewID As Guid

    <Required>
    Public Property ProductID As Guid

    <Required>
    Public Property UserID as GUID

    <Required>
    <Range(1, 5)>
    Public Property Rating As Integer

    <MaxLength(1000)>
    Public Property Comment As String

    Public Property CreatedAt As DateTime = DateTime.Now

    ' Navigation properties
    <ForeignKey("ProductID")>
    Public Overridable Property Product As Product

    <ForeignKey("UserID")>
    Public Overridable Property User As User
End Class