Imports System.IO
Imports System.Threading

Public Class MainForm
    Public strFileName As String
    Public Run As Integer
    Public a As Integer
    Public RefreshRate As Integer = 60
    Public Speicherort As String
    Dim thread As System.Threading.Thread
    Dim thread2 As System.Threading.Thread
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles Me.Load
        CheckForIllegalCrossThreadCalls = False
        If My.Settings.SpeicherDatei = Nothing Then
            MessageBox.Show("Kein Speicherort definiert!, Bitte Speicherort auswählen!!")
        Else
            strFileName = My.Settings.SpeicherDatei
            Label2.Text = My.Settings.SpeicherDatei
            thread2 = New System.Threading.Thread(AddressOf Autostartup)
            thread2.Start()
        End If
        If My.Settings.Autostartup = 1 Then
            CheckBox1.Checked = True
        Else
            CheckBox1.Checked = False
        End If

    End Sub
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        'Start Button 
        If thread2.IsAlive Then
            Me.Button5.PerformClick()
        End If
        If Not IsNothing(strFileName) Then
            Run = 1
            thread = New System.Threading.Thread(AddressOf MainFunction)
            thread.Start()
        Else
            MessageBox.Show("Speicheort ist leer!!!, Bitte einen Ort auswählen.")
        End If

    End Sub
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Run = 0
        Panel1.BackColor = Color.Red
        Label4.Text = "Gestoppt"
        If Not IsNothing(thread) Then
            thread.Abort()
        End If

    End Sub
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim fd As SaveFileDialog = New SaveFileDialog()
        'Dim strFileName As String
        fd.Title = "Bild Speichern"
        fd.InitialDirectory = My.Computer.FileSystem.SpecialDirectories.Desktop
        fd.Filter = "PNG Image|*.png"
        fd.FilterIndex = 1
        fd.RestoreDirectory = True

        If fd.ShowDialog() = DialogResult.OK Then
            strFileName = fd.FileName
            Label2.Text = fd.FileName
            My.Settings.SpeicherDatei = fd.FileName
            My.Settings.Save()
            MessageBox.Show(fd.FileName)
        End If

    End Sub
    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Call TakeScreenshotManual()
    End Sub
    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        On Error Resume Next
        thread2.Abort()
        Label5.Text = ""
        Button5.Hide()
    End Sub
    Private Sub NumericUpDown1_ValueChanged(sender As Object, e As EventArgs) Handles NumericUpDown1.ValueChanged
        If Run = 1 Then
            MessageBox.Show("Bitte zuerst Aufzeichnung stoppen!")
        End If
        RefreshRate = NumericUpDown1.Value
    End Sub
    Private Sub MainFunction()
        If Run = 0 Then
            Panel1.BackColor = Color.Red
            thread.Abort()
        End If
        While (Run = 1)
            Panel1.BackColor = Color.Green
            Label4.Text = "Läuft"
            TakeScreenshot()
            Threading.Thread.Sleep(RefreshRate * 1000)
        End While
    End Sub
    Private Sub Autostartup()
        ' sekunden timer 
        Dim k As Integer
        For k = 1 To 15
            Label5.Text = "Auto Aufzeichnung nach " & 15 - k & " Sek."
            Threading.Thread.Sleep(1000)
        Next
        Me.Label5.Text = ""
        Me.Button5.Hide()
        Me.Button1.PerformClick()
    End Sub
    Private Sub TakeScreenshotManual()
        On Error Resume Next

        Dim mystring As String = System.IO.Path.GetFileName(strFileName) & " " & System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
        Dim myFont As Font = New Font("Arial", 50, FontStyle.Bold, GraphicsUnit.Pixel)
        Dim screenSize As Size = New Size(My.Computer.Screen.Bounds.Width, My.Computer.Screen.Bounds.Height)
        Using screenGrab As New Bitmap(My.Computer.Screen.Bounds.Width, My.Computer.Screen.Bounds.Height)
            'copy the screen to the image
            Using g = Graphics.FromImage(screenGrab)
                g.CopyFromScreen(New Point(0, 0), New Point(0, 0), screenSize)
                g.DrawString(mystring, myFont, Brushes.Red, 0, 0)
            End Using
            'save the image
            If Not strFileName = "" Then
                Dim tempdatei As String
                tempdatei = InputBox("Bitte eine Name für das Bild eingeben!, -OHNE Leerzeichen-", "Fehler quittung")
                If Not IsNothing(tempdatei) Then
                    screenGrab.Save(System.IO.Path.GetDirectoryName(strFileName) & "\" & tempdatei & ".png", System.Drawing.Imaging.ImageFormat.Png)
                Else
                    MessageBox.Show("Vorgang Abgebrochen, da keine Name eingegeben!")
                End If

            End If
        End Using
    End Sub
    Private Sub TakeScreenshot()
        On Error Resume Next
        Dim mystring As String = System.IO.Path.GetFileName(strFileName) & " " & System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
        Dim myFont As Font = New Font("Arial", 50, FontStyle.Bold, GraphicsUnit.Pixel)
        Dim screenSize As Size = New Size(My.Computer.Screen.Bounds.Width, My.Computer.Screen.Bounds.Height)
        Using screenGrab As New Bitmap(My.Computer.Screen.Bounds.Width, My.Computer.Screen.Bounds.Height)
            'copy the screen to the image
            Using g = Graphics.FromImage(screenGrab)
                g.CopyFromScreen(New Point(0, 0), New Point(0, 0), screenSize)
                g.DrawString(mystring, myFont, Brushes.Red, 0, 0)
            End Using
            'save the image
            If Not strFileName = "" Then
                screenGrab.Save(strFileName, System.Drawing.Imaging.ImageFormat.Png)
                '####### ADD Date to picture: 
            Else
                MessageBox.Show("Speicherort ist leer!")
                thread.Abort()
            End If
        End Using
    End Sub



    Private Function CreateShortCut(ByVal TargetName As String, ByVal ShortCutPath As String, ByVal ShortCutName As String) As Boolean
        Dim oShell As Object
        Dim oLink As Object
        'you don’t need to import anything in the project reference to create the Shell Object
        Try
            oShell = CreateObject("WScript.Shell")
            oLink = oShell.CreateShortcut(ShortCutPath & "\" & ShortCutName & ".lnk")

            oLink.TargetPath = TargetName
            oLink.WindowStyle = 1
            oLink.Save()
        Catch ex As Exception
        End Try
        Return True
    End Function
    Private Function RemoveShortCut() As Boolean
        Try
            My.Computer.FileSystem.DeleteFile(Environment.GetFolderPath(Environment.SpecialFolder.Startup) + "\RemotetrackingClient.lnk")
        Catch ex As Exception
        End Try
        Return True
    End Function

    Private Sub NotifyIcon1_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles NotifyIcon1.MouseDoubleClick
        Me.Show()
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.WindowState = FormWindowState.Normal
        NotifyIcon1.Visible = False
    End Sub

    Private Sub Form1_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize
        If Me.WindowState = FormWindowState.Minimized Then
            Me.Hide()
            NotifyIcon1.Visible = True
        End If

    End Sub

    Private Sub Form1_Closed(sender As Object, e As EventArgs) Handles Me.Closed
        Button2.PerformClick()
        MessageBox.Show("Aufzeichnung gestoppt!")
    End Sub

    Private Sub CheckBox1_MouseDown(sender As Object, e As MouseEventArgs) Handles CheckBox1.MouseDown

    End Sub

    Private Sub CheckBox1_MouseClick(sender As Object, e As MouseEventArgs) Handles CheckBox1.MouseClick
        If CheckBox1.Checked = True Then
            a = CreateShortCut(Environment.GetFolderPath(Environment.SpecialFolder.Programs) & "\Mohamed Alaeddin Abdellaoui\RemoteTrackingClient.appref-ms", Environment.GetFolderPath(Environment.SpecialFolder.Startup), "RemoteTrackingClient")
            MessageBox.Show("Programm startet bei der Anmeldung automatisch")
            My.Settings.Autostartup = 1
            My.Settings.Save()

        Else
            a = RemoveShortCut()
            My.Settings.Autostartup = 0
            My.Settings.Save()
            MessageBox.Show("Programm startet nicht mehr automatisch!")
        End If
    End Sub
End Class
