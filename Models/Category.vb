Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema

Public Class Category
    <Key>
    Public Property CategoryID As Guid

    <Required>
    <MaxLength(100)>
    Public Property CategoryName As String

    ' Optional parent category
    Public Property ParentCategoryID As Guid?

    Public Property ImageData As String
    <MaxLength(50)>
    Public Property MimeType As String

    ' Navigation properties
    <ForeignKey("ParentCategoryID")>
    Public Overridable Property ParentCategory As Category

    Public Overridable Property SubCategories As ICollection(Of Category)

    ' Products in this category
    Public Overridable Property Products As ICollection(Of Product)
End Class
