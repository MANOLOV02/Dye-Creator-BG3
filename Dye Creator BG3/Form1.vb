Imports System.ComponentModel
Imports System.IO
Imports System.IO.Compression
Imports System.IO.Packaging
Imports System.Runtime.Serialization
Imports System.Security.Cryptography
Imports System.Xml
Imports LSLib.LS

Public Class Form1
    Private Function SRGB0_1(r As Byte, g As Byte, b As Byte) As String
        Dim s1 As String = Format(Math.Round(r / 255, 8), "0.00000000")
        Dim s2 As String = Format(Math.Round(g / 255, 8), "0.00000000")
        Dim s3 As String = Format(Math.Round(b / 255, 8), "0.00000000")
        Return (s1 + " " + s2 + " " + s3).Replace(Application.CurrentCulture.NumberFormat.NumberDecimalSeparator, ".")
    End Function

    Private Function SRGB0_1(texto As String) As Color
        Dim r As Byte
        Dim g As Byte
        Dim b As Byte
        Dim s() As String = texto.Split(" ")
        If s.Count <> 3 Then Return Color.FromArgb(0, 0, 0)
        Try
            r = CInt(CDbl(s(0).Replace(".", Application.CurrentCulture.NumberFormat.NumberDecimalSeparator)) * 255)
            g = CInt(CDbl(s(1).Replace(".", Application.CurrentCulture.NumberFormat.NumberDecimalSeparator)) * 255)
            b = CInt(CDbl(s(2).Replace(".", Application.CurrentCulture.NumberFormat.NumberDecimalSeparator)) * 255)
        Catch ex As Exception
            r = 0
            g = 0
            b = 0
        End Try
        If r < 0 Or r > 255 Then Return Color.FromArgb(0, 0, 0)
        If g < 0 Or g > 255 Then Return Color.FromArgb(0, 0, 0)
        If b < 0 Or b > 255 Then Return Color.FromArgb(0, 0, 0)
        Return Color.FromArgb(r, g, b)
    End Function


    <Serializable>
    Public Class ModDefinition
        Property Dyes() As New List(Of Dye)
        Property ModFolder As String = "MDyes"
        Property ModId As String = ""
        Property ModAuthor As String = ""
        Property TreasureTables() As New List(Of String)
    End Class

    <Serializable>
    Public Class DyeComponent
        Property Valores As New List(Of String)
    End Class

    <Serializable>
    Public Class Dye
        Property Dye_ID As String = ""
        Property Dye_Colors_ID As String = ""
        Property Name As String = "Dye_Manolo_M"
        Property Label As String = ""
        Property LabelId As String = ""
        Property Label2 As String = ""
        Property Label2Id As String = ""
        Property Icon As String = "Item_LOOT_Dye_RedBrown_01"
        Property Rarity As String = "Common"
        Property Locked As Boolean = False
        Property Components As New DyeComponent
    End Class

    Dim NombreIconos As New SortedDictionary(Of String, Bitmap)
    Dim BaseColors As New SortedDictionary(Of String, String)
    Dim gameModDir As String
    Dim Lista_Armaduras As New List(Of ArmorColor)
    Dim serialArms As New System.Xml.Serialization.XmlSerializer(Lista_Armaduras.GetType)
    Dim Referencias() As String = {"Cloth_Primary", "Cloth_Secondary", "Cloth_Tertiary", "Color_01", "Color_02", "Color_03", "Custom_1", "Custom_2", "Leather_Primary", "Leather_Secondary", "Leather_Tertiary", "Metal_Primary", "Metal_Secondary", "Metal_Tertiary", "Accent_Color"}
    Dim Directorio As String = ""
    Dim copylist(19) As String
    Dim hasloaded As Boolean = False
    Dim ModData As New ModDefinition
    Dim serialMod As New System.Xml.Serialization.XmlSerializer(ModData.GetType)

    Private Sub CheckwriteDirect()
        If IO.File.Exists(IO.Path.Combine(gameModDir, ModData.ModFolder + ".pak")) Then
            CheckBox4.Enabled = True
        Else
            CheckBox4.Enabled = False
        End If
    End Sub

    Private Sub Llena_Lista_treasure()
        ListBox1.Items.Clear()
        For Each strg In ModData.TreasureTables
            ListBox1.Items.Add(strg)
        Next
    End Sub
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ModData.ModId = Guid.NewGuid.ToString
        ModData.ModFolder = "MDyes"
        ModData.ModAuthor = "Manolo"
        ModData.Dyes = New List(Of Dye)
        ModData.TreasureTables.Add("TUT_Chest_Potions")
        ModData.TreasureTables.Add("DEN_Entrance_Trade")
        TextBox17.Text = ModData.ModFolder
        TextBox18.Text = ModData.ModAuthor
        ComboBox4.Items.Add("TUT_Chest_Potions")
        ComboBox4.Items.Add("DEN_Entrance_Trade")
        ComboBox4.Items.Add("DEN_Volo_Trade")
        ComboBox4.Items.Add("CRE_GithQuartermistress_Trade")
        ComboBox4.Items.Add("UND_SocietyOfBrilliance_Hobgoblin")
        ComboBox4.Items.Add("HAV_HarperQuarterMaster")
        ComboBox4.Items.Add("MOO_InfernalTrader_Trade")
        ComboBox4.Items.Add("WYR_SharessCaress_Bartender_Trade")
        ComboBox4.Items.Add("LOW_MysticCarrion_Trade")
        ComboBox4.Items.Add("TWN_Hospital_CorpseTender")
        ComboBox4.Items.Add("SHA_MerregonTrader")  'HERE YOU CAN ADD MORE TRADERS I GOT BORED

        ComboBox4.SelectedIndex = 0

        Llena_Lista_treasure()

        Directorio = My.Settings.Directorio
        Application.CurrentCulture = New Globalization.CultureInfo("en-en") ' This is just to be sure it uses "." as decimal separator otherwise the packing will be wrong.

        gameModDir = IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Larian Studios\Baldur's Gate 3\Mods") ' Should be the game MOD folder location

        CheckwriteDirect()

        Dim runTimeResourceSet As Object
        Llen_armaduras()
        runTimeResourceSet = My.Resources.ResourceManager.GetResourceSet(System.Globalization.CultureInfo.CurrentCulture, True, True)

        For Each dicentry In runTimeResourceSet
            If (dicentry.Value.GetType() Is GetType(Bitmap)) Then
                NombreIconos.Add(dicentry.Key, dicentry.Value)
            End If
        Next

        ' ALL BASE DYES
        BaseColors.Add("Item_LOOT_Dye_Azure_01", "85fc7553-b1ca-cb0c-600c-2d0a1fb4c06c")
        BaseColors.Add("Item_LOOT_Dye_BlackBlue_01", "3a87940e-c9a2-494c-0026-a94a2087e128")
        BaseColors.Add("Item_LOOT_Dye_BlackGreen_01", "88d7c30f-c736-cc70-d005-d1169f73a58f")
        BaseColors.Add("Item_LOOT_Dye_BlackPink_01", "cfada95a-0ef4-0e97-5330-42fff41a7cbe")
        BaseColors.Add("Item_LOOT_Dye_BlackRed_01", "59e211f9-38bf-2013-a66b-27f075a7a057")
        BaseColors.Add("Item_LOOT_Dye_BlackTeal_01", "5f97bbfc-7dca-37b4-0285-768fd66f11e8")
        BaseColors.Add("Item_LOOT_Dye_Blue_01", "5bf267b7-cbba-02f6-64f1-6b7600b6d641")
        BaseColors.Add("Item_LOOT_Dye_BlueGreen_01", "9b822fd0-36ea-d24f-efff-f24e2b1c78c7")
        BaseColors.Add("Item_LOOT_Dye_BluePurple_01", "854e37e1-a840-ac3f-948b-a6630187d3e7")
        BaseColors.Add("Item_LOOT_Dye_BlueYellow_01", "ddc1e83b-8727-7900-94bc-72dc6e78d89a")
        BaseColors.Add("Item_LOOT_Dye_BlueYellow_02", "9d88e168-e638-65fa-feb3-9573ba3e3608")
        BaseColors.Add("Item_LOOT_Dye_Golden_01", "4157e913-f20d-037e-db5c-33a38d2b1e81")
        BaseColors.Add("Item_LOOT_Dye_Green_01", "ea44dc42-196e-5bbf-56e3-10fe5a21eb82")
        BaseColors.Add("Item_LOOT_Dye_Green_02", "ea44dc42-196e-5bbf-56e3-10fe5a21eb82")
        BaseColors.Add("Item_LOOT_Dye_GreenPink_01", "84b1e032-4013-a304-5e1b-867c4c07fc72")
        BaseColors.Add("Item_LOOT_Dye_GreenSage_01", "a9895745-150c-5621-bc1a-c05ea59224e1")
        BaseColors.Add("Item_LOOT_Dye_GreenSwamp_01", "7922733b-ebb1-1d40-2e5d-f68a1a450571")
        BaseColors.Add("Item_LOOT_Dye_IceCream_01", "baf0cd87-d867-0e2a-570f-67162f0c242b")
        BaseColors.Add("Item_LOOT_Dye_IceCream_02", "baf0cd87-d867-0e2a-570f-67162f0c242b")
        BaseColors.Add("Item_LOOT_Dye_IceCream_03", "7c32bee2-2804-ba2f-9421-479fb068dd74")
        BaseColors.Add("Item_LOOT_Dye_IceCream_04", "16febc6c-1fb8-970f-9d3d-73ab5bc3dc73")
        BaseColors.Add("Item_LOOT_Dye_Maroon_01", "1cdd0db3-f51e-b310-1cf8-06b05ae6213b")
        BaseColors.Add("Item_LOOT_Dye_Ocean_01", "8b2bc234-5b59-1dac-ad0b-981dcaadf1f8")
        BaseColors.Add("Item_LOOT_Dye_Orange_01", "d5c2b4ee-0d01-35c4-efe1-97a590cf1b33")
        BaseColors.Add("Item_LOOT_Dye_OrangeBlue_01", "81347759-e898-e086-4e85-8ff9b006f3de")
        BaseColors.Add("Item_LOOT_Dye_Pink_01", "dcda84b0-4981-90a0-0372-626285920845")
        BaseColors.Add("Item_LOOT_Dye_Purple_01", "27e27bb5-ec6d-f79d-6144-ab19625f99ee")
        BaseColors.Add("Item_LOOT_Dye_Purple_02", "323abe30-af8f-38b1-a0bd-bdbf1f30a4ac")
        BaseColors.Add("Item_LOOT_Dye_Purple_03", "3973c28b-e2ce-0fe6-0548-d8e9157a4b0e")
        BaseColors.Add("Item_LOOT_Dye_Purple_04", "7c8ae356-9720-d6b2-02e6-70479f45adec")
        BaseColors.Add("Item_LOOT_Dye_PurpleRed_01", "cca868e6-4720-6a07-8db7-1c117564e4e4")
        BaseColors.Add("Item_LOOT_Dye_Red", "980bdb9c-b9d0-5c57-8b9b-e4ac0db125ec")
        BaseColors.Add("Item_LOOT_Dye_RedBrown_01", "86668c08-3811-9f97-1a82-a7a2bc3da66d")
        BaseColors.Add("Item_LOOT_Dye_RedWhite_01", "ef743f2d-2d6c-74a9-c1e7-8f477269e6be")
        BaseColors.Add("Item_LOOT_Dye_Remover", "00000000-0000-0000-0000-000000000000")
        BaseColors.Add("Item_LOOT_Dye_RichRed_01", "51d9244b-3f97-a169-63bb-cd5773dfc47a")
        BaseColors.Add("Item_LOOT_Dye_RoyalBlue_01", "25f9b6dc-e7ab-ac6a-1d5a-529d02a36358")
        BaseColors.Add("Item_LOOT_Dye_Teal_01", "8b78d035-f64f-5e03-9fa9-ec44a3dc7832")
        BaseColors.Add("Item_LOOT_Dye_WhiteBlack_01", "455c4b21-4cda-3fec-7425-a557d140b972")
        BaseColors.Add("Item_LOOT_Dye_WhiteBrown_01", "612865e1-ac2c-30b7-dc50-207c95d3901f")
        BaseColors.Add("Item_LOOT_Dye_WhiteRed_01", "33f7e7b9-7e66-7893-b18f-e080f39fe3e3")
        BaseColors.Add("Item_LOOT_Laboratory_Flask_Glass_C", "68d055b3-c3df-ab42-857d-cfe747e4a85b")


        For x = 0 To NombreIconos.Count - 1
            ComboBox2.Items.Add(NombreIconos.Keys(x).ToString)
        Next


        Agrega(1)
        ComboBox1.Items.Add(ModData.Dyes(0).Name)
        ComboBox1.SelectedIndex = 0
        Selecciona_Icono()
        Dim x2 As New FileInfo(IO.Path.Combine(Directorio, "Mod.cfg"))
        If x2.Exists = True Then
            Cargar(Directorio)
        End If

    End Sub
    Private Sub Selecciona_Icono()
        Dim indice As Integer = 0
        For x = 0 To NombreIconos.Count - 1
            If ModData.Dyes(ComboBox1.SelectedIndex).Icon = NombreIconos.Keys(x).ToString Then
                indice = x : Exit For
            End If
        Next
        ComboBox2.SelectedIndex = indice

        ComboBox3.SelectedIndex = ComboBox3.Items.IndexOf(ModData.Dyes(ComboBox1.SelectedIndex).Rarity)
    End Sub
    Private Sub Agrega(Indice As Integer)
        Dim nuevo As New Dye
        For x = 0 To 14
            nuevo.Components.Valores.Add("0 0 0")
        Next
        nuevo.Name = "Dye_Manolo_M" + Indice.ToString.PadLeft(3, "0") + "_" + ModData.ModFolder
        nuevo.Dye_ID = Guid.NewGuid.ToString
        nuevo.Dye_Colors_ID = Guid.NewGuid.ToString
        nuevo.Label = "Tinte M" + Indice.ToString.PadLeft(3, "0")
        nuevo.Label2 = "Tiñe el equipo con el tinte M" + Indice.ToString.PadLeft(3, "0")
        nuevo.LabelId = "h" + Guid.NewGuid.ToString.Replace("-", "g")
        nuevo.Label2Id = "h" + Guid.NewGuid.ToString.Replace("-", "g")
        ModData.Dyes.Add(nuevo)
    End Sub


    Private Function Selectpar(pic As PictureBox) As TextBox
        Select Case pic.Name
            Case "PictureBox1"
                Return TextBox1
            Case "PictureBox2"
                Return TextBox2
            Case "PictureBox3"
                Return TextBox3
            Case "PictureBox4"
                Return TextBox4
            Case "PictureBox5"
                Return TextBox5
            Case "PictureBox6"
                Return TextBox6
            Case "PictureBox7"
                Return TextBox7
            Case "PictureBox8"
                Return TextBox8
            Case "PictureBox9"
                Return TextBox9
            Case "PictureBox10"
                Return TextBox10
            Case "PictureBox11"
                Return TextBox11
            Case "PictureBox12"
                Return TextBox12
            Case "PictureBox13"
                Return TextBox13
            Case "PictureBox14"
                Return TextBox14
            Case "PictureBox15"
                Return TextBox15
            Case Else
                Return Nothing
        End Select
    End Function
    Private Function Selectpar(tex As TextBox) As PictureBox
        Select Case tex.Name
            Case "TextBox1"
                Return PictureBox1
            Case "TextBox2"
                Return PictureBox2
            Case "TextBox3"
                Return PictureBox3
            Case "TextBox4"
                Return PictureBox4
            Case "TextBox5"
                Return PictureBox5
            Case "TextBox6"
                Return PictureBox6
            Case "TextBox7"
                Return PictureBox7
            Case "TextBox8"
                Return PictureBox8
            Case "TextBox9"
                Return PictureBox9
            Case "TextBox10"
                Return PictureBox10
            Case "TextBox11"
                Return PictureBox11
            Case "TextBox12"
                Return PictureBox12
            Case "TextBox13"
                Return PictureBox13
            Case "TextBox14"
                Return PictureBox14
            Case "TextBox15"
                Return PictureBox15
            Case Else
                Return Nothing
        End Select
    End Function
    Private Function Selectpar(but As Button) As TextBox
        Select Case but.Name.Replace("ButtonDark", "").Replace("ButtonBrigh", "")
            Case "1"
                Return TextBox1
            Case "2"
                Return TextBox2
            Case "3"
                Return TextBox3
            Case "4"
                Return TextBox4
            Case "5"
                Return TextBox5
            Case "6"
                Return TextBox6
            Case "7"
                Return TextBox7
            Case "8"
                Return TextBox8
            Case "9"
                Return TextBox9
            Case "10"
                Return TextBox10
            Case "11"
                Return TextBox11
            Case "12"
                Return TextBox12
            Case "13"
                Return TextBox13
            Case "14"
                Return TextBox14
            Case "15"
                Return TextBox15
            Case Else
                Return Nothing
        End Select
    End Function
    Private Sub DePicture_a_Texto(ByRef pic As PictureBox, ByRef tex As TextBox)
        ColorDialog1.Color = pic.BackColor
        If ColorDialog1.ShowDialog() <> DialogResult.OK Then ColorDialog1.Color = pic.BackColor
        Dim indice As Integer = CInt(pic.Name.Replace("PictureBox", "")) - 1

        pic.BackColor = ColorDialog1.Color
        Dim bntexto As String = SRGB0_1(pic.BackColor.R, pic.BackColor.G, pic.BackColor.B)
        If bntexto <> tex.Text Then
            tex.Text = SRGB0_1(pic.BackColor.R, pic.BackColor.G, pic.BackColor.B)
            ModData.Dyes(ComboBox1.SelectedIndex).Components.Valores(indice) = tex.Text
        End If

    End Sub
    Private Sub DeTexto_aPicture(ByRef tex As TextBox, ByRef pic As PictureBox)
        Dim ncolor As Color = SRGB0_1(tex.Text)
        Dim indice As Integer = CInt(pic.Name.Replace("PictureBox", "")) - 1
        If ncolor.ToArgb <> pic.BackColor.ToArgb Then
            pic.BackColor = Color.FromArgb(ncolor.R, ncolor.G, ncolor.B)
        End If
        tex.Text = SRGB0_1(pic.BackColor.R, pic.BackColor.G, pic.BackColor.B)
        ModData.Dyes(ComboBox1.SelectedIndex).Components.Valores(indice) = tex.Text
    End Sub
    Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles PictureBox1.Click, PictureBox2.Click, PictureBox3.Click, PictureBox4.Click, PictureBox5.Click, PictureBox6.Click, PictureBox7.Click, PictureBox8.Click, PictureBox9.Click, PictureBox10.Click, PictureBox11.Click, PictureBox12.Click, PictureBox13.Click, PictureBox14.Click, PictureBox15.Click
        DePicture_a_Texto(sender, Selectpar(CType(sender, PictureBox)))
    End Sub

    Private Sub TextBox1_Leave(sender As Object, e As EventArgs) Handles TextBox1.Leave, TextBox2.Leave, TextBox3.Leave, TextBox4.Leave, TextBox5.Leave, TextBox6.Leave, TextBox7.Leave, TextBox8.Leave, TextBox9.Leave, TextBox10.Leave, TextBox11.Leave, TextBox12.Leave, TextBox13.Leave, TextBox14.Leave, TextBox15.Leave
        DeTexto_aPicture(sender, Selectpar(CType(sender, TextBox)))
    End Sub
    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        CheckBox2.Checked = ModData.Dyes(ComboBox1.SelectedIndex).Locked
        Selecciona_Icono()
        Repinta()
    End Sub
    Private Sub Repinta()
        If ComboBox1.Items.Count = 0 Then Exit Sub
        TextBoxDescrip.Text = ModData.Dyes(ComboBox1.SelectedIndex).Label
        TextBoxDescrip2.Text = ModData.Dyes(ComboBox1.SelectedIndex).Label2
        For x = 0 To 14
            Dim textoColor As String = ModData.Dyes(ComboBox1.SelectedIndex).Components.Valores(x)
            Select Case x + 1
                Case 1
                    TextBox1.Text = textoColor
                    Label1.Text = Referencias(x)
                    DeTexto_aPicture(TextBox1, Selectpar(TextBox1))
                Case 2
                    TextBox2.Text = textoColor
                    Label2.Text = Referencias(x)
                    DeTexto_aPicture(TextBox2, Selectpar(TextBox2))
                Case 3
                    TextBox3.Text = textoColor
                    Label3.Text = Referencias(x)
                    DeTexto_aPicture(TextBox3, Selectpar(TextBox3))

                Case 4
                    TextBox4.Text = textoColor
                    Label4.Text = Referencias(x)
                    DeTexto_aPicture(TextBox4, Selectpar(TextBox4))

                Case 5
                    TextBox5.Text = textoColor
                    Label5.Text = Referencias(x)
                    DeTexto_aPicture(TextBox5, Selectpar(TextBox5))

                Case 6
                    TextBox6.Text = textoColor
                    Label6.Text = Referencias(x)
                    DeTexto_aPicture(TextBox6, Selectpar(TextBox6))

                Case 7
                    TextBox7.Text = textoColor
                    Label7.Text = Referencias(x)
                    DeTexto_aPicture(TextBox7, Selectpar(TextBox7))

                Case 8
                    TextBox8.Text = textoColor
                    Label8.Text = Referencias(x)
                    DeTexto_aPicture(TextBox8, Selectpar(TextBox8))

                Case 9
                    TextBox9.Text = textoColor
                    Label9.Text = Referencias(x)
                    DeTexto_aPicture(TextBox9, Selectpar(TextBox9))

                Case 10
                    TextBox10.Text = textoColor
                    Label10.Text = Referencias(x)
                    DeTexto_aPicture(TextBox10, Selectpar(TextBox10))

                Case 11
                    TextBox11.Text = textoColor
                    Label11.Text = Referencias(x)
                    DeTexto_aPicture(TextBox11, Selectpar(TextBox11))

                Case 12
                    TextBox12.Text = textoColor
                    Label12.Text = Referencias(x)
                    DeTexto_aPicture(TextBox12, Selectpar(TextBox12))

                Case 13
                    TextBox13.Text = textoColor
                    Label13.Text = Referencias(x)
                    DeTexto_aPicture(TextBox13, Selectpar(TextBox13))

                Case 14
                    TextBox14.Text = textoColor
                    Label14.Text = Referencias(x)
                    DeTexto_aPicture(TextBox14, Selectpar(TextBox14))

                Case 15
                    TextBox15.Text = textoColor
                    Label15.Text = Referencias(x)
                    DeTexto_aPicture(TextBox15, Selectpar(TextBox15))
            End Select
        Next

    End Sub
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim x As New FolderBrowserDialog With {.SelectedPath = Directorio}
        If x.ShowDialog() = DialogResult.OK Then
            Cargar(x.SelectedPath)
        End If
        Repinta()
        Selecciona_Icono()
    End Sub
    Private Sub Cargar(directorio2 As String)
        Directorio = directorio2
        Dim Archivo As IO.StreamReader
        Dim x2 As New FileInfo(IO.Path.Combine(Directorio, "Mod.cfg"))
        If x2.Exists = True Then
            Archivo = New IO.StreamReader(IO.Path.Combine(Directorio, "Mod.cfg"))
            ModData = serialMod.Deserialize(Archivo)
            Archivo.Close()
        Else
            MsgBox("No config file found in that directory", vbCritical + vbOKOnly, "Error")
            Exit Sub
        End If

        CheckwriteDirect()

        ComboBox1.Items.Clear()
        For z = 0 To ModData.Dyes.Count - 1
            If ModData.Dyes(z).Icon = "" Then ModData.Dyes(z).Icon = "Item_LOOT_Dye_RedBrown_01"
            If ModData.Dyes(z).Rarity = "" Then ModData.Dyes(z).Rarity = "Common"
            ComboBox1.Items.Add(ModData.Dyes(z).Name)
        Next

        ComboBox1.SelectedIndex = 0
        Llena_Lista_treasure()

        hasloaded = True
        My.Settings.Directorio = Directorio

    End Sub
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If hasloaded = False Then If MsgBox("Not ha cargado una versión anterior. Esto hará un nuevo mod incompatible con una versión previa.", vbYesNo + vbCritical, "Cuidado") = MsgBoxResult.No Then Exit Sub
        Dim x As New FolderBrowserDialog With {.SelectedPath = Directorio}
        If x.ShowDialog() = DialogResult.OK Then
            Directorio = x.SelectedPath
            Grabar()
        End If
    End Sub
    Private Sub Grabar()
        Graba_backup()
        Graba_localizacion()
        Graba_Mod()
        Graba_Objects()
        Graba_Treasure()
        Graba_Combo()
        Graba_Root()
        Graba_Valores()
        Pack()
        Graba_Configuracion()
    End Sub

    Private Sub Graba_configuracion()
        Dim Archivo As IO.StreamWriter
        Archivo = New IO.StreamWriter(IO.Path.Combine(Directorio, "Mod.cfg"))
        serialMod.Serialize(Archivo, ModData)
        Archivo.Close()
        My.Settings.Directorio = Directorio
    End Sub

    Private Sub Graba_backup()
        Dim z As New IO.FileInfo(IO.Path.Combine(Directorio, "Mod.cfg"))
        If z.Exists = True Then
            Dim Archivo As IO.StreamReader
            Dim listabak As ModDefinition
            Archivo = New IO.StreamReader(IO.Path.Combine(Directorio, "Mod.cfg"))
            listabak = serialMod.Deserialize(Archivo)
            Archivo.Close()
            Dim Backup As IO.StreamWriter
            Backup = New IO.StreamWriter(IO.Path.Combine(Directorio, "Mod.bak"))
            serialMod.Serialize(Backup, listabak)
            Backup.Flush()
            Backup.Close()
        End If
    End Sub
    Private Sub Graba_localizacion()
        IO.Directory.CreateDirectory(IO.Path.Combine(Directorio, "Localization\Spanish"))
        IO.Directory.CreateDirectory(IO.Path.Combine(Directorio, "Localization\English")) ' YOU CAN ADD ANY LOCALIZACION, FOR NOW IT IS REPEATING FOR ALL THE CHARACTERISTICS. 
        Dim Archivo As IO.StreamWriter
        For y = 0 To 1
            If y = 0 Then Archivo = New IO.StreamWriter(IO.Path.Combine(Directorio, "Localization\English\" + ModData.ModFolder + ".loca.xml")) Else Archivo = New IO.StreamWriter(IO.Path.Combine(Directorio, "Localization\Spanish\" + ModData.ModFolder + ".loca.xml"))
            Archivo.WriteLine("<?xml version=" + Chr(34) + "1.0" + Chr(34) + " encoding=" + Chr(34) + "utf-8" + Chr(34) + "?>")
            Archivo.WriteLine("<contentList>")
            For x = 0 To ModData.Dyes.Count - 1
                Archivo.WriteLine("  <content contentuid=" + Chr(34) + ModData.Dyes(x).LabelId + Chr(34) + " version=" + Chr(34) + "3" + Chr(34) + ">" + ModData.Dyes(x).Label + "</content>")
                Archivo.WriteLine("  <content contentuid=" + Chr(34) + ModData.Dyes(x).Label2Id + Chr(34) + " version=" + Chr(34) + "2" + Chr(34) + ">" + ModData.Dyes(x).Label2 + "</content> ")
            Next
            Archivo.WriteLine("</contentList>")
            Archivo.Flush()
            Archivo.Close()
        Next
    End Sub
    Private Sub Pack()

        ' Packing : NEEDS LSLIB 
        ' A HUGE TRY/CATH. If anything fails at least the folder will be created.

        Try
            Dim Resource As LSLib.LS.LocaResource
            Dim Resource2 As LSLib.LS.Resource
            Dim Formato As LSLib.LS.LocaFormat
            Dim Formato2 As Integer
            Dim conversionParams As LSLib.LS.ResourceConversionParameters
            Dim loadParams As LSLib.LS.ResourceLoadParameters


            ' English
            Resource = LSLib.LS.LocaUtils.Load(IO.Path.Combine(Directorio, "Localization\English\" + ModData.ModFolder + ".loca.xml"))
            Formato = 0
            LocaUtils.Save(Resource, IO.Path.Combine(Directorio, "Localization\English\" + ModData.ModFolder + ".loca.xml").Replace(".xml", ""), Formato)

            ' Spanish
            Resource = LSLib.LS.LocaUtils.Load(IO.Path.Combine(Directorio, "Localization\Spanish\" + ModData.ModFolder + ".loca.xml"))
            Formato = 0
            LocaUtils.Save(Resource, IO.Path.Combine(Directorio, "Localization\Spanish\" + ModData.ModFolder + ".loca.xml").Replace(".xml", ""), Formato)

            ' ADD ANY OTHER YOU ADDED 

            ' Mod, Objetc, Treasure, Combo (No Need)

            ' Root
            Dim que As String = IO.Path.Combine(Directorio, "Public\" + ModData.ModFolder + "\RootTemplates\" + ModData.ModFolder + ".lsf.lsx")
            Dim donde As String = que.Replace(".lsx", "")

            loadParams = ResourceLoadParameters.FromGameVersion(4)
            loadParams.ByteSwapGuids = True ' False si es version anterior
            conversionParams = ResourceConversionParameters.FromGameVersion(4)

            Resource2 = ResourceUtils.LoadResource(que, loadParams)
            Formato2 = 2
            ResourceUtils.SaveResource(Resource2, donde, Formato2, conversionParams)

            ' Valores
            que = IO.Path.Combine(Directorio, "Public\" + ModData.ModFolder + "\Content\Assets\Characters\[PAK]_DYE_Colors\" + ModData.ModFolder + ".lsf.lsx")
            donde = que.Replace(".lsx", "")
            Resource2 = ResourceUtils.LoadResource(que, loadParams)
            Formato2 = 2
            ResourceUtils.SaveResource(Resource2, donde, Formato2, conversionParams)


            Dim build As New PackageBuildData()
            Dim packfile As String = System.IO.Path.GetTempFileName
            Dim jsonfile As String = System.IO.Path.GetTempFileName
            Dim Zipfile As String = IO.Path.Combine(Directorio + "\..") + "\" + ModData.ModFolder + ".zip"


            If CheckBox3.Checked = True Or CheckBox4.Checked = True Then
                ' Package .pak
                build.Version = 18
                build.Compression = CompressionMethod.LZ4
                build.CompressionLevel = LSCompressionLevel.Fast
                build.Priority = 0
                Dim Packager As New Packager()
                Packager.CreatePackage(packfile, Directorio, build).Wait()

                If CheckBox3.Checked = True Then
                    ' Create Json
                    Dim md5 = System.Security.Cryptography.MD5.Create

                    Dim contentBytes As Byte() = File.ReadAllBytes(packfile)
                    md5.TransformFinalBlock(contentBytes, 0, contentBytes.Length)

                    Dim infomd5 As String = BitConverter.ToString(md5.Hash).Replace("-", "").ToLower()

                    Dim Archivo As IO.StreamWriter
                    Archivo = New IO.StreamWriter(jsonfile)
                    Archivo.WriteLine("{" _
                + Chr(34) + "Mods" + Chr(34) + ":[{" _
                + Chr(34) + "Author" + Chr(34) + ":" + Chr(34) + ModData.ModAuthor + Chr(34) + "," _
                + Chr(34) + "Name" + Chr(34) + ":" + Chr(34) + ModData.ModFolder + Chr(34) + "," _
                + Chr(34) + "Folder" + Chr(34) + ":" + Chr(34) + ModData.ModFolder + Chr(34) + "," _
                + Chr(34) + "Version" + Chr(34) + ":null," _
                + Chr(34) + "Description" + Chr(34) + ":" + Chr(34) + "Tintes customizables" + Chr(34) + "," _
                + Chr(34) + "UUID" + Chr(34) + ":" + Chr(34) + ModData.ModId + Chr(34) + "," _
                + Chr(34) + "Created" + Chr(34) + ":" + Chr(34) + "2024-01-26T20:49:03.3322887-03:00" + Chr(34) + "," _
                + Chr(34) + "Dependencies" + Chr(34) + ":[]," _
                + Chr(34) + "Group" + Chr(34) + ":" + Chr(34) + "4aa34aff-4c9c-44b1-8a6d-34aea5f8b826" + Chr(34) + "}]," _
                + Chr(34) + "MD5" + Chr(34) + ":" + Chr(34) + infomd5 + Chr(34) + "}")
                    Archivo.Flush()
                    Archivo.Close()

                    If IO.File.Exists(Zipfile) Then
                        IO.File.Delete(Zipfile)
                    End If

                    Using archive As ZipArchive = System.IO.Compression.ZipFile.Open(Zipfile, ZipArchiveMode.Create)
                        archive.CreateEntryFromFile(jsonfile, "Info.json")
                        archive.CreateEntryFromFile(packfile, ModData.ModFolder + ".pak")
                    End Using

                End If

                ' Graba pack Directo
                If CheckBox4.Enabled = True And CheckBox4.Checked = True Then
                    Try
                        System.IO.File.Copy(packfile, IO.Path.Combine(gameModDir, ModData.ModFolder + ".pak"), True)
                    Catch ex As Exception
                        MsgBox("Can' write the pak file to mod folder. Check the game is closed", vbCritical + vbOKOnly, "Error")
                    End Try

                End If

            End If


            IO.File.Delete(packfile)
            IO.File.Delete(jsonfile)




        Catch ex As Exception
            MsgBox("Error in packing, check LSLIB dll. The folder is created and should be usable with any packer", MsgBoxStyle.Critical + vbOKOnly, "Error in packing")
        End Try


    End Sub
    Private Sub Graba_Mod()
        IO.Directory.CreateDirectory(IO.Path.Combine(Directorio, "Mods\" + ModData.ModFolder + "\"))
        Dim Archivo As IO.StreamWriter
        Archivo = New IO.StreamWriter(IO.Path.Combine(Directorio, "Mods\" + ModData.ModFolder + "\meta.lsx"))
        Archivo.WriteLine("<?xml version=" + Chr(34) + "1.0" + Chr(34) + " encoding=" + Chr(34) + "UTF-8" + Chr(34) + "?>")
        Archivo.WriteLine("<save>")
        Archivo.WriteLine("    <version major=" + Chr(34) + "4" + Chr(34) + " minor=" + Chr(34) + "0" + Chr(34) + " revision=" + Chr(34) + "9" + Chr(34) + " build=" + Chr(34) + "331" + Chr(34) + "/>")
        Archivo.WriteLine("    <region id=" + Chr(34) + "Config" + Chr(34) + ">")
        Archivo.WriteLine("        <node id=" + Chr(34) + "root" + Chr(34) + ">")
        Archivo.WriteLine("            <children>")
        Archivo.WriteLine("                <node id=" + Chr(34) + "Dependencies" + Chr(34) + "/>")
        Archivo.WriteLine("                <node id=" + Chr(34) + "ModuleInfo" + Chr(34) + ">")
        Archivo.WriteLine("                    <attribute id=" + Chr(34) + "Author" + Chr(34) + " type=" + Chr(34) + "LSWString" + Chr(34) + " value=" + Chr(34) + ModData.ModAuthor + Chr(34) + "/>")
        Archivo.WriteLine("                    <attribute id=" + Chr(34) + "CharacterCreationLevelName" + Chr(34) + " type=" + Chr(34) + "FixedString" + Chr(34) + " value=" + Chr(34) + "" + Chr(34) + "/>")
        Archivo.WriteLine("                    <attribute id=" + Chr(34) + "Description" + Chr(34) + " type=" + Chr(34) + "LSWString" + Chr(34) + " value=" + Chr(34) + "Tintes customizables" + Chr(34) + "/>")
        Archivo.WriteLine("                    <attribute id=" + Chr(34) + "Folder" + Chr(34) + " type=" + Chr(34) + "LSWString" + Chr(34) + " value=" + Chr(34) + ModData.ModFolder + Chr(34) + "/>")
        Archivo.WriteLine("                    <attribute id=" + Chr(34) + "GMTemplate" + Chr(34) + " type=" + Chr(34) + "FixedString" + Chr(34) + " value=" + Chr(34) + "" + Chr(34) + "/>")
        Archivo.WriteLine("                    <attribute id=" + Chr(34) + "LobbyLevelName" + Chr(34) + " type=" + Chr(34) + "FixedString" + Chr(34) + " value=" + Chr(34) + "" + Chr(34) + "/>")
        Archivo.WriteLine("                    <attribute id=" + Chr(34) + "MD5" + Chr(34) + " type=" + Chr(34) + "LSString" + Chr(34) + " value=" + Chr(34) + "" + Chr(34) + "/>")
        Archivo.WriteLine("                    <attribute id=" + Chr(34) + "MainMenuBackgroundVideo" + Chr(34) + " type=" + Chr(34) + "FixedString" + Chr(34) + " value=" + Chr(34) + "" + Chr(34) + "/>")
        Archivo.WriteLine("                    <attribute id=" + Chr(34) + "MenuLevelName" + Chr(34) + " type=" + Chr(34) + "FixedString" + Chr(34) + " value=" + Chr(34) + "" + Chr(34) + "/>")
        Archivo.WriteLine("                    <attribute id=" + Chr(34) + "Name" + Chr(34) + " type=" + Chr(34) + "FixedString" + Chr(34) + " value=" + Chr(34) + ModData.ModFolder + Chr(34) + "/>")
        Archivo.WriteLine("                    <attribute id=" + Chr(34) + "NumPlayers" + Chr(34) + " type=" + Chr(34) + "uint8" + Chr(34) + " value=" + Chr(34) + "4" + Chr(34) + "/>")
        Archivo.WriteLine("                    <attribute id=" + Chr(34) + "PhotoBooth" + Chr(34) + " type=" + Chr(34) + "FixedString" + Chr(34) + " value=" + Chr(34) + "" + Chr(34) + "/>")
        Archivo.WriteLine("                    <attribute id=" + Chr(34) + "StartupLevelName" + Chr(34) + " type=" + Chr(34) + "FixedString" + Chr(34) + " value=" + Chr(34) + "" + Chr(34) + "/>")
        Archivo.WriteLine("                    <attribute id=" + Chr(34) + "Tags" + Chr(34) + " type=" + Chr(34) + "LSWString" + Chr(34) + " value=" + Chr(34) + "" + Chr(34) + "/>")
        Archivo.WriteLine("                    <attribute id=" + Chr(34) + "Type" + Chr(34) + " type=" + Chr(34) + "FixedString" + Chr(34) + " value=" + Chr(34) + "Add-on" + Chr(34) + "/>")
        Archivo.WriteLine("                    <attribute id=" + Chr(34) + "UUID" + Chr(34) + " type=" + Chr(34) + "FixedString" + Chr(34) + " value=" + Chr(34) + ModData.ModId + Chr(34) + "/>")
        Archivo.WriteLine("                    <attribute id=" + Chr(34) + "Version" + Chr(34) + " type=" + Chr(34) + "int32" + Chr(34) + " value=" + Chr(34) + "1" + Chr(34) + "/>")
        Archivo.WriteLine("                    <children>")
        Archivo.WriteLine("                        <node id=" + Chr(34) + "PublishVersion" + Chr(34) + ">")
        Archivo.WriteLine("                            <attribute id=" + Chr(34) + "Version" + Chr(34) + " type=" + Chr(34) + "int32" + Chr(34) + " value=" + Chr(34) + "268435456" + Chr(34) + "/>")
        Archivo.WriteLine("                        </node>")
        Archivo.WriteLine("                        <node id=" + Chr(34) + "Scripts" + Chr(34) + "/>")
        Archivo.WriteLine("                        <node id=" + Chr(34) + "TargetModes" + Chr(34) + ">")
        Archivo.WriteLine("                            <children>")
        Archivo.WriteLine("                                <node id=" + Chr(34) + "Target" + Chr(34) + ">")
        Archivo.WriteLine("                                    <attribute id=" + Chr(34) + "Object" + Chr(34) + " type=" + Chr(34) + "FixedString" + Chr(34) + " value=" + Chr(34) + "Story" + Chr(34) + "/>")
        Archivo.WriteLine("                                </node>")
        Archivo.WriteLine("                            </children>")
        Archivo.WriteLine("                        </node>")
        Archivo.WriteLine("                    </children>")
        Archivo.WriteLine("                </node>")
        Archivo.WriteLine("            </children>")
        Archivo.WriteLine("        </node>")
        Archivo.WriteLine("    </region>")
        Archivo.WriteLine("</save>")

        Archivo.Flush()
        Archivo.Close()
    End Sub
    Private Sub Graba_Objects()
        IO.Directory.CreateDirectory(IO.Path.Combine(Directorio, "Public\" + ModData.ModFolder + "\Stats\Generated\Data"))
        Dim Archivo As IO.StreamWriter
        Archivo = New IO.StreamWriter(IO.Path.Combine(Directorio, "Public\" + ModData.ModFolder + "\Stats\Generated\Data\Object.txt"))
        For x = 0 To ModData.Dyes.Count - 1
            Archivo.WriteLine("new entry " + Chr(34) + "OBJ_" + ModData.Dyes(x).Name + Chr(34) + "")
            Archivo.WriteLine("type " + Chr(34) + "Object" + Chr(34) + "")
            Archivo.WriteLine("using " + Chr(34) + "_Dye_Rare" + Chr(34) + "")
            Archivo.WriteLine("data " + Chr(34) + "RootTemplate" + Chr(34) + " " + Chr(34) + ModData.Dyes(x).Dye_ID + Chr(34) + "")
            Archivo.WriteLine("data " + Chr(34) + "Priority" + Chr(34) + " " + Chr(34) + "1" + Chr(34) + "")
            Archivo.WriteLine("data " + Chr(34) + "Rarity" + Chr(34) + " " + Chr(34) + ModData.Dyes(x).Rarity + Chr(34) + "")
            Archivo.WriteLine("")
        Next
        Archivo.Flush()
        Archivo.Close()
    End Sub
    Private Sub Graba_Treasure()
        IO.Directory.CreateDirectory(IO.Path.Combine(Directorio, "Public\" + ModData.ModFolder + "\Stats\Generated"))
        Dim Archivo As IO.StreamWriter
        Archivo = New IO.StreamWriter(IO.Path.Combine(Directorio, "Public\" + ModData.ModFolder + "\Stats\Generated\TreasureTable.txt"))
        For z = 0 To ModData.TreasureTables.Count - 1
            Archivo.WriteLine("new treasuretable " + Chr(34) + ModData.TreasureTables(z) + Chr(34) + "")
            Archivo.WriteLine("CanMerge 1")
            For x = 0 To ModData.Dyes.Count - 1
                Archivo.WriteLine("new subtable " + Chr(34) + "1,1" + Chr(34) + "")
                Archivo.WriteLine("object category " + Chr(34) + "I_OBJ_" + ModData.Dyes(x).Name + Chr(34) + ",1,0,0,0,0,0,0,0")
            Next x
            Archivo.WriteLine("")
        Next z
        Archivo.Flush()
        Archivo.Close()
    End Sub
    Private Sub Graba_Combo()
        IO.Directory.CreateDirectory(IO.Path.Combine(Directorio, "Public\" + ModData.ModFolder + "\Stats\Generated"))
        Dim Archivo As IO.StreamWriter
        Archivo = New IO.StreamWriter(IO.Path.Combine(Directorio, "Public\" + ModData.ModFolder + "\Stats\Generated\ItemCombos.txt"))
        For x = 0 To ModData.Dyes.Count - 1
            Archivo.WriteLine("new ItemCombination " + Chr(34) + "OBJ_" + ModData.Dyes(x).Name + Chr(34) + "")
            Archivo.WriteLine("data " + Chr(34) + "Type 1" + Chr(34) + " " + Chr(34) + "Object" + Chr(34) + "")
            Archivo.WriteLine("data " + Chr(34) + "Object 1" + Chr(34) + " " + Chr(34) + "OBJ_" + ModData.Dyes(x).Name + Chr(34) + "")
            Archivo.WriteLine("data " + Chr(34) + "Transform 1" + Chr(34) + " " + Chr(34) + "None" + Chr(34) + "")
            Archivo.WriteLine("data " + Chr(34) + "Type 2" + Chr(34) + " " + Chr(34) + "Category" + Chr(34) + "")
            Archivo.WriteLine("data " + Chr(34) + "Object 2" + Chr(34) + " " + Chr(34) + "DyableArmor" + Chr(34) + "")
            Archivo.WriteLine("data " + Chr(34) + "Transform 2" + Chr(34) + " " + Chr(34) + "Dye" + Chr(34) + "")
            Archivo.WriteLine("data " + Chr(34) + "DyeColorPresetResource" + Chr(34) + " " + Chr(34) + ModData.Dyes(x).Dye_Colors_ID + Chr(34) + "")
            Archivo.WriteLine("")
            Archivo.WriteLine("new ItemCombinationResult " + Chr(34) + "OBJ_" + ModData.Dyes(x).Name + "_1" + Chr(34) + "")
            Archivo.WriteLine("data " + Chr(34) + "ResultAmount 1" + Chr(34) + " " + Chr(34) + "1" + Chr(34) + "")
            Archivo.WriteLine("")
        Next x
        Archivo.Flush()
        Archivo.Close()
    End Sub
    Private Sub Graba_Root()
        IO.Directory.CreateDirectory(IO.Path.Combine(Directorio, "Public\" + ModData.ModFolder + "\RootTemplates"))
        Dim Archivo As IO.StreamWriter
        Archivo = New IO.StreamWriter(IO.Path.Combine(Directorio, "Public\" + ModData.ModFolder + "\RootTemplates\" + ModData.ModFolder + ".lsf.lsx"))
        Archivo.WriteLine("<?xml version=" + Chr(34) + "1.0" + Chr(34) + " encoding=" + Chr(34) + "utf-8" + Chr(34) + "?>")
        Archivo.WriteLine("<save>")
        Archivo.WriteLine("	<version major=" + Chr(34) + "4" + Chr(34) + " minor=" + Chr(34) + "0" + Chr(34) + " revision=" + Chr(34) + "9" + Chr(34) + " build=" + Chr(34) + "319" + Chr(34) + " />")
        Archivo.WriteLine("	<region id=" + Chr(34) + "Templates" + Chr(34) + ">")
        Archivo.WriteLine("		<node id=" + Chr(34) + "Templates" + Chr(34) + ">")
        Archivo.WriteLine("			<children>")
        For x = 0 To ModData.Dyes.Count - 1
            Archivo.WriteLine("		<!-- Black, Blood Red, Silver -->")
            Archivo.WriteLine("				<node id=" + Chr(34) + "GameObjects" + Chr(34) + ">")

            Archivo.WriteLine("					<attribute id=" + Chr(34) + "ColorPreset" + Chr(34) + " type=" + Chr(34) + "guid" + Chr(34) + " value=" + Chr(34) + ModData.Dyes(x).Dye_Colors_ID + Chr(34) + " />")
            Archivo.WriteLine("					<attribute id=" + Chr(34) + "DisplayName" + Chr(34) + " type=" + Chr(34) + "TranslatedString" + Chr(34) + " handle=" + Chr(34) + ModData.Dyes(x).LabelId + Chr(34) + " version=" + Chr(34) + "3" + Chr(34) + " />")
            Archivo.WriteLine("					<attribute id=" + Chr(34) + "Description" + Chr(34) + " type=" + Chr(34) + "TranslatedString" + Chr(34) + " handle=" + Chr(34) + ModData.Dyes(x).Label2Id + Chr(34) + " version=" + Chr(34) + "2" + Chr(34) + " />")
            Archivo.WriteLine("					<attribute id=" + Chr(34) + "LevelName" + Chr(34) + " type=" + Chr(34) + "FixedString" + Chr(34) + " value=" + Chr(34) + "" + Chr(34) + " />")
            Archivo.WriteLine("					<attribute id=" + Chr(34) + "MapKey" + Chr(34) + " type=" + Chr(34) + "FixedString" + Chr(34) + " value=" + Chr(34) + ModData.Dyes(x).Dye_ID + Chr(34) + " />")
            Archivo.WriteLine("					<attribute id=" + Chr(34) + "ParentTemplateId" + Chr(34) + " type=" + Chr(34) + "FixedString" + Chr(34) + " value=" + Chr(34) + "1a750a66-e5c2-40be-9f62-0a4bf3ddb403" + Chr(34) + " />")
            Archivo.WriteLine("					<attribute id=" + Chr(34) + "Icon" + Chr(34) + " type=" + Chr(34) + "FixedString" + Chr(34) + " value=" + Chr(34) + ModData.Dyes(x).Icon + Chr(34) + " /> ")
            Archivo.WriteLine("					<attribute id=" + Chr(34) + "VisualTemplate" + Chr(34) + " type=" + Chr(34) + "FixedString" + Chr(34) + " value=" + Chr(34) + "0adb9e3a-c865-1cf7-aad1-6fbf9261ccbe" + Chr(34) + " />")
            Archivo.WriteLine("					<attribute id=" + Chr(34) + "Stats" + Chr(34) + " type=" + Chr(34) + "FixedString" + Chr(34) + " value=" + Chr(34) + "OBJ_" + ModData.Dyes(x).Name + Chr(34) + " />")
            Archivo.WriteLine("					<attribute id=" + Chr(34) + "Type" + Chr(34) + " type=" + Chr(34) + "FixedString" + Chr(34) + " value=" + Chr(34) + "item" + Chr(34) + " />")
            Archivo.WriteLine("				</node>")
        Next

        Archivo.WriteLine("			</children>")
        Archivo.WriteLine("		</node>")
        Archivo.WriteLine("	</region>")
        Archivo.WriteLine("</save>")
        Archivo.Flush()
        Archivo.Close()
    End Sub
    Private Sub Graba_Valores()
        IO.Directory.CreateDirectory(IO.Path.Combine(Directorio, "Public\" + ModData.ModFolder + "\Content\Assets\Characters\[PAK]_DYE_Colors"))
        Dim Archivo As IO.StreamWriter
        Archivo = New IO.StreamWriter(IO.Path.Combine(Directorio, "Public\" + ModData.ModFolder + "\Content\Assets\Characters\[PAK]_DYE_Colors\" + ModData.ModFolder + ".lsf.lsx"))
        Archivo.WriteLine("<?xml version=" + Chr(34) + "1.0" + Chr(34) + " encoding=" + Chr(34) + "utf-8" + Chr(34) + "?>")
        Archivo.WriteLine("<save>")
        Archivo.WriteLine("	<version major=" + Chr(34) + "4" + Chr(34) + " minor=" + Chr(34) + "0" + Chr(34) + " revision=" + Chr(34) + "9" + Chr(34) + " build=" + Chr(34) + "0" + Chr(34) + " />")
        Archivo.WriteLine("	<region id=" + Chr(34) + "MaterialPresetBank" + Chr(34) + ">")
        Archivo.WriteLine("		<node id=" + Chr(34) + "MaterialPresetBank" + Chr(34) + ">")
        Archivo.WriteLine("			<children>")
        For x = 0 To ModData.Dyes.Count - 1
            Archivo.WriteLine("				<node id=" + Chr(34) + "Resource" + Chr(34) + ">")
            Archivo.WriteLine("					<attribute id=" + Chr(34) + "ID" + Chr(34) + " type=" + Chr(34) + "FixedString" + Chr(34) + " value=" + Chr(34) + ModData.Dyes(x).Dye_Colors_ID + Chr(34) + " />")
            Archivo.WriteLine("					<attribute id=" + Chr(34) + "Name" + Chr(34) + " type=" + Chr(34) + "LSString" + Chr(34) + " value=" + Chr(34) + ModData.Dyes(x).Name + Chr(34) + " />")
            Archivo.WriteLine("					<attribute id=" + Chr(34) + "_OriginalFileVersion_" + Chr(34) + " type=" + Chr(34) + "int64" + Chr(34) + " value=" + Chr(34) + "144115207403209023" + Chr(34) + " />")
            Archivo.WriteLine("					<children>")
            Archivo.WriteLine("						<node id=" + Chr(34) + "Presets" + Chr(34) + ">")
            Archivo.WriteLine("							<attribute id=" + Chr(34) + "MaterialResource" + Chr(34) + " type=" + Chr(34) + "FixedString" + Chr(34) + " value=" + Chr(34) + "" + Chr(34) + " />")
            Archivo.WriteLine("							<children>")
            Archivo.WriteLine("								<node id=" + Chr(34) + "ColorPreset" + Chr(34) + ">")
            Archivo.WriteLine("									<attribute id=" + Chr(34) + "ForcePresetValues" + Chr(34) + " type=" + Chr(34) + "bool" + Chr(34) + " value=" + Chr(34) + "False" + Chr(34) + " />")
            Archivo.WriteLine("									<attribute id=" + Chr(34) + "GroupName" + Chr(34) + " type=" + Chr(34) + "FixedString" + Chr(34) + " value=" + Chr(34) + "" + Chr(34) + " />")
            Archivo.WriteLine("									<attribute id=" + Chr(34) + "MaterialPresetResource" + Chr(34) + " type=" + Chr(34) + "FixedString" + Chr(34) + " value=" + Chr(34) + "" + Chr(34) + " />")
            Archivo.WriteLine("								</node>")
            Archivo.WriteLine("								<node id=" + Chr(34) + "MaterialPresets" + Chr(34) + " />")
            For y = 0 To 14
                Archivo.WriteLine("								<node id=" + Chr(34) + "Vector3Parameters" + Chr(34) + ">")
                Archivo.WriteLine("									<attribute id=" + Chr(34) + "Color" + Chr(34) + " type=" + Chr(34) + "bool" + Chr(34) + " value=" + Chr(34) + "True" + Chr(34) + " />")
                Archivo.WriteLine("									<attribute id=" + Chr(34) + "Custom" + Chr(34) + " type=" + Chr(34) + "bool" + Chr(34) + " value=" + Chr(34) + "False" + Chr(34) + " />")
                Archivo.WriteLine("									<attribute id=" + Chr(34) + "Enabled" + Chr(34) + " type=" + Chr(34) + "bool" + Chr(34) + " value=" + Chr(34) + "True" + Chr(34) + " />")
                Archivo.WriteLine("									<attribute id=" + Chr(34) + "Parameter" + Chr(34) + " type=" + Chr(34) + "FixedString" + Chr(34) + " value=" + Chr(34) + Referencias(y) + Chr(34) + " />")
                Archivo.WriteLine("									<attribute id=" + Chr(34) + "Value" + Chr(34) + " type=" + Chr(34) + "fvec3" + Chr(34) + " value=" + Chr(34) + ModData.Dyes(x).Components.Valores(y) + Chr(34) + " />")
                Archivo.WriteLine("								</node>")
            Next y
            Archivo.WriteLine("							</children>")
            Archivo.WriteLine("						</node>")
            Archivo.WriteLine("					</children>")
            Archivo.WriteLine("				</node>")
        Next
        Archivo.WriteLine("			</children>")
        Archivo.WriteLine("		</node>")
        Archivo.WriteLine("	</region>")
        Archivo.WriteLine("</save>")
        Archivo.Flush()
        Archivo.Close()
    End Sub
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Agrega(ComboBox1.Items.Count + 1)
        ComboBox1.Items.Add(ModData.Dyes(ComboBox1.Items.Count).Name)
        ComboBox1.SelectedIndex = ComboBox1.Items.Count - 1
    End Sub
    Private Sub TextBoxDescrip_TextChanged(sender As Object, e As EventArgs) Handles TextBoxDescrip2.Leave, TextBoxDescrip.Leave
        ModData.Dyes(ComboBox1.SelectedIndex).Label = TextBoxDescrip.Text
        ModData.Dyes(ComboBox1.SelectedIndex).Label2 = TextBoxDescrip2.Text
    End Sub

    Private Sub ComboBox2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox2.SelectedIndexChanged
        PictureBoxIcon.BackgroundImage = NombreIconos(NombreIconos.Keys(ComboBox2.SelectedIndex))
        ModData.Dyes(ComboBox1.SelectedIndex).Icon = NombreIconos.Keys(ComboBox2.SelectedIndex)
    End Sub

    Private Sub ComboBox3_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox3.SelectedIndexChanged
        ModData.Dyes(ComboBox1.SelectedIndex).Rarity = ComboBox3.Items(ComboBox3.SelectedIndex)
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        If ModData.Dyes(ComboBox1.SelectedIndex).Locked = False Then Button5.Enabled = True
        copylist(0) = TextBox1.Text
        copylist(1) = TextBox2.Text
        copylist(2) = TextBox3.Text
        copylist(3) = TextBox4.Text
        copylist(4) = TextBox5.Text
        copylist(5) = TextBox6.Text
        copylist(6) = TextBox7.Text
        copylist(7) = TextBox8.Text
        copylist(8) = TextBox9.Text
        copylist(9) = TextBox10.Text
        copylist(10) = TextBox11.Text
        copylist(11) = TextBox12.Text
        copylist(12) = TextBox13.Text
        copylist(13) = TextBox14.Text
        copylist(14) = TextBox15.Text
        copylist(15) = TextBoxDescrip.Text
        copylist(16) = TextBoxDescrip2.Text
        copylist(17) = ModData.Dyes(ComboBox1.SelectedIndex).Rarity
        copylist(18) = ModData.Dyes(ComboBox1.SelectedIndex).Icon
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        For x = 0 To 14
            ModData.Dyes(ComboBox1.SelectedIndex).Components.Valores(x) = copylist(x)
        Next
        If CheckBox1.Checked = True Then
            ModData.Dyes(ComboBox1.SelectedIndex).Label = copylist(15)
            ModData.Dyes(ComboBox1.SelectedIndex).Label2 = copylist(16)
            ModData.Dyes(ComboBox1.SelectedIndex).Rarity = copylist(17)
            ModData.Dyes(ComboBox1.SelectedIndex).Icon = copylist(18)
            Selecciona_Icono()
        End If
        Repinta()
    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        Copiar_base(BaseColors(ComboBox2.Items(ComboBox2.SelectedIndex).ToString))
    End Sub
    Private Sub Copiar_base(cual As String)
        Dim xml As New XmlDocument
        xml.LoadXml(My.Resources.DyesOriginales)
        Dim colostr As String = ""
        Dim colorpos As String = ""
        Dim element As XmlNode = xml.ChildNodes(1).ChildNodes(1).ChildNodes(0).ChildNodes(0)
        For Each elem As XmlNode In element.ChildNodes
            If elem.ChildNodes(0).Attributes("value").Value = cual Then
                For Each ecolor As XmlNode In elem.ChildNodes(3).ChildNodes(0).ChildNodes(1).ChildNodes
                    For Each atr As XmlNode In ecolor.ChildNodes
                        colorpos = ecolor.ChildNodes(3).Attributes(2).Value
                        colostr = ecolor.ChildNodes(4).Attributes(2).Value
                        Dim indice As Integer = 0
                        For x = 0 To Referencias.Count - 1
                            If Referencias(x) = colorpos Then indice = x
                        Next
                        ModData.Dyes(ComboBox1.SelectedIndex).Components.Valores(indice) = colostr
                    Next
                    Repinta()
                Next
            End If
        Next

    End Sub

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        Dim actual As Integer = ComboBox1.SelectedIndex
        Dim Nuevo As Integer = ComboBox1.Items.Count
        Agrega(ComboBox1.Items.Count + 1)
        ModData.Dyes(Nuevo).Label = ModData.Dyes(actual).Label
        ModData.Dyes(Nuevo).Label2 = ModData.Dyes(actual).Label2
        ModData.Dyes(Nuevo).Icon = ModData.Dyes(actual).Icon
        ModData.Dyes(Nuevo).Rarity = ModData.Dyes(actual).Rarity
        For x = 0 To 14
            ModData.Dyes(Nuevo).Components.Valores(x) = ModData.Dyes(actual).Components.Valores(x)
        Next
        ComboBox1.Items.Add(ModData.Dyes(ComboBox1.Items.Count).Name)
        ComboBox1.SelectedIndex = ComboBox1.Items.Count - 1
    End Sub



    Private Sub Darker(quien As Button, Optional inverse As Boolean = False)
        Dim texto As TextBox = Selectpar(quien)
        Dim ncolor As Color = SRGB0_1(texto.Text)
        Dim pic As PictureBox = Selectpar(texto)
        Dim indice As Integer = CInt(pic.Name.Replace("PictureBox", "")) - 1
        If ncolor.ToArgb <> pic.BackColor.ToArgb Then
            pic.BackColor = Color.FromArgb(ncolor.R, ncolor.G, ncolor.B)
        End If
        Dim mult As Double = 0.95
        If inverse Then mult = 1.05
        Dim nrd As Integer = pic.BackColor.R
        Dim nrg As Integer = pic.BackColor.G
        Dim nrb As Integer = pic.BackColor.B
        If DarkerR.Checked = True Then nrd = CInt(nrd * mult)
        If DarkerG.Checked = True Then nrg = CInt(nrg * mult)
        If DarkerB.Checked = True Then nrb = CInt(nrb * mult)
        If DarkerR.Checked = True And inverse = True And pic.BackColor.R <> 0 Then If nrd = pic.BackColor.R Then nrd += 1
        If DarkerG.Checked = True And inverse = True And pic.BackColor.G <> 0 Then If nrg = pic.BackColor.G Then nrg += 1
        If DarkerB.Checked = True And inverse = True And pic.BackColor.B <> 0 Then If nrb = pic.BackColor.B Then nrb += 1
        If nrd > 255 Then nrd = 255
        If nrg > 255 Then nrg = 255
        If nrb > 255 Then nrb = 255
        If nrd < 0 Then nrd = 0
        If nrg < 0 Then nrg = 0
        If nrb < 0 Then nrb = 0
        texto.Text = SRGB0_1(nrd, nrg, nrb)
        ModData.Dyes(ComboBox1.SelectedIndex).Components.Valores(indice) = texto.Text
    End Sub
    Private Sub ButtonDark1_Click(sender As Object, e As EventArgs) Handles ButtonDark1.Click, ButtonDark2.Click, ButtonDark3.Click, ButtonDark4.Click, ButtonDark5.Click, ButtonDark6.Click, ButtonDark7.Click, ButtonDark8.Click, ButtonDark9.Click, ButtonDark10.Click, ButtonDark11.Click, ButtonDark12.Click, ButtonDark13.Click, ButtonDark14.Click, ButtonDark15.Click
        Darker(sender)

    End Sub

    Private Sub ButtonBrigh1_Click(sender As Object, e As EventArgs) Handles ButtonBrigh1.Click, ButtonBrigh2.Click, ButtonBrigh3.Click, ButtonBrigh4.Click, ButtonBrigh5.Click, ButtonBrigh6.Click, ButtonBrigh7.Click, ButtonBrigh8.Click, ButtonBrigh9.Click, ButtonBrigh10.Click, ButtonBrigh11.Click, ButtonBrigh12.Click, ButtonBrigh13.Click, ButtonBrigh14.Click, ButtonBrigh15.Click
        Darker(sender, True)
    End Sub

    Private Sub CheckBox2_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox2.CheckedChanged
        ModData.Dyes(ComboBox1.SelectedIndex).Locked = CheckBox2.Checked
        Lock(Not CheckBox2.Checked)
    End Sub
    Private Sub Lock(Estado As Boolean)
        PictureBox1.Enabled = Estado
        PictureBox2.Enabled = Estado
        PictureBox3.Enabled = Estado
        PictureBox4.Enabled = Estado
        PictureBox5.Enabled = Estado
        PictureBox6.Enabled = Estado
        PictureBox7.Enabled = Estado
        PictureBox8.Enabled = Estado
        PictureBox9.Enabled = Estado
        PictureBox10.Enabled = Estado
        PictureBox11.Enabled = Estado
        PictureBox12.Enabled = Estado
        PictureBox13.Enabled = Estado
        PictureBox14.Enabled = Estado
        PictureBox15.Enabled = Estado

        TextBox1.Enabled = Estado
        TextBox2.Enabled = Estado
        TextBox3.Enabled = Estado
        TextBox4.Enabled = Estado
        TextBox5.Enabled = Estado
        TextBox6.Enabled = Estado
        TextBox7.Enabled = Estado
        TextBox8.Enabled = Estado
        TextBox9.Enabled = Estado
        TextBox10.Enabled = Estado
        TextBox11.Enabled = Estado
        TextBox12.Enabled = Estado
        TextBox13.Enabled = Estado
        TextBox14.Enabled = Estado
        TextBox15.Enabled = Estado

        ButtonDark1.Enabled = Estado
        ButtonDark2.Enabled = Estado
        ButtonDark3.Enabled = Estado
        ButtonDark4.Enabled = Estado
        ButtonDark5.Enabled = Estado
        ButtonDark6.Enabled = Estado
        ButtonDark7.Enabled = Estado
        ButtonDark8.Enabled = Estado
        ButtonDark9.Enabled = Estado
        ButtonDark10.Enabled = Estado
        ButtonDark11.Enabled = Estado
        ButtonDark12.Enabled = Estado
        ButtonDark13.Enabled = Estado
        ButtonDark14.Enabled = Estado
        ButtonDark15.Enabled = Estado

        ButtonBrigh1.Enabled = Estado
        ButtonBrigh2.Enabled = Estado
        ButtonBrigh3.Enabled = Estado
        ButtonBrigh4.Enabled = Estado
        ButtonBrigh5.Enabled = Estado
        ButtonBrigh6.Enabled = Estado
        ButtonBrigh7.Enabled = Estado
        ButtonBrigh8.Enabled = Estado
        ButtonBrigh9.Enabled = Estado
        ButtonBrigh10.Enabled = Estado
        ButtonBrigh11.Enabled = Estado
        ButtonBrigh12.Enabled = Estado
        ButtonBrigh13.Enabled = Estado
        ButtonBrigh14.Enabled = Estado
        ButtonBrigh15.Enabled = Estado

        TextBoxDescrip.Enabled = Estado
        TextBoxDescrip2.Enabled = Estado
        ComboBox2.Enabled = Estado
        ComboBox3.Enabled = Estado
        Button6.Enabled = Estado
        Button5.Enabled = False
        If copylist(0) <> "" And Estado = True Then Button5.Enabled = True

        CheckBox1.Checked = Estado

    End Sub

    <Serializable>
    Public Class ArmorColor
        Property Nombre As String = ""
        Property Id As String = ""
        Property ParentId As String = ""
        Property Source As String = "CharPreset"
        Property Components As New DyeComponent
        Property Totree As Boolean = False
    End Class

    Private Sub Llen_armaduras()
        Lista_Armaduras.Clear()
        TreeView1.Nodes.Clear()
        Dim raiz As XmlNode
        Dim xml As New XmlDocument


        Dim REPROCESA As Boolean = False ' TRUE FOR REPROCESSING FROM UNPACKED DATA . VERY INEFFICIENT AND MANUAL BUT I ONLY NEEDED TO WORK ONCE. AFTER PROCESSING I SAVE THE OUTPUT FILE AS RESOURCE

        If REPROCESA Then
            ' Llena Material Resources de Char Preset
            Dim elem2 As XmlNode
            Dim elem3 As XmlNode
            Dim Nombre As String
            Dim Parentid As String
            Dim VisualTemplate As String
            Dim str As String
            Dim id As String
            Dim comp As New DyeComponent

            'Llena los presets
            Dim templateDirectories As New List(Of String)
            templateDirectories.Add("D:\Modders Multi Tool\UnpackedData\Shared\Public\Shared\Content\Assets\Characters\[PAK]_Character_Presets")
            templateDirectories.Add("D:\Modders Multi Tool\UnpackedData\Shared\Public\SharedDev\Content\Assets\Characters\[PAK]_Character_Presets")
            templateDirectories.Add("D:\Modders Multi Tool\UnpackedData\Shared\Public\SharedDev\Content\Assets\Characters\Humans\[PAK]_Male_Clothing")
            templateDirectories.Add("D:\Modders Multi Tool\UnpackedData\Shared\Public\SharedDev\Content\Assets\Characters\Humans\[PAK]_Male_Armor")
            templateDirectories.Add("D:\Modders Multi Tool\UnpackedData\Shared\Public\SharedDev\Content\Assets\Characters\Humans\[PAK]_Female_Armor")
            templateDirectories.Add("D:\Modders Multi Tool\UnpackedData\Shared\Public\SharedDev\Content\Assets\Characters\Humans\[PAK]_Female_Clothing")
            templateDirectories.Add("D:\Modders Multi Tool\UnpackedData\Shared\Public\SharedDev\Content\Assets\Characters\HumansStrong\[PAK]_Male_Armor")
            templateDirectories.Add("D:\Modders Multi Tool\UnpackedData\Shared\Public\SharedDev\Content\Assets\Characters\Humans\[PAK]_FemaleStrong_Armor")
            templateDirectories.Add("D:\Modders Multi Tool\UnpackedData\Shared\Public\Shared\Content\Assets\Characters\Humans\[PAK]_Male_Clothing")
            templateDirectories.Add("D:\Modders Multi Tool\UnpackedData\Shared\Public\Shared\Content\Assets\Characters\Humans\[PAK]_Male_Armor")
            templateDirectories.Add("D:\Modders Multi Tool\UnpackedData\Shared\Public\Shared\Content\Assets\Characters\Humans\[PAK]_Female_Clothing")
            templateDirectories.Add("D:\Modders Multi Tool\UnpackedData\Shared\Public\Shared\Content\Assets\Characters\Humans\[PAK]_Female_Armor")
            templateDirectories.Add("D:\Modders Multi Tool\UnpackedData\Shared\Public\Shared\Content\Assets\Characters\Character Editor Presets\Origin Presets\[PAK]_Karlach")
            templateDirectories.Add("D:\Modders Multi Tool\UnpackedData\Shared\Public\Shared\Content\Assets\Characters\[PAK]_Creature_Presets")
            templateDirectories.Add("D:\Modders Multi Tool\UnpackedData\Shared\Public\Shared\Content\Assets\Characters\[PAK]_DYE_Colors")
            templateDirectories.Add("D:\Modders Multi Tool\UnpackedData\Shared\Public\Shared\Content\[PAK]_CharacterVisuals")
            templateDirectories.Add("D:\Modders Multi Tool\UnpackedData\Shared\Public\SharedDev\Content\Assets\Characters\Githyanki\[PAK]_Male_Clothing")
            templateDirectories.Add("D:\Modders Multi Tool\UnpackedData\Shared\Public\SharedDev\Content\Assets\Characters\Githyanki\[PAK]_Male_Armor")
            templateDirectories.Add("D:\Modders Multi Tool\UnpackedData\Shared\Public\SharedDev\Content\Assets\Characters\Githyanki\[PAK]_FeMale_Clothing")
            templateDirectories.Add("D:\Modders Multi Tool\UnpackedData\Shared\Public\SharedDev\Content\Assets\Characters\Githyanki\[PAK]_FeMale_Armor")
            templateDirectories.Add("D:\Modders Multi Tool\UnpackedData\Shared\Public\Shared\Content\Assets\Characters\Githyanki\[PAK]_Male_Clothing")
            templateDirectories.Add("D:\Modders Multi Tool\UnpackedData\Shared\Public\Shared\Content\Assets\Characters\Githyanki\[PAK]_Male_Armor")
            templateDirectories.Add("D:\Modders Multi Tool\UnpackedData\Shared\Public\Shared\Content\Assets\Characters\Githyanki\[PAK]_FeMale_Clothing")
            templateDirectories.Add("D:\Modders Multi Tool\UnpackedData\Shared\Public\Shared\Content\Assets\Characters\Githyanki\[PAK]_FeMale_Armor")
            templateDirectories.Add("D:\Modders Multi Tool\UnpackedData\Shared\Public\Shared\Content\Assets\Characters\Creatures\Cambion\[PAK]_Male_Clothing")
            templateDirectories.Add("D:\Modders Multi Tool\UnpackedData\Shared\Public\Shared\Content\Assets\Characters\Creatures\Cambion\[PAK]_FeMale_Clothing")
            templateDirectories.Add("D:\Modders Multi Tool\UnpackedData\Shared\Public\Shared\Content\Assets\_Helpers\[PAK]__Helpers")
            templateDirectories.Add("D:\Modders Multi Tool\UnpackedData\Shared\Public\Shared\Content\Assets\[PAK]_ColorAndMaterialRoom")
            templateDirectories.Add("D:\Modders Multi Tool\UnpackedData\Shared\Public\SharedDev\Content\Assets\Characters\HalfOrcs\[PAK]_Male_Clothing")
            templateDirectories.Add("D:\Modders Multi Tool\UnpackedData\Shared\Public\SharedDev\Content\Assets\Characters\HalfOrcs\[PAK]_Male_Armor")
            templateDirectories.Add("D:\Modders Multi Tool\UnpackedData\Shared\Public\SharedDev\Content\Assets\Characters\HalfOrcs\[PAK]_FeMale_Clothing")
            templateDirectories.Add("D:\Modders Multi Tool\UnpackedData\Shared\Public\SharedDev\Content\Assets\Characters\HalfOrcs\[PAK]_FeMale_Armor")
            templateDirectories.Add("D:\Modders Multi Tool\UnpackedData\Shared\Public\Shared\Content\Assets\Characters\Creatures\Mindflayer\[PAK]_Armor")
            templateDirectories.Add("D:\Modders Multi Tool\UnpackedData\Shared\Public\Shared\Content\Assets\Characters\Elves\[PAK]_Male_Armor")
            templateDirectories.Add("D:\Modders Multi Tool\UnpackedData\Shared\Public\Shared\Content\Assets\Characters\Humans\[PAK]_FemaleStrong_Armor")
            templateDirectories.Add("D:\Modders Multi Tool\UnpackedData\Shared\Public\SharedDev\Content\Assets\Characters\Humans\[PAK]_Male_Clothing_Circus")
            templateDirectories.Add("D:\Modders Multi Tool\UnpackedData\Shared\Public\SharedDev\Content\Assets\Characters\Dragonborn\[PAK]_Male_Clothing")
            templateDirectories.Add("D:\Modders Multi Tool\UnpackedData\Shared\Public\SharedDev\Content\Assets\Characters\Creatures\Bhaal_Butler\[PAK]_Body")
            templateDirectories.Add("D:\Modders Multi Tool\UnpackedData\Shared\Public\Shared\Content\Assets\Characters\[PAK]_Shared_Materials")
            templateDirectories.Add("D:\Modders Multi Tool\UnpackedData\Patch5_Hotfix2\Public\Shared\Content\Assets\Loot\[PAK]_Armor")
            templateDirectories.Add("D:\Modders Multi Tool\UnpackedData\Patch5_Hotfix4\Public\Shared\Content\Assets\Characters\HalfElves\Heads\[PAK]_HEL_F_Head_Shadowheart")
            templateDirectories.Add("D:\Modders Multi Tool\UnpackedData\Shared\Public\Shared\Content\Assets\Effects\Materials\[PAK]_Model")
            templateDirectories.Add("D:\Modders Multi Tool\UnpackedData\Patch5_Hotfix2\Public\Shared\Content\Assets\Characters\Humans\Heads\[PAK]_HUM_F_Head_Mizora")
            templateDirectories.Add("D:\Modders Multi Tool\UnpackedData\Shared\Public\Shared\Content\Assets\Characters\Creatures\Merregon\[PAK]_Body")

            For z = 0 To 1
                For y = 0 To templateDirectories.Count - 1

                    xml.Load(IO.Path.Combine(templateDirectories(y), "_merged.lsf.lsx"))

                    Select Case z
                        Case 0
                            'Llena los presets
                            raiz = xml.ChildNodes(1).SelectSingleNode("region[@id='MaterialPresetBank']")
                            If Not IsNothing(raiz) Then
                                raiz = raiz.SelectSingleNode("node[@id='MaterialPresetBank']")
                                If IsNothing(raiz) Then Throw New Exception
                                raiz = raiz.SelectSingleNode("children")
                                If IsNothing(raiz) Then Throw New Exception

                                For Each elem As XmlNode In raiz.SelectNodes("node[@id='Resource']")
                                    str = ""
                                    Nombre = elem.SelectSingleNode("attribute[@id='Name']").Attributes(2).Value
                                    id = elem.SelectSingleNode("attribute[@id='ID']").Attributes(2).Value

                                    If Not IsNothing(elem.SelectSingleNode("children")) AndAlso Not IsNothing(elem.SelectSingleNode("children").SelectSingleNode("node[@id='Presets']")) Then
                                        elem2 = elem.SelectSingleNode("children").SelectSingleNode("node[@id='Presets']")
                                        If Not IsNothing(elem2.SelectSingleNode("children")) Then
                                            comp = New DyeComponent
                                            elem3 = elem2.SelectSingleNode("children")
                                            For x = 0 To 14
                                                comp.Valores.Add("1 1 1")
                                            Next
                                            For Each elem4 As XmlNode In elem3.SelectNodes("node[@id='Vector3Parameters']")
                                                Dim que As String = elem4.SelectSingleNode("attribute[@id='Parameter']").Attributes(2).Value
                                                For x = 0 To 14
                                                    If Referencias(x) = que Then comp.Valores(x) = elem4.SelectSingleNode("attribute[@id='Value']").Attributes(2).Value : str = "Ok"
                                                Next
                                            Next
                                        End If
                                    End If
                                    If str <> "" Then Lista_Armaduras.Add(New ArmorColor With {.Nombre = Nombre, .Components = comp, .Id = id})
                                Next
                            End If
                        Case 1
                            ' Llena Material Banl
                            raiz = xml.ChildNodes(1).SelectSingleNode("region[@id='MaterialBank']")
                            If Not IsNothing(raiz) Then
                                raiz = raiz.SelectSingleNode("node[@id='MaterialBank']")
                                If IsNothing(raiz) Then Throw New Exception
                                raiz = raiz.SelectSingleNode("children")
                                If IsNothing(raiz) Then Throw New Exception

                                For Each elem As XmlNode In raiz.SelectNodes("node[@id='Resource']")
                                    str = ""
                                    Nombre = elem.SelectSingleNode("attribute[@id='Name']").Attributes(2).Value
                                    id = elem.SelectSingleNode("attribute[@id='ID']").Attributes(2).Value

                                    If Nombre = "HUM_M_CLT_Bard_Collar_A" Or Nombre = "HUM_M_Prop_Harper_Pin_A" Or Nombre = "Helper_Invisible_NoOutline" Or Nombre = "Test_Primitives_A_DONTNOD_GrayPlaster_Mat" Or Nombre = "HUM_M_ARM_Shar_Crown_B_Broken" Or Nombre = "HAIR_Shadowheart_Evil_Accessories" Or Nombre = "HAIR_Shadowheart_Accessories" Or Nombre = "HAIR_Mizora" Or Nombre = "DEC_HAG_Victims_Eye" Then
                                        comp = New DyeComponent
                                        For x = 0 To 14
                                            comp.Valores.Add("1 1 1")
                                        Next
                                        str = "Ok"
                                    End If



                                    If Not IsNothing(elem.SelectSingleNode("children")) AndAlso Not IsNothing(elem.SelectSingleNode("children").SelectSingleNode("node[@id='Vector3Parameters']")) Then
                                        comp = New DyeComponent
                                        For x = 0 To 14
                                            comp.Valores.Add("1 1 1")
                                        Next
                                        For Each elem4 As XmlNode In elem.SelectSingleNode("children").SelectNodes("node[@id='Vector3Parameters']")
                                            Dim que As String = elem4.SelectSingleNode("attribute[@id='ParameterName']").Attributes(2).Value
                                            For x = 0 To 14
                                                If Referencias(x) = que Then comp.Valores(x) = elem4.SelectSingleNode("attribute[@id='Value']").Attributes(2).Value : str = "Ok"
                                            Next
                                        Next
                                    End If
                                    If str <> "" Then Lista_Armaduras.Add(New ArmorColor With {.Nombre = Nombre, .Components = comp, .Id = id})
                                Next
                            End If
                    End Select


                Next
            Next


            ' Llena Visual Templates
            Dim materialId As String = ""
            Dim matidencontrado As String = ""
            For y = 0 To 3
                If y = 0 Then xml.Load("D:\Modders Multi Tool\UnpackedData\Shared\Public\Shared\Content\Assets\Characters\Humans\[PAK]_Male_Clothing\_merged.lsf.lsx")
                If y = 1 Then xml.Load("D:\Modders Multi Tool\UnpackedData\Shared\Public\Shared\Content\Assets\Equipment\[PAK]_Humans\_Merged.lsf.lsx")
                If y = 2 Then xml.Load("D:\Modders Multi Tool\UnpackedData\Shared\Public\SharedDev\Content\Assets\Loot\[PAK]_Armor\_Merged.lsf.lsx")
                If y = 3 Then xml.Load("D:\Modders Multi Tool\UnpackedData\Shared\Public\Shared\Content\Assets\Loot\[PAK]_Armor\_Merged.lsf.lsx")
                'If y = 4 Then xml.Load("D:\Modders Multi Tool\UnpackedData\Shared\Public\Shared\Content\Assets\Decoration\[PAK]_Hag\_Merged.lsf.lsx")
                'If y = 5 Then xml.Load("D:\Modders Multi Tool\UnpackedData\Patch5_Hotfix5\Public\Shared\Content\Assets\Decoration\[PAK]_Ketheric\_Merged.lsf.lsx")

                raiz = xml.ChildNodes(1).SelectSingleNode("region[@id='VisualBank']")
                raiz = raiz.SelectSingleNode("node[@id='VisualBank']")
                If IsNothing(raiz) Then Throw New Exception

                For Each elem As XmlElement In raiz.SelectSingleNode("children").ChildNodes
                    materialId = ""
                    id = elem.SelectSingleNode("attribute[@id='ID']").Attributes(2).Value
                    Nombre = elem.SelectSingleNode("attribute[@id='Name']").Attributes(2).Value
                    matidencontrado = ""
                    If Not IsNothing(elem.SelectSingleNode("children")) Then
                        If Not IsNothing(elem.SelectSingleNode("children").SelectSingleNode("node[@id='Objects']")) Then
                            For Each elem4 As XmlNode In elem.SelectSingleNode("children").SelectNodes("node[@id='Objects']")
                                If Not IsNothing(elem4.SelectSingleNode("attribute[@id='MaterialID']")) Then
                                    materialId = elem4.SelectSingleNode("attribute[@id='MaterialID']").Attributes(2).Value
                                    For z = 0 To Lista_Armaduras.Count - 1
                                        If Lista_Armaduras(z).Id = materialId Then
                                            matidencontrado = "Encontrado"
                                            comp = New DyeComponent

                                            ' Pone valores detectables
                                            For x = 0 To 14
                                                comp.Valores.Add(Lista_Armaduras(z).Components.Valores(x))
                                            Next

                                        End If
                                    Next
                                    If matidencontrado = "" Then
                                        Debug.Print("Not Found: " + Nombre)
                                    Else

                                    End If
                                End If
                            Next
                            If matidencontrado <> "" Then Lista_Armaduras.Add(New ArmorColor With {.Nombre = Nombre, .Source = "Visual Templates", .Components = comp, .ParentId = materialId, .Id = id})

                        End If
                    End If
                Next
            Next

            'Llena Reemplazos
            For y = 0 To 3
                If y = 0 Then xml.Load("D:\Modders Multi Tool\UnpackedData\Shared\Public\Shared\RootTemplates\_merged.lsf.lsx")
                If y = 1 Then xml.Load("D:\Modders Multi Tool\UnpackedData\Shared\Public\SharedDev\RootTemplates\_merged.lsf.lsx")
                If y = 2 Then xml.Load("D:\Modders Multi Tool\UnpackedData\Gustav\Public\Gustav\RootTemplates\_merged.lsf.lsx")
                If y = 3 Then xml.Load("D:\Modders Multi Tool\UnpackedData\Gustav\Public\GustavDev\RootTemplates\_merged.lsf.lsx")


                raiz = xml.ChildNodes(1).ChildNodes(1).ChildNodes(0).ChildNodes(0)

                ' Genera all parents
                Dim continuar As Boolean = True
                While continuar
                    continuar = False
                    For Each elem As XmlElement In raiz.ChildNodes
                        str = ""
                        Parentid = ""
                        VisualTemplate = ""

                        If Not IsNothing(elem.SelectSingleNode("attribute[@id='Type']")) AndAlso elem.SelectSingleNode("attribute[@id='Type']").Attributes(2).Value = "item" Then
                            id = elem.SelectSingleNode("attribute[@id='MapKey']").Attributes(2).Value
                            Nombre = elem.SelectSingleNode("attribute[@id='Name']").Attributes(2).Value
                            If Not IsNothing(elem.SelectSingleNode("attribute[@id='ParentTemplateId']")) Then
                                Parentid = elem.SelectSingleNode("attribute[@id='ParentTemplateId']").Attributes(2).Value
                            Else
                                Debug.Print("ThenSin tempolate:" + Nombre)
                            End If

                            If Not IsNothing(elem.SelectSingleNode("attribute[@id='VisualTemplate']")) Then
                                VisualTemplate = elem.SelectSingleNode("attribute[@id='VisualTemplate']").Attributes(2).Value
                            End If

                            str = ""
                            comp = New DyeComponent
                            For x = 0 To 14
                                comp.Valores.Add("-1 -1 -1")
                            Next

                            If Parentid = "a09273ba-6549-4cf9-ba47-615a962baf9f" Then
                                comp = New DyeComponent
                                For x = 0 To 14
                                    comp.Valores.Add("1 1 1")
                                Next
                                str = "Ok"
                            End If


                            ' Pone Parent si lo encuentra
                            Dim parentencontrado As Boolean = False
                            For z = 0 To Lista_Armaduras.Count - 1
                                If Lista_Armaduras(z).Id = Parentid Then
                                    parentencontrado = True
                                    For x = 0 To 14
                                        comp.Valores(x) = Lista_Armaduras(z).Components.Valores(x)
                                        str = "Ok"
                                    Next
                                End If
                            Next

                            ' Pone Visual Template si lo encuentra
                            Dim visualencontrado As Boolean = False
                            For z = 0 To Lista_Armaduras.Count - 1
                                If Lista_Armaduras(z).Id = VisualTemplate Then
                                    For x = 0 To 14
                                        visualencontrado = True
                                        comp.Valores(x) = Lista_Armaduras(z).Components.Valores(x)
                                        str = "Ok"
                                    Next
                                End If
                            Next
                            If visualencontrado = False And VisualTemplate <> "" And parentencontrado = True Then
                                If Nombre <> "DEC_HAG_Mask_A" Then
                                    If Nombre <> "DEC_Ketheric_Armor_Mask_Scrap_A" Then
                                        Debug.Print("Sin Visual")
                                    End If
                                End If
                            End If
                            If Not IsNothing(elem.SelectSingleNode("children")) Then
                                If Not IsNothing(elem.SelectSingleNode("children").SelectSingleNode("node[@id='Equipment']")) Then
                                    elem = elem.SelectSingleNode("children").SelectSingleNode("node[@id='Equipment']")
                                    If Not IsNothing(elem.SelectSingleNode("children")) Then
                                        If Not IsNothing(elem.SelectSingleNode("children").SelectSingleNode("node[@id='VisualSet']")) Then
                                            elem = elem.SelectSingleNode("children").SelectSingleNode("node[@id='VisualSet']")
                                            If Not IsNothing(elem.SelectSingleNode("children")) Then
                                                If Not IsNothing(elem.SelectSingleNode("children").SelectSingleNode("node[@id='MaterialOverrides']")) Then
                                                    elem = elem.SelectSingleNode("children").SelectSingleNode("node[@id='MaterialOverrides']")
                                                    If Not IsNothing(elem.SelectSingleNode("children")) Then
                                                        'Pone Preset
                                                        For Each elem4 As XmlNode In elem.SelectSingleNode("children").SelectNodes("node[@id='MaterialPresets']")
                                                            If Not IsNothing(elem4.SelectSingleNode("children")) Then
                                                                Dim que As String = ""
                                                                Dim group As String = ""
                                                                Dim force As String = ""
                                                                For Each elem5 As XmlNode In elem4.SelectSingleNode("children").SelectNodes("node[@id='Object']")
                                                                    group = elem5.SelectSingleNode("attribute[@id='GroupName']").Attributes(2).Value
                                                                    force = elem5.SelectSingleNode("attribute[@id='ForcePresetValues']").Attributes(2).Value
                                                                    que = elem5.SelectSingleNode("attribute[@id='MaterialPresetResource']").Attributes(2).Value
                                                                    If group = "02 Colour" Then
                                                                        For z = 0 To Lista_Armaduras.Count - 1
                                                                            If Lista_Armaduras(z).Id = que Then
                                                                                For w = 0 To 14
                                                                                    comp.Valores(w) = Lista_Armaduras(z).Components.Valores(w) : str = "Ok"
                                                                                Next
                                                                                Exit For
                                                                            End If
                                                                        Next
                                                                    End If
                                                                Next
                                                                If str = "" And que <> "00000000-0000-0000-0000-000000000000" Then
                                                                    Debug.Print(force + ":" + group + ":" + Nombre + "-" + que)
                                                                End If
                                                            End If
                                                        Next

                                                        ' Pone Color Preset
                                                        For Each elem4 As XmlNode In elem.SelectSingleNode("children").SelectNodes("node[@id='ColorPreset']")
                                                            Dim que As String = ""
                                                            Dim group As String = ""
                                                            Dim force As String = ""
                                                            Dim elem5 As XmlNode = elem4
                                                            que = elem5.SelectSingleNode("attribute[@id='MaterialPresetResource']").Attributes(2).Value
                                                            group = elem5.SelectSingleNode("attribute[@id='GroupName']").Attributes(2).Value
                                                            force = elem5.SelectSingleNode("attribute[@id='ForcePresetValues']").Attributes(2).Value
                                                            If group = "02 Colour" Then
                                                                For z = 0 To Lista_Armaduras.Count - 1
                                                                    If Lista_Armaduras(z).Id = que Then
                                                                        For w = 0 To 14
                                                                            comp.Valores(w) = Lista_Armaduras(z).Components.Valores(w) : str = "Ok"
                                                                        Next
                                                                    End If
                                                                Next
                                                            End If

                                                            If str = "" And (que <> "00000000-0000-0000-0000-000000000000" And que <> "") Then
                                                                Debug.Print(force + ":" + group + ":" + Nombre + "-" + que)
                                                            End If
                                                        Next

                                                        'Reemplaza los colores si tiene reemplazo
                                                        For Each elem4 As XmlNode In elem.SelectSingleNode("children").SelectNodes("node[@id='Vector3Parameters']")
                                                            Dim que As String = elem4.SelectSingleNode("attribute[@id='Parameter']").Attributes(2).Value
                                                            For x = 0 To 14
                                                                If Referencias(x) = que Then
                                                                    comp.Valores(x) = elem4.SelectSingleNode("attribute[@id='Value']").Attributes(2).Value
                                                                    str = "Ok"
                                                                End If
                                                            Next
                                                        Next
                                                    End If
                                                End If
                                            End If
                                        End If
                                    End If
                                End If
                            End If

                            If str <> "" Then
                                Dim encontrado As Boolean = False
                                For x = 0 To Lista_Armaduras.Count - 1
                                    If id = Lista_Armaduras(x).Id Then
                                        encontrado = True
                                        For z = 0 To 14
                                            Lista_Armaduras(x).Components.Valores(z) = comp.Valores(z)
                                        Next
                                        Exit For
                                    End If
                                Next
                                If encontrado = False Then
                                    Lista_Armaduras.Add(New ArmorColor With {.Nombre = Nombre, .Source = "Replacement", .Components = comp, .ParentId = Parentid, .Id = id})
                                    continuar = True
                                End If
                            End If
                        End If
                    Next
                End While
            Next



            Filtra_arbol()
            Dim Archivo As IO.StreamWriter
            Archivo = New IO.StreamWriter("C:\Temp\Colores_Armaduras.xml") ' THIS IS THE OUTPUT FILE THEN I USE LATER AS RESOURCE
            serialArms.Serialize(Archivo, Lista_Armaduras.Where(Function(x) x.Totree = True).ToList)

            ' FINAL CHECK: NO DY SHOULD HAVE THIS
            For x = 0 To Lista_Armaduras.Count - 1
                For z = 0 To 14
                    If Lista_Armaduras(x).Components.Valores(z) = "-1 -1 -1" Then
                        Debug.Print("Error:" + Lista_Armaduras(x).Nombre)
                        Throw New Exception
                    End If
                Next
            Next

        Else
            Dim Archivo As IO.StringReader
            Archivo = New IO.StringReader(My.Resources.Colores_Armaduras)
            Lista_Armaduras = serialArms.Deserialize(Archivo)
            Filtra_arbol()
        End If


    End Sub

    Private Sub TreeView1_AfterSelect(sender As Object, e As TreeViewEventArgs) Handles TreeView1.AfterSelect
        Dim sp As Integer
        sp = TreeView1.SelectedNode.Tag
        Dim componente_armadura As DyeComponent

        If sp <> "-1" Then
            componente_armadura = Lista_Armaduras(sp).Components
        Else
            componente_armadura = New DyeComponent
            For x = 0 To 14
                componente_armadura.Valores.Add("1 1 1")
            Next
        End If

        For x = 0 To 14
            Select Case x
                Case 0
                    LabelB1.Text = Referencias(x)
                    PictureBoxB1.BackColor = SRGB0_1(componente_armadura.Valores(x))
                Case 1
                    LabelB2.Text = Referencias(x)
                    PictureBoxB2.BackColor = SRGB0_1(componente_armadura.Valores(x))
                Case 2
                    LabelB3.Text = Referencias(x)
                    PictureBoxB3.BackColor = SRGB0_1(componente_armadura.Valores(x))
                Case 3
                    LabelB4.Text = Referencias(x)
                    PictureBoxB4.BackColor = SRGB0_1(componente_armadura.Valores(x))
                Case 4
                    LabelB5.Text = Referencias(x)
                    PictureBoxB5.BackColor = SRGB0_1(componente_armadura.Valores(x))
                Case 5
                    LabelB6.Text = Referencias(x)
                    PictureBoxB6.BackColor = SRGB0_1(componente_armadura.Valores(x))
                Case 6
                    LabelB7.Text = Referencias(x)
                    PictureBoxB7.BackColor = SRGB0_1(componente_armadura.Valores(x))
                Case 7
                    LabelB8.Text = Referencias(x)
                    PictureBoxB8.BackColor = SRGB0_1(componente_armadura.Valores(x))
                Case 8
                    LabelB9.Text = Referencias(x)
                    PictureBoxB9.BackColor = SRGB0_1(componente_armadura.Valores(x))
                Case 9
                    LabelB10.Text = Referencias(x)
                    PictureBoxB10.BackColor = SRGB0_1(componente_armadura.Valores(x))
                Case 10
                    LabelB11.Text = Referencias(x)
                    PictureBoxB11.BackColor = SRGB0_1(componente_armadura.Valores(x))
                Case 11
                    LabelB12.Text = Referencias(x)
                    PictureBoxB12.BackColor = SRGB0_1(componente_armadura.Valores(x))
                Case 12
                    LabelB13.Text = Referencias(x)
                    PictureBoxB13.BackColor = SRGB0_1(componente_armadura.Valores(x))
                Case 13
                    LabelB14.Text = Referencias(x)
                    PictureBoxB14.BackColor = SRGB0_1(componente_armadura.Valores(x))
                Case 14
                    LabelB15.Text = Referencias(x)
                    PictureBoxB15.BackColor = SRGB0_1(componente_armadura.Valores(x))
            End Select

        Next

    End Sub



    Private Sub Button9_Click_2(sender As Object, e As EventArgs) Handles Button9.Click
        Filtra_arbol()
    End Sub


    Private Sub Filtra_arbol()
        TreeView1.Nodes.Clear()
        ' Llena Tree
        Dim foundnod As TreeNode = Nothing
        If Lista_Armaduras.Where(Function(x) x.Totree = False).Any Then
            foundnod = TreeView1.Nodes.Add("Base Presets")
            foundnod.Tag = "-1"
        End If

        Dim visuals As TreeNode = Nothing
        If Lista_Armaduras.Where(Function(x) x.Totree = False).Any Then
            visuals = TreeView1.Nodes.Add("Visual Templates")
            visuals.Tag = -1
        End If

        Dim notfoundnod As TreeNode
        notfoundnod = TreeView1.Nodes.Add("Armor colors")
        notfoundnod.Tag = "-1"
        notfoundnod.Expand()

        Dim nod As TreeNode
        For x = 0 To Lista_Armaduras.Count - 1
            nod = Nothing
            Dim filter As Boolean = False
            Select Case Lista_Armaduras(x).Source
                Case "CharPreset"
                    If TextBox16.Text = "" OrElse Lista_Armaduras(x).Nombre.ToUpper.Contains(TextBox16.Text.ToUpper) = True Then filter = False Else filter = True
                    If filter = False Then
                        nod = foundnod.Nodes.Add(Lista_Armaduras(x).Nombre)
                        nod.Tag = x
                    End If
                Case "Visual Templates"
                    If TextBox16.Text = "" OrElse Lista_Armaduras(x).Nombre.ToUpper.Contains(TextBox16.Text.ToUpper) = True Then filter = False Else filter = True
                    If filter = False Then
                        nod = visuals.Nodes.Add(Lista_Armaduras(x).Nombre)
                        nod.Tag = x
                    End If
                Case Else
                    If filter = False Then
                        If Lista_Armaduras(x).ParentId = "a09273ba-6549-4cf9-ba47-615a962baf9f" Then
                            nod = notfoundnod.Nodes.Add(Lista_Armaduras(x).Nombre)
                            nod.Tag = x
                            filter = Findallchildren(nod, Lista_Armaduras(x).Id)
                            If filter = True Then
                                nod.Remove()
                            End If
                            Lista_Armaduras(x).Totree = True

                        End If
                    End If

            End Select

        Next


        TreeView1.Sort()
        If IsNothing(TreeView1.SelectedNode) Then TreeView1.SelectedNode = TreeView1.Nodes(0)
        notfoundnod.Expand()
    End Sub

    Private Function Findallchildren(nod As TreeNode, Parentid As String) As Boolean
        Dim nod2 As TreeNode
        Dim filter As Boolean
        Dim oneNoFilter As Boolean = False
        For x = 0 To Lista_Armaduras.Count - 1
            If Lista_Armaduras(x).ParentId = Parentid Then
                nod2 = nod.Nodes.Add(Lista_Armaduras(x).Nombre)
                nod2.Tag = x
                filter = Findallchildren(nod2, Lista_Armaduras(x).Id)
                If filter = False Then oneNoFilter = True
                Lista_Armaduras(x).Totree = True
                If nod2.Nodes.Count = 0 Then
                    If TextBox16.Text = "" OrElse Lista_Armaduras(x).Nombre.ToUpper.Contains(TextBox16.Text.ToUpper) = True Then
                        filter = False
                        oneNoFilter = True
                    Else
                        filter = True
                    End If
                Else
                    If TextBox16.Text = "" OrElse Lista_Armaduras(x).Nombre.ToUpper.Contains(TextBox16.Text.ToUpper) = True Then
                        filter = False
                        oneNoFilter = True
                    Else
                        filter = filter
                    End If
                End If
                If filter = True Then
                    nod2.Remove()
                End If
            End If
        Next
        Return Not oneNoFilter
    End Function

    Private Sub Button10_Click_1(sender As Object, e As EventArgs) Handles Button10.Click
        If IsNothing(TreeView1.SelectedNode) Then Exit Sub
        copylist(0) = SRGB0_1(PictureBoxB1.BackColor.R, PictureBoxB1.BackColor.G, PictureBoxB1.BackColor.B)
        copylist(1) = SRGB0_1(PictureBoxB2.BackColor.R, PictureBoxB2.BackColor.G, PictureBoxB2.BackColor.B)
        copylist(2) = SRGB0_1(PictureBoxB3.BackColor.R, PictureBoxB3.BackColor.G, PictureBoxB3.BackColor.B)
        copylist(3) = SRGB0_1(PictureBoxB4.BackColor.R, PictureBoxB4.BackColor.G, PictureBoxB4.BackColor.B)
        copylist(4) = SRGB0_1(PictureBoxB5.BackColor.R, PictureBoxB5.BackColor.G, PictureBoxB5.BackColor.B)
        copylist(5) = SRGB0_1(PictureBoxB6.BackColor.R, PictureBoxB6.BackColor.G, PictureBoxB6.BackColor.B)
        copylist(6) = SRGB0_1(PictureBoxB7.BackColor.R, PictureBoxB7.BackColor.G, PictureBoxB7.BackColor.B)
        copylist(7) = SRGB0_1(PictureBoxB8.BackColor.R, PictureBoxB8.BackColor.G, PictureBoxB8.BackColor.B)
        copylist(8) = SRGB0_1(PictureBoxB9.BackColor.R, PictureBoxB9.BackColor.G, PictureBoxB9.BackColor.B)
        copylist(9) = SRGB0_1(PictureBoxB10.BackColor.R, PictureBoxB10.BackColor.G, PictureBoxB10.BackColor.B)
        copylist(10) = SRGB0_1(PictureBoxB11.BackColor.R, PictureBoxB11.BackColor.G, PictureBoxB11.BackColor.B)
        copylist(11) = SRGB0_1(PictureBoxB12.BackColor.R, PictureBoxB12.BackColor.G, PictureBoxB12.BackColor.B)
        copylist(12) = SRGB0_1(PictureBoxB13.BackColor.R, PictureBoxB13.BackColor.G, PictureBoxB13.BackColor.B)
        copylist(13) = SRGB0_1(PictureBoxB14.BackColor.R, PictureBoxB14.BackColor.G, PictureBoxB14.BackColor.B)
        copylist(14) = SRGB0_1(PictureBoxB15.BackColor.R, PictureBoxB15.BackColor.G, PictureBoxB15.BackColor.B)
        copylist(15) = "Base: " + TreeView1.SelectedNode.ToString
        copylist(16) = "Base: " + TreeView1.SelectedNode.ToString
        copylist(17) = "Common"
        copylist(18) = "Item_LOOT_Dye_BlackRed_01"
        If ModData.Dyes(ComboBox1.SelectedIndex).Locked = False Then Button5.Enabled = True
    End Sub

    Private Sub Form1_Closing(sender As Object, e As CancelEventArgs) Handles MyBase.Closing
        My.Settings.Save()
    End Sub

    Private Sub Button8_Click_1(sender As Object, e As EventArgs) Handles Button8.Click
        For x = 0 To ListBox1.Items.Count - 1
            If ComboBox4.Items(ComboBox4.SelectedIndex) = ListBox1.Items(x) Then Exit Sub
        Next
        ListBox1.Items.Add(ComboBox4.Items(ComboBox4.SelectedIndex))
        ModData.TreasureTables.Add(ComboBox4.Items(ComboBox4.SelectedIndex))
    End Sub

    Private Sub Button11_Click_1(sender As Object, e As EventArgs) Handles Button11.Click
        If ListBox1.SelectedIndex <> -1 Then
            If ListBox1.Items.Count > 1 Then
                ListBox1.Items.RemoveAt(ListBox1.SelectedIndex)
                ModData.TreasureTables.RemoveAt(ListBox1.SelectedIndex)
            End If
        End If
    End Sub

    Private Sub TextBox17_TextChanged(sender As Object, e As EventArgs) Handles TextBox17.Leave
        ModData.ModFolder = TextBox17.Text
        CheckwriteDirect()
    End Sub

    Private Sub TextBox18_TextChanged(sender As Object, e As EventArgs) Handles TextBox18.Leave
        ModData.ModAuthor = TextBox18.Text
    End Sub


End Class
