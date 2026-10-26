Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema

Public Class ProductImage
    <Key>
    Public Property ImageID As Guid

    <Required>
    Public Property ProductID As Guid

    <Required>
    Public Property ImageData As String
    <MaxLength(50)>
    Public Property MimeType As String

    Public Property IsMain As Boolean = False

    ' Navigation property
    <ForeignKey("ProductID")>
    Public Overridable Property Product As Product
End Class