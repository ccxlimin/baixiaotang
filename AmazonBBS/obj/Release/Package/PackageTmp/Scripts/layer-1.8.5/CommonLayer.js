
//调用一下方法，需在页面引用
//1、jquery1.8以上版本
//2、<link href="/Scripts/layer/skin/layer.css" rel="stylesheet" type="text/css" />
//3、<script src="/Scripts/layer/layer.min.js" type="text/javascript"></script>

//参数说明：width-宽度  例如：'400px',
//height-高度  例如：'170px',
//title-标题,
//top-离上面的高度  例如：'50px',

/**resize 事件**/
var layerResize = function (layerId, width, win) {
    return function () {
        //重新计算margin-left
        win = win || window;
        var percent = parseFloat(width);
        //console.log(win.innerWidth);
        // var wd=Math.ceil(win.innerWidth*percent/100);
        var wd = Math.ceil(win.$("#xubox_layer" + layerId).width());
        var wh = Math.ceil(win.$("#xubox_layer" + layerId).height());
        var ml = Math.ceil(wd / 2);
        win.$("#xubox_border" + layerId).css({
            "width": wd + 10 + "px",
            "height": wh + 10 + "px"
        });
        win.$("#xubox_iframe" + layerId).css({
            "width": wd + "px",
            "height": wh - 36 + "px"
        });
        // //考虑滚动条
        var offsetX = 4;
        if (win.innerWidth < win.document.body.offsetWidth) {
            offsetX = 0;
        }
        win.$("#xubox_layer" + layerId).css({
            "margin-left": "-" + (ml + offsetX) + "px"
        })
    };
};



var currentClass = "hrBgColor1";
var imgLogo = '<img width="26" height="26" src="/favicon.ico" style="vertical-align:middle"/>&nbsp;';

function AlertMsg(msg, type, conEnd) {
    conEnd = conEnd || function () { };
    var type_p = 1;
    if (type != null && type != undefined && type != "") {
        type_p = type;
    }

    var width_p = '400px';
    var height_p = '170px';
    var conEnd_p = function () { }
    //var top_p = '200px';
    var top_p = "";

    AlertMsgRedirect(width_p, height_p, type_p, msg, conEnd, top_p);
}

function AlertMsgTop(msg, type, Top) {

    var type_p = 1;
    if (type != null && type != undefined && type != "") {
        type_p = type;
    }

    var width_p = '400px';
    var height_p = '170px';
    var conEnd_p = function () { }
    var top_p = Top;

    AlertMsgRedirect(width_p, height_p, type_p, msg, function () { }, top_p);
}

function AlertMsgFn(msg, type, conEnd) {

    var type_p = 1;
    if (type != null && type != undefined && type != "") {
        type_p = type;
    }

    var width_p = '400px';
    var height_p = '170px';
    var conEnd_p = function () {
    }
    var top_p = '200px';

    AlertMsgRedirect(width_p, height_p, type_p, msg, conEnd, top_p);
}

//新老版本转换Icon
function GetMsgType(type) {
    if (type != null && type != undefined) {
        switch (type) {
            case 3: type = 2; break;
            case 4: type = 3; break;
            case 7: type = 4; break;
            case 8: type = 5; break;
        }
    }
    return type;
}

//弹出消息对话框 
function AlertMsgRedirect(width, height, type, msg, conEnd, top) {
    type = GetMsgType(type);

    layer.alert(msg, {
        icon: type,
        title: imgLogo + '友情提示',
        end: conEnd,
        //offset: [top], //注释于 2017-04-13 取消offset可自动进行 垂直和水平 居中
        skin: 'layer-ext-moon' //该皮肤由layer.seaning.com友情扩展。关于皮肤的扩展规则，去这里查阅
    });
}


//弹出消息对话框 
function AlertMsgNoOffset(width, height, type, msg, conEnd) {
    AlertMsgRedirect(width, height, type, msg, conEnd, '');
}

//弹出消息对话框 
function AlertMsgRedirectTitle(width, height, type, msg, conEnd, top, title) {
    type = GetMsgType(type);
    layer.alert(msg, {
        icon: type,
        title: imgLogo + title,
        end: conEnd,
        skin: 'layer-ext-moon' //该皮肤由layer.seaning.com友情扩展。关于皮肤的扩展规则，去这里查阅
    });
}

