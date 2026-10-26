Imports System
Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema

Public Class UserProductView

    <Key>
    Public Property ViewID As Integer

    Public Property UserID As Guid

    Public Property ProductID As Guid

    <DatabaseGenerated(DatabaseGeneratedOption.Computed)>
    Public Property ViewedAt As DateTime = DateTime.Now

End Class
