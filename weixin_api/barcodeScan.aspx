<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="barcodeScan.aspx.cs" Inherits="WechatSDK.barcodeScan" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml" lang="en">
<head runat="server">
    <script src ="https://res.wx.qq.com/open/js/jweixin-1.0.0.js" type="text/javascript" ></script>
    <script src="Scripts/jquery-1.4.1.js" type="text/javascript"></script>
    <script src="Scripts/qrcode.js"></script>
    <meta charset="UTF-8"/>
    <meta name="viewport" content="width=device-width,initial-scale=1,user-scalable=0"/>
    <link href="weui.css" rel="stylesheet" />
    <title>轮胎条码扫描 demo</title>
</head>
    <body>

    <form id="form1" runat="server">
      <div class="wrapper">
 
          <input id="barcodeText" type="text" maxlength="256"/>
          <a href="#" class="weui_btn weui_btn_primary" id="scanQRCode">点击扫条码</a>
      </div>
      <div id="divWechat" class="easyui-dialog" style="width:550px;height:500px;padding:10px 20px">

        <div class="container">
            <div id="imageContainer">
            </div>
    	</div>  
     </div>

    <script>
            var appId = '<%=appId %>'
               , nonceStr = '<%=nonceStr %>'
                , signature = '<%=signature %>'
                , timestamp = '<%=timestamp %>'
                , access_token = '<%=access_token %>';
            wx.config({
                debug: false, // 开启调试模式,调用的所有api的返回值会在客户端alert出来，若要查看传入的参数，可以在pc端打开，参数信息会通过log打出，仅在pc端时才会打印。
                appId: appId, // 必填，公众号的唯一标识
                timestamp: timestamp, // 必填，生成签名的时间戳
                nonceStr: nonceStr, // 必填，生成签名的随机串
                signature: signature, // 必填，签名，见附录1
                jsApiList: ['scanQRCode'] // 必填，需要使用的JS接口列表，所有JS接口列表见附录2
            });

            wx.ready(function () {
                document.querySelector('#scanQRCode').onclick = function () {
                    wx.scanQRCode({
                        needResult: 1,
                        desc: 'scanQRCode desc',
                        scanType:["qrCode","barCode"],
                        success: function (res) {
                            answer = res.resultStr;
                            document.getElementById("barcodeText").value = answer;
                            generate_qrcode_local(answer);
                        }
                    });
                }

                function generate_qrcode_local(answer) {
                    var qrcode = new QRCode(document.getElementById("imageContainer"), {
                        text: "http://www.giti.com/QR?" + answer,
                        width: 256,
                        height: 256,
                        colorDark: "#000000",
                        colorLight: "#ffffff",
                        correctLevel: QRCode.CorrectLevel.H
                    });

                }

                function generate_qrcode_webmethod(answer) {

                    var options = JSON.stringify({ expire_seconds: 1800, action_name: "QR_SCENE", action_info: { "scene": { scene_str: answer } } });
                    console.log(options);

                    $.ajax({
                        type: 'POST',
                        url: 'barcodeScan.aspx/QRcode_url',
                        contentType: 'application/json; charset=utf-8',
                        data: "{}",
                        datatype: 'json',
                        success: function (response) {
                            console.log(response);
                            $("#imageContainer").html("<img style='width: 100%' src='" + response + "'>");
                        },
                        error: function (error) {
                            console.log(error);
                        }
                    });
                }

                function generate_qrcode_http(answer) {
                    
                    xhr = new XMLHttpRequest();
                    var url = "https://api.wechat.com/cgi-bin/qrcode/create?access_token=" + access_token;
                    xhr.open("POST", url, true);
                    //xhr.setRequestHeader("Content-type", "application/json");
                    xhr.onreadystatechange = function () {
                        if (xhr.readyState == 4 && xhr.status == 200) {
                            var json = JSON.parse(xhr.responseText);
                            $("#imageContainer").html("<img style='width: 100%' src='"+json.url+"'>");                         
                        }
                    }
                    var data = JSON.stringify({"expire_seconds": 1800, "action_name": "QR_SCENE", "action_info": {"scene": {"scene_str": answer}}});
                    xhr.send(data);
                }
           })
     </script>

    </form>
</body>
</html>