function AlertMsgRedirectTitle_BtnText(width, height, type, msg, conEnd, top, title, btnText) {
    type = GetMsgType(type);
    layer.alert(msg, {
        icon: type,
        title: imgLogo + title,
        end: conEnd,
        btn: [btnText],
        skin: 'layer-ext-moon' //该皮肤由layer.seaning.com友情扩展。关于皮肤的扩展规则，去这里查阅
    });
}

//父窗体弹出消息框
function AlertMsgRedirect_P(width, height, type, msg, conEnd, top) {
    var topNum = "200px";

    if (top != undefined && top != null && top != "") {
        topNum = top;
    }

    type = GetMsgType(type);
    parent.layer.alert(msg,
        {
            icon: type,
            title: imgLogo + '系统提示',
            end: conEnd,
            skin: 'layer-ext-moon' //该皮肤由layer.seaning.com友情扩展。关于皮肤的扩展规则，去这里查阅
        }
    );
}


//弹出iframe层（带end关闭事件）
function AlertIframe(srcUrl, width, height, top, title, conEnd) {
    var win_height = $(window).height(),
        win_width = $(window).width();
    width = width ? win_width > parseInt(width.replace("px", "")) ? width : win_width * 0.8 + "px" : width;
    height = height ? win_height > parseInt(height.replace("px", "")) ? height : win_height * 0.8 + "px" : height;

    var idIndex = layer.open({
        type: 2,
        title: imgLogo + title,
        maxmin: true,//最大最小化
        shadeClose: false, //点击遮罩关闭层
        area: [width, height],
        offset: [top],
        content: srcUrl,
        end: conEnd,
        //以下为多窗口模式下，选中置顶
        zIndex: layer.zIndex, //重点1
        success: function (layero) {
            layer.setTop(layero); //重点2
        }
    });

    if (width.indexOf("%") > 0) {
        $(window).on("resize", layerResize(idIndex, width));
    }
}

//弹出iframe层--最大化--隐藏最大化最小化按钮（带end关闭事件）
function AlertIframe_Max(srcUrl, title, conEnd) {
    var idIndex = layer.open({
        type: 2,
        title: imgLogo + title,
        maxmin: !1,//最大最小化
        shadeClose: false, //点击遮罩关闭层
        area: ['100%', '100%'],
        content: srcUrl,
        end: conEnd,
        //以下为多窗口模式下，选中置顶
        zIndex: layer.zIndex, //重点1
        success: function (layero) {
            layer.setTop(layero); //重点2
        }
    });
    layer.full(idIndex);
}

//弹出iframe层--最大化--显示最大化最小化按钮（带end关闭事件）
function AlertIframe_Max_WithMaxMin(srcUrl, title, conEnd) {
    var idIndex = layer.open({
        type: 2,
        title: imgLogo + title,
        maxmin: !0,//最大最小化
        shadeClose: false, //点击遮罩关闭层
        area: ['90%', '90%'],
        offset: ['0px'],
        content: srcUrl,
        end: conEnd,
        //以下为多窗口模式下，选中置顶
        zIndex: layer.zIndex, //重点1
        success: function (layero) {
            layer.setTop(layero); //重点2
        }
    });
    layer.full(idIndex);
}

//弹出iframe层（带end关闭事件）
function AlertIframeForWinForm(srcUrl, width, height, top, title, conEnd) {
    var idIndex = layer.open({
        type: 2,
        title: imgLogo + title,
        maxmin: true,//最大最小化
        shadeClose: false, //点击遮罩关闭层
        area: [width, height],
        offset: [top],
        content: "",
        end: conEnd,
        //以下为多窗口模式下，选中置顶
        zIndex: layer.zIndex, //重点1
        success: function (layero) {
            layer.setTop(layero); //重点2
        }
    });
    if (width.indexOf("%") > 0) {
        $(window).on("resize", layerResize(idIndex, width));
    }
    if (srcUrl.indexOf("?") >= 0) {
        srcUrl += "&tta=" + (new Date()).getTime();
    }
    document.getElementById("layui-layer-iframe" + idIndex).src = srcUrl;
}

//弹出iframe层
function AlertIframeTitle(srcUrl, width, height, top, title, conEnd) { AlertIframe(srcUrl, width, height, top, title, conEnd) }

//弹出iframe层（带end关闭事件）
function AlertIframe_Notitle(srcUrl, width, height, top, title, conEnd) { AlertIframe(srcUrl, width, height, top, title, conEnd) }

function AlertIframe_NotitleFix(srcUrl, width, height, top, title, conEnd) { AlertIframe(srcUrl, width, height, top, title, conEnd) }

