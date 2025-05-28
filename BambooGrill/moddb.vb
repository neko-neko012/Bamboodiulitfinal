Imports System.IO
Imports System.Text
Imports System.Security.Cryptography
Imports MySql.Data.MySqlClient

Module modDB
    Public myadocon As New MySqlConnection
    Public conn As New MySqlConnection
    Public cmd As New MySqlCommand
    Public cmdRead As MySqlDataReader

    Public db_server As String = "127.0.0.1"
    Public db_uid As String = "root"
    Public db_pwd As String = ""
    Public db_name As String = "bamboo_grill"
    Public strConnection As String = "server=" & db_server & ";uid=" & db_uid & ";password=" & db_pwd & ";database=" & db_name & ";" & "allowuservariables='True';"

    Public Structure LoggedUser
        Dim id As Integer
        Dim name As String
        Dim position As String
        Dim username As String
        Dim password As String
        Dim type As Integer
    End Structure

    Public CurrentLoggedUser As LoggedUser = Nothing


    Public Sub UpdateConnectionString()
        Try
            Dim config As String = System.IO.Directory.GetCurrentDirectory & "\config.txt"
            Dim text As String = Nothing
            If System.IO.File.Exists(config) Then
                Using reader As System.IO.StreamReader = New System.IO.StreamReader(config)

                    text = reader.ReadToEnd
                End Using
                Dim arr_text() As String = Split(text, vbCrLf)

                strConnection = "server=" & Split(arr_text(0), "=")(1) & ";uid=" & Split(arr_text(1), "=")(1) & ";password=" & Split(arr_text(2), "=")(1) & ";database=" & Split(arr_text(3), "=")(1) & ";" & "allowuservariables='True';"
            Else
                MsgBox("Do not exist")
            End If
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub



    Public Sub openConn(ByVal db_name As String)
        Try
            With conn
                If .State = ConnectionState.Open Then .Close()
                .ConnectionString = strConnection
                .Open()
            End With
        Catch EX As Exception
            MsgBox(EX.Message, MsgBoxStyle.Critical)
        End Try
    End Sub



    Public Sub readQuery(ByVal sql As String)
        Try
            openConn(db_name)
            With cmd
                .Connection = conn
                .CommandText = sql
                cmdRead = .ExecuteReader
            End With
        Catch EX As Exception
            MsgBox(EX.Message, MsgBoxStyle.Critical)
        End Try
    End Sub


    Public Function isConnectedToLocalServer() As Boolean
        Dim result As Boolean = False
        Try
            myadocon = New MySqlConnection
            myadocon.ConnectionString = strConnection
            Try
                myadocon.Open()
                If myadocon.State = ConnectionState.Open Then
                    result = True

                    MessageBox.Show("Successfully connected to server")
                Else
                    result = False
                    MessageBox.Show("Fail to connect to server")
                End If
            Catch ex As Exception
                Return False
            End Try
            ' If myadocon.State = ConnectionState.Open Then
            ' myadocon.Close()
            ' End If
        Catch
            Return False
        End Try
        Return result
    End Function

    Function isConnectedToDatabase() As Boolean
        Dim result As Boolean = False
        Try

            Dim myadocon As New MySqlConnection()
            myadocon.ConnectionString = strConnection

            Try
                myadocon.Open()


                If myadocon.State = ConnectionState.Open Then
                    result = True

                    MessageBox.Show("Successfully connected to the database!", "Connection Status", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Else
                    result = False

                    MessageBox.Show("Failed to connect to the database.", "Connection Status", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            Catch ex As MySqlException

                result = False

                MessageBox.Show("Error: " & ex.Message, "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Finally

                If myadocon.State = ConnectionState.Open Then
                    myadocon.Close()
                End If
            End Try
        Catch
            result = False
            MessageBox.Show("Unexpected error occurred while connecting to the database.", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

        Return result
    End Function




    Function LoadToDGV(ByVal query As String, ByVal dgv As DataGridView) As Integer
        Try
            readQuery(query)
            Dim dt As DataTable = New DataTable
            dt.Load(cmdRead)

            dgv.Columns.Clear() ' Clear existing columns
            dgv.AutoGenerateColumns = True ' Allow alias-based headers
            dgv.DataSource = dt
            dgv.Refresh()

            Return dgv.Rows.Count
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical)
        End Try
        Return 0
    End Function




    Public Function Encrypt(ByVal clearText As String) As String
        Dim EncryptionKey As String = "MAKV2SPBNI99212"
        Dim clearBytes As Byte() = Encoding.Unicode.GetBytes(clearText)
        Using encryptor As Aes = Aes.Create()
            Dim pdb As New Rfc2898DeriveBytes(EncryptionKey, New Byte() {&H49, &H76, &H61, &H6E, &H20, &H4D, &H65, &H64, &H76, &H65, &H64, &H65, &H76})
            encryptor.Key = pdb.GetBytes(32)
            encryptor.IV = pdb.GetBytes(16)
            Using ms As New MemoryStream()
                Using cs As New CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write)
                    cs.Write(clearBytes, 0, clearBytes.Length)
                    cs.Close()
                End Using
                clearText = Convert.ToBase64String(ms.ToArray())
            End Using
        End Using
        Return clearText
    End Function

    ' Decryption function (if needed)
    Public Function Decrypt(ByVal cipherText As String) As String
        Dim EncryptionKey As String = "MAKV2SPBNI99212"
        Dim cipherBytes As Byte() = Convert.FromBase64String(cipherText)
        Using encryptor As Aes = Aes.Create()
            Dim pdb As New Rfc2898DeriveBytes(EncryptionKey, New Byte() {&H49, &H76, &H61, &H6E, &H20, &H4D, &H65, &H64, &H76, &H65, &H64, &H65, &H76})
            encryptor.Key = pdb.GetBytes(32)
            encryptor.IV = pdb.GetBytes(16)
            Using ms As New MemoryStream()
                Using cs As New CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write)
                    cs.Write(cipherBytes, 0, cipherBytes.Length)
                    cs.Close()
                End Using
                cipherText = Encoding.Unicode.GetString(ms.ToArray())
            End Using
        End Using
        Return cipherText
    End Function

    ' use for logs 
    Sub Logs(ByVal transaction As String, Optional ByVal events As String = "*_Click")
        Try
            Using conn As New MySqlConnection(strConnection)
                If conn.State <> ConnectionState.Open Then
                    conn.Open()
                End If
                If conn.State = ConnectionState.Open Then
                    Dim query As String = String.Format("INSERT INTO logs (dt, user_accounts_id, event, transactions) VALUES (NOW(), {0}, '{1}', '{2}')",
                                                    CurrentLoggedUser.id,
                                                    events,
                                                    transaction)
                    Using cmd As New MySqlCommand(query, conn)
                        cmd.ExecuteNonQuery()
                    End Using
                Else
                    MessageBox.Show("Database connection is not valid or open.", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show("Error occurred while logging: " & ex.Message, "Logging Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub










End Module
