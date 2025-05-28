Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Drawing.Printing
Imports System.IO
Imports System.Security.Cryptography
Imports System.Text
Imports System.Windows.Forms.VisualStyles.VisualStyleElement.Header
Imports BambooGrill.modDB
Imports Guna.UI.WinForms
Imports Guna.UI2.WinForms
Imports MySql.Data.MySqlClient
Imports PdfSharp.Drawing
Imports PdfSharp.Pdf

Public Class Dashboard


    Private WithEvents timer As New Timer()
    Public Property UserFirstName As String
    Public Property UserLastName As String
    Public Property CurrentUserRole As String
    Private WithEvents reloadTimer As New Timer()
    Private currentDisplayedOrders As New List(Of String) ' Optional: Track displayed tableNumbers
    ' Declare the panel at form level
    Private WithEvents printPanel As New Panel()
    Private employeeBitmap As Bitmap ' Used for printing
    Private inventoryBitmap As Bitmap

    Public Sub New(firstName As String, lastName As String, role As String)


        ' Store the passed-in values
        Me.UserFirstName = firstName
        Me.UserLastName = lastName
        Me.CurrentUserRole = role
        InitializeComponent()



        ' Check role and hide buttons if cashier

    End Sub

    Private Sub Dashboard_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        timer.Interval = 1000 ' 1000 milliseconds = 1 second
        reloadTimer.Interval = 3000 ' 3 seconds
        reloadTimer.Start()
        settingsrolecmbbx.Items.Clear()
        settingsrolecmbbx.Items.AddRange(New Object() {"Admin", "Cashier", "Staff"})
        settingsrolecmbbx.SelectedIndex = 0
        If Me.CurrentUserRole.ToLower() = "cashier" Then
            TabControl1.TabPages.Remove(dashboardnbtn)
            TabControl1.TabPages.Remove(employeebtn)
            TabControl1.TabPages.Remove(settingsbtn)

            ' Optional: hide tab pages from TabControl1 if needed
            'TabControl1.TabPages.Remove(TabPageDashboard)
            ' TabControl1.TabPages.Remove(TabPageEmployee)
            ' TabControl1.TabPages.Remove(TabPageSettings)
        End If
        If Me.CurrentUserRole.ToLower() = "staff" Then
            TabControl1.TabPages.Remove(dashboardnbtn)
            TabControl1.TabPages.Remove(employeebtn)
            TabControl1.TabPages.Remove(customerbtn)
            TabControl1.TabPages.Remove(salesbtn)
            TabControl1.TabPages.Remove(settingsbtn)
            TabControl1.TabPages.Remove(takeorderbtn)
            TabControl1.TabPages.Remove(inventorybtn)
            TabControl1.TabPages.Remove(expensesbtn)
            ' Optional: hide tab pages from TabControl1 if needed
            'TabControl1.TabPages.Remove(TabPageDashboard)
            ' TabControl1.TabPages.Remove(TabPageEmployee)
            ' TabControl1.TabPages.Remove(TabPageSettings)
        End If




        TOhitainasalbtn.Tag = "hita inasal"
        TOpitsoinasalbtn.Tag = "pitso inasal"
        TOliempoinasalbtn.Tag = "liempo inasal"
        TOporkchopbtn.Tag = "porkchop inasal"
        TObbqinasalbtn.Tag = "bbq inasal"
        TOfriedchckbtn.Tag = "fried chicken"
        TObutteredchckbtn.Tag = "buttered Chicken"
        TOchckenteribtn.Tag = "chicken teriyaki"
        TOchckenparmbtn.Tag = "chicken parmessan"
        TOlumpiabtn.Tag = "lumpia"
        TOchopseuybtn.Tag = "chopseuy"
        TOlechonkwlbtn.Tag = "L. kawali"
        TOsisigbtn.Tag = "sisig"

        TOwater350btn.Tag = "water 350ml"
        Towater500btn.Tag = "water 500ml"
        TOcokemismobtn.Tag = "coke mismo"
        TOroyalmismobtn.Tag = "royal mismo"
        TOmdewmismobtn.Tag = "mountain dew mismo"
        TOspritemismobtn.Tag = "sprite mismo"
        TOcoke15btn.Tag = "coke 1.5"
        TOroyal15btn.Tag = "royal 1.5"
        TOsprite15btn.Tag = "sprite 1.5"
        TOsmgpilsenbtn.Tag = "san miguel pale pilsen"
        TOrh500btn.Tag = "red horse 500ml"
        TOhalohalombtn.Tag = "halo halo small"
        TOhalohalolbtn.Tag = "halo halo large"
        TOchckenteriplatterbtn.Tag = "chicken teriyaki platter"
        TOchckenparmplatterbtn.Tag = "chicken parmesan platter"
        TObttrdchckplatterbtn.Tag = "buttered chicken platter"
        TOsisigplatterbtn.Tag = "sisig platter"
        TOchopseuyplatterbtn.Tag = "chopseuy platter"
        TOlumpiaplatterbtn.Tag = "lumpia platter"
        TOpancitsmallbtn.Tag = "pancit guisado"
        TOcreamycarbbtn.Tag = "carbonara"

        TObenguetblendbtn.Tag = "benguet blend"
        TObarakobtn.Tag = "barako"
        TOkalingarbstabtn.Tag = "kalinga robusta"
        TOfrenchrstbtn.Tag = "french roast"
        TOitalianespblendbtn.Tag = "italian espresso blend"
        TOhouseblndarabicabtn.Tag = "house blend arabica"
        TObenguetarabicabtn.Tag = "benguet arabica"
        TOpremiumbrkbtn.Tag = "premium barako"
        TOsagadaarabicabtn.Tag = "sagada arabica"
        TOprembenguetarabcabtn.Tag = "premium  benguet arabica"
        TObutterscotchbtn.Tag = "butterscotch"
        TOcaramelbtn.Tag = "caramel"
        TOcinnamonbtn.Tag = "cinnamon"
        TOdoublechocobtn.Tag = "double choco"
        TOhazelnutbtn.Tag = "hazelnut"
        TOirishcreambtn.Tag = "irish cream"
        TOmacadamiabtn.Tag = "macademia"
        TOmocha.Tag = "mocha"
        TOvanillabtn.Tag = "vanilla"
        TOhazelnutvanillabtn.Tag = "hazelnut vanilla"
        TOcookiesandcreambtn.Tag = "cookies and cream"
        TObaileysbtn.Tag = "baileys"



        timer.Start()  ' Start the timer

        ' Display the current time and date when the form loads
        Userlabelplchlder.Text = UserFirstName & " " & UserLastName


        LoadTotalCustomers()
        LoadTotalSales()
        LoadTotalEmployees()
        LoadTotalExpenses()


        dbsalescombobox.Items.AddRange({"This Day", "This Week", "This Month", "This Year"})
        dbsalescombobox.SelectedIndex = 0  ' Set default filter option to "This Day"


        dbsaleschartcmbbx.Items.AddRange({"This Day", "This Week", "This Month", "This Year"})
        dbsaleschartcmbbx.SelectedIndex = 0

        ' Load all sales data by default
        LoadAllSalesData()
        SetupDataGridView()

        'expenses cmbbx
        expcmbbx.Items.Clear()
        expcmbbx.Items.AddRange(New String() {"Day", "Week", "Month", "Year"})
        expcmbbx.SelectedIndex = 0 ' This ensures SelectedItem is never Nothing
        LoadExpenses()

        TOpaymentmethodcmbbx.Items.Clear()
        TOpaymentmethodcmbbx.Items.Add("Cash")
        TOpaymentmethodcmbbx.Items.Add("Gcash")

        ' Populate Transaction Type
        TOtransactiontypecmbbx.Items.Clear()
        TOtransactiontypecmbbx.Items.Add("Dine-in")
        TOtransactiontypecmbbx.Items.Add("Take out")

        'mga read only dyan
        empdgv.ReadOnly = True
        salesdgv.ReadOnly = True
        invdgv.ReadOnly = True
        cusdgv.ReadOnly = True
        expdgv.ReadOnly = True
        TOreceiptdgv.ReadOnly = True
        userdgv.ReadOnly = True


        expunitofmeasurecmbbx.Items.Clear()
        exptranstypecmbbx.Items.Clear()

        ' Populate Unit of Measure ComboBox
        Dim units As String() = {"kg", "g", "Liters", "ml", "gallon", "pcs"}
        expunitofmeasurecmbbx.Items.AddRange(units)

        ' Populate Transaction Type ComboBox
        Dim paymentTypes As String() = {"Cash", "GCash"}
        exptranstypecmbbx.Items.AddRange(paymentTypes)
        PopulateExpenseUpdateComboBoxes()


        EmployeeAddPanel.Visible = False
        ExpensesAddPanel.Visible = False
        InventoryAddPanel.Visible = False

        CustomerUpdatePanel.Visible = False
        EmpUpdatePanel.Visible = False
        InventoryUpdatePanel.Visible = False
        ExpensesUpdatePanel.Visible = False

        LoadAllSalesData()


        'dashboard ng pinas
        Me.WindowState = FormWindowState.Maximized
    End Sub


    Private Sub dbsaleschartcmbbx_SelectedIndexChanged(sender As Object, e As EventArgs)
        LoadSalesChart()
    End Sub


    Private Sub LoadSalesChart()
        Try
            Dim filter As String = dbsaleschartcmbbx.SelectedItem.ToString()
            Dim dateCondition As String = ""

            Select Case filter
                Case "This Day"
                    dateCondition = "WHERE DATE(DateOrdered) = CURDATE()"
                Case "This Week"
                    dateCondition = "WHERE YEARWEEK(DateOrdered, 1) = YEARWEEK(CURDATE(), 1)"
                Case "This Month"
                    dateCondition = "WHERE MONTH(DateOrdered) = MONTH(CURDATE()) AND YEAR(DateOrdered) = YEAR(CURDATE())"
                Case "This Year"
                    dateCondition = "WHERE YEAR(DateOrdered) = YEAR(CURDATE())"
            End Select

            Dim query As String = "
            SELECT 
                DATE(DateOrdered) AS SaleDate,
                SUM(NetAmount) AS TotalSales
            FROM sales
            " & dateCondition & "
            GROUP BY SaleDate
            ORDER BY SaleDate ASC
        "

            Using connection As New MySqlConnection(conn.ConnectionString)
                connection.Open()

                Dim cmd As New MySqlCommand(query, connection)
                Dim reader As MySqlDataReader = cmd.ExecuteReader()

                ' Clear previous data
                dbsaleschart.Series.Clear()
                dbsaleschart.Series.Add("Sales")
                dbsaleschart.Series("Sales").ChartType = DataVisualization.Charting.SeriesChartType.Column ' Or Line

                While reader.Read()
                    Dim dateVal As Date = Convert.ToDateTime(reader("SaleDate"))
                    Dim total As Decimal = Convert.ToDecimal(reader("TotalSales"))

                    dbsaleschart.Series("Sales").Points.AddXY(dateVal.ToString("MM-dd"), total)
                End While
            End Using
        Catch ex As Exception
            MessageBox.Show("Error loading chart data: " & ex.Message)
        End Try
    End Sub

    ' In Dashboard form
    Public Sub UpdateUserLabel(firstName As String, lastName As String)
        Userlabelplchlder.Text = firstName & " " & lastName
    End Sub

    Private Sub SetupDataGridView()
        With dbrecentsalestable
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            .ColumnHeadersVisible = True
            .RowHeadersVisible = False
            .AllowUserToResizeRows = False
            .AllowUserToResizeColumns = False
            .ReadOnly = True
        End With
    End Sub

    ' This method will be triggered when the ComboBox selection changes
    Private Sub dbsalescombobox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles dbsalescombobox.SelectedIndexChanged
        FilterSalesData() ' Filter data based on ComboBox selection
    End Sub

    Private Sub LoadAllSalesData()
        Dim query As String = "
        SELECT 
            SID AS 'SaleID', 
            CID AS 'CustomerID',
            ReceiptNumber AS 'Receipt#',
            TimeOrdered AS 'Time',
            DateOrdered AS 'Date',
            ItemsOrdered AS 'Items',
            QTY AS 'Quantity',
            SubTotal AS 'Subtotal',
            DiscountAmount AS 'Discount',
            NetAmount AS 'Total',
            PaymentMethod AS 'Payment Method',
            TransactionType AS 'Transaction Type',
            OrderStatus AS 'Order Status',
            RefundStatus AS 'Refund',
            DateRecorded AS 'Date Recorded',
            ProcessedBy AS 'Processed By',
            Notes AS 'Notes'
        FROM sales 
        ORDER BY DateRecorded DESC
        LIMIT 10"

        LoadToDGV(query, dbrecentsalestable)
        With dbrecentsalestable
            .ColumnHeadersHeight = 50 ' Adjust height
            .ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True
            .AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells
            .DefaultCellStyle.WrapMode = DataGridViewTriState.True
            .RowTemplate.Height = 30 ' Optional, for data rows
            .RowHeadersWidth = 70


        End With


    End Sub

    Private Sub FilterSalesData()
        ' Get the selected filter value
        Dim filter As String = dbsalescombobox.SelectedItem.ToString()
        Dim dateCondition As String = ""

        ' Determine the query condition based on the selected filter
        Select Case filter
            Case "This Day"
                dateCondition = "WHERE DATE(DateOrdered) = CURDATE()"
            Case "This Week"
                dateCondition = "WHERE YEARWEEK(DateOrdered, 1) = YEARWEEK(CURDATE(), 1)"
            Case "This Month"
                dateCondition = "WHERE MONTH(DateOrdered) = MONTH(CURDATE()) AND YEAR(DateOrdered) = YEAR(CURDATE())"
            Case "This Year"
                dateCondition = "WHERE YEAR(DateOrdered) = YEAR(CURDATE())"
            Case Else
                ' If filter is "Cancel" or any custom filter, load all data
                dateCondition = ""
        End Select

        ' Create the query with the selected filter
        Dim query As String = "
        SELECT 
            SID AS 'SaleID', 
            CID AS 'CustomerID',
            ReceiptNumber AS 'Receipt#',
            TimeOrdered AS 'Time',
            DateOrdered AS 'Date',
            ItemsOrdered AS 'Items',
            QTY AS 'Quantity',
            SubTotal AS 'Subtotal',
            DiscountAmount AS 'Discount',
            NetAmount AS 'Total',
            PaymentMethod AS 'Payment Method',
            TransactionType AS 'Transaction Type',
            OrderStatus AS 'Order Status',
            RefundStatus AS 'Refund',
            DateRecorded AS 'Date Recorded',
            ProcessedBy AS 'Processed By',
            Notes AS 'Notes'
        FROM sales " & dateCondition & "
        ORDER BY DateRecorded DESC
        LIMIT 10"

        ' Load the filtered data into the DataGridView
        LoadToDGV(query, dbrecentsalestable)
    End Sub

    ' Method to update time and date


    Private Sub Timer_Tick(sender As Object, e As EventArgs) Handles timer.Tick
        UpdateTimeAndDate()
        LoadPendingOrders()

    End Sub

    ' Method to update time and date
    Private Sub UpdateTimeAndDate()
        ' Set the current time and date in the label
        timeanddate.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") ' Format: "2025-05-02 14:45:00"
    End Sub
    Private Sub LoadTotalCustomers()
        Try
            Using connection As New MySqlConnection(strConnection)
                connection.Open()

                Dim cmd As New MySqlCommand("SELECT COUNT(*) FROM customer", connection)
                Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())

                totalcustomerplchldr.Text = count.ToString()

            End Using
        Catch ex As Exception
            MessageBox.Show("Error loading customer count: " & ex.Message)
        End Try
    End Sub
    Private Sub LoadTotalSales()
        Try
            Using connection As New MySqlConnection(strConnection)
                connection.Open()

                Dim cmd As New MySqlCommand("SELECT COUNT(*) FROM sales", connection)
                Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())

                totalsalesplcholder.Text = count.ToString()

            End Using
        Catch ex As Exception
            MessageBox.Show("Error loading sales count: " & ex.Message)
        End Try
    End Sub
    Private Sub LoadTotalEmployees()
        Try
            Using connection As New MySqlConnection(strConnection)
                connection.Open()

                Dim cmd As New MySqlCommand("SELECT COUNT(*) FROM employee", connection)
                Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())

                totalemployeesplcholder.Text = count.ToString()
            End Using
        Catch ex As Exception
            MessageBox.Show("Error loading employee count: " & ex.Message)
        End Try
    End Sub


    Private Sub LoadTotalExpenses()
        Try
            Using connection As New MySqlConnection(conn.ConnectionString)
                connection.Open()

                Dim cmd As New MySqlCommand("SELECT IFNULL(SUM(total), 0) FROM expensesoverview", connection)
                Dim totalExpenses As Decimal = Convert.ToDecimal(cmd.ExecuteScalar())

                totalexpensesplcholder.Text = "₱ " & totalExpenses.ToString("N2") ' Format as currency

            End Using
        Catch ex As Exception
            MessageBox.Show("Error loading total expenses: " & ex.Message)
        End Try
    End Sub

    'EMPLOYEE MANAGEMENT
    Private Sub LoadAllEmployees()
        Dim query As String = "SELECT 
        EID AS 'Employee ID',
        FirstName AS 'First Name',
        MiddleName AS 'Middle Name',
        LastName AS 'Last Name',
        Role,
        PhoneNumber AS 'Phone',
        Email,
        Barangay,
        City,
        Province,
        ZipCode AS 'ZIP',
        ActiveEmployee AS 'Active?',
        HireDate AS 'Hired',
        DateLeft AS 'Left On',
        Notes
    FROM employee 
    ORDER BY HireDate DESC
    LIMIT 10"

        LoadToDGV(query, empdgv)
        With empdgv
            .ColumnHeadersHeight = 50 ' Adjust height
            .ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True
            .AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells
            .DefaultCellStyle.WrapMode = DataGridViewTriState.True
            .RowTemplate.Height = 30 ' Optional, for data rows
            .RowHeadersWidth = 70


        End With
    End Sub
    Private Sub FilterEmployeeByStatus()
        Dim status As String = empdgvcmbbx.SelectedItem?.ToString()

        If String.IsNullOrEmpty(status) OrElse status = "All" Then
            LoadAllEmployees()
            Return
        End If

        Dim query As String = $"SELECT 
        EID AS 'Employee ID',
        FirstName AS 'First Name',
        MiddleName AS 'Middle Name',
        LastName AS 'Last Name',
        Role,
        PhoneNumber AS 'Phone',
        Email,
        Barangay,
        City,
        Province,
        ZipCode AS 'ZIP',
        ActiveEmployee AS 'Active?',
        HireDate AS 'Hired',
        DateLeft AS 'Left On',
        Notes
    FROM employee 
    WHERE ActiveEmployee = '{status}'
    ORDER BY HireDate DESC
     LIMIT 10"

        LoadToDGV(query, empdgv)
    End Sub
    Private Sub SearchEmployeesByName()
        Dim keyword As String = empdgvsearchbox.Text.Trim()

        Dim query As String = $"SELECT 
        EID AS 'Employee ID',
        FirstName AS 'First Name',
        MiddleName AS 'Middle Name',
        LastName AS 'Last Name',
        Role,
        PhoneNumber AS 'Phone',
        Email,
        Barangay,
        City,
        Province,
        ZipCode AS 'ZIP',
        ActiveEmployee AS 'Active?',
        HireDate AS 'Hired',
        DateLeft AS 'Left On',
        Notes
    FROM employee 
    WHERE 
        FirstName LIKE '%{keyword}%' OR 
        MiddleName LIKE '%{keyword}%' OR 
        LastName LIKE '%{keyword}%'
    ORDER BY HireDate DESC
         LIMIT 10"

        LoadToDGV(query, empdgv)
    End Sub
    Private Sub EmployeeManagementTab_Load(sender As Object, e As EventArgs) Handles EmployeeManagementTab.Enter
        LoadAllEmployees()

        ' Optional: Initialize ComboBox
        empdgvcmbbx.Items.Clear()
        empdgvcmbbx.Items.AddRange(New String() {"All", "Yes", "No"})
        empdgvcmbbx.SelectedIndex = 0 ' Default to All
        EmployeeAddPanel.Visible = False
        EmpUpdatePanel.Visible = False

        empupdrolecmbbx.Items.AddRange({"Admin", "Cashier", "Crew"})
        PopulateEmployeeRoles()
    End Sub

    Private Sub empdgvcmbbx_SelectedIndexChanged(sender As Object, e As EventArgs) Handles empdgvcmbbx.SelectedIndexChanged
        FilterEmployeeByStatus()
    End Sub

    ' Search Button Click
    Private Sub empdgvsearchbtn_Click(sender As Object, e As EventArgs) Handles empdgvsearchbtn.Click
        SearchEmployeesByName()
    End Sub

    ' Optional: Dynamic live search as you type
    Private Sub empdgvsearchbox_TextChanged(sender As Object, e As EventArgs) Handles empdgvsearchbox.TextChanged
        SearchEmployeesByName()
    End Sub


    'add employee
    Private Sub AddEmployee()
        ' Get values from form
        Dim fname As String = empfirstnametextbox.Text.Trim()
        Dim mname As String = empmiddlenametextbox.Text.Trim()
        Dim lname As String = emplastnametextbox.Text.Trim()
        Dim role As String = emprolecmbbx.Text.Trim()
        Dim phone As String = empphonenumtextbox.Text.Trim()
        Dim email As String = empemailtextbox.Text.Trim()
        Dim province As String = empprovincetextbox.Text.Trim()
        Dim city As String = empcitytextbox.Text.Trim()
        Dim barangay As String = empbarangaytextbox.Text.Trim()
        Dim zip As String = empzipcodetextbox.Text.Trim()
        Dim notes As String = empnotestextbox.Text.Trim()
        Dim hireDate As String = DateTime.Now.ToString("yyyy-MM-dd")

        ' Basic validation
        If fname = "" Or lname = "" Or role = "" Then
            MessageBox.Show("First name, last name, and role are required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' SQL Insert
        Dim query As String = "
        INSERT INTO employee 
        (FirstName, MiddleName, LastName, Role, PhoneNumber, Email, Province, City, Barangay, ZipCode, ActiveEmployee, HireDate, DateLeft, Notes)
        VALUES
        (@FirstName, @MiddleName, @LastName, @Role, @PhoneNumber, @Email, @Province, @City, @Barangay, @ZipCode, 'Yes', @HireDate, NULL, @Notes)
    "

        Using connection As New MySqlConnection(conn.ConnectionString)
            Using cmd As New MySqlCommand(query, connection)
                cmd.Parameters.AddWithValue("@FirstName", fname)
                cmd.Parameters.AddWithValue("@MiddleName", mname)
                cmd.Parameters.AddWithValue("@LastName", lname)
                cmd.Parameters.AddWithValue("@Role", role)
                cmd.Parameters.AddWithValue("@PhoneNumber", phone)
                cmd.Parameters.AddWithValue("@Email", email)
                cmd.Parameters.AddWithValue("@Province", province)
                cmd.Parameters.AddWithValue("@City", city)
                cmd.Parameters.AddWithValue("@Barangay", barangay)
                cmd.Parameters.AddWithValue("@ZipCode", zip)
                cmd.Parameters.AddWithValue("@HireDate", hireDate)
                cmd.Parameters.AddWithValue("@Notes", notes)

                Try
                    connection.Open()
                    cmd.ExecuteNonQuery()
                    MessageBox.Show("Employee added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    LoadAllEmployees() ' Refresh DGV
                Catch ex As Exception
                    MessageBox.Show("Error adding employee: " & ex.Message)
                End Try
            End Using
        End Using
    End Sub

    Private Sub empaddbtn_Click(sender As Object, e As EventArgs) Handles empaddbtn.Click
        EmployeeAddPanel.Visible = True


    End Sub

    Private Sub empadddbbtn_Click(sender As Object, e As EventArgs) Handles empadddbbtn.Click
        AddEmployee()
    End Sub

    Private Sub empcancelbtn_Click(sender As Object, e As EventArgs) Handles empcancelbtn.Click
        EmployeeAddPanel.Visible = False
    End Sub
    Private Sub PopulateEmployeeRoles()
        emprolecmbbx.Items.Clear()
        emprolecmbbx.Items.AddRange(New String() {"Admin", "Cashier", "Staff"})
        emprolecmbbx.SelectedIndex = 0 ' Set default selection
    End Sub

    'update button
    Private Sub empupdatebtn_Click(sender As Object, e As EventArgs) Handles empupdatebtn.Click
        If empdgv.SelectedRows.Count = 0 Then
            MessageBox.Show("Please select an employee to update.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' Get selected row
        Dim row As DataGridViewRow = empdgv.SelectedRows(0)

        ' Fill the update form
        empupdfirstname.Text = row.Cells("First Name").Value.ToString()
        empupdmiddlename.Text = row.Cells("Middle Name").Value.ToString()
        empupdlastname.Text = row.Cells("Last Name").Value.ToString()
        empupdrolecmbbx.Text = row.Cells("Role").Value.ToString()
        empupdphonenumber.Text = row.Cells("Phone").Value.ToString()
        empupdemail.Text = row.Cells("Email").Value.ToString()
        empupdprovince.Text = row.Cells("Province").Value.ToString()
        empupdcity.Text = row.Cells("City").Value.ToString()
        empupdbrgy.Text = row.Cells("Barangay").Value.ToString()
        empupdzip.Text = row.Cells("ZIP").Value.ToString()
        empupdnotes.Text = row.Cells("Notes").Value.ToString()



        ' Show update panel
        EmpUpdatePanel.Visible = True

    End Sub

    Private Sub updbtn_Click(sender As Object, e As EventArgs) Handles updbtn.Click
        ' Get updated values
        Dim fname As String = empupdfirstname.Text.Trim()
        Dim mname As String = empupdmiddlename.Text.Trim()
        Dim lname As String = empupdlastname.Text.Trim()
        Dim role As String = empupdrolecmbbx.Text.Trim()
        Dim phone As String = empupdphonenumber.Text.Trim()
        Dim email As String = empupdemail.Text.Trim()
        Dim province As String = empupdprovince.Text.Trim()
        Dim city As String = empupdcity.Text.Trim()
        Dim barangay As String = empupdbrgy.Text.Trim()
        Dim zip As String = empupdzip.Text.Trim()
        Dim notes As String = empupdnotes.Text.Trim()

        ' Validation
        If fname = "" Or lname = "" Or role = "" Then
            MessageBox.Show("First name, last name, and role are required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' Get selected employee ID
        If empdgv.SelectedRows.Count = 0 Then
            MessageBox.Show("Please select an employee to update.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim selectedRow As DataGridViewRow = empdgv.SelectedRows(0)
        Dim empID As Integer = Convert.ToInt32(selectedRow.Cells("Employee ID").Value)

        ' Update query with WHERE clause (use the original column name "EID")
        Dim query As String = "
    UPDATE employee SET
        FirstName = @FirstName,
        MiddleName = @MiddleName,
        LastName = @LastName,
        Role = @Role,
        PhoneNumber = @PhoneNumber,
        Email = @Email,
        Province = @Province,
        City = @City,
        Barangay = @Barangay,
        ZipCode = @ZipCode,
        Notes = @Notes
    WHERE EID = @EID
    "

        Try
            Using connection As New MySqlConnection(strConnection)
                connection.Open()

                Using cmd As New MySqlCommand(query, connection)
                    cmd.Parameters.AddWithValue("@FirstName", fname)
                    cmd.Parameters.AddWithValue("@MiddleName", mname)
                    cmd.Parameters.AddWithValue("@LastName", lname)
                    cmd.Parameters.AddWithValue("@Role", role)
                    cmd.Parameters.AddWithValue("@PhoneNumber", phone)
                    cmd.Parameters.AddWithValue("@Email", email)
                    cmd.Parameters.AddWithValue("@Province", province)
                    cmd.Parameters.AddWithValue("@City", city)
                    cmd.Parameters.AddWithValue("@Barangay", barangay)
                    cmd.Parameters.AddWithValue("@ZipCode", zip)
                    cmd.Parameters.AddWithValue("@Notes", notes)
                    cmd.Parameters.AddWithValue("@EID", empID) ' Ensure this matches the column name in the DB

                    cmd.ExecuteNonQuery()
                End Using
            End Using

            MessageBox.Show("Employee updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            EmpUpdatePanel.Visible = False

            LoadAllEmployees()
        Catch ex As Exception
            MessageBox.Show("Error updating employee: " & ex.Message)
        End Try
    End Sub


    Private Sub updcancelbtn_Click(sender As Object, e As EventArgs) Handles updcancelbtn.Click
        EmpUpdatePanel.Visible = False

    End Sub

    'delete emp
    Private Sub empdeletebtn_Click(sender As Object, e As EventArgs) Handles empdeletebtn.Click
        ' Ensure an employee is selected
        If empdgv.SelectedRows.Count = 0 Then
            MessageBox.Show("Please select an employee to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' Get the selected row
        Dim row As DataGridViewRow = empdgv.SelectedRows(0)
        Dim empID As Integer = Convert.ToInt32(row.Cells("Employee ID").Value)

        ' Ask for confirmation
        Dim result As DialogResult = MessageBox.Show("Are you sure you want to delete this employee?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
        If result = DialogResult.Yes Then
            ' Execute delete query
            Dim query As String = "DELETE FROM employee WHERE EID = @EID"

            Try
                Using connection As New MySqlConnection(strConnection)
                    Using cmd As New MySqlCommand(query, connection)
                        cmd.Parameters.AddWithValue("@EID", empID)

                        connection.Open()
                        cmd.ExecuteNonQuery()
                    End Using
                End Using

                MessageBox.Show("Employee deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

                ' Reload the DataGridView to reflect changes
                LoadAllEmployees()
            Catch ex As Exception
                MessageBox.Show("Error deleting employee: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub


    'print emp

    Private Sub GenerateEmployeePrintPanel(empData As DataGridViewRow)
        Try
            ' Clear the panel before regenerating it
            printPanel.Controls.Clear()
            printPanel.BorderStyle = BorderStyle.FixedSingle
            printPanel.Size = New Size(500, 650)
            printPanel.BackColor = Color.White
            printPanel.AutoScroll = True

            ' These are the exact DataGridView column headers you use
            Dim displayNames As New Dictionary(Of String, String) From {
            {"Employee ID", "Employee ID"},
            {"First Name", "First Name"},
            {"Middle Name", "Middle Name"},
            {"Last Name", "Last Name"},
            {"Role", "Role"},
            {"Phone", "Phone"},
            {"Email", "Email"},
            {"Barangay", "Barangay"},
            {"City", "City"},
            {"Province", "Province"},
            {"ZIP", "ZIP"},
            {"Active?", "Active?"},
            {"Hired", "Hired"},
            {"Left On", "Left On"},
            {"Notes", "Notes"}
        }

            Dim yOffset As Integer = 10

            ' Display each label
            For Each colKey In displayNames.Keys
                Dim value As String = If(empData.Cells(colKey).Value?.ToString(), "N/A")

                Dim lbl As New Label With {
                .AutoSize = True,
                .Font = New Font("Segoe UI", 10, FontStyle.Regular),
                .Location = New Point(10, yOffset),
                .Text = $"{displayNames(colKey)}: {value}"
            }

                printPanel.Controls.Add(lbl)
                yOffset += 30
            Next

            ' Add the Print Button at the bottom
            Dim printBtn As New Button With {
            .Text = "Print",
            .Font = New Font("Segoe UI", 10, FontStyle.Bold),
            .Size = New Size(100, 30),
            .Location = New Point(10, yOffset + 10)
        }
            AddHandler printBtn.Click, AddressOf PrintEmployeePanel
            printPanel.Controls.Add(printBtn)

            ' Show panel on form
            If Not Me.Controls.Contains(printPanel) Then
                Me.Controls.Add(printPanel)
            End If

            printPanel.BringToFront()
            printPanel.Location = New Point(20, 20) ' You can adjust this location

        Catch ex As Exception
            MessageBox.Show("Error while generating employee panel: " & ex.Message, "Panel Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub


    Private Sub empprinttopdfbtn_Click(sender As Object, e As EventArgs) Handles empprinttopdfbtn.Click
        If empdgv.SelectedRows.Count = 0 Then
            MessageBox.Show("Please select an employee row first.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim selectedRow As DataGridViewRow = empdgv.SelectedRows(0)
        GenerateEmployeePrintPanel(selectedRow)
    End Sub

    Private Sub PrintEmployeePanel(sender As Object, e As EventArgs)
        Try
            ' --- Hide border and Print button ---
            printPanel.BorderStyle = BorderStyle.None

            ' Find and hide the Print button inside the panel (if it exists)
            Dim printButton As Button = Nothing
            For Each ctrl As Control In printPanel.Controls
                If TypeOf ctrl Is Button AndAlso ctrl.Text = "Print" Then
                    printButton = DirectCast(ctrl, Button)
                    printButton.Visible = False
                    Exit For
                End If
            Next

            ' Create bitmap of the panel
            employeeBitmap = New Bitmap(printPanel.Width, printPanel.Height)
            printPanel.DrawToBitmap(employeeBitmap, New Rectangle(0, 0, printPanel.Width, printPanel.Height))

            ' Setup and show print dialog
            Dim printDocument As New Printing.PrintDocument()
            AddHandler printDocument.PrintPage, AddressOf EmpPrintDocument_PrintPage

            Dim printDialog As New PrintDialog With {
            .Document = printDocument
        }

            ' Show the print dialog and proceed with printing if confirmed
            If printDialog.ShowDialog() = DialogResult.OK Then
                printDocument.Print()
            End If



        Catch ex As Exception
            MessageBox.Show("An error occurred while printing: " & ex.Message, "Print Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub


    Private Sub EmpPrintDocument_PrintPage(sender As Object, e As Printing.PrintPageEventArgs)
        Try
            If employeeBitmap IsNot Nothing Then
                e.Graphics.DrawImage(employeeBitmap, 0, 0)
                printPanel.Visible = False
                ' Or, optionally remove it from the form entirely:
                ' Me.Controls.Remove(printPanel)
            End If

        Catch ex As Exception
            MessageBox.Show("An error occurred while rendering print page: " & ex.Message, "Render Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub


    'Inventory handlings

    Private Sub InventoryPanelTab_Load(sender As Object, e As EventArgs) Handles InvetoryPanelTab.Enter
        LoadAllInventory()
        InventoryAddPanel.Visible = False
        InventoryUpdatePanel.Visible = False
        LoadInventoryData("SELECT * FROM inventory ORDER BY ExpDate ASC") ' Default query
        If invexpirycmbbx.Items.Count = 0 Then
            invexpirycmbbx.Items.Add("Soonest Expiring")
            invexpirycmbbx.Items.Add("Least Expiring")
        End If
        invunitofmeasurecmbbx.Items.AddRange(New String() {"kg", "g", "Liters", "ml", "gallon", "pcs"})
        invtransactioncmbbx.Items.AddRange(New String() {"Purchase", "Delivery"})
        invperishablecmbbx.Items.AddRange(New String() {"Yes", "No"})
        If invunitofmeasurecmbbx.SelectedItem Is Nothing OrElse
            invtransactioncmbbx.SelectedItem Is Nothing OrElse
            invperishablecmbbx.SelectedItem Is Nothing Then
            MessageBox.Show("Please select a value for all dropdowns.", "Missing Info", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        invupdunitofmeasurecmbbx.Items.AddRange(New String() {"kg", "g", "Liters", "ml", "gallon", "pcs"})
        invupdtransactioncmbbx.Items.AddRange(New String() {"Purchase", "Delivery"})
        invupdperishablecmbbx.Items.AddRange(New String() {"Yes", "No"})
        If invupdunitofmeasurecmbbx.SelectedItem Is Nothing OrElse
            invupdtransactioncmbbx.SelectedItem Is Nothing OrElse
            invupdperishablecmbbx.SelectedItem Is Nothing Then
            MessageBox.Show("Please select a value for all dropdowns.", "Missing Info", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        ' Optionally set a default selection
        invexpirycmbbx.SelectedIndex = 0
        invunitofmeasurecmbbx.SelectedIndex = 0
        invtransactioncmbbx.SelectedIndex = 0
        invperishablecmbbx.SelectedIndex = 0

        invupdunitofmeasurecmbbx.SelectedIndex = 0
        invupdtransactioncmbbx.SelectedIndex = 0
        invupdperishablecmbbx.SelectedIndex = 0
        InventoryAddPanel.Visible = False
        InventoryUpdatePanel.Visible = False
    End Sub
    Private Sub invtransactioncmbbx_SelectedIndexChanged(sender As Object, e As EventArgs) Handles invtransactioncmbbx.SelectedIndexChanged
        Dim isDelivery As Boolean = invtransactioncmbbx.SelectedItem.ToString() = "Delivery"

        ' Editable if Delivery, Not Editable if Purchase
        invsuppliername.ReadOnly = Not isDelivery
        invsuppliernumber.ReadOnly = Not isDelivery
        invdeliverydatetimepicker.Enabled = isDelivery
    End Sub


    Private Sub invexpirycmbbx_SelectedIndexChanged(sender As Object, e As EventArgs) Handles invexpirycmbbx.SelectedIndexChanged
        ' Check selected option in ComboBox
        Dim sortOrder As String = invexpirycmbbx.SelectedItem.ToString()

        Dim query As String = ""
        If sortOrder = "Soonest Expiring" Then
            query = "SELECT * FROM inventory ORDER BY ExpDate ASC"
        ElseIf sortOrder = "Least Expiring" Then
            query = "SELECT * FROM inventory ORDER BY ExpDate DESC"
        End If

        ' Execute query and bind data to DataGridView
        If Not String.IsNullOrEmpty(query) Then
            LoadInventoryData(query)
        End If
    End Sub

    Private Sub LoadInventoryData(query As String)
        ' Execute query and bind data to DataGridView
        LoadToDGV(query, invdgv)

        ' Customize the DataGridView appearance
        With invdgv
            .ColumnHeadersHeight = 50 ' Adjust height of column headers
            .ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True ' Allow wrapping of header text
            .AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells ' Automatically adjust row height to fit content
            .DefaultCellStyle.WrapMode = DataGridViewTriState.True ' Allow text wrapping in cells
            .RowTemplate.Height = 30 ' Set height of data rows
            .RowHeadersWidth = 70 ' Set width of row headers
        End With
    End Sub

    Private Sub SearchInventory()
        Dim searchTerm As String = invsearchtextbox.Text

        Dim query As String = "SELECT * FROM inventory WHERE ItemName LIKE @searchTerm"
        Try
            Using connection As New MySqlConnection(strConnection)
                Using cmd As New MySqlCommand(query, connection)
                    cmd.Parameters.AddWithValue("@searchTerm", "%" & searchTerm & "%")
                    Dim dt As New DataTable()
                    connection.Open()
                    dt.Load(cmd.ExecuteReader())
                    invdgv.DataSource = dt
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error searching inventory: " & ex.Message)
        End Try
    End Sub

    Private Sub invsearchtextbox_TextChanged(sender As Object, e As EventArgs) Handles invsearchtextbox.TextChanged
        If String.IsNullOrEmpty(invsearchtextbox.Text) Then
            LoadInventoryData("SELECT * FROM inventory ORDER BY ExpDate ASC") ' Default query
        Else
            SearchInventory()
        End If
    End Sub

    Private Sub invsearchbtn_Click(sender As Object, e As EventArgs) Handles invsearchbtn.Click
        SearchInventory()
    End Sub

    'adding in the inventory
    Private Sub SaveInventoryItem()
        ' Ensure database connection is defined
        Using conn As New MySqlConnection(strConnection)
            Try
                conn.Open()

                ' Validate and convert numerical inputs
                Dim qty As Decimal
                If Not Decimal.TryParse(invquantity.Text, qty) Then
                    MessageBox.Show("Invalid quantity. Please enter a numeric value.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Sub
                End If

                Dim purPrice As Decimal
                If Not Decimal.TryParse(invpurchaseprice.Text, purPrice) Then
                    MessageBox.Show("Invalid purchase price. Please enter a numeric value.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Sub
                End If

                Dim reorderLevel As Integer
                If Not Integer.TryParse(invreorderlvl.Text, reorderLevel) Then
                    MessageBox.Show("Invalid reorder level. Please enter a whole number.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Sub
                End If

                ' Validate ComboBoxes
                If invunitofmeasurecmbbx.SelectedItem Is Nothing Then
                    MessageBox.Show("Please select a unit of measure.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Sub
                End If

                If invtransactioncmbbx.SelectedItem Is Nothing Then
                    MessageBox.Show("Please select a transaction type.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Sub
                End If

                If invperishablecmbbx.SelectedItem Is Nothing Then
                    MessageBox.Show("Please select if the item is perishable.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Sub
                End If

                ' SQL insert query
                Dim query As String = "INSERT INTO inventory (
                ItemName, Qty, UnitOfMeasure, TransactionType,
                SupplierName, SupplierNumber, DelDate, PurDate, ExpDate,
                PurPriceperPiece, Perishable, ReorderLevel, DateRecorded,
                ProcessedBy, Notes
            ) VALUES (
                @ItemName, @Qty, @UnitOfMeasure, @TransactionType,
                @SupplierName, @SupplierNumber, @DelDate, @PurDate, @ExpDate,
                @PurPriceperPiece, @Perishable, @ReorderLevel, @DateRecorded,
                @ProcessedBy, @Notes
            )"

                Using cmd As New MySqlCommand(query, conn)
                    ' Basic fields
                    cmd.Parameters.AddWithValue("@ItemName", invitemname.Text)
                    cmd.Parameters.AddWithValue("@Qty", qty)
                    cmd.Parameters.AddWithValue("@UnitOfMeasure", invunitofmeasurecmbbx.SelectedItem.ToString())

                    cmd.Parameters.AddWithValue("@TransactionType", invtransactioncmbbx.SelectedItem.ToString())

                    ' Supplier fields
                    cmd.Parameters.AddWithValue("@SupplierName", invsuppliername.Text)
                    cmd.Parameters.AddWithValue("@SupplierNumber", invsuppliernumber.Text)
                    cmd.Parameters.AddWithValue("@DelDate", invdeliverydatetimepicker.Value.Date)

                    ' Dates
                    cmd.Parameters.AddWithValue("@PurDate", invpurchasedatetimepicker.Value.Date)
                    cmd.Parameters.AddWithValue("@ExpDate", invexpirydatetimepicker.Value.Date)

                    ' Other fields
                    cmd.Parameters.AddWithValue("@PurPriceperPiece", purPrice)
                    cmd.Parameters.AddWithValue("@Perishable", invperishablecmbbx.SelectedItem.ToString())
                    cmd.Parameters.AddWithValue("@ReorderLevel", reorderLevel)
                    cmd.Parameters.AddWithValue("@DateRecorded", DateTime.Now)
                    cmd.Parameters.AddWithValue("@ProcessedBy", CurrentUserRole)
                    cmd.Parameters.AddWithValue("@Notes", invnotes.Text)

                    ' Execute the insert
                    cmd.ExecuteNonQuery()
                    MessageBox.Show("Inventory item successfully added!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    LoadAllInventory()
                End Using

            Catch ex As Exception
                MessageBox.Show("Error inserting inventory item: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Using
    End Sub


    Private Sub invadddbbtn_Click(sender As Object, e As EventArgs) Handles invadddbbtn.Click
        SaveInventoryItem()
    End Sub

    Private Sub invaddcancelbtn_Click(sender As Object, e As EventArgs) Handles invaddcancelbtn.Click
        InventoryAddPanel.Visible = False
    End Sub

    Private Sub invaddbtn_Click(sender As Object, e As EventArgs) Handles invaddbtn.Click
        InventoryAddPanel.Visible = True
    End Sub

    'inventory update
    Private Sub invupdatebtn_Click(sender As Object, e As EventArgs) Handles invupdatebtn.Click
        If invdgv.SelectedRows.Count = 0 Then
            MessageBox.Show("Please select an inventory item to update.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' Get selected row
        Dim row As DataGridViewRow = invdgv.SelectedRows(0)

        ' Fill the update form
        invupditemname.Text = row.Cells("ItemName").Value.ToString()
        invupdquantity.Text = row.Cells("Qty").Value.ToString()
        invupdunitofmeasurecmbbx.Text = row.Cells("UnitOfMeasure").Value.ToString()
        invupdtransactioncmbbx.Text = row.Cells("TransactionType").Value.ToString()
        invupdsuppliername.Text = row.Cells("SupplierName").Value.ToString()
        invupdsuppliernumber.Text = row.Cells("SupplierNumber").Value.ToString()

        ' Convert DateTime fields safely
        invupddeliverydatetimepicker.Value = If(IsDBNull(row.Cells("DelDate").Value), Now, Convert.ToDateTime(row.Cells("DelDate").Value))
        invupdpurchasedatetimepicker.Value = If(IsDBNull(row.Cells("PurDate").Value), Now, Convert.ToDateTime(row.Cells("PurDate").Value))
        invupdexpirydatetimepicker.Value = If(IsDBNull(row.Cells("ExpDate").Value), Now, Convert.ToDateTime(row.Cells("ExpDate").Value))

        invupdpurchaseprice.Text = row.Cells("PurPriceperPiece").Value.ToString()
        invupdperishablecmbbx.Text = row.Cells("Perishable").Value.ToString()
        invupdreorderlvl.Text = row.Cells("ReorderLevel").Value.ToString()
        invupdnotes.Text = row.Cells("Notes").Value.ToString()

        ' Save ItemID as a hidden tag for later use in update
        invupditemname.Tag = row.Cells("ItemID").Value

        ' Show the update panel
        InventoryUpdatePanel.Visible = True
    End Sub

    Private Sub invupdbtn_Click(sender As Object, e As EventArgs) Handles invupdbtn.Click
        ' Validate selection
        If invdgv.SelectedRows.Count = 0 Then
            MessageBox.Show("Please select an inventory item to update.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' Get selected row & ItemID
        Dim selectedRow As DataGridViewRow = invdgv.SelectedRows(0)
        Dim itemID As Integer = Convert.ToInt32(selectedRow.Cells("ItemID").Value)

        ' Get values from input controls
        Dim itemName As String = invupditemname.Text.Trim()
        Dim qty As Integer = If(IsNumeric(invupdquantity.Text), Convert.ToInt32(invupdquantity.Text), 0)
        Dim unit As String = invupdunitofmeasurecmbbx.Text.Trim()
        Dim transactionType As String = invupdtransactioncmbbx.Text.Trim()
        Dim supplierName As String = invupdsuppliername.Text.Trim()
        Dim supplierNumber As String = invupdsuppliernumber.Text.Trim()
        Dim delDate As Date = invupddeliverydatetimepicker.Value
        Dim purDate As Date = invupdpurchasedatetimepicker.Value
        Dim expDate As Date = invupdexpirydatetimepicker.Value
        Dim purPrice As Decimal = If(IsNumeric(invupdpurchaseprice.Text), Convert.ToDecimal(invupdpurchaseprice.Text), 0)
        Dim perishable As String = invupdperishablecmbbx.Text.Trim()
        Dim reorderLvl As Integer = If(IsNumeric(invupdreorderlvl.Text), Convert.ToInt32(invupdreorderlvl.Text), 0)
        Dim notes As String = invupdnotes.Text.Trim()

        ' Calculate total expenses
        Dim totalExpenses As Decimal = qty * purPrice

        ' Prepare SQL query
        Dim query As String = "
        UPDATE inventory SET
            ItemName = @ItemName,
            Qty = @Qty,
            UnitOfMeasure = @UnitOfMeasure,
            TransactionType = @TransactionType,
            SupplierName = @SupplierName,
            SupplierNumber = @SupplierNumber,
            DelDate = @DelDate,
            PurDate = @PurDate,
            ExpDate = @ExpDate,
            PurPricePerPiece = @PurPricePerPiece,
            Perishable = @Perishable,
            ReorderLevel = @ReorderLevel,
            Notes = @Notes,
            TotalExpenses = @TotalExpenses
        WHERE ItemID = @ItemID
        "

        Try
            Using connection As New MySqlConnection(strConnection)
                connection.Open()
                Using cmd As New MySqlCommand(query, connection)
                    cmd.Parameters.AddWithValue("@ItemName", itemName)
                    cmd.Parameters.AddWithValue("@Qty", qty)
                    cmd.Parameters.AddWithValue("@UnitOfMeasure", unit)
                    cmd.Parameters.AddWithValue("@TransactionType", transactionType)
                    cmd.Parameters.AddWithValue("@SupplierName", supplierName)
                    cmd.Parameters.AddWithValue("@SupplierNumber", supplierNumber)
                    cmd.Parameters.AddWithValue("@DelDate", delDate)
                    cmd.Parameters.AddWithValue("@PurDate", purDate)
                    cmd.Parameters.AddWithValue("@ExpDate", expDate)
                    cmd.Parameters.AddWithValue("@PurPricePerPiece", purPrice)
                    cmd.Parameters.AddWithValue("@Perishable", perishable)
                    cmd.Parameters.AddWithValue("@ReorderLevel", reorderLvl)
                    cmd.Parameters.AddWithValue("@Notes", notes)
                    cmd.Parameters.AddWithValue("@TotalExpenses", totalExpenses)
                    cmd.Parameters.AddWithValue("@ItemID", itemID)

                    cmd.ExecuteNonQuery()
                End Using
            End Using

            MessageBox.Show("Inventory item updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

            InventoryUpdatePanel.Visible = False
            LoadAllInventory() ' Your function to reload the inventory DataGridView
        Catch ex As Exception
            MessageBox.Show("Error updating inventory item: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Private Sub LoadAllInventory()
        Dim query As String = "
    SELECT 

        ItemID,
        ItemName AS 'Item Name',
        Qty AS 'Quantity',
        UnitOfMeasure AS 'Unit',
        TransactionType AS 'Transaction',
        SupplierName AS 'Supplier',
        SupplierNumber AS 'Contact',
        DelDate AS 'Delivery Date',
        PurDate AS 'Purchase Date',
        ExpDate AS 'Expiry Date',
        PurPricePerPiece AS 'Price/pc',
        Perishable AS 'Perishable?',
        ReorderLevel AS 'Reorder Level',
        DateRecorded AS 'Recorded On',
        ProcessedBy AS 'Processed By',
        Notes,
        TotalExpenses AS 'Total Cost'
    FROM inventory
    ORDER BY DateRecorded DESC
    LIMIT 10"

        LoadToDGV(query, invdgv)

        With invdgv
            .ColumnHeadersHeight = 50
            .ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True
            .AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells
            .DefaultCellStyle.WrapMode = DataGridViewTriState.True
            .RowTemplate.Height = 30
            .RowHeadersWidth = 70
        End With
    End Sub
    'inventory del button
    Private Sub invdeletebtn_Click(sender As Object, e As EventArgs) Handles invdeletebtn.Click
        ' Ensure an item is selected
        If invdgv.SelectedRows.Count = 0 Then
            MessageBox.Show("Please select an Item to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' Get the selected row
        Dim row As DataGridViewRow = invdgv.SelectedRows(0)
        Dim ItemID As Integer = Convert.ToInt32(row.Cells("ItemID").Value) ' Use the original column name

        ' Ask for confirmation
        Dim result As DialogResult = MessageBox.Show("Are you sure you want to delete this Item?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
        If result = DialogResult.Yes Then
            Dim query As String = "DELETE FROM inventory WHERE ItemID = @ItemID"

            Try
                Using connection As New MySqlConnection(strConnection)
                    Using cmd As New MySqlCommand(query, connection)
                        cmd.Parameters.AddWithValue("@ItemID", ItemID)
                        connection.Open()
                        cmd.ExecuteNonQuery()
                    End Using
                End Using

                MessageBox.Show("Item deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                LoadAllInventory()
            Catch ex As Exception
                MessageBox.Show("Error deleting Item: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    'invprinttopdf
    Private Sub GenerateInventoryPrintPanel(invData As DataGridViewRow)
        Try
            printPanel.Controls.Clear()
            printPanel.BorderStyle = BorderStyle.FixedSingle
            printPanel.Size = New Size(550, 700)
            printPanel.BackColor = Color.White
            printPanel.AutoScroll = True

            ' Use actual database column names
            Dim displayNames As New Dictionary(Of String, String) From {
            {"ItemID", "Item ID"},
            {"ItemName", "Item Name"},
            {"Qty", "Quantity"},
            {"UnitOfMeasure", "Unit"},
            {"TransactionType", "Transaction"},
            {"SupplierName", "Supplier"},
            {"SupplierNumber", "Contact"},
            {"DelDate", "Delivery Date"},
            {"PurDate", "Purchase Date"},
            {"ExpDate", "Expiry Date"},
            {"PurPricePerPiece", "Price per Piece"},
            {"Perishable", "Perishable?"},
            {"ReorderLevel", "Reorder Level"},
            {"DateRecorded", "Recorded On"},
            {"ProcessedBy", "Processed By"},
            {"Notes", "Notes"},
            {"TotalExpenses", "Total Cost"}
        }

            Dim yOffset As Integer = 10

            For Each colKey In displayNames.Keys
                Dim value As String = If(invData.Cells(colKey).Value?.ToString(), "N/A")

                Dim lbl As New Label With {
                .AutoSize = True,
                .Font = New Font("Segoe UI", 10, FontStyle.Regular),
                .Location = New Point(10, yOffset),
                .Text = $"{displayNames(colKey)}: {value}"
            }

                printPanel.Controls.Add(lbl)
                yOffset += 30
            Next

            ' Add the Print button
            Dim printBtn As New Button With {
            .Text = "Print",
            .Font = New Font("Segoe UI", 10, FontStyle.Bold),
            .Size = New Size(100, 30),
            .Location = New Point(10, yOffset + 10)
        }
            AddHandler printBtn.Click, AddressOf PrintInventoryPanel
            printPanel.Controls.Add(printBtn)

            ' Show the panel
            If Not Me.Controls.Contains(printPanel) Then
                Me.Controls.Add(printPanel)
            End If

            printPanel.BringToFront()
            printPanel.Location = New Point(20, 20)

        Catch ex As Exception
            MessageBox.Show("Error while generating inventory panel: " & ex.Message, "Panel Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub invprinttopdfbtn_Click(sender As Object, e As EventArgs) Handles invprinttopdfbtn.Click
        If invdgv.SelectedRows.Count = 0 Then
            MessageBox.Show("Please select an inventory row first.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim selectedRow As DataGridViewRow = invdgv.SelectedRows(0)
        GenerateInventoryPrintPanel(selectedRow)
    End Sub
    Private Sub PrintInventoryPanel(sender As Object, e As EventArgs)
        Try
            ' --- Hide border and Print button ---
            printPanel.BorderStyle = BorderStyle.None

            ' Find and hide the Print button inside the panel (if it exists)
            Dim printButton As Button = Nothing
            For Each ctrl As Control In printPanel.Controls
                If TypeOf ctrl Is Button AndAlso ctrl.Text = "Print" Then
                    printButton = DirectCast(ctrl, Button)
                    printButton.Visible = False
                    Exit For
                End If
            Next

            ' Draw the panel into a bitmap
            inventoryBitmap = New Bitmap(printPanel.Width, printPanel.Height)
            printPanel.DrawToBitmap(inventoryBitmap, New Rectangle(0, 0, printPanel.Width, printPanel.Height))

            ' Print setup
            Dim printDocument As New Printing.PrintDocument()
            AddHandler printDocument.PrintPage, AddressOf InvPrintDocument_PrintPage

            Dim printDialog As New PrintDialog With {
            .Document = printDocument
        }

            If printDialog.ShowDialog() = DialogResult.OK Then
                printDocument.Print()
            End If

            ' --- Restore the panel UI after printing ---


        Catch ex As Exception
            MessageBox.Show("An error occurred while printing: " & ex.Message, "Print Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub InvPrintDocument_PrintPage(sender As Object, e As Printing.PrintPageEventArgs)
        Try
            If inventoryBitmap IsNot Nothing Then
                e.Graphics.DrawImage(inventoryBitmap, 0, 0)
                printPanel.Visible = False ' Optional: or Me.Controls.Remove(printPanel)
            End If
        Catch ex As Exception
            MessageBox.Show("An error occurred during rendering: " & ex.Message, "Render Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub


    'Sales chuchuness


    'delete
    Private Sub salesdeletebtn_Click(sender As Object, e As EventArgs) Handles salesdeletebtn.Click
        ' Check if a row is selected
        If salesdgv.SelectedRows.Count = 0 Then
            MessageBox.Show("Please select a sale to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' Get selected Sale ID (SID)
        Dim row As DataGridViewRow = salesdgv.SelectedRows(0)
        Dim SID As Integer = Convert.ToInt32(row.Cells("Sales ID").Value)

        ' Confirm deletion
        Dim result As DialogResult = MessageBox.Show("Are you sure you want to delete this sale?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If result = DialogResult.Yes Then
            Dim query As String = "DELETE FROM sales WHERE SID = @SID"

            Try
                Using conn As New MySqlConnection(strConnection)
                    Using cmd As New MySqlCommand(query, conn)
                        cmd.Parameters.AddWithValue("@SID", SID)
                        conn.Open()
                        cmd.ExecuteNonQuery()
                    End Using
                End Using

                MessageBox.Show("Sale deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                LoadAllSales() ' Reload sales after deletion
            Catch ex As Exception
                MessageBox.Show("Error deleting sale: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    Private Sub LoadAllSales()
        ' Add pagination or date range filtering
        Dim query As String = "
    SELECT 
        SID AS 'Sales ID',
        CID AS 'Customer ID',
        ReceiptNumber AS 'Receipt #',
        TimeOrdered AS 'Time',
        DateOrdered AS 'Date',
        ItemsOrdered AS 'Items',
        QTY AS 'Qty',
        SubTotal AS 'Subtotal',
        DiscountAmount AS 'Discount',
        NetAmount AS 'Net Total',
        PaymentMethod AS 'Payment',
        TransactionType AS 'Type',
        OrderStatus AS 'Status',
        RefundStatus AS 'Refund',
        DateRecorded AS 'Recorded On',
        ProcessedBy AS 'Processed By',
        Notes
    FROM sales
    ORDER BY DateOrdered DESC
    LIMIT 10" ' Limit to 500 most recent records

        LoadToDGV(query, salesdgv)

        With salesdgv
            ' Suspend layout while updating
            .SuspendLayout()

            ' Set these properties BEFORE loading data
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None
            .ColumnHeadersHeight = 50
            .ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False ' Avoid wrapping headers
            .AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None ' Disable during load
            .DefaultCellStyle.WrapMode = DataGridViewTriState.False
            .RowTemplate.Height = 30
            .RowHeadersWidth = 70

            ' Load data
            LoadToDGV(query, salesdgv)

            ' Configure columns after data is loaded
            For Each col As DataGridViewColumn In .Columns
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
            Next

            ' Only enable these features after data is loaded
            .AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells
            .DefaultCellStyle.WrapMode = DataGridViewTriState.True

            ' Resume layout
            .ResumeLayout(True)
        End With
    End Sub


    Private Sub SalesPanelTab_Load(sender As Object, e As EventArgs) Handles SalesPanelTab.Enter
        With salescmbbx
            .Items.Clear()
            .Items.Add("This Day")
            .Items.Add("This Week")
            .Items.Add("This Month")
            .Items.Add("This Year")
            .SelectedIndex = 0 ' Default selection
        End With

        LoadAllSales()

    End Sub

    Private Sub salescmbbx_SelectedIndexChanged(sender As Object, e As EventArgs) Handles salescmbbx.SelectedIndexChanged
        Dim query As String = "
    SELECT 
        SID AS 'Sales ID',
        CID AS 'Customer ID',
        ReceiptNumber AS 'Receipt #',
        TimeOrdered AS 'Time',
        DateOrdered AS 'Date',
        ItemsOrdered AS 'Items',
        QTY AS 'Qty',
        SubTotal AS 'Subtotal',
        DiscountAmount AS 'Discount',
        NetAmount AS 'Net Total',
        PaymentMethod AS 'Payment',
        TransactionType AS 'Type',
        OrderStatus AS 'Status',
        RefundStatus AS 'Refund',
        DateRecorded AS 'Recorded On',
        ProcessedBy AS 'Processed By',
        Notes
    FROM sales
    WHERE "

        Select Case salescmbbx.SelectedItem.ToString()
            Case "This Day"
                query &= "DateOrdered = CURDATE()"
            Case "This Week"
                query &= "YEARWEEK(DateOrdered, 1) = YEARWEEK(CURDATE(), 1)"
            Case "This Month"
                query &= "MONTH(DateOrdered) = MONTH(CURDATE()) AND YEAR(DateOrdered) = YEAR(CURDATE())"
            Case "This Year"
                query &= "YEAR(DateOrdered) = YEAR(CURDATE())"
        End Select

        query &= " ORDER BY DateOrdered DESC"

        LoadToDGV(query, salesdgv)
    End Sub
    Private Sub SearchSales(keyword As String)
        Dim query As String = "
SELECT 
    SID AS 'Sales ID',
    CID AS 'Customer ID',
    ReceiptNumber AS 'Receipt #',
    TimeOrdered AS 'Time',
    DateOrdered AS 'Date',
    ItemsOrdered AS 'Items',
    QTY AS 'Qty',
    SubTotal AS 'Subtotal',
    DiscountAmount AS 'Discount',
    NetAmount AS 'Net Total',
    PaymentMethod AS 'Payment',
    TransactionType AS 'Type',
    OrderStatus AS 'Status',
    RefundStatus AS 'Refund',
    DateRecorded AS 'Recorded On',
    ProcessedBy AS 'Processed By',
    Notes
FROM sales
WHERE ReceiptNumber LIKE @kw OR ItemsOrdered LIKE @kw
ORDER BY DateOrdered DESC"

        Try
            Using conn As New MySqlConnection(strConnection)
                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@kw", "%" & keyword & "%")
                    Dim adapter As New MySqlDataAdapter(cmd)
                    Dim table As New DataTable()
                    adapter.Fill(table)
                    salesdgv.DataSource = table
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Search failed: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Private Sub salessearchbtn_Click(sender As Object, e As EventArgs) Handles salessearchbtn.Click
        Dim keyword As String = salestextbox.Text.Trim()

        If keyword = "" Then
            MessageBox.Show("Please enter a search term.", "Empty Search", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        SearchSales(keyword)
    End Sub
    Private Sub salestextbox_TextChanged(sender As Object, e As EventArgs) Handles salestextbox.TextChanged
        Dim keyword As String = salestextbox.Text.Trim()
        If keyword <> "" Then
            SearchSales(keyword)
        Else
            ' Optional: Clear DataGridView or reload all sales
            salesdgv.DataSource = Nothing
        End If
    End Sub

    'update sales



    Private Sub CustomerTabPanel_Enter(sender As Object, e As EventArgs) Handles CustomerTabPanel.Enter
        If customercmbbx IsNot Nothing AndAlso customercmbbx.Items.Count = 0 Then
            With customercmbbx.Items
                .Clear()
                .Add("This Day")
                .Add("This Week")
                .Add("This Month")
                .Add("This Year")
            End With
            customercmbbx.SelectedIndex = 0
        End If
        CustomerUpdatePanel.Visible = False
        LoadComboBoxValues()
    End Sub
    Private Sub LoadComboBoxValues()
        ' Transaction Type
        custranstypecmbbx.Items.Clear()
        custranstypecmbbx.Items.AddRange(New String() {"Dine in", "Take out"})

        ' Order Status
        cusupdorderstatus.Items.Clear()
        cusupdorderstatus.Items.AddRange(New String() {"Served", "Pending", "Cancelled"})

        ' Payment Method
        cusupdpaymentmethodcmbbx.Items.Clear()
        cusupdpaymentmethodcmbbx.Items.AddRange(New String() {"Gcash", "Cash"})

        ' Refund Status
        cusupdrefundcmbbx.Items.Clear()
        cusupdrefundcmbbx.Items.AddRange(New String() {"Yes", "No"})

        ' Optional: Set default selected index (first item)
        If custranstypecmbbx.Items.Count > 0 Then custranstypecmbbx.SelectedIndex = 0
        If cusupdorderstatus.Items.Count > 0 Then cusupdorderstatus.SelectedIndex = 0
        If cusupdpaymentmethodcmbbx.Items.Count > 0 Then cusupdpaymentmethodcmbbx.SelectedIndex = 0
        If cusupdrefundcmbbx.Items.Count > 0 Then cusupdrefundcmbbx.SelectedIndex = 0
    End Sub


    Private Sub customersearchbtn_Click(sender As Object, e As EventArgs) Handles customersearchbtn.Click
        LoadAllCustomers()
    End Sub

    Private Sub customersearchbox_TextChanged(sender As Object, e As EventArgs) Handles customersearchbox.TextChanged
        LoadAllCustomers()
    End Sub
    Private Sub LoadAllCustomers()
        Dim searchText As String = customersearchbox.Text.Trim().Replace("'", "''")

        Dim query As String = "
    SELECT 
        CID,
        FirstName AS 'First Name',
        LastName AS 'Last Name',
        PhoneNumber AS 'Contact',
        Address,
        TableNumber AS 'Table #',
        ItemsOrdered AS 'Ordered Items',
        QTY AS 'Quantity',
        TransactionType AS 'Transaction',
        OrderStatus AS 'Status',
        TimeOrdered AS 'Time',
        DateOrdered AS 'Date Ordered',
        SubTotal AS 'Subtotal',
        DiscountAmount AS 'Discount',
        NetAmount AS 'Net',
        PaymentMethod AS 'Payment',
        ReceiptNumber AS 'Receipt #',
        RefundStatus AS 'Refunded?',
        DateRecorded AS 'Recorded On',
        ProcessedBy AS 'Processed By',
        Notes
    FROM customer
    "

        ' Add WHERE clause only if search text is not empty
        If Not String.IsNullOrWhiteSpace(searchText) Then
            query &= " WHERE 
        FirstName LIKE '%" & searchText & "%' OR 
        LastName LIKE '%" & searchText & "%' OR 
        PhoneNumber LIKE '%" & searchText & "%' OR 
        Address LIKE '%" & searchText & "%' OR 
        ItemsOrdered LIKE '%" & searchText & "%' OR 
        ReceiptNumber LIKE '%" & searchText & "%' "
        End If

        query &= " ORDER BY DateRecorded DESC"

        LoadToDGV(query, cusdgv)

        With cusdgv
            .ColumnHeadersHeight = 50
            .ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True
            .AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells
            .DefaultCellStyle.WrapMode = DataGridViewTriState.True
            .RowTemplate.Height = 30
            .RowHeadersWidth = 70
        End With
    End Sub

    Private Sub customercmbbx_SelectedIndexChanged(sender As Object, e As EventArgs) Handles customercmbbx.SelectedIndexChanged
        Dim query As String = ""

        Select Case customercmbbx.SelectedItem.ToString()
            Case "This Day"
                query = "
            SELECT 
                CID,
                CONCAT(FirstName, ' ', LastName) AS 'Customer Name',
                PhoneNumber AS 'Phone',
                Address,
                TableNumber AS 'Table',
                ItemsOrdered AS 'Items',
                QTY AS 'Quantity',
                TransactionType AS 'Transaction',
                OrderStatus AS 'Status',
                TimeOrdered AS 'Time',
                DateOrdered AS 'Date',
                SubTotal AS 'Subtotal',
                DiscountAmount AS 'Discount',
                NetAmount AS 'Net Total',
                PaymentMethod AS 'Payment',
                ReceiptNumber AS 'Receipt No.',
                RefundStatus AS 'Refund?',
                DateRecorded AS 'Recorded On',
                ProcessedBy AS 'Processed By',
                Notes
            FROM customer
            WHERE DATE(DateOrdered) = CURDATE()
            ORDER BY DateOrdered DESC"

            Case "This Week"
                query = "
            SELECT 
                CID,
                CONCAT(FirstName, ' ', LastName) AS 'Customer Name',
                PhoneNumber AS 'Phone',
                Address,
                TableNumber AS 'Table',
                ItemsOrdered AS 'Items',
                QTY AS 'Quantity',
                TransactionType AS 'Transaction',
                OrderStatus AS 'Status',
                TimeOrdered AS 'Time',
                DateOrdered AS 'Date',
                SubTotal AS 'Subtotal',
                DiscountAmount AS 'Discount',
                NetAmount AS 'Net Total',
                PaymentMethod AS 'Payment',
                ReceiptNumber AS 'Receipt No.',
                RefundStatus AS 'Refund?',
                DateRecorded AS 'Recorded On',
                ProcessedBy AS 'Processed By',
                Notes
            FROM customer
            WHERE YEARWEEK(DateOrdered, 1) = YEARWEEK(CURDATE(), 1)
            ORDER BY DateOrdered DESC"

            Case "This Month"
                query = "
            SELECT 
                CID,
                CONCAT(FirstName, ' ', LastName) AS 'Customer Name',
                PhoneNumber AS 'Phone',
                Address,
                TableNumber AS 'Table',
                ItemsOrdered AS 'Items',
                QTY AS 'Quantity',
                TransactionType AS 'Transaction',
                OrderStatus AS 'Status',
                TimeOrdered AS 'Time',
                DateOrdered AS 'Date',
                SubTotal AS 'Subtotal',
                DiscountAmount AS 'Discount',
                NetAmount AS 'Net Total',
                PaymentMethod AS 'Payment',
                ReceiptNumber AS 'Receipt No.',
                RefundStatus AS 'Refund?',
                DateRecorded AS 'Recorded On',
                ProcessedBy AS 'Processed By',
                Notes
            FROM customer
            WHERE MONTH(DateOrdered) = MONTH(CURDATE())
            AND YEAR(DateOrdered) = YEAR(CURDATE())
            ORDER BY DateOrdered DESC"

            Case "This Year"
                query = "
            SELECT 
                CID,
                CONCAT(FirstName, ' ', LastName) AS 'Customer Name',
                PhoneNumber AS 'Phone',
                Address,
                TableNumber AS 'Table',
                ItemsOrdered AS 'Items',
                QTY AS 'Quantity',
                TransactionType AS 'Transaction',
                OrderStatus AS 'Status',
                TimeOrdered AS 'Time',
                DateOrdered AS 'Date',
                SubTotal AS 'Subtotal',
                DiscountAmount AS 'Discount',
                NetAmount AS 'Net Total',
                PaymentMethod AS 'Payment',
                ReceiptNumber AS 'Receipt No.',
                RefundStatus AS 'Refund?',
                DateRecorded AS 'Recorded On',
                ProcessedBy AS 'Processed By',
                Notes
            FROM customer
            WHERE YEAR(DateOrdered) = YEAR(CURDATE())
            ORDER BY DateOrdered DESC
            LIMIT 10"
        End Select

        ' Load the filtered data into the DataGridView
        LoadToDGV(query, cusdgv)

        With cusdgv
            .ColumnHeadersHeight = 50
            .ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True
            .AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells
            .DefaultCellStyle.WrapMode = DataGridViewTriState.True
            .RowTemplate.Height = 30
            .RowHeadersWidth = 70
        End With
    End Sub



    'customer update
    Private selectedCustomerRow As DataGridViewRow = Nothing

    Private Sub cusdgv_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles cusdgv.CellClick
        ' Check if the user clicked a valid row (ignoring header)
        If e.RowIndex >= 0 Then
            ' Store the selected row
            Dim selectedRow As DataGridViewRow = cusdgv.Rows(e.RowIndex)

            ' Enable the update button only after a valid row is selected
            cusupdatebtn.Enabled = True
            ' You can also store the row globally if needed for other logic
            selectedCustomerRow = selectedRow
        End If
    End Sub

    Private Sub cusupdatebtn_Click(sender As Object, e As EventArgs) Handles cusupdatebtn.Click
        ' Check if no row is selected
        If selectedCustomerRow Is Nothing Then
            MessageBox.Show("Please select a customer to update.", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Try
            ' Show the update panel
            CustomerUpdatePanel.Visible = True

            ' Get the selected row (this is the row clicked before the button press)
            Dim row As DataGridViewRow = selectedCustomerRow

            ' Transfer data to the respective fields
            cusupdfirstname.Text = GetCellValue(row, "FirstName")
            cusupdlastname.Text = GetCellValue(row, "LastName")
            cusupdphonenum.Text = GetCellValue(row, "PhoneNumber")
            cusupdaddress.Text = GetCellValue(row, "Address")
            cusupdtablenum.Text = GetCellValue(row, "TableNumber")
            cusupditemsordered.Text = GetCellValue(row, "ItemsOrdered")
            cusupdquantity.Text = GetCellValue(row, "QTY")
            custranstypecmbbx.Text = GetCellValue(row, "TransactionType")
            cusupdorderstatus.Text = GetCellValue(row, "OrderStatus")
            cusupdsubtotal.Text = GetCellValue(row, "SubTotal")
            cusupdiscount.Text = GetCellValue(row, "DiscountAmount")
            cusupdnetamount.Text = GetCellValue(row, "NetAmount")
            cusupdpaymentmethodcmbbx.Text = GetCellValue(row, "PaymentMethod")
            cusupdreceiptnumber.Text = GetCellValue(row, "ReceiptNumber")
            cusupdrefundcmbbx.Text = GetCellValue(row, "RefundStatus")
            cusupdprocessedbycmbbx.Text = GetCellValue(row, "ProcessedBy")
            cusupdnotes.Text = GetCellValue(row, "Notes")
        Catch ex As Exception
            MessageBox.Show("Error loading selected customer data: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' Helper function to safely get cell value by column name
    Private Function GetCellValue(row As DataGridViewRow, columnName As String) As String
        If row.DataGridView.Columns.Contains(columnName) Then
            Return If(row.Cells(columnName).Value IsNot Nothing, row.Cells(columnName).Value.ToString(), "")
        Else
            Return ""
        End If
    End Function


    Private Sub cusupdatedbbtn_Click(sender As Object, e As EventArgs) Handles cusupdatedbbtn.Click
        ' Validate input data
        If String.IsNullOrWhiteSpace(cusupdfirstname.Text) Then
            MessageBox.Show("Please enter a first name.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        If String.IsNullOrWhiteSpace(cusupdlastname.Text) Then
            MessageBox.Show("Please enter a last name.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' Get values from CustomerUpdatePanel
        Dim firstName As String = cusupdfirstname.Text.Trim()
        Dim lastName As String = cusupdlastname.Text.Trim()
        Dim phoneNumber As String = cusupdphonenum.Text.Trim()
        Dim address As String = cusupdaddress.Text.Trim()
        Dim tableNumber As Integer = If(IsNumeric(cusupdtablenum.Text), Convert.ToInt32(cusupdtablenum.Text), 0)
        Dim itemsOrdered As String = cusupditemsordered.Text.Trim()
        Dim qty As Integer = If(IsNumeric(cusupdquantity.Text), Convert.ToInt32(cusupdquantity.Text), 0)
        Dim transactionType As String = custranstypecmbbx.Text.Trim()
        Dim orderStatus As String = cusupdorderstatus.Text.Trim()
        Dim subTotal As Decimal = If(IsNumeric(cusupdsubtotal.Text), Convert.ToDecimal(cusupdsubtotal.Text), 0)
        Dim discountAmount As Decimal = If(IsNumeric(cusupdiscount.Text), Convert.ToDecimal(cusupdiscount.Text), 0)
        Dim netAmount As Decimal = If(IsNumeric(cusupdnetamount.Text), Convert.ToDecimal(cusupdnetamount.Text), 0)
        Dim paymentMethod As String = cusupdpaymentmethodcmbbx.Text.Trim()
        Dim receiptNumber As String = cusupdreceiptnumber.Text.Trim()
        Dim refundStatus As String = cusupdrefundcmbbx.Text.Trim()
        Dim processedBy As String = cusupdprocessedbycmbbx.Text.Trim()
        Dim notes As String = cusupdnotes.Text.Trim()

        ' Get the selected customer ID
        Dim customerID As Integer = Convert.ToInt32(cusdgv.SelectedRows(0).Cells("CID").Value)

        ' Prepare SQL update query
        Dim query As String = "
        UPDATE customer SET
            FirstName = @FirstName,
            LastName = @LastName,
            PhoneNumber = @PhoneNumber,
            Address = @Address,
            TableNumber = @TableNumber,
            ItemsOrdered = @ItemsOrdered,
            QTY = @QTY,
            TransactionType = @TransactionType,
            OrderStatus = @OrderStatus,
            SubTotal = @SubTotal,
            DiscountAmount = @DiscountAmount,
            NetAmount = @NetAmount,
            PaymentMethod = @PaymentMethod,
            ReceiptNumber = @ReceiptNumber,
            RefundStatus = @RefundStatus,
            ProcessedBy = @ProcessedBy,
            Notes = @Notes
        WHERE CID = @CustomerID
    "

        Try
            Using connection As New MySqlConnection(strConnection)
                connection.Open()
                Using cmd As New MySqlCommand(query, connection)
                    cmd.Parameters.AddWithValue("@FirstName", firstName)
                    cmd.Parameters.AddWithValue("@LastName", lastName)
                    cmd.Parameters.AddWithValue("@PhoneNumber", phoneNumber)
                    cmd.Parameters.AddWithValue("@Address", address)
                    cmd.Parameters.AddWithValue("@TableNumber", tableNumber)
                    cmd.Parameters.AddWithValue("@ItemsOrdered", itemsOrdered)
                    cmd.Parameters.AddWithValue("@QTY", qty)
                    cmd.Parameters.AddWithValue("@TransactionType", transactionType)
                    cmd.Parameters.AddWithValue("@OrderStatus", orderStatus)
                    cmd.Parameters.AddWithValue("@SubTotal", subTotal)
                    cmd.Parameters.AddWithValue("@DiscountAmount", discountAmount)
                    cmd.Parameters.AddWithValue("@NetAmount", netAmount)
                    cmd.Parameters.AddWithValue("@PaymentMethod", paymentMethod)
                    cmd.Parameters.AddWithValue("@ReceiptNumber", receiptNumber)
                    cmd.Parameters.AddWithValue("@RefundStatus", refundStatus)
                    cmd.Parameters.AddWithValue("@ProcessedBy", processedBy)
                    cmd.Parameters.AddWithValue("@Notes", notes)
                    cmd.Parameters.AddWithValue("@CustomerID", customerID)

                    cmd.ExecuteNonQuery()
                End Using
            End Using

            MessageBox.Show("Customer record updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

            CustomerUpdatePanel.Visible = False
            LoadAllCustomers() ' Refresh the data grid with updated info
        Catch ex As Exception
            MessageBox.Show("Error updating customer record: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub cuscancelbtn_Click(sender As Object, e As EventArgs) Handles cuscancelbtn.Click
        CustomerUpdatePanel.Visible = False
    End Sub
    Private Sub cusdeletebtn_Click(sender As Object, e As EventArgs) Handles cusdeletebtn.Click
        ' Ensure a customer is selected
        If cusdgv.SelectedRows.Count = 0 Then
            MessageBox.Show("Please select a customer to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' Get the selected row
        Dim row As DataGridViewRow = cusdgv.SelectedRows(0)
        Dim customerID As Integer = Convert.ToInt32(row.Cells("CustomerID").Value) ' Replace with your actual column name for the customer ID

        ' Ask for confirmation
        Dim result As DialogResult = MessageBox.Show("Are you sure you want to delete this customer?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
        If result = DialogResult.Yes Then
            ' Execute delete query
            Dim query As String = "DELETE FROM Customers WHERE CustomerID = @CustomerID" ' Adjust table and column names as needed

            Try
                Using connection As New MySqlConnection(strConnection)
                    Using cmd As New MySqlCommand(query, connection)
                        cmd.Parameters.AddWithValue("@CustomerID", customerID)

                        connection.Open()
                        cmd.ExecuteNonQuery()
                    End Using
                End Using

                MessageBox.Show("Customer deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

                ' Reload the DataGridView to reflect changes
                LoadAllCustomers() ' Call this function to reload the customer data (implement this as needed)

            Catch ex As Exception
                MessageBox.Show("Error deleting customer: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub


    Private Sub InitializeTOreceiptColumns()
        If TOreceiptdgv.Columns.Count = 0 Then
            With TOreceiptdgv.Columns
                .Add("receipt_id", "Receipt No")
                .Add("qty", "Qty")
                .Add("item_name", "Item Name")
                .Add("price", "Price")
                .Add("total", "Total")
            End With

            ' Set each column's default properties (optional but cleaner)
            TOreceiptdgv.Columns("receipt_id").Width = 80
            TOreceiptdgv.Columns("qty").Width = 50
            TOreceiptdgv.Columns("item_name").Width = 150
            TOreceiptdgv.Columns("price").Width = 70
            TOreceiptdgv.Columns("total").Width = 70
        End If
        With TOreceiptdgv
            .ColumnHeadersHeight = 50 ' Adjust height
            .ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True
            .AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells
            .DefaultCellStyle.WrapMode = DataGridViewTriState.True
            .RowTemplate.Height = 30 ' Optional, for data rows
            .RowHeadersWidth = 70


        End With

    End Sub



    Private Sub LoadDataGridViewStyle()
        ' Assuming TOreceiptdgv is your DataGridView inside the TakeOrderTabControl
        With TOreceiptdgv
            ' Set column header height
            .ColumnHeadersHeight = 50

            ' Enable text wrapping in column headers
            .ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True

            ' Set row height to automatically adjust to content
            .AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells

            ' Enable text wrapping in cells
            .DefaultCellStyle.WrapMode = DataGridViewTriState.True

            ' Set row height
            .RowTemplate.Height = 30

            ' Set width of row headers
            .RowHeadersWidth = 70

            ' Add alternating row color for better readability
            .AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray

            ' Set the default font size for the grid for a better look
            .DefaultCellStyle.Font = New Font("Segoe UI", 10)

            ' Set background color of the DataGridView
            .BackgroundColor = Color.White

            ' Set the border style of the DataGridView
            .BorderStyle = BorderStyle.FixedSingle

            ' Set the column header style (for background color, text color, and font)
            .ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 122, 204) ' Blue color
            .ColumnHeadersDefaultCellStyle.ForeColor = Color.White
            .ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 10, FontStyle.Bold)

            ' Customize row selection behavior
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect
            .DefaultCellStyle.SelectionBackColor = Color.DodgerBlue
            .DefaultCellStyle.SelectionForeColor = Color.White

            ' Set vertical alignment of the text in the cells to center
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

            ' Set horizontal alignment of the text in the header to center
            .ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        End With
    End Sub
    Private Sub TakeOrderTabControl_SelectedIndexChanged(sender As Object, e As EventArgs) Handles TakeOrderTabControl.SelectedIndexChanged
        If TakeOrderTabControl.SelectedTab.Name = "TakeOrderTab" Then
            InitializeTOreceiptColumns() ' Ensure columns exist
            LoadDataGridViewStyle()

            ' Apply styles
        End If

    End Sub

    Private Sub TOreceiptdgvclear_Click(sender As Object, e As EventArgs)
        TOreceiptdgv.Rows.Clear()
    End Sub

    'Config singit lang
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



    Private Sub TOitemButton_Click(sender As Object, e As EventArgs) _
 Handles TOhitainasalbtn.Click, TOpitsoinasalbtn.Click, TOliempoinasalbtn.Click,
         TOporkchopbtn.Click, TObbqinasalbtn.Click, TOfriedchckbtn.Click,
         TObutteredchckbtn.Click, TOchckenteribtn.Click, TOchckenparmbtn.Click,
         TOlumpiabtn.Click, TOchopseuybtn.Click, TOlechonkwlbtn.Click, TOsisigbtn.Click,
         TOwater350btn.Click, Towater500btn.Click, TOcokemismobtn.Click,
         TOroyalmismobtn.Click, TOmdewmismobtn.Click, TOspritemismobtn.Click,
         TOcoke15btn.Click, TOroyal15btn.Click, TOsprite15btn.Click, TOsmgpilsenbtn.Click,
         TOrh500btn.Click, TOhalohalombtn.Click, TOhalohalolbtn.Click,
         TOchckenteriplatterbtn.Click, TOchckenparmplatterbtn.Click, TObttrdchckplatterbtn.Click,
         TOsisigplatterbtn.Click, TOchopseuyplatterbtn.Click, TOlumpiaplatterbtn.Click,
         TOpancitsmallbtn.Click, TOcreamycarbbtn.Click,
         TObenguetblendbtn.Click, TObarakobtn.Click, TOkalingarbstabtn.Click,
         TOfrenchrstbtn.Click, TOitalianespblendbtn.Click, TOhouseblndarabicabtn.Click,
         TObenguetarabicabtn.Click, TOpremiumbrkbtn.Click, TOsagadaarabicabtn.Click,
         TOprembenguetarabcabtn.Click, TObutterscotchbtn.Click, TOcaramelbtn.Click,
         TOcinnamonbtn.Click, TOdoublechocobtn.Click, TOhazelnutbtn.Click,
         TOirishcreambtn.Click, TOmacadamiabtn.Click, TOmocha.Click,
         TOvanillabtn.Click, TOhazelnutvanillabtn.Click, TOcookiesandcreambtn.Click,
         TObaileysbtn.Click

        Try
            Dim btn As Guna.UI2.WinForms.Guna2GradientButton = CType(sender, Guna.UI2.WinForms.Guna2GradientButton)
            Dim itemName As String = btn.Tag.ToString()
            Dim price As Decimal = 0
            Dim totalQtyAvailable As Integer = 0

            ' Ensure DGV columns exist
            InitializeTOreceiptColumns()

            ' 1. Connect to DB to get price and available quantity
            Using conn As New MySqlConnection(strConnection)
                conn.Open()
                Dim cmd As New MySqlCommand("SELECT price, TotalQuantityAvailable FROM item_breakdown WHERE ItemName = @name", conn)
                cmd.Parameters.AddWithValue("@name", itemName)
                Dim reader As MySqlDataReader = cmd.ExecuteReader()

                If reader.Read() Then
                    price = Convert.ToDecimal(reader("price"))
                    totalQtyAvailable = Convert.ToInt32(reader("TotalQuantityAvailable"))
                Else
                    MessageBox.Show("Item not found in the database: " & itemName)
                    Exit Sub
                End If
                reader.Close()
            End Using

            ' 2. Check quantity already in DGV
            Dim currentQtyInReceipt As Integer = 0
            For Each row As DataGridViewRow In TOreceiptdgv.Rows
                If Not row.IsNewRow AndAlso row.Cells("item_name").Value.ToString().Trim().ToLower() = itemName.Trim().ToLower() Then
                    currentQtyInReceipt += Convert.ToInt32(row.Cells("qty").Value)
                End If
            Next

            ' 3. Check for stock limit
            If currentQtyInReceipt >= totalQtyAvailable Then
                MessageBox.Show("Sold Out: " & itemName)
                Exit Sub
            End If

            ' 4. Update or add item
            Dim found As Boolean = False
            For Each row As DataGridViewRow In TOreceiptdgv.Rows
                If Not row.IsNewRow AndAlso row.Cells("item_name").Value.ToString().Trim().ToLower() = itemName.Trim().ToLower() Then
                    Dim currentQty As Integer = Convert.ToInt32(row.Cells("qty").Value)
                    row.Cells("qty").Value = currentQty + 1
                    row.Cells("total").Value = (currentQty + 1) * price
                    found = True
                    Exit For
                End If
            Next

            If Not found Then
                TOreceiptdgv.Rows.Add(Nothing, 1, itemName, price, price)
            End If
            UpdateReceiptSummary()

        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
        End Try
    End Sub

    Private Sub TOdonebtn_Click(sender As Object, e As EventArgs) Handles TOdonebtn.Click
        ' Declare needed variables
        Dim receiptNumber As String = ""
        Dim itemList As String = ""
        Dim qtyList As String = ""
        Dim tableNumber As String = TOtablenumbertxtbox.Text
        Dim transactionType As String = TOtransactiontypecmbbx.Text

        Try
            ' Input Validations
            If String.IsNullOrWhiteSpace(tableNumber) OrElse TOreceiptdgv.Rows.Count = 0 Then
                MessageBox.Show("No items in the receipt.")
                Exit Sub
            End If

            If Not Decimal.TryParse(TOmoneytextbox.Text, Nothing) Then
                MessageBox.Show("Invalid money input.")
                Exit Sub
            End If

            If String.IsNullOrWhiteSpace(TOpaymentmethodcmbbx.Text) OrElse String.IsNullOrWhiteSpace(transactionType) Then
                MessageBox.Show("Please select payment and transaction type.")
                Exit Sub
            End If

            Dim subtotal As Decimal = Decimal.Parse(TOSubtotalplcholderlbl.Text)
            Dim discount As Decimal = 0
            Decimal.TryParse(TOdiscounttextbox.Text, discount)

            If discount > subtotal Then
                MessageBox.Show("Discount cannot exceed the subtotal.")
                Exit Sub
            End If

            Dim netAmount As Decimal = subtotal - discount
            Dim moneyGiven As Decimal = Decimal.Parse(TOmoneytextbox.Text)

            If moneyGiven < netAmount Then
                MessageBox.Show("Insufficient payment.")
                Exit Sub
            End If

            receiptNumber = GenerateNextReceiptNumber()

            ' Extract items and quantities
            Dim itemNames As New List(Of String)
            Dim quantities As New List(Of String)

            For Each row As DataGridViewRow In TOreceiptdgv.Rows
                If Not row.IsNewRow Then
                    itemNames.Add(row.Cells("item_name").Value.ToString())
                    quantities.Add(row.Cells("qty").Value.ToString())
                End If
            Next

            itemList = String.Join(", ", itemNames)
            qtyList = String.Join(", ", quantities)
            Dim processedBy As String = Environment.UserName & " (" & CurrentUserRole & ")"

            ' Insert into Customer Table
            Dim cid As Integer
            Using conn As New MySqlConnection(strConnection)
                conn.Open()
                Dim custCmd As New MySqlCommand("INSERT INTO customer 
                (FirstName, LastName, PhoneNumber, Address, TableNumber, ItemsOrdered, QTY, TransactionType, 
                OrderStatus, TimeOrdered, DateOrdered, SubTotal, DiscountAmount, NetAmount, PaymentMethod, 
                ReceiptNumber, RefundStatus, DateRecorded, ProcessedBy, Notes) 
                VALUES 
                (@fn, @ln, @ph, @addr, @tbl, @items, @qty, @ttype, 'Pending', NOW(), NOW(), @sub, @disc, @net, 
                @pmethod, @rnum, 'No', NOW(), @user, '')", conn)

                With custCmd.Parameters
                    .AddWithValue("@fn", If(String.IsNullOrWhiteSpace(TOfirstnametxtbox.Text), DBNull.Value, TOfirstnametxtbox.Text))
                    .AddWithValue("@ln", If(String.IsNullOrWhiteSpace(TOlastnametxtbox.Text), DBNull.Value, TOlastnametxtbox.Text))
                    .AddWithValue("@ph", If(String.IsNullOrWhiteSpace(TOphonenumtxtbox.Text), DBNull.Value, TOphonenumtxtbox.Text))
                    .AddWithValue("@addr", If(String.IsNullOrWhiteSpace(TOaddresstxtbox.Text), DBNull.Value, TOaddresstxtbox.Text))
                    .AddWithValue("@tbl", tableNumber)
                    .AddWithValue("@items", itemList)
                    .AddWithValue("@qty", qtyList)
                    .AddWithValue("@ttype", transactionType)
                    .AddWithValue("@sub", subtotal)
                    .AddWithValue("@disc", If(discount = 0, DBNull.Value, discount))
                    .AddWithValue("@net", netAmount)
                    .AddWithValue("@pmethod", TOpaymentmethodcmbbx.Text)
                    .AddWithValue("@rnum", receiptNumber)
                    .AddWithValue("@user", processedBy)
                End With

                custCmd.ExecuteNonQuery()
                cid = CType(custCmd.LastInsertedId, Integer)
            End Using

            ' Insert into Sales Table
            Using conn As New MySqlConnection(strConnection)
                conn.Open()
                Dim salesCmd As New MySqlCommand("INSERT INTO sales 
                (CID, ReceiptNumber, TimeOrdered, DateOrdered, ItemsOrdered, QTY, SubTotal, 
                DiscountAmount, NetAmount, PaymentMethod, TransactionType, OrderStatus, RefundStatus, 
                DateRecorded, ProcessedBy, Notes) 
                VALUES 
                (@cid, @rnum, NOW(), NOW(), @items, @qty, @sub, @disc, @net, @pmethod, 
                @ttype, 'Pending', 'No', NOW(), @user, '')", conn)

                With salesCmd.Parameters
                    .AddWithValue("@cid", cid)
                    .AddWithValue("@rnum", receiptNumber)
                    .AddWithValue("@items", itemList)
                    .AddWithValue("@qty", qtyList)
                    .AddWithValue("@sub", subtotal)
                    .AddWithValue("@disc", If(discount = 0, DBNull.Value, discount))
                    .AddWithValue("@net", netAmount)
                    .AddWithValue("@pmethod", TOpaymentmethodcmbbx.Text)
                    .AddWithValue("@ttype", transactionType)
                    .AddWithValue("@user", processedBy)
                End With

                salesCmd.ExecuteNonQuery()
            End Using

            ' Update Inventory
            For Each row As DataGridViewRow In TOreceiptdgv.Rows
                If Not row.IsNewRow Then
                    Dim itemName As String = row.Cells("item_name").Value.ToString()
                    Dim qtyToSubtract As Integer = Convert.ToInt32(row.Cells("qty").Value)

                    Using conn As New MySqlConnection(strConnection)
                        conn.Open()
                        Dim updateCmd As New MySqlCommand("UPDATE item_breakdown 
                        SET TotalQuantityAvailable = TotalQuantityAvailable - @qty 
                        WHERE ItemName = @name", conn)
                        updateCmd.Parameters.AddWithValue("@qty", qtyToSubtract)
                        updateCmd.Parameters.AddWithValue("@name", itemName)
                        updateCmd.ExecuteNonQuery()
                    End Using
                End If
            Next

            ' Show change
            Dim changeDue As Decimal = moneyGiven - netAmount
            MessageBox.Show("Transaction complete!" & vbCrLf & "Change: ₱" & changeDue.ToString("F2"))

            ' Reset form
            TOfirstnametxtbox.Clear()
            TOlastnametxtbox.Clear()
            TOphonenumtxtbox.Clear()
            TOaddresstxtbox.Clear()
            TOtablenumbertxtbox.Clear()
            TOmoneytextbox.Clear()
            TOdiscounttextbox.Clear()
            TOpaymentmethodcmbbx.SelectedIndex = -1
            TOtransactiontypecmbbx.SelectedIndex = -1
            TOSubtotalplcholderlbl.Text = "0.00"
            TOreceiptdgv.Rows.Clear()

            ' Generate PDF receipt
            MessageBox.Show("Generating PDF receipt...", "Receipt", MessageBoxButtons.OK, MessageBoxIcon.Information)

            Try
                Dim downloadsPath As String = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) & "\Downloads\"
                Dim pdfFileName As String = $"Receipt_{receiptNumber}_{DateTime.Now:yyyyMMddHHmmss}.pdf"
                Dim fullPath As String = Path.Combine(downloadsPath, pdfFileName)

                Dim printDoc As New Printing.PrintDocument()
                AddHandler printDoc.PrintPage, Sub(senderDoc, eDoc)
                                                   PrintReceiptPage(senderDoc, eDoc, receiptNumber, tableNumber,
                                                   TOfirstnametxtbox.Text, TOlastnametxtbox.Text,
                                                   TOphonenumtxtbox.Text, TOaddresstxtbox.Text,
                                                   transactionType, TOpaymentmethodcmbbx.Text,
                                                   subtotal, discount, netAmount, moneyGiven, changeDue,
                                                   TOreceiptdgv)
                                               End Sub

                printDoc.PrinterSettings.PrinterName = "Microsoft Print to PDF"
                printDoc.PrinterSettings.PrintToFile = True
                printDoc.PrinterSettings.PrintFileName = fullPath

                printDoc.Print()

                MessageBox.Show($"Receipt saved as PDF:{vbCrLf}{fullPath}", "Receipt Printed", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Catch exPrint As Exception
                MessageBox.Show($"Error generating PDF receipt:{vbCrLf}{exPrint.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try

        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
            Exit Sub
        End Try



    End Sub
    Private Sub LoadPendingOrders()
        servingstbllayout.Controls.Clear()

        Using conn As New MySqlConnection(strConnection)
            conn.Open()
            Dim cmd As New MySqlCommand("
                SELECT s.CID, c.TableNumber, s.ItemsOrdered, s.QTY, s.TransactionType 
                FROM sales s 
                INNER JOIN customer c ON s.CID = c.CID
                WHERE s.OrderStatus = 'Pending'
            ", conn)

            Using reader As MySqlDataReader = cmd.ExecuteReader()
                While reader.Read()
                    Dim cardPanel As New Panel()
                    cardPanel.Size = New Size(260, 140)
                    cardPanel.BackColor = Color.LightYellow
                    cardPanel.BorderStyle = BorderStyle.FixedSingle
                    cardPanel.Margin = New Padding(10)

                    ' Table number
                    Dim lblTable As New Label()
                    lblTable.Text = "Table #: " & reader("TableNumber").ToString()
                    lblTable.Font = New Font("Segoe UI", 10, FontStyle.Bold)
                    lblTable.Location = New Point(10, 10)
                    lblTable.AutoSize = True

                    ' Order details
                    Dim txtDetails As New TextBox()
                    txtDetails.Multiline = True
                    txtDetails.ReadOnly = True
                    txtDetails.Location = New Point(10, 35)
                    txtDetails.Size = New Size(240, 60)
                    txtDetails.BackColor = Color.LightYellow
                    txtDetails.BorderStyle = BorderStyle.None

                    ' Items + quantities
                    Dim items As String() = reader("ItemsOrdered").ToString().Split(","c)
                    Dim qtys As String() = reader("QTY").ToString().Split(","c)
                    For i As Integer = 0 To Math.Min(items.Length - 1, qtys.Length - 1)
                        txtDetails.Text &= items(i).Trim() & " x" & qtys(i).Trim() & Environment.NewLine
                    Next

                    ' Transaction type
                    Dim lblType As New Label()
                    lblType.Text = "Type: " & reader("TransactionType").ToString()
                    lblType.Location = New Point(10, 100)
                    lblType.AutoSize = True

                    ' Buttons
                    Dim btnServe As New Button()
                    btnServe.Text = "Serve"
                    btnServe.Size = New Size(75, 25)
                    btnServe.Location = New Point(130, 100)

                    Dim btnCancel As New Button()
                    btnCancel.Text = "Cancel"
                    btnCancel.Size = New Size(75, 25)
                    btnCancel.Location = New Point(210, 100)

                    Dim cid As Integer = Convert.ToInt32(reader("CID"))

                    ' Serve handler
                    AddHandler btnServe.Click, Sub()
                                                   MarkOrderStatus(cid, "Served")
                                                   LoadPendingOrders()
                                               End Sub

                    ' Cancel handler
                    AddHandler btnCancel.Click, Sub()
                                                    Dim result = MessageBox.Show("Are you sure you want to cancel this order?", "Cancel Order", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                                                    If result = DialogResult.Yes Then
                                                        MarkOrderStatus(cid, "Cancelled")
                                                        LoadPendingOrders()
                                                    End If
                                                End Sub

                    ' Add to panel
                    cardPanel.Controls.Add(lblTable)
                    cardPanel.Controls.Add(txtDetails)
                    cardPanel.Controls.Add(lblType)
                    cardPanel.Controls.Add(btnServe)
                    cardPanel.Controls.Add(btnCancel)

                    ' Add panel to flow layout
                    servingstbllayout.Controls.Add(cardPanel)
                End While
            End Using
        End Using
    End Sub


    Private Sub MarkOrderStatus(cid As Integer, status As String)
        Using conn As New MySqlConnection(strConnection)
            conn.Open()
            Dim cmd As New MySqlCommand("UPDATE customer SET OrderStatus = @status WHERE CID = @cid", conn)
            If status = "Served" Then
                ' Actually store "Completed" in the database
                cmd.Parameters.AddWithValue("@status", "Completed")
            Else
                cmd.Parameters.AddWithValue("@status", "Cancelled")
            End If
            cmd.Parameters.AddWithValue("@cid", cid)
            cmd.ExecuteNonQuery()
        End Using
    End Sub




    Private Sub PrintReceiptPage(sender As Object, e As Printing.PrintPageEventArgs,
                           receiptNumber As String, tableNumber As String,
                           firstName As String, lastName As String,
                           phoneNumber As String, address As String,
                           transactionType As String, paymentMethod As String,
                           subtotal As Decimal, discount As Decimal,
                           netAmount As Decimal, moneyGiven As Decimal,
                           changeDue As Decimal, dgv As DataGridView)
        ' Fonts
        Dim titleFont As New Font("Arial", 18, FontStyle.Bold)
        Dim headerFont As New Font("Arial", 12, FontStyle.Bold)
        Dim regularFont As New Font("Arial", 10)
        Dim smallFont As New Font("Arial", 8)

        ' Margins and positioning
        Dim leftMargin As Integer = e.MarginBounds.Left
        Dim topMargin As Integer = e.MarginBounds.Top
        Dim yPos As Integer = topMargin

        ' Business header
        e.Graphics.DrawString("YOUR BUSINESS NAME", titleFont, Brushes.Black, leftMargin, yPos)
        yPos += 30
        e.Graphics.DrawString("Business Address", regularFont, Brushes.Black, leftMargin, yPos)
        yPos += 20
        e.Graphics.DrawString("Contact Information", regularFont, Brushes.Black, leftMargin, yPos)
        yPos += 30

        ' Receipt header
        e.Graphics.DrawString("RECEIPT", headerFont, Brushes.Black, leftMargin, yPos)
        yPos += 25
        e.Graphics.DrawString($"Receipt #: {receiptNumber}", regularFont, Brushes.Black, leftMargin, yPos)
        e.Graphics.DrawString($"Date: {DateTime.Now:yyyy-MM-dd HH:mm}", regularFont, Brushes.Black, leftMargin + 200, yPos)
        yPos += 20

        ' Customer info (only show if available)
        If Not String.IsNullOrWhiteSpace(firstName) Or Not String.IsNullOrWhiteSpace(lastName) Then
            e.Graphics.DrawString($"Customer: {firstName} {lastName}", regularFont, Brushes.Black, leftMargin, yPos)
            yPos += 20
        End If

        If Not String.IsNullOrWhiteSpace(phoneNumber) Then
            e.Graphics.DrawString($"Phone: {phoneNumber}", regularFont, Brushes.Black, leftMargin, yPos)
            yPos += 20
        End If

        If Not String.IsNullOrWhiteSpace(address) Then
            e.Graphics.DrawString($"Address: {address}", regularFont, Brushes.Black, leftMargin, yPos)
            yPos += 20
        End If

        e.Graphics.DrawString($"Table #: {tableNumber}", regularFont, Brushes.Black, leftMargin, yPos)
        e.Graphics.DrawString($"Type: {transactionType}", regularFont, Brushes.Black, leftMargin + 200, yPos)
        yPos += 30

        ' Items header
        e.Graphics.DrawString("ITEM", headerFont, Brushes.Black, leftMargin, yPos)
        e.Graphics.DrawString("QTY", headerFont, Brushes.Black, leftMargin + 200, yPos)
        e.Graphics.DrawString("PRICE", headerFont, Brushes.Black, leftMargin + 250, yPos)
        yPos += 20

        ' Draw line
        e.Graphics.DrawLine(Pens.Black, leftMargin, yPos, leftMargin + 350, yPos)
        yPos += 10

        ' Items list
        For Each row As DataGridViewRow In dgv.Rows
            If Not row.IsNewRow Then
                Dim itemName As String = row.Cells("item_name").Value.ToString()
                Dim quantity As String = row.Cells("qty").Value.ToString()
                Dim price As Decimal = Decimal.Parse(row.Cells("price").Value.ToString())
                Dim total As Decimal = quantity * price

                ' Wrap text if needed
                Dim itemNameRect As New RectangleF(leftMargin, yPos, 180, 40)
                e.Graphics.DrawString(itemName, regularFont, Brushes.Black, itemNameRect)
                e.Graphics.DrawString(quantity, regularFont, Brushes.Black, leftMargin + 200, yPos)
                e.Graphics.DrawString(price.ToString("F2"), regularFont, Brushes.Black, leftMargin + 250, yPos)

                ' Adjust yPos based on text height
                Dim textHeight As Integer = CInt(e.Graphics.MeasureString(itemName, regularFont, 180).Height)
                yPos += Math.Max(textHeight, 20)
            End If
        Next

        ' Draw line
        yPos += 10
        e.Graphics.DrawLine(Pens.Black, leftMargin, yPos, leftMargin + 350, yPos)
        yPos += 20

        ' Totals
        e.Graphics.DrawString("Subtotal:", headerFont, Brushes.Black, leftMargin + 150, yPos)
        e.Graphics.DrawString(subtotal.ToString("F2"), regularFont, Brushes.Black, leftMargin + 250, yPos)
        yPos += 20

        e.Graphics.DrawString("Discount:", headerFont, Brushes.Black, leftMargin + 150, yPos)
        e.Graphics.DrawString(discount.ToString("F2"), regularFont, Brushes.Black, leftMargin + 250, yPos)
        yPos += 20

        e.Graphics.DrawString("Net Total:", headerFont, Brushes.Black, leftMargin + 150, yPos)
        e.Graphics.DrawString(netAmount.ToString("F2"), regularFont, Brushes.Black, leftMargin + 250, yPos)
        yPos += 20

        e.Graphics.DrawString("Payment Method:", headerFont, Brushes.Black, leftMargin + 100, yPos)
        e.Graphics.DrawString(paymentMethod, regularFont, Brushes.Black, leftMargin + 250, yPos)
        yPos += 20

        e.Graphics.DrawString("Amount Paid:", headerFont, Brushes.Black, leftMargin + 150, yPos)
        e.Graphics.DrawString(moneyGiven.ToString("F2"), regularFont, Brushes.Black, leftMargin + 250, yPos)
        yPos += 20

        e.Graphics.DrawString("Change:", headerFont, Brushes.Black, leftMargin + 150, yPos)
        e.Graphics.DrawString(changeDue.ToString("F2"), regularFont, Brushes.Black, leftMargin + 250, yPos)
        yPos += 30

        ' Footer
        e.Graphics.DrawString("Thank you for your business!", headerFont, Brushes.Black, leftMargin + 50, yPos)
        yPos += 20
        e.Graphics.DrawString("Processed by: Jennie Adlawan ", smallFont, Brushes.Black, leftMargin, yPos)
    End Sub



    Private Function GenerateNextReceiptNumber() As String
        Try
            Using conn As New MySqlConnection(strConnection)
                conn.Open()
                ' Get the maximum ReceiptNumber from the sales table
                Dim cmd As New MySqlCommand("SELECT MAX(ReceiptNumber) FROM sales", conn)
                Dim maxReceipt As Object = cmd.ExecuteScalar()

                Dim nextNumber As Integer = 1

                If maxReceipt IsNot DBNull.Value AndAlso maxReceipt IsNot Nothing Then
                    Dim maxReceiptStr As String = maxReceipt.ToString()
                    ' Extract numeric part after "RN-"
                    Dim numericPart As String = maxReceiptStr.Replace("RN-", "")
                    If Integer.TryParse(numericPart, nextNumber) Then
                        nextNumber += 1
                    Else
                        nextNumber = 1
                    End If
                End If

                Return "RN-" & nextNumber.ToString("0000")
            End Using
        Catch ex As Exception
            MessageBox.Show("Error generating receipt number: " & ex.Message)
            Return "RN-9999"
        End Try
    End Function



    Private Sub TOreceiptdgv_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles TOreceiptdgv.CellValueChanged
        Try
            ' Check if the changed cell is the "qty" column
            If e.RowIndex >= 0 AndAlso TOreceiptdgv.Columns(e.ColumnIndex).Name = "qty" Then

                RecalculateTotals() ' Recalculate the totals whenever quantity changes
            End If
        Catch ex As Exception
            MessageBox.Show("Error updating totals: " & ex.Message)
        End Try
    End Sub

    Private Sub TOreceiptdgv_CurrentCellDirtyStateChanged(sender As Object, e As EventArgs) Handles TOreceiptdgv.CurrentCellDirtyStateChanged
        ' Commit changes to the cell if it is dirty (modified)
        If TOreceiptdgv.IsCurrentCellDirty Then
            TOreceiptdgv.CommitEdit(DataGridViewDataErrorContexts.Commit)
        End If
    End Sub

    Private Sub RecalculateTotals()
        Dim subtotal As Decimal = 0

        ' Loop through each row and calculate the subtotal based on qty * price
        For Each row As DataGridViewRow In TOreceiptdgv.Rows
            If Not row.IsNewRow Then
                Dim qty As Integer = Convert.ToInt32(row.Cells("qty").Value)
                Dim price As Decimal = Convert.ToDecimal(row.Cells("price").Value) ' Assuming price is the column name
                subtotal += qty * price
            End If
        Next

        TOSubtotalplcholderlbl.Text = subtotal.ToString("F2")

        ' Calculate discount and update the net amount
        Dim discount As Decimal = 0
        Decimal.TryParse(TOdiscounttextbox.Text, discount)

        TOnetamounttotalplcholderlbl.Text = (subtotal - discount).ToString("F2")
    End Sub

    Private Sub UpdateReceiptSummary()
        Try
            Dim subtotal As Decimal = 0
            For Each row As DataGridViewRow In TOreceiptdgv.Rows
                If Not row.IsNewRow Then
                    subtotal += Convert.ToDecimal(row.Cells("total").Value)
                End If
            Next

            TOSubtotalplcholderlbl.Text = subtotal.ToString("F2")

            ' Get discount and calculate net amount
            Dim discount As Decimal = 0
            If Not String.IsNullOrWhiteSpace(TOdiscounttextbox.Text) Then
                Decimal.TryParse(TOdiscounttextbox.Text, discount)
            End If

            Dim netAmount As Decimal = subtotal - discount
            TOnetamounttotalplcholderlbl.Text = netAmount.ToString("F2")
        Catch ex As Exception
            MsgBox("Error updating totals: " & ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub

    Private Sub TOdiscounttextbox_TextChanged(sender As Object, e As EventArgs) Handles TOdiscounttextbox.TextChanged
        RecalculateTotals() ' Recalculate totals whenever the discount changes
    End Sub

    Private Sub TOreceiptdgvclear_Click_1(sender As Object, e As EventArgs) Handles TOreceiptdgvclear.Click
        ' Clear the receipt and totals when the clear button is clicked
        TOreceiptdgv.Rows.Clear()
        TOSubtotalplcholderlbl.Text = "0.00"
        TOnetamounttotalplcholderlbl.Text = "0.00"
    End Sub

    Private Sub TOreceiptdgv_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles TOreceiptdgv.CellClick
        ' Prevent header row clicks
        If e.RowIndex >= 0 Then
            ' Check if the "Quantity" column is clicked
            If TOreceiptdgv.Columns.Contains("qty") Then
                ' Get the quantity value
                Dim qty As Integer = Convert.ToInt32(TOreceiptdgv.Rows(e.RowIndex).Cells("qty").Value)

                ' If the quantity is 1, remove the row from the DataGridView
                If qty = 1 Then
                    TOreceiptdgv.Rows.RemoveAt(e.RowIndex)
                    UpdateReceiptSummary()
                    ' Update subtotal after removing the row
                Else
                    ' Otherwise, set the quantity to 1
                    TOreceiptdgv.Rows(e.RowIndex).Cells("qty").Value = 1
                End If
            Else
                ' Fallback: Use column index (assuming "Quantity" is the 2nd column, adjust as necessary)
                Dim qty As Integer = Convert.ToInt32(TOreceiptdgv.Rows(e.RowIndex).Cells(2).Value)

                ' If the quantity is 1, remove the row
                If qty = 1 Then
                    TOreceiptdgv.Rows.RemoveAt(e.RowIndex)
                    UpdateReceiptSummary() ' Update subtotal after removing the row
                Else
                    ' Otherwise, set the quantity to 1
                    TOreceiptdgv.Rows(e.RowIndex).Cells(2).Value = 1
                End If
            End If
        End If
    End Sub

    Private Sub TOreceiptdgv_CellMouseUp(sender As Object, e As DataGridViewCellMouseEventArgs) Handles TOreceiptdgv.CellMouseUp
        ' Handle the mouse up event to update the quantity to 1
        If e.RowIndex >= 0 Then
            ' Commit the edit first if the user clicked a cell
            TOreceiptdgv.EndEdit()

            ' Update the quantity to 1
            If TOreceiptdgv.Columns.Contains("qty") Then
                TOreceiptdgv.Rows(e.RowIndex).Cells("qty").Value = 1
            Else
                ' Use column index if column name is not found
                TOreceiptdgv.Rows(e.RowIndex).Cells(2).Value = 1
            End If
        End If
    End Sub
    Private Sub TOreceiptPDF_Click(sender As Object, e As EventArgs)
        Try
            ' Create a new PrintDocument
            Dim printDocument As New Printing.PrintDocument()

            ' Attach the PrintPage event handler
            AddHandler printDocument.PrintPage, AddressOf PrintDocument_PrintPage

            ' Create and configure the print dialog
            Dim printDialog As New PrintDialog()
            printDialog.Document = printDocument

            ' Show dialog and print if confirmed
            If printDialog.ShowDialog() = DialogResult.OK Then
                printDocument.Print()
            End If
        Catch ex As Exception
            MessageBox.Show("An error occurred while printing: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub PrintDocument_PrintPage(sender As Object, e As Printing.PrintPageEventArgs)
        Try
            ' Create a bitmap with the size of the panel
            Dim bmp As New Bitmap(receiptdgvtablelayout.Width, receiptdgvtablelayout.Height)

            ' Draw the panel content to the bitmap
            receiptdgvtablelayout.DrawToBitmap(bmp, New Rectangle(0, 0, Panel1.Width, Panel1.Height))

            ' Print the bitmap on the page
            e.Graphics.DrawImage(bmp, 0, 0)
        Catch ex As Exception
            MessageBox.Show("An error occurred during rendering: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    'Expenses
    Private Sub LoadExpenses()
        If expcmbbx.SelectedItem Is Nothing Then Return

        Dim selectedRange As String = expcmbbx.SelectedItem.ToString()
        Try
            Dim filter As String = expcmbbx.SelectedItem.ToString()
            Dim searchTerm As String = expsearchbox.Text.Trim()
            Dim query As String = "
            SELECT 
                ExID AS 'ID',
                ItemName AS 'Item Name',
                Qty AS 'Quantity',
                UnitOfMeasure AS 'Unit',
                Amount AS 'Amount',
                Description,
                DateIncurred AS 'Date',
                PaymentMethod AS 'Payment Method',
                ReceiptNumber AS 'Receipt #',
                PaymentReferenceNumber AS 'Ref #',
                Total,
                DateRecorded AS 'Recorded',
                ProcessedBy,
                Notes
            FROM expensesoverview
            WHERE 1 = 1
        "

            ' Time-based filtering
            Select Case filter
                Case "Day"
                    query &= " AND DateIncurred = CURDATE()"
                Case "Week"
                    query &= " AND WEEK(DateIncurred) = WEEK(CURDATE()) AND YEAR(DateIncurred) = YEAR(CURDATE())"
                Case "Month"
                    query &= " AND MONTH(DateIncurred) = MONTH(CURDATE()) AND YEAR(DateIncurred) = YEAR(CURDATE())"
                Case "Year"
                    query &= " AND YEAR(DateIncurred) = YEAR(CURDATE())"
            End Select

            ' Search filter
            If Not String.IsNullOrEmpty(searchTerm) Then
                query &= $" AND (Description LIKE '%{searchTerm}%' OR PaymentMethod LIKE '%{searchTerm}%' OR ReceiptNumber LIKE '%{searchTerm}%' OR Notes LIKE '%{searchTerm}%')"
            End If

            query &= " ORDER BY DateIncurred DESC
            LIMIT 10"

            LoadToDGV(query, expdgv)

            ' Format the DGV
            With expdgv
                .ColumnHeadersHeight = 50
                .ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True
                .AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells
                .DefaultCellStyle.WrapMode = DataGridViewTriState.True
                .RowTemplate.Height = 30
                .RowHeadersWidth = 70
            End With

            UpdateExpensesChartAndTotal()
        Catch ex As Exception
            MsgBox("An error occurred while loading expenses: " & ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub
    Private Sub UpdateExpensesChartAndTotal()
        Try
            expchart.Series.Clear()
            Dim totalAmount As Decimal = 0
            Dim categoryTotals As New Dictionary(Of String, Decimal)

            For Each row As DataGridViewRow In expdgv.Rows
                If Not row.IsNewRow Then
                    Dim amount As Decimal = Convert.ToDecimal(row.Cells("Total").Value)
                    Dim description As String = row.Cells("Description").Value.ToString()

                    totalAmount += amount

                    If categoryTotals.ContainsKey(description) Then
                        categoryTotals(description) += amount
                    Else
                        categoryTotals(description) = amount
                    End If
                End If
            Next

            ' Add data to chart
            Dim series = expchart.Series.Add("Expenses")
            For Each kvp In categoryTotals
                series.Points.AddXY(kvp.Key, kvp.Value)
            Next

            ' Update total label
            exptotalintab.Text = "₱ " & totalAmount.ToString("N2")
        Catch ex As Exception
            MsgBox("An error occurred while updating chart or total: " & ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub
    Private Sub expcmbbx_SelectedIndexChanged(sender As Object, e As EventArgs) Handles expcmbbx.SelectedIndexChanged
        LoadExpenses()
    End Sub

    Private Sub expsearchbox_TextChanged(sender As Object, e As EventArgs)
        LoadExpenses()
    End Sub

    Private Sub expsearchbtn_Click(sender As Object, e As EventArgs) Handles expsearchbtn.Click
        LoadExpenses()
    End Sub

    Private Sub ExpensesPanel_Enter(sender As Object, e As EventArgs) Handles ExpensesPanel.Enter
        If expcmbbx.Items.Count = 0 Then
            expcmbbx.Items.AddRange(New String() {"Day", "Week", "Month", "Year"})
            expcmbbx.SelectedIndex = 0
        End If
        LoadExpenses()
    End Sub


    Private Sub AddExpense()
        ' Get values from the form
        Dim itemName As String = expitemnametxtbx.Text.Trim()
        Dim qty As Decimal
        Dim unitOfMeasure As String = expunitofmeasurecmbbx.Text
        Dim amount As Decimal
        Dim paymentMethod As String = exptranstypecmbbx.Text
        Dim receiptNumber As String = expreceiptnumbertxtbx.Text.Trim()
        Dim referenceNumber As String = exprefenumbertxtbx.Text.Trim()
        Dim notes As String = expnotestxtbx.Text.Trim()
        Dim processedBy As String = CurrentUserRole ' Assuming this is declared somewhere
        Dim dateIncurred As String = DateTime.Now.ToString("yyyy-MM-dd")
        Dim dateRecorded As String = DateTime.Now.ToString("yyyy-MM-dd")
        Dim itemID As Object = DBNull.Value
        Dim total As Decimal = 0D

        ' Input validation
        If Not Decimal.TryParse(expqtytxtbx.Text, qty) Then
            MessageBox.Show("Please enter a valid quantity.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If Not Decimal.TryParse(expamounttxtbx.Text, amount) Then
            MessageBox.Show("Please enter a valid amount.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        total = qty * amount

        ' Optional logic: if itemName matches something in inventory, get ItemID
        Dim invQuery As String = "SELECT ItemID FROM inventory WHERE ItemName LIKE @itemName LIMIT 1"

        Using connection As New MySqlConnection(conn.ConnectionString)
            connection.Open()

            ' Check inventory match
            Using cmdCheck As New MySqlCommand(invQuery, connection)
                cmdCheck.Parameters.AddWithValue("@itemName", "%" & itemName & "%")
                Dim reader = cmdCheck.ExecuteReader()
                If reader.Read() Then
                    itemID = reader("ItemID").ToString()
                End If
                reader.Close()
            End Using

            ' Now insert the expense
            Dim insertQuery As String = "
        INSERT INTO expensesoverview
        ( ItemID, ItemName, Qty, UnitOfMeasure, Amount, Description, DateIncurred, 
         PaymentMethod, ReceiptNumber, PaymentReferenceNumber, Total, DateRecorded, ProcessedBy, Notes)
        VALUES
        ( @ItemID, @ItemName, @Qty, @UnitOfMeasure, @Amount, @Description, @DateIncurred, 
         @PaymentMethod, @ReceiptNumber, @PaymentReferenceNumber, @Total, @DateRecorded, @ProcessedBy, @Notes)
        "

            Using cmd As New MySqlCommand(insertQuery, connection)

                cmd.Parameters.AddWithValue("@ItemID", If(String.IsNullOrEmpty(itemID.ToString()), DBNull.Value, itemID))
                cmd.Parameters.AddWithValue("@ItemName", itemName)
                cmd.Parameters.AddWithValue("@Qty", qty)
                cmd.Parameters.AddWithValue("@UnitOfMeasure", unitOfMeasure)
                cmd.Parameters.AddWithValue("@Amount", amount)
                cmd.Parameters.AddWithValue("@Description", itemName)
                cmd.Parameters.AddWithValue("@DateIncurred", dateIncurred)
                cmd.Parameters.AddWithValue("@PaymentMethod", paymentMethod)
                cmd.Parameters.AddWithValue("@ReceiptNumber", If(paymentMethod = "Cash", receiptNumber, ""))
                cmd.Parameters.AddWithValue("@PaymentReferenceNumber", If(paymentMethod = "Gcash", referenceNumber, ""))
                cmd.Parameters.AddWithValue("@Total", total)
                cmd.Parameters.AddWithValue("@DateRecorded", dateRecorded)
                cmd.Parameters.AddWithValue("@ProcessedBy", processedBy)
                cmd.Parameters.AddWithValue("@Notes", notes)

                Try
                    cmd.ExecuteNonQuery()
                    MessageBox.Show("Expense added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    ExpensesAddPanel.Visible = False
                    LoadExpenses() ' if you have this
                Catch ex As Exception
                    MessageBox.Show("Error inserting expense: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End Using
        End Using
    End Sub
    Private Sub expaddbutton_Click(sender As Object, e As EventArgs) Handles expaddbutton.Click
        expitemnametxtbx.Text = ""
        expqtytxtbx.Text = "1"
        expunitofmeasurecmbbx.SelectedIndex = 0
        exptranstypecmbbx.SelectedIndex = 0 ' Defaults to "Cash"
        expamounttxtbx.Text = "0.00"
        expreceiptnumbertxtbx.Text = ""
        exprefenumbertxtbx.Text = ""
        expnotestxtbx.Text = ""

        ExpensesAddPanel.Visible = True
    End Sub

    Private Sub exptranstypecmbbx_SelectedIndexChanged(sender As Object, e As EventArgs) Handles exptranstypecmbbx.SelectedIndexChanged
        MessageBox.Show("Payment method changed to: " & exptranstypecmbbx.Text)

        Select Case exptranstypecmbbx.Text.Trim().ToLower()
            Case "gcash"
                exprefenumbertxtbx.Enabled = True
                expreceiptnumbertxtbx.Enabled = False
            Case "cash"
                exprefenumbertxtbx.Enabled = False
                expreceiptnumbertxtbx.Enabled = True
            Case Else
                exprefenumbertxtbx.Enabled = False
                expreceiptnumbertxtbx.Enabled = False
        End Select
    End Sub


    Private Sub expadddbbtn_Click(sender As Object, e As EventArgs) Handles expadddbbtn.Click
        AddExpense()
    End Sub
    Private Sub expcancelbtn_Click(sender As Object, e As EventArgs) Handles expcancelbtn.Click
        ExpensesAddPanel.Visible = False
    End Sub

    'update of expenses
    Private Sub expupdatebutton_Click(sender As Object, e As EventArgs) Handles expupdatebutton.Click
        If expdgv.SelectedRows.Count = 0 Then
            MessageBox.Show("Please select an expense to update.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim selectedRow As DataGridViewRow = expdgv.SelectedRows(0)

        ' Fill the fields with selected row data
        expupditemnametxtbx.Text = selectedRow.Cells(1).Value?.ToString()
        expupdqtytxtbx.Text = selectedRow.Cells(2).Value?.ToString()
        expupdunitofmeasurecmbbx.Text = selectedRow.Cells(3).Value?.ToString()
        expupdtranstypecmbbx.Text = selectedRow.Cells(7).Value?.ToString()
        expupdamounttxtbx.Text = selectedRow.Cells(4).Value?.ToString()
        expupdreceiptnumtxtbx.Text = selectedRow.Cells(8).Value?.ToString()
        expupdrefnumbertxtbx.Text = selectedRow.Cells(9).Value?.ToString()
        expupdnotestxtbx.Text = selectedRow.Cells(13).Value?.ToString()


        ' Adjust enabled fields based on payment method
        exptranstypecmbbx_SelectedIndexChanged(Nothing, Nothing)

        ExpensesUpdatePanel.Visible = True
    End Sub
    Private Sub expupdatedbbtn_Click(sender As Object, e As EventArgs) Handles expupdatedbbtn.Click

        Dim selectedRow As DataGridViewRow = expdgv.SelectedRows(0)
        Dim itemName As String = expupditemnametxtbx.Text.Trim()
        Dim unitOfMeasure As String = expupdunitofmeasurecmbbx.Text
        Dim paymentMethod As String = expupdtranstypecmbbx.Text
        Dim receiptNumber As String = expupdreceiptnumtxtbx.Text.Trim()
        Dim referenceNumber As String = expupdrefnumbertxtbx.Text.Trim()
        Dim notes As String = expupdnotestxtbx.Text.Trim()
        Dim qty As Decimal
        Dim amount As Decimal
        Dim total As Decimal = 0D
        Dim itemID As Object = DBNull.Value

        If Not Decimal.TryParse(expupdqtytxtbx.Text, qty) Then
            MessageBox.Show("Please enter a valid quantity.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If Not Decimal.TryParse(expupdamounttxtbx.Text, amount) Then
            MessageBox.Show("Please enter a valid amount.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        total = qty * amount

        ' Debugging Output
        Console.WriteLine("Updating Expense: ItemName={0}, Qty={1}, Amount={2}, Total={3}", itemName, qty, amount, total)

        ' Check if item is in inventory
        Dim invQuery As String = "SELECT ItemID FROM inventory WHERE ItemName LIKE @itemName LIMIT 1"
        Using conn As New MySqlConnection(strConnection)
            conn.Open()

            Using cmdInv As New MySqlCommand(invQuery, conn)
                cmdInv.Parameters.AddWithValue("@itemName", "%" & itemName & "%")
                Dim reader = cmdInv.ExecuteReader()
                If reader.Read() Then
                    itemID = reader("ItemID")
                End If
                reader.Close()
            End Using

            ' Update the record
            Dim updateQuery As String = "
    UPDATE expensesoverview SET 
        ItemID = @ItemID,
        ItemName = @ItemName,
        Qty = @Qty,
        UnitOfMeasure = @UnitOfMeasure,
        Amount = @Amount,
        Description = @Description,
        PaymentMethod = @PaymentMethod,
        ReceiptNumber = @ReceiptNumber,
        PaymentReferenceNumber = @PaymentReferenceNumber,
        Total = @Total,
        Notes = @Notes
    WHERE ExID = @ExID"

            Using cmdUpdate As New MySqlCommand(updateQuery, conn)
                cmdUpdate.Parameters.AddWithValue("@ItemID", itemID)
                cmdUpdate.Parameters.AddWithValue("@ItemName", itemName)
                cmdUpdate.Parameters.AddWithValue("@Qty", qty)
                cmdUpdate.Parameters.AddWithValue("@UnitOfMeasure", unitOfMeasure)
                cmdUpdate.Parameters.AddWithValue("@Amount", amount)
                cmdUpdate.Parameters.AddWithValue("@Description", itemName) ' Verify this if necessary
                cmdUpdate.Parameters.AddWithValue("@PaymentMethod", paymentMethod)
                cmdUpdate.Parameters.AddWithValue("@ReceiptNumber", If(paymentMethod = "Cash", receiptNumber, ""))
                cmdUpdate.Parameters.AddWithValue("@PaymentReferenceNumber", If(paymentMethod = "Gcash", referenceNumber, ""))
                cmdUpdate.Parameters.AddWithValue("@Total", total)
                cmdUpdate.Parameters.AddWithValue("@Notes", notes)
                cmdUpdate.Parameters.AddWithValue("@ExID", selectedRow.Cells(0).Value) ' Make sure ExID is correctly passed

                Try
                    cmdUpdate.ExecuteNonQuery()
                    MessageBox.Show("Expense updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    ExpensesUpdatePanel.Visible = False
                    LoadExpenses() ' Reload the DataGridView
                Catch ex As Exception
                    MessageBox.Show("Error updating expense: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End Using
        End Using
    End Sub

    Private Sub expupcancelbtn_Click(sender As Object, e As EventArgs) Handles expupcancelbtn.Click
        ExpensesUpdatePanel.Visible = False
    End Sub
    Private Sub expupdtranstypecmbbx_SelectedIndexChanged(sender As Object, e As EventArgs) Handles expupdtranstypecmbbx.SelectedIndexChanged
        Select Case expupdtranstypecmbbx.Text.Trim().ToLower()
            Case "gcash"
                expupdrefnumbertxtbx.Enabled = True
                expupdreceiptnumtxtbx.Enabled = False
            Case "cash"
                expupdrefnumbertxtbx.Enabled = False
                expupdreceiptnumtxtbx.Enabled = True
            Case Else
                expupdrefnumbertxtbx.Enabled = False
                expupdreceiptnumtxtbx.Enabled = False
        End Select
    End Sub

    Private Sub PopulateExpenseUpdateComboBoxes()
        ' Populate Unit of Measure ComboBox
        Dim units() As String = {"kg", "g", "Liters", "ml", "gallon", "pcs"}
        expupdunitofmeasurecmbbx.Items.Clear()
        expupdunitofmeasurecmbbx.Items.AddRange(units)

        ' Populate Transaction Type ComboBox
        Dim paymentTypes() As String = {"Cash", "GCash"}
        expupdtranstypecmbbx.Items.Clear()
        expupdtranstypecmbbx.Items.AddRange(paymentTypes)
    End Sub
    Private Sub expdeletebtn_Click(sender As Object, e As EventArgs) Handles expdeletebtn.Click
        If expdgv.SelectedRows.Count = 0 Then
            MessageBox.Show("Please select an expense to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' Confirm before deleting
        Dim result As DialogResult = MessageBox.Show("Are you sure you want to delete the selected expense?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If result = DialogResult.No Then
            Return
        End If

        ' Get the ExID of the selected row
        Dim selectedRow As DataGridViewRow = expdgv.SelectedRows(0)
        Dim exID As Integer = Convert.ToInt32(selectedRow.Cells(0).Value)

        ' Delete from the database
        Dim deleteQuery As String = "DELETE FROM expensesoverview WHERE ExID = @ExID"

        Using conn As New MySqlConnection(strConnection)
            conn.Open()

            Using cmdDelete As New MySqlCommand(deleteQuery, conn)
                cmdDelete.Parameters.AddWithValue("@ExID", exID)

                Try
                    cmdDelete.ExecuteNonQuery()
                    MessageBox.Show("Expense deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

                    ' Remove the row from the DataGridView
                    expdgv.Rows.RemoveAt(selectedRow.Index)

                    ' Optionally, reload the DataGridView to reflect changes
                    ' LoadExpenses() 

                Catch ex As Exception
                    MessageBox.Show("Error deleting expense: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End Using
        End Using
    End Sub
    'add user

    Private Sub settingsregisterbtn_Click(sender As Object, e As EventArgs) Handles settingsregisterbtn.Click
        ' Validate required fields
        If String.IsNullOrWhiteSpace(settingsfirstnametxtbx.Text) OrElse
       String.IsNullOrWhiteSpace(settingslastnametxtbx.Text) OrElse
       String.IsNullOrWhiteSpace(settingsregisterusernametxtbx.Text) OrElse
       String.IsNullOrWhiteSpace(settingspasswordtxtbx.Text) OrElse
       settingsrolecmbbx.SelectedItem Is Nothing Then

            MessageBox.Show("Please fill out all fields!", "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        ' Collect input
        Dim firstName As String = settingsfirstnametxtbx.Text.Trim()
        Dim lastName As String = settingslastnametxtbx.Text.Trim()
        Dim username As String = settingsregisterusernametxtbx.Text.Trim()
        Dim password As String = EncryptPassword(settingspasswordtxtbx.Text.Trim())  ' Encrypt the password
        Dim role As String = settingsrolecmbbx.SelectedItem.ToString()
        Dim dateCreated As String = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")

        Dim checkQuery As String = "SELECT COUNT(*) FROM users WHERE Username = @Username"
        Dim insertQuery As String = "INSERT INTO users (FirstName, LastName, Username, Password, Role, DateCreated) " &
                                "VALUES (@FirstName, @LastName, @Username, @Password, @Role, @DateCreated)"

        Try
            openConn(db_name)

            If conn IsNot Nothing AndAlso conn.State = ConnectionState.Open Then
                ' Check for existing username
                Using checkCmd As New MySqlCommand(checkQuery, conn)
                    checkCmd.Parameters.AddWithValue("@Username", username)
                    Dim userExists As Integer = Convert.ToInt32(checkCmd.ExecuteScalar())
                    If userExists > 0 Then
                        MessageBox.Show("Username already exists. Please choose another.", "Duplicate Username", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Exit Sub
                    End If
                End Using

                ' Insert new user
                Using cmd As New MySqlCommand(insertQuery, conn)
                    cmd.Parameters.AddWithValue("@FirstName", firstName)
                    cmd.Parameters.AddWithValue("@LastName", lastName)
                    cmd.Parameters.AddWithValue("@Username", username)
                    cmd.Parameters.AddWithValue("@Password", password)  ' encrypted password
                    cmd.Parameters.AddWithValue("@Role", role)
                    cmd.Parameters.AddWithValue("@DateCreated", dateCreated)

                    Dim result As Integer = cmd.ExecuteNonQuery()
                    If result > 0 Then
                        MessageBox.Show("Registration successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

                        ' Optionally reset fields or switch panels here
                        settingsfirstnametxtbx.Clear()
                        settingslastnametxtbx.Clear()
                        settingsregisterusernametxtbx.Clear()
                        settingspasswordtxtbx.Clear()
                        settingsrolecmbbx.SelectedIndex = -1
                    Else
                        MessageBox.Show("Registration failed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End If
                End Using
            Else
                MessageBox.Show("Database connection failed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If conn IsNot Nothing AndAlso conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
    End Sub
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

    '3rd user level

    Private Sub LoadUsers()
        Try
            openConn(db_name)
            If conn.State = ConnectionState.Open Then
                Dim query As String = "SELECT UserID, FirstName, LastName, Role FROM users"
                Dim adapter As New MySqlDataAdapter(query, conn)
                Dim table As New DataTable()
                adapter.Fill(table)

                userdgv.DataSource = table

                ' Optional: Hide the UserID column
                userdgv.Columns("UserID").Visible = False
            Else
                MessageBox.Show("Database connection failed.")
            End If
        Catch ex As Exception
            MessageBox.Show("Error loading users: " & ex.Message)
        Finally
            If conn IsNot Nothing AndAlso conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
    End Sub
    Private Sub deleteuserbtn_Click(sender As Object, e As EventArgs) Handles deleteuserbtn.Click
        If userdgv.SelectedRows.Count = 0 Then
            MessageBox.Show("Please select a user to delete.")
            Return
        End If

        ' Confirm delete
        If MessageBox.Show("Are you sure you want to delete this user?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) = DialogResult.No Then
            Return
        End If

        ' Get UserID from selected row
        Dim selectedRow As DataGridViewRow = userdgv.SelectedRows(0)
        Dim userId As Integer = Convert.ToInt32(selectedRow.Cells("UserID").Value)

        Try
            openConn(db_name)
            If conn.State = ConnectionState.Open Then
                Dim query As String = "DELETE FROM users WHERE UserID = @UserID"
                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@UserID", userId)

                    Dim result As Integer = cmd.ExecuteNonQuery()
                    If result > 0 Then
                        MessageBox.Show("User deleted successfully.")
                        LoadUsers() ' Refresh the DataGridView
                    Else
                        MessageBox.Show("Failed to delete user.")
                    End If
                End Using
            End If
        Catch ex As Exception
            MessageBox.Show("Error deleting user: " & ex.Message)
        Finally
            If conn IsNot Nothing AndAlso conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
    End Sub



End Class