function AlertIframeMove(srcUrl, width, height, top, title, conEnd) { AlertIframe(srcUrl, width, height, top, title, conEnd) }

//弹出iframe层（带end关闭事件）
function AlertIframeFix(srcUrl, width, height, top, title, conEnd) {
    //AlertIframe(srcUrl, width, height, top, title, conEnd);
    //fixed: false, //不固定

    var win_height = $(window).height(),
        win_width = $(window).width();
    width = width ? win_width > parseInt(width.replace("px", "")) ? width : win_width * 0.8 + "px" : width;
    height = height ? win_height > parseInt(height.replace("px", "")) ? height : win_height * 0.8 + "px" : height;

    var idIndex = layer.open({
        type: 2,
        title: imgLogo + title,
        maxmin: true,//最大最小化
        shadeClose: false, //点击遮罩关闭层
        area: [width, height],
        offset: [top],
        content: srcUrl,
        end: conEnd,
        fixed: false, //不固定
        //以下为多窗口模式下，选中置顶
        zIndex: layer.zIndex, //重点1
        success: function (layero) {
            layer.setTop(layero); //重点2
        }
    });

    if (width.indexOf("%") > 0) {
        $(window).on("resize", layerResize(idIndex, width));
    }
}

//父级弹出不带标题的iframe层
function AlertIframe_Notitle_P(srcUrl, width, height, top, title, conEnd) { parent.AlertIframe(srcUrl, width, height, top, title, conEnd) }

//父级弹出iframe层（带end关闭事件）
function AlertIframeFix_P(srcUrl, width, height, top, title, conEnd) {
    var win_height = $(window).height(),
        win_width = $(window).width();
    width = width ? win_width > parseInt(width.replace("px", "")) ? width : win_width * 0.8 + "px" : width;
    height = height ? win_height > parseInt(height.replace("px", "")) ? height : win_height * 0.8 + "px" : height;

    var idIndex = layer.open({
        type: 2,
        title: imgLogo + title,
        maxmin: true,//最大最小化
        shadeClose: false, //点击遮罩关闭层
        area: [width, height],
        offset: [top],
        content: srcUrl,
        end: conEnd,
        fixed: false, //不固定
        //以下为多窗口模式下，选中置顶
        zIndex: layer.zIndex, //重点1
        success: function (layero) {
            layer.setTop(layero); //重点2
        }
    });

    if (width.indexOf("%") > 0) {
        $(window).on("resize", layerResize(idIndex, width));
    }
}

function AlertIframeNoClose(srcUrl, width, height, top, title, conEnd) { AlertIframe(srcUrl, width, height, top, title, conEnd) }

//弹出iframe层（带end关闭事件）
function AlertIframe_P(srcUrl, width, height, top, title, conEnd) { parent.AlertIframe(srcUrl, width, height, top, title, conEnd) }

//弹出iframe层（带end关闭事件）
function AlertIframe_PP(srcUrl, width, height, top, title, conEnd) { parent.AlertIframeFix_P(srcUrl, width, height, top, title, conEnd) }

//弹出iframe层（无遮罩层）
function AlertIframeNoShade(srcUrl, width, height, top, title, conEnd) {
    var win_height = $(window).height(),
        win_width = $(window).width();
    width = width ? win_width > parseInt(width.replace("px", "")) ? width : win_width * 0.8 + "px" : width;
    height = height ? win_height > parseInt(height.replace("px", "")) ? height : win_height * 0.8 + "px" : height;

    var idIndex = layer.open({
        type: 2,
        title: imgLogo + title,
        maxmin: true,//最大最小化
        shadeClose: false, //点击遮罩关闭层
        shade: false,
        area: [width, height],
        offset: [top],
        content: srcUrl,
        end: conEnd,
        //以下为多窗口模式下，选中置顶
        zIndex: layer.zIndex, //重点1
        success: function (layero) {
            layer.setTop(layero); //重点2
        }
    });
    if (width.indexOf("%") > 0) {
        $(window).on("resize", layerResize(idIndex, width));
    }
    return idIndex;
}

function setSizeWindow() {
    //改变窗口大小，自动适应大小
    var index = parent.layer.getFrameIndex(window.name);
    parent.layer.iframeAuto(index);
}

function ClosePage() {
    if (window.parent.LoadPage) {
        window.parent.LoadPage();
    }
    var index = parent.layer.getFrameIndex(window.name);
    parent.layer.close(index);
}

