Imports System.IO
Imports MySql.Data.MySqlClient
Imports System.Security.Cryptography
Imports System.Text

Public Class Form1
    ' Database variables
    Private keySequence As New List(Of Keys)
    Private targetSequence As Keys() = {Keys.ShiftKey Or Keys.Up, Keys.ShiftKey Or Keys.Up, Keys.Down, Keys.Down}
    Private lastKeyTime As DateTime = DateTime.Now
    Private sequenceTimeout As TimeSpan = TimeSpan.FromSeconds(3) ' reset if too slow

    ' Form Load

    Public Function EncryptPassword(ByVal plainText As String) As String
        Using sha256 As SHA256 = SHA256.Create()
            Dim bytes As Byte() = Encoding.UTF8.GetBytes(plainText)
            Dim hashBytes As Byte() = sha256.ComputeHash(bytes)
            Dim sb As New StringBuilder()
            For Each b In hashBytes
                sb.Append(b.ToString("x2"))
            Next
            Return sb.ToString()
        End Using
    End Function
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.WindowState = FormWindowState.Maximized

        ' Set the initial visibility of the panels
        loginpanel.Visible = True
        registerpanel.Visible = False
        ConfigPanel.Visible = False
        openConn(db_name) ' From modDB


        Try
            Dim sql As String = "SELECT COUNT(*) FROM users"
            Dim userCount As Integer = 0

            If conn IsNot Nothing AndAlso conn.State = ConnectionState.Open Then
                ' Get the number of users
                cmd = New MySqlCommand(sql, conn)
                userCount = Convert.ToInt32(cmd.ExecuteScalar())

                If userCount = 0 Then
                    ' No users found → Show Register Panel
                    MessageBox.Show("No admin found! Please register an Admin first.", "Setup Required", MessageBoxButtons.OK, MessageBoxIcon.Information)

                    registerpanel.Visible = True
                    registerpanel.BringToFront()

                    ' Fill ComboBox with roles
                    Guna2ComboBox1.Items.Clear()
                    Guna2ComboBox1.Items.AddRange(New Object() {"Admin", "Cashier", "Staff"})
                    Guna2ComboBox1.SelectedIndex = 0

                    ' Optional: Restrict registration to Admin only
                    AddHandler Guna2ComboBox1.SelectedIndexChanged, AddressOf OnlyAdminSelectable
                Else
                    ' Users found → Show Login Panel
                    loginpanel.Visible = True
                    loginpanel.BringToFront()
                End If
            Else
                MessageBox.Show("Failed to connect to the database.", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If

        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If conn IsNot Nothing AndAlso conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
    End Sub




    Private Sub OnlyAdminSelectable(sender As Object, e As EventArgs)
        ' Check if ComboBox selected item is not Nothing
        If Guna2ComboBox1.SelectedItem Is Nothing OrElse Guna2ComboBox1.SelectedItem.ToString() <> "Admin" Then
            MessageBox.Show("You must register as Admin first!", "Role Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Guna2ComboBox1.SelectedIndex = 0 ' Force back to Admin
        End If
    End Sub

    ' Register button click handler
    Private Sub registerbtn_Click(sender As Object, e As EventArgs) Handles registerbtn.Click
        ' Validate fields
        If String.IsNullOrWhiteSpace(firstnametxtbx.Text) OrElse
           String.IsNullOrWhiteSpace(lastnametxtbx.Text) OrElse
           String.IsNullOrWhiteSpace(registerusernametxtbx.Text) OrElse
           String.IsNullOrWhiteSpace(registerpasswordtxtbx.Text) OrElse
           Guna2ComboBox1.SelectedItem Is Nothing Then

            MessageBox.Show("Please fill out all fields!", "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        ' Prepare data
        Dim firstName As String = firstnametxtbx.Text.Trim()
        Dim lastName As String = lastnametxtbx.Text.Trim()
        Dim username As String = registerusernametxtbx.Text.Trim()
        Dim password As String = EncryptPassword(registerpasswordtxtbx.Text.Trim())
        Dim role As String = Guna2ComboBox1.SelectedItem.ToString()
        Dim dateCreated As String = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")

        ' Connect to DB
        openConn(db_name)

        ' Insert user
        Dim sql As String = "INSERT INTO users (FirstName, LastName, Username, Password, Role, DateCreated) VALUES (@FirstName, @LastName, @Username, @Password, @Role, @DateCreated)"

        Try
            If conn IsNot Nothing AndAlso conn.State = ConnectionState.Open Then
                cmd = New MySqlCommand(sql, conn)
                cmd.Parameters.Clear()
                cmd.Parameters.AddWithValue("@FirstName", firstName)
                cmd.Parameters.AddWithValue("@LastName", lastName)
                cmd.Parameters.AddWithValue("@Username", username)
                cmd.Parameters.AddWithValue("@Password", password)  ' Storing the password as it is, no hashing
                cmd.Parameters.AddWithValue("@Role", role)
                cmd.Parameters.AddWithValue("@DateCreated", dateCreated)

                Dim result As Integer = cmd.ExecuteNonQuery()

                If result > 0 Then
                    MessageBox.Show("Registration successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

                    ' After registration, go back to login
                    registerpanel.Visible = False
                    loginpanel.Visible = True

                    ' Clear fields
                    firstnametxtbx.Clear()
                    lastnametxtbx.Clear()
                    registerusernametxtbx.Clear()
                    registerpasswordtxtbx.Clear()
                    Guna2ComboBox1.SelectedIndex = -1
                Else
                    MessageBox.Show("Registration failed!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            Else
                MessageBox.Show("Connection to database failed during registration!")
            End If

        Catch ex As Exception
            MessageBox.Show("Error during registration: " & ex.Message)
        Finally
            If conn IsNot Nothing AndAlso conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
    End Sub

    Private loggedInFirstName As String
    Private loggedInLastName As String
    Private Sub Loginbtn_Click(sender As Object, e As EventArgs) Handles Loginbtn.Click
        ' Validate login fields
        If String.IsNullOrWhiteSpace(usernametxtbx.Text) OrElse String.IsNullOrWhiteSpace(passwordtxtbx.Text) Then
            MessageBox.Show("Please enter both username and password!", "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        Dim username As String = usernametxtbx.Text.Trim()
        Dim inputPassword As String = EncryptPassword(passwordtxtbx.Text.Trim())

        ' Connect to DB
        openConn(db_name)

        Dim sql As String = "SELECT FirstName, LastName, Role FROM users WHERE Username = @Username AND Password = @Password"

        Try
            If conn IsNot Nothing AndAlso conn.State = ConnectionState.Open Then
                cmd = New MySqlCommand(sql, conn)
                cmd.Parameters.Clear()
                cmd.Parameters.AddWithValue("@Username", username)
                cmd.Parameters.AddWithValue("@Password", inputPassword)  ' In production, always use hashing!

                Dim dr As MySqlDataReader = cmd.ExecuteReader()

                If dr.Read() Then
                    ' Store logged-in user information
                    loggedInFirstName = dr("FirstName").ToString()
                    loggedInLastName = dr("LastName").ToString()
                    Dim userRole As String = dr("Role").ToString()

                    ' Call Dashboard form and pass first name, last name, and role
                    Dim dashboardForm As New Dashboard(loggedInFirstName, loggedInLastName, userRole)

                    MessageBox.Show("Login Successful!", "Welcome", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Me.Hide()
                    dashboardForm.Show()
                Else
                    MessageBox.Show("Incorrect Username or Password!", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            Else
                MessageBox.Show("Connection to database failed during login!")
            End If

        Catch ex As Exception
            MessageBox.Show("Error during login: " & ex.Message)
        Finally
            If conn IsNot Nothing AndAlso conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
    End Sub
    Private Sub configcheckconnbtn_Click(sender As Object, e As EventArgs) Handles configcheckconnbtn.Click
        Dim testConnStr As String = $"server={configservertextbox.Text};uid={configusernametextbox.Text};password={configpasswordtextbox.Text};database={configdatabasenametextbox.Text};allowuservariables=True;"

        Try
            Using testConn As New MySqlConnection(testConnStr)
                testConn.Open()
                If testConn.State = ConnectionState.Open Then
                    MessageBox.Show("✅ Connection successful!", "Connection Test", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show("❌ Connection failed: " & ex.Message, "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Private Sub configupdateconnbtn_Click(sender As Object, e As EventArgs) Handles configupdateconnbtn.Click
        Dim configPath As String = Path.Combine(Application.StartupPath, "config.txt")

        Try
            ' Save to config.txt
            Dim lines As New List(Of String) From {
            "server=" & configservertextbox.Text,
            "uid=" & configusernametextbox.Text,
            "password=" & configpasswordtextbox.Text,
            "database=" & configdatabasenametextbox.Text
        }

            File.WriteAllLines(configPath, lines)

            ' Update global connection string in modDB
            modDB.strConnection = $"server={configservertextbox.Text};uid={configusernametextbox.Text};password={configpasswordtextbox.Text};database={configdatabasenametextbox.Text};allowuservariables=True;"

            MessageBox.Show("✅ Connection settings updated successfully.", "Updated", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show("❌ Failed to update config: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub



    Protected Overrides Function ProcessCmdKey(ByRef msg As Message, keyData As Keys) As Boolean
        If keyData = (Keys.Shift Or Keys.C) Then
            ConfigPanel.Visible = True

            Return True ' prevent further handling if needed
        End If
        If keyData = (Keys.Shift Or Keys.X) Then
            ConfigPanel.Visible = False

            Return True ' prevent further handling if needed
        End If

        Return MyBase.ProcessCmdKey(msg, keyData)
    End Function
    'new user



End Class
