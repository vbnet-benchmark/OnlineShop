Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema

Public Class Shop
    <Key>
    Public Property ShopID As Guid

    <Required>
    Public Property UserID as GUID

    <Required>
    <MaxLength(100)>
    Public Property ShopName As String

    <MaxLength(1000)>
    Public Property Description As String

    Public Property CreatedAt As DateTime = DateTime.Now

    ' Customization properties
    Public Property LogoBase64 As String
    <MaxLength(50)>
    Public Property LogoMimeType As String

    Public Property BannerBase64 As String

    <MaxLength(50)>
    Public Property BannerMimeType As String
    <MaxLength(7)>
    Public Property ThemeColor As String

    Public Property IsActive As Boolean
    ' Navigation properties
    <ForeignKey("UserID")>
    Public Overridable Property Owner As User
    Public Overridable Property Products As ICollection(Of Product)
End Class