function AlertConfirmTop(width, height, msg, yesBtnText, noBtnText, conYes, conNo, type, title, top) {

    AlertConfirm(width, height, msg, yesBtnText, noBtnText, conYes, conNo, type, title);
}

function AlertConfirmTop_P(width, height, msg, yesBtnText, noBtnText, conYes, conNo, type, title, top) {

    parent.layer.confirm(msg, {
        icon: 3,
        btn: [yesBtnText, noBtnText] //按钮
    }, conYes, conNo);
}

function AlertConfirm(msg, yesBtnText, noBtnText, conYes, conNo) {

    return layer.confirm(msg, {
        icon: 3,
        btn: [yesBtnText, noBtnText], //按钮
        title: ["确认提示", "height:auto;"]
    }, conYes, conNo);
}

function ChangeDateFormat_P(cellval) {
    if (cellval != null && cellval != undefined && cellval != "") {
        var date = new Date(parseInt(cellval.replace("/Date(", "").replace(")/", ""), 10));
        var month = date.getMonth() + 1 < 10 ? "0" + (date.getMonth() + 1) : date.getMonth() + 1;
        var currentDate = date.getDate() < 10 ? "0" + date.getDate() : date.getDate();
        var hours = date.getHours() < 10 ? "0" + date.getHours() : date.getHours();
        var minutes = date.getMinutes() < 10 ? "0" + date.getMinutes() : date.getMinutes();
        var seconds = date.getSeconds() < 10 ? "0" + date.getSeconds() : date.getSeconds();
        return date.getFullYear() + "-" + month + "-" + currentDate + " " + hours + ":" + minutes + ":" + seconds;
    }
    else
        return "";
}

function ChangeDateFormatReceivableAmount(cellval) {
    if (cellval != null && cellval != undefined && cellval != "") {
        var date = new Date(parseInt(cellval.replace("/Date(", "").replace(")/", ""), 10));
        var month = date.getMonth() + 1 < 10 ? "0" + (date.getMonth() + 1) : date.getMonth() + 1;
        var currentDate = date.getDate() < 10 ? "0" + date.getDate() : date.getDate();
        var hours = date.getHours() < 10 ? "0" + date.getHours() : date.getHours();
        var minutes = date.getMinutes() < 10 ? "0" + date.getMinutes() : date.getMinutes();
        var seconds = date.getSeconds() < 10 ? "0" + date.getSeconds() : date.getSeconds();
        return date.getFullYear() + "/" + month + "/" + currentDate;
    }
    else
        return "";
}

function ChangeDateFormat(cellval) {
    try {
        if (cellval != null && cellval != undefined && cellval != "") {
            var date = new Date(parseInt(cellval.replace("/Date(", "").replace(")/", ""), 10));
            var month = date.getMonth() + 1 < 10 ? "0" + (date.getMonth() + 1) : date.getMonth() + 1;
            var currentDate = date.getDate() < 10 ? "0" + date.getDate() : date.getDate();
            return date.getFullYear() + "-" + month + "-" + currentDate;
        }
        else {
            return "";
        }
    } catch (e) {
        return "";
    }
}

function tbMouseOver(obj) {
    currentClass = $(obj).attr('class');
    $(obj).attr('class', 'hrBgColor2');
}

function tbMouseOut(obj) {
    $(obj).attr('class', currentClass);
}

