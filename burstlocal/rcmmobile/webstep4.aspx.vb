﻿Imports mRCMAPI
Imports System.Xml
Imports System.Data
Imports System.Globalization
Imports Telerik.Web.UI

Partial Class webstep4
  Inherits BasePage
  Dim sErrorMsg As String = ""
  Dim WS_RCMClientAPI As New WS_RCMClientAPI.RCMClientAPI
  Dim m_nodelist As XmlNodeList
  Dim m_node As XmlNode
  Dim sAction As String = ""
  Public oDateTimecl As New DateTimecl
  Dim sPickupDate As DateTime = Now()
  Dim sReturnDate As DateTime = Now()
  Dim sCurrencySymbol As String = ConfigurationManager.AppSettings("CurrencySymbol")
  Dim sRequestType As String = ""
  Dim btFlightRequired As Boolean = False

  Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
    'If Request.QueryString("type") <> "quote" Then
    '  tr_CVV.Visible = True
    'Else
    '  tr_CVV.Visible = False
    'End If
    If Session("ErrorMsg") <> "" Then
      If Session("ErrorMsg") = "RCMReference Key is not valid" Or Session("ErrorMsg") = "RCMReference Key cannot be blank." Then
        'Session("ErrorMsg") = ""
        Response.Redirect("WebStep1.aspx")
      End If
    End If

    datePrimaryDriverDOB.MinDate = DateTime.Today.Date.AddYears(-90)
    datePrimaryDriverDOB.MaxDate = DateTime.Today.Date.AddYears(-16)

    dateLicenseExpiry.MinDate = DateTime.Today.Date.AddMonths(5)
    'LabelTextEnterYourContactDetails
    LabelTextRateSummary.labelLeftText = "Rate Summary"
    LabelTextRateSummary.LabelImage = "~/images/file.png"

    LabelTextEnterYourContactDetails.labelLeftText = "Contact Details"
    LabelTextEnterYourContactDetails.LabelImage = "~/images/file.png"

    LabelTextAlreadyCustomer.labelLeftText = "Already a Customer ?"
    LabelTextAlreadyCustomer.LabelImage = "~/images/file.png"

    If System.Configuration.ConfigurationManager.AppSettings("DisplayOptionalPayment") <> "" And System.Configuration.ConfigurationManager.AppSettings("DisplayOptionalPayment") = "True" Then
      LabelTextPayment.labelLeftText = "Payment Details (Optional)"
    Else
      LabelTextPayment.labelLeftText = "Payment Details"
    End If

    LabelTextPayment.LabelImage = "~/images/file.png"

    If System.Configuration.ConfigurationManager.AppSettings("DisplayOptionalAdditionalContactDetails") <> "" And System.Configuration.ConfigurationManager.AppSettings("DisplayOptionalAdditionalContactDetails") = "True" Then
      LabelTextAdditionalContactDetails.labelLeftText = "Contact Details (Optional)"
    Else
      LabelTextAdditionalContactDetails.labelLeftText = "Contact Details"
    End If

    LabelTextAdditionalContactDetails.LabelImage = "~/images/file.png"
    ' ''''' Credit Card 
    ''ddlCCType
    'rfvalddlCCType.Visible = False
    'Label12.Visible = False

    ''CardNumber
    'rfvaltxtCardNumber.Visible = False

    ''CardName
    'Label15.Visible = False
    'rfvaltxtNameOnCard.Visible = False
    ''Expiry 
    'rfvalddlExpiryDateMonth.Visible = False
    'rfvalddlExpiryDateYear.Visible = False
    ''cvv
    'Label14.Visible = False
    'rfvaltxtCCV.Visible = False
    'Terms
    tr_chkbxTerms.Visible = False
    'CC Message
    tr_CCMessage.Visible = False

    'If System.Configuration.ConfigurationManager.AppSettings("CreditCardMadatory") <> "" And System.Configuration.ConfigurationManager.AppSettings("CreditCardMadatory") = "True" Then
    '  rfvalddlCCType.Visible = True
    '  rfvaltxtCardNumber.Visible = True
    '  rfvaltxtNameOnCard.Visible = True
    '  rfvaltxtCCV.Visible = True
    '  rfvalddlExpiryDateMonth.Visible = True
    '  rfvalddlExpiryDateYear.Visible = True
    'End If
    If Request.QueryString("type") <> "quote" And System.Configuration.ConfigurationManager.AppSettings("AllowTermsAndConditionCheckBox") <> "" And System.Configuration.ConfigurationManager.AppSettings("AllowTermsAndConditionCheckBox") = "True" Then
      tr_chkbxTerms.Visible = True
    End If
    star_dob.Visible = False
    star_License.Visible = False
    star_LicenseIssued.Visible = False
    star_Expiry.Visible = False
    star_BAddress.Visible = False

    'Li

    If (Not String.IsNullOrEmpty(Request.Form(btnExistingCustomer.UniqueID))) = False Then
      sRequestType = Request.QueryString("type")
      If sRequestType = "quote" Then
        btnRequestBooking.Text = "Email Quote"
        tr_address.Visible = False
        tr_city.Visible = False
        tr_postcode.Visible = False
        tr_Country.Visible = False
        tr_RentalSource.Visible = False
        CCHeader.Visible = False
        tr_CCMessage.Visible = False
        'CCHeaderMsg.Visible = False
        'tblPayment.Visible = False
        'tr_CCType.Visible = False
        'tr_CardNumber.Visible = False
        'tr_NameOnCard.Visible = False
        'tr_ExpiryDate.Visible = False
        'txtAddress
        'Label8.Visible = False
        'RequiredFieldValidator10.Visible = False
        'txtNoOfTravelling
        Label7.Visible = False
        RequiredFieldValidator14.Visible = False
        'txtCustomerEmailID
        'Label6.Visible = False
        'RequiredFieldValidator13.Visible = False
        'txtCustomerPhone
        'Label9.Visible = False
        'RequiredFieldValidator7.Visible = False
        'txtPrimaryDriverLicenseNo
        'Label3.Visible = False
        'Label2.Visible = False
        'RequiredFieldValidator8.Visible = False
        'txtLicenseIssuedCountry
        'Label3.Visible = False
        'RequiredFieldValidator9.Visible = False
        'txtCustomerPhone
        Label9.Visible = False
        RequiredFieldValidator7.Visible = False
        RegularExpressionValidator6.Visible = False
        'txtCity
        'Label10.Visible = False
        'RequiredFieldValidator11.Visible = False
        'txtPostalCode
        'Label11.Visible = False
        'RequiredFieldValidator12.Visible = False
        trNoTravelling.Visible = False
        trBDOB.Visible = False
        trLicense.Visible = False
        trLicenseIssued.Visible = False
        trExpiry.Visible = False
        tr_AdditionalDetails.Visible = False
      ElseIf sRequestType = "booking" Then
        btnRequestBooking.Text = "Book Now"
        If System.Configuration.ConfigurationManager.AppSettings("TextDisplayWithCreditCard") <> "" Then
          tr_CCMessage.Visible = True
          lblCCMessage_.Text = System.Configuration.ConfigurationManager.AppSettings("TextDisplayWithCreditCard")
        End If
      End If
      btnRequestBooking.PostBackUrl = "webstep5.aspx?type=" & sRequestType
      Dim postedValues As NameValueCollection = Request.Form
      Dim sInsuranceID As String = Request.Form("ctl00$MainContent$rbtnInsurance") 'don't forget the ToString()!
      'postedValues("InsuranceID")
      'Dim sQuoteRequest As String = postedValues("btnEmailMeQuote")
      Dim iTotalExtraFeesCount As Integer = postedValues("txtTotalExtraFeesCount")
      Dim sKmsFreeID As String = postedValues("ctl00$MainContent$rbtnKmFeeExtra")
      Dim sAreaOfUseID As String = postedValues("ctl00$MainContent$ddlAreaOfUse")
      'Dim ArrayExtraFeeParams As String()
      Dim ArrayExtraFeeParams() As String
      Dim sExtrafeesID_ As String = ""
      Dim sExtrafeesIDs As String = ""
      Dim sExtrafeesQty As String = ""
      Dim sExtrafeesQtys As String = ""
      Dim sExtrafeesParam As String = ""
      Dim k As Integer = 0
      Dim j As Integer = 0
      For i = 1 To iTotalExtraFeesCount
        If postedValues("ExtraFeesID" & i) <> "" Then
          k = k + 1
        End If
      Next
      ReDim ArrayExtraFeeParams(k - 1)

      For i = 1 To iTotalExtraFeesCount
        If postedValues("ExtraFeesID" & i) <> "" Then
          sExtrafeesID_ = postedValues("ExtraFeesID" & i)
          sExtrafeesQty = postedValues("ExtraFeesQty" & i)
          If sExtrafeesQty = 0 Then
            sExtrafeesQty = 1
          End If
          ArrayExtraFeeParams(j) = sExtrafeesID_ & "," & sExtrafeesQty
          j = j + 1
        End If
      Next

      If k = 0 Then
        k = 1
        ReDim ArrayExtraFeeParams(k - 1)
        ArrayExtraFeeParams(j) = ","
      End If

      Dim m_nodelist As XmlNodeList
      Dim m_node As XmlNode

      'FlightRequired Area---
      Dim sResponseFlightRequired As String = ""
      Dim dsFlightRequired As DataSet = WS_RCMClientAPI.requestFlightRequired(Session("sReferenceKey"))
      sResponseFlightRequired = IOcl.GetXMLasStringFromDataSet(dsFlightRequired)
      Dim xFlightRequiredResponse = New XmlDocument()
      ''Load the Xml file
      xFlightRequiredResponse.LoadXml(sResponseFlightRequired)

      'Check the eror Element List
      m_nodelist = xFlightRequiredResponse.SelectNodes("/FlightNoReqdDetails/FlightNoReqd/Required")
      'Loop through the nodes
      For Each m_node In m_nodelist
        'Get the ErrorCode Element Value
        btFlightRequired = m_node.ChildNodes.Item(0).InnerText
      Next
      m_nodelist = Nothing
      m_node = Nothing

      '''' Shifted Optional Contact details here as some customer has Flight no required
      If btFlightRequired = False And System.Configuration.ConfigurationManager.AppSettings("DisplayOptionalAdditionalContactDetails") <> "" And System.Configuration.ConfigurationManager.AppSettings("DisplayOptionalAdditionalContactDetails") = "True" Then
        LabelTextAdditionalContactDetails.labelLeftText = "Contact Details (Optional)"
      Else
        LabelTextAdditionalContactDetails.labelLeftText = "Contact Details (Others)"
      End If
      If btFlightRequired = True And Request.QueryString("type") <> "quote" Then
        trFlight.Style.Add("display", "table-row !important")
        RvalArrivalFlight.Enabled = True
      Else
        trFlight.Style.Add("display", "none")
      End If
      '''' End Shifted Code

      'Insurance Area---
      Dim sResponseInsurance As String = ""
      'Dim sConfirmInsuranceExtraSuccess As String = "False"
      Dim dsInsurance As DataSet = WS_RCMClientAPI.confirmInsuranceFees(sInsuranceID, Session("sReferenceKey"))
      sResponseInsurance = IOcl.GetXMLasStringFromDataSet(dsInsurance)
      Dim xInsuranceDocResponse = New XmlDocument()
      ''Load the Xml file
      xInsuranceDocResponse.LoadXml(sResponseInsurance)

      'Check the eror Element List
      m_nodelist = xInsuranceDocResponse.SelectNodes("/ConfirmInsuranceExtraDetails/ConfirmInsuranceExtra/Error")
      'Loop through the nodes
      For Each m_node In m_nodelist
        'Get the ErrorCode Element Value
        sErrorMsg = m_node.ChildNodes.Item(0).InnerText
      Next
      m_nodelist = Nothing
      m_node = Nothing


      'KmFeeExtra Area---
      If Not IsNothing(sKmsFreeID) Then
        Dim sResponseKmFeeExtra As String = ""
        'Dim sConfirmKmFeeExtraExtraSuccess As String = "False"
        Dim dsKmFeeExtra As DataSet = WS_RCMClientAPI.confirmKmFees(sKmsFreeID, Session("sReferenceKey"))
        sResponseKmFeeExtra = IOcl.GetXMLasStringFromDataSet(dsKmFeeExtra)
        Dim xKmFeeExtraDocResponse = New XmlDocument()
        ''Load the Xml file
        xKmFeeExtraDocResponse.LoadXml(sResponseKmFeeExtra)

        'Check the eror Element List
        m_nodelist = xKmFeeExtraDocResponse.SelectNodes("/ConfirmKmsExtraDetails/ConfirmKmsExtra/Error")
        'Loop through the nodes
        For Each m_node In m_nodelist
          'Get the ErrorCode Element Value
          sErrorMsg = m_node.ChildNodes.Item(0).InnerText
        Next
        m_nodelist = Nothing
        m_node = Nothing
      End If

      'Area of Use ---
      If Not IsNothing(sAreaOfUseID) Then
        Dim sResponseAreaOfUse As String = ""
        Dim dsAreaOfUse As DataSet = WS_RCMClientAPI.confirmAreaofUse(sAreaOfUseID, Session("sReferenceKey"))
        sResponseAreaOfUse = IOcl.GetXMLasStringFromDataSet(dsAreaOfUse)
        Dim xAreaOfUseDocResponse = New XmlDocument()
        ''Load the Xml file
        xAreaOfUseDocResponse.LoadXml(sResponseAreaOfUse)

        'Check the eror Element List
        m_nodelist = xAreaOfUseDocResponse.SelectNodes("/ConfirmAreaofUseDetails/confirmAreaofUse/Error")
        'Loop through the nodes
        For Each m_node In m_nodelist
          'Get the ErrorCode Element Value
          sErrorMsg = m_node.ChildNodes.Item(0).InnerText
        Next
        m_nodelist = Nothing
        m_node = Nothing
      End If
      '----------- Area of Use ----------------------
      'sConfirmInsuranceExtraSuccess = xInsuranceDocResponse.SelectSingleNode("ConfirmInsuranceExtraDetails/ConfirmInsuranceExtra/Success").InnerText
      'Insurance Area Ends ---

      If sErrorMsg = "" Then

        'ExtraFees Area---
        Dim sResponseExtraFees As String = ""
        'Dim sConfirmExtraFees As String = "False"
        'Dim bArrayNothing As Boolean = True
        'For i = 0 To UBound(ArrayExtraFeeParams)
        '  If IsNothing(ArrayExtraFeeParams(i)) Then
        '    bArrayNothing = False
        '  End If
        'Next
        'If Not bArrayNothing = False Then
        If sExtrafeesID_ <> "" Then
          'If ArrayExtraFeeParams(0) <> "," Then
          Dim dsExtraFees As DataSet = WS_RCMClientAPI.confirmExtraFees(Session("sReferenceKey"), ArrayExtraFeeParams)
          sResponseExtraFees = IOcl.GetXMLasStringFromDataSet(dsExtraFees)

          Dim xExtraFeesDocResponse = New XmlDocument()
          ''Load the Xml file
          xExtraFeesDocResponse.LoadXml(sResponseExtraFees)

          'Check the eror Element List
          m_nodelist = xExtraFeesDocResponse.SelectNodes("/ConfirmExtraFeesDetails/ConfirmExtraFees/Error")
          'Loop through the nodes
          For Each m_node In m_nodelist
            'Get the ErrorCode Element Value
            sErrorMsg = m_node.ChildNodes.Item(0).InnerText
          Next
          m_nodelist = Nothing
          m_node = Nothing

          'Check the eror Element List
          m_nodelist = xExtraFeesDocResponse.SelectNodes("/NewDataSet/ErrorDetails/Error")
          'Loop through the nodes
          For Each m_node In m_nodelist
            'Get the ErrorCode Element Value
            sErrorMsg = m_node.ChildNodes.Item(0).InnerText
          Next
          m_nodelist = Nothing
          m_node = Nothing
        End If
        'End If
        ' End If
        'sConfirmExtraFees = xExtraFeesDocResponse.SelectSingleNode("ConfirmExtraFeesDetails/ConfirmExtraFees/Success").InnerText
        'ExtraFees Area Ends ---
        If sErrorMsg = "" Then
          Dim sAction As String = ""
          'sBookingRequest
          'sQuoteRequest
          sAction = IIf(sRequestType = Nothing, "Quote", "booking")
          'Response.Redirect("WebStep5.aspx?action=" & sAction)
        Else
          'tr_lblMsg.Visible = True
          'lblMsg.Visible = True
          Session("ErrorMsg") = sErrorMsg
        End If
      End If
      Session("ErrorMsg") = sErrorMsg
      If Session("ErrorMsg") <> "" Then
        litErrorMessage.Visible = True
        ErrorDiv.Visible = True
        lblErrmsg.Visible = True
        litErrorMessage.Text = Session("ErrorMsg")
        Session("ErrorMsg") = ""
      Else
        'lblCtype.Text = Session("CarType")
        'imgCar.Src = System.Configuration.ConfigurationManager.AppSettings("ImgDBPath") & "/DB/" & System.Configuration.ConfigurationManager.AppSettings("CompanyCode") & "/mobile/" & Session("CarImageName")
        'pnlBooking.Visible = True
        step4header.TitleImage = "~/images/file.png"
        'step4header.labelLeft = "Booking Summary"
        
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'confirmBookingTotal Area---
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Dim sResponseConfirmBookingTotal As String = ""
        Dim dsConfirmBookingTotal As DataSet = WS_RCMClientAPI.confirmBookingTotal("mrinal@rentalcarmanager.com", Session("sReferenceKey"))
        sResponseConfirmBookingTotal = IOcl.GetXMLasStringFromDataSet(dsConfirmBookingTotal)
        Dim xConfirmBookingTotalDocResponse = New XmlDocument()
        ''Load the Xml file
        xConfirmBookingTotalDocResponse.LoadXml(sResponseConfirmBookingTotal)
        '<ConfirmBookingTotalDetails><ErrorDetails><Error>Error:18. Please contact customer care in case you receive this error</Error></ErrorDetails></ConfirmBookingTotalDetails>
        'Check the eror Element List
        m_nodelist = xConfirmBookingTotalDocResponse.SelectNodes("/ConfirmBookingTotalDetails/ErrorDetails/Error")
        'Loop through the nodes
        For Each Me.m_node In m_nodelist
          'Get the ErrorCode Element Value
          sErrorMsg = m_nodelist.Item(0).InnerText
        Next
        m_nodelist = Nothing

        'Check the eror Element List
        m_nodelist = xConfirmBookingTotalDocResponse.SelectNodes("/ConfirmBookingTotalDetails/ConfirmBookingTotal/Error")
        'Loop through the nodes
        For Each Me.m_node In m_nodelist
          'Get the ErrorCode Element Value
          sErrorMsg = m_nodelist.Item(0).InnerText
        Next
        m_nodelist = Nothing
        'Response.ContentType = "text/xml; charset=utf-8"
        'Response.Write(sResponseConfirmBookingTotal)
        'Response.Write(sErrorMsg)
        'Response.End()

        If sErrorMsg = "" Then
          'Added fix as it is throwing error 
          If (Not String.IsNullOrEmpty(xConfirmBookingTotalDocResponse.SelectSingleNode("ConfirmBookingTotalDetails/ConfirmBookingTotal/ReferenceKey").InnerText)) = False Then
            Response.Redirect("WebStep1.aspx")
          End If
          Dim sReferenceKey As String = xConfirmBookingTotalDocResponse.SelectSingleNode("ConfirmBookingTotalDetails/ConfirmBookingTotal/ReferenceKey").InnerText
          Dim sCurrency As String = xConfirmBookingTotalDocResponse.SelectSingleNode("ConfirmBookingTotalDetails/ConfirmBookingTotal/Currency").InnerText
          Dim sHtmlReloctionFees As String = ""
          Dim ReloctionFeesName As String = ""
          Dim ReloctionFeesRate As String = ""
          'Dim bAllowCCV As Boolean = Stringcl.ConvertYesNoToDatabaseBit(xConfirmBookingTotalDocResponse.SelectSingleNode("ConfirmBookingTotalDetails/ConfirmBookingTotal/AllowCCV").InnerText)
          'If bAllowCCV = False Then
          '  tr_CVV.Visible = False
          'End If
          m_nodelist = xConfirmBookingTotalDocResponse.SelectNodes("ConfirmBookingTotalDetails/ConfirmBookingTotal/ReloctionFees/Details")
          If m_nodelist.Count > 0 Then
            'Response.Write("Mrinal")
            ' ''Loop through the nodes
            For Each m_node In m_nodelist
              ReloctionFeesName = m_node.ChildNodes.ItemOf(0).InnerText
              ReloctionFeesRate = m_node.ChildNodes.ItemOf(1).InnerText
              sHtmlReloctionFees = sHtmlReloctionFees & "<tr><td align='left' class='text' colspan='1'><div class='DivNotFixedWidth'>" & ReloctionFeesName & " </div></td><td align='right'  colspan='2' style='vertical-align:top;'> " & sCurrencySymbol & ReloctionFeesRate & "</td></tr>"
            Next
            m_node = Nothing
            m_nodelist = Nothing
          End If
          '''''''''''' End ReloctionFees ''''''''''''''''''''''''''''''''
          'DiscountRate
          'HolidaysExtraFees
          Dim sTotalEstimate As String = xConfirmBookingTotalDocResponse.SelectSingleNode("ConfirmBookingTotalDetails/ConfirmBookingTotal/TotalEstimate").InnerText
          Dim sPickUpLocationName As String = xConfirmBookingTotalDocResponse.SelectSingleNode("ConfirmBookingTotalDetails/ConfirmBookingTotal/PickUpDetails/LocationName").InnerText
          Dim sPickUpLocationDateTime As String = xConfirmBookingTotalDocResponse.SelectSingleNode("ConfirmBookingTotalDetails/ConfirmBookingTotal/PickUpDetails/Date").InnerText
          Dim sDropOffLocationDateTime As String = xConfirmBookingTotalDocResponse.SelectSingleNode("ConfirmBookingTotalDetails/ConfirmBookingTotal/DropOffDetails/Date").InnerText
          Dim sDropOffLocationName As String = xConfirmBookingTotalDocResponse.SelectSingleNode("ConfirmBookingTotalDetails/ConfirmBookingTotal/DropOffDetails/LocationName").InnerText

          lblPDateTime.Text = sPickUpLocationDateTime
          lblPLoc.Text = sPickUpLocationName ' & "(" & BrandName & ")"
          lblDLoc.Text = sDropOffLocationName
          lblDDateTime.Text = sDropOffLocationDateTime
          'lblTotalEstimateValue.Text = sTotalEstimate & "(" & sCurrency & ")"

          Dim sVehicledetailsType As String = xConfirmBookingTotalDocResponse.SelectSingleNode("ConfirmBookingTotalDetails/ConfirmBookingTotal/Vehicledetails/Type").InnerText
          Dim sVehicledetailsDescription As String = xConfirmBookingTotalDocResponse.SelectSingleNode("ConfirmBookingTotalDetails/ConfirmBookingTotal/Vehicledetails/Description").InnerText
          Dim sVehicledetailsImageName As String = xConfirmBookingTotalDocResponse.SelectSingleNode("ConfirmBookingTotalDetails/ConfirmBookingTotal/Vehicledetails/ImageName").InnerText
          Dim sVehicledetailsImagePath As String = xConfirmBookingTotalDocResponse.SelectSingleNode("ConfirmBookingTotalDetails/ConfirmBookingTotal/Vehicledetails/ImagePath").InnerText

          Dim sVehicledetailsImageNameMobile As String = xConfirmBookingTotalDocResponse.SelectSingleNode("ConfirmBookingTotalDetails/ConfirmBookingTotal/Vehicledetails/ImageNameMobile").InnerText
          Dim sVehicledetailsImagePathMobile As String = xConfirmBookingTotalDocResponse.SelectSingleNode("ConfirmBookingTotalDetails/ConfirmBookingTotal/Vehicledetails/ImagePathMobile").InnerText

          lblVehicle.Text = sVehicledetailsType
          imgCar.ImageUrl = sVehicledetailsImagePathMobile
          'Dim sHtmlDisplayVehicleDetails As String = ""
          'sHtmlDisplayVehicleDetails = "<tr><th colspan='2' align='left'>&nbsp;<u>Vehicle Details</u></th></tr>"
          'sHtmlDisplayVehicleDetails = sHtmlDisplayVehicleDetails & "<tr>"
          'sHtmlDisplayVehicleDetails = sHtmlDisplayVehicleDetails & "<td>&nbsp;" & sVehicledetailsType & "</td>"
          'sHtmlDisplayVehicleDetails = sHtmlDisplayVehicleDetails & "<td rowspan='2' align='right'>&nbsp;<img src='" & sVehicledetailsImagePath & "' width='150' /></td>"
          'sHtmlDisplayVehicleDetails = sHtmlDisplayVehicleDetails & "</tr>"
          'sHtmlDisplayVehicleDetails = sHtmlDisplayVehicleDetails & "<tr>"
          'sHtmlDisplayVehicleDetails = sHtmlDisplayVehicleDetails & "<td>&nbsp;" & sVehicledetailsDescription & "</td>"
          'sHtmlDisplayVehicleDetails = sHtmlDisplayVehicleDetails & "</tr>"
          'ltrDisplayVehicleDetails_.Text = "<table BORDER='0' bgcolor='#FFFFFF' cellPadding=2 width='100%'>" & sHtmlDisplayVehicleDetails & "</table>"

          '' Tax details ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
          Dim m_nodeTaxDetailslist As XmlNodeList
          Dim m_nodeTaxDetails As XmlNode

          Dim TaxDetailsName As String = ""
          Dim TaxDetailsRate As String = ""
          Dim TaxDetailsInclusive As String = ""
          'Dim iCount As Integer = 0
          m_nodeTaxDetailslist = xConfirmBookingTotalDocResponse.SelectNodes("/ConfirmBookingTotalDetails/ConfirmBookingTotal/TaxDetails")

          Dim sHtmlTaxDetails As String = ""
          TaxDetailsName = ""
          TaxDetailsRate = ""
          TaxDetailsInclusive = ""

          For Each m_nodeTaxDetails In m_nodeTaxDetailslist
            If m_nodeTaxDetails.HasChildNodes Then
              For i = 0 To m_nodeTaxDetails.ChildNodes.Count - 1
                If m_nodeTaxDetails.ChildNodes.ItemOf(i).HasChildNodes Then
                  TaxDetailsName = m_nodeTaxDetails.ChildNodes.ItemOf(i).ChildNodes(0).InnerText
                  TaxDetailsRate = m_nodeTaxDetails.ChildNodes.ItemOf(i).ChildNodes(1).InnerText
                  TaxDetailsInclusive = m_nodeTaxDetails.ChildNodes.ItemOf(i).ChildNodes(2).InnerText
                  If TaxDetailsInclusive = "Yes" Then
                    TaxDetailsInclusive = "Inc."
                  Else
                    TaxDetailsInclusive = "Not Inc."
                  End If
                  If TaxDetailsRate <> "" And TaxDetailsRate > 0 Then
                    sHtmlTaxDetails = sHtmlTaxDetails & "<tr>"
                    sHtmlTaxDetails = sHtmlTaxDetails & "<td align='right' class='' colspan='3'>&nbsp;(" & TaxDetailsInclusive & "&nbsp;" & TaxDetailsName
                    sHtmlTaxDetails = sHtmlTaxDetails & " of " & sCurrencySymbol & TaxDetailsRate & " " & ")</td>"
                    sHtmlTaxDetails = sHtmlTaxDetails & "</tr>"
                  End If
                End If
                'i = i + 1
              Next

            End If
            'iCount = iCount + 1
          Next
          '' Tax Details end ''''''''''''''''''''''''''


          ''''''''''''''''''''
          ''''' Season '''''''
          ''''''''''''''''''''
          '<SeasonName>SEASON 2013 </SeasonName><Days>15</Days><Hours>0</Hours><Rate>4.00</Rate><Discount>0</Discount><StandardRate>4.00</StandardRate><Cost>60.00</Cost>
          Dim sHtmlEachSeason As String = ""
          Dim m_nodeSeason As XmlNode
          Dim sSeasonName As String = ""
          Dim sDiscount As String = ""
          Dim sNoOfDaysEachSeason As String = ""
          Dim sNoOfHoursEachSeason As String = ""
          Dim sRateEachSeason As String = ""
          Dim sDiscountRateEachSeason As String = ""
          Dim sDiscountTypeRateEachSeason As String = ""
          Dim sStandardRate As String = ""
          Dim sCostEachSeason As String = ""
          '''''
          'Get the Element Value
          ''Loop through the nodes
          'Dim iSeasonCount As Integer = 0
          Dim iHoursSeasonColspan As Integer = 0
          For i = 0 To xConfirmBookingTotalDocResponse.GetElementsByTagName("EachSeason").Count - 1
            '<SeasonName>cheap season</SeasonName><Days>15</Days><Hours>0</Hours><Rate>85.50</Rate><StandardRate>95.00</StandardRate><Cost>1282.50</Cost>
            '<SeasonName>SEASON 2013 </SeasonName><Days>15</Days><Hours>0</Hours><Rate>4.00</Rate><Discount>0</Discount><StandardRate>4.00</StandardRate><Cost>60.00</Cost>
            '<SeasonName>cheap season</SeasonName><Days>15</Days><Hours>0</Hours><Rate>58.50</Rate><Discount>10Percentage</Discount>
            '<DiscountType>Percentage</DiscountType><StandardRate>65.00</StandardRate><Cost>877.50</Cost>
            m_nodeSeason = xConfirmBookingTotalDocResponse.GetElementsByTagName("EachSeason")(i)
            'iSeasonCount = iSeasonCount + 1
            sSeasonName = ""
            sNoOfDaysEachSeason = ""
            sRateEachSeason = ""
            sSeasonName = m_nodeSeason.SelectSingleNode("SeasonName").InnerText
            sNoOfDaysEachSeason = m_nodeSeason.SelectSingleNode("Days").InnerText
            sNoOfHoursEachSeason = m_nodeSeason.SelectSingleNode("Hours").InnerText
            sRateEachSeason = m_nodeSeason.SelectSingleNode("Rate").InnerText
            sDiscountRateEachSeason = m_nodeSeason.SelectSingleNode("Discount").InnerText
            sDiscountTypeRateEachSeason = m_nodeSeason.SelectSingleNode("DiscountType").InnerText
            sStandardRate = m_nodeSeason.SelectSingleNode("StandardRate").InnerText
            sCostEachSeason = m_nodeSeason.SelectSingleNode("Cost").InnerText
            If sDiscountTypeRateEachSeason = "Percentage" Then
              sDiscountTypeRateEachSeason = "%"
            ElseIf sDiscountTypeRateEachSeason = "Dollars" Then
              sDiscountTypeRateEachSeason = sCurrencySymbol
            End If
            If sNoOfHoursEachSeason = "" Then sNoOfHoursEachSeason = "0"
            'sHtmlEachSeason = sHtmlEachSeason & "<tr><td align='left' class='text' colspan='1' style='padding-bottom:3px;'>Season#" & iSeasonCount & " : " & sSeasonName & "</td></tr><tr><td> Rate : "
            'sHtmlEachSeason = sHtmlEachSeason & "<tr><td align='left' class='text' colspan=''>" & sSeasonName & "</td>"
            'sHtmlEachSeason = sHtmlEachSeason & "<td align='left' class='text' colspan=''>" & sNoOfDaysEachSeason & " Days &amp; " & sNoOfHoursEachSeason & " hours @ " & sCurrencySymbol & " " & sRateEachSeason & " (per day)</td>"
            'sHtmlEachSeason = sHtmlEachSeason & "<td align='right' class='text' colspan=''>   " & sCostEachSeason & "(" & sCurrencySymbol & ")</td></tr>"

            sHtmlEachSeason = sHtmlEachSeason & "<tr><td class='text' style='vertical-align:top;' colspan='2'>" & sNoOfDaysEachSeason & " Days @ " & sCurrencySymbol & sRateEachSeason
            If sDiscountRateEachSeason <> "" And sDiscountRateEachSeason <> "0" Then
              If sDiscountTypeRateEachSeason = sCurrencySymbol Then
                sHtmlEachSeason = sHtmlEachSeason & "&nbsp;(" & sCurrencySymbol & sStandardRate & " less " & sDiscountTypeRateEachSeason & sDiscountRateEachSeason & " off)"
              ElseIf sDiscountTypeRateEachSeason = "%" Then
                sHtmlEachSeason = sHtmlEachSeason & "&nbsp;(" & sCurrencySymbol & sStandardRate & " less " & sDiscountRateEachSeason & " " & sDiscountTypeRateEachSeason & " off)"
              End If
            End If
            sHtmlEachSeason = sHtmlEachSeason & "</td>"
            If sNoOfHoursEachSeason <> 0 Then
              sHtmlEachSeason = sHtmlEachSeason & "<td class='text' style='vertical-align:top;'>" & sNoOfDaysEachSeason & " Days &amp; " & sNoOfHoursEachSeason & " hours @ " & sCurrencySymbol & sRateEachSeason & "</td>"
            Else
              iHoursSeasonColspan = "2"
            End If
            'colspan='" & iHoursSeasonColspan & "' 
            sHtmlEachSeason = sHtmlEachSeason & "<td class='text' align='right' valign='bottom'>&nbsp;" & sCurrencySymbol & sCostEachSeason & "</td></tr>"
            m_nodeSeason = Nothing
          Next
          ''''''''''''''''''''''''
          ''''' End Season '''''''
          ''''''''''''''''''''''''

          '''''''''''' Area of Use ''''''''''''''''''''''''''''''''
          Dim sHtmlAreaofUsed As String = ""
          Dim AreaofUsedName As String = ""
          Dim AreaofUsedType As String = ""
          Dim AreaofUsedRate As String = ""
          Dim sAreaofUsedQty As String = ""
          Dim sAreaofUsedCost As String = ""
          Dim sAreaofUsedDescription As String = ""

          m_nodelist = xConfirmBookingTotalDocResponse.SelectNodes("ConfirmBookingTotalDetails/ConfirmBookingTotal/AreaofUse/Details")
          If m_nodelist.Count > 0 Then
            ' ''Loop through the nodes
            For Each m_node In m_nodelist
              AreaofUsedName = m_node.ChildNodes.ItemOf(0).InnerText
              AreaofUsedType = m_node.ChildNodes.ItemOf(1).InnerText
              AreaofUsedRate = m_node.ChildNodes.ItemOf(2).InnerText
              sAreaofUsedQty = m_node.ChildNodes.ItemOf(3).InnerText
              sAreaofUsedCost = m_node.ChildNodes.ItemOf(4).InnerText
              sAreaofUsedDescription = m_node.ChildNodes.ItemOf(5).InnerText
              'sHtmlAreaofUsed = sHtmlAreaofUsed & "<tr><td align='left' class='text' colspan=''>Area of Use</td><td align='left' class='text' colspan='2'>" & AreaofUsedName & "</td></tr>"
              sHtmlAreaofUsed = sHtmlAreaofUsed & "<tr><td align='left' class='text' colspan=''>Area of Use</td><td style='vertical-align:top;'><div class='DivNotFixedWidth'>" & AreaofUsedName & "</div></td></tr>"
            Next
            m_node = Nothing
            m_nodelist = Nothing
          Else
            'tr_AreaOfUse.Visible = False
          End If
          '''''''''''' End Area of Use ''''''''''''''''''''''''''''''''

          ''''''''''''''''''''''' KmFeeDetails  ''''''''''''''''''''''''''''''''''''''
          '<KmFeeDetails><Name>100 Kms/Day</Name><Rate>20.00</Rate>
          '<Description>Daily rate @20.0000, 100 Kms free per day, additional per Kms 0.1500 , max charge 100.00 per day</Description><Qty>1</Qty><Cost>200.00</Cost></KmFeeDetails>
          '<KmFeeDetails><Name>200 Kms/Day</Name><Rate>0.00</Rate><Description>200 Kms free per day, additional per Kms 0.2500 , max charge 15.00 per day</Description><Qty>1</Qty><Cost>0.00</Cost><FreeKms>200</FreeKms><AdditionalKmRate>0.25</AdditionalKmRate><MaxCost>15.00</MaxCost></KmFeeDetails>
          Dim sFreeKmsCount As String = xConfirmBookingTotalDocResponse.SelectNodes("ConfirmBookingTotalDetails/ConfirmBookingTotal/KmFeeDetails").Count
          Dim sHtmlFreeKms As String = ""
          If sFreeKmsCount <> "" And sFreeKmsCount <> "0" Then
            Dim sFreeKms As String = xConfirmBookingTotalDocResponse.SelectSingleNode("/ConfirmBookingTotalDetails/ConfirmBookingTotal/KmFeeDetails/Name").InnerText
            Dim sFreeKmsRate As String = xConfirmBookingTotalDocResponse.SelectSingleNode("/ConfirmBookingTotalDetails/ConfirmBookingTotal/KmFeeDetails/Rate").InnerText
            Dim sFreeKmsDescription As String = xConfirmBookingTotalDocResponse.SelectSingleNode("/ConfirmBookingTotalDetails/ConfirmBookingTotal/KmFeeDetails/Description").InnerText
            Dim sFreeKmsCost As String = xConfirmBookingTotalDocResponse.SelectSingleNode("/ConfirmBookingTotalDetails/ConfirmBookingTotal/KmFeeDetails/Cost").InnerText

            'Dim sFreeKmsAdditionalKmRate As String = xConfirmBookingTotalDocResponse.SelectSingleNode("/ConfirmBookingTotalDetails/ConfirmBookingTotal/KmFeeDetails/AdditionalKmRate").InnerText

            'sFreeKmsDescription
            sHtmlFreeKms = sHtmlFreeKms & "<tr><td style='vertical-align:top;' colspan='3' ><div class='DivNotFixedWidth'>" & sFreeKmsDescription & "</div></td></tr>"
            'sHtmlFreeKms = sHtmlFreeKms & "<tr><td style='vertical-align:top;' colspan='2' ><div class='DivNotFixedWidth'>" & sFreeKms & "</div></td><td align='right'  colspan='1' style='vertical-align:top;'>" & sCurrencySymbol & sFreeKmsCost & "</td></tr>"
            '"<tr><td style='vertical-align:top;'><div class='DivNotFixedWidth'>" & sSeasonName & " </div></td><td style='vertical-align:top;'><div>&nbsp;"
          End If
          '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
          ''''''''''''''''''''''' KmFeeDetails  ''''''''''''''''''''''''''''''''''''''
          Dim sInsuranceFeeCount As String = xConfirmBookingTotalDocResponse.SelectNodes("ConfirmBookingTotalDetails/ConfirmBookingTotal/InsuranceFeeDetails").Count
          Dim sHtmlInsuranceFee As String = ""
          If sInsuranceFeeCount <> "" And sInsuranceFeeCount > 0 Then
            Dim sInsuranceFeeName As String = xConfirmBookingTotalDocResponse.SelectSingleNode("/ConfirmBookingTotalDetails/ConfirmBookingTotal/InsuranceFeeDetails/Name").InnerText
            Dim sInsuranceFeeType As String = xConfirmBookingTotalDocResponse.SelectSingleNode("/ConfirmBookingTotalDetails/ConfirmBookingTotal/InsuranceFeeDetails/Type").InnerText
            Dim sInsuranceFeeRate As String = xConfirmBookingTotalDocResponse.SelectSingleNode("/ConfirmBookingTotalDetails/ConfirmBookingTotal/InsuranceFeeDetails/Rate").InnerText
            Dim sInsuranceFeeCost As String = xConfirmBookingTotalDocResponse.SelectSingleNode("/ConfirmBookingTotalDetails/ConfirmBookingTotal/InsuranceFeeDetails/Cost").InnerText

            If sInsuranceFeeType = "Percentage" Then
              sInsuranceFeeType = "%"
            ElseIf sInsuranceFeeType = "Daily" Then
              sInsuranceFeeType = "Per Day"
            End If

            'sHtmlInsuranceFee = sHtmlInsuranceFee & "<tr><td align='left' class='text' colspan='1'>" & sInsuranceFeeName & " (" & sInsuranceFeeType & ") @ " & sInsuranceFeeRate & "(" & sCurrencySymbol & ")  = " & sInsuranceFeeCost & "(" & sCurrencySymbol & ")</td></tr>"
            sHtmlInsuranceFee = sHtmlInsuranceFee & "<tr><td align='left' class='text' colspan='1'><div class='DivNotFixedWidth'>" & sInsuranceFeeName & "</div></td><td align='left'  colspan='1' style='vertical-align:top;'> " & sCurrencySymbol & sInsuranceFeeRate & " " & sInsuranceFeeType & "</td><td align='right'  colspan='1' style='vertical-align:top;'> " & sCurrencySymbol & sInsuranceFeeCost & "</td></tr>"
          End If
          '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
          'Get Vechicle MandatoryFees Element List
          Dim sHtmlMandatoryFees As String = ""
          m_nodelist = xConfirmBookingTotalDocResponse.SelectNodes("/ConfirmBookingTotalDetails/ConfirmBookingTotal/MandatoryFees/Details")
          'Loop through the nodes
          '<Name>Airport Fee</Name><Rate>5.00</Rate><Type>Fixed</Type><Cost>5.00</Cost>
          Dim sMandatoryFeesName As String = ""
          Dim sMandatoryFeesType As String = ""
          Dim sMandatoryFeesRate As String = ""
          Dim sMandatoryFeesCost As String = ""
          For Each m_node In m_nodelist
            'Get the MandatoryFees Name Element Value
            sMandatoryFeesName = m_node.SelectSingleNode("Name").InnerText
            'Get the CategoryType Name Element Value
            sMandatoryFeesType = m_node.SelectSingleNode("Type").InnerText
            sMandatoryFeesRate = m_node.SelectSingleNode("Rate").InnerText
            sMandatoryFeesCost = m_node.SelectSingleNode("Cost").InnerText

            'sHtmlMandatoryFees = sHtmlMandatoryFees & "<tr><td align='left' class='text' colspan=''>" & sMandatoryFeesName & " @ " & sMandatoryFeesRate & "(" & sCurrencySymbol & ") = " & sMandatoryFeesCost & "(" & sCurrencySymbol & ")</td></tr>"
            sHtmlMandatoryFees = sHtmlMandatoryFees & "<tr><td align='left' class='text' colspan='1'><div class='DivNotFixedWidth'>" & sMandatoryFeesName & " </div></td>"
            If sMandatoryFeesType = "Percentage" Then
              sHtmlMandatoryFees = sHtmlMandatoryFees & "<td align='left'  colspan='1' style='vertical-align:top;'> " & sMandatoryFeesRate & "%</td>"
            Else
              sHtmlMandatoryFees = sHtmlMandatoryFees & "<td align='left'  colspan='1' style='vertical-align:top;'> " & sCurrencySymbol & sMandatoryFeesRate & "</td>"
            End If
            sHtmlMandatoryFees = sHtmlMandatoryFees & "<td align='right'  colspan='1' style='vertical-align:top;'> " & sCurrencySymbol & sMandatoryFeesCost & "</td></tr>"
          Next
          m_node = Nothing
          m_nodelist = Nothing
          'ltrMandatoryFees_.Text = sHtmlMandatoryFees
          '<MerchantFeeDetails><Details><Name>Credit card charge</Name><Type>Percentage</Type><Cost>7.95</Cost></Details></MerchantFeeDetails>
          m_nodelist = xConfirmBookingTotalDocResponse.SelectNodes("/ConfirmBookingTotalDetails/ConfirmBookingTotal/MandatoryFees/MerchantFeeDetails/Details")
          'Loop through the nodes
          '<Name>After hours Airport Terminal carpark return and I-site fee</Name><Type>Fixed</Type><Rate>50.00</Rate><Cost>50.00</Cost>
          Dim sMerchantFeeDetailsName As String = ""
          Dim sMerchantFeeDetailsType As String = ""
          Dim sMerchantFeeDetailsCost As String = ""
          For Each m_node In m_nodelist
            sMerchantFeeDetailsName = m_node.ChildNodes.Item(0).InnerText
            sMerchantFeeDetailsType = m_node.ChildNodes.Item(1).InnerText
            sMerchantFeeDetailsCost = m_node.ChildNodes.Item(2).InnerText
            'sHtmlMandatoryFees = sHtmlMandatoryFees & "<tr><td align='left' class='text' colspan=''>" & sMandatoryFeesName & " @ " & sMandatoryFeesRate & "(" & sCurrencySymbol & ") = " & sMandatoryFeesCost & "(" & sCurrencySymbol & ")</td></tr>"
            sHtmlMandatoryFees = sHtmlMandatoryFees & "<tr><td align='left' class='text' colspan='1'><div class='DivNotFixedWidth'>" & sMerchantFeeDetailsName & "</div></td><td align='right'  colspan='3' style='vertical-align:top;'> " & sCurrencySymbol & sMerchantFeeDetailsCost & "</td></tr>"
          Next
          m_node = Nothing
          m_nodelist = Nothing

          'End Vechicle MandatoryFees Element List

          '-- Holidays
          '  <HolidaysExtraFees><Type>Fixed</Type><ChargeQty /><Cost>45.00</Cost></HolidaysExtraFees>
          Dim sHolidaysExtraFees As String = ""
          m_nodelist = xConfirmBookingTotalDocResponse.SelectNodes("/ConfirmBookingTotalDetails/ConfirmBookingTotal/HolidaysExtraFees")
          If m_nodelist.Item(0).HasChildNodes = True Then
            Dim sHolidaysExtraFeesName As String = ""
            Dim sHolidaysExtraFeesType As String = ""
            Dim sHolidaysExtraFeesCost As String = ""
            Dim sHolidaysExtraFeesChargeQty As String = ""
            For Each m_node In m_nodelist
              sHolidaysExtraFeesName = "Holiday Charges" 'm_node.ChildNodes.Item(0).InnerText
              sHolidaysExtraFeesType = m_node.Item("Type").InnerText
              sHolidaysExtraFeesCost = m_node.Item("Cost").InnerText
              sHolidaysExtraFeesChargeQty = m_node.Item("ChargeQty").InnerText
              sHolidaysExtraFees = sHolidaysExtraFees & "<tr><td align='left' class='text' colspan='1'><div class='DivNotFixedWidth'>" & sHolidaysExtraFeesName & "&nbsp;</div></td>"
              sHolidaysExtraFees = sHolidaysExtraFees & "<td align='left'  colspan='0' style='vertical-align:top;'> " & sHolidaysExtraFeesType & "</td>"
              sHolidaysExtraFees = sHolidaysExtraFees & "<td align='right'  colspan='2' style='vertical-align:top;'> " & sCurrencySymbol & sHolidaysExtraFeesCost & "</td></tr>"
            Next
          End If
          m_node = Nothing
          m_nodelist = Nothing
          ''''''''''''''''''''''' FreeDayDetails  ''''''''''''''''''''''''''''''''''''''
          Dim sHtmlFreeDayDetails As String = ""
          If xConfirmBookingTotalDocResponse.GetElementsByTagName("FreeDayDetails").Item(0).HasChildNodes Then
            Dim sFreeDayDetailsCount As String = xConfirmBookingTotalDocResponse.SelectNodes("ConfirmBookingTotalDetails/ConfirmBookingTotal/FreeDayDetails").Count
            If sFreeDayDetailsCount <> "" Then
              Dim sFreeDayDetailsDays As String = xConfirmBookingTotalDocResponse.SelectSingleNode("/ConfirmBookingTotalDetails/ConfirmBookingTotal/FreeDayDetails/Days").InnerText
              Dim sFreeDayDetailsCost As String = xConfirmBookingTotalDocResponse.SelectSingleNode("/ConfirmBookingTotalDetails/ConfirmBookingTotal/FreeDayDetails/Rate").InnerText
              sHtmlFreeDayDetails = "<tr><td align='left' class='text' colspan='2'>" & sFreeDayDetailsDays & " Free Day </td><td align='right' class='text' colspan='2'>" & sFreeDayDetailsCost & "(" & sCurrencySymbol & ")</td></tr>"
              'sHtmlFreeDayDetails = "<tr><td align='left' class='text' colspan=''>" & sMandatoryFeesName & " </td><td align='left' class='text' colspan=''>@" & sMandatoryFeesRate & "(" & sCurrencySymbol & ") </td><td class='text'>  " & sMandatoryFeesCost & "(" & sCurrencySymbol & ")</td></tr>"
              'ltrDisplayVehicleFreeDayDetails_.Text = "<table BORDER='0' bgcolor='#FFFFFF' cellPadding=2 width='100%'>" & sHtmlFreeDayDetails & "</table>"
            End If
          End If
          ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

          'ltrAllSeasonRates_.Text = "<table BORDER='1' bgcolor='#FFFFFF' cellPadding=2 width='100%'>" & sHtmlEachSeason & sHtmlFreeKms & sHtmlAreaofUsed & sHtmlInsuranceFee & sHtmlMandatoryFees & sHtmlFreeDayDetails & "</table>"

          ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
          ' For other fees
          ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
          Dim sHtmlAllFeeDetails As String = ""
          Dim AllFeeDetailsName As String = ""
          Dim AllFeeDetailsType As String = ""
          Dim AllFeeDetailsRate As String = ""
          Dim AllFeeDetailsQty As String = ""
          Dim AllFeeDetailsCost As String = ""
          Dim AllFeeDetailsStampDuty As String = ""
          'Dim AllFeeDetailsGST As String = ""
          m_nodelist = xConfirmBookingTotalDocResponse.SelectNodes("/ConfirmBookingTotalDetails/ConfirmBookingTotal/ExtraFees/Details")
          'sHtmlAllFeeDetails = "<tr><th colspan='6' align='left'>&nbsp;<u>Fee Details</u></th></tr>"
          'sHtmlAllFeeDetails = sHtmlAllFeeDetails & "<tr><td class='text'>&nbsp;<u>Name</u></td><td class='text'>&nbsp;<u>Type</u></td><td class='text' align='right'>&nbsp;<u>Price</u></td><td class='text' align=''>&nbsp;<u>Quantity</u></td><td class='text'>&nbsp;<u>Description</u></td><td class='text' align='right'>&nbsp;<u>Cost</u></td></tr>"
          AllFeeDetailsName = ""
          AllFeeDetailsType = ""
          AllFeeDetailsRate = ""
          AllFeeDetailsQty = ""
          AllFeeDetailsStampDuty = ""
          AllFeeDetailsCost = ""
          'AllFeeDetailsGST = ""
          '<Name>BABY SEAT</Name><Type>Fixed</Type><Rate>20.0000</Rate><Qty>1</Qty><Cost>20.00</Cost><StampDuty />
          '<Name>GPS</Name><Rate>10.0000</Rate><Type>Daily</Type><Qty>1</Qty><Cost>210.00</Cost><StampDuty />
          For Each m_node In m_nodelist
            If m_node.HasChildNodes Then
              AllFeeDetailsName = m_node.SelectSingleNode("Name").InnerText
              AllFeeDetailsType = m_node.SelectSingleNode("Type").InnerText
              If InStr(AllFeeDetailsType, "Percentage") > 0 Then
                AllFeeDetailsType = "%"
              ElseIf AllFeeDetailsType = "Daily" Then
                AllFeeDetailsType = "Per Day"
              End If
              AllFeeDetailsRate = m_node.SelectSingleNode("Rate").InnerText
              AllFeeDetailsQty = m_node.SelectSingleNode("Qty").InnerText
              AllFeeDetailsCost = m_node.SelectSingleNode("Cost").InnerText
              AllFeeDetailsStampDuty = m_node.SelectSingleNode("StampDuty").InnerText
              'AllFeeDetailsGST = m_node.ChildNodes.ItemOf(6).InnerText

              'sHtmlMandatoryFees = sHtmlMandatoryFees & "<tr><td align='left' class='text' colspan='1'><div class='DivNotFixedWidth'>" & sMandatoryFeesName & " </div></td><td align='right'  colspan='2' style='vertical-align:top;'> " & sCurrencySymbol & sMandatoryFeesRate & "</td></tr>"

              sHtmlAllFeeDetails = sHtmlAllFeeDetails & "<tr>"
              sHtmlAllFeeDetails = sHtmlAllFeeDetails & "<td align='left' class='text' colspan=''><div class='DivNotFixedWidth'>" & AllFeeDetailsName & "</div></td>"
              sHtmlAllFeeDetails = sHtmlAllFeeDetails & "<td>" & IIf(InStr(AllFeeDetailsType, "%") > 0, "", sCurrencySymbol) & FormatNumber(AllFeeDetailsRate, 2) & " " & AllFeeDetailsType & "</td>"
              sHtmlAllFeeDetails = sHtmlAllFeeDetails & "<td align='right'>" & sCurrencySymbol & AllFeeDetailsCost & "</td>"
              sHtmlAllFeeDetails = sHtmlAllFeeDetails & "</tr>"
            End If
          Next
          'Dim sRow1 As String = sHtmlEachSeason & sHtmlFreeKms & sHtmlAreaofUsed & sHtmlInsuranceFee & sHtmlMandatoryFees & sHtmlReloctionFees
          'Dim sFeehtml As String = ""
          'sHtmlAllFeeDetails 
          'ltrAllFeeDetails_.Text
          'sFeehtml = "<table BORDER='0' bgcolor='#FFFFFF' cellspacing='1' cellPadding='2' width='100%'>"
          'sFeehtml = sFeehtml & sRow1
          'sFeehtml = sFeehtml & sHtmlAllFeeDetails
          'sFeehtml = sFeehtml & sHtmlFreeDayDetails
          'sFeehtml = sFeehtml & "<tr><td align='right' class='text' colspan='1' valign='top'>&nbsp;Total Estimate of Charges : &nbsp;" & sTotalEstimate & "(" & sCurrencySymbol & ")</td></tr>"
          'sFeehtml = sFeehtml & sHtmlTaxDetails
          'sFeehtml = sFeehtml & "</table>"
          'ltrAllFeeDetails_.Text = sFeehtml
          step4header.labelRight = sCurrency & sCurrencySymbol & " " & FormatNumber(sTotalEstimate, 2)
          ltrAllFeeDetails_.Text = "<table border='0' width='100%'>"
          ltrAllFeeDetails_.Text = ltrAllFeeDetails_.Text & sHtmlEachSeason & sHtmlFreeKms & sHtmlAreaofUsed & sHtmlInsuranceFee & sHtmlMandatoryFees & sHolidaysExtraFees & sHtmlReloctionFees & sHtmlAllFeeDetails & sHtmlFreeDayDetails
          ltrAllFeeDetails_.Text = ltrAllFeeDetails_.Text & "<tr><td align='right' class='text' colspan='2'>Total Estimate </td><td align='right'> " & sCurrencySymbol & sTotalEstimate & "</td></tr>"
          ltrAllFeeDetails_.Text = ltrAllFeeDetails_.Text & sHtmlTaxDetails
          'ltrMandatoryTotal.Text = "<table><tr><td valign='top'> <u>Mandatory Details</u> </td></tr>" & sHtmlMandatoryFees & "</table>"
          ltrAllFeeDetails_.Text = ltrAllFeeDetails_.Text & "</table>"


          ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
          ' For other fees end
          ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Else
          Session("ErrorMsg") = sErrorMsg
        End If
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'END confirmBookingTotal Area---
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        If Session("ErrorMsg") <> "" Then
          If Session("ErrorMsg") = "RCMReference Key is not valid" Or Session("ErrorMsg") = "RCMReference Key cannot be blank." Then
            'Session("ErrorMsg") = ""
            Response.Redirect("WebStep1.aspx")
          End If
          litErrorMessage.Visible = True
          ErrorDiv.Visible = True
          lblErrmsg.Visible = True
          litErrorMessage.Text = Session("ErrorMsg")
          mainTable1.Visible = False
          pnlAlreadyCustomer.Visible = False
          Session("ErrorMsg") = ""
        Else
          ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
          '------------------- date part -----------------------------------------------------------------------------------
          ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
          sPickupDate = sPickupDate.AddYears(5)

          ''oDateTimecl.PopulateDaysWithZero(1, ddlLicenseExpiryDateDay)
          ''If Numericcl.GetIntValue(Day(sPickupDate)) < 10 Then
          ''  ddlLicenseExpiryDateDay.SelectedValue = Stringcl.AddZeroToString(Day(sPickupDate))
          ''Else
          ''  ddlLicenseExpiryDateDay.SelectedValue = Day(sPickupDate)
          ''End If

          ''oDateTimecl.PopulateMonths(3, ddlLicenseExpiryDateMonth)
          ''If Numericcl.GetIntValue(Month(sPickupDate)) < 10 Then
          ''  ddlLicenseExpiryDateMonth.SelectedValue = Stringcl.AddZeroToString(Month(sPickupDate))
          ''Else
          ''  ddlLicenseExpiryDateMonth.SelectedValue = Month(sPickupDate)
          ''End If

          ''oDateTimecl.PopulateYears(2, ddlLicenseExpiryDateYear)
          ''ddlLicenseExpiryDateYear.SelectedValue = Year(sPickupDate)


          ''sReturnDate = sReturnDate.AddYears(-20)

          ''oDateTimecl.PopulateDaysWithZero(1, ddlPrimaryDriverDOBDay)
          ''If Numericcl.GetIntValue(Day(sReturnDate)) < 10 Then
          ''  ddlPrimaryDriverDOBDay.SelectedValue = Stringcl.AddZeroToString(Day(sReturnDate))
          ''Else
          ''  ddlPrimaryDriverDOBDay.SelectedValue = Day(sReturnDate)
          ''End If

          ''oDateTimecl.PopulateMonths(3, ddlPrimaryDriverDOBMonth)
          ''If Numericcl.GetIntValue(Month(sReturnDate)) < 10 Then
          ''  ddlPrimaryDriverDOBMonth.SelectedValue = Stringcl.AddZeroToString(Month(sReturnDate))
          ''Else
          ''  ddlPrimaryDriverDOBMonth.SelectedValue = Month(sReturnDate)
          ''End If

          ''oDateTimecl.PopulateYears(1, ddlPrimaryDriverDOBYear)
          ''ddlPrimaryDriverDOBYear.SelectedValue = Year(sReturnDate)

          'oDateTimecl.PopulateMonths(1, ddlExpiryDateMonth)
          ''ddlExpiryDateMonth.Items.Insert(0, New ListItem("Month", ""))
          'oDateTimecl.PopulateYears(6, ddlExpiryDateYear)
          ''ddlExpiryDateYear.Items.Insert(0, New ListItem("Year", ""))
          ''Dim itemToRemoveddlExpiryDateYear As ListItem = ddlExpiryDateYear.Items.FindByValue("0")
          ''If ddlExpiryDateYear IsNot Nothing Then
          ''  ddlExpiryDateYear.Items.Remove(itemToRemoveddlExpiryDateYear)
          ''End If

          ''Dim itemToRemoveddlLicenseExpiryDateYear As ListItem = ddlLicenseExpiryDateYear.Items.FindByValue("0")
          ''If itemToRemoveddlLicenseExpiryDateYear IsNot Nothing Then
          ''  ddlLicenseExpiryDateYear.Items.Remove(itemToRemoveddlLicenseExpiryDateYear)
          ''End If

          ''Dim itemToRemoveddlPrimaryDriverDOBYear As ListItem = ddlPrimaryDriverDOBYear.Items.FindByValue("0")
          ''If ddlPrimaryDriverDOBYear IsNot Nothing Then
          ''  ddlPrimaryDriverDOBYear.Items.Remove(itemToRemoveddlPrimaryDriverDOBYear)
          ''End If

          ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
          '------------------- date part Ends ------------------------------------------------------------------------------

          ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

          'RentalSource Area---
          Dim sResponseRentalSource As String = ""
          Dim dsRentalSource As DataSet = WS_RCMClientAPI.requestRentalSource(Session("sReferenceKey"))
          sResponseRentalSource = IOcl.GetXMLasStringFromDataSet(dsRentalSource)
          If dsRentalSource.Tables.Count > 1 Then
            If dsRentalSource.Tables(2).Rows.Count >= 1 Then
              Dim xRentalSourceDocResponse = New XmlDocument()
              ''Load the Xml file
              xRentalSourceDocResponse.LoadXml(sResponseRentalSource)

              'Check the eror Element List
              m_nodelist = xRentalSourceDocResponse.SelectNodes("/CustomerRentalSourceDetails/RentalSourceDetails/Error")
              'Loop through the nodes
              For Each Me.m_node In m_nodelist
                'Get the ErrorCode Element Value
                sErrorMsg = m_node.ChildNodes.Item(0).InnerText
              Next
              m_nodelist = Nothing
              m_node = Nothing
              If sErrorMsg = "" Then
                ddlRentalSourceID.DataTextField = dsRentalSource.Tables(2).Columns("Name").ToString()
                ddlRentalSourceID.DataValueField = dsRentalSource.Tables(2).Columns("ID").ToString()
                ddlRentalSourceID.DataSource = dsRentalSource.Tables(2)
                ddlRentalSourceID.DataBind()
              Else
                Session("ErrorMsg") = sErrorMsg
              End If
            Else
              tr_RentalSource.Visible = False
            End If
          Else
            tr_RentalSource.Visible = False
          End If
          'RentalSource Area Ends ---

          'CustomerCountry Area---
          Dim sResponseCustomerCountry As String = ""
          Dim dsCustomerCountry As DataSet = WS_RCMClientAPI.requestCustomerCountryDetails(Session("sReferenceKey"))
          sResponseCustomerCountry = IOcl.GetXMLasStringFromDataSet(dsCustomerCountry)
          Dim xCustomerCountryDocResponse = New XmlDocument()
          ''Load the Xml file
          xCustomerCountryDocResponse.LoadXml(sResponseCustomerCountry)

          'Check the eror Element List
          m_nodelist = xCustomerCountryDocResponse.SelectNodes("/CustomerCustomerCountryDetails/CustomerCountryDetails/Error")
          'Loop through the nodes
          For Each Me.m_node In m_nodelist
            'Get the ErrorCode Element Value
            sErrorMsg = m_node.ChildNodes.Item(0).InnerText
          Next
          m_nodelist = Nothing
          m_node = Nothing
          ddlCustomerCountryID.DataTextField = dsCustomerCountry.Tables(2).Columns("Name").ToString()
          ddlCustomerCountryID.DataValueField = dsCustomerCountry.Tables(2).Columns("ID").ToString()
          ddlCustomerCountryID.DataSource = dsCustomerCountry.Tables(2)
          ddlCustomerCountryID.DataBind()

          ddlLicenseIssuedCountry.DataTextField = dsCustomerCountry.Tables(2).Columns("Name").ToString()
          ddlLicenseIssuedCountry.DataValueField = dsCustomerCountry.Tables(2).Columns("ID").ToString()
          ddlLicenseIssuedCountry.DataSource = dsCustomerCountry.Tables(2)
          ddlLicenseIssuedCountry.DataBind()
          'ddlLicenseIssuedCountry.SelectedValue = "7"
          'ddlCustomerCountryID.SelectedValue = "7"

          'CustomerCountry Area Ends ---

          ''CustomerCCType Area---
          'Dim sResponseCCType As String = ""
          'Dim dsCCType As DataSet = WS_RCMClientAPI.requestCreditCardType(Session("sReferenceKey"))
          'sResponseCCType = IOcl.GetXMLasStringFromDataSet(dsCCType)
          'Dim xCCTypeDocResponse = New XmlDocument()
          ' ''Load the Xml file
          'xCCTypeDocResponse.LoadXml(sResponseCCType)

          ''Check the eror Element List
          'm_nodelist = xCCTypeDocResponse.SelectNodes("/CreditCardTypeDetails/CreditCardType/Error")
          ''Loop through the nodes
          'For Each Me.m_node In m_nodelist
          '  'Get the ErrorCode Element Value
          '  sErrorMsg = m_node.ChildNodes.Item(0).InnerText
          'Next
          'm_nodelist = Nothing
          'm_node = Nothing
          'If sErrorMsg = "" Then
          '  ddlCCType.DataTextField = dsCCType.Tables(1).Columns("CardType").ToString()
          '  ddlCCType.DataValueField = dsCCType.Tables(1).Columns("ID").ToString()
          '  ddlCCType.DataSource = dsCCType.Tables(1)
          '  ddlCCType.DataBind()
          '  ddlCCType.Items.Insert(0, "Select Card")
          'Else
          '  Session("ErrorMsg") = sErrorMsg
          'End If
          'CustomerCCType Area Ends ---
        End If
      End If
    End If
  End Sub
  'Protected Sub btnRequestBooking_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRequestBooking.Click
  '  Response.Redirect("webstep5.aspx?type=" & sRequestType)
  'End Sub

  Protected Sub btnExistingCustomer_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnExistingCustomer.Click
    litErrorMessage.Visible = False
    ErrorDiv.Visible = False
    lblErrmsg.Visible = False
    litErrorMessage.Text = ""
    Session("ErrorMsg") = ""
    datePrimaryDriverDOB.MinDate = DateTime.Today.Date.AddYears(-90)
    datePrimaryDriverDOB.MaxDate = DateTime.Today.Date
    dateLicenseExpiry.MinDate = DateTime.Today.Date.AddYears(-10)
    Dim sResponseExistingCustomerDetails As String = ""
    Dim dsExistingCustomerDetails As DataSet = WS_RCMClientAPI.confirmExistingCustomerDetails(Stringcl.GetValueTidyUpCode(txtLastName_pnlAlreadyCustomer.Text), txtEmail_pnlAlreadyCustomer.Text, Session("sReferenceKey"))
    sResponseExistingCustomerDetails = IOcl.GetXMLasStringFromDataSet(dsExistingCustomerDetails)
    Dim xExistingCustomerDetailsDocResponse = New XmlDocument()
    ''Load the Xml file
    xExistingCustomerDetailsDocResponse.LoadXml(sResponseExistingCustomerDetails)

    'Check the eror Element List
    m_nodelist = xExistingCustomerDetailsDocResponse.SelectNodes("/ExistingCustomerDetails/CustomerDetails/Error")
    'Loop through the nodes
    For Each m_node In m_nodelist
      'Get the ErrorCode Element Value
      sErrorMsg = m_node.ChildNodes.Item(0).InnerText
    Next
    m_nodelist = Nothing
    m_node = Nothing

    'Check the eror Element List
    '<ExistingCustomerDetails><ErrorDetails><Error>Error:18. Please contact customer care in case you receive this error</Error></ErrorDetails></ExistingCustomerDetails>
    m_nodelist = xExistingCustomerDetailsDocResponse.SelectNodes("/ExistingCustomerDetails/ErrorDetails/Error")
    'Loop through the nodes
    For Each m_node In m_nodelist
      'Get the ErrorCode Element Value
      sErrorMsg = m_node.ChildNodes.Item(0).InnerText
    Next
    m_nodelist = Nothing
    m_node = Nothing
    Session("ErrorMsg") = sErrorMsg
    If sErrorMsg = "" Then
      Dim sPrimaryDriverDOB As String = ""
      Dim sPrimaryDriverDOBArray() As String
      Dim sPrimaryDriverDOBDay As String = ""
      Dim sPrimaryDriverDOBMonth As String = ""
      Dim sPrimaryDriverDOBYear As String = ""
      If xExistingCustomerDetailsDocResponse.SelectSingleNode("ExistingCustomerDetails/CustomerDetails/Details/DOB").InnerText <> "" Then
        sPrimaryDriverDOB = xExistingCustomerDetailsDocResponse.SelectSingleNode("ExistingCustomerDetails/CustomerDetails/Details/DOB").InnerText
        sPrimaryDriverDOBArray = Split(sPrimaryDriverDOB, "/")
        sPrimaryDriverDOBDay = sPrimaryDriverDOBArray(1)
        sPrimaryDriverDOBMonth = sPrimaryDriverDOBArray(0)
        sPrimaryDriverDOBYear = sPrimaryDriverDOBArray(2)
        'sPrimaryDriverDOB = sPrimaryDriverDOBMonth & "/" & sPrimaryDriverDOBDay & "/" & sPrimaryDriverDOBDay
        'dd-MM-yyyy
        'datePrimaryDriverDOB = String.Format("{0:MM/dd/yyyy}", sPrimaryDriverDOB)
        If sPrimaryDriverDOBYear <> "2100" Then
          datePrimaryDriverDOB.DbSelectedDate = sPrimaryDriverDOB
        End If
        'Convert.ToDateTime(sPrimaryDriverDOB, New CultureInfo("en-US"))
        'ddlPrimaryDriverDOBDay.SelectedValue = sPrimaryDriverDOBDay
        'ddlPrimaryDriverDOBMonth.SelectedValue = sPrimaryDriverDOBMonth
        'ddlPrimaryDriverDOBYear.SelectedValue = sPrimaryDriverDOBYear
      End If

      Dim sLicenseExpiryDate As String = ""
      Dim sLicenseExpiryDateArray() As String
      Dim sLicenseExpiryDateDay As String = ""
      Dim sLicenseExpiryDateMonth As String = ""
      Dim sLicenseExpiryDateYear As String = ""
      If xExistingCustomerDetailsDocResponse.SelectSingleNode("ExistingCustomerDetails/CustomerDetails/Details/LicenseExpiryDate").InnerText <> "" Then
        sLicenseExpiryDate = xExistingCustomerDetailsDocResponse.SelectSingleNode("ExistingCustomerDetails/CustomerDetails/Details/LicenseExpiryDate").InnerText
        sLicenseExpiryDateArray = Split(sLicenseExpiryDate, "/")
        sLicenseExpiryDateDay = sLicenseExpiryDateArray(1)
        sLicenseExpiryDateMonth = sLicenseExpiryDateArray(0)
        sLicenseExpiryDateYear = sLicenseExpiryDateArray(2)
        'ddlLicenseExpiryDateDay.SelectedValue = sLicenseExpiryDateDay
        'ddlLicenseExpiryDateMonth.SelectedValue = sLicenseExpiryDateMonth
        'If ddlLicenseExpiryDateYear.Items.FindByValue(sLicenseExpiryDateYear) IsNot Nothing Then
        'ddlLicenseExpiryDateYear.SelectedValue = sLicenseExpiryDateYear
        'End If
        'sLicenseExpiryDate = sLicenseExpiryDateMonth & "/" & sLicenseExpiryDateDay & "/" & sLicenseExpiryDateDay
        If sLicenseExpiryDateYear <> "2100" Then
          dateLicenseExpiry.DbSelectedDate = sLicenseExpiryDate
        End If
      End If

      txtCustomerFirstName.Text = xExistingCustomerDetailsDocResponse.SelectSingleNode("ExistingCustomerDetails/CustomerDetails/Details/FirstName").InnerText
      txtCustomerLastName.Text = xExistingCustomerDetailsDocResponse.SelectSingleNode("ExistingCustomerDetails/CustomerDetails/Details/LastName").InnerText
      'txtPrimaryDriverDOB.Text = xExistingCustomerDetailsDocResponse.SelectSingleNode("ExistingCustomerDetails/CustomerDetails/Details/DOB").InnerText
      txtPrimaryDriverLicenseNo.Text = xExistingCustomerDetailsDocResponse.SelectSingleNode("ExistingCustomerDetails/CustomerDetails/Details/LicenseNo").InnerText
      If xExistingCustomerDetailsDocResponse.SelectSingleNode("ExistingCustomerDetails/CustomerDetails/Details/LicenseIssued").InnerText <> "" And IsNumeric(xExistingCustomerDetailsDocResponse.SelectSingleNode("ExistingCustomerDetails/CustomerDetails/Details/LicenseIssued").InnerText) Then
        'txtLicenseIssuedCountry.Text = xExistingCustomerDetailsDocResponse.SelectSingleNode("ExistingCustomerDetails/CustomerDetails/Details/LicenseIssued").InnerText
        ddlLicenseIssuedCountry.SelectedValue = xExistingCustomerDetailsDocResponse.SelectSingleNode("ExistingCustomerDetails/CustomerDetails/Details/LicenseIssued").InnerText
      End If
      'txtLicenseExpiryDate.Text = xExistingCustomerDetailsDocResponse.SelectSingleNode("ExistingCustomerDetails/CustomerDetails/Details/LicenseExpiryDate").InnerText
      txtCustomerEmailID.Text = xExistingCustomerDetailsDocResponse.SelectSingleNode("ExistingCustomerDetails/CustomerDetails/Details/CustomerEmail").InnerText
      txtAddress.Text = xExistingCustomerDetailsDocResponse.SelectSingleNode("ExistingCustomerDetails/CustomerDetails/Details/Address").InnerText
      txtCity.Text = xExistingCustomerDetailsDocResponse.SelectSingleNode("ExistingCustomerDetails/CustomerDetails/Details/City").InnerText
      lblBState.Text = xExistingCustomerDetailsDocResponse.SelectSingleNode("ExistingCustomerDetails/CustomerDetails/Details/State").InnerText
      txtPostalCode.Text = xExistingCustomerDetailsDocResponse.SelectSingleNode("ExistingCustomerDetails/CustomerDetails/Details/PostCode").InnerText
      txtCustomerPhone.Text = xExistingCustomerDetailsDocResponse.SelectSingleNode("ExistingCustomerDetails/CustomerDetails/Details/Phone").InnerText
      If xExistingCustomerDetailsDocResponse.SelectSingleNode("ExistingCustomerDetails/CustomerDetails/Details/CountryID").InnerText <> "" Then
        ddlCustomerCountryID.SelectedValue = xExistingCustomerDetailsDocResponse.SelectSingleNode("ExistingCustomerDetails/CustomerDetails/Details/CountryID").InnerText
      End If
    Else
      'tr_lblMsg.Visible = True
      'lblMsg.Visible = True
      'lblMsg.Text = sErrorMsg
      If Session("ErrorMsg") <> "" Then
        If Session("ErrorMsg") = "RCMReference Key is not valid" Then
          'Session("ErrorMsg") = ""
          Response.Redirect("WebStep1.aspx")
        End If
        litErrorMessage.Visible = True
        ErrorDiv.Visible = True
        lblErrmsg.Visible = True
        litErrorMessage.Text = Session("ErrorMsg")
        Session("ErrorMsg") = ""
      End If

      ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
      '------------------- date part -----------------------------------------------------------------------------------
      ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
      'If Numericcl.GetIntValue(Day(sPickupDate)) < 10 Then
      '  ddlLicenseExpiryDateDay.SelectedValue = Stringcl.AddZeroToString(Day(sPickupDate))
      'Else
      '  ddlLicenseExpiryDateDay.SelectedValue = Day(sPickupDate)
      'End If
      'If Numericcl.GetIntValue(Month(sPickupDate)) < 10 Then
      '  ddlLicenseExpiryDateMonth.SelectedValue = Stringcl.AddZeroToString(Month(sPickupDate))
      'Else
      '  ddlLicenseExpiryDateMonth.SelectedValue = Month(sPickupDate)
      'End If
      'ddlLicenseExpiryDateYear.SelectedValue = Year(sPickupDate)


      'If Numericcl.GetIntValue(Day(sReturnDate)) < 10 Then
      '  ddlPrimaryDriverDOBDay.SelectedValue = Stringcl.AddZeroToString(Day(sReturnDate))
      'Else
      '  ddlPrimaryDriverDOBDay.SelectedValue = Day(sReturnDate)
      'End If

      'If Numericcl.GetIntValue(Month(sReturnDate)) < 10 Then
      '  ddlPrimaryDriverDOBMonth.SelectedValue = Stringcl.AddZeroToString(Month(sReturnDate))
      'Else
      '  ddlPrimaryDriverDOBMonth.SelectedValue = Month(sReturnDate)
      'End If
      'ddlPrimaryDriverDOBYear.SelectedValue = Year(sReturnDate)

      ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
      '------------------- date part Ends ------------------------------------------------------------------------------

      ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

      txtCustomerFirstName.Text = ""
      txtCustomerLastName.Text = ""
      txtPrimaryDriverLicenseNo.Text = ""
      'txtLicenseIssuedCountry.Text = ""
      'ddlLicenseIssuedCountry.SelectedValue = "7"
      txtCustomerEmailID.Text = ""
      txtAddress.Text = ""
      txtCity.Text = ""
      txtStateProvince.Text = ""
      txtPostalCode.Text = ""
      txtCustomerPhone.Text = ""
      'ddlCustomerCountryID.SelectedValue = "7"
    End If
  End Sub
  Protected Sub Page_PreInit(sender As Object, e As System.EventArgs) Handles Me.PreInit
    Page.Theme = Session("Theme")
  End Sub

  Protected Sub ddlRentalSourceID_ItemDataBound(sender As Object, e As Telerik.Web.UI.RadComboBoxItemEventArgs) Handles ddlRentalSourceID.ItemDataBound
    If e.Item.Text = "Online Booking API" Then
      Dim item As RadComboBoxItem = TryCast(e.Item, RadComboBoxItem)
      item.Visible = False
    End If
  End Sub
End Class
