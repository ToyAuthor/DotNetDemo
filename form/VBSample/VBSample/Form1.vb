Imports CSLibrary

Public Class Form1
	Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
		Label1.Text = "123"
		Dim obj As Class1 = New Class1()
		Label1.Text = obj.countingStart(4).ToString()
	End Sub
End Class