function strlen(str) {
    var i;
    var len;
    len = 0;
    for (i = 0; i < str.length; i++) {
        if (str.charCodeAt(i) > 255) {
            len += 2;
        }
        else {
            len++;
        }
    }
    return len;
}
function GetStrByLen(str, l) {
    if (str == undefined || str == null) {
        return "";
    }
    if (l == undefined || l == null) {
        return str.toString();
    }
    var tmp = "";
    var len;
    len = 0;
    for (var i = 0; i < str.length; i++) {
        if (str.charCodeAt(i) > 255) {
            len += 2;
        }
        else {
            len++;
        }
        tmp = tmp + "" + str.substring(i, i + 1);
        if (len >= l) {
            return tmp + "...";
        }
    }
    return tmp;

}
//itemCount 记录总数
//pageCount 总页数
//pageIndex 当前页数
function CreatePager(itemCount, pageCount, pageIndex) {

    var prePage = parseInt(pageIndex) - 1;
    if (prePage <= 0) {
        prePage = 1;
    }
    var nextPage = parseInt(pageIndex) + 1;
    if (nextPage > parseInt(pageCount)) {
        nextPage = pageCount;
    }

    var firsturl = "javascript:";
    var preurl = "javascript:";
    var nexturl = "javascript:";
    var lasturl = "javascript:";
    if (pageIndex == 1) {
        firsturl += "void(0);";
        preurl += "void(0);";
        nexturl += "ajaxPageChange(" + nextPage + ");";
        lasturl += "ajaxPageChange(" + pageCount + ");";
    }
    else if (pageIndex == pageCount) {
        firsturl += "ajaxPageChange(1);";
        preurl += "ajaxPageChange(" + prePage + ");";
        nexturl += "void(0);";
        lasturl += "void(0);";
    }
    else {
        firsturl += "ajaxPageChange(1);";
        preurl += "ajaxPageChange(" + prePage + ");";
        nexturl += "ajaxPageChange(" + nextPage + ");";
        lasturl += "ajaxPageChange(" + pageCount + ");";
    }

    if (pageCount <= 1) {
        firsturl = "javascript:void(0)";
        preurl = "javascript:void(0)";
        nexturl = "javascript:void(0)";
        lasturl = "javascript:void(0)";
    }

    var pageTr = "<div class='p3'>共<span style='color:red'>" + itemCount + "</span>条数据&nbsp;&nbsp;&nbsp;&nbsp;第<span id='_pageIndex'>" + pageIndex + "</span>页/" +
        "共<span id=\"pageCount\">" + pageCount
        + "</span>页&nbsp;&nbsp;&nbsp;&nbsp;<a href=\"" + firsturl
        + "\"" + (pageIndex == 1 ? " style='cursor:not-allowed;' " : "") + ">首页</a>&nbsp;<a href=\"" + preurl
        + "\"" + (pageIndex == 1 ? " style='cursor:not-allowed;' " : "") + ">上一页</a>&nbsp;<a href=\"" + nexturl
        + "\"" + (pageIndex == pageCount ? " style='cursor:not-allowed;' " : "") + ">下一页</a>&nbsp;<a href=\"" + lasturl
        + "\"" + (pageIndex == pageCount ? " style='cursor:not-allowed;' " : "") + ">尾页</a>&nbsp;转到&nbsp;<input type=\"text\" id=\"tbCurrentPageIndex\" style='width:20px;' onkeyup=\"value=value.replace(/[^\\d]/g,'')\"  maxlength=\"4\"  value=\"" + pageIndex + "\"/>&nbsp;页<input type=\"button\" id=\"btnGO\" value=\"GO\" onclick='BtnGoPage();'/>" + "</div>";

    return pageTr;
}

function BtnGoPage() {
    var pageIndex = 1;
    if ($("#tbCurrentPageIndex").val() != "") {
        pageIndex = $("#tbCurrentPageIndex").val();
    }
    if (pageIndex > parseInt($("#pageCount").html())) {
        pageIndex = $("#pageCount").html();
    }
    if (pageIndex <= 0) {
        pageIndex = 1;
    }
    ajaxPageChange(pageIndex);
}

//弹出DIV层（无标题栏）
function AlertDivNoTitle(divId, width, height, top, end) {
    var divIndex = layer.open({
        type: 1,
        title: false,
        closeBtn: 1,
        area: [width, height],
        shadeClose: false,//单击遮罩层关闭窗口
        content: $(divId),
        end: end
    });
    return divIndex;
}

//弹出div层
function AlertDiv(divId, width, height, title, top) {
    var divIndex = layer.open({
        type: 1,
        title: imgLogo + title,
        closeBtn: 1,
        area: [width, height],
        shadeClose: false,//单击遮罩层关闭窗口
        content: $(divId)
    });

    return divIndex;
}

//弹出div元素
function AlertDivWithOutID(content, width, height, title, conEnd) {
    var divIndex = layer.open({
        type: 1,
        title: imgLogo + title,
        closeBtn: 1,
        area: [width, height],
        shadeClose: !0,//单击遮罩层关闭窗口
        content: content,
        end: conEnd
    });
    return divIndex;
}

//最大化弹出div层
function AlertDivWithOutID_MAX(content, title, conEnd) {
    var idIndex = layer.open({
        type: 1,
        title: imgLogo + title,
        maxmin: !1,//最大最小化
        shadeClose: false, //点击遮罩关闭层
        area: ['100%', '100%'],
        content: content,
        end: conEnd,
        //以下为多窗口模式下，选中置顶
        //zIndex: layer.zIndex, //重点1
        //success: function (layero) {
        //    layer.setTop(layero); //重点2
        //}
    });
    layer.full(idIndex);
    return idIndex;
}
//弹出div层
function AlertDiv_End(divId, width, height, title, top, conEnd) {
    var divIndex = layer.open({
        type: 1,
        title: imgLogo + title,
        closeBtn: 1,
        area: [width, height],
        shadeClose: false,//单击遮罩层关闭窗口
        end: conEnd,
        content: $(divId)
    });
    return divIndex;
}


