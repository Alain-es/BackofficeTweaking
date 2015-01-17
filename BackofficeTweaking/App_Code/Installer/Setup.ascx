<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Setup.ascx.cs" Inherits="BackofficeTweaking.Installer.Setup" %>
<%@ Register TagPrefix="umb" Assembly="controls" Namespace="umbraco.uicontrols" %>

<div style="padding: 10px 10px 0;">
    <div style="float: left; position: relative;">
        <img src="/App_Plugins/BackofficeTweaking/Installer/Logo.png" alt="BackofficeTweaking" />
    </div>
    <div style="float: left; position: relative; margin: 10px 30px;">
        <h3>BackofficeTweaking successfully installed!</h3>
    </div>
    <div style="clear: both;"></div>
    <asp:Panel runat="server" ID="pnlFinished" Visible="true">
        <div style="padding: 30px; color: #999;">
            <h4>BackofficeTweaking is ready to use.</h4>
        </div>
    </asp:Panel>
</div>
