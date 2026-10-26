Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema

Public Class Message
    <Key>
    Public Property MessageID As Guid

    <Required>
    Public Property ConversationID As Guid

    <Required>
    Public Property SenderID As Guid

    Public Property MessageText As String
    Public Property SentAt As DateTime = DateTime.Now

    ' Navigation properties
    <ForeignKey("ConversationID")>
    Public Overridable Property Conversation As Conversation

    <ForeignKey("SenderID")>
    Public Overridable Property Sender As User
End Class