//弹出div层(无遮罩)
function AlertDivNoShade(divId, width, height, title, top) {
    var divIndex = layer.open({
        type: 1,
        title: imgLogo + title,
        closeBtn: 1,
        area: [width, height],
        shadeClose: false,//单击遮罩层关闭窗口
        shade: false,
        content: $(divId)
    });
    return divIndex;
}

function AlertDivNoShadeNoFix(divId, width, height, title, top) {
    var divIndex = layer.open({
        type: 1,
        title: imgLogo + title,
        closeBtn: 1,
        area: [width, height],
        shadeClose: false,//单击遮罩层关闭窗口
        shade: false,
        fixed: false,
        content: $(divId),
        offset: top
    });
    return divIndex;
}

//弹出div层无关闭按钮
function AlertDivNoClose(divId, width, height, title, top) {
    var divIndex = layer.open({
        type: 1,
        title: imgLogo + title,
        closeBtn: 0,
        area: [width, height],
        shadeClose: false,//单击遮罩层关闭窗口
        shade: false,
        fixed: false,
        content: $(divId)
    });
    return divIndex;
}

function GetRequest(argname) {
    var url = document.location.href;
    var arrStr = url.substring(url.indexOf("?") + 1).split("&");
    //return arrStr;
    for (var i = 0; i < arrStr.length; i++) {
        var loc = arrStr[i].indexOf(argname + "=");
        if (loc != -1) {
            return arrStr[i].replace(argname + "=", "").replace("?", "");
            break;
        }
    }
    return "";
}

//选择员工
function ShowEmpSelect(city, whereToShow, type) {
    var index = 0;
    if (parseInt(type) > 0) {
        index = parent.AlertIframeNoShade("/Tool/EmpSelectIndex?City=" + city + "&Show=" + whereToShow + "&index=1", "400px" + whereToShow, "400px", "70px", "员工选择", function () { });
    }
    else {
        index = AlertIframeNoShade("/Tool/EmpSelectIndex?City=" + city + "&Show=" + whereToShow, "400px" + whereToShow, "400px", "70px", "员工选择", function () { });
    }
}
//获取当前日期
function getToday() {
    var date_ = new Date();
    var year = date_.getFullYear();
    var month = date_.getMonth() + 1;
    if (month < 10)
        month = "0" + month;
    var day = date_.getDate();
    if (day < 10)
        day = "0" + day;
    return year + "-" + month + "-" + day;
}

function SubStrPhone(phone) {
    if (phone.length > 4) {
        return phone.replace(phone.substr(3, 5), '*****');
    }
    else {
        return phone;
    }
}
function PlayAudio(SoftPhoneIp, userfield, obj) {
    $(obj).css("color", "#0657B3");
    AlertIframeTitle("/Audio/Play?SoftPhoneIp=" + SoftPhoneIp + "&userfield=" + userfield, "400px", "200px", "50px", "录音播放");
}

function PlayAudio_New(SoftPhoneIp, userfield, obj, cityCode) {
    $(obj).addClass("PlayAudioColor");
    //AlertIframeTitle("/Audio/Play?SoftPhoneIp=" + SoftPhoneIp + "&userfield=" + userfield + "&city=" + cityCode, "400px", "200px", "50px", "录音播放");
    AlertIframeForWinForm("/Audio/Play?SoftPhoneIp=" + SoftPhoneIp + "&userfield=" + userfield + "&city=" + cityCode, "400px", "200px", "50px", "录音播放");
}

function getQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return unescape(r[2]); return null;
}

function AlertCanUseQueryTxt(CanUseQuery) {
    AlertMsgRedirect('520px', '180px', 8, '非常抱歉，为了保证系统正常运行，请在' + CanUseQuery + '后再试，谢谢您的配合。<br/>此次限制最终解释权归信息中心所有！', function () { }, '200px');
}

