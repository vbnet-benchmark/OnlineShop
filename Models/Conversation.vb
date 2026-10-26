Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema
Imports System.Web.Services.Description

Public Class Conversation
    <Key>
    Public Property ConversationID As Guid

    <Required>
    Public Property User1ID As Guid

    <Required>
    Public Property User2ID As Guid

    Public Property CreatedAt As DateTime = DateTime.Now

    ' Navigation properties
    <ForeignKey("User1ID")>
    Public Overridable Property User1 As User

    <ForeignKey("User2ID")>
    Public Overridable Property User2 As User

    Public Overridable Property Messages As ICollection(Of Message)
End Class