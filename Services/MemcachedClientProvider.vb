Imports Enyim.Caching
Imports Enyim.Caching.Configuration

''' <summary>
''' Shares a single EnyimMemcachedCore client across the whole site. A memcached connection pool is
''' meant to be opened once and reused -- unlike OnlineShopContext, which is deliberately opened
''' fresh per request/Using block -- so this is a simple shared instance rather than a per-call
''' construction.
'''
''' Server endpoint is hardcoded for now; a real deployment would read it from Web.config
''' appSettings instead (left as a follow-up, not needed for local/dev use).
''' </summary>
Public Module MemcachedClientProvider

    Private ReadOnly _client As IMemcachedClient = BuildClient()

    Private Function BuildClient() As IMemcachedClient
        Dim config As New MemcachedClientConfiguration()
        config.AddServer("localhost", 11211)
        Return New MemcachedClient(config)
    End Function

    Public ReadOnly Property Client As IMemcachedClient
        Get
            Return _client
        End Get
    End Property

End Module