/**弹出带双按钮的可操作界面的弹窗 */
function AlertActionAreaWithConfirm($content, title, yesBtn, noBtn, yesFunction, noFunction) {
    return layer.open({
        type: 1 //此处以iframe举例
        , title: title
        , area: ['auto', 'auto']
        , shade: 0.3
        , maxmin: false
        //, offset: [ //为了演示，随机坐标
        //    Math.random() * ($(window).height() - 300)
        //    , Math.random() * ($(window).width() - 390)
        //]
        , content: $content
        , btn: [yesBtn || "确定", noBtn || '取消'] //只是为了演示
        , yes: function () {
            yesFunction && yesFunction()
        }
        , end: function () {
            noFunction && noFunction();
        }
        , resize: !1,
        //, zIndex: layer.zIndex //重点1
        //, success: function (layero) {
        //    layer.setTop(layero); //重点2
        //}
    });
}

/**弹出带双按钮的可操作界面的弹窗 */
function AlertActionAreaWithConfirmWithSize($content, title, width, height, yesBtn, noBtn, yesFunction, noFunction) {
    return layer.open({
        type: 1 //此处以iframe举例
        , title: title
        , area: [width || 'auto', height || 'auto']
        , shade: 0.3
        , maxmin: false
        //, offset: [ //为了演示，随机坐标
        //    Math.random() * ($(window).height() - 300)
        //    , Math.random() * ($(window).width() - 390)
        //]
        , content: $content
        , btn: [yesBtn || "确定", noBtn || '取消'] //只是为了演示
        , yes: function () {
            yesFunction && yesFunction()
        },
        end: function () {
            noFunction && noFunction();
        }
        , resize: !1,
        //, zIndex: layer.zIndex //重点1
        //, success: function (layero) {
        //    layer.setTop(layero); //重点2
        //}
    });
}

/**弹出Prompt 一行文本 */
function AlertPromptText(title, defaultValue, yesBtnText, noBtnText, callback, maxLength) {
    return layer.prompt({
        value: defaultValue || '',
        title: title,
        formType: 0,
        //shadeClose: !0,
        btn: [yesBtnText || '确定', noBtnText || '取消'],
        maxlength: maxLength || 500,
    }, function (value, index, ele) {
        callback && callback(value, index, ele);
    });
}

/**弹出Prompt 密码框 */
function AlertPromptPassword(title, defaultValue, yesBtnText, noBtnText, callback, maxLength) {
    return layer.prompt({
        value: defaultValue || '',
        title: title,
        formType: 1,
        //shadeClose: !0,
        btn: [yesBtnText || '确定', noBtnText || '取消'],
        maxlength: maxLength || 500,
    }, function (value, index, ele) {
        callback && callback(value, index, ele);
    });
}

/**弹出Prompt 多行文本 */
function AlertPromptTextarea(title, defaultValue, yesBtnText, noBtnText, callback, maxLength) {
    return layer.prompt({
        value: defaultValue || '',
        title: title,
        formType: 2,
        //shadeClose: !0,
        btn: [yesBtnText || '确定', noBtnText || '取消'],
        maxlength: maxLength || 500,
    }, function (value, index, ele) {
        callback && callback(value, index, ele);
    });
}

/**
2  ** 加法函数，用来得到精确的加法结果
3  ** 说明：javascript的加法结果会有误差，在两个浮点数相加的时候会比较明显。这个函数返回较为精确的加法结果。
4  ** 调用：accAdd(arg1,arg2)
5  ** 返回值：arg1加上arg2的精确结果
6  **/
function accAdd(arg1, arg2) {
    var r1, r2, m, c;
    try {
        r1 = arg1.toString().split(".")[1].length;
    }
    catch (e) {
        r1 = 0;
    }
    try {
        r2 = arg2.toString().split(".")[1].length;
    }
    catch (e) {
        r2 = 0;
    }
    c = Math.abs(r1 - r2);
    m = Math.pow(10, Math.max(r1, r2));
    if (c > 0) {
        var cm = Math.pow(10, c);
        if (r1 > r2) {
            arg1 = Number(arg1.toString().replace(".", ""));
            arg2 = Number(arg2.toString().replace(".", "")) * cm;
        } else {
            arg1 = Number(arg1.toString().replace(".", "")) * cm;
            arg2 = Number(arg2.toString().replace(".", ""));
        }
    } else {
        arg1 = Number(arg1.toString().replace(".", ""));
        arg2 = Number(arg2.toString().replace(".", ""));
    }
    return (arg1 + arg2) / m;
}

/**
 ** 减法函数，用来得到精确的减法结果
 ** 说明：javascript的减法结果会有误差，在两个浮点数相减的时候会比较明显。这个函数返回较为精确的减法结果。
 ** 调用：accSub(arg1,arg2)
 ** 返回值：arg1加上arg2的精确结果
 **/
