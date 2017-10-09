Imports System.Data.SqlClient
Imports System.Net
Imports System.IO
Imports System.ComponentModel

Structure ParamsTable
    Dim id As Integer
    Dim id_user As Integer
    Dim event_type As Integer
    Dim viewed As Integer
    Dim last_date As Date
    Dim strict_date As Integer
End Structure

Structure IdParams
    Dim id As Integer
    Dim headerText As fontParams
    Dim aText As fontParams
    Dim notes As fontParams
    Dim path_to_scrn As String
    Dim reference As String
End Structure

Structure fontParams
    Dim text As String
    Dim fontSize As Integer
    Dim fontName As String
    Dim bold As Integer
    Dim color As String
    Dim textType As Integer
End Structure

Public Class Form1

    Dim paramsForIds As List(Of IdParams)

    Public Sub AddNotification(ParamArray ByVal userIds As Integer())

        ' тут мы должны определять ид пользователя, и грузить только для него

        Dim EventTable As List(Of ParamsTable) = GetListOfEvents(userIds)
        If EventTable Is Nothing Then
            Exit Sub
        End If

        Dim ids As New List(Of Integer)
        For i As Integer = 0 To EventTable.Count - 1
            ids.Add(EventTable(i).id)
        Next

        paramsForIds = GetParamsForIds(ids)

        For i As Integer = 0 To EventTable.Count - 1
            If EventTable(i).event_type = 2 Then
                MessageBox.Show(paramsForIds(i).aText.text)
            End If
        Next

        Dim popup As New Form
        popup.StartPosition = FormStartPosition.CenterScreen
        popup.ControlBox = False
        popup.FormBorderStyle = FormBorderStyle.Sizable
        popup.Size = New Size(700, 650)
        popup.BackColor = Color.GhostWhite

        Dim ltblp As New TableLayoutPanel
        ltblp.Dock = DockStyle.Fill
        ltblp.ColumnCount = 10
        ltblp.RowCount = 20
        With ltblp
            With .ColumnStyles
                .Add(New ColumnStyle(SizeType.Percent, 10.0!))
                .Add(New ColumnStyle(SizeType.Percent, 10.0!))
                .Add(New ColumnStyle(SizeType.Percent, 10.0!))
                .Add(New ColumnStyle(SizeType.Percent, 10.0!))
                .Add(New ColumnStyle(SizeType.Percent, 10.0!))
                .Add(New ColumnStyle(SizeType.Percent, 10.0!))
                .Add(New ColumnStyle(SizeType.Percent, 10.0!))
                .Add(New ColumnStyle(SizeType.Percent, 10.0!))
                .Add(New ColumnStyle(SizeType.Percent, 10.0!))
                .Add(New ColumnStyle(SizeType.Percent, 10.0!))
            End With
            With .RowStyles
                .Add(New RowStyle(SizeType.Percent, 5.0!))
                .Add(New RowStyle(SizeType.Percent, 5.0!))
                .Add(New RowStyle(SizeType.Percent, 5.0!))
                .Add(New RowStyle(SizeType.Percent, 5.0!))
                .Add(New RowStyle(SizeType.Percent, 5.0!))
                .Add(New RowStyle(SizeType.Percent, 5.0!))
                .Add(New RowStyle(SizeType.Percent, 5.0!))
                .Add(New RowStyle(SizeType.Percent, 5.0!))
                .Add(New RowStyle(SizeType.Percent, 5.0!))
                .Add(New RowStyle(SizeType.Percent, 5.0!))
                .Add(New RowStyle(SizeType.Percent, 5.0!))
                .Add(New RowStyle(SizeType.Percent, 5.0!))
                .Add(New RowStyle(SizeType.Percent, 5.0!))
                .Add(New RowStyle(SizeType.Percent, 5.0!))
                .Add(New RowStyle(SizeType.Percent, 5.0!))
                .Add(New RowStyle(SizeType.Percent, 5.0!))
                .Add(New RowStyle(SizeType.Percent, 5.0!))
                .Add(New RowStyle(SizeType.Percent, 5.0!))
                .Add(New RowStyle(SizeType.Percent, 5.0!))
                .Add(New RowStyle(SizeType.Percent, 5.0!))
            End With
        End With

        popup.Controls.Add(ltblp)
        Call ConfigureForm(ltblp, EventTable, paramsForIds)
        Call CreateButtons(popup, ltblp)

        If EventTable.Count > 0 Then

            popup.ShowDialog()

            If popup.DialogResult = DialogResult.OK Then
                popup.Close()
            End If
        End If


    End Sub

    Private Sub ConfigureForm(popup As TableLayoutPanel, eventTable As List(Of ParamsTable), paramsForIds As List(Of IdParams))

        popup.BackColor = Color.GhostWhite
        Dim checkDate As Date
        Dim gb As New GroupBox
        gb.Dock = DockStyle.Fill
        gb.Height = 25
        gb.Name = "myGroupBox"
        Dim locationX As Integer = 5
        For i As Integer = 0 To eventTable.Count - 1
            checkDate = eventTable(i).last_date
            '    ' strict_date:
            '    ' 0 - строго в определенный день
            '    ' 1 - до определенного дня
            If ((eventTable(i).strict_date = 0 AndAlso Today.Date = checkDate) OrElse (eventTable(i).strict_date = 1 AndAlso Today.Date <= checkDate)) AndAlso eventTable(i).event_type = 1 Then
                Dim rbtn As New RadioButton
                rbtn.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
                rbtn.Text = ""
                rbtn.Name = i.ToString
                rbtn.Width = 15
                rbtn.Location = New Point(locationX, 0)
                locationX += 30
                If locationX = 35 Then ' первую кнопку ставим в чекнутое состояние
                    rbtn.Checked = True
                    Call ShowNotification(popup, paramsForIds(i))
                End If
                gb.Controls.Add(rbtn)
                AddHandler rbtn.Click, AddressOf RB_Clicked
            End If

        Next
        popup.Controls.Add(gb, 1, 20)
        popup.SetColumnSpan(gb, 10)
    End Sub

    Private Sub ShowNotification(popup As TableLayoutPanel, params As IdParams)

        Try
            popup.Controls.RemoveByKey("hText")
            popup.Controls.RemoveByKey("aText")
            popup.Controls.RemoveByKey("image")
            popup.Controls.RemoveByKey("notes")
            popup.Controls.RemoveByKey("reference")
        Catch ex As Exception

        End Try

        Dim curRow As Integer = 1
        ' Header Text
        If params.headerText.text <> "" Then
            Dim hText As New Label
            hText.Name = "hText"
            hText.Anchor = AnchorStyles.Top Or AnchorStyles.Left
            hText.AutoSize = True
            hText.Text = params.headerText.text
            If params.headerText.bold = 1 Then
                hText.Font = New Font(params.headerText.fontName, params.headerText.fontSize, FontStyle.Bold)
            Else
                hText.Font = New Font(params.headerText.fontName, params.headerText.fontSize, FontStyle.Regular)
            End If
            Try
                hText.ForeColor = Color.FromName(params.headerText.color)
            Catch ex As Exception
                hText.ForeColor = ColorTranslator.FromHtml("#" + params.headerText.color)
            End Try
            popup.Controls.Add(hText)
            popup.SetCellPosition(hText, New TableLayoutPanelCellPosition(1, curRow))
            popup.SetRowSpan(hText, 2)
            popup.SetColumnSpan(hText, 8)
            curRow += 2
        End If

        ' aText
        If params.aText.text <> "" Then
            Dim aText As New Label
            aText.Name = "aText"
            aText.Anchor = AnchorStyles.Left Or AnchorStyles.Top
            aText.AutoSize = True
            aText.Text = params.aText.text
            If params.aText.bold = 1 Then
                aText.Font = New Font(params.aText.fontName, params.aText.fontSize, FontStyle.Bold)
            Else
                aText.Font = New Font(params.aText.fontName, params.aText.fontSize, FontStyle.Regular)
            End If
            Try
                aText.ForeColor = Color.FromName(params.aText.color)
            Catch ex As Exception
                aText.ForeColor = ColorTranslator.FromHtml("#" + params.aText.color)
            End Try
            popup.Controls.Add(aText)
            popup.SetCellPosition(aText, New TableLayoutPanelCellPosition(1, curRow))
            popup.SetRowSpan(aText, 2)
            popup.SetColumnSpan(aText, 8)
            curRow += 2
        End If

        ' image
        Dim pb As New PictureBox
        pb.Name = "image"
        pb.SizeMode = PictureBoxSizeMode.StretchImage
        pb.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
        Try
            Dim req As FtpWebRequest = CType(WebRequest.Create(params.path_to_scrn), FtpWebRequest)
            req.Method = WebRequestMethods.Ftp.DownloadFile
            req.Credentials = New NetworkCredential("Guide", "DjdbyANGCegth777")
            Dim response As FtpWebResponse = req.GetResponse()
            Dim responseStream As Stream = response.GetResponseStream()
            Dim img = Image.FromStream(responseStream)
            pb.Image = img
            pb.Dock = DockStyle.Fill
            popup.Controls.Add(pb)
            popup.SetCellPosition(pb, New TableLayoutPanelCellPosition(1, curRow))
            popup.SetRowSpan(pb, 12)
            popup.SetColumnSpan(pb, 8)
            AddHandler pb.Click, AddressOf image_Clicked
            curRow += 12
        Catch ex As Exception
        End Try

        ' notes
        If params.notes.text <> "" Then
            Dim notes As New Label
            notes.Name = "notes"
            notes.Anchor = AnchorStyles.Left Or AnchorStyles.Top
            notes.AutoSize = True
            notes.Text = params.notes.text
            If params.notes.bold = 1 Then
                notes.Font = New Font(params.notes.fontName, params.notes.fontSize, FontStyle.Bold)
            Else
                notes.Font = New Font(params.notes.fontName, params.notes.fontSize, FontStyle.Regular)
            End If
            Try
                notes.ForeColor = Color.FromName(params.notes.color)
            Catch ex As Exception
                notes.ForeColor = ColorTranslator.FromHtml("#" + params.notes.color)
            End Try
            popup.Controls.Add(notes)
            popup.SetCellPosition(notes, New TableLayoutPanelCellPosition(1, curRow))
            popup.SetRowSpan(notes, 2)
            popup.SetColumnSpan(notes, 8)
            curRow += 2
        End If

        ' reference
        If params.reference <> "0" Then
            Dim btn As New Button
            btn.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
            btn.Width = 160
            btn.Name = "reference"
            btn.Text = "Еще новости по ссылке"
            AddHandler btn.Click, Sub(sender, e) OpenBrowser(params.reference)
            popup.Controls.Add(btn)
            popup.SetCellPosition(btn, New TableLayoutPanelCellPosition(1, curRow))
            popup.SetRowSpan(btn, 1)
            popup.SetColumnSpan(btn, 8)
        End If

        Call setAsViewed(params.id)

    End Sub

    Private Sub OpenBrowser(reference As String)
        Dim frm As New Form
        frm.WindowState = FormWindowState.Maximized
        Dim wb As New WebBrowser
        wb.Dock = DockStyle.Fill
        frm.Controls.Add(wb)
        Try
            Dim uri = New UriBuilder(New Uri(reference))
            If reference.Substring(0, 3) = "ftp" Then
                uri.UserName = "Guide"
                uri.Password = "DjdbyANGCegth777"
            End If
            wb.Navigate(uri.Uri)
            frm.Show()
        Catch ex As Exception
            MessageBox.Show("Не удалось открыть ссылку.")
        End Try
    End Sub

    Private Sub image_Clicked(sender As Object, e As EventArgs)
        Dim pb As PictureBox = DirectCast(sender, PictureBox)

        Dim pb2 As New PictureBox
        pb2.SizeMode = PictureBoxSizeMode.StretchImage
        pb2.Image = pb.Image

        Dim frm As New Form()
        frm.Size = New Size(480, 360)
        pb2.Size = frm.Size
        frm.Controls.Add(pb2)
        AddHandler frm.Resize, AddressOf Form_Resize
        frm.Show()
    End Sub

    Private Sub Form_Resize(sender As Object, e As EventArgs)
        Dim frm As Form = DirectCast(sender, Form)
        frm.Controls(0).Size = frm.Size
    End Sub

    Private Sub RB_Clicked(sender As Object, e As EventArgs)
        Dim rb As RadioButton = DirectCast(sender, RadioButton)
        Call ShowNotification(DirectCast(rb.Parent.Parent, TableLayoutPanel), paramsForIds(Int32.Parse(rb.Name)))
    End Sub

    Private Function GetListOfEvents(ParamArray ByVal id_user As Integer()) As List(Of ParamsTable)

        Dim query As String = "SELECT [id], [id_user], [event_type], [viewed], [last_date], [strict_date] FROM [EventTable] where [viewed] = 0 AND ( "

        For i As Integer = 0 To id_user.Count - 1
            query += " [id_user]=" + id_user(i).ToString + " OR"
        Next
        query = Mid(query, 1, Len(query) - 2) + " )"

        Dim cn As New SqlConnection(DefineConStrtoSRV(4))
        Dim adap As New SqlDataAdapter(query, cn)
        Dim dt As New DataTable
        adap.Fill(dt)
        cn.Dispose()
        adap.Dispose()

        Dim ptbls As New List(Of ParamsTable)
        For i As Integer = 0 To dt.Rows.Count - 1
            Dim itm As New ParamsTable
            itm.id = dt(i)(0)
            itm.id_user = dt(i)(1)
            itm.event_type = dt(i)(2)
            itm.viewed = dt(i)(3)
            itm.last_date = dt(i)(4)
            itm.strict_date = dt(i)(5)
            ptbls.Add(itm)
        Next

        Return ptbls
    End Function

    Private Function GetParamsForIds(ids As List(Of Integer)) As List(Of IdParams)

        If ids.Count = 0 Then
            Return New List(Of IdParams)
        End If

        Dim query As String = "Select [id] ,[headerText] ,[aText] ,[notes] ,[path_to_scrn] ,[reference]
  From [CBR_Emm_test].[dbo].[eventTableExplanations]
  Where ("
        For i As Integer = 0 To ids.Count - 1
            query += " id = " + ids(i).ToString + " OR"
        Next
        query = Mid(query, 1, Len(query) - 2) + ")"

        Dim cn As New SqlConnection(DefineConStrtoSRV(4))
        Dim adap As New SqlDataAdapter(query, cn)
        Dim dt As New DataTable
        Dim dt2 As New DataTable
        adap.Fill(dt)

        query = "SELECT A.id, [fontSize] ,[fontName] ,[bold] ,[color] ,[textType] FROM (
SELECT id from [CBR_Emm_test].[dbo].[eventTableExplanations]) as A
inner join
(SELECT [id] ,[fontSize] ,[fontName] ,[bold] ,[color] ,[textType]
  FROM [CBR_Emm_test].[dbo].[eventTableExplanations2]) as B ON A.id=B.id order by A.id, textType"

        adap = New SqlDataAdapter(query, cn)
        adap.Fill(dt2)

        cn.Dispose()
        adap.Dispose()

        Call SetDefaultValuesForDt(dt)

        Dim ptbls As New List(Of IdParams)
        For i As Integer = 0 To dt.Rows.Count - 1
            Dim itm As New IdParams
            itm.id = dt(i)(0)
            itm.headerText.text = dt(i)(1)
            itm.aText.text = dt(i)(2)
            itm.notes.text = dt(i)(3)
            itm.path_to_scrn = dt(i)(4)
            itm.reference = dt(i)(5)
            ptbls.Add(itm)
        Next

        For i As Integer = 0 To ptbls.Count - 1

            Dim idPrms As IdParams = ptbls(i)
            Dim id As Integer = ptbls(i).id
            Dim dv As New DataView(dt2, "id=" + id.ToString, Nothing, DataViewRowState.CurrentRows)

            If dv.Count = 0 Then
                For j As Integer = 0 To 2
                    Dim prms As New fontParams
                    Call SetDefaultValuesForPrms(prms, j)
                    Select Case prms.textType
                        Case 0 ' headerText
                            prms.text = ptbls(i).headerText.text
                            idPrms.headerText = prms
                        Case 1 ' aText
                            prms.text = ptbls(i).aText.text
                            idPrms.aText = prms
                        Case 2 ' notes
                            prms.text = ptbls(i).notes.text
                            idPrms.notes = prms
                    End Select
                Next
            End If

            Dim textType As Integer = 0

            For j As Integer = 0 To dv.Count - 1

                Dim prms As New fontParams

                If textType <> dv(j)(5) Then
                    Call SetDefaultValuesForPrms(prms, textType)
                    Select Case prms.textType
                        Case 0 ' headerText
                            prms.text = ptbls(i).headerText.text
                            idPrms.headerText = prms
                        Case 1 ' aText
                            prms.text = ptbls(i).aText.text
                            idPrms.aText = prms
                        Case 2 ' notes
                            prms.text = ptbls(i).notes.text
                            idPrms.notes = prms
                    End Select
                    textType += 1
                    If textType < 3 Then
                        j -= 1
                    End If
                    Continue For
                End If
                prms.fontSize = dv(j)(1)
                prms.fontName = dv(j)(2)
                prms.bold = dv(j)(3)
                prms.color = dv(j)(4)
                prms.textType = dv(j)(5)

                Select Case prms.textType
                    Case 0 ' headerText
                        prms.text = ptbls(i).headerText.text
                        idPrms.headerText = prms
                    Case 1 ' aText
                        prms.text = ptbls(i).aText.text
                        idPrms.aText = prms
                    Case 2 ' notes
                        prms.text = ptbls(i).notes.text
                        idPrms.notes = prms
                End Select
                textType += 1

                If idPrms.notes.textType = 0 AndAlso j = dv.Count - 1 AndAlso textType = 2 Then
                    j -= 1
                End If

            Next
            ptbls(i) = idPrms
            textType = 0
        Next

        Return ptbls

    End Function

    Function DefineConStrtoSRV(NumOfBonds As Integer, Optional sa As Boolean = False) As String
        DefineConStrtoSRV = ""
        Dim NameBD As String = ""
        Select Case NumOfBonds
            Case 4 : NameBD = "CBR_Emm_Test"
        End Select
        If NameBD <> "" Then
            If sa = True Then
            Else
                DefineConStrtoSRV = "Data Source=212.42.46.12;Initial Catalog=CBR_Emm_Test;Persist Security Info=True;User ID=EmmWarrior;Password=1qwerTY"            'User ID=balans;Password=ygcXTB4hAuSp5c" '"Data Source=212.42.46.12;Initial Catalog=" & NameBD & ";Persist Security Info=True;User ID=EmmWarrior;Password=1qwerTY"
            End If
        End If
    End Function

    Private Sub setAsViewed(id As Integer)

        Dim cn As New SqlConnection(DefineConStrtoSRV(4))
        Dim query = cn.CreateCommand()
        query.CommandText = "UPDATE [dbo].[EventTable] SET [viewed]=1 WHERE id=@id"
        query.Parameters.AddWithValue("@id", id)
        cn.Open()
        query.ExecuteNonQuery()
        cn.Close()
        query.Dispose()
        cn.Dispose()

    End Sub

    Private Sub SetDefaultValuesForDt(dt As DataTable)
        For i As Integer = 0 To dt.Rows.Count - 1
            For j As Integer = 0 To dt.Columns.Count - 1
                If IsDBNull(dt(i)(j)) Then
                    dt(i)(j) = 0
                End If
            Next
        Next
    End Sub

    Private Sub SetDefaultValuesForPrms(ByRef prms As fontParams, textType As Integer)

        Dim cn As New SqlConnection(DefineConStrtoSRV(4))
        Dim adap As New SqlDataAdapter("SELECT fontSize, fontName, bold, color, textType from eventTableExplanations2 where id = -1", cn)
        Dim dt As New DataTable
        adap.Fill(dt)
        cn.Dispose()
        adap.Dispose()

        prms.fontSize = dt(0)(0)
        prms.fontName = dt(0)(1)
        prms.bold = dt(0)(2)
        prms.color = dt(0)(3)
        prms.textType = textType
    End Sub

    Private Sub CreateButtons(popup As Form, ltblp As TableLayoutPanel)
        Dim button1 As New Button
        button1.DialogResult = DialogResult.OK
        button1.Text = ""
        button1.Size = New Size(40, 40)
        Dim req As HttpWebRequest = WebRequest.Create("https://cdn4.iconfinder.com/data/icons/evil-icons-user-interface/64/close2-32.png")
        Dim response As HttpWebResponse = req.GetResponse()
        Dim responseStream As Stream = response.GetResponseStream()
        Dim img = Image.FromStream(responseStream)
        button1.BackgroundImage = img
        button1.BackgroundImageLayout = ImageLayout.Stretch

        button1.Name = "OK"
        button1.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        button1.FlatStyle = FlatStyle.Flat
        button1.FlatAppearance.BorderSize = 0
        button1.FlatAppearance.MouseDownBackColor = Color.Transparent
        button1.FlatAppearance.MouseOverBackColor = Color.Transparent
        button1.BackColor = Color.Transparent

        popup.AcceptButton = button1
        ltblp.Controls.Add(button1)
        ltblp.SetCellPosition(button1, New TableLayoutPanelCellPosition(10, 0))
        ltblp.SetRowSpan(button1, 2)

        If ltblp.Controls("myGroupBox").Controls.Count > 1 Then
            button1 = New Button
            button1.Size = New Size(50, 50)
            req = WebRequest.Create("https://cdn4.iconfinder.com/data/icons/evil-icons-user-interface/64/arrow_left-128.png")
            response = req.GetResponse()
            responseStream = response.GetResponseStream()
            img = Image.FromStream(responseStream)
            button1.BackgroundImage = img
            button1.BackgroundImageLayout = ImageLayout.Stretch
            button1.Anchor = AnchorStyles.Left
            button1.FlatStyle = FlatStyle.Flat
            button1.FlatAppearance.BorderSize = 0
            button1.FlatAppearance.MouseDownBackColor = Color.Transparent
            button1.FlatAppearance.MouseOverBackColor = Color.Transparent
            button1.BackColor = Color.Transparent
            AddHandler button1.Click, AddressOf leftBtnClick
            ltblp.Controls.Add(button1)
            ltblp.SetCellPosition(button1, New TableLayoutPanelCellPosition(0, 9))
            ltblp.SetRowSpan(button1, 3)

            button1 = New Button
            button1.Size = New Size(50, 50)
            req = WebRequest.Create("https://cdn4.iconfinder.com/data/icons/evil-icons-user-interface/64/arrow_right-128.png")
            response = req.GetResponse()
            responseStream = response.GetResponseStream()
            img = Image.FromStream(responseStream)
            button1.BackgroundImage = img
            button1.BackgroundImageLayout = ImageLayout.Stretch
            button1.Anchor = AnchorStyles.Left
            button1.FlatStyle = FlatStyle.Flat
            button1.FlatAppearance.BorderSize = 0
            button1.FlatAppearance.MouseDownBackColor = Color.Transparent
            button1.FlatAppearance.MouseOverBackColor = Color.Transparent
            button1.BackColor = Color.Transparent
            AddHandler button1.Click, AddressOf rightBtnClick
            ltblp.Controls.Add(button1)
            ltblp.SetCellPosition(button1, New TableLayoutPanelCellPosition(10, 9))
            ltblp.SetRowSpan(button1, 3)
        End If

        response.Dispose()
        responseStream.Dispose()
    End Sub

    Private Sub leftBtnClick(sender As Object, e As EventArgs)
        Dim gb As GroupBox = DirectCast(sender, Button).Parent.Parent.Controls(0).Controls("myGroupBox")
        For i As Integer = 0 To gb.Controls.Count - 1
            If DirectCast(gb.Controls(i), RadioButton).Checked Then
                Dim rb As RadioButton
                Dim nextId As String = (Int32.Parse(gb.Controls(i).Name) - 1).ToString
                Try
                    DirectCast(gb.Controls(nextId), RadioButton).Checked = True
                    rb = DirectCast(gb.Controls(nextId), RadioButton)
                Catch ex As Exception
                    DirectCast(gb.Controls(gb.Controls.Count - 1), RadioButton).Checked = True
                    rb = DirectCast(gb.Controls(gb.Controls.Count - 1), RadioButton)
                End Try
                Call ShowNotification(DirectCast(rb.Parent.Parent, TableLayoutPanel), paramsForIds(Int32.Parse(rb.Name)))
                Exit Sub
            End If
        Next
    End Sub

    Private Sub rightBtnClick(sender As Object, e As EventArgs)
        Dim gb As GroupBox = DirectCast(sender, Button).Parent.Parent.Controls(0).Controls("myGroupBox")
        For i As Integer = 0 To gb.Controls.Count - 1
            If DirectCast(gb.Controls(i), RadioButton).Checked Then
                Dim rb As RadioButton
                Dim nextId As String = (Int32.Parse(gb.Controls(i).Name) + 1).ToString
                Try
                    DirectCast(gb.Controls(nextId), RadioButton).Checked = True
                    rb = DirectCast(gb.Controls(nextId), RadioButton)
                Catch ex As Exception
                    DirectCast(gb.Controls(0), RadioButton).Checked = True
                    rb = DirectCast(gb.Controls(0), RadioButton)
                End Try
                Call ShowNotification(DirectCast(rb.Parent.Parent, TableLayoutPanel), paramsForIds(Int32.Parse(rb.Name)))
                Exit Sub
            End If
        Next
    End Sub

End Class
