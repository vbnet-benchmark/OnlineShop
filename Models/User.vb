Imports System.ComponentModel.DataAnnotations
Imports System.Web.Mvc
Imports System.Web.Services.Description

Public Class User
    <Key>
    Public Property UserID as GUID

    <Required>
    <MaxLength(50)>
    Public Property Username As String

    <Required>
    <MaxLength(100)>
    Public Property Email As String

    <Required>
    <MaxLength(256)>
    Public Property PasswordHash As String

    <MaxLength(50)>
    Public Property FirstName As String

    <MaxLength(50)>
    Public Property LastName As String

    Public Property CreatedAt As DateTime = DateTime.Now
    Public Property LastLogin As DateTime?
    Public Property IsEmailVerified As Boolean
    Public Property VerificationToken As Guid

    ' The user's real, authoritative membership tier ("Standard" or "VIP"), set by account
    ' upgrade logic (billing, admin action, etc.) -- never by the storefront checkout pages.
    <MaxLength(20)>
    Public Property MembershipTier As String = "Standard"

    ' Navigation properties
    Public Overridable Property Shops As ICollection(Of Shop)
    Public Overridable Property Orders As ICollection(Of Order)
    Public Overridable Property Reviews As ICollection(Of Review)
    Public Overridable Property Favorites As ICollection(Of Favorite)
    Public Overridable Property SentMessages As ICollection(Of Message)
    Public Overridable Property Conversations As ICollection(Of Conversation)
End Class