function accSub(arg1, arg2) {
    var r1, r2, m, n;
    try {
        r1 = arg1.toString().split(".")[1].length;
    }
    catch (e) {
        r1 = 0;
    }
    try {
        r2 = arg2.toString().split(".")[1].length;
    }
    catch (e) {
        r2 = 0;
    }
    m = Math.pow(10, Math.max(r1, r2)); //last modify by deeka //动态控制精度长度
    n = (r1 >= r2) ? r1 : r2;
    return ((arg1 * m - arg2 * m) / m).toFixed(n);
}

/**
 ** 乘法函数，用来得到精确的乘法结果
 ** 说明：javascript的乘法结果会有误差，在两个浮点数相乘的时候会比较明显。这个函数返回较为精确的乘法结果。
 ** 调用：accMul(arg1,arg2)
 ** 返回值：arg1乘以 arg2的精确结果
 **/
function accMul(arg1, arg2) {
    var m = 0, s1 = arg1.toString(), s2 = arg2.toString();
    try {
        m += s1.split(".")[1].length;
    }
    catch (e) {
    }
    try {
        m += s2.split(".")[1].length;
    }
    catch (e) {
    }
    return Number(s1.replace(".", "")) * Number(s2.replace(".", "")) / Math.pow(10, m);
}
/** 
 ** 除法函数，用来得到精确的除法结果
 ** 说明：javascript的除法结果会有误差，在两个浮点数相除的时候会比较明显。这个函数返回较为精确的除法结果。
 ** 调用：accDiv(arg1,arg2)
 ** 返回值：arg1除以arg2的精确结果
 **/
function accDiv(arg1, arg2) {
    var t1 = 0, t2 = 0, r1, r2;
    try {
        t1 = arg1.toString().split(".")[1].length;
    }
    catch (e) {
    }
    try {
        t2 = arg2.toString().split(".")[1].length;
    }
    catch (e) {
    }
    with (Math) {
        r1 = Number(arg1.toString().replace(".", ""));
        r2 = Number(arg2.toString().replace(".", ""));
        return (r1 / r2) * pow(10, t2 - t1);
    }
}

//itemCount 记录总数
//pageCount 总页数
//pageIndex 当前页数
function addPaging(containerId, funcName, itemCount, pageCount, pageIndex) {
    $("#" + containerId).empty();
    var firsturl = "javascript:void(0);";
    var preurl = "javascript:void(0);";
    var nexturl = "javascript:void(0);";
    var lasturl = "javascript:void(0);";
    if (parseInt(pageIndex) > 1) {
        firsturl = "javascript:" + funcName + "(1);";
        preurl = "javascript:" + funcName + "(" + (parseInt(pageIndex) - 1) + ");";
    }
    if (parseInt(pageIndex) < pageCount) {
        nexturl = "javascript:" + funcName + "(" + (parseInt(pageIndex) + 1) + ");";
        lasturl = "javascript:" + funcName + "(" + pageCount + ");";
    }

    var pageTr = "共<span style='color:red'>" + itemCount + "</span>条数据&nbsp;&nbsp;&nbsp;&nbsp;"
        + "第<span>" + pageIndex + "</span>页/共<span id=\"pageCount\">" + pageCount + "</span>页&nbsp;&nbsp;&nbsp;&nbsp;"
        + "<a href=\"" + firsturl + "\">首页</a>&nbsp;"
        + "<a href=\"" + preurl + "\">上一页</a>&nbsp;"
        + "<a href=\"" + nexturl + "\">下一页</a>&nbsp;"
        + "<a href=\"" + lasturl + "\">尾页</a>&nbsp;"
        + "转到&nbsp;<input type=\"text\" id=\"targetIndex_" + funcName + "\" style='width:20px;' onkeyup=\"var reg=/^[\\d]+$/g; if(value.match(reg)==null || parseInt(value)>" + pageCount + "){value='';}\"} maxlength=\"4\"  value=\"" + pageIndex + "\"/>&nbsp;页"
        + "<input type=\"button\" value=\"GO\" onclick='" + funcName + "($(\"#targetIndex_" + funcName + "\").val())'/>";
    $("#" + containerId).append(pageTr);
}

//重写layer.load
layer.load = function (msg) { return layer.msg(msg || "正在加载数据，请稍候…", { icon: 16, shade: 0.5, time: 0 }) }